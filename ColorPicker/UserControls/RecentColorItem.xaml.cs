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
using LeoCorpLibrary;
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

		int[] CurrentColor;
		public RecentColorItem(int r, int g, int b, bool addToHistory = true)
		{
			InitializeComponent();
			R = r; G = g; B = b; // Set
			CurrentColor = new int[] { R, G, B }; // Set the current color


			if (Global.Settings.RestoreColorHistory.Value && addToHistory)
			{
				Global.ColorContentHistory.PickerColorsRGB.Add(CurrentColor);
			}

			InitUI(); // Load the UI
		}

		readonly string s = Global.Settings.RGBSeparator; // Set
		private void InitUI()
		{
			Border.Background = new SolidColorBrush
			{
				Color = Color.FromRgb((byte)R, (byte)G, (byte)B)
			}; // Set background
			ToolTip.Content = $"{Properties.Resources.RGB}: {R}{s}{G}{s}{B}\n" +
				$"{Properties.Resources.HEX}: #{ColorHelper.ColorConverter.RgbToHex(new((byte)R, (byte)G, (byte)B))}\n" +
				$"{Properties.Resources.HSV}: {Global.GetHsvString(ColorHelper.ColorConverter.RgbToHsv(new((byte)R, (byte)G, (byte)B)))}\n" +
				$"{Properties.Resources.HSL}: {Global.GetHslString(ColorHelper.ColorConverter.RgbToHsl(new((byte)R, (byte)G, (byte)B)))}\n" +
				$"{Properties.Resources.CMYK}: {Global.GetCmykString(ColorHelper.ColorConverter.RgbToCmyk(new((byte)R, (byte)G, (byte)B)))}"; // Set text
		}

		private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Clipboard.SetText(Global.Settings.FavoriteColorType switch
			{
				ColorTypes.RGB => $"{R}{s}{G}{s}{B}",
				ColorTypes.HEX => "#" + (Global.Settings.HEXUseUpperCase.Value ? ColorsConverter.RGBtoHEX(R, G, B).Value.ToUpper() : ColorsConverter.RGBtoHEX(R, G, B).Value.ToLower()),
				ColorTypes.HSV => Global.GetHsvString(ColorHelper.ColorConverter.RgbToHsv(new((byte)R, (byte)G, (byte)B))),
				ColorTypes.HSL => Global.GetHslString(ColorHelper.ColorConverter.RgbToHsl(new((byte)R, (byte)G, (byte)B))),
				ColorTypes.CMYK => Global.GetCmykString(ColorHelper.ColorConverter.RgbToCmyk(new((byte)R, (byte)G, (byte)B))),
				_ => $"{R}{s}{G}{s}{B}"
			}); // Copy
		}

		private void Border_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			try
			{
				foreach (int[] item in Global.ColorContentHistory.PickerColorsRGB)
				{
					if (item[0] == R && item[1] == G && item[2] == B)
					{
						Global.ColorContentHistory.PickerColorsRGB.Remove(item);
						break;
					}
				}

				(Parent as WrapPanel).Children.Remove(this); // Remove from Wrap panel
				HistoryManager.Save(); // Save changes
			}
			catch { }
		}
	}
}
