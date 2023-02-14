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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ColorPicker.UserControls
{
	/// <summary>
	/// Interaction logic for ColorItem.xaml
	/// </summary>
	public partial class ColorItem : UserControl
	{
		string HexColor { get; init; }
		RGB RgbColor { get; set; }
		public ColorItem(string hexColor)
		{
			InitializeComponent();
			HexColor = hexColor;

			InitUI();
		}

		private void InitUI()
		{
			RgbColor = ColorHelper.ColorConverter.HexToRgb(new(HexColor));
			Color color = Color.FromRgb(RgbColor.R, RgbColor.G, RgbColor.B);

			ColorBorder.Background = new SolidColorBrush { Color = color };
			ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = color };
			
			RgbTxt.Text = $"{RgbColor.R}; {RgbColor.G}; {RgbColor.B}"; // Set text
			HEXTxt.Text = HexColor; // Set text
		}

		private void ColorBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Clipboard.SetText($"{RgbColor.R}; {RgbColor.G}; {RgbColor.B}");
		}
	}
}
