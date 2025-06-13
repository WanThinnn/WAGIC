using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace MagicWPF
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient httpClient;
        private const string API_BASE_URL = "http://localhost:5000";
        private DispatcherTimer statusTimer;
        
        // Thêm flag để tránh mở result window nhiều lần
        private bool resultWindowOpened = false;
        private bool evaluationCompleted = false;

        public MainWindow()
        {
            InitializeComponent();
            httpClient = new HttpClient();

            // Setup timer để check status định kỳ
            statusTimer = new DispatcherTimer();
            statusTimer.Interval = TimeSpan.FromSeconds(2);
            statusTimer.Tick += StatusTimer_Tick;

            // Khởi tạo danh sách datasets mặc định
            InitializeDefaultDatasets();
            LoadDatasets();
            LoadModels();
            
            // Disable visualize button ban đầu
            btnVisualize.IsEnabled = false;
            
            LogMessage("✅ Ứng dụng khởi động thành công. API: " + API_BASE_URL);
        }
        private void InitializeDefaultDatasets()
        {
            var defaultDatasets = new List<string>
        {
            "cadets",
            "fivedirections",
            "streamspot",
            "theia",
            "trace",
            "wget"
        };

            cmbDatasets.ItemsSource = defaultDatasets;
            if (defaultDatasets.Count > 0)
                cmbDatasets.SelectedIndex = 0;
        }

        private async void StatusTimer_Tick(object sender, EventArgs e)
        {
            await CheckTrainingStatus();
            await CheckEvaluationStatus();
        }

        private async void LoadDatasets()
        {
            try
            {
                var response = await httpClient.GetAsync($"{API_BASE_URL}/api/datasets");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<List<string>>>(content);

                    if (result.success)
                    {
                        cmbDatasets.ItemsSource = result.data;
                        if (result.data.Count > 0)
                            cmbDatasets.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Lỗi load datasets: {ex.Message}");
            }
        }

        private async void LoadModels()
        {
            try
            {
                var response = await httpClient.GetAsync($"{API_BASE_URL}/api/models");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<List<ModelInfo>>>(content);

                    if (result.success)
                    {
                        cmbModels.ItemsSource = result.data;
                        if (result.data.Count > 0)
                            cmbModels.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Lỗi load models: {ex.Message}");
            }
        }

        private async void BtnStartTraining_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDatasets.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn dataset!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                btnStartTraining.IsEnabled = false;
                btnStopTraining.IsEnabled = true;

                var dataset = cmbDatasets.SelectedItem.ToString();
                var requestData = new { dataset = dataset };
                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                LogMessage($"Bắt đầu training cho dataset: {dataset}");

                var response = await httpClient.PostAsync($"{API_BASE_URL}/api/train", content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResponse<object>>(responseText);
                    if (result.success)
                    {
                        LogMessage($"{result.message}");
                        statusTimer.Start(); // Bắt đầu check status
                    }
                    else
                    {
                        LogMessage($"{result.error}");
                        ResetTrainingUI();
                    }
                }
                else
                {
                    LogMessage($"HTTP Error: {response.StatusCode}");
                    ResetTrainingUI();
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Exception: {ex.Message}");
                ResetTrainingUI();
            }
        }

        private async void BtnStartEvaluation_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDatasets.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn dataset!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Reset flags khi bắt đầu evaluation mới
                evaluationCompleted = false;
                resultWindowOpened = false;
                
                btnStartEvaluation.IsEnabled = false;
                btnStopEvaluation.IsEnabled = true;

                var dataset = cmbDatasets.SelectedItem.ToString();
                var requestData = new { dataset = dataset };
                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                LogMessage($"🔍 Bắt đầu evaluation cho dataset: {dataset}");

                var response = await httpClient.PostAsync($"{API_BASE_URL}/api/eval", content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResponse<object>>(responseText);
                    if (result.success)
                    {
                        LogMessage($"✅ {result.message}");
                        statusTimer.Start(); // Bắt đầu check status
                    }
                    else
                    {
                        LogMessage($"❌ {result.error}");
                        ResetEvaluationUI();
                    }
                }
                else
                {
                    LogMessage($"❌ HTTP Error: {response.StatusCode}");
                    ResetEvaluationUI();
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Exception: {ex.Message}");
                ResetEvaluationUI();
            }
        }

        private async Task CheckTrainingStatus()
        {
            try
            {
                var response = await httpClient.GetAsync($"{API_BASE_URL}/api/train/status");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<TrainingStatus>>(content);

                    if (result.success)
                    {
                        var status = result.data;
                        progressTraining.Value = status.progress;
                        txtTrainingStatus.Text = status.message;

                        if (!status.is_training)
                        {
                            ResetTrainingUI();
                            if (status.progress == 100)
                            {
                                LogMessage("Training hoàn thành!");
                                LoadModels(); // Refresh models list
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Lỗi check training status: {ex.Message}");
            }
        }

        private async Task CheckEvaluationStatus()
        {
            try
            {
                var response = await httpClient.GetAsync($"{API_BASE_URL}/api/eval/status");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<EvaluationStatus>>(content);

                    if (result.success)
                    {
                        var status = result.data;
                        progressEvaluation.Value = status.progress;
                        txtEvaluationStatus.Text = status.message;

                        if (status.result != null)
                        {
                            var resultText = FormatEvaluationResult(status.result);
                            txtEvaluationResult.Text = resultText;
                            btnVisualize.IsEnabled = true;
                        }

                        if (!status.is_evaluating)
                        {
                            ResetEvaluationUI();
                            
                            // Chỉ xử lý completion một lần
                            if (status.progress == 100 && !evaluationCompleted)
                            {
                                evaluationCompleted = true; // Đánh dấu đã hoàn thành
                                statusTimer.Stop(); // Dừng timer
                                
                                LogMessage("🎉 Evaluation hoàn thành!");
                                if (status.result != null)
                                {
                                    LogMessage($"📊 Kết quả chi tiết:\n{FormatEvaluationResult(status.result)}");
                                    
                                    if (status.message.Contains("visualization hoàn thành"))
                                    {
                                        LogMessage("📈 Biểu đồ visualization đã được tạo tự động!");
                                        
                                        // Tự động mở result window chỉ một lần
                                        if (!resultWindowOpened)
                                        {
                                            await Task.Delay(2000);
                                            OpenResultWindow();
                                        }
                                    }
                                    else
                                    {
                                        LogMessage("💡 Bạn có thể nhấn 'Tạo Biểu đồ' để xem visualization!");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Lỗi check evaluation status: {ex.Message}");
            }
        }

        private string FormatEvaluationResult(EvaluationResult result)
        {
            if (result == null) return "";
            
            var formatted = new StringBuilder();
            
            if (!string.IsNullOrEmpty(result.auc))
                formatted.AppendLine($"AUC: {result.auc}");
            if (!string.IsNullOrEmpty(result.f1))
                formatted.AppendLine($"F1: {result.f1}");
            if (!string.IsNullOrEmpty(result.precision))
                formatted.AppendLine($"Precision: {result.precision}");
            if (!string.IsNullOrEmpty(result.recall))
                formatted.AppendLine($"Recall: {result.recall}");
            
            if (!string.IsNullOrEmpty(result.tn) || !string.IsNullOrEmpty(result.tp) || 
                !string.IsNullOrEmpty(result.fn) || !string.IsNullOrEmpty(result.fp))
            {
                formatted.AppendLine("Confusion Matrix:");
                if (!string.IsNullOrEmpty(result.tn))
                    formatted.AppendLine($"   TN: {result.tn}");
                if (!string.IsNullOrEmpty(result.fn))
                    formatted.AppendLine($"   FN: {result.fn}");
                if (!string.IsNullOrEmpty(result.tp))
                    formatted.AppendLine($"   TP: {result.tp}");
                if (!string.IsNullOrEmpty(result.fp))
                    formatted.AppendLine($"   FP: {result.fp}");
            }
            
            if (!string.IsNullOrEmpty(result.test_auc))
                formatted.AppendLine($"{result.test_auc}");
            
            return formatted.ToString().Trim();
        }

        private void ResetTrainingUI()
        {
            btnStartTraining.IsEnabled = true;
            btnStopTraining.IsEnabled = false;
        }

        private void OpenResultWindow()
        {
            try
            {
                if (!resultWindowOpened)
                {
                    resultWindowOpened = true; // Đánh dấu đã mở
                    var resultWindow = new WAGIC.result_windows();
                    
                    // Reset flag khi window đóng
                    resultWindow.Closed += (s, e) => resultWindowOpened = false;
                    
                    resultWindow.Show();
                    LogMessage("📊 Đã mở cửa sổ kết quả visualization!");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Lỗi mở cửa sổ kết quả: {ex.Message}");
            }
        }

        private async void BtnVisualize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnVisualize.IsEnabled = false;
                LogMessage("📊 Đang tạo biểu đồ visualization...");

                var response = await httpClient.PostAsync($"{API_BASE_URL}/api/visualize", null);
                var responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResponse<object>>(responseText);
                    if (result.success)
                    {
                        LogMessage("✅ " + result.message);
                        
                        // Tự động mở result window chỉ khi chưa mở
                        await Task.Delay(1000);
                        OpenResultWindow();
                    }
                    else
                    {
                        LogMessage("❌ " + result.error);
                        MessageBox.Show("Lỗi: " + result.error, "Lỗi", 
                                       MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    LogMessage($"❌ HTTP Error: {response.StatusCode}");
                    MessageBox.Show($"Lỗi HTTP: {response.StatusCode}", "Lỗi", 
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Exception: {ex.Message}");
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnVisualize.IsEnabled = true;
            }
        }

        private void ResetEvaluationUI()
        {
            btnStartEvaluation.IsEnabled = true;
            btnStopEvaluation.IsEnabled = false;
            // Không reset các flags ở đây vì có thể còn cần dùng
        }

        private void BtnRefreshDatasets_Click(object sender, RoutedEventArgs e)
        {
            LoadDatasets();
            LogMessage("Đã refresh danh sách datasets");
        }

        private void BtnRefreshModels_Click(object sender, RoutedEventArgs e)
        {
            LoadModels();
            LogMessage("Đã refresh danh sách models");
        }

        private void LogMessage(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            txtLog.Text += $"[{timestamp}] {message}\n";
            txtLog.ScrollToEnd();
        }

        protected override void OnClosed(EventArgs e)
        {
            statusTimer?.Stop();
            httpClient?.Dispose();
            base.OnClosed(e);
        }

        private void cmbDatasets_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmbDatasets.SelectedItem != null)
            {
                var selectedDataset = cmbDatasets.SelectedItem.ToString();
                LogMessage($"Đã chọn dataset: {selectedDataset}");
            }
        }

        private void btnStopEvaluation_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    // Data Models
    public class ApiResponse<T>
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string error { get; set; }
        public T data { get; set; }
    }

    public class TrainingStatus
    {
        public bool is_training { get; set; }
        public int progress { get; set; }
        public string message { get; set; }
        public string start_time { get; set; }
        public string dataset { get; set; }
    }

    public class EvaluationStatus
    {
        public bool is_evaluating { get; set; }
        public int progress { get; set; }
        public string message { get; set; }
        public EvaluationResult result { get; set; }
        public string dataset { get; set; }
    }

    public class ModelInfo
    {
        public string dataset { get; set; }
        public long file_size { get; set; }
        public string modified_time { get; set; }
        public string file_path { get; set; }
    }

    public class EvaluationResult
    {
        public string auc { get; set; }
        public string f1 { get; set; }
        public string precision { get; set; }
        public string recall { get; set; }
        public string tn { get; set; }
        public string fn { get; set; }
        public string tp { get; set; }
        public string fp { get; set; }
        public string test_auc { get; set; }
    }
}