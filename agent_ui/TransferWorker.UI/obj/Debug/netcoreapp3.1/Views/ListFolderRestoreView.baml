   M S B A M L   `   `   `  ����L  HTransferWorker.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:TransferWorker.UI.ViewsTransferWorker.UI.Views  5      3    -TransferWorker.UI.Views.ListFolderRestoreView   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7local%clr-namespace:TransferWorker.UI.Views     5      -   5          >�Name$  
ConfigList��5      .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $��60 �6   �� 5      �� 5      $��3 �6   �� 5      $��50 �6   .+�5   
   � $��0��6   {�6   �� *
!IsVisible+ 5      �� 5      �� 5      .+�5      �� $��Verdana)�6   $
��Left=�68   $��Bottom=�6S   .+�5      �� $��0��6   $
��Bold"�6,   $��15(�6>   $��0,0,10,0q�6L   $
��Left=�6}   ��6^   �� *NameContainer+6}   5      �� 5      $��Bottom=�6      MaterialDesignDivider#���   5      $��1��5      $��Stretch=�5       J� SnapsToDevicePixels$
 True��5      � 5       $��1��6    � ColumnDefinitions 5!      �� 5"      $��50 �6'   �� 5#      $	��450 �6'   �� 5$      .+�5(      �� $��0��6   $��5 0 0 0q�6+   $
��Left=�6    d� 	IsChecked 6<   �� *
IsSelected+��6]   �� *btnSelectAll+6   �� 5)      $��1��6   $
��Tên��6,   $��SemiBold"�68   $��14(�6P   ���#52b4eb$
��Left=�5*      $��Center=�6/   �� 5,      $��2��6   $,��&Thời gian chỉnh sửa gần nhất��6,   $��SemiBold"�6R   $��14(�6j   ���#52b4eb$
��Left=�5-      $��Center=�6/   5.      5/      �� 50   
   $��Bottom=�6   #���   51      $��2��52      $��Stretch=�53      $
 True��54      � 55   
   $��10��56   *   $��1��66   $	��Top=�6k    � IsIndeterminate 6   �� *
IsProgress+y�6C   �� *HiddenIsProgress+6k   �� 57   
   $��1��6   $��	10 10 0 0q�6D   y�6!   �� *IsFirstText+%#Bạn vui lòng chọn vùng chứa6t   � 58   
   $��1��6   {�6   �� *!IsProgress+.+�59      �� ��6   �� *Files+��5:      �� 5;       �� 
TargetType" ��6   .�5<      ��  �� Property � �� �� Value$ 0 0 0 1q�6<   �� 5=       � �� �#ededed68   �� 5>       � ��$ 30��63   5?      ��5A      �� 5B      .R�5C      �  5D      �� 5E   "   $��40 �63   �� 5F   "   $��20 �63   �� 5G   "   $	��430 �63   �� 5H   "   $	��130 �63   .+�5J      �� $��0��6'    68   �� *	isChecked+�� 5L      $��0��6*   $��  ��6:   $��50��6D   ���#333	 J� InputBindings	 5M   "   d� 5N   &   
 d� MouseAction$
 	LeftClicke�6�    �� Command 63   ��  �� ElementName$ 
ConfigList�� �� Path$ DataContext.Tickss� �� CommandParameter 6x   �� 6�   5P      �� 5R      $��1��6$   $��20��64   $��/Assets/folder.png��6?   y�6[   �� *
IsFolder+6}   �� 5S      $��1��6$   $��20��64   $��/Assets/file.png��6?   y�6Y   �� *IsFile+6y   �� 5U      $��2��6)   $��Center=�6S   ���#333��69   �� *
FileName+	 5V   "   d� 5W   &   $
 	LeftClicke�6�    63   �� $ 
ConfigList��$ DataContext.SelectFolder� 6~   �� 6�   5Y      �� 5Z      $��3��6(   ���#333$��Right=�6o   ��6J   �� *LastModifiedDisplay+6o   5[      5\      5^      5_      � 5`   
   $��2��6   $��0,10,0,0q�6#   � 5a   
   $��3��6    5b      �� 5c      $	��120 �6#   �� 5d      �� 5e      $��50 �6#   �� 5f      $	��100 �6#   .+�5h      �� $��0��6   $"��Đường dẫn phục hồi��6)   $��Center=�6C   �� 5i      $��1��6    �� AcceptsReturn$
 True��6&   $��	10,0,10,0q�6<   $��Center=�5j   '   ��6
   �� *PathRestore+6'   �� 5l      -   5        $��2��5l      ���#333
�������$��0q�6K   
�������$	��...��6�   $��0,0,10,0q�5m      �� 5n      $��3��6   ���#006bce$��0,5,0,0q�6:   $��Right=�5o      $��Center=�6"   $	��100��5p      $��40��6   
�������$��Center=�68   {�5n   L   �� *
!IsVisible+��5q      �� *
btnRestore+.��6$   Phục hồi6.   5s      5u      5v      