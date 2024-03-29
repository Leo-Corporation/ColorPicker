﻿/*
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
using Synethia;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for ChromaticWheelPage.xaml
/// </summary>
public partial class ChromaticWheelPage : Page
{
	bool code = !Global.Settings.UseSynethia; // checks if the code as already been implemented

	public ChromaticWheelPage()
	{
		InitializeComponent();
		InitUI();

		Loaded += (o, e) => SynethiaManager.InjectSynethiaCode(this, Global.SynethiaConfig.PagesInfo, 1, ref code); // injects the code in the page
	}

	private void InitUI()
	{
		TitleTxt.Text = $"{Properties.Resources.Picker} > {Properties.Resources.ChromaticWheel}";
		CircleBtn_Click(this, null);
	}

	private void CircleBtn_Click(object sender, RoutedEventArgs e)
	{
		UnCheckAllButtons();
		CheckButton(CircleBtn);

		BitmapImage img = new();
		img.BeginInit();
		img.UriSource = new Uri("pack://application:,,,/ColorPicker;component/Images/ColorWheel.png");
		img.EndInit();

		WheelImg.Source = img; // Set the image source
	}

	internal void DiscBtn_Click(object sender, RoutedEventArgs e)
	{
		UnCheckAllButtons();
		CheckButton(DiscBtn);
		BitmapImage img = new();
		img.BeginInit();
		img.UriSource = new Uri("pack://application:,,,/ColorPicker;component/Images/ColorDisc2.png");
		img.EndInit();

		WheelImg.Source = img; // Set the image source
		Global.SynethiaConfig.ActionsInfo[1].UsageCount++; // Increment the usage counter
	}

	private void ShadesBtn_Click(object sender, RoutedEventArgs e)
	{
		UnCheckAllButtons();
		CheckButton(ShadesBtn);
		BitmapImage img = new();
		img.BeginInit();
		img.UriSource = new Uri("pack://application:,,,/ColorPicker;component/Images/ColorDisc.png");
		img.EndInit();

		WheelImg.Source = img; // Set the image source
	}

	private void WheelImg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		System.Drawing.Bitmap bitmap = new(1, 1);
		System.Drawing.Graphics GFX = System.Drawing.Graphics.FromImage(bitmap);
		GFX.CopyFromScreen(System.Windows.Forms.Cursor.Position, new System.Drawing.Point(0, 0), bitmap.Size);
		var pixel = bitmap.GetPixel(0, 0);
		LoadDetails(new(new(pixel.R, pixel.G, pixel.B)));
		PreviewBorder.Visibility = Visibility.Visible;
	}

	private void LoadDetails(ColorInfo colorInfo)
	{
		ColorBorder.Background = new SolidColorBrush { Color = Color.FromRgb(colorInfo.RGB.R, colorInfo.RGB.G, colorInfo.RGB.B) };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = Color.FromRgb(colorInfo.RGB.R, colorInfo.RGB.G, colorInfo.RGB.B) };

		ColorInfo = colorInfo;
		RgbTxt.Text = $"{colorInfo.RGB.R}{Global.Settings.RgbSeparator}{colorInfo.RGB.G}{Global.Settings.RgbSeparator}{colorInfo.RGB.B}";
		HexTxt.Text = $"#{colorInfo.HEX.Value}";
		HsvTxt.Text = $"{colorInfo.HSV.H}, {colorInfo.HSV.S}, {colorInfo.HSV.V}";
		HslTxt.Text = $"{colorInfo.HSL.H}, {colorInfo.HSL.S}, {colorInfo.HSL.L}";
		CmykTxt.Text = $"{colorInfo.CMYK.C}, {colorInfo.CMYK.M}, {colorInfo.CMYK.Y}, {colorInfo.CMYK.K}";
		XyzTxt.Text = $"{colorInfo.XYZ.X:0.00}..; {colorInfo.XYZ.Y:0.00}..; {colorInfo.XYZ.Z:0.00}..";
		YiqTxt.Text = $"{colorInfo.YIQ.Y:0.00}..; {colorInfo.YIQ.I:0.00}..; {colorInfo.YIQ.Q:0.00}..";
		YuvTxt.Text = $"{colorInfo.YUV.Y:0.00}..; {colorInfo.YUV.U:0.00}..; {colorInfo.YUV.V:0.00}..";
		DecTxt.Text = colorInfo.DEC.Value.ToString();
	}
	ColorInfo ColorInfo { get; set; } = new(new(0, 0, 0));
	private void UnCheckAllButtons()
	{
		CircleBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		DiscBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		ShadesBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
	}

	internal Button CheckedButton;
	internal void CheckButton(Button button) { button.Background = Global.GetColorFromResource("LightAccentColor"); CheckedButton = button; }

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
	public static event EventHandler<PageEventArgs> GoClick;

	private void PreviewBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Global.PalettePage.InitFromColor(ColorInfo);
		GoClick?.Invoke(this, new(AppPages.ColorPalette));
	}

	private void CopyDecBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(DecTxt.Text);
	}
}
