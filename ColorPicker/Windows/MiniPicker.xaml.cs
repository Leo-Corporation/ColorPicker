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
	/// Interaction logic for MiniPicker.xaml
	/// </summary>
	public partial class MiniPicker : Window
	{
		DispatcherTimer timer = new() { Interval = new(0, 0, 0, 0, 1) };
		bool u = Global.Settings.HEXUseUpperCase.Value;
		public MiniPicker()
		{
			InitializeComponent();
			timer.Tick += (o, e) =>
			{
				Bitmap bitmap = new(1, 1);
				Graphics GFX = Graphics.FromImage(bitmap);
				GFX.CopyFromScreen(Env.GetMouseCursorPosition(), new System.Drawing.Point(0, 0), bitmap.Size);
				var pixel = bitmap.GetPixel(0, 0);

				ColorDisplayer.Background = new SolidColorBrush { Color = System.Windows.Media.Color.FromRgb(pixel.R, pixel.G, pixel.B) }; // Set color

				// Convert to HEX
				var hexColor = ColorsConverter.RGBtoHEX(pixel); // Convert

				// Display
				RedTxt.Text = $"{Properties.Resources.RedP} {pixel.R}"; // Set text
				GreenTxt.Text = $"{Properties.Resources.GreenP} {pixel.G}"; // Set text
				BlueTxt.Text = $"{Properties.Resources.BlueP} {pixel.B}"; // Set text
				HEXTxt.Text = $"{Properties.Resources.HEXP} #{(u ? hexColor.Value.ToUpper() : hexColor.Value)}"; // Set text
			};

			timer.Start();
		}
	}
}
