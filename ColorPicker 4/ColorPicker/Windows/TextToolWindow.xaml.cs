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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorPicker.Windows;

/// <summary>
/// Interaction logic for TextToolWindow.xaml
/// </summary>
public partial class TextToolWindow : Window
{
	public TextToolWindow()
	{
		InitializeComponent();
		InitUI();
	}

	private void InitUI()
	{
		System.Drawing.Text.InstalledFontCollection installedFonts = new();
		foreach (System.Drawing.FontFamily fontFamily in installedFonts.Families)
		{
			FontComboBox.Items.Add(fontFamily.Name);
		}
		FontComboBox.Text = FontComboBox.Items.Contains(Global.Settings.TextToolFont) ? Global.Settings.TextToolFont : "Arial"; // Set default value

		FontSizeTxt.Text = Global.Settings.TextToolFontSize.ToString(); // Set the default font size
		foreground = Global.Settings.IsDarkTheme ? System.Drawing.Color.FromArgb(255, 255, 255) : System.Drawing.Color.FromArgb(0, 0, 0);
		background = Global.Settings.IsDarkTheme ? System.Drawing.Color.FromArgb(0, 0, 0) : System.Drawing.Color.FromArgb(255, 255, 255);

		if (Global.Settings.TextToolFontColor != "_default")
		{
			var rgbFore = ColorHelper.ColorConverter.HexToRgb(new(Global.Settings.TextToolFontColor));
			var foreColor = new SolidColorBrush { Color = Color.FromRgb(rgbFore.R, rgbFore.G, rgbFore.B) };
			foreground = System.Drawing.Color.FromArgb(rgbFore.R, rgbFore.G, rgbFore.B);

			RegularTxt.Foreground = foreColor; // Set the foreground color
			ItalicTxt.Foreground = foreColor; // Set foreground color
			BoldTxt.Foreground = foreColor; // Set foreground color

			ForegroundBorder.Background = foreColor; // Set the border color
		}

		if (Global.Settings.TextToolBackgroundColor != "_default")
		{
			var rgbBack = ColorHelper.ColorConverter.HexToRgb(new(Global.Settings.TextToolBackgroundColor));
			var backColor = new SolidColorBrush { Color = Color.FromRgb(rgbBack.R, rgbBack.G, rgbBack.B) };
			background = System.Drawing.Color.FromArgb(rgbBack.R, rgbBack.G, rgbBack.B);

			RegularTxt.Background = backColor; // Set the background color
			ItalicTxt.Background = backColor; // Set the background color
			BoldTxt.Background = backColor; // Set the background color

			BackgroundBorder.Background = backColor; // Set the border color
			TextPanel.Background = backColor; // Set background color

		}
		(ContrastTxt.Text, ContrastBorder.Background) = Global.GetContrast(new int[] { foreground.R, foreground.G, foreground.B }, new int[] { background.R, background.G, background.B });
	}

	private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
	{
		WindowState = WindowState.Minimized; // Minimize the window
	}

	private void CloseBtn_Click(object sender, RoutedEventArgs e)
	{
		Close(); // Close the window
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

	private void FontSizeTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		Regex regex = new("[^0-9]+");
		e.Handled = regex.IsMatch(e.Text);
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
			(ContrastTxt.Text, ContrastBorder.Background) = Global.GetContrast(new int[] { foreground.R, foreground.G, foreground.B }, new int[] { background.R, background.G, background.B });
		}
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
			(ContrastTxt.Text, ContrastBorder.Background) = Global.GetContrast(new int[] { foreground.R, foreground.G, foreground.B }, new int[] { background.R, background.G, background.B });
		}
	}
}
