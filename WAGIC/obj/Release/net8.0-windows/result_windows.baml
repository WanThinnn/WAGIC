   M S B A M L   `   `   `  ���� @  <WAGIC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WAGICWAGIC      WAGIC.result_windows   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     %localclr-namespace:WAGIC       � Title$)  #🎯 MAGIC Model Evaluation Results��$	��700��$
��1200�� � WindowStartupLocation$ CenterScreen=� � WindowState$ 	Maximized=�.���  � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0�����#2E86AB �� Padding$ 15q�._���  �� Orientation$ 
Horizontal=�$��Center=�.+��� $,��&📊 MAGIC Model Performance Dashboard��$��24(�$
��Bold"�
�������� $��1��.+��� -    ��Name$ imageScrollViewer��$
��Auto=�$
��Auto=�$
��True��.���� -    ��Name$ 	imgResult��$��Uniform=�$��Center=�$��Center=�$��10q��� $ 
Horizontal=�$��Right=�$	��Top=�$��20q�
������� J� Opacity$	 0.8Q�.+��� -   	 ��Name$	 	btnZoomIn��$��🔍+��$��40��$��30��$��2q��� -   $	 
btnZoomOut��$��🔍-��$��40��$��30��$��2q��� -   $	 btnZoomReset��$
��🔄��$��40��$��30��$��2q�
 � ToolTip$
 
Reset Zoom���� -    ��Name$ txtZoomLevel��$
��100%��$��Center=�$
��10,0q�$
��Bold"��� $��2�����#F0F0F0$ 10q�._��� $ 
Horizontal=�.+��� -   $ 	txtStatus��$ ��Đang tải kết quả...��$��Center=�$��0,0,20,0q��� -   $	 
btnRefresh��$��🔄 Refresh��$��80��$��30���� -	   $	 	btnSaveAs��$��💾 Save As...��$	��100��$��30��$
��10,0q��� -
   $	 btnFitToWindow��$��📐 Fit to Window��$	��110��$��30��$
��10,0q��� -   $	 btnActualSize��$��📏 Actual Size��$	��100��$��30��$
��10,0q��� -   $	 btnClose��$��	❌ Close��$��70��$��30��$
��10,0q�