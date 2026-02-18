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
using ColorPicker.UserControls;
using Microsoft.Win32;
using Synethia;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for ImageExtractionPage.xaml
/// </summary>
public partial class ImageExtractorPage : Page
{
	bool code = !Global.Settings.UseSynethia; // checks if the code as already been implemented
	readonly List<string> filePaths = [];
	private Dictionary<RGB, int> Colors = [];
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

		ImageScrollViewer.Visibility = ImageDisplayer.Children.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
		ColorPlaceholder.Visibility = ImageDisplayer.Children.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
		if (ImageDisplayer.Children.Count == 0) ColorDisplayerBorder.Visibility = Visibility.Collapsed;
		DragZone.Visibility = ImageDisplayer.Children.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
	}

	private void BrowseBtn_Click(object sender, RoutedEventArgs e)
	{
		OpenFileDialog openFileDialog = new()
		{
			Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.ico|All Files|*.*",
			Multiselect = true,
		};
		if (openFileDialog.ShowDialog() == true)
		{
			filePaths.AddRange(openFileDialog.FileNames);
			LoadImageUI();
		}
	}
	bool ascending = false;

	private async void ExtractBtn_Click(object sender, RoutedEventArgs e)
	{
		if (filePaths.Count == 0) return;

		bool precisionValid = int.TryParse(PrecisionTxt.Text, out var precision);

		var colors = await GetImageColorFrequenciesAsync(filePaths, precisionValid ? precision : 10, ascending);
		Colors = colors;
		LoadColorDisplayer(colors);
	}

	private void LoadColorDisplayer(Dictionary<RGB, int> colors)
	{
		ColorDisplayer.Children.Clear();

		int c = 0;
		bool hasMaxSpecified = int.TryParse(ColorNumberTxt.Text, out int max);
		foreach (var color in colors)
		{
			if (c > (hasMaxSpecified ? max - 1 : 20)) break;
			c++;
			ColorDisplayer.Children.Add(new ColorFrequenceItem(new ColorHelper.RGB(color.Key.R, color.Key.G, color.Key.B), color.Value));
		}

		if (ColorDisplayer.Children.Count == 0)
		{
			ColorDisplayerBorder.Visibility = Visibility.Collapsed;
			ColorPlaceholder.Visibility = Visibility.Visible;
		}
		else
		{
			ColorDisplayerBorder.Visibility = Visibility.Visible;
			ColorPlaceholder.Visibility = Visibility.Collapsed;
		}
	}

	static async Task<Dictionary<RGB, int>> GetImageColorFrequenciesAsync(List<string> imagePaths, int step, bool ascending)
	{
		return await Task.Run(() =>
		{
			Dictionary<RGB, int> colorFrequencies = [];
			// Clamp step to minimum of 1 to avoid division by zero
			int effectiveStep = Math.Max(1, step);

			for (int i = 0; i < imagePaths.Count; i++)
			{
				using Bitmap image = new(imagePaths[i]);
				int width = image.Width;
				int height = image.Height;

				BitmapData bitmapData = image.LockBits(
					new Rectangle(0, 0, width, height),
					ImageLockMode.ReadOnly,
					PixelFormat.Format32bppArgb);

				try
				{
					int bytesPerPixel = 4; // Format32bppArgb
					int stride = bitmapData.Stride;
					nint scan0 = bitmapData.Scan0;
					int byteCount = stride * height;
					byte[] pixels = new byte[byteCount];
					Marshal.Copy(scan0, pixels, 0, byteCount);

					for (int y = 0; y < height; y += effectiveStep)
					{
						for (int x = 0; x < width; x += effectiveStep)
						{
							int index = (y * stride) + (x * bytesPerPixel);
							byte b = pixels[index];
							byte g = pixels[index + 1];
							byte r = pixels[index + 2];

							RGB rgbColor = new(r, g, b);
							if (colorFrequencies.TryGetValue(rgbColor, out int value))
								colorFrequencies[rgbColor] = value + 1;
							else
								colorFrequencies.Add(rgbColor, 1);
						}
					}
				}
				finally
				{
					image.UnlockBits(bitmapData);
				}
			}

			return ascending
				? colorFrequencies.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value)
				: colorFrequencies.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
		});
	}

	class RGB(byte r, byte g, byte b)
	{
		public byte R { get; } = r;
		public byte G { get; } = g;
		public byte B { get; } = b;

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

	private async void ExportCSVBtn_Click(object sender, RoutedEventArgs e)
	{
		SaveFileDialog saveFileDialog = new()
		{
			Filter = "CSV Files|*.csv|All Files|*.*"
		};
		if (saveFileDialog.ShowDialog() == true)
		{
			await ExportToCSVAsync(Colors, saveFileDialog.FileName, (CommaRadioBtn.IsChecked ?? true) ? "," : ";", IncludeFrequenceChk.IsChecked ?? false);
		}
	}

	private static async Task ExportToCSVAsync(Dictionary<RGB, int> colors, string fileName, string separator, bool includeFreq)
	{
		try
		{
			using StreamWriter writer = new(fileName, false, Encoding.UTF8);
			foreach (var color in colors)
			{
				var hex = ColorHelper.ColorConverter.RgbToHex(new(color.Key.R, color.Key.G, color.Key.B));
				string line = includeFreq
					? $"#{hex}{separator}{color.Value}{separator}"
					: $"#{hex}{separator}";
				await writer.WriteLineAsync(line);
			}
		}
		catch { }
	}

	private void SortBtn_Click(object sender, RoutedEventArgs e)
	{
		ascending = !ascending;
		SortBtn.Content = ascending ? "\uF149" : "\uF19C";

		Colors = ascending ? Colors.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value) : Colors.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
		LoadColorDisplayer(Colors);
	}

	private void DragZone_Drop(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

			foreach (string file in files)
			{
				string extension = Path.GetExtension(file).ToLower();

				if (extension is ".jpg" or ".png" or ".jpeg" or ".bmp" or ".gif" or ".ico")
				{
					filePaths.Add(file);
				}
			}
			LoadImageUI();
		}
	}

	private void DragZone_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		BrowseBtn_Click(sender, e);
	}
}