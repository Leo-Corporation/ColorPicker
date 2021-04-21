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
using ColorPicker.Windows;
using Gma.System.MouseKeyHook;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;

namespace ColorPicker.Pages
{
	/// <summary>
	/// Interaction logic for PickerPage.xaml
	/// </summary>
	public partial class PickerPage : Page
	{
		bool isRunning = false;
		private IKeyboardMouseEvents m_GlobalHook;
		DispatcherTimer dispatcherTimer = new();
		public PickerPage()
		{
			InitializeComponent();
			m_GlobalHook = Hook.GlobalEvents();
			InitUI(); // Init UI
			dispatcherTimer.Interval = new(0, 0, 0, 0, 1); // Interval
			dispatcherTimer.Tick += (o, e) =>
			{
				Bitmap bitmap = new(1, 1);
				Graphics GFX = Graphics.FromImage(bitmap);
				GFX.CopyFromScreen(Env.GetMouseCursorPosition(), new System.Drawing.Point(0, 0), bitmap.Size);
				var pixel = bitmap.GetPixel(0, 0);

				ColorDisplayer.Background = new SolidColorBrush { Color = Color.FromRgb(pixel.R, pixel.G, pixel.B) }; // Set color

				RedSlider.Value = pixel.R; // Set value
				GreenSlider.Value = pixel.G; // Set value
				BlueSlider.Value = pixel.B; // Set value

				// MiniPicker
				miniPicker.Left = Env.GetMouseCursorPositionWPF().X; // Define position
				miniPicker.Top = Env.GetMouseCursorPositionWPF().Y; // Define position
			};
		}

		private void InitUI()
		{
			// Generate random color
			Random random = new();
			int r = random.Next(0, 255); int g = random.Next(0, 255); int b = random.Next(0, 255); // Generate random values
			
			// Display
			ColorDisplayer.Background = new SolidColorBrush { Color = Color.FromRgb((byte)r, (byte)g, (byte)b) }; // Display color
			RedSlider.Value = r; // Set value
			GreenSlider.Value = g; // Set value
			BlueSlider.Value = b; // Set value

			// Convert to HEX
			var hex = ColorsConverter.RGBtoHEX(r, g, b); // Convert
			HEXTxt.Text = $"{Properties.Resources.HEXP} #{hex.Value}";

			m_GlobalHook.KeyPress += (o, e) =>
			{
				if (e.KeyChar.ToString().ToLower() == "c")
				{
					if (isRunning)
					{
						Clipboard.SetText($"{RedSlider.Value};{GreenSlider.Value};{BlueSlider.Value}"); // Copy
					}
				}
				else if (e.KeyChar.ToString().ToLower() == "s")
				{
					if (isRunning)
					{
						dispatcherTimer.Stop();
						miniPicker.Hide();
						isRunning = false;
						SelectColorBtn.Content = Properties.Resources.SelectColor; // Set text
					}
					else
					{
						dispatcherTimer.Start();
						miniPicker.Show();
						isRunning = true;
						SelectColorBtn.Content = Properties.Resources.Stop; // Set text
					}
				}
			};
		}

		private void RedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ColorDisplayer.Background = new SolidColorBrush { Color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value) };
			RedValueTxt.Text = RedSlider.Value.ToString(); // Set text

			var h = ColorsConverter.RGBtoHEX((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value); // Convert
			HEXTxt.Text = $"{Properties.Resources.HEXP} #{h.Value}";
		}

		private void GreenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ColorDisplayer.Background = new SolidColorBrush { Color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value) };
			GreenValueTxt.Text = GreenSlider.Value.ToString(); // Set text

			var h = ColorsConverter.RGBtoHEX((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value); // Convert
			HEXTxt.Text = $"{Properties.Resources.HEXP} #{h.Value}";
		}

		private void BlueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ColorDisplayer.Background = new SolidColorBrush { Color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value) };
			BlueValueTxt.Text = BlueSlider.Value.ToString(); // Set text

			var h = ColorsConverter.RGBtoHEX((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value); // Convert
			HEXTxt.Text = $"{Properties.Resources.HEXP} #{h.Value}";
		}

		MiniPicker miniPicker = new(); // MiniPicker window
		private void SelectColorBtn_Click(object sender, RoutedEventArgs e)
		{
			if (!isRunning)
			{
				dispatcherTimer.Start(); // Start
				SelectColorBtn.Content = Properties.Resources.Stop; // Set text
				isRunning = true;

				miniPicker.Left = Env.GetMouseCursorPositionWPF().X; // Define position
				miniPicker.Top = Env.GetMouseCursorPositionWPF().Y; // Define position
				miniPicker.Show(); // Show
			}
			else
			{
				dispatcherTimer.Stop(); // Stop
				SelectColorBtn.Content = Properties.Resources.SelectColor; // Set text
				isRunning = false;
				miniPicker.Hide(); // Hide
			}
		}

		private void CopyBtn_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText($"{RedSlider.Value};{GreenSlider.Value};{BlueSlider.Value}"); // Copy
		}

		private void CopyHEXBtn_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText("#" + ColorsConverter.RGBtoHEX((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value).Value); // Copy
		}
	}
}