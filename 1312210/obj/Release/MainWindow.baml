   M S B A M L   `   `   `  ����   1312210    _1312210.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  Gomoku��$	��900��$
��1280�� � 	Resources    brushWatermarkBackground �� Color  brushWatermarkForeground  brushWatermarkBorder   BooleanToVisibilityConverter  EntryFieldStyle �� 
TargetType �� Property �� Value��%�   &	        &	      &	 +     &	 =     &	 B     �� $ White���� $ LightSteelBlue���� $ Indigo���� �� " �.���  � ��$ Stretch=���  � ��$ Center=���  � ��$
 20,0q�.��� .+�(�  (� Stretch$ Uniform=�$��Right=�.���� -    ��Name$ boardCanvas��
���   �$	��600��$	��600��$��50,100,600,50q�$��Right=�.+��� $*��$GOMOKU - FIVE IN A ROW - MULTIPLAYER��$��69��$	��699��$��36(����	#FFEEEEEE �� Left$	 250��
	 �� Top$
	 -105��� $��0��#��[  #��[ $	 775��$��0q�$	 1��.+��� $	��5,2q�$��Guest��$��20(�#��[ y��� 
 �� ElementName$
 tb_name�� �� Path$ Text.IsEmpty� �� 	Converter# [ �� -    ��Name$ tb_name��$��31��$	��260��$	 768��$��20(�
������ #��[ �� $��
Your Name:��$��40��$	��112��$��20(�
�������$	 651��$	 -4���� -    ��Name$ btn_name��$��31��$��85��$
 1052��$��Change!��$��20(����	#FFEEEEEE�� $
��Chat��$��40��$	��486��$��20(����#337AB7$	 651��$	 50��
��������� -    ��Name$ rtb_conversation��$	��386��$	��486��$	 651��$	 90�� �� 
IsReadOnly .  �� 
LineHeight$ 3��$��20(�" �� VerticalScrollBarVisibility$ Visible=��� -   $ btn_send��$��31��$��85��$
 1040��$
��Send��$��20(����#F0AD4E
�������$		 490��� $��0��#��[  #��[ $	 657��$��0q�$		 490��.+��� $	��5,2q�$��Type your message here...��$��20(�#��[ y��� $
 
tb_message��$ Text.IsEmpty�# [ �� -   $ 
tb_message��$��31��$	��371��$	 657��$��20(�$		 491��
������ #��[ 