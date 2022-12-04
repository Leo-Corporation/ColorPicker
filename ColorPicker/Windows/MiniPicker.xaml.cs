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
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ColorPicker.Windows;

/// <summary>
/// Interaction logic for MiniPicker.xaml
/// </summary>
public partial class MiniPicker : Window
{
	internal DispatcherTimer timer = new() { Interval = new(0, 0, 0, 0, 1) };
	readonly bool u = Global.Settings.HEXUseUpperCase.Value;
	public MiniPicker()
	{
		InitializeComponent();
		timer.Tick += (o, e) =>
		{
			var pos = System.Windows.Forms.Cursor.Position;
			Bitmap bitmap = new(1, 1);
			Graphics GFX = Graphics.FromImage(bitmap);
			GFX.CopyFromScreen(pos, new System.Drawing.Point(0, 0), bitmap.Size);
			var pixel = bitmap.GetPixel(0, 0);

			ColorDisplayer.Background = new SolidColorBrush { Color = System.Windows.Media.Color.FromRgb(pixel.R, pixel.G, pixel.B) }; // Set color

			// Convert to HEX
			var hexColor = ColorHelper.ColorConverter.RgbToHex(new(pixel.R, pixel.R, pixel.B)); // Convert

			// Display
			RedTxt.Text = $"{Properties.Resources.RedP} {pixel.R}"; // Set text
			GreenTxt.Text = $"{Properties.Resources.GreenP} {pixel.G}"; // Set text
			BlueTxt.Text = $"{Properties.Resources.BlueP} {pixel.B}"; // Set text
			HEXTxt.Text = $"{Properties.Resources.HEXP} #{(u ? hexColor.Value.ToUpper() : hexColor.Value)}"; // Set text
			CoordsTxt.Text = $"x: {pos.X}, y: {pos.Y}";
		};
	}
}
