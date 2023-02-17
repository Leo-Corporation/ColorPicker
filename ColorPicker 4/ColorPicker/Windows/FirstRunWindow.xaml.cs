﻿/*
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
using ColorPicker.Pages.FirstRunPages;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ColorPicker.Windows;

/// <summary>
/// Interaction logic for FirstRunWindow.xaml
/// </summary>
public partial class FirstRunWindow : Window
{
	static WelcomePage WelcomePage => new(); // PageID = 0
	static TutorialPage TutorialPage => new(); // PageID = 1
	static ThemePage ThemePage => new(); // PageID = 2
	static LanguagePage LanguagePage => new(); // PageID = 3
	static UpdatePage UpdatePage => new(); // PageID = 4

	int pageID = 0;
	public FirstRunWindow()
	{
		InitializeComponent();
		InitUI(); // Load the UI
	}

	private void InitUI()
	{
		PageViewer.Navigate(WelcomePage); // Show welcome page
	}

	private void CloseBtn_Click(object sender, RoutedEventArgs e)
	{
		Application.Current.Shutdown(); // Exit
	}

	private void NextBtn_Click(object sender, RoutedEventArgs e)
	{
		pageID++; // Increment
		PageViewer.Navigate(pageID switch
		{
			0 => WelcomePage,
			1 => TutorialPage,
			2 => ThemePage,
			3 => LanguagePage,
			4 => UpdatePage,
			_ => WelcomePage // By default go the home page
		}); // Navigate to the next page

		if (pageID == 4)
		{
			NextTxt.Text = Properties.Resources.LetsGo; // Set text
		}

		if (pageID == 5)
		{
			Global.Settings.IsFirstRun = false;
			SettingsManager.Save();

			Process.Start(Directory.GetCurrentDirectory() + @"\ColorPicker.exe"); // Start
			Application.Current.Shutdown(); // Close
		}
	}
}
