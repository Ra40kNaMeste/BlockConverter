   M S B A M L   `   `   `  ����I  EBlockConverter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:BlockConverter.BlocksBlockConverter.Blocks  5      W0clr-namespace:BlockConverter.Blocks.ChildsBlocks"BlockConverter.Blocks.ChildsBlocks  5      K*clr-namespace:BlockConverter.Blocks.ShapesBlockConverter.Blocks.Shapes  5      \ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089-   'BlockConverter.Blocks.ConverterMultiply   multiply)  #BlockConverter.Blocks.ConverterSumm  summ5  /BlockConverter.Blocks.ConverterThicnessToHeight  toHeight4  .BlockConverter.Blocks.ConverterThicnessToWidth  toWidth3  -BlockConverter.Blocks.ConverterWidthToThiness  widthToThiness  �� 
TargetType  	lineStyle �� Property �� Value(   "BlockConverter.Blocks.Shapes.Arrow  
arrowStyle  SelectBrush �� Color  DefaultBrush0   *BlockConverter.Blocks.Shapes.StarAngleLine 	 twoLineStyle!   BlockConverter.Blocks.Block 
 
blockStyle7   1BlockConverter.Blocks.ChildsBlocks.ConditionBlock  conditionBlockStyle*	   $BlockConverter.Blocks.BlockCycleBase  blockCycleBaseStyle  beginBlockStyle �� BasedOn �� 
TargetType ��Name%
   BlockConverter.Blocks.BlockBase 
  	RootColor �� RadiusX	 �� RelativeSource
 �� Mode �� Path �� 	Converter �� ConverterParameter �� RadiusY `� 	Converter �� ElementName �� AncestorType 
  Text �� TextWrapping �� AcceptsReturn  endBlockStyle � RowDefinitions   
StartPoint�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      .blocks#clr-namespace:BlockConverter.Blocks  5      ;childs0clr-namespace:BlockConverter.Blocks.ChildsBlocks  5      5shapes*clr-namespace:BlockConverter.Blocks.Shapes  5      %O  &	        &	      &	 &     &	 9     &	 L     &	 _     &	 �     &	 �    &	 �    &		 �    &	
     &	 c    &	 �    &	     7 
 7   7  7  7  7  &	 �    7 
 7   7  7  7  7     5      6     5      6     5      6'     5	      6&     5
      6%   �� 5      "  ��6   .�5   
   ��  � ��
 �   �6#   5      �� 5      "   6   .�5   
   ��  � ��
 �   �6#   �� 5   
    � ��$ 20��6&   �� 5   
    � ��
 �   �6!   �� 5   
    �@ Length$ 5Q�6#   �� 5   
    �@ 
StartPoint$ 3Q�6'   5      �� 5      $
 Aqua��6*   �� 5      $ Black��6+   �� 5      "   6   .�5   
   ��  � ��
 �   �6#   5      �� 5      "   6   .�5   
   ��  � ��$ 5q�6$   5      �� 5      "   6   .�5    
   ��  � ��$ 40q�6$   5!      �� 5#      "  	 6   .�5$   
   ��  �@	 BottomPadding$ 30Q�6*   5%      �� 5&      "   6   8   .�5'   
   ��  � �� 5(      �� 5)      "  6"   .R�5*      �� ._�5+      � .+�5,      �� $ 	rootBlock��6�   $��0��6(   #��z 65   $��2��6Z   $��Center=�6n   
������� 5-   )   �� 	 �� $

 Self=�$ ActualHeight�8  $	 0.5�� 5.   )   �� 	 �� $

 Self=�$ RadiusX���5/   "   `� 50   &   8  .1�51   *   �� $
 text��62   $ ActualHeight�6E   �� 52   *   $ Padding�6�   8  	 62   ��  blocks:Block$
 FindAncestor=�6�   53   '   ��55   "   `� 56   &   8  .1�57   *   �� $
 text��62   $ ActualWidth�6E   �� 58   *   $ Padding�6�   8  	 62   ��  blocks:Block$
 FindAncestor=�6�   59   '   5;      �� 5<      $
��text��6�   $��0��6&   #��z 63   $��Center=�6Q   $��Center=�6l   $
 Wrap=�6�   $
 True��6�   .��6�   5=      5?      5@      5A      5C      5D      �� 5E      "   6   8   .�5F   
   ��  � �� 5G      �� 5H      "  6"   .R�5I      �� ._�5J      �  5K      �� 5L   "   �� 5M   "   $
��Auto �60   .+�5O        #���  6+   $ 3Q�6P   5P      �� 5Q      $ 	rootBlock��6{   $��1��6(   #��z 65   $��2��6Z   
������� 5R   )   �� 	 �� $

 Self=�$ ActualHeight�8  $	 0.5�� 5S   )   �� 	 �� $

 Self=�$ RadiusX���5T   "   `� 5U   &   8  .1�5V   *   �� $
 text��62   $ ActualHeight�6E   �� 5W   *   $ Padding�6�   8  	 62   ��  blocks:Block$
 FindAncestor=�6�   5X   '   ��5Z   "   `� 5[   &   8  .1�5\   *   �� $
 text��62   $ ActualWidth�6E   �� 5]   *   $ Padding�6�   8  	 62   ��  blocks:Block$
 FindAncestor=�6�   5^   '   5`      �� 5a      $
��text��6�   $��1��6&   #��z 63   $��Center=�6Q   $��Center=�6n   $
 Wrap=�6�   $
 True��6�   5b      5c      5d      5f      5g      5i      