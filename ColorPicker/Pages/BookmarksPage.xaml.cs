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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorPicker.Pages
{
	/// <summary>
	/// Interaction logic for BookmarksPage.xaml
	/// </summary>
	public partial class BookmarksPage : Page
	{
		public BookmarksPage()
		{
			InitializeComponent();
			CheckButton(ColorsBtn);
			Loaded += (o, e) => InitUI();
		}

		private void InitUI()
		{
			// Clear all items
			ColorsBookmarks.Children.Clear();
			PalettesBookmarks.Children.Clear();
			GradientsBookmarks.Children.Clear();
			TextBookmarks.Children.Clear();

			// Load the "Colors" bookmarks
			for (int i = 0; i < Global.Bookmarks.ColorBookmarks.Count; i++)
			{
				ColorsBookmarks.Children.Add(new ColorItem(Global.Bookmarks.ColorBookmarks[i]));
			}

			// Load the "Palettes" bookmarks
			for (int i = 0; i < Global.Bookmarks.PaletteBookmarks.Count; i++)
			{
				PalettesBookmarks.Children.Add(new PaletteItem(Global.Bookmarks.PaletteBookmarks[i]));
			}

			// Load the "Gradients" bookmarks
			for (int i = 0; i < Global.Bookmarks.GradientBookmarks.Count; i++)
			{
				GradientsBookmarks.Children.Add(new GradientItem(Global.Bookmarks.GradientBookmarks[i]));
			}

			// Load the "Text" bookmarks
			for (int i = 0; i < Global.Bookmarks.TextBookmarks.Count; i++)
			{
				TextBookmarks.Children.Add(new TextItem(Global.Bookmarks.TextBookmarks[i]));
			}
		}

		private void ColorsBtn_Click(object sender, RoutedEventArgs e)
		{
			UnCheckAllButtons();
			CheckButton(ColorsBtn);
			ColorsBookmarks.Visibility = Visibility.Visible;
		}

		private void PaletteBtn_Click(object sender, RoutedEventArgs e)
		{
			UnCheckAllButtons();
			CheckButton(PaletteBtn);
			PalettesBookmarks.Visibility = Visibility.Visible;
		}

		private void EmptyHistoryBtn_Click(object sender, RoutedEventArgs e)
		{
			// If the user doesn't want to empty the history anymore, stop here.
			if (MessageBox.Show(Properties.Resources.EmptyHistoryMsg, Properties.Resources.EmptyBookmarks, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
				return;

			// Get the current selected history
			if (ColorsBookmarks.Visibility == Visibility.Visible)
			{
				Global.Bookmarks.ColorBookmarks.Clear(); // Empty history
			}
			else if (PalettesBookmarks.Visibility == Visibility.Visible)
			{
				Global.Bookmarks.PaletteBookmarks.Clear(); // Empty history
			}
			else if (TextBookmarks.Visibility == Visibility.Visible)
			{
				Global.Bookmarks.TextBookmarks.Clear(); // Empty history
			}
			else
			{
				Global.Bookmarks.GradientBookmarks.Clear(); // Empty history
			}

			InitUI(); // Refresh the UI
			Global.SelectorPage.LoadDetails();
			Global.GradientPage.LoadGradientUI();
			Global.PalettePage.InitPaletteUI();
			Global.TextPage.InitUI();
		}

		private void GradientsBtn_Click(object sender, RoutedEventArgs e)
		{
			UnCheckAllButtons();
			CheckButton(GradientsBtn);
			GradientsBookmarks.Visibility = Visibility.Visible;
		}

		private void UnCheckAllButtons()
		{
			ColorsBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
			PaletteBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
			GradientsBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
			TextBtn.Background = new SolidColorBrush { Color = Colors.Transparent };

			ColorsBookmarks.Visibility = Visibility.Collapsed;
			PalettesBookmarks.Visibility = Visibility.Collapsed;
			GradientsBookmarks.Visibility = Visibility.Collapsed;
			TextBookmarks.Visibility = Visibility.Collapsed;
		}

		private void CheckButton(Button button) => button.Background = new SolidColorBrush { Color = Global.GetColorFromResource("LightAccentColor") };

		private void TextBtn_Click(object sender, RoutedEventArgs e)
		{
			UnCheckAllButtons();
			CheckButton(TextBtn);
			TextBookmarks.Visibility = Visibility.Visible;
		}
	}
}
