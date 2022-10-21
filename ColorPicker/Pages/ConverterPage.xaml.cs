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
using LeoCorpLibrary;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorPicker.Pages;

/// <summary>
/// Interaction logic for ConverterPage.xaml
/// </summary>
public partial class ConverterPage : Page
{
	string rgbColor, hexColor, hsvColor, hslColor, cmykColor, yiqColor, xyzColor = "";
	readonly string sep = Global.Settings.RGBSeparator; // Set
	readonly bool u = Global.Settings.HEXUseUpperCase.Value; // Set
	public ConverterPage()
	{
		InitializeComponent();
		InitUI(); // Init UI
	}

	private void InitUI()
	{
		// Load ColorTypeComboBox
		for (int i = 0; i < Enum.GetValues(typeof(ColorTypes)).Length; i++)
		{
			ColorTypeComboBox.Items.Add(Global.ColorTypesToString((ColorTypes)i)); // Add
		}

		ColorTypeComboBox.SelectedIndex = (int)Global.Settings.FavoriteColorType; // Set index

		// Generate random color
		Random random = new(); // Create new random
		int r = random.Next(0, 255); // Generate random number between 0 and 255
		int g = random.Next(0, 255); // Generate random number between 0 and 255
		int b = random.Next(0, 255); // Generate random number between 0 and 255

		switch ((ColorTypes)ColorTypeComboBox.SelectedIndex)
		{
			case ColorTypes.RGB:
				ColorTxt.Text = $"{r}{sep}{g}{sep}{b}"; // Set text
				break;
			case ColorTypes.HEX:
				ColorTxt.Text = $"#{ColorHelper.ColorConverter.RgbToHex(new((byte)r, (byte)g, (byte)b))}";
				break;
			case ColorTypes.HSV:
				var hsv = ColorHelper.ColorConverter.RgbToHsv(new((byte)r, (byte)g, (byte)b)); // Convert
				HueTxt.Text = hsv.H.ToString(); // Set text
				SatTxt.Text = hsv.S.ToString(); // Set text
				ValTxt.Text = hsv.V.ToString(); // Set text
				break;
			case ColorTypes.HSL:
				var hsl = ColorHelper.ColorConverter.RgbToHsl(new((byte)r, (byte)g, (byte)b)); // Convert
				HTxt.Text = hsl.H.ToString(); // Set text
				STxt.Text = hsl.S.ToString(); // Set text
				LTxt.Text = hsl.L.ToString(); // Set text
				break;
			case ColorTypes.CMYK:
				var cmyk = ColorHelper.ColorConverter.RgbToCmyk(new((byte)r, (byte)g, (byte)b)); // Convert
				CTxt.Text = cmyk.C.ToString(); // Set text
				MTxt.Text = cmyk.M.ToString(); // Set text
				YTxt.Text = cmyk.Y.ToString(); // Set text
				KTxt.Text = cmyk.K.ToString(); // Set text
				break;
			case ColorTypes.XYZ:
				var xyz = ColorHelper.ColorConverter.RgbToXyz(new((byte)r, (byte)g, (byte)b)); // Convert
				XTxt.Text = xyz.X.ToString();
				XYTxt.Text = xyz.Y.ToString();
				ZTxt.Text = xyz.Z.ToString();
				break;
			case ColorTypes.YIQ:
				var yiq = ColorHelper.ColorConverter.RgbToYiq(new((byte)r, (byte)g, (byte)b)); // Convert
				YQTxt.Text = yiq.Y.ToString();
				ITxt.Text = yiq.I.ToString();
				QTxt.Text = yiq.Q.ToString();
				break;
		}
	}

	private void ColorTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (!string.IsNullOrEmpty(ColorTxt.Text))
		{
			try
			{
				if (ColorTypeComboBox.Text == Properties.Resources.RGB)
				{
					string[] rgb = ColorTxt.Text.Split(new string[] { sep }, StringSplitOptions.None); // Split
					var hsv = ColorsConverter.RGBtoHSV(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2])); // Convert
					var hsl = ColorHelper.ColorConverter.RgbToHsl(new((byte)int.Parse(rgb[0]), (byte)int.Parse(rgb[1]), (byte)int.Parse(rgb[2])));
					var cmyk = ColorHelper.ColorConverter.RgbToCmyk(new((byte)int.Parse(rgb[0]), (byte)int.Parse(rgb[1]), (byte)int.Parse(rgb[2])));
					var xyz = ColorHelper.ColorConverter.RgbToXyz(new((byte)int.Parse(rgb[0]), (byte)int.Parse(rgb[1]), (byte)int.Parse(rgb[2])));
					var yiq = ColorHelper.ColorConverter.RgbToYiq(new((byte)int.Parse(rgb[0]), (byte)int.Parse(rgb[1]), (byte)int.Parse(rgb[2])));

					RGBTxt.Text = $"{Properties.Resources.RGB} {rgb[0]}{sep}{rgb[1]}{sep}{rgb[2]}"; // Set text
					HEXTxt.Text = $"{Properties.Resources.HEX} #{(u ? ColorsConverter.RGBtoHEX(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2])).Value.ToUpper() : ColorsConverter.RGBtoHEX(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2])).Value)}"; // Set text
					HSVTxt.Text = $"{Properties.Resources.HSV} ({hsv.Hue},{hsv.Saturation},{hsv.Value})"; // Set text
					HSLTxt.Text = $"{Properties.Resources.HSL} ({hsl.H},{hsl.S},{hsl.L})"; // Set text
					CMYKTxt.Text = $"{Properties.Resources.CMYK} {cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}";
					XYZTxt.Text = $"{Properties.Resources.XYZ}\n{Global.GetXyzString(xyz)}";
					YIQTxt.Text = $"{Properties.Resources.YIQ}\n{Global.GetYiqString(yiq)}";

					rgbColor = $"{rgb[0]}{sep}{rgb[1]}{sep}{rgb[2]}"; // Set text
					hexColor = $"#{(u ? ColorsConverter.RGBtoHEX(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2])).Value.ToUpper() : ColorsConverter.RGBtoHEX(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2])).Value.ToLower())}"; // Set text
					hsvColor = $"({hsv.Hue},{hsv.Saturation},{hsv.Value})"; // Set
					hslColor = $"({hsl.H},{hsl.S},{hsl.L})"; // Set
					cmykColor = $"{cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}"; // Set
					xyzColor = Global.GetXyzString(xyz);
					yiqColor = Global.GetYiqString(yiq);
				}
				else if (ColorTypeComboBox.Text == Properties.Resources.HEX)
				{
					var rgb = ColorsConverter.HEXtoRGB(new() { Value = ColorTxt.Text }); // Convert
					string hex = ColorTxt.Text.StartsWith("#") ? ColorTxt.Text : "#" + ColorTxt.Text; // Set
					var hsv = ColorsConverter.RGBtoHSV(rgb); // Convert
					var hsl = ColorHelper.ColorConverter.HexToHsl(new(hex)); // Convert
					var cmyk = ColorHelper.ColorConverter.HexToCmyk(new(hex));
					var xyz = ColorHelper.ColorConverter.HexToXyz(new(hex));
					var yiq = ColorHelper.ColorConverter.HexToYiq(new(hex));

					RGBTxt.Text = $"{Properties.Resources.RGB} {rgb.R}{sep}{rgb.G}{sep}{rgb.B}"; // Set text
					HEXTxt.Text = $"{Properties.Resources.HEX} {(u ? hex.ToUpper() : hex)}"; // Set text
					HSVTxt.Text = $"{Properties.Resources.HSV} ({hsv.Hue},{hsv.Saturation},{hsv.Value})"; // Set text
					HSLTxt.Text = $"{Properties.Resources.HSL} ({hsl.H},{hsl.S},{hsl.L})"; // Set text
					CMYKTxt.Text = $"{Properties.Resources.CMYK} {cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}";
					XYZTxt.Text = $"{Properties.Resources.XYZ}\n{Global.GetXyzString(xyz)}";
					YIQTxt.Text = $"{Properties.Resources.YIQ}\n{Global.GetYiqString(yiq)}";

					rgbColor = $"{rgb.R}{sep}{rgb.G}{sep}{rgb.B}"; // Set text
					hexColor = $"{(u ? hex.ToUpper() : hex)}"; // Set text
					hsvColor = $"({hsv.Hue},{hsv.Saturation},{hsv.Value})"; // Set
					hslColor = $"({hsl.H},{hsl.S},{hsl.L})"; // Set
					cmykColor = $"{cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}"; // Set
					xyzColor = Global.GetXyzString(xyz);
					yiqColor = Global.GetYiqString(yiq);
				}

				string[] rC = rgbColor.Split(new string[] { sep }, StringSplitOptions.None); // Split
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
			var xyz = ColorHelper.ColorConverter.HsvToXyz(new(h, (byte)s, (byte)v));
			var yiq = ColorHelper.ColorConverter.HsvToYiq(new(h, (byte)s, (byte)v));

			RGBTxt.Text = $"{Properties.Resources.RGB} {rgb.R}{sep}{rgb.G}{sep}{rgb.B}"; // Set text
			HEXTxt.Text = $"{Properties.Resources.HEX} #{(u ? hex.Value.ToUpper() : hex.Value)}"; // Set text
			HSVTxt.Text = $"{Properties.Resources.HSV} ({h},{s},{v})"; // Set text
			HSLTxt.Text = $"{Properties.Resources.HSL} ({hsl.H},{hsl.S},{hsl.L})"; // Set text
			CMYKTxt.Text = $"{Properties.Resources.CMYK} {cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}";
			XYZTxt.Text = $"{Properties.Resources.XYZ}\n{Global.GetXyzString(xyz)}";
			YIQTxt.Text = $"{Properties.Resources.YIQ}\n{Global.GetYiqString(yiq)}";

			rgbColor = $"{rgb.R}{sep}{rgb.G}{sep}{rgb.B}"; // Set text
			hexColor = $"#{(u ? hex.Value.ToUpper() : hex.Value)}"; // Set text
			hsvColor = $"({h},{s},{v})"; // Set
			hslColor = $"({hsl.H},{hsl.S},{hsl.L})"; // Set
			cmykColor = $"{cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}"; // Set
			xyzColor = Global.GetXyzString(xyz);
			yiqColor = Global.GetYiqString(yiq);

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
			var xyz = ColorHelper.ColorConverter.HslToXyz(new(h, (byte)s, (byte)l));
			var yiq = ColorHelper.ColorConverter.HslToYiq(new((byte)h, (byte)s, (byte)l)); // Convert color


			RGBTxt.Text = $"{Properties.Resources.RGB} {rgb.R}{sep}{rgb.G}{sep}{rgb.B}"; // Set text
			HEXTxt.Text = $"{Properties.Resources.HEX} #{(u ? hex.Value.ToUpper() : hex.Value)}"; // Set text
			HSVTxt.Text = $"{Properties.Resources.HSV} ({hsv.H},{hsv.S},{hsv.V})"; // Set text
			HSLTxt.Text = $"{Properties.Resources.HSL} ({h},{s},{l})"; // Set text
			CMYKTxt.Text = $"{Properties.Resources.CMYK} {cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}";
			XYZTxt.Text = $"{Properties.Resources.XYZ}\n{Global.GetXyzString(xyz)}";
			YIQTxt.Text = $"{Properties.Resources.YIQ}\n{Global.GetYiqString(yiq)}";

			hslColor = $"({h},{s},{l})"; // Set
			hsvColor = $"({hsv.H},{hsv.S},{hsv.V})"; // Set
			rgbColor = $"{rgb.R}{sep}{rgb.G}{sep}{rgb.B}"; // Set
			hexColor = $"#{(u ? hex.Value.ToUpper() : hex.Value)}"; // Set
			cmykColor = $"{cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}"; // Set
			xyzColor = Global.GetXyzString(xyz);
			yiqColor = Global.GetYiqString(yiq);

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
			var xyz = ColorHelper.ColorConverter.CmykToXyz(new((byte)c, (byte)m, (byte)y, (byte)k)); // Convert color
			var yiq = ColorHelper.ColorConverter.CmykToYiq(new((byte)c, (byte)m, (byte)y, (byte)k)); // Convert color

			RGBTxt.Text = $"{Properties.Resources.RGB} {rgb.R}{sep}{rgb.G}{sep}{rgb.B}"; // Set text
			HEXTxt.Text = $"{Properties.Resources.HEX} #{(u ? hex.Value.ToUpper() : hex.Value)}"; // Set text
			HSVTxt.Text = $"{Properties.Resources.HSV} ({hsv.H},{hsv.S},{hsv.V})"; // Set text
			HSLTxt.Text = $"{Properties.Resources.HSL} ({hsl.H},{hsl.S},{hsl.L})"; // Set text
			CMYKTxt.Text = $"{Properties.Resources.CMYK} {c},{m},{y},{k}"; // Set text
			XYZTxt.Text = $"{Properties.Resources.XYZ}\n{Global.GetXyzString(xyz)}";
			YIQTxt.Text = $"{Properties.Resources.YIQ}\n{Global.GetYiqString(yiq)}";

			hslColor = $"({hsl.H},{hsl.S},{hsl.L})"; // Set
			hsvColor = $"({hsv.H},{hsv.S},{hsv.V})"; // Set
			rgbColor = $"{rgb.R}{sep}{rgb.G}{sep}{rgb.B}"; // Set
			hexColor = $"#{(u ? hex.Value.ToUpper() : hex.Value)}"; // Set
			cmykColor = $"{c},{m},{y},{k}"; // Set
			xyzColor = Global.GetXyzString(xyz);
			yiqColor = Global.GetYiqString(yiq);

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

	private void ConvertXyz()
	{
		try
		{
			ColorHelper.XYZ xyz = new(double.Parse(XTxt.Text), double.Parse(XYTxt.Text), double.Parse(ZTxt.Text));

			var rgb = ColorHelper.ColorConverter.XyzToRgb(xyz);
			var hex = ColorHelper.ColorConverter.XyzToHex(xyz);
			var hsl = ColorHelper.ColorConverter.XyzToHsl(xyz);
			var hsv = ColorHelper.ColorConverter.XyzToHsv(xyz);
			var cmyk = ColorHelper.ColorConverter.XyzToCmyk(xyz);

			RGBTxt.Text = $"{Properties.Resources.RGB} {rgb.R}{sep}{rgb.G}{sep}{rgb.B}"; // Set text
			HEXTxt.Text = $"{Properties.Resources.HEX} #{(u ? hex.Value.ToUpper() : hex.Value)}"; // Set text
			HSVTxt.Text = $"{Properties.Resources.HSV} ({hsv.H},{hsv.S},{hsv.V})"; // Set text
			HSLTxt.Text = $"{Properties.Resources.HSL} ({hsl.H},{hsl.S},{hsl.L})"; // Set text
			CMYKTxt.Text = $"{Properties.Resources.CMYK} {Global.GetCmykString(cmyk)}"; // Set text
			XYZTxt.Text = $"{Properties.Resources.XYZ}\n{Global.GetXyzString(xyz)}";

			hslColor = $"({hsl.H},{hsl.S},{hsl.L})"; // Set
			hsvColor = $"({hsv.H},{hsv.S},{hsv.V})"; // Set
			rgbColor = $"{rgb.R}{sep}{rgb.G}{sep}{rgb.B}"; // Set
			hexColor = $"#{(u ? hex.Value.ToUpper() : hex.Value)}"; // Set
			cmykColor = Global.GetCmykString(cmyk); // Set
			xyzColor = Global.GetXyzString(xyz);

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

	private void ConvertYiq()
	{
		try
		{
			ColorHelper.YIQ yiq = new(double.Parse(YQTxt.Text), double.Parse(ITxt.Text), double.Parse(QTxt.Text));

			var rgb = ColorHelper.ColorConverter.YiqToRgb(yiq);
			var hex = ColorHelper.ColorConverter.YiqToHex(yiq);
			var hsl = ColorHelper.ColorConverter.YiqToHsl(yiq);
			var hsv = ColorHelper.ColorConverter.YiqToHsv(yiq);
			var cmyk = ColorHelper.ColorConverter.YiqToCmyk(yiq);
			var xyz = ColorHelper.ColorConverter.YiqToXyz(yiq);

			RGBTxt.Text = $"{Properties.Resources.RGB} {rgb.R}{sep}{rgb.G}{sep}{rgb.B}"; // Set text
			HEXTxt.Text = $"{Properties.Resources.HEX} #{(u ? hex.Value.ToUpper() : hex.Value)}"; // Set text
			HSVTxt.Text = $"{Properties.Resources.HSV} ({hsv.H},{hsv.S},{hsv.V})"; // Set text
			HSLTxt.Text = $"{Properties.Resources.HSL} ({hsl.H},{hsl.S},{hsl.L})"; // Set text
			CMYKTxt.Text = $"{Properties.Resources.CMYK} {Global.GetCmykString(cmyk)}"; // Set text
			XYZTxt.Text = $"{Properties.Resources.XYZ}\n{Global.GetXyzString(xyz)}";
			YIQTxt.Text = $"{Properties.Resources.YIQ}\n{Global.GetYiqString(yiq)}";

			hslColor = $"({hsl.H},{hsl.S},{hsl.L})"; // Set
			hsvColor = $"({hsv.H},{hsv.S},{hsv.V})"; // Set
			rgbColor = $"{rgb.R}{sep}{rgb.G}{sep}{rgb.B}"; // Set
			hexColor = $"#{(u ? hex.Value.ToUpper() : hex.Value)}"; // Set
			cmykColor = Global.GetCmykString(cmyk); // Set
			xyzColor = Global.GetXyzString(xyz);
			yiqColor = Global.GetYiqString(yiq);

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

	private void CopyCMYKBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(cmykColor); // Copy
	}

	private void XTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		ConvertXyz();
	}

	private void CopyYIQBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(yiqColor.Replace("\n", "")); // Copy
	}

	private void YQTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		ConvertYiq();
	}

	private void CopyXYZBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(xyzColor.Replace("\n", "")); // Copy
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

		XTxt.Text = ""; // Clear
		XYTxt.Text = ""; // Clear
		ZTxt.Text = ""; // Clear

		YQTxt.Text = ""; // Clear
		ITxt.Text = ""; // Clear
		QTxt.Text = ""; // Clear

		switch (ColorTypeComboBox.SelectedIndex)
		{
			case 0: // RGB
				ColorTxt.Visibility = Visibility.Visible; // Show
				HSVGrid.Visibility = Visibility.Collapsed; // Hide
				HSLGrid.Visibility = Visibility.Collapsed; // Hide
				CMYKGrid.Visibility = Visibility.Collapsed; // Hide
				XYZGrid.Visibility = Visibility.Collapsed; // Hide
				YIQGrid.Visibility = Visibility.Collapsed; // Hide
				break;
			case 1: // HEX
				ColorTxt.Visibility = Visibility.Visible; // Show
				HSVGrid.Visibility = Visibility.Collapsed; // Hide
				HSLGrid.Visibility = Visibility.Collapsed; // Hide
				CMYKGrid.Visibility = Visibility.Collapsed; // Hide
				XYZGrid.Visibility = Visibility.Collapsed; // Hide
				YIQGrid.Visibility = Visibility.Collapsed; // Hide
				break;
			case 2: // HSV
				ColorTxt.Visibility = Visibility.Collapsed; // Hide
				HSVGrid.Visibility = Visibility.Visible; // Show
				HSLGrid.Visibility = Visibility.Collapsed; // Hide
				CMYKGrid.Visibility = Visibility.Collapsed; // Hide
				XYZGrid.Visibility = Visibility.Collapsed; // Hide
				YIQGrid.Visibility = Visibility.Collapsed; // Hide
				break;
			case 3: // HSL
				ColorTxt.Visibility = Visibility.Collapsed; // Hide
				HSVGrid.Visibility = Visibility.Collapsed; // Hide
				HSLGrid.Visibility = Visibility.Visible; // Show
				CMYKGrid.Visibility = Visibility.Collapsed; // Hide
				XYZGrid.Visibility = Visibility.Collapsed; // Hide
				YIQGrid.Visibility = Visibility.Collapsed; // Hide
				break;
			case 4: // CMYK
				ColorTxt.Visibility = Visibility.Collapsed; // Hide
				HSVGrid.Visibility = Visibility.Collapsed; // Hide
				HSLGrid.Visibility = Visibility.Collapsed; // Hide
				CMYKGrid.Visibility = Visibility.Visible; // Show
				XYZGrid.Visibility = Visibility.Collapsed; // Hide
				YIQGrid.Visibility = Visibility.Collapsed; // Hide
				break;
			case 5: // YIQ
				ColorTxt.Visibility = Visibility.Collapsed; // Hide
				HSVGrid.Visibility = Visibility.Collapsed; // Hide
				HSLGrid.Visibility = Visibility.Collapsed; // Hide
				CMYKGrid.Visibility = Visibility.Collapsed; // Hide
				XYZGrid.Visibility = Visibility.Collapsed; // Hide
				YIQGrid.Visibility = Visibility.Visible; // Show
				break;
			case 6: // XYZ
				ColorTxt.Visibility = Visibility.Collapsed; // Hide
				HSVGrid.Visibility = Visibility.Collapsed; // Hide
				HSLGrid.Visibility = Visibility.Collapsed; // Hide
				CMYKGrid.Visibility = Visibility.Collapsed; // Hide
				XYZGrid.Visibility = Visibility.Visible; // Show
				YIQGrid.Visibility = Visibility.Collapsed; // Hide
				break;
		}
	}
}
