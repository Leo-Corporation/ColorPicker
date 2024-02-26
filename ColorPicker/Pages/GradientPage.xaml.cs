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
/// Interaction logic for GradientPage.xaml
/// </summary>
public partial class GradientPage : Page
{
	bool code = !Global.Settings.UseSynethia; // checks if the code as already been implemented

	public GradientPage()
	{
		InitializeComponent();
		InitUI();

		Loaded += (o, e) => SynethiaManager.InjectSynethiaCode(this, Global.SynethiaConfig.PagesInfo, 5, ref code); // injects the code in the page
	}

	private void InitUI()
	{
		TitleTxt.Text = $"{Properties.Resources.Creation} > {Properties.Resources.Gradient}";
		GenerateRandomGradient();
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

	internal void GenerateRandomGradient()
	{
		from = Global.GenerateRandomColorDrawing();
		to = Global.GenerateRandomColorDrawing();
		ForegroundBorder.Background = new SolidColorBrush { Color = Color.FromRgb(from.R, from.G, from.B) };
		BackgroundBorder.Background = new SolidColorBrush { Color = Color.FromRgb(to.R, to.G, to.B) };

		LoadGradientUI();
		Global.SynethiaConfig.ActionsInfo[0].UsageCount++; // Increment the usage counter
	}

	internal void LoadGradientUI()
	{
		double.TryParse(RotateAngleTxt.Text, out double angle);
		GradientBorder.Background = new LinearGradientBrush
		{
			GradientStops =
			[
				new GradientStop(Color.FromRgb(from.R, from.G, from.B), 0),
				new GradientStop(Color.FromRgb(to.R, to.G, to.B), 1),
			],
			StartPoint = new(0.5, 1),
			EndPoint = new(0.5, 0),
			RelativeTransform = new RotateTransform()
			{
				Angle = double.IsNaN(angle) ? 0 : angle,
				CenterX = 0.5,
				CenterY = 0.5
			},
		};
		var color1 = ColorHelper.ColorConverter.RgbToHex(new(from.R, from.G, from.B));
		var color2 = ColorHelper.ColorConverter.RgbToHex(new(to.R, to.G, to.B));
		CssCodeTxt.Text = $"background: linear-gradient({angle}deg, rgba({from.R}, {from.G}, {from.B}, 1) 0%, rgba({to.R}, {to.G}, {to.B}, 1) 100%);";
		XamlCodeTxt.Text = $"<Rectangle Width=\"100\" Height=\"100\">\n" +
			$"\t<Rectangle.Fill>\n" +
			$"\t\t<LinearGradientBrush StartPoint=\"0.5,1\" EndPoint=\"0.5,0\">\n" +
			$"\t\t\t<GradientStop Color=\"#{color1}\" Offset=\"0\"/>\n" +
			$"\t\t\t<GradientStop Color=\"#{color2}\" Offset=\"1\"/>\n" +
			$"\t\t</LinearGradientBrush>\n" +
			$"\t</Rectangle.Fill>\n" +
			$"\t<Rectangle.RenderTransform>\n" +
			$"\t\t<RotateTransform Angle=\"{(double.IsNaN(angle) ? 0 : angle)}\" CenterX=\"0.5\" CenterY=\"0.5\"/>\n" +
			$"\t</Rectangle.RenderTransform>\n" +
			$"</Rectangle>";

		CurrentGradient = new(
			[
				new(ColorHelper.ColorConverter.RgbToHex(new(from.R, from.G, from.B)).Value, 0),
				new(ColorHelper.ColorConverter.RgbToHex(new(to.R, to.G, to.B)).Value, 1),
			],
		angle);

		// Load the bookmark icon
		if (!Global.Bookmarks.GradientBookmarks.Contains(CurrentGradient))
		{
			BookmarkBtn.Content = "\uF1F6";
			BookmarkToolTip.Content = Properties.Resources.AddBookmark;

			return;
		}
		BookmarkBtn.Content = "\uF1F8";
		BookmarkToolTip.Content = Properties.Resources.RemoveBookmark;
	}

	internal System.Drawing.Color from, to;
	Gradient CurrentGradient { get; set; }
	private void ForegroundBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		var bg = ForegroundBorder.Background as SolidColorBrush;
		ColorInfo = new(new(bg.Color.R, bg.Color.G, bg.Color.B));
		RgbBtn_Click(SelectedColorBtn, null);

		ColorSelector.IsOpen = true;
		isColor1 = true;
	}

	private void GenerateGradientBtn_Click(object sender, RoutedEventArgs e)
	{
		GenerateRandomGradient();
	}

	private void RotateAngleTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (GradientBorder is null) return;
		LoadGradientUI();
	}

	private void RotateAngleTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		Regex regex = new("[^0-9]+");
		e.Handled = regex.IsMatch(e.Text);
	}

	private void CopyCssBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(CssCodeTxt.Text);
	}

	private void BookmarkBtn_Click(object sender, RoutedEventArgs e)
	{
		if (Global.Bookmarks.GradientBookmarks.Contains(CurrentGradient))
		{
			Global.Bookmarks.GradientBookmarks.Remove(CurrentGradient);
			BookmarkBtn.Content = "\uF1F6";
			BookmarkToolTip.Content = Properties.Resources.AddBookmark;

			return;
		}
		Global.Bookmarks.GradientBookmarks.Add(CurrentGradient);
		BookmarkBtn.Content = "\uF1F8";
		BookmarkToolTip.Content = Properties.Resources.RemoveBookmark;
	}

	private void CopyXamlBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(XamlCodeTxt.Text);
	}

	private void BackgroundBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		var bg = BackgroundBorder.Background as SolidColorBrush;
		ColorInfo = new(new(bg.Color.R, bg.Color.G, bg.Color.B));
		RgbBtn_Click(SelectedColorBtn, null);

		ColorSelector.IsOpen = true;
		isColor1 = false;
	}
	ColorInfo ColorInfo { get; set; } = new(new(0, 0, 0));
	bool isColor1 = false;
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

	private void SelectColorBtn_Click(object sender, RoutedEventArgs e)
	{
		ColorSelector.IsOpen = false;
		var color = new SolidColorBrush { Color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B) }; // Set color

		if (isColor1)
		{
			from = System.Drawing.Color.FromArgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B);
			ForegroundBorder.Background = color;
			LoadGradientUI();
			return;
		}
		to = System.Drawing.Color.FromArgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B);
		BackgroundBorder.Background = color;
		LoadGradientUI();
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

	private void ForegroundBorder_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
	{
		new ColorDetailsWindow((SolidColorBrush)((Border)sender).Background).Show();
	}

	private void ExpandCssBtn_Click(object sender, RoutedEventArgs e)
	{
		CssBorder.Visibility = CssBorder.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
		ExpandCssBtn.Content = CssBorder.Visibility == Visibility.Visible ? "\uF2B7" : "\uF2A4";
	}

	private void ExpandXamlBtn_Click(object sender, RoutedEventArgs e)
	{
		XamlBorder.Visibility = XamlBorder.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
		ExpandXamlBtn.Content = XamlBorder.Visibility == Visibility.Visible ? "\uF2B7" : "\uF2A4";
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
