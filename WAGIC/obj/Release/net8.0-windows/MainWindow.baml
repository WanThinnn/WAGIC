   M S B A M L   `   `   `  ���� 	  WAGIC    MagicWPF.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$,  &MAGIC - ML Model Training & Evaluation��$	��700��$	��900�� � WindowStartupLocation$ CenterScreen=�.��� $��20q� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $��* �.+��� $��0��$&�� 🧠 MAGIC - APT Detection Model��$��28(�$
��Bold"�$��Center=�$��0,0,0,20q����#2E86AB�� $��1��$��📊 Dataset Selection��$
��0,10q�.���� .+���  �� Orientation$ 
Horizontal=�$��10q�.+��� $��Chọn Dataset:��$	��120��$��Center=��� -    ��Name$ cmbDatasets��$	��200��$��30�� �� DisplayMemberPath$ .�� �� SelectedValuePath$ .���� -    ��Name$ btnRefreshDatasets��$��🔄 Refresh��$��80��$��30��$��10,0,0,0q��� $ 
Horizontal=�$��10q�.+��� $��Models đã train:��$	��120��$��Center=��� -   $ 	cmbModels��$	��200��$��30��$ dataset��$ dataset���� -   $ btnRefreshModels��$��🔄 Refresh��$��80��$��30��$��10,0,0,0q��� $��2��$��🎯 Training��$
��0,10q�.���� .+��� $ 
Horizontal=�$��10q�$��Center=�.+��� -   $ btnStartTraining��$ ��🚀 Bắt đầu Training��$	��150��$��35��$��5q��� -   $ btnStopTraining��$��⏹️ Dừng Training��$	��150��$��35��$��5q�${�False���� $��10q�.+��� -    ��Name$ txtTrainingStatus��$��Sẵn sàng training...��$	��0,5q�$
��Bold"�� -   	 �Name$	 progressTraining��$��20��
 � Minimum$
 0Q� � Maximum$	 100Q� � Value$ 0Q��� $��3��$��📈 Evaluation��$
��0,10q�.���� .+��� $ 
Horizontal=�$��10q�$��Center=�.+��� -	   $ btnStartEvaluation��$"��🔍 Bắt đầu Evaluation��$	��140��$��35��$��5q��� -
   $ btnStopEvaluation��$��⏹️ Dừng Evaluation��$	��140��$��35��$��5q�${�False���� -   $ btnVisualize��$��📊 Tạo Biểu đồ��$	��120��$��35��$��5q��� $��10q�.+��� -   $ txtEvaluationStatus��$��Sẵn sàng evaluation...��$	��0,5q�$
��Bold"�� -   $	 progressEvaluation��$��20��$
 0Q�$	 100Q�$ 0Q��� -   $ txtEvaluationResult��$�� ��$
��0,10q�$��16(�$
��Bold"�
��� � ��� $��4��$��📋 Activity Log��$
��0,10q�.���� .���� -    ��Name$ txtLog�� �� 
IsReadOnly$
 True�� �� TextWrapping$
 Wrap=�
���   �
���2�2�$��Consolas)�$��11(�$��10q�