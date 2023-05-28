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
using ColorPicker.Enums;
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

namespace ColorPicker.UserControls;
/// <summary>
/// Interaction logic for TextItem.xaml
/// </summary>
public partial class TextItem : UserControl
{
	BookmarkText BookmarkText { get; init; }
	public TextItem(BookmarkText bookmarkText)
	{
		InitializeComponent();
		BookmarkText = bookmarkText;
		InitUI();
	}

	private void InitUI()
	{
		RGB f = ColorHelper.ColorConverter.HexToRgb(new(BookmarkText.ForegroundColor));
		RGB b = ColorHelper.ColorConverter.HexToRgb(new(BookmarkText.BackgroundColor));
		SolidColorBrush fore = new() { Color = Color.FromRgb(f.R, f.G, f.B) };
		SolidColorBrush back = new() { Color = Color.FromRgb(b.R, b.G, b.B) };

		TitleTxt.Foreground = fore;
		TextTxt.Foreground = fore;
		BgBorder.Background = back;

		TitleTxt.FontFamily = new(BookmarkText.FontFamily);
		TextTxt.FontFamily = new(BookmarkText.FontFamily);
	}

	private void DeleteBtn_Click(object sender, RoutedEventArgs e)
	{
		Global.Bookmarks.TextBookmarks.Remove(BookmarkText);
		Global.BookmarksPage.TextBookmarks.Children.Remove(this);
		Global.TextPage.InitUI();
	}

	public static event EventHandler<PageEventArgs> GoClick;

	private void GoBtn_Click(object sender, RoutedEventArgs e)
	{
		Global.TextPage.InitFromBookmark(BookmarkText);
		GoClick?.Invoke(this, new(AppPages.TextTool));
	}
}
