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
using ColorHelper;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorPicker.UserControls
{
	/// <summary>
	/// Interaction logic for PaletteHistoryItem.xaml
	/// </summary>
	public partial class PaletteHistoryItem : UserControl
	{
		internal RGB[] Colors { get; init; }
		public PaletteHistoryItem(RGB[] colors)
		{
			InitializeComponent();
			Colors = colors;

			InitUI(); // Load the UI
		}

		private void InitUI()
		{
			ColorB1.Background = new SolidColorBrush { Color = Color.FromRgb(Colors[0].R, Colors[0].G, Colors[0].B) }; // Set the background color
			ColorB2.Background = new SolidColorBrush { Color = Color.FromRgb(Colors[1].R, Colors[1].G, Colors[1].B) }; // Set the background color
			ColorB3.Background = new SolidColorBrush { Color = Color.FromRgb(Colors[2].R, Colors[2].G, Colors[2].B) }; // Set the background color
			ColorB4.Background = new SolidColorBrush { Color = Color.FromRgb(Colors[3].R, Colors[3].G, Colors[3].B) }; // Set the background color
			ColorB5.Background = new SolidColorBrush { Color = Color.FromRgb(Colors[4].R, Colors[4].G, Colors[4].B) }; // Set the background color
			ColorB6.Background = new SolidColorBrush { Color = Color.FromRgb(Colors[5].R, Colors[5].G, Colors[5].B) }; // Set the background color
			ColorB7.Background = new SolidColorBrush { Color = Color.FromRgb(Colors[6].R, Colors[6].G, Colors[6].B) }; // Set the background color
			ColorB8.Background = new SolidColorBrush { Color = Color.FromRgb(Colors[7].R, Colors[7].G, Colors[7].B) }; // Set the background color
		}

		private void ColorB1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Border border = (Border)sender; // Convert to border
			var color = ((SolidColorBrush)border.Background).Color; // Get background color
			Clipboard.SetText($"{color.R}{Global.Settings.RGBSeparator}{color.G}{Global.Settings.RGBSeparator}{color.B}"); // Copy
		}
	}
}
