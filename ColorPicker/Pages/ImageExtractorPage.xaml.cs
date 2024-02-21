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
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for ImageExtractionPage.xaml
/// </summary>
public partial class ImageExtractorPage : Page
{
	bool code = Global.Settings.UseSynethia ? false : true; // checks if the code as already been implemented
	List<string> filePaths = new();
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
			ImageDisplayer.Children.Add(new ImageItem(filePaths[i]));
		}
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

		Thread colorExtractionThread = new(() =>
		{
			var colors = GetImageColorFrequencies(filePaths[0]);

			// Update the UI on the main thread
			Application.Current.Dispatcher.Invoke(() =>
			{
				foreach (var color in colors)
				{
					ColorDisplayer.Children.Add(new ColorFrequenceItem(new ColorHelper.RGB(color.Key.R, color.Key.G, color.Key.B), color.Value));
				}
			});
		});

		colorExtractionThread.Start();
	}

	static Dictionary<RGB, int> GetImageColorFrequencies(string imagePath)
	{
		using (Bitmap image = new(imagePath))
		{
			int width = image.Width;
			int height = image.Height;

			Dictionary<RGB, int> colorFrequencies = new();

			for (int x = 0; x < width; x++)
			{
				if (x % 10 != 0) continue;
				for (int y = 0; y < height; y++)
				{
					if (y % 10 != 0) continue;
					System.Drawing.Color pixelColor = image.GetPixel(x, y);
					RGB rgbColor = new(pixelColor.R, pixelColor.G, pixelColor.B);

					if (colorFrequencies.ContainsKey(rgbColor))
						colorFrequencies[rgbColor]++;
					else
						colorFrequencies.Add(rgbColor, 1);
				}
			}

			return colorFrequencies;
		}
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
}