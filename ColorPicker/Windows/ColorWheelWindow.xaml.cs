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
using ColorPicker.Enums;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ColorPicker.Windows;

/// <summary>
/// Interaction logic for ColorWheelWindow.xaml
/// </summary>
public partial class ColorWheelWindow : Window
{
	bool isRunning, selectionMode = false;
	readonly TextBox ParentTextBox;
	readonly DispatcherTimer dispatcherTimer = new();

	int r, g, b; // RGB values
	string hex; // HEX value
	ColorTypes colorTypeForSelection;

	public ColorWheelWindow()
	{
		InitializeComponent();

		dispatcherTimer.Interval = new(0, 0, 0, 0, 1); // Interval
		dispatcherTimer.Tick += (o, e) =>
		{
			Bitmap bitmap = new(1, 1);
			Graphics GFX = Graphics.FromImage(bitmap);
			GFX.CopyFromScreen(System.Windows.Forms.Cursor.Position, new System.Drawing.Point(0, 0), bitmap.Size);
			var pixel = bitmap.GetPixel(0, 0);

			RedTxt.Text = $"{Properties.Resources.RedP} {pixel.R}"; // Set text
			GreenTxt.Text = $"{Properties.Resources.GreenP} {pixel.G}"; // Set text
			BlueTxt.Text = $"{Properties.Resources.BlueP} {pixel.B}"; // Set text

			r = pixel.R;
			g = pixel.G;
			b = pixel.B;

			// Convert to HEX
			bool u = Global.Settings.HEXUseUpperCase.Value;
			var h = ColorHelper.ColorConverter.RgbToHex(new(pixel.R, pixel.G, pixel.B)); // Convert
			HEXTxt.Text = $"{Properties.Resources.HEXP} #{(u ? h.Value.ToUpper() : h.Value.ToLower())}";
			hex = $"#{(u ? h.Value.ToUpper() : h.Value.ToLower())}";

			ColorDisplayer.Background = new SolidColorBrush { Color = System.Windows.Media.Color.FromRgb(pixel.R, pixel.G, pixel.B) }; // Set color
		};
	}

	private void UncheckAllTabs()
	{
		WheelBtn.Foreground = new SolidColorBrush { Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(App.Current.Resources["Foreground1"].ToString()) }; // Set the color back to its default value
		DiscBtn.Foreground = new SolidColorBrush { Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(App.Current.Resources["Foreground1"].ToString()) }; // Set the color back to its default value
		PaletteBtn.Foreground = new SolidColorBrush { Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(App.Current.Resources["Foreground1"].ToString()) }; // Set the color back to its default value

		WheelBtn.Background = new SolidColorBrush { Color = Colors.Transparent }; // Remove the background color
		DiscBtn.Background = new SolidColorBrush { Color = Colors.Transparent }; // Remove the background color
		PaletteBtn.Background = new SolidColorBrush { Color = Colors.Transparent }; // Remove the background color
	}

	public ColorWheelWindow(bool selectMode, TextBox textBox, ColorTypes colorType = ColorTypes.RGB)
	{
		InitializeComponent();

		selectionMode = selectMode; // Set selection mode
		ParentTextBox = textBox; // Set textbox
		colorTypeForSelection = colorType; // Set color type

		dispatcherTimer.Interval = new(0, 0, 0, 0, 1); // Interval
		dispatcherTimer.Tick += (o, e) =>
		{
			Bitmap bitmap = new(1, 1);
			Graphics GFX = Graphics.FromImage(bitmap);
			GFX.CopyFromScreen(System.Windows.Forms.Cursor.Position, new System.Drawing.Point(0, 0), bitmap.Size);
			var pixel = bitmap.GetPixel(0, 0);

			RedTxt.Text = $"{Properties.Resources.RedP} {pixel.R}"; // Set text
			GreenTxt.Text = $"{Properties.Resources.GreenP} {pixel.G}"; // Set text
			BlueTxt.Text = $"{Properties.Resources.BlueP} {pixel.B}"; // Set text

			r = pixel.R;
			g = pixel.G;
			b = pixel.B;

			// Convert to HEX
			bool u = Global.Settings.HEXUseUpperCase.Value;
			var h = ColorHelper.ColorConverter.RgbToHex(new(pixel.R, pixel.G, pixel.B)); // Convert
			HEXTxt.Text = $"{Properties.Resources.HEXP} #{(u ? h.Value.ToUpper() : h.Value.ToLower())}";
			hex = $"#{(u ? h.Value.ToUpper() : h.Value.ToLower())}";

			ColorDisplayer.Background = new SolidColorBrush { Color = System.Windows.Media.Color.FromRgb(pixel.R, pixel.G, pixel.B) }; // Set color
		};
	}

	private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
	{
		WindowState = WindowState.Minimized; // Minimize the window
	}

	private void CloseBtn_Click(object sender, RoutedEventArgs e)
	{
		Close(); // Close the window
	}

	private void SelectColorBtn_Click(object sender, RoutedEventArgs e)
	{
		if (!selectionMode)
		{
			ColorDisplayer.Visibility = Visibility.Visible; // Show
			ColorInforPanel.Visibility = Visibility.Visible; // Show

			CopyBtn.Visibility = Visibility.Visible; // Show
			CopyHEXBtn.Visibility = Visibility.Visible; // Show

			if (!isRunning)
			{
				dispatcherTimer.Start(); // Start
				SelectColorBtn.Content = Properties.Resources.Stop; // Set text
				isRunning = true;
			}
			else
			{
				dispatcherTimer.Stop(); // Stop
				SelectColorBtn.Content = Properties.Resources.SelectColor; // Set text
				isRunning = false;
			}
		}
		else
		{
			ParentTextBox.Text = colorTypeForSelection switch
			{
				ColorTypes.HEX => hex, // Set text with HEX color
				_ => $"{r}{Global.Settings.RGBSeparator}{g}{Global.Settings.RGBSeparator}{b}" // Set text with RGB color
			};
			Close(); // Close
		}
	}

	private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ColorDisplayer.Visibility = Visibility.Visible; // Show
		ColorInforPanel.Visibility = Visibility.Visible; // Show

		CopyBtn.Visibility = Visibility.Visible; // Show
		CopyHEXBtn.Visibility = Visibility.Visible; // Show
		SelectColorBtn.Content = Properties.Resources.SelectColor; // Set text

		dispatcherTimer.Stop(); // Stop
		isRunning = false;

		Bitmap bitmap = new(1, 1);
		Graphics GFX = Graphics.FromImage(bitmap);
		GFX.CopyFromScreen(System.Windows.Forms.Cursor.Position, new System.Drawing.Point(0, 0), bitmap.Size);
		var pixel = bitmap.GetPixel(0, 0);

		RedTxt.Text = $"{Properties.Resources.RedP} {pixel.R}"; // Set text
		GreenTxt.Text = $"{Properties.Resources.GreenP} {pixel.G}"; // Set text
		BlueTxt.Text = $"{Properties.Resources.BlueP} {pixel.B}"; // Set text

		r = pixel.R;
		g = pixel.G;
		b = pixel.B;

		// Convert to HEX
		bool u = Global.Settings.HEXUseUpperCase.Value;
		var h = ColorHelper.ColorConverter.RgbToHex(new(pixel.R, pixel.G, pixel.B)); // Convert
		HEXTxt.Text = $"{Properties.Resources.HEXP} #{(u ? h.Value.ToUpper() : h.Value.ToLower())}";
		hex = $"#{(u ? h.Value.ToUpper() : h.Value.ToLower())}";

		ColorDisplayer.Background = new SolidColorBrush { Color = System.Windows.Media.Color.FromRgb(pixel.R, pixel.G, pixel.B) }; // Set color
	}

	private void Image_MouseLeave(object sender, MouseEventArgs e)
	{
		dispatcherTimer.Stop(); // Stop
	}

	private void WheelBtn_Click(object sender, RoutedEventArgs e)
	{
		UncheckAllTabs(); // Uncheck all tabs
		WheelBtn.Foreground = new SolidColorBrush { Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(App.Current.Resources["WindowButtonsHoverForeground1"].ToString()) }; // Set the color back to its default value
		WheelBtn.Background = new SolidColorBrush { Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(App.Current.Resources["AccentColor"].ToString()) }; // Set the color back to its default value

		BitmapImage img = new();
		img.BeginInit();
		img.UriSource = new Uri("pack://application:,,,/ColorPicker;component/Images/ColorWheel.png");
		img.EndInit();

		WheelImg.Source = img; // Set the image source
	}

	private void PaletteBtn_Click(object sender, RoutedEventArgs e)
	{
		UncheckAllTabs(); // Uncheck all tabs
		PaletteBtn.Foreground = new SolidColorBrush { Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(App.Current.Resources["WindowButtonsHoverForeground1"].ToString()) }; // Set the color back to its default value
		PaletteBtn.Background = new SolidColorBrush { Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(App.Current.Resources["AccentColor"].ToString()) }; // Set the color back to its default value

		BitmapImage img = new();
		img.BeginInit();
		img.UriSource = new Uri("pack://application:,,,/ColorPicker;component/Images/ColorDisc.png");
		img.EndInit();

		WheelImg.Source = img; // Set the image source

	}

	private void DiscBtn_Click(object sender, RoutedEventArgs e)
	{
		UncheckAllTabs(); // Uncheck all tabs
		DiscBtn.Foreground = new SolidColorBrush { Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(App.Current.Resources["WindowButtonsHoverForeground1"].ToString()) }; // Set the color back to its default value
		DiscBtn.Background = new SolidColorBrush { Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(App.Current.Resources["AccentColor"].ToString()) }; // Set the color back to its default value

		BitmapImage img = new();
		img.BeginInit();
		img.UriSource = new Uri("pack://application:,,,/ColorPicker;component/Images/ColorDisc2.png");
		img.EndInit();

		WheelImg.Source = img; // Set the image source
	}

	private void Image_MouseEnter(object sender, MouseEventArgs e)
	{
		if (isRunning)
		{
			dispatcherTimer.Start();
		}
	}

	private void CopyBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText($"{r}{Global.Settings.RGBSeparator}{g}{Global.Settings.RGBSeparator}{b}"); // Copy
	}

	private void CopyHEXBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(hex); // Copy HEX
	}
}
