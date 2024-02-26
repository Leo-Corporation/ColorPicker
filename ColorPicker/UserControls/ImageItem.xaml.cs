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

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ColorPicker.UserControls
{
	/// <summary>
	/// Interaction logic for ImageItem.xaml
	/// </summary>
	public partial class ImageItem : UserControl
	{
		public string Path { get; }
		public List<string> Items { get; }
		public Action Delete { get; }

		public ImageItem(string path, List<string> items, Action delete)
		{
			InitializeComponent();
			Path = path;
			Items = items;
			Delete = delete;
			InitUI();
		}


		private void InitUI()
		{
			try
			{
				var bitmap = new BitmapImage();
				var stream = File.OpenRead(Path);
				bitmap.BeginInit();
				bitmap.CacheOption = BitmapCacheOption.OnLoad;
				bitmap.StreamSource = stream;
				bitmap.DecodePixelWidth = 256;
				bitmap.EndInit();
				stream.Close();
				stream.Dispose();
				bitmap.Freeze();
				Img.ImageSource = bitmap;
			}
			catch
			{

			}
		}

		private void DeleteBtn_Click(object sender, RoutedEventArgs e)
		{
			Items.Remove(Path);
			Delete();
		}
	}
}
