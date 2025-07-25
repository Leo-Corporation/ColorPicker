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
using ColorHelper;
using ColorPicker.Classes;
using ColorPicker.Enums;
using ColorPicker.Windows;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ColorPicker.UserControls;

/// <summary>
/// Interaction logic for PaletteItem.xaml
/// </summary>
public partial class PaletteItem : UserControl
{
	string HexColor { get; init; }
	ColorInfo ColorInfo { get; set; }
	public PaletteItem(string hexColor)
	{
		InitializeComponent();
		HexColor = hexColor;
		ColorInfo = new(ColorHelper.ColorConverter.HexToRgb(new(HexColor)));

		InitUI();
	}

	private void InitUI()
	{
		// Intitial Color
		Color color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B);
		ColorBorder.Background = new SolidColorBrush { Color = color };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = color };
		ColorBorder.ToolTip = new ToolTip()
		{
			Background = Global.GetColorFromResource("Background1"),
			Foreground = Global.GetColorFromResource("Foreground1"),
			Content = ColorInfo.ToString()
		};

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
					Clipboard.SetDataObject(Global.Settings.DefaultColorType switch
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
	}

	private void ColorBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		Clipboard.SetDataObject(Global.Settings.DefaultColorType switch
		{
			ColorTypes.HEX => ColorInfo.HEX.Value,
			ColorTypes.HSV => $"{ColorInfo.HSV.H},{ColorInfo.HSV.S},{ColorInfo.HSV.V}",
			ColorTypes.HSL => $"{ColorInfo.HSL.H},{ColorInfo.HSL.S},{ColorInfo.HSL.L}",
			ColorTypes.CMYK => $"{ColorInfo.CMYK.C},{ColorInfo.CMYK.M},{ColorInfo.CMYK.Y},{ColorInfo.CMYK.K}",
			ColorTypes.XYZ => $"{ColorInfo.XYZ.X}; {ColorInfo.XYZ.Y}; {ColorInfo.XYZ.Z}",
			ColorTypes.YIQ => $"{ColorInfo.YIQ.Y}; {ColorInfo.YIQ.I}; {ColorInfo.YIQ.Q}",
			ColorTypes.YUV => $"{ColorInfo.YUV.Y}; {ColorInfo.YUV.U}; {ColorInfo.YUV.V}",
			ColorTypes.DEC => ColorInfo.DEC.Value.ToString(),
			_ => $"{ColorInfo.RGB.R}{Global.Settings.RgbSeparator}{ColorInfo.RGB.G}{Global.Settings.RgbSeparator}{ColorInfo.RGB.B}"
		});
	}

	private void DeleteBtn_Click(object sender, RoutedEventArgs e)
	{
		Global.Bookmarks.PaletteBookmarks.Remove(HexColor);
		Global.BookmarksPage.PalettesBookmarks.Children.Remove(this);
		Global.PalettePage.InitPaletteUI();
		Global.BookmarksPage.PaletteBtn_Click(sender, e);
	}

	public static event EventHandler<PageEventArgs> GoClick;

	private void GoBtn_Click(object sender, RoutedEventArgs e)
	{
		Global.PalettePage.RgbBtn_Click(Global.PalettePage.HexBtn, e);
		Global.PalettePage.Txt5.Text = HexColor;
		GoClick?.Invoke(this, new(Enums.AppPages.ColorPalette));
	}
}
