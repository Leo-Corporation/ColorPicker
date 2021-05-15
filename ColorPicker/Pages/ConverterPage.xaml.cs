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
using ColorPicker.Classes;
using ColorPicker.Enums;
using LeoCorpLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorPicker.Pages
{
	/// <summary>
	/// Interaction logic for ConverterPage.xaml
	/// </summary>
	public partial class ConverterPage : Page
	{
		string rgbColor, hexColor, hsvColor, hslColor, cmykColor = "";
		public ConverterPage()
		{
			InitializeComponent();
			InitUI(); // Init UI
		}

		private void InitUI()
		{
			ColorTypeComboBox.Items.Add(Global.ColorTypesToString(ColorTypes.RGB)); // Add
			ColorTypeComboBox.Items.Add(Global.ColorTypesToString(ColorTypes.HEX)); // Add
			ColorTypeComboBox.Items.Add(Global.ColorTypesToString(ColorTypes.HSV)); // Add
			ColorTypeComboBox.Items.Add(Global.ColorTypesToString(ColorTypes.HSL)); // Add
			ColorTypeComboBox.Items.Add(Global.ColorTypesToString(ColorTypes.CMYK)); // Add
			ColorTypeComboBox.SelectedIndex = 0; // Set index

			// Generate random color
			Random random = new(); // Create new random
			int r = random.Next(0, 255); // Generate random number between 0 and 255
			int g = random.Next(0, 255); // Generate random number between 0 and 255
			int b = random.Next(0, 255); // Generate random number between 0 and 255
			ColorTxt.Text = $"{r};{g};{b}"; // Set text
		}

		private void ColorTxt_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!string.IsNullOrEmpty(ColorTxt.Text))
			{
				try
				{
					if (ColorTypeComboBox.Text == Properties.Resources.RGB)
					{
						string[] rgb = ColorTxt.Text.Split(new string[] { ";" }, StringSplitOptions.None); // Split
						var hsv = ColorsConverter.RGBtoHSV(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2])); // Convert
						var hsl = ColorHelper.ColorConverter.RgbToHsl(new((byte)int.Parse(rgb[0]), (byte)int.Parse(rgb[1]), (byte)int.Parse(rgb[2])));
						var cmyk = ColorHelper.ColorConverter.RgbToCmyk(new((byte)int.Parse(rgb[0]), (byte)int.Parse(rgb[1]), (byte)int.Parse(rgb[2])));

						RGBTxt.Text = $"{Properties.Resources.RGB} {rgb[0]};{rgb[1]};{rgb[2]}"; // Set text
						HEXTxt.Text = $"{Properties.Resources.HEX} #{ColorsConverter.RGBtoHEX(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2])).Value}"; // Set text
						HSVTxt.Text = $"{Properties.Resources.HSV} ({hsv.Hue},{hsv.Saturation},{hsv.Value})"; // Set text
						HSLTxt.Text = $"{Properties.Resources.HSL} ({hsl.H},{hsl.S},{hsl.L})"; // Set text
						CMYKTxt.Text = $"{Properties.Resources.CMYK} {cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}";

						rgbColor = $"{rgb[0]};{rgb[1]};{rgb[2]}"; // Set text
						hexColor = $"#{ColorsConverter.RGBtoHEX(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2])).Value}"; // Set text
						hsvColor = $"({hsv.Hue},{hsv.Saturation},{hsv.Value})"; // Set
						hslColor = $"({hsl.H},{hsl.S},{hsl.L})"; // Set
						cmykColor = $"{cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}"; // Set
					}
					else if (ColorTypeComboBox.Text == Properties.Resources.HEX)
					{
						var rgb = ColorsConverter.HEXtoRGB(new() { Value = ColorTxt.Text }); // Convert
						string hex = ColorTxt.Text.StartsWith("#") ? ColorTxt.Text : "#" + ColorTxt.Text; // Set
						var hsv = ColorsConverter.RGBtoHSV(rgb); // Convert
						var hsl = ColorHelper.ColorConverter.HexToHsl(new(hex)); // Convert
						var cmyk = ColorHelper.ColorConverter.HexToCmyk(new(hex));

						RGBTxt.Text = $"{Properties.Resources.RGB} {rgb.R};{rgb.G};{rgb.B}"; // Set text
						HEXTxt.Text = $"{Properties.Resources.HEX} {hex}"; // Set text
						HSVTxt.Text = $"{Properties.Resources.HSV} ({hsv.Hue},{hsv.Saturation},{hsv.Value})"; // Set text
						HSLTxt.Text = $"{Properties.Resources.HSL} ({hsl.H},{hsl.S},{hsl.L})"; // Set text
						CMYKTxt.Text = $"{Properties.Resources.CMYK} {cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}";

						rgbColor = $"{rgb.R};{rgb.G};{rgb.B}"; // Set text
						hexColor = $"{hex}"; // Set text
						hsvColor = $"({hsv.Hue},{hsv.Saturation},{hsv.Value})"; // Set
						hslColor = $"({hsl.H},{hsl.S},{hsl.L})"; // Set
						cmykColor = $"{cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}"; // Set
					}

					string[] rC = rgbColor.Split(new string[] { ";" }, StringSplitOptions.None); // Split
					ColorDisplayer.Background = new SolidColorBrush { Color = Color.FromRgb((byte)int.Parse(rC[0]), (byte)int.Parse(rC[1]), (byte)int.Parse(rC[2])) };

					IconValidMsgTxt.Foreground = new SolidColorBrush { Color = Color.FromRgb(0, 223, 57) }; // Set foreground color
					IconValidMsgTxt.Text = "\uF299"; // Set icon
					ValidMsgTxt.Text = Properties.Resources.ColorValid; // Set text
				}
				catch
				{
					IconValidMsgTxt.Foreground = new SolidColorBrush { Color = Color.FromRgb(255, 69, 0) }; // Set foreground color
					IconValidMsgTxt.Text = "\uF36E"; // Set icon
					ValidMsgTxt.Text = Properties.Resources.InvalidColor; // Set text
				} 
			}
		}

		private void CopyBtn_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(rgbColor); // Copy
		}

		private void HueTxt_TextChanged(object sender, TextChangedEventArgs e)
		{
			ConvertHSV(); // Convert HSV color
		}

		private void SatTxt_TextChanged(object sender, TextChangedEventArgs e)
		{
			ConvertHSV(); // Convert HSV color
		}

		private void ValTxt_TextChanged(object sender, TextChangedEventArgs e)
		{
			ConvertHSV(); // Convert HSV color
		}

		private void ConvertHSV()
		{
			try
			{
				int h = int.Parse(HueTxt.Text); // Parse
				int s = int.Parse(SatTxt.Text); // Parse
				int v = int.Parse(ValTxt.Text); // Parse
				var rgb = ColorHelper.ColorConverter.HsvToRgb(new(h, (byte)s, (byte)v)); // Convert
				var hex = ColorsConverter.RGBtoHEX(rgb.R, rgb.G, rgb.B); // Convert
				var hsl = ColorHelper.ColorConverter.RgbToHsl(rgb); // Convert
				var cmyk = ColorHelper.ColorConverter.HsvToCmyk(new(h, (byte)s, (byte)v));

				RGBTxt.Text = $"{Properties.Resources.RGB} {rgb.R};{rgb.G};{rgb.B}"; // Set text
				HEXTxt.Text = $"{Properties.Resources.HEX} #{hex.Value}"; // Set text
				HSVTxt.Text = $"{Properties.Resources.HSV} ({h},{s},{v})"; // Set text
				HSLTxt.Text = $"{Properties.Resources.HSL} ({hsl.H},{hsl.S},{hsl.L})"; // Set text
				CMYKTxt.Text = $"{Properties.Resources.CMYK} {cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}";

				rgbColor = $"{rgb.R};{rgb.G};{rgb.B}"; // Set text
				hexColor = $"{hex}"; // Set text
				hsvColor = $"({h},{s},{v})"; // Set
				hslColor = $"({hsl.H},{hsl.S},{hsl.L})"; // Set
				cmykColor = $"{cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}"; // Set

				ColorDisplayer.Background = new SolidColorBrush { Color = Color.FromRgb(rgb.R, rgb.G, rgb.B) }; // Set color

				IconValidMsgTxt.Foreground = new SolidColorBrush { Color = Color.FromRgb(0, 223, 57) }; // Set foreground color
				IconValidMsgTxt.Text = "\uF299"; // Set icon
				ValidMsgTxt.Text = Properties.Resources.ColorValid; // Set text
			}
			catch
			{
				IconValidMsgTxt.Foreground = new SolidColorBrush { Color = Color.FromRgb(255, 69, 0) }; // Set foreground color
				IconValidMsgTxt.Text = "\uF36E"; // Set icon
				ValidMsgTxt.Text = Properties.Resources.InvalidColor; // Set text
			}
		}

		private void ConvertHSL()
		{
			try
			{
				int h = int.Parse(HTxt.Text); // Parse
				int s = int.Parse(STxt.Text); // Parse
				int l = int.Parse(LTxt.Text); // Parse
				var rgb = ColorHelper.ColorConverter.HslToRgb(new(h, (byte)s, (byte)l)); // Convert
				var hex = ColorsConverter.RGBtoHEX(rgb.R, rgb.G, rgb.B); // Convert
				var hsv = ColorHelper.ColorConverter.RgbToHsv(rgb); // Convert
				var cmyk = ColorHelper.ColorConverter.HslToCmyk(new(h, (byte)s, (byte)l));

				RGBTxt.Text = $"{Properties.Resources.RGB} {rgb.R};{rgb.G};{rgb.B}"; // Set text
				HEXTxt.Text = $"{Properties.Resources.HEX} #{hex.Value}"; // Set text
				HSVTxt.Text = $"{Properties.Resources.HSV} ({hsv.H},{hsv.S},{hsv.V})"; // Set text
				HSLTxt.Text = $"{Properties.Resources.HSL} ({h},{s},{l})"; // Set text
				CMYKTxt.Text = $"{Properties.Resources.CMYK} {cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}";

				hslColor = $"({h},{s},{l})"; // Set
				hsvColor = $"({hsv.H},{hsv.S},{hsv.V})"; // Set
				rgbColor = $"{rgb.R};{rgb.G};{rgb.B}"; // Set
				hexColor = $"#{hex.Value}"; // Set
				cmykColor = $"{cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}"; // Set

				ColorDisplayer.Background = new SolidColorBrush { Color = Color.FromRgb(rgb.R, rgb.G, rgb.B) }; // Set color

				IconValidMsgTxt.Foreground = new SolidColorBrush { Color = Color.FromRgb(0, 223, 57) }; // Set foreground color
				IconValidMsgTxt.Text = "\uF299"; // Set icon
				ValidMsgTxt.Text = Properties.Resources.ColorValid; // Set text
			}
			catch
			{
				IconValidMsgTxt.Foreground = new SolidColorBrush { Color = Color.FromRgb(255, 69, 0) }; // Set foreground color
				IconValidMsgTxt.Text = "\uF36E"; // Set icon
				ValidMsgTxt.Text = Properties.Resources.InvalidColor; // Set text
			}
		}

		private void ConvertCMYK()
		{
			try
			{
				int c = int.Parse(CTxt.Text);
				int m = int.Parse(MTxt.Text);
				int y = int.Parse(YTxt.Text);
				int k = int.Parse(KTxt.Text);
				var rgb = ColorHelper.ColorConverter.CmykToRgb(new((byte)c, (byte)m, (byte)y, (byte)k)); // Convert color
				var hex = ColorHelper.ColorConverter.CmykToHex(new((byte)c, (byte)m, (byte)y, (byte)k)); // Convert color
				var hsl = ColorHelper.ColorConverter.CmykToHsl(new((byte)c, (byte)m, (byte)y, (byte)k)); // Convert color
				var hsv = ColorHelper.ColorConverter.CmykToHsv(new((byte)c, (byte)m, (byte)y, (byte)k)); // Convert color

				RGBTxt.Text = $"{Properties.Resources.RGB} {rgb.R};{rgb.G};{rgb.B}"; // Set text
				HEXTxt.Text = $"{Properties.Resources.HEX} #{hex.Value}"; // Set text
				HSVTxt.Text = $"{Properties.Resources.HSV} ({hsv.H},{hsv.S},{hsv.V})"; // Set text
				HSLTxt.Text = $"{Properties.Resources.HSL} ({hsl.H},{hsl.S},{hsl.L})"; // Set text
				CMYKTxt.Text = $"{Properties.Resources.CMYK} {c},{m},{y},{k}"; // Set text

				hslColor = $"({hsl.H},{hsl.S},{hsl.L})"; // Set
				hsvColor = $"({hsv.H},{hsv.S},{hsv.V})"; // Set
				rgbColor = $"{rgb.R};{rgb.G};{rgb.B}"; // Set
				hexColor = $"#{hex.Value}"; // Set
				cmykColor = $"{c},{m},{y},{k}"; // Set

				ColorDisplayer.Background = new SolidColorBrush { Color = Color.FromRgb(rgb.R, rgb.G, rgb.B) }; // Set color

				IconValidMsgTxt.Foreground = new SolidColorBrush { Color = Color.FromRgb(0, 223, 57) }; // Set foreground color
				IconValidMsgTxt.Text = "\uF299"; // Set icon
				ValidMsgTxt.Text = Properties.Resources.ColorValid; // Set text
			}
			catch
			{
				IconValidMsgTxt.Foreground = new SolidColorBrush { Color = Color.FromRgb(255, 69, 0) }; // Set foreground color
				IconValidMsgTxt.Text = "\uF36E"; // Set icon
				ValidMsgTxt.Text = Properties.Resources.InvalidColor; // Set text
			}
		}

		private void CopyHSVBtn_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(hsvColor); // Copy
		}

		private void HTxt_TextChanged(object sender, TextChangedEventArgs e)
		{
			ConvertHSL();
		}

		private void STxt_TextChanged(object sender, TextChangedEventArgs e)
		{
			ConvertHSL();
		}

		private void LTxt_TextChanged(object sender, TextChangedEventArgs e)
		{
			ConvertHSL();
		}

		private void CTxt_TextChanged(object sender, TextChangedEventArgs e)
		{
			ConvertCMYK();
		}

		private void CopyHSLBtn_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(hslColor);
		}

		private void CopyHEXBtn_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(hexColor); // Copy
		}

		private void ColorTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ColorTxt.Text = ""; // Clear
			ColorTxt.MaxLength = ColorTypeComboBox.SelectedIndex switch
			{
				0 => 11,
				1 => 7,
				_ => 11
			}; // Set max length

			HueTxt.Text = ""; // Clear
			SatTxt.Text = ""; // Clear
			ValTxt.Text = ""; // Clear

			HTxt.Text = ""; // Clear
			STxt.Text = ""; // Clear
			LTxt.Text = ""; // Clear

			CTxt.Text = ""; // Clear
			MTxt.Text = ""; // Clear
			YTxt.Text = ""; // Clear
			KTxt.Text = ""; // Clear

			switch (ColorTypeComboBox.SelectedIndex)
			{
				case 0: // RGB
					ColorTxt.Visibility = Visibility.Visible; // Show
					HSVGrid.Visibility = Visibility.Collapsed; // Hide
					HSLGrid.Visibility = Visibility.Collapsed; // Hide
					CMYKGrid.Visibility = Visibility.Collapsed; // Hide
					break;
				case 1: // HEX
					ColorTxt.Visibility = Visibility.Visible; // Show
					HSVGrid.Visibility = Visibility.Collapsed; // Hide
					HSLGrid.Visibility = Visibility.Collapsed; // Hide
					CMYKGrid.Visibility = Visibility.Collapsed; // Hide
					break;
				case 2: // HSV
					ColorTxt.Visibility = Visibility.Collapsed; // Hide
					HSVGrid.Visibility = Visibility.Visible; // Show
					HSLGrid.Visibility = Visibility.Collapsed; // Hide
					CMYKGrid.Visibility = Visibility.Collapsed; // Hide
					break;
				case 3: // HSL
					ColorTxt.Visibility = Visibility.Collapsed; // Hide
					HSVGrid.Visibility = Visibility.Collapsed; // Hide
					HSLGrid.Visibility = Visibility.Visible; // Show
					CMYKGrid.Visibility = Visibility.Collapsed; // Hide
					break;
				case 4: // CMYK
					ColorTxt.Visibility = Visibility.Collapsed; // Hide
					HSVGrid.Visibility = Visibility.Collapsed; // Hide
					HSLGrid.Visibility = Visibility.Collapsed; // Hide
					CMYKGrid.Visibility = Visibility.Visible; // Show
					break;
			}
		}
	}
}
