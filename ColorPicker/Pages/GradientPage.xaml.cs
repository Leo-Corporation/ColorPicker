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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for GradientPage.xaml
/// </summary>
public partial class GradientPage : Page
{
	bool code = Global.Settings.UseSynethia ? false : true; // checks if the code as already been implemented

	public GradientPage()
	{
		InitializeComponent();
		InitUI();

		Loaded += (o, e) => SynethiaManager.InjectSynethiaCode(this, Global.SynethiaConfig.PagesInfo, 5, ref code); // injects the code in the page
	}

	private void InitUI()
	{
		TitleTxt.Text = $"{Properties.Resources.Creation} > {Properties.Resources.Gradient}";
		GenerateRandomGradient();
	}

	internal void GenerateRandomGradient()
	{
		from = Global.GenerateRandomColorDrawing();
		to = Global.GenerateRandomColorDrawing();
		ForegroundBorder.Background = new SolidColorBrush { Color = Color.FromRgb(from.R, from.G, from.B) };
		BackgroundBorder.Background = new SolidColorBrush { Color = Color.FromRgb(to.R, to.G, to.B) };

		LoadGradientUI();
		Global.SynethiaConfig.ActionsInfo[0].UsageCount++; // Increment the usage counter
	}

	internal void LoadGradientUI()
	{
		double.TryParse(RotateAngleTxt.Text, out double angle);
		GradientBorder.Background = new LinearGradientBrush
		{
			GradientStops = new GradientStopCollection
			{
				new GradientStop(Color.FromRgb(from.R, from.G, from.B), 0),
				new GradientStop(Color.FromRgb(to.R, to.G, to.B), 1),
			},
			StartPoint = new(0.5, 1),
			EndPoint = new(0.5, 0),
			RelativeTransform = new RotateTransform()
			{
				Angle = double.IsNaN(angle) ? 0 : angle,
				CenterX = 0.5,
				CenterY = 0.5
			},
		};
		var color1 = ColorHelper.ColorConverter.RgbToHex(new(from.R, from.G, from.B));
		var color2 = ColorHelper.ColorConverter.RgbToHex(new(to.R, to.G, to.B));
		CssCodeTxt.Text = $"background: linear-gradient({angle}deg, rgba({from.R}, {from.G}, {from.B}, 1) 0%, rgba({to.R}, {to.G}, {to.B}, 1) 100%);";
		XamlCodeTxt.Text = $"<Rectangle Width=\"100\" Height=\"100\">\n" +
			$"\t<Rectangle.Fill>\n" +
			$"\t\t<LinearGradientBrush StartPoint=\"0.5,1\" EndPoint=\"0.5,0\">\n" +
			$"\t\t\t<GradientStop Color=\"#{color1}\" Offset=\"0\"/>\n" +
			$"\t\t\t<GradientStop Color=\"#{color2}\" Offset=\"1\"/>\n" +
			$"\t\t</LinearGradientBrush>\n" +
			$"\t</Rectangle.Fill>\n" +
			$"\t<Rectangle.RenderTransform>\n" +
			$"\t\t<RotateTransform Angle=\"{(double.IsNaN(angle) ? 0 : angle)}\" CenterX=\"0.5\" CenterY=\"0.5\"/>\n" +
			$"\t</Rectangle.RenderTransform>\n" +
			$"</Rectangle>";

		CurrentGradient = new(
			new()
			{
				new(ColorHelper.ColorConverter.RgbToHex(new(from.R, from.G, from.B)).Value, 0),
				new(ColorHelper.ColorConverter.RgbToHex(new(to.R, to.G, to.B)).Value, 1),
			},
		angle);

		// Load the bookmark icon
		if (!Global.Bookmarks.GradientBookmarks.Contains(CurrentGradient))
		{
			BookmarkBtn.Content = "\uF1F6";
			return;
		}
		BookmarkBtn.Content = "\uF1F8";
	}

	internal System.Drawing.Color from, to;
	Gradient CurrentGradient { get; set; }
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

	private void RotateAngleTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (GradientBorder is null) return;
		LoadGradientUI();
	}

	private void RotateAngleTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		Regex regex = new("[^0-9]+");
		e.Handled = regex.IsMatch(e.Text);
	}

	private void CopyCssBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(CssCodeTxt.Text);
	}

	private void BookmarkBtn_Click(object sender, RoutedEventArgs e)
	{
		if (Global.Bookmarks.GradientBookmarks.Contains(CurrentGradient))
		{
			Global.Bookmarks.GradientBookmarks.Remove(CurrentGradient);
			BookmarkBtn.Content = "\uF1F6";
			return;
		}
		Global.Bookmarks.GradientBookmarks.Add(CurrentGradient);
		BookmarkBtn.Content = "\uF1F8";
	}

	private void CopyXamlBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(XamlCodeTxt.Text);
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
