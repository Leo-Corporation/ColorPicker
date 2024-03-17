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
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorPicker.Windows;
/// <summary>
/// Interaction logic for ColorDetailsWindow.xaml
/// </summary>
public partial class ColorDetailsWindow : Window
{
	ColorInfo ColorInfo { get; init; }
	SolidColorBrush BackgroundSolidBrush { get; init; }
	SolidColorBrush HoverForegroundSolidBrush { get; set; }
	public ColorDetailsWindow(SolidColorBrush color)
	{
		InitializeComponent();
		ColorInfo = new(new(color.Color.R, color.Color.G, color.Color.B));
		BackgroundSolidBrush = color;
		InitUI();
	}

	private void InitUI()
	{
		WindowBorder.Background = BackgroundSolidBrush;
		var bgBtn = IsColorDark(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B) ? new SolidColorBrush { Color = ChangeBrightness(BackgroundSolidBrush.Color, 0.5f) } : new SolidColorBrush { Color = ChangeBrightness(BackgroundSolidBrush.Color, -0.5f) };
		Foreground = bgBtn;
		HoverForegroundSolidBrush = new SolidColorBrush(Color.FromArgb(50, bgBtn.Color.R, bgBtn.Color.G, bgBtn.Color.B)) ?? BackgroundSolidBrush;

		CopyRgbBtn.Background = bgBtn;
		CopyHexBtn.Background = bgBtn;
		CopyHsvBtn.Background = bgBtn;
		CopyHslBtn.Background = bgBtn;
		CopyCmykBtn.Background = bgBtn;
		CopyDecBtn.Background = bgBtn;
		CopyXyzBtn.Background = bgBtn;
		CopyYiqBtn.Background = bgBtn;
		CopyYuvBtn.Background = bgBtn;

		MinimizeBtn.Foreground = bgBtn;
		CloseBtn.Foreground = bgBtn;

		CopyRgbBtn.Foreground = BackgroundSolidBrush;
		CopyHexBtn.Foreground = BackgroundSolidBrush;
		CopyHsvBtn.Foreground = BackgroundSolidBrush;
		CopyHslBtn.Foreground = BackgroundSolidBrush;
		CopyCmykBtn.Foreground = BackgroundSolidBrush;
		CopyDecBtn.Foreground = BackgroundSolidBrush;
		CopyXyzBtn.Foreground = BackgroundSolidBrush;
		CopyYiqBtn.Foreground = BackgroundSolidBrush;
		CopyYuvBtn.Foreground = BackgroundSolidBrush;

		// Details
		RgbTxt.Text = $"{ColorInfo.RGB.R}{Global.Settings.RgbSeparator}{ColorInfo.RGB.G}{Global.Settings.RgbSeparator}{ColorInfo.RGB.B}";
		HexTxt.Text = $"#{ColorInfo.HEX.Value}";
		HsvTxt.Text = $"{ColorInfo.HSV.H}, {ColorInfo.HSV.S}, {ColorInfo.HSV.V}";
		HslTxt.Text = $"{ColorInfo.HSL.H}, {ColorInfo.HSL.S}, {ColorInfo.HSL.L}";
		CmykTxt.Text = $"{ColorInfo.CMYK.C}, {ColorInfo.CMYK.M}, {ColorInfo.CMYK.Y}, {ColorInfo.CMYK.K}";
		DecTxt.Text = $"{ColorInfo.DEC.Value}";
		XyzTxt.Text = $"{ColorInfo.XYZ.X}; {ColorInfo.XYZ.Y}; {ColorInfo.XYZ.Z}";
		YiqTxt.Text = $"{ColorInfo.YIQ.Y}; {ColorInfo.YIQ.I}; {ColorInfo.YIQ.Q}";
		YuvTxt.Text = $"{ColorInfo.YUV.Y}; {ColorInfo.YUV.U}; {ColorInfo.YUV.V}";
	}

	public static bool IsColorDark(int red, int green, int blue)
	{
		double brightness = Math.Sqrt(
			red * red * .241 +
			green * green * .691 +
			blue * blue * .068);

		return brightness < 130;
	}


	public static Color ChangeBrightness(Color color, float factor)
	{
		float r = color.R / 255f;
		float g = color.G / 255f;
		float b = color.B / 255f;

		float adjustedR = Math.Clamp(r + factor, 0f, 1f);
		float adjustedG = Math.Clamp(g + factor, 0f, 1f);
		float adjustedB = Math.Clamp(b + factor, 0f, 1f);

		return Color.FromRgb((byte)(adjustedR * 255), (byte)(adjustedG * 255), (byte)(adjustedB * 255));
	}

	private void CopyYiqBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText($"{ColorInfo.YIQ.Y}; {ColorInfo.YIQ.I}; {ColorInfo.YIQ.Q}");
	}

	private void CopyXyzBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText($"{ColorInfo.XYZ.X}; {ColorInfo.XYZ.Y}; {ColorInfo.XYZ.Z}");
	}

	private void CopyCmykBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText($"{ColorInfo.CMYK.C}, {ColorInfo.CMYK.M}, {ColorInfo.CMYK.Y}, {ColorInfo.CMYK.K}");
	}

	private void CopyYuvBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText($"{ColorInfo.YUV.Y}; {ColorInfo.YUV.U}; {ColorInfo.YUV.V}");
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

	private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
	{
		WindowState = WindowState.Minimized;
	}

	private void CloseBtn_Click(object sender, RoutedEventArgs e)
	{
		Close();
	}

	private void Border_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
	{
		((Border)sender).Background = HoverForegroundSolidBrush;
	}

	private void Border_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
	{
		((Border)sender).Background = null;
	}

	private void CopyDecBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(DecTxt.Text);
	}
}
