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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for GradientPage.xaml
/// </summary>
public partial class GradientPage : Page
{
	public GradientPage()
	{
		InitializeComponent();
		InitUI();
	}

	private void InitUI()
	{
		TitleTxt.Text = $"{Properties.Resources.Creation} > {Properties.Resources.Gradient}";
		GenerateRandomGradient();
	}

	private void GenerateRandomGradient()
	{
		from = Global.GenerateRandomColorDrawing();
		to = Global.GenerateRandomColorDrawing();
		ForegroundBorder.Background = new SolidColorBrush { Color = Color.FromRgb(from.R, from.G, from.B) };
		BackgroundBorder.Background = new SolidColorBrush { Color = Color.FromRgb(to.R, to.G, to.B) };

		LoadGradientUI();
	}

	private void LoadGradientUI()
	{
		GradientBorder.Background = new LinearGradientBrush
		{
			GradientStops = new GradientStopCollection
			{
				new GradientStop(Color.FromRgb(from.R, from.G, from.B), 0),
				new GradientStop(Color.FromRgb(to.R, to.G, to.B), 1),
			},
		};
	}

	System.Drawing.Color from, to;
	private void ForegroundBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		System.Windows.Forms.ColorDialog colorDialog = new()
		{
			AllowFullOpen = true,
			FullOpen = true
		}; // Create color picker/dialog

		if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) // If the user selected a color
		{
			var color = new SolidColorBrush { Color = Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B) }; // Set color
			from = colorDialog.Color;
			ForegroundBorder.Background = color;
		}
		LoadGradientUI();
	}

	private void GenerateGradientBtn_Click(object sender, RoutedEventArgs e)
	{
		GenerateRandomGradient();
	}

	private void BackgroundBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		System.Windows.Forms.ColorDialog colorDialog = new()
		{
			AllowFullOpen = true,
			FullOpen = true
		}; // Create color picker/dialog

		if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) // If the user selected a color
		{
			var color = new SolidColorBrush { Color = Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B) }; // Set color
			to = colorDialog.Color;
			BackgroundBorder.Background = color;
		}
		LoadGradientUI();
	}
}
