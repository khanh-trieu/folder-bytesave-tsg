   M S B A M L   `   `   `  ����L  HTransferWorker.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:TransferWorker.UI.ViewsTransferWorker.UI.Views  5      K*clr-namespace:TransferWorker.UI.ViewModelsTransferWorker.UI.ViewModels  5   	   ,    &TransferWorker.UI.Views.MainWindowView   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7views%clr-namespace:TransferWorker.UI.Views     5      <local*clr-namespace:TransferWorker.UI.ViewModels     5   	   $	��640��5   	    
  vmMain  � DataContext#  [  5	   	   $��12(�5
   	   $��Roboto,sans-serif)�5   	    � Title$ ByteSave Backup��5   	   $	��450��6!   $
��1000��6.    � 	Resources 5       �� DataType2   ,TransferWorker.UI.ViewModels.MainWindowModel,   &TransferWorker.UI.Views.MainWindowView0   *TransferWorker.UI.ViewModels.InfoViewModel&    TransferWorker.UI.Views.InfoView<   6TransferWorker.UI.ViewModels.EditConfigBackupViewModel2   ,TransferWorker.UI.Views.EditConfigBackupView;   5TransferWorker.UI.ViewModels.AddConfigBackupViewModel1   +TransferWorker.UI.Views.AddConfigBackupView@	   :TransferWorker.UI.ViewModels.EditConfigAppSettingViewModel6
   0TransferWorker.UI.Views.EditConfigAppSettingView<   6TransferWorker.UI.ViewModels.ConfigMenuBackupViewModel2   ,TransferWorker.UI.Views.ConfigMenuBackupView;   5TransferWorker.UI.ViewModels.ListMenuRestoreViewModel1   +TransferWorker.UI.Views.ListMenuRestoreView=   7TransferWorker.UI.ViewModels.ListFolderRestoreViewModel3   -TransferWorker.UI.Views.ListFolderRestoreView>   8TransferWorker.UI.ViewModels.ConfigDetailBackupViewModel3   -TransferWorker.UI.Views.ConfigDetaiBackupView6   0TransferWorker.UI.ViewModels.ConfigMenuViewModel,   &TransferWorker.UI.Views.ConfigMenuView8   2TransferWorker.UI.ViewModels.ConfigDetailViewModel.   (TransferWorker.UI.Views.ConfigDetailView?   9TransferWorker.UI.ViewModels.AddConfigAppSettingViewModel5   /TransferWorker.UI.Views.AddConfigAppSettingView3   -TransferWorker.UI.ViewModels.HistoryViewModel)   #TransferWorker.UI.Views.HistoryView3   -TransferWorker.UI.ViewModels.SettingViewModel)   #TransferWorker.UI.Views.SettingView��%  (��       5      *, +)(�� 2     5      *, +)(�� d     5      *, +)(�� �     5      *, +)(�� �     5      *,	 +)(�� �     5!      *, +)(�� ,    5$      *, +)(�� ^    5'      *, +)(�� �    5*      *, +)(�� �    5-      *, +)(�� �    50      *, +)(�� &    53      *, +)(�� X    56      *, +)(�� �    59      *, +)�� 5   
   "  6   .R�5        5      �� 5   
   "  6   .R�5        5      �� 5   
   "  6   .R�5        5      �� 5   
   "  6   .R�5        5      �� 5   
   " 	 6   .R�5      
  5       �� 5!   
   "  6   .R�5"        5#      �� 5$   
   "  6   .R�5%        5&      �� 5'   
   "  6   .R�5(        5)      �� 5*   
   "  6   .R�5+        5,      �� 5-   
   "  6   .R�5.        5/      �� 50   
   "  6   .R�51        52      �� 53   
   "  6   .R�54        55      �� 56   
   "  6   .R�57        58      �� 59   
   "  6   .R�5:        5;      5<      .��5>      �  � RowDefinitions 5@   
   �� 5A      $��60 �6   �� 5B      $��2 �6   �� 5C      .+�5F   
   � $��0��6   ���#ffffff$��	0 0 0 -10q�62   {�6E   �� *	IsLicense+ � ColumnDefinitions 5G      �� 5H      $��0.21* �6#   �� 5I      $
��0.7* �6#   .+�5K      �� $��/Assets/tsg.png��6   $��0��6-   $��10,10,10,10q�6=    J� Opacity$ 1Q�6S   � 5M      $��1��6    5N      �� 5O      $
��0.7* �6'   �� 5P      $
��0.7* �6'   �� 5Q      $
��0.7* �6'   �� 5R      $
��0.7* �6'   �� 5S      $
��0.7* �6'   �� 5T      $
��0.7* �6'   .+�5V      �� $��0��6   $��0q�6*   $��15(�66   $��50��6D     Connect#���  6P     BackgroudButton#��[ 6w   $��Center=�5X      $��Center=�6*   ��5W      �� *btnAppSettingConfigView+5X   *   �� 5Y      $��1��6   $��15(�6)   $��50��67    
 Backup#���  6C   #��[ 6g   $��Center=�5[      $��Center=�6*   ��5Z   	   �� *btnFolderConfigView+5[   *   �� 5\      $��2��6   $��15(�6)   $��50��67     Restore#���  6C   $��0 0 1 0q�6h   ���#ffffff#��[ 6�   $��Center=�5^      $��Center=�6*   ��5]   	   �� *btnRestoreView+5^   *   �� 5_      $��3��6     Setting#���  6)   $��50��6M   $��15(�6Y   #��[ 6i   $��Center=�5a   	   $��Center=�6$   ��5`   
   �� *btnSettingView+5a   $   �� 5b      $��4��6   $��0q�6)   $��15(�65   $��50��6C     Information#���  6O   #��[ 5c      $��Center=�5d   	   ��6$   �� *btnInfoView+�� 5e      $��5��6     History#���  6)   $��50��6M   $��15(�6Y   #��[ 6i   $��Center=�5g   	   $��Center=�6$   ��5f   
   �� *btnHistoryView+5g   $   5h      5i      �� 5j   
   $��1��6   $��4��6   
�������$��10q�6F   � 5k   
   $��2��6   $��2��6    5l      �� 5m      $
��0.3* �6#   �� 5n      $��5 �6#   �� 5o      �� 5p      $��5 �6#   .+�5r      � $��0��6   ���#D7DFF1.+�5s      '   !TransferWorker.UI.Views.ChildView  ��6"   �� *ContentMenu+6D   5u      �� 5v      $��1��6   $��5��6%   
�������$��1q�6F   � 5w      $��2��6   .+�5y        ��6"   �� *ContentDetail+6F   5{      5�      5�      5�      