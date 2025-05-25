using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.ComponentModel;

namespace WAGIC
{
    /// <summary>
    /// Interaction logic for result_windows.xaml
    /// </summary>
    public partial class result_windows : Window
    {
        private readonly HttpClient httpClient;
        private const string API_BASE_URL = "http://localhost:5000";
        private string currentImagePath;
        private double currentZoom = 1.0;
        private const double ZoomStep = 0.2;
        private const double MinZoom = 0.1;
        private const double MaxZoom = 5.0;

        public result_windows()
        {
            InitializeComponent();
            httpClient = new HttpClient();
            LoadLatestResultImage();
        }

        public result_windows(string imagePath) : this()
        {
            currentImagePath = imagePath;
            LoadSpecificImage(imagePath);
        }

        private async void LoadLatestResultImage()
        {
            try
            {
                txtStatus.Text = "🔍 Đang tìm kết quả mới nhất...";
                
                var imageBytes = await GetLatestResultImage();
                
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    LoadImageFromBytes(imageBytes);
                    txtStatus.Text = $"✅ Đã tải kết quả thành công ({DateTime.Now:HH:mm:ss})";
                }
                else
                {
                    txtStatus.Text = "❌ Không tìm thấy kết quả. Vui lòng chạy evaluation trước.";
                }
            }
            catch (Exception ex)
            {
                txtStatus.Text = $"❌ Lỗi: {ex.Message}";
                MessageBox.Show($"Lỗi khi tải kết quả: {ex.Message}", "Lỗi", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSpecificImage(string imagePath)
        {
            try
            {
                if (File.Exists(imagePath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.EndInit();
                    
                    imgResult.Source = bitmap;
                    txtStatus.Text = $"✅ Đã tải: {Path.GetFileName(imagePath)}";
                    ResetZoom();
                }
                else
                {
                    txtStatus.Text = "❌ File không tồn tại!";
                }
            }
            catch (Exception ex)
            {
                txtStatus.Text = $"❌ Lỗi: {ex.Message}";
            }
        }

        private async Task<byte[]> GetLatestResultImage()
        {
            try
            {
                var response = await httpClient.GetAsync($"{API_BASE_URL}/api/latest-result-image");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private void LoadImageFromBytes(byte[] imageBytes)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = new MemoryStream(imageBytes);
                bitmap.EndInit();
                
                imgResult.Source = bitmap;
                ResetZoom();
                
                // Save temp file để có thể save as
                var tempPath = Path.Combine(Path.GetTempPath(), $"magic_result_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                File.WriteAllBytes(tempPath, imageBytes);
                currentImagePath = tempPath;
            }
            catch (Exception ex)
            {
                txtStatus.Text = $"❌ Lỗi hiển thị: {ex.Message}";
            }
        }

        // Zoom functionality
        private void ApplyZoom()
        {
            if (imgResult.Source != null)
            {
                var transform = new ScaleTransform(currentZoom, currentZoom);
                imgResult.RenderTransform = transform;
                txtZoomLevel.Text = $"{(int)(currentZoom * 100)}%";
            }
        }

        private void ResetZoom()
        {
            currentZoom = 1.0;
            ApplyZoom();
        }

        private void BtnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (currentZoom < MaxZoom)
            {
                currentZoom += ZoomStep;
                ApplyZoom();
            }
        }

        private void BtnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (currentZoom > MinZoom)
            {
                currentZoom -= ZoomStep;
                ApplyZoom();
            }
        }

        private void BtnZoomReset_Click(object sender, RoutedEventArgs e)
        {
            ResetZoom();
        }

        private void ImgResult_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Delta > 0 && currentZoom < MaxZoom)
                {
                    currentZoom += ZoomStep;
                    ApplyZoom();
                }
                else if (e.Delta < 0 && currentZoom > MinZoom)
                {
                    currentZoom -= ZoomStep;
                    ApplyZoom();
                }
                e.Handled = true;
            }
        }

        private void BtnFitToWindow_Click(object sender, RoutedEventArgs e)
        {
            if (imgResult.Source != null)
            {
                var imageSource = imgResult.Source as BitmapSource;
                if (imageSource != null)
                {
                    var containerWidth = imageScrollViewer.ActualWidth - 20; // Margin
                    var containerHeight = imageScrollViewer.ActualHeight - 20;
                    
                    var scaleX = containerWidth / imageSource.PixelWidth;
                    var scaleY = containerHeight / imageSource.PixelHeight;
                    
                    currentZoom = Math.Min(scaleX, scaleY);
                    ApplyZoom();
                }
            }
        }

        private void BtnActualSize_Click(object sender, RoutedEventArgs e)
        {
            currentZoom = 1.0;
            ApplyZoom();
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadLatestResultImage();
        }

        private void BtnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentImagePath) || !File.Exists(currentImagePath))
            {
                MessageBox.Show("Không có hình để lưu!", "Cảnh báo", 
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png|JPEG Image|*.jpg|All Files|*.*",
                DefaultExt = "png",
                FileName = $"magic_results_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    File.Copy(currentImagePath, saveDialog.FileName, true);
                    MessageBox.Show($"Đã lưu thành công!\n{saveDialog.FileName}", "Thành công", 
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", 
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            httpClient?.Dispose();
            
            // Cleanup temp file
            if (!string.IsNullOrEmpty(currentImagePath) && 
                currentImagePath.Contains(Path.GetTempPath()) && 
                File.Exists(currentImagePath))
            {
                try
                {
                    File.Delete(currentImagePath);
                }
                catch { }
            }
            
            base.OnClosing(e);
        }
    }
}
