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
using System.Text.RegularExpressions;
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
/// Interaction logic for TextPage.xaml
/// </summary>
public partial class TextPage : Page
{
	public TextPage()
	{
		InitializeComponent();
		InitUI();
	}

	private void InitUI()
	{
		TitleTxt.Text = $"{Properties.Resources.ColorTools} > {Properties.Resources.TextTool}";

		System.Drawing.Text.InstalledFontCollection installedFonts = new();
		foreach (System.Drawing.FontFamily fontFamily in installedFonts.Families)
		{
			FontComboBox.Items.Add(fontFamily.Name);
		}
		FontComboBox.Text = "Arial";

		// Set default Color
		foreground = System.Drawing.Color.Black;
		background = System.Drawing.Color.White;
		LoadConstrastUI();
	}

	private void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		try
		{
			RegularTxt.FontFamily = new(FontComboBox.SelectedItem.ToString()); // Set font family
			ItalicTxt.FontFamily = new(FontComboBox.SelectedItem.ToString()); // Set font family
			BoldTxt.FontFamily = new(FontComboBox.SelectedItem.ToString()); // Set font family
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
	private void ForegroundBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		System.Windows.Forms.ColorDialog colorDialog = new()
		{
			AllowFullOpen = true,
		}; // Create color picker/dialog

		if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) // If the user selected a color
		{
			var color = new SolidColorBrush { Color = Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B) }; // Set color
			foreground = colorDialog.Color;
			ForegroundBorder.Background = color;

			RegularTxt.Foreground = color; // Set foreground color
			ItalicTxt.Foreground = color; // Set foreground color
			BoldTxt.Foreground = color; // Set foreground color
		}
		LoadConstrastUI();
	}

	private void BackgroundBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		System.Windows.Forms.ColorDialog colorDialog = new()
		{
			AllowFullOpen = true,
		}; // Create color picker/dialog

		if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) // If the user selected a color
		{
			var color = new SolidColorBrush { Color = Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B) }; // Set color
			background = colorDialog.Color;
			BackgroundBorder.Background = color;

			RegularTxt.Background = color; // Set background color
			ItalicTxt.Background = color; // Set background color
			BoldTxt.Background = color; // Set background color
			TextPanel.Background = color; // Set background color
		}
		LoadConstrastUI();
	}

	private void LoadConstrastUI()
	{
		(string, int) contrast = Global.GetContrast(new int[] { foreground.R, foreground.G, foreground.B }, new int[] { background.R, background.G, background.B });
		Grid.SetRow(IndicatorArrow, contrast.Item2);

		ContrastTxt.Text = contrast.Item1;
	}
}
