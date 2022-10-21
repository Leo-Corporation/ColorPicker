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
using ColorPicker.UserControls;
using ColorPicker.Windows;
using LeoCorpLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorPicker.Pages;

/// <summary>
/// Interaction logic for PalettePage.xaml
/// </summary>
public partial class PalettePage : Page
{
	RGB[] CurrentColorPalette { get; set; }
	string CurrentRGBColor { get; set; }
	internal List<string> SavedColorPalettes { get; set; }

	public PalettePage()
	{
		InitializeComponent();
		InitUI();
	}

	private void InitUI()
	{
		// Initialize list
		SavedColorPalettes = new(); // Init list

		// Load ColorTypeComboBox
		for (int i = 0; i < Enum.GetValues(typeof(ColorTypes)).Length; i++)
		{
			ColorTypeComboBox.Items.Add(Global.ColorTypesToString((ColorTypes)i)); // Add
		}

		ColorTypeComboBox.SelectedIndex = (int)Global.Settings.FavoriteColorType; // Set index

		// Generate random color
		Random random = new(); // Create new random
		byte r = (byte)random.Next(0, 255); // Generate random number between 0 and 255
		byte g = (byte)random.Next(0, 255); // Generate random number between 0 and 255
		byte b = (byte)random.Next(0, 255); // Generate random number between 0 and 255

		switch ((ColorTypes)ColorTypeComboBox.SelectedIndex)
		{
			case ColorTypes.RGB:
				RGBTxt.Text = $"{r}{Global.Settings.RGBSeparator}{g}{Global.Settings.RGBSeparator}{b}"; // Set text
				break;
			case ColorTypes.HEX:
				RGBTxt.Text = $"#{ColorHelper.ColorConverter.RgbToHex(new(r, g, b))}";
				break;
			case ColorTypes.HSV:
				var hsv = ColorHelper.ColorConverter.RgbToHsv(new(r, g, b)); // Convert
				HueTxt.Text = hsv.H.ToString(); // Set text
				SatTxt.Text = hsv.S.ToString(); // Set text
				ValTxt.Text = hsv.V.ToString(); // Set text
				break;
			case ColorTypes.HSL:
				var hsl = ColorHelper.ColorConverter.RgbToHsl(new(r, g, b)); // Convert
				HTxt.Text = hsl.H.ToString(); // Set text
				STxt.Text = hsl.S.ToString(); // Set text
				LTxt.Text = hsl.L.ToString(); // Set text
				break;
			case ColorTypes.CMYK:
				var cmyk = ColorHelper.ColorConverter.RgbToCmyk(new(r, g, b)); // Convert
				CTxt.Text = cmyk.C.ToString(); // Set text
				MTxt.Text = cmyk.M.ToString(); // Set text
				YTxt.Text = cmyk.Y.ToString(); // Set text
				KTxt.Text = cmyk.K.ToString(); // Set text
				break;
			case ColorTypes.XYZ:
				var xyz = ColorHelper.ColorConverter.RgbToXyz(new(r, g, b)); // Convert
				XTxt.Text = xyz.X.ToString();
				XYTxt.Text = xyz.Y.ToString();
				ZTxt.Text = xyz.Z.ToString();
				break;
			case ColorTypes.YIQ:
				var yiq = ColorHelper.ColorConverter.RgbToYiq(new(r, g, b)); // Convert
				YQTxt.Text = yiq.Y.ToString();
				ITxt.Text = yiq.I.ToString();
				QTxt.Text = yiq.Q.ToString();
				break;
		}

		// History
		if (Global.Settings.RestorePaletteColorHistory.Value && Global.ColorContentHistory.PaletteColorsRGB.Count > 0)
		{
			for (int i = 0; i < Global.ColorContentHistory.PaletteColorsRGB.Count; i++)
			{
				RGB[] restoredColorPalette = new RGB[8];
				for (int j = 0; j < Global.ColorContentHistory.PaletteColorsRGB[i].Count; j++)
				{
					restoredColorPalette[j] = new((byte)Global.ColorContentHistory.PaletteColorsRGB[i][j][0],
						(byte)Global.ColorContentHistory.PaletteColorsRGB[i][j][1],
						(byte)Global.ColorContentHistory.PaletteColorsRGB[i][j][2]);
				}

				SavedColorPalettes.Add($"{restoredColorPalette[7].R};{restoredColorPalette[7].G};{restoredColorPalette[7].B}"); // Add to saved palettes
				HistoryDisplayer.Children.Add(new PaletteHistoryItem(restoredColorPalette, HistoryDisplayer, false, Global.ColorContentHistory.PaletteColorsRGB[i]));
			}
		}
	}

	private static RGB[] GetShades(HSL hsl)
	{
		// Dark shades
		HSL darkShade = new(hsl.H, hsl.S, (hsl.L == 30) ? (byte)15 : (byte)30);
		RGB darkShadeRgb = ColorHelper.ColorConverter.HslToRgb(darkShade);

		// Regular shades
		var l = hsl.L - 16;
		HSL regularShade = new(hsl.H, hsl.S, (byte)l);
		RGB regularShadeRgb = ColorHelper.ColorConverter.HslToRgb(regularShade);

		// Tint shades
		var s = hsl.S - 20;
		var l2 = hsl.L + 6;

		HSL tintShade = new(hsl.H, (byte)s, (byte)l2);
		RGB tintShadeRgb = ColorHelper.ColorConverter.HslToRgb(tintShade);

		RGB[] colors = { darkShadeRgb, regularShadeRgb, tintShadeRgb };

		return colors;
	}

	private void RGBTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		try
		{
			byte[] rgb;
			if (ColorTypeComboBox.SelectedIndex == 0)
			{
				var split = RGBTxt.Text.Split(Global.Settings.RGBSeparator, StringSplitOptions.None);
				rgb = new byte[]
				{
				(byte)int.Parse(split[0]),
				(byte)int.Parse(split[1]),
				(byte)int.Parse(split[2])
				};
			}
			else
			{
				rgb = new byte[]
				{
				ColorHelper.ColorConverter.HexToRgb(new(RGBTxt.Text)).R,
				ColorHelper.ColorConverter.HexToRgb(new(RGBTxt.Text)).G,
				ColorHelper.ColorConverter.HexToRgb(new(RGBTxt.Text)).B
				};
			}
			GeneratePalette(rgb);
		}
		catch {	}
	}

	private void GeneratePalette(byte[] rgb)
	{
		try
		{
			bool u = Global.Settings.HEXUseUpperCase.Value;

			// Set default color


			ColorDisplayer.Background = new SolidColorBrush { Color = Color.FromRgb(rgb[0], rgb[1], rgb[2]) };
			BaseShade.Background = new SolidColorBrush { Color = Color.FromRgb(rgb[0], rgb[1], rgb[2]) };

			// Get shades
			HSL hsl = ColorHelper.ColorConverter.RgbToHsl(new(rgb[0], rgb[1], rgb[2]));

			var shades = GetShades(hsl); // Get shades
			var shades1 = GetShades(ColorHelper.ColorConverter.RgbToHsl(shades[0])); // Get shades

			DBaseShade.Background = new SolidColorBrush { Color = Color.FromRgb(shades[0].R, shades[0].G, shades[0].B) };

			string hex1 = ColorHelper.ColorConverter.RgbToHex(shades[0]).Value; // Dark
			string hex2 = ColorHelper.ColorConverter.RgbToHex(shades[1]).Value; // Regular
			string hex3 = ColorHelper.ColorConverter.RgbToHex(new(rgb[0], rgb[1], rgb[2])).Value;
			string hex4 = ColorHelper.ColorConverter.RgbToHex(shades[2]).Value; // Tint shade
			string hex5 = ColorHelper.ColorConverter.RgbToHex(shades1[0]).Value; // Dark shade
			string hex6 = ColorHelper.ColorConverter.RgbToHex(shades1[1]).Value; // Dark regular
			string hex7 = ColorHelper.ColorConverter.RgbToHex(shades[0]).Value; // Dark base
			string hex8 = ColorHelper.ColorConverter.RgbToHex(shades1[2]).Value; // Dark tint

			// "Lighter" shades

			DarkShade.Background = new SolidColorBrush { Color = Color.FromRgb(shades[0].R, shades[0].G, shades[0].B) };
			RegularShade.Background = new SolidColorBrush { Color = Color.FromRgb(shades[1].R, shades[1].G, shades[1].B) };
			TintShade.Background = new SolidColorBrush { Color = Color.FromRgb(shades[2].R, shades[2].G, shades[2].B) };

			DarkShadeToolTip.Content = $"{Properties.Resources.RGB}: {shades[0].R}{Global.Settings.RGBSeparator}{shades[0].G}{Global.Settings.RGBSeparator}{shades[0].B}" +
				$"\n{Properties.Resources.HEX}: #{(u ? hex1.ToUpper() : hex1.ToLower())}";
			RegularShadeToolTip.Content = $"{Properties.Resources.RGB}: {shades[1].R}{Global.Settings.RGBSeparator}{shades[1].G}{Global.Settings.RGBSeparator}{shades[1].B}" +
				$"\n{Properties.Resources.HEX}: #{(u ? hex2.ToUpper() : hex2.ToLower())}";
			BaseShadeToolTip.Content = $"{Properties.Resources.RGB}: {rgb[0]}{Global.Settings.RGBSeparator}{rgb[1]}{Global.Settings.RGBSeparator}{rgb[2]}" +
				$"\n{Properties.Resources.HEX}: #{(u ? hex3.ToUpper() : hex3.ToLower())}";
			TintShadeToolTip.Content = $"{Properties.Resources.RGB}: {shades[2].R}{Global.Settings.RGBSeparator}{shades[2].G}{Global.Settings.RGBSeparator}{shades[2].B}" +
				$"\n{Properties.Resources.HEX}: #{(u ? hex4.ToUpper() : hex4.ToLower())}";

			// "Darker" shades

			DDarkShade.Background = new SolidColorBrush { Color = Color.FromRgb(shades1[0].R, shades1[0].G, shades1[0].B) };
			DRegularShade.Background = new SolidColorBrush { Color = Color.FromRgb(shades1[1].R, shades1[1].G, shades1[1].B) };
			DTintShade.Background = new SolidColorBrush { Color = Color.FromRgb(shades1[2].R, shades1[2].G, shades1[2].B) };

			DDarkShadeToolTip.Content = $"{Properties.Resources.RGB}: {shades1[0].R}{Global.Settings.RGBSeparator}{shades1[0].G}{Global.Settings.RGBSeparator}{shades1[0].B}" +
				$"\n{Properties.Resources.HEX}: #{(u ? hex5.ToUpper() : hex5.ToLower())}";
			DRegularShadeToolTip.Content = $"{Properties.Resources.RGB}: {shades1[1].R}{Global.Settings.RGBSeparator}{shades1[1].G}{Global.Settings.RGBSeparator}{shades1[1].B}" +
				$"\n{Properties.Resources.HEX}: #{(u ? hex6.ToUpper() : hex6.ToLower())}";
			DBaseShadeToolTip.Content = $"{Properties.Resources.RGB}: {shades[0].R}{Global.Settings.RGBSeparator}{shades[0].G}{Global.Settings.RGBSeparator}{shades[0].B}" +
				$"\n{Properties.Resources.HEX}: #{(u ? hex7.ToUpper() : hex7.ToLower())}";
			DTintShadeToolTip.Content = $"{Properties.Resources.RGB}: {shades1[2].R}{Global.Settings.RGBSeparator}{shades1[2].G}{Global.Settings.RGBSeparator}{shades1[2].B}" +
				$"\n{Properties.Resources.HEX}: #{(u ? hex8.ToUpper() : hex8.ToLower())}";

			// "Brightness" shades
			// Get colors
			HSL light1 = new(hsl.H, hsl.S, 90);
			HSL light2 = new(hsl.H, hsl.S, 84);
			HSL light3 = new(hsl.H, hsl.S, 72);
			HSL light4 = new(hsl.H, hsl.S, 60);
			HSL light5 = new(hsl.H, hsl.S, 48);
			HSL light6 = new(hsl.H, hsl.S, 36);
			HSL light7 = new(hsl.H, hsl.S, 24);
			HSL light8 = new(hsl.H, hsl.S, 12);

			RGB rgbLight1 = ColorHelper.ColorConverter.HslToRgb(light1); // Convert HSL color to RGB
			RGB rgbLight2 = ColorHelper.ColorConverter.HslToRgb(light2); // Convert HSL color to RGB
			RGB rgbLight3 = ColorHelper.ColorConverter.HslToRgb(light3); // Convert HSL color to RGB
			RGB rgbLight4 = ColorHelper.ColorConverter.HslToRgb(light4); // Convert HSL color to RGB
			RGB rgbLight5 = ColorHelper.ColorConverter.HslToRgb(light5); // Convert HSL color to RGB
			RGB rgbLight6 = ColorHelper.ColorConverter.HslToRgb(light6); // Convert HSL color to RGB
			RGB rgbLight7 = ColorHelper.ColorConverter.HslToRgb(light7); // Convert HSL color to RGB
			RGB rgbLight8 = ColorHelper.ColorConverter.HslToRgb(light8); // Convert HSL color to RGB

			string hexLight1 = u ? ColorHelper.ColorConverter.HslToHex(light1).Value.ToUpper() : ColorHelper.ColorConverter.HslToHex(light1).Value.ToLower(); // Convert to HEX (tooltips)
			string hexLight2 = u ? ColorHelper.ColorConverter.HslToHex(light2).Value.ToUpper() : ColorHelper.ColorConverter.HslToHex(light2).Value.ToLower(); // Convert to HEX (tooltips)
			string hexLight3 = u ? ColorHelper.ColorConverter.HslToHex(light3).Value.ToUpper() : ColorHelper.ColorConverter.HslToHex(light3).Value.ToLower(); // Convert to HEX (tooltips)
			string hexLight4 = u ? ColorHelper.ColorConverter.HslToHex(light4).Value.ToUpper() : ColorHelper.ColorConverter.HslToHex(light4).Value.ToLower(); // Convert to HEX (tooltips)
			string hexLight5 = u ? ColorHelper.ColorConverter.HslToHex(light5).Value.ToUpper() : ColorHelper.ColorConverter.HslToHex(light5).Value.ToLower(); // Convert to HEX (tooltips)
			string hexLight6 = u ? ColorHelper.ColorConverter.HslToHex(light6).Value.ToUpper() : ColorHelper.ColorConverter.HslToHex(light6).Value.ToLower(); // Convert to HEX (tooltips)
			string hexLight7 = u ? ColorHelper.ColorConverter.HslToHex(light7).Value.ToUpper() : ColorHelper.ColorConverter.HslToHex(light7).Value.ToLower(); // Convert to HEX (tooltips)
			string hexLight8 = u ? ColorHelper.ColorConverter.HslToHex(light8).Value.ToUpper() : ColorHelper.ColorConverter.HslToHex(light8).Value.ToLower(); // Convert to HEX (tooltips)

			Light1.Background = new SolidColorBrush { Color = Color.FromRgb(rgbLight1.R, rgbLight1.G, rgbLight1.B) }; // Set background color
			Light2.Background = new SolidColorBrush { Color = Color.FromRgb(rgbLight2.R, rgbLight2.G, rgbLight2.B) }; // Set background color
			Light3.Background = new SolidColorBrush { Color = Color.FromRgb(rgbLight3.R, rgbLight3.G, rgbLight3.B) }; // Set background color
			Light4.Background = new SolidColorBrush { Color = Color.FromRgb(rgbLight4.R, rgbLight4.G, rgbLight4.B) }; // Set background color
			Light5.Background = new SolidColorBrush { Color = Color.FromRgb(rgbLight5.R, rgbLight5.G, rgbLight5.B) }; // Set background color
			Light6.Background = new SolidColorBrush { Color = Color.FromRgb(rgbLight6.R, rgbLight6.G, rgbLight6.B) }; // Set background color
			Light7.Background = new SolidColorBrush { Color = Color.FromRgb(rgbLight7.R, rgbLight7.G, rgbLight7.B) }; // Set background color
			Light8.Background = new SolidColorBrush { Color = Color.FromRgb(rgbLight8.R, rgbLight8.G, rgbLight8.B) }; // Set background color

			// Set tool tips text
			Light1ToolTip.Content = $"{Properties.Resources.RGB}: {rgbLight1.R}{Global.Settings.RGBSeparator}{rgbLight1.G}{Global.Settings.RGBSeparator}{rgbLight1.B}" +
				$"\n{Properties.Resources.HEX}: #{hexLight1}";
			Light2ToolTip.Content = $"{Properties.Resources.RGB}: {rgbLight2.R}{Global.Settings.RGBSeparator}{rgbLight2.G}{Global.Settings.RGBSeparator}{rgbLight2.B}" +
				$"\n{Properties.Resources.HEX}: #{hexLight2}";
			Light3ToolTip.Content = $"{Properties.Resources.RGB}: {rgbLight3.R}{Global.Settings.RGBSeparator}{rgbLight3.G}{Global.Settings.RGBSeparator}{rgbLight3.B}" +
				$"\n{Properties.Resources.HEX}: #{hexLight3}";
			Light4ToolTip.Content = $"{Properties.Resources.RGB}: {rgbLight4.R}{Global.Settings.RGBSeparator}{rgbLight4.G}{Global.Settings.RGBSeparator}{rgbLight4.B}" +
				$"\n{Properties.Resources.HEX}: #{hexLight4}";
			Light5ToolTip.Content = $"{Properties.Resources.RGB}: {rgbLight5.R}{Global.Settings.RGBSeparator}{rgbLight5.G}{Global.Settings.RGBSeparator}{rgbLight5.B}" +
				$"\n{Properties.Resources.HEX}: #{hexLight5}";
			Light6ToolTip.Content = $"{Properties.Resources.RGB}: {rgbLight6.R}{Global.Settings.RGBSeparator}{rgbLight6.G}{Global.Settings.RGBSeparator}{rgbLight6.B}" +
				$"\n{Properties.Resources.HEX}: #{hexLight6}";
			Light7ToolTip.Content = $"{Properties.Resources.RGB}: {rgbLight7.R}{Global.Settings.RGBSeparator}{rgbLight7.G}{Global.Settings.RGBSeparator}{rgbLight7.B}" +
				$"\n{Properties.Resources.HEX}: #{hexLight7}";
			Light8ToolTip.Content = $"{Properties.Resources.RGB}: {rgbLight8.R}{Global.Settings.RGBSeparator}{rgbLight8.G}{Global.Settings.RGBSeparator}{rgbLight8.B}" +
				$"\n{Properties.Resources.HEX}: #{hexLight8}";

			// History
			RGB[] c1 = shades.Append(shades1);
			CurrentColorPalette = c1.Append(shades[0], new RGB(rgb[0], rgb[1], rgb[2]));
			CurrentRGBColor = $"{rgb[0]};{rgb[1]};{rgb[2]}";
		}
		catch
		{

		}
	}

	private void RandomColorBtn_Click(object sender, RoutedEventArgs e)
	{
		// Generate random color
		int r, g, b;
		Random random = new();

		r = random.Next(0, 255); // Generate random number between 0 and 255
		g = random.Next(0, 255); // Generate random number between 0 and 255
		b = random.Next(0, 255); // Generate random number between 0 and 255

		RGBTxt.Text = ColorTypeComboBox.SelectedIndex switch
		{
			0 => $"{r}{Global.Settings.RGBSeparator}{g}{Global.Settings.RGBSeparator}{b}", // Set text
			1 => $"#{(Global.Settings.HEXUseUpperCase.Value ? ColorHelper.ColorConverter.RgbToHex(new((byte)r, (byte)g, (byte)b)).Value.ToUpper() : ColorHelper.ColorConverter.RgbToHex(new((byte)r, (byte)g, (byte)b)).Value.ToLower())}", // Set text
			_ => $"{r}{Global.Settings.RGBSeparator}{g}{Global.Settings.RGBSeparator}{b}" // Set text
		};
	}

	private static string GetRgbStringFromBorder(Border border)
	{
		var color = ((SolidColorBrush)border.Background).Color; // Get the color

		return $"{color.R}{Global.Settings.RGBSeparator}{color.G}{Global.Settings.RGBSeparator}{color.B}";
	}

	private static string GetHexStringFromBorder(Border border)
	{
		var color = ((SolidColorBrush)border.Background).Color; // Get the color
		string hex = Global.Settings.HEXUseUpperCase.Value ? ColorHelper.ColorConverter.RgbToHex(new(color.R, color.G, color.B)).Value.ToUpper()
														   : ColorHelper.ColorConverter.RgbToHex(new(color.R, color.G, color.B)).Value.ToLower();
		return $"#{hex}";
	}

	private void DarkShade_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Clipboard.SetText(ColorTypeComboBox.SelectedIndex switch
		{
			0 => GetRgbStringFromBorder((Border)sender),
			1 => GetHexStringFromBorder((Border)sender),
			_ => GetRgbStringFromBorder((Border)sender)
		}); // Copy
	}

	internal void HistoryBtn_Click(object sender, RoutedEventArgs e)
	{
		if (HistoryDisplayer.Visibility == Visibility.Visible)
		{
			HistoryDisplayer.Visibility = Visibility.Collapsed;
			PaletteContent.Visibility = Visibility.Visible;
			HistoryBtn.Content = "\uF47F"; // Set text
		}
		else
		{
			HistoryDisplayer.Visibility = Visibility.Visible;
			PaletteContent.Visibility = Visibility.Collapsed;
			HistoryBtn.Content = "\uF36A"; // Set text
		}
	}

	private void AddToHistoryBtn_Click(object sender, RoutedEventArgs e)
	{
		if (!SavedColorPalettes.Contains(CurrentRGBColor))
		{
			SavedColorPalettes.Add(CurrentRGBColor); // Add to saved palettes
			HistoryDisplayer.Children.Add(new PaletteHistoryItem(CurrentColorPalette, HistoryDisplayer));
		}
	}

	private void ColorDisplayer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		new ColorWheelWindow(true, RGBTxt, (ColorTypes)ColorTypeComboBox.SelectedIndex).Show(); // Show window
	}

	private void ColorTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		RGBTxt.Text = ""; // Clear
		RGBTxt.MaxLength = ColorTypeComboBox.SelectedIndex switch
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
				RGBTxt.Visibility = Visibility.Visible; // Show
				HSVGrid.Visibility = Visibility.Collapsed; // Hide
				HSLGrid.Visibility = Visibility.Collapsed; // Hide
				CMYKGrid.Visibility = Visibility.Collapsed; // Hide
				XYZGrid.Visibility = Visibility.Collapsed; // Hide
				YIQGrid.Visibility = Visibility.Collapsed; // Hide
				break;
			case 1: // HEX
				RGBTxt.Visibility = Visibility.Visible; // Show
				HSVGrid.Visibility = Visibility.Collapsed; // Hide
				HSLGrid.Visibility = Visibility.Collapsed; // Hide
				CMYKGrid.Visibility = Visibility.Collapsed; // Hide
				XYZGrid.Visibility = Visibility.Collapsed; // Hide
				YIQGrid.Visibility = Visibility.Collapsed; // Hide
				break;
			case 2: // HSV
				RGBTxt.Visibility = Visibility.Collapsed; // Hide
				HSVGrid.Visibility = Visibility.Visible; // Show
				HSLGrid.Visibility = Visibility.Collapsed; // Hide
				CMYKGrid.Visibility = Visibility.Collapsed; // Hide
				XYZGrid.Visibility = Visibility.Collapsed; // Hide
				YIQGrid.Visibility = Visibility.Collapsed; // Hide
				break;
			case 3: // HSL
				RGBTxt.Visibility = Visibility.Collapsed; // Hide
				HSVGrid.Visibility = Visibility.Collapsed; // Hide
				HSLGrid.Visibility = Visibility.Visible; // Show
				CMYKGrid.Visibility = Visibility.Collapsed; // Hide
				XYZGrid.Visibility = Visibility.Collapsed; // Hide
				YIQGrid.Visibility = Visibility.Collapsed; // Hide
				break;
			case 4: // CMYK
				RGBTxt.Visibility = Visibility.Collapsed; // Hide
				HSVGrid.Visibility = Visibility.Collapsed; // Hide
				HSLGrid.Visibility = Visibility.Collapsed; // Hide
				CMYKGrid.Visibility = Visibility.Visible; // Show
				XYZGrid.Visibility = Visibility.Collapsed; // Hide
				YIQGrid.Visibility = Visibility.Collapsed; // Hide
				break;
			case 5: // YIQ
				RGBTxt.Visibility = Visibility.Collapsed; // Hide
				HSVGrid.Visibility = Visibility.Collapsed; // Hide
				HSLGrid.Visibility = Visibility.Collapsed; // Hide
				CMYKGrid.Visibility = Visibility.Collapsed; // Hide
				XYZGrid.Visibility = Visibility.Collapsed; // Hide
				YIQGrid.Visibility = Visibility.Visible; // Show
				break;
			case 6: // XYZ
				RGBTxt.Visibility = Visibility.Collapsed; // Hide
				HSVGrid.Visibility = Visibility.Collapsed; // Hide
				HSLGrid.Visibility = Visibility.Collapsed; // Hide
				CMYKGrid.Visibility = Visibility.Collapsed; // Hide
				XYZGrid.Visibility = Visibility.Visible; // Show
				YIQGrid.Visibility = Visibility.Collapsed; // Hide
				break;
		}
	}

	private void HueTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		try
		{
			RGB rgb = ColorHelper.ColorConverter.HsvToRgb(new(int.Parse(HueTxt.Text), (byte)int.Parse(SatTxt.Text), (byte)int.Parse(ValTxt.Text)));
			GeneratePalette(new byte[] { rgb.R, rgb.G, rgb.B });
		}
		catch { }
	}

	private void HTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		try
		{
			RGB rgb = ColorHelper.ColorConverter.HslToRgb(new(int.Parse(HTxt.Text), (byte)int.Parse(STxt.Text), (byte)int.Parse(LTxt.Text)));
			GeneratePalette(new byte[] { rgb.R, rgb.G, rgb.B });
		}
		catch {	}
	}

	private void CTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		try
		{
			RGB rgb = ColorHelper.ColorConverter.CmykToRgb(new((byte)int.Parse(CTxt.Text), (byte)int.Parse(MTxt.Text), (byte)int.Parse(YTxt.Text), (byte)int.Parse(KTxt.Text)));
			GeneratePalette(new byte[] { rgb.R, rgb.G, rgb.B });
		}
		catch { }
	}

	private void XTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		try
		{
			RGB rgb = ColorHelper.ColorConverter.XyzToRgb(new(double.Parse(XTxt.Text), double.Parse(XYTxt.Text), double.Parse(ZTxt.Text)));
			GeneratePalette(new byte[] { rgb.R, rgb.G, rgb.B });
		}
		catch {	}
	}

	private void YQTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		try
		{
			RGB rgb = ColorHelper.ColorConverter.YiqToRgb(new(double.Parse(YQTxt.Text), double.Parse(ITxt.Text), double.Parse(QTxt.Text)));
			GeneratePalette(new byte[] { rgb.R, rgb.G, rgb.B });
		}
		catch { }
	}
}
