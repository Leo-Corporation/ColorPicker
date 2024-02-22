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
using ColorPicker.Windows;
using Synethia;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for PalettePage.xaml
/// </summary>
public partial class PalettePage : Page
{
	bool code = !Global.Settings.UseSynethia; // checks if the code as already been implemented
	public PalettePage()
	{
		InitializeComponent();
		InitUI();

		Loaded += (o, e) => SynethiaManager.InjectSynethiaCode(this, Global.SynethiaConfig.PagesInfo, 4, ref code); // injects the code in the page

	}
	private void InitUI()
	{
		TitleTxt.Text = $"{Properties.Resources.Creation} > {Properties.Resources.Palette}";
		try
		{
			(int r, int g, int b) = Global.GenerateRandomColor();
			Txt1.Text = r.ToString();
			Txt2.Text = g.ToString();
			Txt3.Text = b.ToString();
			ColorInfo = new(new((byte)r, (byte)g, (byte)b));
		}
		catch { }
		RgbBtn_Click(Global.Settings.DefaultColorType switch
		{
			ColorTypes.HEX => HexBtn,
			ColorTypes.HSV => HsvBtn,
			ColorTypes.HSL => HslBtn,
			ColorTypes.CMYK => CmykBtn,
			ColorTypes.XYZ => XyzBtn,
			ColorTypes.YIQ => YiqBtn,
			ColorTypes.YUV => YuvBtn,
			ColorTypes.DEC => DecBtn,
			_ => RgbBtn
		}, null);
	}
	internal ColorInfo ColorInfo { get; set; }

	private RGB ConvertToRgb()
	{
		if (SelectedColorBtn == RgbBtn) return new((byte)int.Parse(Txt1.Text),
											 (byte)int.Parse(Txt2.Text),
											 (byte)int.Parse(Txt3.Text));
		if (SelectedColorBtn == HexBtn) return ColorHelper.ColorConverter.HexToRgb(new(Txt5.Text));
		else if (SelectedColorBtn == HsvBtn) return ColorHelper.ColorConverter.HsvToRgb(new(int.Parse(Txt1.Text),
											 (byte)int.Parse(Txt2.Text),
											 (byte)int.Parse(Txt3.Text)));
		else if (SelectedColorBtn == HslBtn) return ColorHelper.ColorConverter.HslToRgb(new(int.Parse(Txt1.Text),
											 (byte)int.Parse(Txt2.Text),
											 (byte)int.Parse(Txt3.Text)));
		else if (SelectedColorBtn == CmykBtn) return ColorHelper.ColorConverter.CmykToRgb(new((byte)int.Parse(Txt1.Text),
											 (byte)int.Parse(Txt2.Text),
											 (byte)int.Parse(Txt3.Text),
											 (byte)int.Parse(Txt4.Text)));
		else if (SelectedColorBtn == XyzBtn) return ColorHelper.ColorConverter.XyzToRgb(new(double.Parse(Txt1.Text),
											 double.Parse(Txt2.Text),
											 double.Parse(Txt3.Text)));
		else if (SelectedColorBtn == YuvBtn) return ColorHelper.ColorConverter.YuvToRgb(new(double.Parse(Txt1.Text),
											 double.Parse(Txt2.Text),
											 double.Parse(Txt3.Text)));
		else if (SelectedColorBtn == DecBtn) return new DEC(int.Parse(Txt5.Text)).ToRgb();
		else return ColorHelper.ColorConverter.YiqToRgb(new(double.Parse(Txt1.Text),
											 double.Parse(Txt2.Text),
											 double.Parse(Txt3.Text)));
	}

	internal Button SelectedColorBtn { get; set; }
	private void UnCheckAllButtons()
	{
		RgbBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		HexBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		HsvBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		HslBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		CmykBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		XyzBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		YiqBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		YuvBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		DecBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
	}

	// Note: This event handler is used for all the choices
	internal void RgbBtn_Click(object sender, RoutedEventArgs? e)
	{
		var btn = (Button)sender;

		UnCheckAllButtons();
		CheckButton(btn);
		SelectedColorBtn = btn;
		LoadInputUI();
	}

	internal void CheckButton(Button button) => button.Background = Global.GetColorFromResource("LightAccentColor");

	private void HideAllInput()
	{
		DisplayText1.Visibility = Visibility.Collapsed;
		DisplayText2.Visibility = Visibility.Collapsed;
		DisplayText3.Visibility = Visibility.Collapsed;
		DisplayText4.Visibility = Visibility.Collapsed;
		DisplayText5.Visibility = Visibility.Collapsed; // Special textbox for hex

		// Clear text to avoid errors
		Txt1.Text = "";
		Txt2.Text = "";
		Txt3.Text = "";
		Txt4.Text = "";
		Txt5.Text = ""; // Special textbox for hex

		B1.Visibility = Visibility.Collapsed;
		B2.Visibility = Visibility.Collapsed;
		B3.Visibility = Visibility.Collapsed;
		B4.Visibility = Visibility.Collapsed;
		B5.Visibility = Visibility.Collapsed; // Special textbox for hex
	}

	private void LoadInputUI()
	{
		HideAllInput();
		if (SelectedColorBtn != HexBtn && SelectedColorBtn != DecBtn)
		{
			DisplayText1.Visibility = Visibility.Visible;
			DisplayText2.Visibility = Visibility.Visible;
			DisplayText3.Visibility = Visibility.Visible;
			DisplayText4.Visibility = SelectedColorBtn == CmykBtn ? Visibility.Visible : Visibility.Collapsed;

			B1.Visibility = Visibility.Visible;
			B2.Visibility = Visibility.Visible;
			B3.Visibility = Visibility.Visible;
			B4.Visibility = SelectedColorBtn == CmykBtn ? Visibility.Visible : Visibility.Collapsed;
		}

		if (SelectedColorBtn == RgbBtn)
		{
			DisplayText1.Text = "R";
			DisplayText2.Text = "G";
			DisplayText3.Text = "B";

			Txt1.Text = ColorInfo.RGB.R.ToString();
			Txt2.Text = ColorInfo.RGB.G.ToString();
			Txt3.Text = ColorInfo.RGB.B.ToString();
		}
		else if (SelectedColorBtn == HexBtn)
		{
			DisplayText5.Visibility = Visibility.Visible;

			DisplayText5.Text = Properties.Resources.HEX;
			B5.Visibility = Visibility.Visible;

			Txt5.Text = ColorInfo.HEX.Value;
		}
		else if (SelectedColorBtn == HsvBtn)
		{
			DisplayText1.Text = "H";
			DisplayText2.Text = "S";
			DisplayText3.Text = "V";

			Txt1.Text = ColorInfo.HSV.H.ToString();
			Txt2.Text = ColorInfo.HSV.S.ToString();
			Txt3.Text = ColorInfo.HSV.V.ToString();
		}
		else if (SelectedColorBtn == HslBtn)
		{
			DisplayText1.Text = "H";
			DisplayText2.Text = "S";
			DisplayText3.Text = "L";

			Txt1.Text = ColorInfo.HSL.H.ToString();
			Txt2.Text = ColorInfo.HSL.S.ToString();
			Txt3.Text = ColorInfo.HSL.L.ToString();
		}
		else if (SelectedColorBtn == CmykBtn)
		{
			DisplayText1.Text = "C";
			DisplayText2.Text = "M";
			DisplayText3.Text = "Y";
			DisplayText4.Text = "K";

			Txt1.Text = ColorInfo.CMYK.C.ToString();
			Txt2.Text = ColorInfo.CMYK.M.ToString();
			Txt3.Text = ColorInfo.CMYK.Y.ToString();
			Txt4.Text = ColorInfo.CMYK.K.ToString();
		}
		else if (SelectedColorBtn == XyzBtn)
		{
			DisplayText1.Text = "X";
			DisplayText2.Text = "Y";
			DisplayText3.Text = "Z";

			Txt1.Text = ColorInfo.XYZ.X.ToString();
			Txt2.Text = ColorInfo.XYZ.Y.ToString();
			Txt3.Text = ColorInfo.XYZ.Z.ToString();
		}
		else if (SelectedColorBtn == YiqBtn)
		{
			DisplayText1.Text = "Y";
			DisplayText2.Text = "I";
			DisplayText3.Text = "Q";

			Txt1.Text = ColorInfo.YIQ.Y.ToString();
			Txt2.Text = ColorInfo.YIQ.I.ToString();
			Txt3.Text = ColorInfo.YIQ.Q.ToString();
		}
		else if (SelectedColorBtn == YuvBtn)
		{
			DisplayText1.Text = "Y";
			DisplayText2.Text = "U";
			DisplayText3.Text = "V";

			Txt1.Text = ColorInfo.YUV.Y.ToString();
			Txt2.Text = ColorInfo.YUV.U.ToString();
			Txt3.Text = ColorInfo.YUV.V.ToString();
		}
		else if (SelectedColorBtn == DecBtn)
		{
			DisplayText5.Visibility = Visibility.Visible;

			DisplayText5.Text = Properties.Resources.DEC;
			B5.Visibility = Visibility.Visible;

			Txt5.Text = ColorInfo.DEC.Value.ToString();
		}
	}

	internal void InitPaletteUI(bool setColor = false)
	{
		if (!setColor) ColorInfo = new ColorInfo(ConvertToRgb());
		ColorBorder.Background = new SolidColorBrush { Color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B) };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B) };

		// Shades
		ShadesPanel.Children.Clear();
		BrightnessPanel.Children.Clear();
		HuePanel.Children.Clear();

		RGB[][] palettes = [Global.GetShades(ColorInfo.HSL), Global.GetBrightness(ColorInfo.HSL), Global.GetHues(ColorInfo.HSL)];
		for (int k = 0; k < palettes.Length; k++)
		{
			var shades = palettes[k];
			for (int i = 0; i < shades.Length; i++)
			{
				CornerRadius radius = i == 0 ? new(10, 0, 0, 10) : new(0);
				if (i == shades.Length - 1) radius = new(0, 10, 10, 0);

				Border border = new()
				{
					Cursor = Cursors.Hand,
					Height = 50,
					Width = 50,
					CornerRadius = radius,
					Background = new SolidColorBrush { Color = Color.FromRgb(shades[i].R, shades[i].G, shades[i].B) },
					Effect = new DropShadowEffect()
					{
						BlurRadius = 15,
						Opacity = 0.2,
						ShadowDepth = 0,
						Color = Color.FromRgb(shades[i].R, shades[i].G, shades[i].B)
					},
					ToolTip = new ToolTip()
					{
						Background = Global.GetColorFromResource("Background1"),
						Foreground = Global.GetColorFromResource("Foreground1"),
						Content = new ColorInfo(new(shades[i].R, shades[i].G, shades[i].B)).ToString()
					},
				};

				int j = i > shades.Length ? shades.Length - 1 : i; // Avoid index out of range
				border.MouseLeftButtonUp += (o, e) =>
				{
					var info = new ColorInfo(new(shades[j].R, shades[j].G, shades[j].B));
					Clipboard.SetText(Global.Settings.DefaultColorType switch
					{
						ColorTypes.HEX => info.HEX.Value,
						ColorTypes.HSV => $"{info.HSV.H},{info.HSV.S},{info.HSV.V}",
						ColorTypes.HSL => $"{info.HSL.H},{info.HSL.S},{info.HSL.L}",
						ColorTypes.CMYK => $"{info.CMYK.C},{info.CMYK.M},{info.CMYK.Y},{info.CMYK.K}",
						ColorTypes.XYZ => $"{info.XYZ.X}; {info.XYZ.Y}; {info.XYZ.Z}",
						ColorTypes.YIQ => $"{info.YIQ.Y}; {info.YIQ.I}; {info.YIQ.Q}",
						ColorTypes.YUV => $"{info.YUV.Y}; {info.YUV.U}; {info.YUV.V}",
						ColorTypes.DEC => info.DEC.Value.ToString(),
						_ => $"{shades[j].R}{Global.Settings.RgbSeparator}{shades[j].G}{Global.Settings.RgbSeparator}{shades[j].B}"
					});
				};

				border.MouseRightButtonUp += (o, e) =>
				{
					new ColorDetailsWindow(new SolidColorBrush { Color = Color.FromRgb(shades[j].R, shades[j].G, shades[j].B) }).Show();
				};
				if (k == 0) ShadesPanel.Children.Add(border);
				else if (k == 1) BrightnessPanel.Children.Add(border);
				else HuePanel.Children.Add(border);
			}
		}

		// Load the bookmark icon
		if (!Global.Bookmarks.PaletteBookmarks.Contains(ColorInfo.HEX.Value))
		{
			BookmarkBtn.Content = "\uF1F6";
			BookmarkToolTip.Content = Properties.Resources.AddBookmark;

			return;
		}
		BookmarkBtn.Content = "\uF1F8";
		BookmarkToolTip.Content = Properties.Resources.RemoveBookmark;
	}

	private void Txt1_TextChanged(object sender, TextChangedEventArgs e)
	{
		try
		{
			InitPaletteUI();
		}
		catch { }
	}

	private void Txt1_CanExecute(object sender, CanExecuteRoutedEventArgs e)
	{
		if (e.Command == ApplicationCommands.Paste)
		{
			e.CanExecute = true;
			e.Handled = true;
		}
	}

	private void Txt1_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		try
		{
			if (e.Command == ApplicationCommands.Paste)
			{
				string text = Clipboard.GetText()
					.Replace("(", "")
					.Replace(")", "")
					.Replace(" ", "");
				if (SelectedColorBtn == HsvBtn || SelectedColorBtn == HslBtn || SelectedColorBtn == CmykBtn)
				{
					var split = text.Split(",");
					Txt1.Text = split[0];
					Txt2.Text = split[1];
					Txt3.Text = split[2];
					Txt4.Text = split.Length > 3 ? split[3] : "";
				}
				else if (SelectedColorBtn == HexBtn || SelectedColorBtn == DecBtn)
				{
					Txt5.Text = text;
				}
				else
				{
					var split = text.Split(new string[] { ";", Global.Settings.RgbSeparator ?? ";" }, StringSplitOptions.None);
					Txt1.Text = split[0];
					Txt2.Text = split[1];
					Txt3.Text = split[2];
				}

				e.Handled = true;
			}
		}
		catch { }
	}

	private void BookmarkBtn_Click(object sender, RoutedEventArgs e)
	{
		if (Global.Bookmarks.PaletteBookmarks.Contains(ColorInfo.HEX.Value))
		{
			Global.Bookmarks.PaletteBookmarks.Remove(ColorInfo.HEX.Value);
			BookmarkBtn.Content = "\uF1F6";
			BookmarkToolTip.Content = Properties.Resources.AddBookmark;

			return;
		}
		Global.Bookmarks.PaletteBookmarks.Add(ColorInfo.HEX.Value); // Add to color bookmarks
		BookmarkBtn.Content = "\uF1F8";
		BookmarkToolTip.Content = Properties.Resources.RemoveBookmark;
	}

	internal void ColorBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		try
		{
			(int r, int g, int b) = Global.GenerateRandomColor();
			ColorInfo = new(new((byte)r, (byte)g, (byte)b));
			Global.SynethiaConfig.ActionsInfo[4].UsageCount++; // Increment the usage counter
		}
		catch { }
		RgbBtn_Click(SelectedColorBtn, null);
	}

	internal void InitFromColor(ColorInfo color)
	{
		ColorInfo = color;
		Global.SynethiaConfig.ActionsInfo[4].UsageCount++; // Increment the usage counter
		RgbBtn_Click(SelectedColorBtn, null);
	}
}
