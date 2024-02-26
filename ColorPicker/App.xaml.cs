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
using ColorPicker.Windows;
using PeyrSharp.Env;
using System.Windows;

namespace ColorPicker;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	private void Application_Startup(object sender, StartupEventArgs e)
	{
		Global.ChangeTheme();
		Global.ChangeLanguage();
		// Bookmarks system
		Global.Bookmarks = XmlSerializerManager.LoadFromXml<Bookmarks>($@"{FileSys.AppDataPath}\Léo Corporation\ColorPicker Max\Bookmarks.xml") ?? new()
		{
			ColorBookmarks = [],
			PaletteBookmarks = [],
			GradientBookmarks = []
		};

		if (Global.Bookmarks.ColorBookmarks is null) Global.Bookmarks.ColorBookmarks = [];
		if (Global.Bookmarks.PaletteBookmarks is null) Global.Bookmarks.PaletteBookmarks = [];
		if (Global.Bookmarks.GradientBookmarks is null) Global.Bookmarks.GradientBookmarks = [];
		if (Global.Bookmarks.TextBookmarks is null) Global.Bookmarks.TextBookmarks = [];

		// Pages
		Global.SelectorPage = new();
		Global.ChromaticWheelPage = new();
		Global.ConverterPage = new();
		Global.TextPage = new();
		Global.ContrastPage = new();
		Global.ImageExtractorPage = new();
		Global.PalettePage = new();
		Global.GradientPage = new();
		Global.AiGenPage = new();
		Global.HarmoniesPage = new();
		Global.HomePage = new();
		Global.BookmarksPage = new();
		Global.SettingsPage = new();

		if (!Global.Settings.IsFirstRun)
		{
			new MainWindow().Show();
		}
		else
		{
			new FirstRunWindow().Show();
		}
	}
}
