   M S B A M L   `   `   `  ����D  @BlocksLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:BlocksLib.BlocksBlocksLib.Blocks  5      A%clr-namespace:BlocksLib.Blocks.ShapesBlocksLib.Blocks.Shapes  5      ?$clr-namespace:BlocksLib.Blocks.ToolsBlocksLib.Blocks.Tools  5      Q-clr-namespace:BlocksLib.Blocks.CustomControlsBlocksLib.Blocks.CustomControls  5      \ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a.    (BlocksLib.Blocks.Tools.ConverterMultiply   multiply)   #BlocksLib.Blocks.Tools.ConverterMax  max<   6BlocksLib.Blocks.Tools.ConverterSwitchThicknessToWidth  	swToWidth"   BlocksLib.Blocks.SwitchBlock  �� 
TargetType*   $BlocksLib.Blocks.MultiChildBlockBase �� Property �� Value    BlocksLib.Blocks.BlockBase �� 
TargetType!   BlocksLib.Blocks.BlockPanel#   BlocksLib.Blocks.Shapes.Arrow  
arrowStyle   BottomPadding ��Name	 �� X1	 �� Y1	 �� X2		 �� Y2
 �� Stretch �� RelativeSource �� AncestorType �� Path8   2BlocksLib.Blocks.CustomControls.TextBoxNotEditable  Name  conditionBlockStyle   	RootColorB	   <BlocksLib.Blocks.CustomControls.TextBoxNotEditableExtensions 	  MouseDownHandled   IsEdit �� Mode �� UpdateSourceTrigger*
  $Microsoft.Xaml.Behaviors.Interaction 
  Triggers+  %Microsoft.Xaml.Behaviors.EventTrigger   	EventName*  $Microsoft.Xaml.Behaviors.TriggerBase   Actions8  2Microsoft.Xaml.Behaviors.Core.ChangePropertyAction   PropertyName   Value  lineAddSwitchItemStyle2  ,Microsoft.Xaml.Behaviors.InvokeCommandAction   Command!   BlocksLib.Blocks.AutoCanvas   Center �� ElementName  	lineStyle   Left `� 	Converter"   BlocksLib.Blocks.SwitchPanel  Name �� Orientation  �� DataType&    BlocksLib.Blocks.SwitchBlockItem! � ColumnDefinitions" �� ValidationRules5  /System.Windows.Controls.DataErrorValidationRule,  &System.Windows.Controls.ValidationRule#   ValidationStep$ � LayoutTransform% �� Angle  selectLineSwitchItemStyle;  5BlocksLib.Blocks.ContentPresenterWithHorizontalCenter
&   Top�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      )blocksclr-namespace:BlocksLib.Blocks  5      0shapes%clr-namespace:BlocksLib.Blocks.Shapes  5      .tools$clr-namespace:BlocksLib.Blocks.Tools  5      8cntrls-clr-namespace:BlocksLib.Blocks.CustomControls  5      3i+http://schemas.microsoft.com/xaml/behaviors  5      %)  &	        &	      &	 &     '  9     7  7     5      6     5	      6     5
      6,   �� 5      "   6   .�5   
   ��  �@ BottomPadding$ 10Q�6*   �� 5   
    �@ Text$ x?��6!   �� 5   
    � �� 5      �� 5      "  6"   .R�5      �� ._�5        .+�5        #���  6+   #��z��6P   #��z 6v   #��z 6�   $��Center=�6�   �� 5      $ setHC��6#   
������ $ 0��6G   $ 0��6N   $ 1��6U   $	 0��6\   $

 Fill=�6c   ��5   #   ��  ��  blocks:BlockPanel$ HorizontalCenter�  5      $ 	rootBlock��68   #���  6K   #��z 6y   	 False5   8   #��z��6u    5   <   ��  ��  blocks:SwitchBlock$ IsBlockSelect�$ TwoWay=���5   <   ��  ��  blocks:SwitchBlock$
 Text�$ TwoWay=�$ PropertyChanged=�.��5   �    5   "     5   &   $ PreviewMouseDoubleClick��65   . 5   *     $ IsEdit��6A   $
 True��6W   5   +   5   '     5    &   $ LostKeyboardFocus��65   . 5!   *     $ IsEdit��6A   $ False��6W   5"   +   5#   '   .��5$   :   5%      � 5&      .+�5'   "   �� #���  6'   $ 0��6X   $ 0��6_   $ 0��6f   $	 1��6m   $

 Fill=�6t   $��Center=�6�   #��z 6�   5)   #   �� 5*   "   #��z 6,   #��z 6T   
������  5+   &     5,   *   $ 	MouseDown��69   . 5-   .      6D   ��  ��  blocks:SwitchBlock$ AddItem�5.   +   50   #   51        52       60   �� $ itemsViewer��$ HorizontalCenter�.+�53   "   �� $ 0��6'   $ 0��6.   $ 1��65   $	 0��6<   $

 Fill=�6C   #���  6R    6v   �� $ itemsViewer��$ HCExtremeElements.Left���54   &   `� 55   *   8   .1�56   .   �� $ itemsViewer��66   $ HCExtremeElements�6P   �� 57   .   $ itemsViewer��66   $ ActualWidth�6P   58   +   5:   #     5<   "   $ itemsViewer��65   $��Center=�6J   
������ ��6�   ��  ��  blocks:SwitchBlock$ Items���5=   &   �� 5>   *   .R�5?   .   �� $ 
Horizontal=�69   5@   +   ��5B   &   �� 5C   *   "   67   (��       *, +).R�5D   .   � ! 5E   2   �� 5F   6   ��6G   ��  ��  blocks:SwitchBlock$ BottomPadding��� 5G   6   �� 5H   6   ��6G   ��  ��  blocks:SwitchBlock$ BottomPadding�.+�5J   2     $��1��6D   $��Center=�6T   .+�5K   6   � ! 5L   :   �� 5M   >   �� 5N   >   ��6O   �� $	 key��$ ActualHeight�.+�5P   :   �� $	��key��6B   $��Center=�6M   
������ .��6�   ��5Q   >   �� 5R   B   $	 Key�6J   $ TwoWay=�6U   " 5S   F     5T   J   $# UpdatedValue=�6b   5V   C   .��5W   L   $ 5X   >   �� 5Y   B   $% 90Q�6R   .��5Z   W   5[   ;   �� 5\   :   #���  6?   $��2��6s   $

 Fill=�5]   =   $ 0��6L   $ 0��6S   $ 0��6Z   $	 1��6a   $��Center=�6h   ��5\   �   ��  ��  blocks:SwitchBlock$ BottomPadding���5^   =   ��  ��  blocks:SwitchBlock$ BottomPadding�5_   ;   �� 5`   :   $��1��6D   $

 Fill=�6T   
������  5a   >     5b   B   $ 	MouseDown��6Q   . 5c   F      6\   �� *
SelectItem+5d   C   5f   ;   5h   7     5i   6   $��2��6|   ��6b   �� *Value+6|   �� 5j   6   $��3��6;   
���   �$

 Fill=�5k   F   $ 0��6U   $ 0��6\   $ 0��6c   $	 1��6j   $��Center=�6q   ��5j   H   ��  ��  blocks:SwitchBlock$ BottomPadding���5l   7   ��  ��  blocks:SwitchBlock$ BottomPadding�5n   3   5o   /   5p   +   5r   #   �� 5t   "   $ 0��6'   $ 0��6.   $ 1��65   $	 0��6<   $

 Fill=�6C   #���  6R    6v   �� $ itemsViewer��$ HCExtremeElements.Left�& 6�   �� $ itemsViewer��$ ActualHeight���5u   &   `� 5v   *   8  .1�5w   .   �� $ itemsViewer��66   $ HCExtremeElements�6P   �� 5x   .   $ itemsViewer��66   $ ActualWidth�6P   5y   +   5{   #   5|      �� 5}      #���  6#   $ 0��6G   $ 0��6N   $ 0��6U   $	 1��6\   $

 Fill=�6c   $��Center=�6r   #��z 6�   5~      5      5�      5�      5�      5�      5�      