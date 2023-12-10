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
using ColorPicker.Windows;
using Synethia;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for TextPage.xaml
/// </summary>
public partial class TextPage : Page
{
	bool code = Global.Settings.UseSynethia ? false : true; // checks if the code as already been implemented

	public TextPage()
	{
		InitializeComponent();
		InitUI();

		Loaded += (o, e) => SynethiaManager.InjectSynethiaCode(this, Global.SynethiaConfig.PagesInfo, 3, ref code); // injects the code in the page

	}

	internal void InitUI()
	{
		FontComboBox.Items.Clear();
		TitleTxt.Text = $"{Properties.Resources.ColorTools} > {Properties.Resources.TextTool}";

		System.Drawing.Text.InstalledFontCollection installedFonts = new();
		foreach (System.Drawing.FontFamily fontFamily in installedFonts.Families)
		{
			FontComboBox.Items.Add(fontFamily.Name);
		}
		FontComboBox.Text = Global.Settings.TextToolFont;
		FontSizeTxt.Text = Global.Settings.TextToolFontSize.ToString();

		// Set default Color

		ColorHelper.RGB fgd = ColorHelper.ColorConverter.HexToRgb(new(Global.Settings.TextToolForeground));
		ColorHelper.RGB bgd = ColorHelper.ColorConverter.HexToRgb(new(Global.Settings.TextToolBackground));

		foreground = System.Drawing.Color.FromArgb(fgd.R, fgd.G, fgd.B);
		background = System.Drawing.Color.FromArgb(bgd.R, bgd.G, bgd.B);

		var foreBrush = new SolidColorBrush { Color = Color.FromRgb(fgd.R, fgd.G, fgd.B) }; // Set color
		var backBrush = new SolidColorBrush { Color = Color.FromRgb(bgd.R, bgd.G, bgd.B) }; // Set color

		RegularTxt.Foreground = foreBrush; // Set foreground color
		ItalicTxt.Foreground = foreBrush; // Set foreground color
		BoldTxt.Foreground = foreBrush; // Set foreground color

		RegularTxt.Background = backBrush; // Set background color
		ItalicTxt.Background = backBrush; // Set background color
		BoldTxt.Background = backBrush; // Set background color
		TextPanel.Background = backBrush; // Set background color

		LoadConstrastUI();
		RgbBtn_Click(RgbBtn, null);
		SelectedColorBtn = Global.Settings.DefaultColorType switch
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
		};
		BookmarkText bookmarkText = new(FontComboBox.SelectedItem.ToString(), ColorHelper.ColorConverter.RgbToHex(new(foreground.R, foreground.G, foreground.B)).Value, ColorHelper.ColorConverter.RgbToHex(new(background.R, background.G, background.B)).Value);
		BookmarkBtn.Content = !Global.Bookmarks.TextBookmarks.Contains(bookmarkText) ? "\uF1F6" : "\uF1F8";
	}

	internal void InitFromBookmark(BookmarkText bookmarkText)
	{
		// Set default Color

		ColorHelper.RGB fgd = ColorHelper.ColorConverter.HexToRgb(new(bookmarkText.ForegroundColor));
		ColorHelper.RGB bgd = ColorHelper.ColorConverter.HexToRgb(new(bookmarkText.BackgroundColor));

		foreground = System.Drawing.Color.FromArgb(fgd.R, fgd.G, fgd.B);
		background = System.Drawing.Color.FromArgb(bgd.R, bgd.G, bgd.B);

		var foreBrush = new SolidColorBrush { Color = Color.FromRgb(fgd.R, fgd.G, fgd.B) }; // Set color
		var backBrush = new SolidColorBrush { Color = Color.FromRgb(bgd.R, bgd.G, bgd.B) }; // Set color

		RegularTxt.Foreground = foreBrush; // Set foreground color
		ItalicTxt.Foreground = foreBrush; // Set foreground color
		BoldTxt.Foreground = foreBrush; // Set foreground color
		ForegroundBorder.Background = foreBrush;

		RegularTxt.Background = backBrush; // Set background color
		ItalicTxt.Background = backBrush; // Set background color
		BoldTxt.Background = backBrush; // Set background color
		TextPanel.Background = backBrush; // Set background color
		BackgroundBorder.Background = backBrush;

		FontComboBox.SelectedItem = bookmarkText.FontFamily;

		LoadConstrastUI();
		RgbBtn_Click(RgbBtn, null);
	}

	private void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		try
		{
			RegularTxt.FontFamily = new(FontComboBox.SelectedItem.ToString()); // Set font family
			ItalicTxt.FontFamily = new(FontComboBox.SelectedItem.ToString()); // Set font family
			BoldTxt.FontFamily = new(FontComboBox.SelectedItem.ToString()); // Set font family
			BookmarkText bookmarkText = new(FontComboBox.SelectedItem.ToString(), ColorHelper.ColorConverter.RgbToHex(new(foreground.R, foreground.G, foreground.B)).Value, ColorHelper.ColorConverter.RgbToHex(new(background.R, background.G, background.B)).Value);
			BookmarkBtn.Content = !Global.Bookmarks.TextBookmarks.Contains(bookmarkText) ? "\uF1F6" : "\uF1F8";
		}
		catch { }
	}

	private void FontSizeTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		try
		{
			RegularTxt.FontSize = int.Parse(FontSizeTxt.Text); // Set font size
			ItalicTxt.FontSize = int.Parse(FontSizeTxt.Text); // Set font size
			BoldTxt.FontSize = int.Parse(FontSizeTxt.Text); // Set font size
		}
		catch { }
	}

	private void FontSizeTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		Regex regex = new("[^0-9]+");
		e.Handled = regex.IsMatch(e.Text);
	}

	System.Drawing.Color foreground, background;
	bool isForeground = false;
	private void ForegroundBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		isForeground = true;

		var bg = ForegroundBorder.Background as SolidColorBrush;
		ColorInfo = new(new(bg.Color.R, bg.Color.G, bg.Color.B));

		RgbBtn_Click(SelectedColorBtn, null);
		ColorSelector.IsOpen = true;
	}

	private void BackgroundBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		isForeground = false;

		var bg = BackgroundBorder.Background as SolidColorBrush;
		ColorInfo = new(new(bg.Color.R, bg.Color.G, bg.Color.B));

		RgbBtn_Click(SelectedColorBtn, null);
		ColorSelector.IsOpen = true;
	}
	Random random = new();

	private void GenerateGradientBtn_Click(object sender, System.Windows.RoutedEventArgs e)
	{
		// 1. Generate two random colors
		int n = random.Next(0, 2); // if n = 0, fg will be light; otherwise, fg will be dark

		var fg = n == 0 ? ColorHelper.ColorGenerator.GetLightRandomColor<ColorHelper.RGB>() : ColorHelper.ColorGenerator.GetDarkRandomColor<ColorHelper.RGB>();
		var bg = n == 0 ? ColorHelper.ColorGenerator.GetDarkRandomColor<ColorHelper.RGB>() : ColorHelper.ColorGenerator.GetLightRandomColor<ColorHelper.RGB>();

		foreground = System.Drawing.Color.FromArgb(fg.R, fg.G, fg.B);
		background = System.Drawing.Color.FromArgb(bg.R, bg.G, bg.B);

		SolidColorBrush fgBrush = new() { Color = Color.FromRgb(fg.R, fg.G, fg.B) };
		SolidColorBrush bgBrush = new() { Color = Color.FromRgb(bg.R, bg.G, bg.B) };

		// 2. Set the background and foreground color
		BackgroundBorder.Background = bgBrush;
		RegularTxt.Background = bgBrush; // Set background color
		ItalicTxt.Background = bgBrush; // Set background color
		BoldTxt.Background = bgBrush; // Set background color
		TextPanel.Background = bgBrush; // Set background color

		ForegroundBorder.Background = fgBrush;
		RegularTxt.Foreground = fgBrush; // Set Foreground color
		ItalicTxt.Foreground = fgBrush; // Set Foreground color
		BoldTxt.Foreground = fgBrush; // Set Foreground color

		// 3. Update the contrast UI
		LoadConstrastUI();

		// 4. Update the bookmark button
		BookmarkText bookmarkText = new(FontComboBox.SelectedItem.ToString(), ColorHelper.ColorConverter.RgbToHex(new(foreground.R, foreground.G, foreground.B)).Value, ColorHelper.ColorConverter.RgbToHex(new(background.R, background.G, background.B)).Value);
		BookmarkBtn.Content = !Global.Bookmarks.TextBookmarks.Contains(bookmarkText) ? "\uF1F6" : "\uF1F8";
	}

	internal void LoadConstrastUI()
	{
		(string, int) contrast = Global.GetContrast(new int[] { foreground.R, foreground.G, foreground.B }, new int[] { background.R, background.G, background.B });
		Grid.SetRow(IndicatorArrow, contrast.Item2);

		ContrastTxt.Text = contrast.Item1;
		Global.SynethiaConfig.ActionsInfo[3].UsageCount++; // Increment the usage counter
	}

	ColorInfo ColorInfo { get; set; } = new(new(0, 0, 0));

	Button SelectedColorBtn { get; set; }
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

		// Update the bookmark button
		BookmarkText bookmarkText = new(FontComboBox.SelectedItem.ToString(), ColorHelper.ColorConverter.RgbToHex(new(foreground.R, foreground.G, foreground.B)).Value, ColorHelper.ColorConverter.RgbToHex(new(background.R, background.G, background.B)).Value);
		BookmarkBtn.Content = !Global.Bookmarks.TextBookmarks.Contains(bookmarkText) ? "\uF1F6" : "\uF1F8";
	}

	private void CheckButton(Button button) => button.Background = Global.GetColorFromResource("LightAccentColor");

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

	private void Txt1_TextChanged(object sender, TextChangedEventArgs e)
	{
		try
		{
			ColorInfo = new ColorInfo(ConvertToRgb());
			ColorBorder.Background = new SolidColorBrush { Color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B) };
			ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B) };

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
					var split = text.Split(";");
					Txt1.Text = split[0];
					Txt2.Text = split[1];
					Txt3.Text = split[2];
				}

				e.Handled = true;
			}
		}
		catch { }
	}

	private void SelectColorBtn_Click(object sender, RoutedEventArgs e)
	{
		ColorSelector.IsOpen = false;
		var color = new SolidColorBrush { Color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B) }; // Set color

		if (isForeground)
		{
			foreground = System.Drawing.Color.FromArgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B);
			ForegroundBorder.Background = color;
			RegularTxt.Foreground = color; // Set foreground color
			ItalicTxt.Foreground = color; // Set foreground color
			BoldTxt.Foreground = color; // Set foreground color 
			return;
		}

		background = System.Drawing.Color.FromArgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B);
		BackgroundBorder.Background = color;
		RegularTxt.Background = color; // Set background color
		ItalicTxt.Background = color; // Set background color
		BoldTxt.Background = color; // Set background color 
		TextPanel.Background = color;

		LoadConstrastUI();
	}

	private void ColorBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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

	private void BookmarkBtn_Click(object sender, RoutedEventArgs e)
	{
		BookmarkText bookmarkText = new(FontComboBox.SelectedItem.ToString(), ColorHelper.ColorConverter.RgbToHex(new(foreground.R, foreground.G, foreground.B)).Value, ColorHelper.ColorConverter.RgbToHex(new(background.R, background.G, background.B)).Value);
		if (Global.Bookmarks.TextBookmarks.Contains(bookmarkText))
		{
			Global.Bookmarks.TextBookmarks.Remove(bookmarkText);
			BookmarkBtn.Content = "\uF1F6";
			BookmarkToolTip.Content = Properties.Resources.AddBookmark;

			return;
		}
		Global.Bookmarks.TextBookmarks.Add(bookmarkText); // Add to color bookmarks
		BookmarkBtn.Content = "\uF1F8";
		BookmarkToolTip.Content = Properties.Resources.RemoveBookmark;
	}

	private void ForegroundBorder_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
	{
		new ColorDetailsWindow((SolidColorBrush)((Border)sender).Background).Show();
	}

	private ColorHelper.RGB ConvertToRgb()
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

}
