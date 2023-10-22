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
using ColorPicker.Classes;
using ColorPicker.Enums;
using System;
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
		ColorInfo ColorInfo { get; set; }
		public ColorItem(string hexColor)
		{
			InitializeComponent();
			HexColor = hexColor;
			ColorInfo = new(ColorHelper.ColorConverter.HexToRgb(new(HexColor)));

			InitUI();
		}

		private void InitUI()
		{
			Color color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B);

			ColorBorder.Background = new SolidColorBrush { Color = color };
			ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = color };
			ColorBorder.ToolTip = new ToolTip()
			{
				Background = new SolidColorBrush { Color = Global.GetColorFromResource("Background1") },
				Foreground = new SolidColorBrush { Color = Global.GetColorFromResource("Foreground1") },
				Content = ColorInfo.ToString()
			};

			RgbTxt.Text = $"{ColorInfo.RGB.R}; {ColorInfo.RGB.G}; {ColorInfo.RGB.B}"; // Set text
			HEXTxt.Text = HexColor; // Set text
		}

		private void ColorBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Clipboard.SetText(Global.Settings.DefaultColorType switch
			{
				ColorTypes.HEX => ColorInfo.HEX.Value,
				ColorTypes.HSV => $"{ColorInfo.HSV.H},{ColorInfo.HSV.S},{ColorInfo.HSV.V}",
				ColorTypes.HSL => $"{ColorInfo.HSL.H},{ColorInfo.HSL.S},{ColorInfo.HSL.L}",
				ColorTypes.CMYK => $"{ColorInfo.CMYK.C},{ColorInfo.CMYK.M},{ColorInfo.CMYK.Y},{ColorInfo.CMYK.K}",
				ColorTypes.XYZ => $"{ColorInfo.XYZ.X}; {ColorInfo.XYZ.Y}; {ColorInfo.XYZ.Z}",
				ColorTypes.YIQ => $"{ColorInfo.YIQ.Y}; {ColorInfo.YIQ.I}; {ColorInfo.YIQ.Q}",
				ColorTypes.YUV => $"{ColorInfo.YUV.Y}; {ColorInfo.YUV.U}; {ColorInfo.YUV.V}",
				ColorTypes.DEC => ColorInfo.DEC.Value.ToString(),
				_ => $"{ColorInfo.RGB.R};{ColorInfo.RGB.G};{ColorInfo.RGB.B}"
			});
		}

		private void DeleteBtn_Click(object sender, RoutedEventArgs e)
		{
			Global.Bookmarks.ColorBookmarks.Remove(HexColor);
			Global.BookmarksPage.ColorsBookmarks.Children.Remove(this);
			Global.SelectorPage.LoadDetails();
			Global.ConverterPage.LoadDetails();
		}

		public static event EventHandler<PageEventArgs> GoClick;

		private void GoBtn_Click(object sender, RoutedEventArgs e)
		{
			Global.SelectorPage.RedSlider.Value = ColorInfo.RGB.R;
			Global.SelectorPage.GreenSlider.Value = ColorInfo.RGB.G;
			Global.SelectorPage.BlueSlider.Value = ColorInfo.RGB.B;

			GoClick?.Invoke(sender, new(AppPages.Selector));
		}

		private void CopyRGB_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText($"{ColorInfo.RGB.R}; {ColorInfo.RGB.G}; {ColorInfo.RGB.B}");
		}

		private void CopyHEX_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText($"#{ColorInfo.HEX.Value}");
		}

		private void CopyHSV_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText($"{ColorInfo.HSV.H}, {ColorInfo.HSV.S}, {ColorInfo.HSV.V}");
		}

		private void CopyHSL_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText($"{ColorInfo.HSL.H}, {ColorInfo.HSL.S}, {ColorInfo.HSL.L}");
		}

		private void CopyCMYK_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText($"{ColorInfo.CMYK.C}, {ColorInfo.CMYK.M}, {ColorInfo.CMYK.Y}, {ColorInfo.CMYK.K}");
		}

		private void CopyXYZ_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText($"{ColorInfo.XYZ.X}; {ColorInfo.XYZ.Y}; {ColorInfo.XYZ.Z}");
		}

		private void CopyYIQ_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText($"{ColorInfo.YIQ.Y}; {ColorInfo.YIQ.I}; {ColorInfo.YIQ.Q}");
		}

		private void CopyYUV_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText($"{ColorInfo.YUV.Y}; {ColorInfo.YUV.U}; {ColorInfo.YUV.V}");
		}

		private void CopyDEC_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(ColorInfo.DEC.Value.ToString());
		}

		private void MoreBtn_Click(object sender, RoutedEventArgs e)
		{
			MorePopup.IsOpen = true;
        }

		private void EditConverterBtn_Click(object sender, RoutedEventArgs e)
		{
			Global.ConverterPage.ColorInfo = ColorInfo;
			Global.ConverterPage.LoadDetails(true);
			GoClick?.Invoke(sender, new(AppPages.Converter));
		}

		private void EditPaletteBtn_Click(object sender, RoutedEventArgs e)
		{
			Global.PalettePage.ColorInfo = ColorInfo;
			Global.PalettePage.InitPaletteUI(true);
			GoClick?.Invoke(sender, new PageEventArgs(AppPages.ColorPalette));
		}
	}
}
