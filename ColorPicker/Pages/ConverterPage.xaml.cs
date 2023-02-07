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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for ConverterPage.xaml
/// </summary>
public partial class ConverterPage : Page
{
	public ConverterPage()
	{
		InitializeComponent();
		InitUI();
	}

	private void InitUI()
	{
		TitleTxt.Text = $"{Properties.Resources.ColorTools} > {Properties.Resources.Converter}";
		RgbBtn_Click(RgbBtn, null);
		try
		{
			(int r, int g, int b) = Global.GenerateRandomColor();
			Txt1.Text = r.ToString();
			Txt2.Text = g.ToString();
			Txt3.Text = b.ToString();
		}
		catch { }
	}

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
	}

	// Note: This event handler is used for all the choices
	private void RgbBtn_Click(object sender, RoutedEventArgs? e)
	{
		var btn = (Button)sender;

		UnCheckAllButtons();
		CheckButton(btn);
		SelectedColorBtn = btn;
		LoadInputUI();
	}

	private void CheckButton(Button button) => button.Background = new SolidColorBrush { Color = Global.GetColorFromResource("LightAccentColor") };
	ColorInfo ColorInfo { get; set; }

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
		else return ColorHelper.ColorConverter.YiqToRgb(new(double.Parse(Txt1.Text),
											 double.Parse(Txt2.Text),
											 double.Parse(Txt3.Text)));
	}

	private void LoadDetails()
	{
		ColorInfo = new ColorInfo(ConvertToRgb());
		RgbTxt.Text = $"{ColorInfo.RGB.R}; {ColorInfo.RGB.G}; {ColorInfo.RGB.B}";
		HexTxt.Text = $"#{ColorInfo.HEX.Value}";
		HsvTxt.Text = $"{ColorInfo.HSV.H}, {ColorInfo.HSV.S}, {ColorInfo.HSV.V}";
		HslTxt.Text = $"{ColorInfo.HSL.H}, {ColorInfo.HSL.S}, {ColorInfo.HSL.L}";
		CmykTxt.Text = $"{ColorInfo.CMYK.C}, {ColorInfo.CMYK.M}, {ColorInfo.CMYK.Y}, {ColorInfo.CMYK.K}";
		XyzTxt.Text = $"{ColorInfo.XYZ.X:0.00}.., {ColorInfo.XYZ.Y:0.00}.., {ColorInfo.XYZ.Z:0.00}..";
		YiqTxt.Text = $"{ColorInfo.YIQ.Y:0.00}.., {ColorInfo.YIQ.I:0.00}.., {ColorInfo.YIQ.Q:0.00}..";
		YuvTxt.Text = $"{ColorInfo.YUV.Y:0.00}.., {ColorInfo.YUV.U:0.00}.., {ColorInfo.YUV.V:0.00}..";

		ColorBorder.Background = new SolidColorBrush { Color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B) };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B) };

	}

	private void CopyYiqBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText($"{ColorInfo.YIQ.Y}, {ColorInfo.YIQ.I}, {ColorInfo.YIQ.Q}");
	}

	private void CopyXyzBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText($"{ColorInfo.XYZ.X}, {ColorInfo.XYZ.Y}, {ColorInfo.XYZ.Z}");
	}

	private void CopyCmykBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText($"{ColorInfo.CMYK.C}, {ColorInfo.CMYK.M}, {ColorInfo.CMYK.Y}, {ColorInfo.CMYK.K}");
	}

	private void CopyYuvBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText($"{ColorInfo.YUV.Y}, {ColorInfo.YUV.U}, {ColorInfo.YUV.V}");
	}

	private void CopyHslBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(HslTxt.Text);
	}

	private void CopyHsvBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(HsvTxt.Text);
	}

	private void CopyHexBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(HexTxt.Text);
	}

	private void CopyRgbBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(RgbTxt.Text);
	}

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
		if (SelectedColorBtn != HexBtn)
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
		}
		else if (SelectedColorBtn == HexBtn)
		{
			DisplayText5.Visibility = Visibility.Visible;

			DisplayText5.Text = Properties.Resources.HEX;
			B5.Visibility = Visibility.Visible;
		}
		else if (SelectedColorBtn == HsvBtn)
		{
			DisplayText1.Text = "H";
			DisplayText2.Text = "S";
			DisplayText3.Text = "V";
		}
		else if (SelectedColorBtn == HslBtn)
		{
			DisplayText1.Text = "H";
			DisplayText2.Text = "S";
			DisplayText3.Text = "L";
		}
		else if (SelectedColorBtn == CmykBtn)
		{
			DisplayText1.Text = "C";
			DisplayText2.Text = "M";
			DisplayText3.Text = "Y";
			DisplayText4.Text = "K";
		}
		else if (SelectedColorBtn == XyzBtn)
		{
			DisplayText1.Text = "X";
			DisplayText2.Text = "Y";
			DisplayText3.Text = "Z";
		}
		else if (SelectedColorBtn == YiqBtn)
		{
			DisplayText1.Text = "Y";
			DisplayText2.Text = "I";
			DisplayText3.Text = "Q";
		}
		else if (SelectedColorBtn == YuvBtn)
		{
			DisplayText1.Text = "Y";
			DisplayText2.Text = "U";
			DisplayText3.Text = "V";
		}
	}

	private void Txt1_TextChanged(object sender, TextChangedEventArgs e)
	{
		try
		{
			LoadDetails();
		}catch { }
	}
}
