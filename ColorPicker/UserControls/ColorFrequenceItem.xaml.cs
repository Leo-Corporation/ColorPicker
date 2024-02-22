/*
MIT License

Copyright (c) Léo Corporation

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. 
*/

using ColorHelper;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorPicker.UserControls
{
	/// <summary>
	/// Interaction logic for ColorFrequenceItem.xaml
	/// </summary>
	public partial class ColorFrequenceItem : UserControl
	{
		public RGB Color { get; }
		public int Freq { get; }

		public ColorFrequenceItem(RGB color, int freq)
		{
			InitializeComponent();
			Color = color;
			Freq = freq;
			InitUI();
		}

		private void InitUI()
		{
			ColorBorder.Background = new SolidColorBrush { Color = System.Windows.Media.Color.FromRgb(Color.R, Color.G, Color.B) };
			ColorTxt.Text = $"#{ColorHelper.ColorConverter.RgbToHex(Color).Value}";
			FreqTxt.Text = Freq.ToString();
		}

		private void ColorBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{

		}
	}
}
