   M S B A M L   `   `   `  ����D  @BlocksLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:BlocksLib.BlocksBlocksLib.Blocks  5      A%clr-namespace:BlocksLib.Blocks.ShapesBlocksLib.Blocks.Shapes  5      ?$clr-namespace:BlocksLib.Blocks.ToolsBlocksLib.Blocks.Tools  5      Q-clr-namespace:BlocksLib.Blocks.CustomControlsBlocksLib.Blocks.CustomControls  5      \ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a.    (BlocksLib.Blocks.Tools.ConverterMultiply   multiply*   $BlocksLib.Blocks.Tools.ConverterSumm  summ)   #BlocksLib.Blocks.Tools.ConverterMax  max6   0BlocksLib.Blocks.Tools.ConverterDoubleToThicness  
toThicness!   BlocksLib.Blocks.BlockCycle  �� 
TargetType �� Property �� Value �� 
TargetType!   BlocksLib.Blocks.AutoCanvas ��Name%   BlocksLib.Blocks.BlockCycleBase   	FalseText
   Top `� 	Converter �� Path	 �� 	Converter
 �� ConverterParameter �� RelativeSource �� AncestorType �� ElementName �� Mode   Left �� Stretch � Points    BlocksLib.Blocks.BlockBase   	RootColor `� ConverterParameter!   BlocksLib.Blocks.BlockPanel  Name �� 	Converter �� ConverterParameter#	   BlocksLib.Blocks.Shapes.Arrow  
arrowStyle*
   $BlocksLib.Blocks.MultiChildBlockBase 
  BottomPadding8   2BlocksLib.Blocks.CustomControls.TextBoxNotEditable  Name  conditionBlockStyleB   <BlocksLib.Blocks.CustomControls.TextBoxNotEditableExtensions   MouseDownHandled   IsEdit �� Mode �� UpdateSourceTrigger*  $Microsoft.Xaml.Behaviors.Interaction   Triggers+  %Microsoft.Xaml.Behaviors.EventTrigger   	EventName*  $Microsoft.Xaml.Behaviors.TriggerBase   Actions8  2Microsoft.Xaml.Behaviors.Core.ChangePropertyAction    PropertyName!   Value" � ColumnDefinitions	# �� X1	$ �� Y1	% �� X2	& �� Y2  	lineStyle'   	TrueColor(   TrueText) �� Padding* ��Name;  5BlocksLib.Blocks.ContentPresenterWithHorizontalCenter+  Name,   
TrueSource- ��Name. 	  Rotate�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      )blocksclr-namespace:BlocksLib.Blocks  5      0shapes%clr-namespace:BlocksLib.Blocks.Shapes  5      .tools$clr-namespace:BlocksLib.Blocks.Tools  5      8cntrls-clr-namespace:BlocksLib.Blocks.CustomControls  5      3i+http://schemas.microsoft.com/xaml/behaviors  5      %�  &	        &	      &	 &     &	 9     '  L     7  7   7   7   7  7   7   7  7   7   7  7   7   7  7   7   7  7   7   7   7  7   7  7   7  7   7   7     5      6     5	      6     5
      6     5      6&   �� 5      "   6   .�5   
   ��  � �� 5      �� 5      "  6"   .R�5      �� ._�5        .+�5      �� $ noText��6(   #��z 68   #��z��6[   #��z��6�    5   "   `� 5   &   8   .1�5   *   �� $ BottomPadding�6s   8	  $
 2��6�    62   ��  blocks:BlockCycle6�   �� 5   *   $ 	rootBlock��62   $ ActualHeight�6J   8	  $	
 0.5��6�   �� 5   *   $ ActualHeight�6^   8	  $
 -1��6�    62   �� $
 Self=�6�   5   '    5   "   `� 5   &   8  .1�5   *   �� $	 Pnl��62   $ HorizontalCenter�6D   �� 5   *   $ 	rootBlock��62   $ ActualWidth�6J   8	  $	
 0.5��6�   �� 5   *   $ BottomPadding�6s   8	  $
 2��6�    62   ��  blocks:BlockCycle6�   5    '   5"      � 5$      $
 Fill=�6'    �   #��z 6O   ��5%   "   `� 5&   &   8  .1�5'   *   �� $ BottomPadding�6s   8	  $
 2��6�    62   ��  blocks:BlockCycle6�   �� 5(   *   $ yesBlock��62   $ ActualHeight�6I   �� 5)   *   $ 	rootBlock��62   $ ActualHeight�6J   8	 	 $	
 0.5��6�   �� 5*   *   $ selectYesBlock��62   $ ActualHeight�6O   5+   '   ��5-   "   `� 5.   &   8 
 $ 2��6T   .1�5/   *   �� $	 Pnl��62   $ ActualWidth�6D   �� 50   *   $	 Pnl��62   $ HorizontalCenter�6D   8	  $
 -1��6�   �� 51   *   $ noText��62   $ ActualWidth�6G   �� 53   *   $ 	rootBlock��62   $ ActualWidth�6J   8	  $	
 0.5��6�   �� 54   *   $ noText��62   $ ActualWidth�6G   55   '    57   "   `� 58   &   8  .1�59   *   �� $ BottomPadding�6s   8	  $
 2��6�    62   ��  blocks:BlockCycle6�   �� 5:   *   $ 	rootBlock��62   $ ActualHeight�6J   8	  $	
 0.5��6�   5;   '    5=   "   `� 5>   &   8  .1�5?   *   �� $	 Pnl��62   $ HorizontalCenter�6D   �� 5@   *   $ BottomPadding�6s   8	  $
 2��6�    62   ��  blocks:BlockCycle6�   5A   '   5C        5F      $	 Pnl��60    5G   #   �� *BottomPadding+8  $ 2��.+�5H   "   	  #���  6/   #��z��6T   #��z 6z   ��6�   �� *BottomPadding+8  $ 2��5J   #     5L   "   $ 	rootBlock��6<   #���  6O   #��z 6}   	 False5O   8   #��z��6u    5M   <   ��  ��  blocks:BlockCycle$ IsBlockSelect�$ TwoWay=���5N   <   ��  ��  blocks:BlockCycle$
 Text�$ TwoWay=�$ PropertyChanged=�.��5O   �    5P   &     5Q   *   $ PreviewMouseDoubleClick��69   . 5R   .     $  IsEdit��6E   $
! True��6[   5S   /   5T   +     5U   *   $ LostKeyboardFocus��69   . 5V   .     $  IsEdit��6E   $! False��6[   5W   /   5X   +   .��5Y   >   5Z   #   � 5\   "   " 5]   &   �� 5^   *   $
��auto �6;   �� 5_   *   $
��auto �6;   �� 5`   *   $
��auto �6;   .+�5b   &   �� $��1��6+   $# 0��6;   $$ 0��6B   $% 0��6I   $& 1��6P   $
 Fill=�6W   #���  6f   #��z' 6�   #��z 6�   �� 5c   &   $ yesText��6t   #��z( 60   $) 5,0,0,0q�6R   $��2��6d   #��z��6�   #��z��6�   �� 5d   &   ��60   �� $ yesText��$ ActualWidth��� 5e   &   $* selectYesBlock��6D   $��3��60   $
 Fill=�6\   
������ 6k   5f   #     5g   "   $+ yesBlock��6u   #��z, 6N   6u   �� 5h   "   $# 0��6'   $$ 0��6.   $% 0��65   $& 1��6<   #��z 6C   $
 Fill=�6l   #���  6{   #��z' 6�   5i      �� 5k      $- setHC��6N   $# 0��6#   $$ 0��6*   $% 1��61   $& 0��68   $
 Fill=�6?   ��5l   "   `� 5m   &   8  .1�5n   *   �� $ BottomPadding�6s   8	  $
 2��6�    62   ��  blocks:BlockCycle6�   �� 5o   *   $	 Pnl��62   $ HorizontalCenter�6D   5p   '   5r      � 5v      #��z' 6'   $
 Fill=�6L    �   # z 5w   '   # z 6_   ��5x   "   `� 5y   &   8  $ 1��6T   .1�5z   *   �� $	 Pnl��62   $ HorizontalCenter�6D   �� 5{   *   $ BottomPadding�6s    62   ��  blocks:BlockCycle6s   �� 5}   *   $ 	rootBlock��62   $ ActualWidth�6J   8	  $	
 0.5��6�   �� 5~   *   $ BottomPadding�6s    62   ��  blocks:BlockCycle6s   5   '   ��5�   "   `� 5�   &   8  .1�5�   *   �� $	 Pnl��62   $ ActualHeight�6D   �� 5�   *   $ BottomPadding�6s   8	  $
 -1��6�    62   ��  blocks:BlockCycle6�   5�   '   5�      	  5�      #���  6+   #��z' 6P   $. Right=�6u   # z 6�   ��5�   +   �� *BottomPadding+8  $ 2����5�   "   `� 5�   &   8  .1�5�   *   �� $ BottomPadding�6s    62   ��  blocks:BlockCycle6s   �� 5�   *   $	 Pnl��62   $ HorizontalCenter�6D   5�   '   5�      5�      5�      5�      5�      �� 5�   
    �@
 BottomPadding$ 10Q�6*   �� 5�   
    �@ Text$ x?��6!   5�      5�      