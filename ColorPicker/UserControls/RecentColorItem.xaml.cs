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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorPicker.UserControls
{
	/// <summary>
	/// Interaction logic for RecentColorItem.xaml
	/// </summary>
	public partial class RecentColorItem : UserControl
	{
		int R { get; init; }
		int G { get; init; }
		int B { get; init; }
		public RecentColorItem(int r, int g, int b)
		{
			InitializeComponent();
			R = r; G = g; B = b; // Set
			InitUI(); // Load the UI
		}

		string s = Global.Settings.RGBSeparator; // Set
		private void InitUI()
		{
			Border.Background = new SolidColorBrush
			{
				Color = Color.FromRgb((byte)R, (byte)G, (byte)B)
			}; // Set background
			ToolTip.Content = $"{Properties.Resources.RGB}: {R}{s}{G}{s}{B}\n" +
				$"{Properties.Resources.HEX}: #{ColorHelper.ColorConverter.RgbToHex(new((byte)R, (byte)G, (byte)B))}"; // Set text
		}

		private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Clipboard.SetText($"{R}{s}{G}{s}{B}"); // Set text
		}
	}
}
