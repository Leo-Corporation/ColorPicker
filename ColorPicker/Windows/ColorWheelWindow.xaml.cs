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
using LeoCorpLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ColorPicker.Windows
{
	/// <summary>
	/// Interaction logic for ColorWheelWindow.xaml
	/// </summary>
	public partial class ColorWheelWindow : Window
	{
		bool isRunning = false;
		readonly DispatcherTimer dispatcherTimer = new();
		public ColorWheelWindow()
		{
			InitializeComponent();

			dispatcherTimer.Interval = new(0, 0, 0, 0, 1); // Interval
			dispatcherTimer.Tick += (o, e) =>
			{
				Bitmap bitmap = new(1, 1);
				Graphics GFX = Graphics.FromImage(bitmap);
				GFX.CopyFromScreen(Env.GetMouseCursorPosition(), new System.Drawing.Point(0, 0), bitmap.Size);
				var pixel = bitmap.GetPixel(0, 0);

				RedTxt.Text = $"{Properties.Resources.RedP} {pixel.R}"; // Set text
				GreenTxt.Text = $"{Properties.Resources.GreenP} {pixel.G}"; // Set text
				BlueTxt.Text = $"{Properties.Resources.BlueP} {pixel.B}"; // Set text

				// Convert to HEX
				bool u = Global.Settings.HEXUseUpperCase.Value;
				var h = ColorsConverter.RGBtoHEX(pixel.R, pixel.G, pixel.B); // Convert
				HEXTxt.Text = $"{Properties.Resources.HEXP} #{(u ? h.Value.ToUpper() : h.Value.ToLower())}";

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
			ColorDisplayer.Visibility = Visibility.Visible; // Show
			ColorInforPanel.Visibility = Visibility.Visible; // Show

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
	}
}
