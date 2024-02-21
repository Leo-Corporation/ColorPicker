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
using ColorPicker.UserControls;
using Microsoft.Win32;
using Synethia;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.LinkLabel;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for ImageExtractionPage.xaml
/// </summary>
public partial class ImageExtractorPage : Page
{
	bool code = Global.Settings.UseSynethia ? false : true; // checks if the code as already been implemented
	readonly List<string> filePaths = new();
	private Dictionary<RGB, int> Colors = new();
	public ImageExtractorPage()
	{
		InitializeComponent();
		InitUI();
		Loaded += (o, e) => SynethiaManager.InjectSynethiaCode(this, Global.SynethiaConfig.PagesInfo, 8, ref code); // injects the code in the page
	}

	private void InitUI()
	{
		TitleTxt.Text = $"{Properties.Resources.ColorTools} > {Properties.Resources.ImageExtractor}";
	}

	private void LoadImageUI()
	{
		ImageDisplayer.Children.Clear();
		for (int i = 0; i < filePaths.Count; i++)
		{
			ImageDisplayer.Children.Add(new ImageItem(filePaths[i], filePaths, LoadImageUI));
		}
		ClearBtn.Visibility = filePaths.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
	}

	private void BrowseBtn_Click(object sender, RoutedEventArgs e)
	{
		OpenFileDialog openFileDialog = new()
		{
			Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.ico|All Files|*.*"
		};
		if (openFileDialog.ShowDialog() == true)
		{
			filePaths.AddRange(openFileDialog.FileNames);
			LoadImageUI();
		}
	}

	private async void ExtractBtn_Click(object sender, RoutedEventArgs e)
	{
		if (filePaths.Count == 0) return;
		ColorDisplayer.Children.Clear();

		bool precisionValid = int.TryParse(PrecisionTxt.Text, out var precision);

		var colors = await GetImageColorFrequenciesAsync(filePaths[0], precisionValid ? precision : 10);
		Colors = colors;

		int c = 0;
		foreach (var color in colors)
		{
			if (c > 20) break;
			c++;
			ColorDisplayer.Children.Add(new ColorFrequenceItem(new ColorHelper.RGB(color.Key.R, color.Key.G, color.Key.B), color.Value));
		}
	}

	static async Task<Dictionary<RGB, int>> GetImageColorFrequenciesAsync(string imagePath, int step)
	{
		return await Task.Run(() =>
		{
			using Bitmap image = new(imagePath);
			int width = image.Width;
			int height = image.Height;

			Dictionary<RGB, int> colorFrequencies = new();

			for (int x = 0; x < width; x++)
			{
				if (x % step != 0) continue;
				for (int y = 0; y < height; y++)
				{
					if (y % step != 0) continue;
					System.Drawing.Color pixelColor = image.GetPixel(x, y);
					RGB rgbColor = new(pixelColor.R, pixelColor.G, pixelColor.B);

					if (colorFrequencies.ContainsKey(rgbColor))
						colorFrequencies[rgbColor]++;
					else
						colorFrequencies.Add(rgbColor, 1);
				}
			}

			return colorFrequencies.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
		});
	}

	class RGB
	{
		public byte R { get; }
		public byte G { get; }
		public byte B { get; }

		public RGB(byte r, byte g, byte b)
		{
			R = r;
			G = g;
			B = b;
		}

		public override int GetHashCode()
		{
			return (R << 16) | (G << 8) | B;
		}

		public override bool Equals(object? obj)
		{
			return obj is RGB rgb && rgb.R == R && rgb.G == G && rgb.B == B;
		}
	}

	private void OptionsBtn_Click(object sender, RoutedEventArgs e)
	{
		OptionsPopup.IsOpen = true;
	}

	private void ClearBtn_Click(object sender, RoutedEventArgs e)
	{
		filePaths.Clear();
		LoadImageUI();
		ColorDisplayer.Children.Clear();
	}

	private void ExportBtn_Click(object sender, RoutedEventArgs e)
	{
		ExportCSVPopup.IsOpen = true;
	}

	private void ExportCSVBtn_Click(object sender, RoutedEventArgs e)
	{
		SaveFileDialog saveFileDialog = new()
		{
			Filter = "CSV Files|*.csv|All Files|*.*"
		};
		if (saveFileDialog.ShowDialog() == true)
		{
			ExportToCSV(Colors, saveFileDialog.FileName, (CommaRadioBtn.IsChecked ?? true) ? "," : ";", IncludeFrequenceChk.IsChecked ?? false);
		}
	}

	private void ExportToCSV(Dictionary<RGB, int> colors, string fileName, string separator, bool includeFreq)
	{
		string text = "";
		foreach (var color in colors)
		{
			var hex = ColorHelper.ColorConverter.RgbToHex(new(color.Key.R, color.Key.G, color.Key.B));
			text += $"#{hex}{separator}{(includeFreq ? color.Value.ToString() + separator : "")}\n";
		}
		try
		{
			using StreamWriter writer = new(fileName, false, Encoding.UTF8);
			writer.WriteLine(text);
		}
		catch { }
	}
}