   M S B A M L   `   `   `  ����I  EBlockConverter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:BlockConverterBlockConverter  5   	   =#clr-namespace:BlockConverter.BlocksBlockConverter.Blocks  5   	   K*clr-namespace:BlockConverter.Blocks.ShapesBlockConverter.Blocks.Shapes  5   	   W0clr-namespace:BlockConverter.Blocks.ChildsBlocks"BlockConverter.Blocks.ChildsBlocks  5	   	       BlockConverter.MainWindow   5   	   \ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:BlockConverter     5   	   6blocks#clr-namespace:BlockConverter.Blocks     5   	   =shapes*clr-namespace:BlockConverter.Blocks.Shapes     5   	   HchildBlocks0clr-namespace:BlockConverter.Blocks.ChildsBlocks     5	   	     � Title$  
MainWindow��5   	   $	��800��6   $	��800��6)    � 	Resources 5      -  'BlockConverter.Blocks.ConverterMultiply   multiply��%   &	          5   
   6#   5      .��5      �  � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5       � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      �� 5      $
��auto �6   .+�5   
   o� $��2��6   .@�5      m� $
��File��6   .@�5      m� $��ExportToPng��6   5      5      �� 5    
   $
��Auto=�6   $
��Auto=�6<   $��1��6_   $��1��6o   .��5!      � -   5         �Name$
 root��5!      5"      �� 5#   
   -   5         ��Name$ 	listBlock��5#      $��1��6%   .@�5$      !   BlockConverter.Blocks.Block    
blockStyle#���  6   $��Center=�6@   #   BlockConverter.Blocks.BlockIf  5%        blockCycleBaseStyle#���  6   $��Center=�6K   &    BlockConverter.Blocks.BlockCycle  5&      #���  6    $��Center=�6N   *   $BlockConverter.Blocks.BlockPostCycle  5'      #���  6$   $��Center=�6R   5(      �� 5)   
   $��2��6   .+�5*      �� $	��Add��6   ��6#   �� *
AddBlock+��6@   ��  �� ElementName$ 	listBlock�� �� Path$ SelectedItem��� 5+      $��Remove��6   ��6&   �� *RemoveBlock+5,      5-      5/      