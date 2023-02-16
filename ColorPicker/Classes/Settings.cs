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
using ColorPicker.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorPicker.Classes
{
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
	}
}
