   M S B A M L   `   `   `  ����L  HTransferWorker.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:TransferWorker.UI.ViewsTransferWorker.UI.Views  5      K*clr-namespace:TransferWorker.UI.ViewModelsTransferWorker.UI.ViewModels  5      5    /TransferWorker.UI.Views.AddConfigAppSettingView   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7local%clr-namespace:TransferWorker.UI.Views     5      @	viewmodel*clr-namespace:TransferWorker.UI.ViewModels     5      -   5          >�Name$  
ConfigList��5
      $��13(�5      .��5      � .+�5   
   ]� $��0��6   .+�5      ]� $��Bottom=�6   $��Right=�60   $��0,0,0,10q�6L   .+�5      �� $��2q�6   ���#076a98$��30��6B    �� CornerRadius$ 3��6P   ���#ffffff$��80��6�   ._�5      �� $��     Thêm     ��6!    � Cursor$
 Hand��67   $��Center=�6E   $��Center=�6b    J� InputBindings 5      d� 5       d� MouseAction$ 	LeftClicke�6�    �� Command 6+   ��  �� ElementName$ 
ConfigList�� �� Path$ DataContext.Ok� �� CommandParameter 6n   �� *System.Reactive.Unit+6�   5      5      �� 5      $��5 0 5 0q�6   $��2q�6*   ���#ededed$ 3��6U   ���#333$��80��6�   ._�5      �� $
 Hand��6    $��
Quay lại��6.   $��Center=�6>   $��Center=�6[    5      d� 5      $ 	LeftClicke�6�    6+   �� $ 
ConfigList��$ DataContext.Cancel� 6r   �� *System.Reactive.Unit+6�   5      5      5      �� 5"      .+�5#      �� $��0��6   $��16(�6,   $��DemiBold"�6:   $��	0,15,10,0q�6Q   $9��3Thêm mới chuỗi kết nối đến blob storage��6d   $��Center=�6�   � 5$      $��0��6   	 � ColumnDefinitions	 5%      �� 5&      $	��150 �6+   �� 5'      �� 5(      $��0 �6+   .+�5*      � $��0��6   	 5+      �� 5,      �� 5-      $
��Auto �6/   .+�5/      �� $��1��6$   $��	0,15,10,0q�64   $��Right=�6H   
���  ��$	��(*)��6v   �� 50      $��0��6$   $��0,15,0,0q�66   $��Tên kết nối:��6H   $��Right=�6\   51      �� 52      $��1��6   $��	0,10,10,0q�6/   ��53   	   �� *NameAppSetting+
 �� Mode$
 TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�54      � 55      $��1��6   	 56      �� 57      $	��150 �6+   �� 58      �� 59      $��0 �6+   .+�5;      � $��0��6   	 5<      �� 5=      �� 5>      $
��Auto �6/   .+�5@      �� $��1��6$   $��	0,15,10,0q�64   $��Right=�6H   
���  ��$	��(*)��6v   �� 5A      $��0��6$   $��0,15,0,0q�65   $��Chuỗi kết nối:��6G   $��Right=�6]   5B      �� 5D      $��1��6    �� TextWrapping$
 Wrap=�6/    �� AcceptsReturn$
 True��6C   $��	0,10,10,0q�6X   ��5E   
   �� *StorageConnectionString+$
 TwoWay=�$ PropertyChanged=��� 5G      $��2��6   $��/Assets/icon.png��5H      $��20��6   $
��Left=�6)   6F   5I      � 5J      $��2��6   	 5K      �� 5L      $	��150 �6+   �� 5M      �� 5N      $��0 �6+   .+�5P      �� $��0��6    $��	0,15,10,0q�61   $��Tài khoản:��6D   $��Right=�6V   �� 5Q      $��1��6   ${�False��6/   $
 True��6A   $��	0,10,10,0q�6V   ��5R   	   �� *AccountName+�� 5T      $��2��6   $��/Assets/icon.png��5U      $��20��6   $
��Left=�6(   6E   5V      � 5W      $��3��6   .+�5X      ��  �� 
IsReadOnly$
 True��6   $	��500��60   6?   5Y      5Z      5[      5]      5^      