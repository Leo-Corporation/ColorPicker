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
using ColorPicker.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorPicker.UserControls;

/// <summary>
/// Interaction logic for ColorGridItem.xaml
/// </summary>
public partial class ColorGridItem : UserControl
{

	public RGB ForegroundColor { get; }
	public RGB BackgroundColor { get; }

	public ColorGridItem(HSL foregroundColor, HSL backgroundColor, double limit)
	{
		InitializeComponent();
		ForegroundColor = ColorHelper.ColorConverter.HslToRgb(foregroundColor);
		BackgroundColor = ColorHelper.ColorConverter.HslToRgb(backgroundColor);

		InitUI(limit);
	}

	private void InitUI(double limit)
	{
		double contrast = Global.GetContrastDouble([ForegroundColor.R, ForegroundColor.G, ForegroundColor.B], [BackgroundColor.R, BackgroundColor.G, BackgroundColor.B]);
		if (contrast < limit)
		{
			ColorBorder.Background = Global.GetColorFromResource("Background2");
			return;
		}

		ColorBorder.Background = new SolidColorBrush { Color = Color.FromRgb(BackgroundColor.R, BackgroundColor.G, BackgroundColor.B) };
		RatioTxt.Foreground = new SolidColorBrush { Color = Color.FromRgb(ForegroundColor.R, ForegroundColor.G, ForegroundColor.B) };

		BackgroundTooltip.Text = new ColorInfo(new(BackgroundColor.R, BackgroundColor.G, BackgroundColor.B)).ToString();
		ForegroundTooltip.Text = new ColorInfo(new(ForegroundColor.R, ForegroundColor.G, ForegroundColor.B)).ToString();

		RatioTxt.Text = contrast.ToString();
	}

	private void ForegroundDetails_Click(object sender, RoutedEventArgs e)
	{
		new ColorDetailsWindow(new SolidColorBrush { Color = Color.FromRgb(ForegroundColor.R, ForegroundColor.G, ForegroundColor.B) }).Show();
	}

	private void BackgroundDetails_Click(object sender, RoutedEventArgs e)
	{
		new ColorDetailsWindow(new SolidColorBrush { Color = Color.FromRgb(BackgroundColor.R, BackgroundColor.G, BackgroundColor.B) }).Show();
	}
}
