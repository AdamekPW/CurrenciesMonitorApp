   M S B A M L   `   `   `  ���� A  =Waluty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WalutyWaluty      Waluty.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     R NOxyPlot.Wpf, Version=2.1.2.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0Y UOxyPlot.Wpf.Shared, Version=2.1.2.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0"oxyhttp://oxyplot.org/wpf   "localclr-namespace:Waluty   -     � Title$  Currency monitor��$	��450��$	��800�� � DataContext    Waluty.MainViewModel   � 	Resources    Waluty.IconConverter   iconConverter��%   &	          .��� ���#21201f$��4 4 4 4q�.+��  � ColumnDefinitions �� $��0.34* ��� $��0.66* �.+�� $��0��$��2 0 0 0q� � RowDefinitions �� $
��0.1* ��� $
��0.1* ��� $
��0.8* �.+�� $��0��$��0 2 0 2q� �� $
��0.5* ��� $
��0.5* �.+��� $��0��
����İ�$��0 1 0 0q�._��� $��20(�$��Center=�$��Center=����� *" AdditionalDataInfo.effectiveDate+�� $��1��
����İ�._��� -   $��UpdateDataButton��$��16(�$
��Bold"�$��37��$��Center=�
������ ���� *UpdateCommand+.��Update� $��1��
������� �� �� �� $��43 �.+��� $��0��._��� $��Currency��$��20(� �� TextAlignment$ Center=�$��Center=�$
��Bold"��� $��1��._��� $��Value [PLN]��$��20(�$ Center=�$��Center=�$
��Bold"��� $��2��$��4q�
�������._��� $
��Auto=�$��Disabled=�.���� ���� *CurrencyList+���� .R��� 
���   �$��1q� �� CornerRadius$ 6��._��  �� $��0.45* ��� $��0.45* ��� $
��0.1* �.+��� $ 7��$��1q�$��0�� ^� InitialShowDelay$ 0��
������� � ToolTip �� *
currency+._��� $��20(�$ Center=�$
��Bold"�$��Arial)�$��Center=�$��Center=����� *code+�� $ 7��$��1q�$��1��
�������._��� $��20(�$ Center=�$��Center=�$��Center=�$
��Bold"����� *mid+�� $��2��$��1q�$��Center=�$��Center=�._��� -   $��Stretch=�$��Stretch=�
����������� 	 d� 	IsChecked	 �� *	IsChecked+

 � Tag
 �� *code+� $��1��$��0 0 4 0q� �� $��0.08* ��� $��0.12* ��� $
��0.8* �.+�� -    �Name$ ChoosingGraph��$��0��.+��� $��3 0 0 0q�._��� ���� *
ChartsList+���� .R�� ���� .R��� $	��101��$��35��
����ح�._��� -   
������ $��16(�$
��Bold"�
���   �$��Stretch=�$��Stretch=����� *name+� $��1��
�������$��4 1 0 1q� �� �� �� .+��� $��0��$��4 0 0 0q�._��  �� $
��0.8* ��� $
��0.2* �.+�� $��0�� �� $	��30* ��� $	��37* � �� �� .+��� $��0��$��15(�$��DemiBold"�$ Center=�$��2��$��0,0,0,19q�$��2������ *AllChartStats[0].name+�� $��1��$&  Comparison with the previous day��$ 0��$��15(�$��DemiBold"�$ Center=�$��2������ *ChartStatsTextColor+���� *AllChartStats[0].difference+� $��1��.+��� $��Stretch=�$��Stretch=����� *	ArrowIcon+ �� 	Converter# [  �� $��1��
���   �$��1 0 1 0q�._��  �� �� .+��� $��0��$��Last 30 days��$ Center=�$��DemiBold"�$��15(�� $��1�� �� �� .+��� $��1��$ Center=�$��15(����� *AllChartStats[1].difference+�� $��0��$ Center=�$��15(����� *AllChartStats[1].name+�� $��2��._��  �� �� .+��� $��0��$��Last 90 days��$ Center=�$��DemiBold"�$��15(�� $��1�� �� �� .+��� $��0��$ Center=�$��15(����� *AllChartStats[2].name+�� $��1��$ Center=�$��15(����� *AllChartStats[2].difference+� $��2��$��4 0 0 0q�.+�  OxyPlot.Wpf.PlotView  -     Name$ myPlot��  OxyPlot.Wpf.PlotViewBase   Model �� *	CostModel+