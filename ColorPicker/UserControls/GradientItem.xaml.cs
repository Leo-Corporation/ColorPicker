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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorPicker.UserControls;

/// <summary>
/// Interaction logic for GradientItem.xaml
/// </summary>
public partial class GradientItem : UserControl
{
	Gradient Gradient { get; init; }
	public GradientItem(Gradient gradient)
	{
		InitializeComponent();
		Gradient = gradient;

		InitUI();
	}

	private void InitUI()
	{
		// Color Border
		LinearGradientBrush linearGradientBrush = new()
		{
			StartPoint = new(0.5, 1),
			EndPoint = new(0.5, 0),
			RelativeTransform = new RotateTransform()
			{
				Angle = double.IsNaN(Gradient.Angle) ? 0 : Gradient.Angle,
				CenterX = 0.5,
				CenterY = 0.5
			},
		};

		for (int i = 0; i < Gradient.Stops.Count; i++)
		{
			ColorInfo colorInfo = new(ColorHelper.ColorConverter.HexToRgb(new(Gradient.Stops[i].Color)));
			Color color = Color.FromRgb(colorInfo.RGB.R, colorInfo.RGB.G, colorInfo.RGB.B);
			linearGradientBrush.GradientStops.Add(new(color, Gradient.Stops[i].Stop));

			// Text
			if (i == 0) FromTxt.Text = $"{colorInfo.RGB.R}{Global.Settings.RgbSeparator}{colorInfo.RGB.G}{Global.Settings.RgbSeparator}{colorInfo.RGB.B}";
			if (i == Gradient.Stops.Count - 1) ToTxt.Text = $"{colorInfo.RGB.R}{Global.Settings.RgbSeparator}{colorInfo.RGB.G}{Global.Settings.RgbSeparator}{colorInfo.RGB.B}";

		}

		ColorBorder.Background = linearGradientBrush;
		AngleTxt.Text = Gradient.Angle.ToString();
	}

	private void DeleteBtn_Click(object sender, RoutedEventArgs e)
	{
		Global.Bookmarks.GradientBookmarks.Remove(Gradient);
		Global.BookmarksPage.GradientsBookmarks.Children.Remove(this);
		Global.GradientPage.LoadGradientUI();
	}

	public static event EventHandler<PageEventArgs> GoClick;

	private void GoBtn_Click(object sender, RoutedEventArgs e)
	{
		RGB rgb = ColorHelper.ColorConverter.HexToRgb(new(Gradient.Stops[0].Color));
		RGB rgb2 = ColorHelper.ColorConverter.HexToRgb(new(Gradient.Stops[^1].Color));

		Global.GradientPage.from = System.Drawing.Color.FromArgb(rgb.R, rgb.G, rgb.B);
		Global.GradientPage.to = System.Drawing.Color.FromArgb(rgb2.R, rgb2.G, rgb2.B);

		Global.GradientPage.ForegroundBorder.Background = new SolidColorBrush { Color = Color.FromRgb(rgb.R, rgb.G, rgb.B) };
		Global.GradientPage.BackgroundBorder.Background = new SolidColorBrush { Color = Color.FromRgb(rgb2.R, rgb2.G, rgb2.B) };
		Global.GradientPage.RotateAngleTxt.Text = Gradient.Angle.ToString();

		Global.GradientPage.LoadGradientUI();

		GoClick?.Invoke(sender, new(Enums.AppPages.ColorGradient));
	}
}
