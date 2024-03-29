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
using ColorPicker.Windows;
using System.Windows;

namespace ColorPicker;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	protected override void OnStartup(StartupEventArgs e)
	{
		SettingsManager.Load(); // Load settings
		HistoryManager.Load(); // Load the color history

		Global.ChangeTheme(); // Change the theme
		Global.ChangeLanguage(); // Change the language

		Global.SettingsPage = new(); // Create a new SettingsPage
		Global.PickerPage = new(); // Create a new PickerPage
		Global.ConverterPage = new(); // Create a new ConverterPage
		Global.PalettePage = new(); // Create a new ConverterPage

		if (Global.Settings.IsFirstRun.Value)
		{
			new FirstRunWindow().Show(); // Show the "First run" experience
		}
		else
		{
			int? pageID = (e.Args.Length >= 2 && e.Args[0] == "/page") ? int.Parse(e.Args[1]) : null;

			new MainWindow(pageID == null ? Global.Settings.StartupPage : (Enums.Pages)pageID).Show(); // Launch ColorPicker
			Global.CreateJumpLists(); // Create the jump lists
		}
	}
}
