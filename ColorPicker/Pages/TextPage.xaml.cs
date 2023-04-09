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
using Synethia;
using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for TextPage.xaml
/// </summary>
public partial class TextPage : Page
{
	bool code = Global.Settings.UseSynethia ? false : true; // checks if the code as already been implemented

	public TextPage()
	{
		InitializeComponent();
		InitUI();

		Loaded += (o, e) => SynethiaManager.InjectSynethiaCode(this, Global.SynethiaConfig.PagesInfo, 3, ref code); // injects the code in the page

	}

	private void InitUI()
	{
		TitleTxt.Text = $"{Properties.Resources.ColorTools} > {Properties.Resources.TextTool}";

		System.Drawing.Text.InstalledFontCollection installedFonts = new();
		foreach (System.Drawing.FontFamily fontFamily in installedFonts.Families)
		{
			FontComboBox.Items.Add(fontFamily.Name);
		}
		FontComboBox.Text = Global.Settings.TextToolFont;
		FontSizeTxt.Text = Global.Settings.TextToolFontSize.ToString();

		// Set default Color

		ColorHelper.RGB fgd = ColorHelper.ColorConverter.HexToRgb(new(Global.Settings.TextToolForeground));
		ColorHelper.RGB bgd = ColorHelper.ColorConverter.HexToRgb(new(Global.Settings.TextToolBackground));

		foreground = System.Drawing.Color.FromArgb(fgd.R, fgd.G, fgd.B);
		background = System.Drawing.Color.FromArgb(bgd.R, bgd.G, bgd.B);

		var foreBrush = new SolidColorBrush { Color = Color.FromRgb(fgd.R, fgd.G, fgd.B) }; // Set color
		var backBrush = new SolidColorBrush { Color = Color.FromRgb(bgd.R, bgd.G, bgd.B) }; // Set color

		RegularTxt.Foreground = foreBrush; // Set foreground color
		ItalicTxt.Foreground = foreBrush; // Set foreground color
		BoldTxt.Foreground = foreBrush; // Set foreground color

		RegularTxt.Background = backBrush; // Set background color
		ItalicTxt.Background = backBrush; // Set background color
		BoldTxt.Background = backBrush; // Set background color
		TextPanel.Background = backBrush; // Set background color

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
	Random random = new();

	private void GenerateGradientBtn_Click(object sender, System.Windows.RoutedEventArgs e)
	{
		// 1. Generate two random colors
		int n = random.Next(0, 2); // if n = 0, fg will be light; otherwise, fg will be dark

		var fg = n == 0 ? ColorHelper.ColorGenerator.GetLightRandomColor<ColorHelper.RGB>() : ColorHelper.ColorGenerator.GetDarkRandomColor<ColorHelper.RGB>();
		var bg = n == 0 ? ColorHelper.ColorGenerator.GetDarkRandomColor<ColorHelper.RGB>() : ColorHelper.ColorGenerator.GetLightRandomColor<ColorHelper.RGB>();

		foreground = System.Drawing.Color.FromArgb(fg.R, fg.G, fg.B);
		background = System.Drawing.Color.FromArgb(bg.R, bg.G, bg.B);

		SolidColorBrush fgBrush = new() { Color = Color.FromRgb(fg.R, fg.G, fg.B) };
		SolidColorBrush bgBrush = new() { Color = Color.FromRgb(bg.R, bg.G, bg.B) };

		// 2. Set the background and foreground color
		BackgroundBorder.Background = bgBrush;
		RegularTxt.Background = bgBrush; // Set background color
		ItalicTxt.Background = bgBrush; // Set background color
		BoldTxt.Background = bgBrush; // Set background color
		TextPanel.Background = bgBrush; // Set background color

		ForegroundBorder.Background = fgBrush;
		RegularTxt.Foreground = fgBrush; // Set Foreground color
		ItalicTxt.Foreground = fgBrush; // Set Foreground color
		BoldTxt.Foreground = fgBrush; // Set Foreground color

		// 3. Update the contrast UI
		LoadConstrastUI();
	}

	internal void LoadConstrastUI()
	{
		(string, int) contrast = Global.GetContrast(new int[] { foreground.R, foreground.G, foreground.B }, new int[] { background.R, background.G, background.B });
		Grid.SetRow(IndicatorArrow, contrast.Item2);

		ContrastTxt.Text = contrast.Item1;
		Global.SynethiaConfig.ActionsInfo[3].UsageCount++; // Increment the usage counter
	}
}
