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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for SelectorPage.xaml
/// </summary>
public partial class SelectorPage : Page
{
	readonly DispatcherTimer timer = new();
	public SelectorPage()
	{
		InitializeComponent();
		InitUI();
	}

	private void InitUI()
	{
		TitleTxt.Text = $"{Properties.Resources.Picker} > {Properties.Resources.Selector}";

		timer.Interval = new(0, 0, 0, 0, 1); // Interval
		timer.Tick += (o, e) =>
		{
			// Get the pixel from the screen
			System.Drawing.Bitmap bitmap = new(1, 1); // Create a bitmap where the color of the pixel is going to be copied
			System.Drawing.Graphics GFX = System.Drawing.Graphics.FromImage(bitmap);
			GFX.CopyFromScreen(System.Windows.Forms.Cursor.Position, new System.Drawing.Point(0, 0), bitmap.Size); // Get the color of the pixel at the mouse position
			var pixel = bitmap.GetPixel(0, 0); // Copy to the bitmap

			RedSlider.Value = pixel.R; // Set value
			GreenSlider.Value = pixel.G; // Set value
			BlueSlider.Value = pixel.B; // Set value
		};
	}

	private void RedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		ColorBorder.Background = new SolidColorBrush { Color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value) };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value) };
		RedValueTxt.Text = RedSlider.Value.ToString(); // Set text
	}

	private void GreenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		ColorBorder.Background = new SolidColorBrush { Color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value) };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value) };
		GreenValueTxt.Text = GreenSlider.Value.ToString(); // Set text
	}

	private void BlueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		ColorBorder.Background = new SolidColorBrush { Color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value) };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value) };
		BlueValueTxt.Text = BlueSlider.Value.ToString(); // Set text
	}

	bool selecting = false;
	private void SelectBtn_Click(object sender, RoutedEventArgs e)
	{
		if (selecting) timer.Stop();
		else timer.Start();
		selecting = !selecting;
	}

	private void ColorBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		(RedSlider.Value, GreenSlider.Value, BlueSlider.Value) = Global.GenerateRandomColor();
	}
}
