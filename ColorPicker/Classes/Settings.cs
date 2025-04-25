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
using ColorPicker.Enums;

namespace ColorPicker.Classes;

public class Settings
{
	public Settings()
	{
		Theme = Themes.System;
		Language = Languages.Default;
		DefaultColorType = ColorTypes.RGB;
		DefaultPage = AppPages.Home;
		UseSynethia = true;
		IsFirstRun = true;
		CheckUpdateOnStart = true;
		SelectKeyboardShortcut = "Shift+S";
		CopyKeyboardShortcut = "Shift+C";
		UseKeyboardShortcuts = true;
		TextToolFont = "Arial";
		TextToolFontSize = 12;
		TextToolForeground = "#000000";
		TextToolBackground = "#FFFFFF";
		IsMaximized = false;
		ApiKey = "";
		Model = "gpt-3.5-turbo";
		SupportedModels = ["gpt-3.5-turbo", "gpt-4"];
		RgbSeparator = ";";
		UseUpperCasesHex = false;
		LaunchOnStart = false;
	}

	public Themes Theme { get; set; }
	public Languages Language { get; set; }
	public ColorTypes DefaultColorType { get; set; }
	public AppPages DefaultPage { get; set; }
	public bool UseSynethia { get; set; }
	public bool IsFirstRun { get; set; }
	public bool CheckUpdateOnStart { get; set; }
	public string SelectKeyboardShortcut { get; set; }
	public string CopyKeyboardShortcut { get; set; }
	public bool UseKeyboardShortcuts { get; set; }
	public string TextToolFont { get; set; }
	public int TextToolFontSize { get; set; }
	public string TextToolForeground { get; set; }
	public string TextToolBackground { get; set; }
	public bool IsMaximized { get; set; }
	public string? ApiKey { get; set; }
	public string? Model { get; set; }
	public string[]? SupportedModels { get; set; }
	public string? RgbSeparator { get; set; }
	public bool? UseUpperCasesHex { get; set; }
	public bool? LaunchOnStart { get; set; }
}
