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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorPicker.UserControls
{
	/// <summary>
	/// Interaction logic for PaletteHistoryItem.xaml
	/// </summary>
	public partial class PaletteHistoryItem : UserControl
	{
		internal RGB[] Colors { get; init; }
		private StackPanel ParentStackPanel { get; init; }
		public PaletteHistoryItem(RGB[] colors, StackPanel parent)
		{
			InitializeComponent();
			Colors = colors;
			ParentStackPanel = parent; // Set

			InitUI(); // Load the UI
		}

		private void InitUI()
		{
			// Borders
			ColorB1.Background = new SolidColorBrush { Color = Color.FromRgb(Colors[0].R, Colors[0].G, Colors[0].B) }; // Set the background color
			ColorB2.Background = new SolidColorBrush { Color = Color.FromRgb(Colors[1].R, Colors[1].G, Colors[1].B) }; // Set the background color
			ColorB3.Background = new SolidColorBrush { Color = Color.FromRgb(Colors[2].R, Colors[2].G, Colors[2].B) }; // Set the background color
			ColorB4.Background = new SolidColorBrush { Color = Color.FromRgb(Colors[3].R, Colors[3].G, Colors[3].B) }; // Set the background color
			ColorB5.Background = new SolidColorBrush { Color = Color.FromRgb(Colors[4].R, Colors[4].G, Colors[4].B) }; // Set the background color
			ColorB6.Background = new SolidColorBrush { Color = Color.FromRgb(Colors[5].R, Colors[5].G, Colors[5].B) }; // Set the background color
			ColorB7.Background = new SolidColorBrush { Color = Color.FromRgb(Colors[6].R, Colors[6].G, Colors[6].B) }; // Set the background color
			ColorB8.Background = new SolidColorBrush { Color = Color.FromRgb(Colors[7].R, Colors[7].G, Colors[7].B) }; // Set the background color

			// Tooltips
			B1ToolTip.Content = $"{Properties.Resources.RGB}: {Colors[0].R}{Global.Settings.RGBSeparator}{Colors[0].G}{Global.Settings.RGBSeparator}{Colors[0].B}"; // Set tooltip text
			B2ToolTip.Content = $"{Properties.Resources.RGB}: {Colors[1].R}{Global.Settings.RGBSeparator}{Colors[1].G}{Global.Settings.RGBSeparator}{Colors[1].B}"; // Set tooltip text
			B3ToolTip.Content = $"{Properties.Resources.RGB}: {Colors[2].R}{Global.Settings.RGBSeparator}{Colors[2].G}{Global.Settings.RGBSeparator}{Colors[2].B}"; // Set tooltip text
			B4ToolTip.Content = $"{Properties.Resources.RGB}: {Colors[3].R}{Global.Settings.RGBSeparator}{Colors[3].G}{Global.Settings.RGBSeparator}{Colors[3].B}"; // Set tooltip text
			B5ToolTip.Content = $"{Properties.Resources.RGB}: {Colors[4].R}{Global.Settings.RGBSeparator}{Colors[4].G}{Global.Settings.RGBSeparator}{Colors[4].B}"; // Set tooltip text
			B6ToolTip.Content = $"{Properties.Resources.RGB}: {Colors[5].R}{Global.Settings.RGBSeparator}{Colors[5].G}{Global.Settings.RGBSeparator}{Colors[5].B}"; // Set tooltip text
			B7ToolTip.Content = $"{Properties.Resources.RGB}: {Colors[6].R}{Global.Settings.RGBSeparator}{Colors[6].G}{Global.Settings.RGBSeparator}{Colors[6].B}"; // Set tooltip text
			B8ToolTip.Content = $"{Properties.Resources.RGB}: {Colors[7].R}{Global.Settings.RGBSeparator}{Colors[7].G}{Global.Settings.RGBSeparator}{Colors[7].B}"; // Set tooltip text
		}

		private void ColorB1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Border border = (Border)sender; // Convert to border
			var color = ((SolidColorBrush)border.Background).Color; // Get background color
			Clipboard.SetText($"{color.R}{Global.Settings.RGBSeparator}{color.G}{Global.Settings.RGBSeparator}{color.B}"); // Copy
		}

		private void DeleteBtn_Click(object sender, RoutedEventArgs e)
		{
			Global.PalettePage.SavedColorPalettes.Remove($"{Colors[7].R};{Colors[7].G};{Colors[7].B}"); // Remove from virtual history
			ParentStackPanel.Children.Remove(this); // Remove color palette
		}
	}
}
