   M S B A M L   `   `   `  ����I  EBlockConverter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:BlockConverterBlockConverter  5   	   D @BlocksLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullF1clr-namespace:BlocksLib.Blocks;assembly=BlocksLibBlocksLib.Blocks 5   	   ;"clr-namespace:BlockConverter.ToolsBlockConverter.Tools  5	   	   d@clr-namespace:BlocksLib.Blocks.CustomControls;assembly=BlocksLibBlocksLib.Blocks.CustomControls 5
   	   E'clr-namespace:BlockConverter.ViewModelsBlockConverter.ViewModels  5   	   C&clr-namespace:BlockConverter.ResourcesBlockConverter.Resources  5   	       BlockConverter.MainWindow   5   	   \ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:BlockConverter     5   	   _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  5   	   >blocks1clr-namespace:BlocksLib.Blocks;assembly=BlocksLib  5   	   .tools"clr-namespace:BlockConverter.Tools  5	   	   Kcntr@clr-namespace:BlocksLib.Blocks.CustomControls;assembly=BlocksLib  5
   	   :s,http://schemas.microsoft.com/winfx/2006/xaml     5   	   6vm'clr-namespace:BlockConverter.ViewModels     5   	   6res&clr-namespace:BlockConverter.Resources     5   	     � Title$  Block Converter��5   	   $
��auto��6!   $
��auto��6/    J� InputBindings 5      �� 5   
    �� 	Modifiers$
 Ctrlf�6   
 �� Key$ X��6&    �� Command 6.   �� *RemoveBlock+ �� CommandParameter 5        BlocksLib.Blocks.Block  5         TypeName$ Block=�6   5      �� 5   
   $ Delete��6    6"   �� *RemoveBlock+ 5        5      $ Block=�6   5      �� 5   
   $
 Ctrlf�6   $ B��6&    6.   �� *
AddBlock+ 5        5      $ Block=�6   5      �� 5    
   $
 Ctrlf�6   $ E��6&    6.   �� *
AddBlock+ 5!        5"      $ EndBlock=�6   5$      �� 5%   
   $
 Ctrlf�6   $ O��6&    6.   �� *
AddBlock+ 5&        5'      $ OutputBlock=�6   5)      �� 5*   
   $
 Ctrlf�6   $ M��6&    6.   �� *
AddBlock+ 5+      "  BlocksLib.Blocks.BlockMethod  5,      5.      �� 5/   
   $
 Ctrlf�6   $ I��6&    6.   �� *
AddBlock+ 50        51      $ 
InputBlock=�6   53      �� 54   
   $
 Ctrlf�6   $ R��6&    6.   �� *
AddBlock+ 55      !  BlocksLib.Blocks.BlockCycle  56         TypeName$ 
BlockCycle=�6$   58      �� 59   
   $
 Ctrlf�6   $ P��6&    6.   �� *
AddBlock+ 5:        5;      $ BlockPostCycle=�6$   5=      �� 5>   
   $
 Ctrlf�6   $ F��6&    6.   �� *
AddBlock+ 5?        5@      $ BlockForCycle=�6$   5B      �� 5C   
   $
 Ctrlf�6   $ T��6&    6.   �� *
AddBlock+ 5D        BlocksLib.Blocks.BlockIf  5E      5G      �� 5H   
   $
 Ctrlf�6   $ K��6&    6.   �� *
AddBlock+ 5I      "  BlocksLib.Blocks.SwitchBlock  5J      5L      �� 5M   
   $
 Ctrlf�6   $ Z��6&    6.   �� *Undo+�� 5N   
   $
 Ctrlf�6   $ Y��6&    6.   �� *Redo+�� 5O   
   $
 Ctrlf�6   $ C��6&    6.   �� *
CopyBlocks+�� 5P   
   $
 Ctrlf�6   $ V��6&    6.   �� *PasteBlocks+ � 	Resources 5R      ,  &BlockConverter.Tools.ConverterMultiply   multiply	 �� 
TargetType
 �� Property �� Value  DefaultSchemeHeaderTemplate �� Orientation8  2BlocksLib.Blocks.CustomControls.TextBoxNotEditableB  <BlocksLib.Blocks.CustomControls.TextBoxNotEditableExtensions   MouseDownHandled  NameSchemeStyle �� Mode*	  $Microsoft.Xaml.Behaviors.Interaction 	  Triggers+
  %Microsoft.Xaml.Behaviors.EventTrigger 
  	EventName*  $Microsoft.Xaml.Behaviors.TriggerBase   Actions8  2Microsoft.Xaml.Behaviors.Core.ChangePropertyAction   PropertyName   Value  CloseSchemeButtonStyle �� RelativeSource �� AncestorType �� Path   DefaultSchemeContentTemplate $  AddNewSchemeButtonHeaderTemplate  AddSchemeButtonStyle % !AddNewSchemeButtonContentTemplate��%�  &	        '��      &	 �     &	     &	 �    &	 '      5S   
   6"   �� 5T   
   "	 ��6   .�5U      �� 
 � ��$ 3��6&   �� 5V      
 � ��$ Stretch=�64   �� 5W      
 � ��$ Stretch=�62   5X      �� 5Y   
   .R�5Z      �� ._�5[      �� $ 
Horizontal=�6   .+�5\        	 False6.   #���  6i   ��6�   �� *RootComponent.Name+$ TwoWay=�.��6�    5]      
  5^      $ PreviewMouseDoubleClick��6-   . 5_   "     $ IsEdit��69   $
 True��6O   5`   #   5a      
  5b      $ LostKeyboardFocus��6-   . 5c   "     $ IsEdit��69   $ False��6O   5d   #   5e      .��5f   2   5g      �� 5h      #���  6   ��6N   ��  �� 
 Window$ DataContext.RemoveScheme���5i   %   ��  ��  
StackPanel$ DataContext�5j      5k      5l      �� 5m   
   .R�5n      �� $
��Auto=�6   .��5o      �� $��1��6#   ��60   �� *	RootPanel+5p      5q      �� 5r   
   .R�5s      �� ._�5t      �� #���  6   $��0q�6H   ��6S   ��  �� 
 Window$ DataContext.AddScheme�5u      5v      �� 5w   
   .R�5x      � 5y      5{      .��5|      �  � ColumnDefinitions 5}   
   �� 5~      $
��Auto �6   �� 5      $
��Auto �6   �� 5�      $��* �6   �� 5�      $
��Auto �6   �� 5�      $��* �6    � RowDefinitions 5�   
   �� 5�      $
��auto �6   �� 5�      $
��auto �6   �� 5�      $��5* �6   �� 5�      $��* �6   .+�5�   
   o� $��5��6   .@�5�      m� (   "BlockConverter.Resources.Resources   MenuFileName#��Z 6   .@�5�      m�    MenuExportToPngName#��Z 6    m� CommandParameter$	 PNG��6l    m� Command 6Q   �� *Export+6l   m� 5�         MenuSaveAsName#��Z 6    6L   �� *Save+m� 5�         MenuOpenName#��Z 6    6J   �� *Open+m� 5�         MenuCreateNewScheme#��Z 6    6Q   �� *	AddScheme+m� 5�          MenuSettingsName#��Z  6    6N   �� *OpenSettings+ 6o   ��  �� 
 Window5�      m� 5�      !   MenuEditName#��Z! 6   .@�5�      m� "   MenuAddBlockName#��Z" 6    6N   �� *
AddBlock+ 6k   �� # �� ElementName$# 	listBlock��$ SelectedItem�m� 5�      $   MenuRemoveBlockName#��Z$ 6    6Q   �� *RemoveBlock+m� 5�      %   MenuUndoName#��Z% 6    6J   �� *Undo+m� 5�      &   MenuRedoName#��Z& 6    6J   �� *Redo+m� 5�      '   MenuCopyBlocksName#��Z' 6    6P   �� *
CopyBlocks+m� 5�      (   MenuPasteBlocksName#��Z( 6    6Q   �� *PasteBlocks+m� 5�      )   MenuConvertName#��Z) 6    6M   �� *	Convert+ 5�      �� $# skriptInput��$
 Text�5�      �� 5�      ��6   �� *Instructions+* �� SelectedItem* 6<   �� *SelectInstruction+��5�      �� 5�      .R�5�      �� ��6$   �� $
 Name�5�       5�      
  5�      $ SelectionChanged��6%   . 5�      2  ,Microsoft.Xaml.Behaviors.InvokeCommandAction  +   PassEventArgsToCommand$
+ True��6Q   ,   Command, 60   �� *ReadLaguange+6Q   5�      5�      5�      �� 5�   
   $��1��6   $��2��6'   $��2��64   �� 5�   
   $��3��6   $��3��6'   $��2��61   $��2��6>   7   1BlockConverter.Tools.TabControlByItemPlanceholder  5�   
   $��2��6-   $��2��6:   $��2��6K   ��6[   �� *	Schemes+* 6{   �� *SelectScheme+$ TwoWay=���5�      =   7BlockConverter.Tools.TemplateSelectorByItemPlanceholder  5�      -   NewItemTemplate#- [ 6;   .   DefaultTemplate#. [ 6   / �� ContentTemplateSelector/ 5�        5�      #- [ 6;   #. [ 6�   5�      �� 5�   
   -   5        0 ��Name$0 	listBlock��5�      $��2��6I   $��2��6V   ��6%   �� *InputBlocks+��5�      �� 5�      "	 ��6   .�5�      �� 
 � ��$ Stretch=�6C   5�       5�      
  5�      $ MouseDoubleClick��6!   . 5�        , 6,   �� *
AddBlock+1   CommandParameter1 6I   �� $# 	listBlock��$ SelectedItem�5�      5�      �� 5�   
   $��1��6   $��2��6"   $ 
Horizontal=�62   .+�5�      ��   AddBlockButtonStyle#���  6   2   AddBlockToolTip3 � ToolTip#3 Z2 5�      ��6H   �� *
AddBlock+��5�      �� $# 	listBlock��$ SelectedItem��� 5�       	 RemoveBlockButtonStyle#��� 	 6   4   RemoveBlockToolTip#3 Z4 5�      ��6K   �� *RemoveBlock+�� 5�       
 UndoButtonStyle#��� 
 6   5   UndoBlockToolTip#3 Z5 5�      ��6I   �� *Undo+�� 5�        RedoButtonStyle#���  6   6   RedoBlockToolTip#3 Z6 5�      ��6I   �� *Redo+�� 5�        CopyButtonStyle#���  6   7   CopyBlocksToolTip#3 Z7 5�      ��6J   �� *
CopyBlocks+�� 5�        PasteButtonStyle#���  6   8   PasteBlocksToolTip#3 Z8 5�      ��6K   �� *PasteBlocks+�� 5�        ConvertButtonStyle#���  6   9   ConvertToolTip#3 Z9 5�      ��6G   �� *	Convert+��5�      �� $# skriptInput��$
 Text��� 5�      5�      �� 5�   
   $
��Auto=�6   $��4��6<   $��2��6L   .��5�      �� -   5        : ��Name$: skriptInput��5�      ; �� TextWrapping$
; Wrap=�6+   < �� AcceptsReturn$
< True��6?   5�      �� 5�   
   $��3��6   $��4��6$   .��5�      �� $
��Wrap=�60   ��6   �� *Dialog+��6D   �� *DialogColor+5�      5�      5�      