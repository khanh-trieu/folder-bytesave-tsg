   M S B A M L   `   `   `  ����L  HTransferWorker.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:TransferWorker.UI.ViewsTransferWorker.UI.Views  5      2    ,TransferWorker.UI.Views.ConfigMenuBackupView   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7local%clr-namespace:TransferWorker.UI.Views     5      b ^MaterialDesignThemes.Wpf, Version=3.2.0.1979, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  6B   -   5          >�Name$  
ConfigList��5      $��13(�5      .��5      �  � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $��5 �6   �� 5      .+�5   
   � $��0��6   ���#7F898C.+�5      �� $��Verdana)�6      Backgroudstack#��[  5      .+�5      � .+�5      �� $��0��6    $��	10,10,0,5q�60   $
��Bold"�6D     
BackupList#���  6V     ThemeBackgroundColor#���  6z   $
��Left=�6�   $��Center=�6�   �� 5      -   5        $
��btn5��5      $��0,5,5,5q�6?   $��Right=�6P   $��0q�6m    � ToolTip$$ Thêm mới tác vụ sao lưu��5   <   $��1��5      ��5   )   Z� ��5      ��  �� Path$! DataContext.btnAddItemBakup� �� RelativeSource ��  �� AncestorType
 Window.��5      �� $��/Assets/plus.png��6    $��20��6:   5      5      �� 5      $��0 0 0 5q�6   65   5      5      �� 5    
   $��1��6   
�������$��0,0,0,1q�66   � 5!   
   $��2��6   .+�5"      �� $
��Auto=�6   $
��Auto=�6>   .��5#      ��  � IsSharedSizeScope$
 True��5%      ��5$      �� *Items+��5&      �� 5'      .R�5(      ��  ��Name$ Border��6%    �� Padding$ 5q�65   ���#ffffff$��0 0 0 1q�5)   	     MaterialDesignDivider#���  6#   ._�5*   "   � 	 � ColumnDefinitions	 5+   &   �� 5,   *   
 z� SharedSizeGroup$
 Checkerz��6;   �� 5-   *   �� 5.   *   $��50 �6;   .+�50   &   �� $��5 5 0 0q�63   $��1��6D   .+�51   *   �� $��CharacterEllipsis=�6L   $	��130��6m   ���#333��65   �� *name+ 6�   �� *name+ J� InputBindings 52   .   d� 53   2    d� MouseAction$ 	LeftClicke�6�    �� Command 6?   ��  �� ElementName$ 
ConfigList��$ DataContext.Detail� �� CommandParameter 6�   �� 6�   55   +   56   '   (  "MaterialDesignThemes.Wpf.ColorZone  57   &   $��2��6?   $��0 5 0 0q�6O   ���#333$��Right=�6r   ��6�   Z� .��58   *   ]� .+�59   .   '  !MaterialDesignThemes.Wpf.PopupBox  $��0q�5:         PlacementMode$ RightAndAlignTopEdges=�5;         PopupContent. 5<   2   �� .@�5=   6   �� $��Xóa ��6A   $��1��6M   $
��Left=�6^   $��Center=�6y   $��70��6�   ��5>   :   �� 5?   >    �� 
TargetType" ��6D   .�5@   B   ��  �� Property � �� �� Value �#3333336`    �� Triggers 5A   B   P� 5B   F    P� Property �@J�IsMouseOver P� Value$
 True��6f   . �5C   J   ��  � ��
 �  ��6h   5D   G   5F   ?    5I   :   d� 5J   >   $ 	LeftClicke�6�    6K   �� $ 
ConfigList��$ DataContext.Delete� 6�   �� 6�   �� 5M   ;   $��0��6A   $��/Assets/delete.png��6Q   $��0 0 5 0q�6n   $��15��6   6�   5N   7   5P   3   5Q   /   5R   +   5S   '   5T   #   5V       �� Triggers 5W      �� 5X   "    �� Value$
 True��5Z       �� Binding 5Y      �� *	JobName+.a�5[   &   ��  �� 
TargetName$ Border��5\        MaterialDesignSelection# �  5^       � ��5_   #   5a      5c      5d      5e      5f      5g      