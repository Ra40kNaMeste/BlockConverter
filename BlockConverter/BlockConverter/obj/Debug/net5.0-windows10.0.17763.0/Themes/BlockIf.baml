   M S B A M L   `   `   `  ����I  EBlockConverter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:BlockConverter.BlocksBlockConverter.Blocks  5      K*clr-namespace:BlockConverter.Blocks.ShapesBlockConverter.Blocks.Shapes  5      W0clr-namespace:BlockConverter.Blocks.ChildsBlocks"BlockConverter.Blocks.ChildsBlocks  5      \ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089/   )BlockConverter.Blocks.ConvertDoubleToRect 
  toRect-  'BlockConverter.Blocks.ConverterMultiply  multiply1  +BlockConverter.Blocks.ConveterDoubleToPoint  toPoint3  -BlockConverter.Blocks.ConverterWidthToThiness  widthToThiness  �� 
TargetType  answerStyle �� Property �� Value#   BlockConverter.Blocks.BlockIf �� 
TargetType � ColumnDefinitions � RowDefinitions(   "BlockConverter.Blocks.Shapes.Arrow  
arrowStyle0   *BlockConverter.Blocks.Shapes.StarAngleLine  twoLineStyle   
FalseColor   IsCenter*   $BlockConverter.Blocks.BlockCycleBase   BottomPadding	 �� RelativeSource
 �� AncestorType �� Path   
AngleLines,   &BlockConverter.Blocks.Shapes.AngleLine   Angle ��Name   	TrueColor7	   1BlockConverter.Blocks.ChildsBlocks.ConditionBlock 	 Name  conditionBlockStyle%
   BlockConverter.Blocks.BlockBase 
  Text 	  Text 
  	RootColor ��Name   
TrueSource   NoSource  Name�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      .blocks#clr-namespace:BlockConverter.Blocks  5      5shapes*clr-namespace:BlockConverter.Blocks.Shapes  5      ;childs0clr-namespace:BlockConverter.Blocks.ChildsBlocks  5      %�	  &	        &	      &	 &     &	 9     &	 L     '  �     7  7     5      6!     5      6     5      6#     5	      6%   �� 5      "  ��6   .�5   
   ��  � ��$ 	0,-20,0,0q�6#   �� 5   
    � ��$ Center=�6.   5      �� 5      "   6   .�5   
   ��  � �� 5      �� 5      "  6"   .R�5      �� ._�5      �  5      �� 5   "   $
��Auto �63   �� 5   "   $
��Auto �63   �� 5   "   $
��Auto �63    5      �� 5   "   $��* �60   �� 5   "   $
��Auto �60   �� 5   "   $
��Auto �60   �� 5   "   $
��Auto �60   .+�5         #���  6+   $��1��6P   $��0��6`     5!      #���  63   #��z 6Z   $��0��6�   $��1��6�   $��Right=�6�   $ False��6�   #��z 5"   3   ��6^   �� 	 �� 
 blocks:BlockIf$ NoSource.ActualWidth� 5$   &     $ 0Q�67     5%   &   $	 -90Q�67   5'      �� 5(      8��  $��Right=�6M   $��0��6i   $��1��6y   $��нет��6�   �� 5)      $ selectNoBlock��6X   
������ $��0��6;   $��1��6K   6X     5+      #���  63   $��2��6Z   $��1��6j   #��z 6w   $
��Left=�6�   $ False��6�   #��z 5,   3   #��z 5-   3   ��5,   ^   �� 	 �� 
 blocks:BlockIf$ TrueSource.ActualWidth� 5/   &     $	 180Q�67     50   &   $	 -90Q�67   52      �� 53      8�� $
��да��6M   $��1��6W   $��2��6d   $
��Left=�6t   �� 54      $ selectYesBlock��6X   
������ $��1��6;   $��2��6H   6X   	  57      $ 	rootBlock��6�   #���  64   $��1��6b   $��1��6r   # z 6   $��Stretch=�6�   #��z 6�   �� 59      $ yesBlock��6V   #��z 6/   $��2��6h   $��2��6u   �� 5:      $ noBlock��6T   #��z 6/   $��2��6e   $��0��6r     5=      #���  63   #��z 6Z   $��0��6�   $��Right=�6�   $ False��6�   $��3��5>   3   #��z 6@   #��z 5?   3   ��5>   k   �� 	 �� 
 blocks:BlockIf$ NoSource.ActualWidth� 5A   &     $ 0Q�67     5B   &   $ 90Q�67   5D        5E      #���  63   #��z 6Z   $��2��6   $��3��6�   $ False��6�   #��z 5F   3   #��z 5G   3   ��5F   ^   �� 	 �� 
 blocks:BlockIf$ TrueSource.ActualWidth� 5I   &     $	 180Q�67     5J   &   $ 90Q�67   5L        5M      $ downLine��6�   #���  63   $��1��6Z   $��3��6j   $ False��6w   #��z 6�    5O   &     $	 180Q�67     5P   &   $	 -90Q�67     5Q   &   $ 0Q�67   5S      5T      5U      5V      5X      5Y      5Z      