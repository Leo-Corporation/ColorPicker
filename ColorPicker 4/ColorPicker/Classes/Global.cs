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
using ColorPicker.Pages;
using Microsoft.Win32;
using PeyrSharp.Enums;
using PeyrSharp.Env;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;

namespace ColorPicker.Classes;

/// <summary>
/// The <see cref="Global"/> class contains various methods and properties.
/// </summary>
public static class Global
{
	/// <summary>
	/// The current version of ColorPicker.
	/// </summary>
	public static string Version => "4.6.0.2212";

	/// <summary>
	/// List of the available languages.
	/// </summary>
	public static List<string> LanguageList => new() { "English (United States)", "Français (France)", "Italiano (Italia)", "中文（简体）" };

	/// <summary>
	/// List of the available languages codes.
	/// </summary>
	public static List<string> LanguageCodeList => new() { "en-US", "fr-FR", "it-IT", "zh-CN" };

	/// <summary>
	/// The <see cref="Pages.PickerPage"/>.
	/// </summary>
	public static PickerPage PickerPage { get; set; }

	/// <summary>
	/// The <see cref="Pages.ConverterPage"/>.
	/// </summary>
	public static ConverterPage ConverterPage { get; set; }

	/// <summary>
	/// The <see cref="Pages.PalettePage"/>.
	/// </summary>
	public static PalettePage PalettePage { get; set; }

	/// <summary>
	/// The <see cref="Pages.SettingsPage"/>.
	/// </summary>
	public static SettingsPage SettingsPage { get; set; }

	/// <summary>
	/// Settings of ColorPicker.
	/// </summary>
	public static Settings Settings { get; set; }

	/// <summary>
	/// The content of the history of ColorPicker.
	/// </summary>
	public static ColorContentHistory ColorContentHistory { get; set; }

	/// <summary>
	/// Last version link.
	/// </summary>
	public static string LastVersionLink => "https://raw.githubusercontent.com/Leo-Corporation/LeoCorp-Docs/master/Liens/Update%20System/ColorPicker/Version.txt";

	/// <summary>
	/// Gets the "Hi" sentence message.
	/// </summary>
	public static string GetHiSentence
	{
		get
		{
			if (DateTime.Now.Hour >= 21 && DateTime.Now.Hour <= 7) // If between 9PM & 7AM
			{
				return Properties.Resources.GoodNight + ", " + Environment.UserName + "."; // Return the correct value
			}
			else if (DateTime.Now.Hour >= 7 && DateTime.Now.Hour <= 12) // If between 7AM - 12PM
			{
				return Properties.Resources.Hi + ", " + Environment.UserName + "."; // Return the correct value
			}
			else if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour <= 17) // If between 12PM - 5PM
			{
				return Properties.Resources.GoodAfternoon + ", " + Environment.UserName + "."; // Return the correct value
			}
			else if (DateTime.Now.Hour >= 17 && DateTime.Now.Hour <= 21) // If between 5PM - 9PM
			{
				return Properties.Resources.GoodEvening + ", " + Environment.UserName + "."; // Return the correct value
			}
			else
			{
				return Properties.Resources.Hi + ", " + Environment.UserName + "."; // Return the correct value
			}
		}
	}

	/// <summary>
	/// If the user is trying to new keyboard shortcuts, the value should be false.
	/// </summary>
	public static bool KeyBoardShortcutsAvailable { get; set; }

	/// <summary>
	/// <c>ToString()</c> method for <see cref="ColorTypes"/> enum.
	/// </summary>
	/// <param name="colorTypes">The enum.</param>
	/// <returns>A <see cref="string"/> value.</returns>
	public static string ColorTypesToString(ColorTypes colorTypes)
	{
		return colorTypes switch
		{
			ColorTypes.HEX => Properties.Resources.HEX,
			ColorTypes.RGB => Properties.Resources.RGB,
			ColorTypes.HSV => Properties.Resources.HSV,
			ColorTypes.HSL => Properties.Resources.HSL,
			ColorTypes.CMYK => Properties.Resources.CMYK,
			ColorTypes.XYZ => Properties.Resources.XYZ,
			ColorTypes.YIQ => Properties.Resources.YIQ,
			_ => Properties.Resources.RGB
		}; // Return value
	}

	public static string ColorTypesToCopyString(ColorTypes colorTypes)
	{
		return colorTypes switch
		{
			ColorTypes.HEX => Properties.Resources.CopyHEX,
			ColorTypes.RGB => Properties.Resources.CopyRGB,
			ColorTypes.HSV => Properties.Resources.CopyHSV,
			ColorTypes.HSL => Properties.Resources.CopyHSL,
			ColorTypes.CMYK => Properties.Resources.CopyCMYK,
			ColorTypes.XYZ => Properties.Resources.CopyXYZ,
			ColorTypes.YIQ => Properties.Resources.CopyYIQ,
			_ => Properties.Resources.CopyRGB
		}; // Return value
	}

	/// <summary>
	/// Changes the application's theme.
	/// </summary>
	public static void ChangeTheme()
	{
		App.Current.Resources.MergedDictionaries.Clear();
		ResourceDictionary resourceDictionary = new(); // Create a resource dictionary

		if (!Settings.IsThemeSystem.HasValue)
		{
			Settings.IsThemeSystem = false;
		}

		if (Settings.IsThemeSystem.Value)
		{
			Settings.IsDarkTheme = IsSystemThemeDark(); // Set
		}

		if (Settings.IsDarkTheme) // If the dark theme is on
		{
			resourceDictionary.Source = new Uri("..\\Themes\\Dark.xaml", UriKind.Relative); // Add source
		}
		else
		{
			resourceDictionary.Source = new Uri("..\\Themes\\Light.xaml", UriKind.Relative); // Add source
		}

		App.Current.Resources.MergedDictionaries.Add(resourceDictionary); // Add the dictionary
	}

	public static bool IsSystemThemeDark()
	{
		if (Sys.CurrentWindowsVersion != WindowsVersion.Windows10 && Sys.CurrentWindowsVersion != WindowsVersion.Windows11)
		{
			return false; // Avoid errors on older OSs
		}

		var t = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", "1");
		return t switch
		{
			0 => true,
			1 => false,
			_ => false
		}; // Return
	}

	/// <summary>
	/// Changes the application's language.
	/// </summary>
	public static void ChangeLanguage()
	{
		if (Settings.Language == "_default") return;
		Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Settings.Language); // Change
	}

	internal static string GetHsvString(ColorHelper.HSV hsv) => $"({hsv.H},{hsv.S},{hsv.V})";

	internal static string GetHslString(ColorHelper.HSL hsl) => $"({hsl.H},{hsl.S},{hsl.L})";

	internal static string GetCmykString(ColorHelper.CMYK cmyk) => $"{cmyk.C},{cmyk.M},{cmyk.Y},{cmyk.K}";

	internal static string GetXyzString(ColorHelper.XYZ xyz) => $"{xyz.X};\n{xyz.Y};\n{xyz.Z}";

	internal static string GetYiqString(ColorHelper.YIQ yiq) => $"{yiq.Y};\n{yiq.I};\n{yiq.Q}";

	internal static void CreateJumpLists()
	{
		JumpList jumpList = new(); // Create a jump list

		jumpList.JumpItems.Add(new JumpTask
		{
			Title = Properties.Resources.Picker,
			Arguments = "/page 0",
			Description = Properties.Resources.WelcomePicker,
			CustomCategory = Properties.Resources.Tasks,
			IconResourcePath = Assembly.GetEntryAssembly().Location
		}); // Add Picker jump task

		jumpList.JumpItems.Add(new JumpTask
		{
			Title = Properties.Resources.Converter,
			Arguments = "/page 1",
			Description = Properties.Resources.WelcomeConverter,
			CustomCategory = Properties.Resources.Tasks,
			IconResourcePath = Assembly.GetEntryAssembly().Location
		}); // Add Converter jump task

		jumpList.JumpItems.Add(new JumpTask
		{
			Title = Properties.Resources.Palette,
			Arguments = "/page 2",
			Description = Properties.Resources.WelcomePalette,
			CustomCategory = Properties.Resources.Tasks,
			IconResourcePath = Assembly.GetEntryAssembly().Location
		}); // Add Picker jump task

		jumpList.ShowRecentCategory = false; // Hide the recent category
		jumpList.ShowFrequentCategory = false; // Hide the frequent category


		JumpList.SetJumpList(Application.Current, jumpList); // Set the jump list
	}

	public static double GetLuminance(int r, int g, int b)
	{
		int[] rgb = { r, g, b };
		double[] res = new double[3];

		int i = 0;
		foreach (int val in rgb)
		{
			double v = val / 255d;
			res[i] = v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
			i++;
		}

		return res[0] * 0.2126 + res[1] * 0.7152 + res[2] * 0.0722;
	}


	public static (string, SolidColorBrush) GetContrast(int[] rgb1, int[] rgb2)
	{
		var lum1 = GetLuminance(rgb1[0], rgb1[1], rgb1[2]);
		var lum2 = GetLuminance(rgb2[0], rgb2[1], rgb2[2]);

		var brightest = Math.Max(lum1, lum2);
		var darkest = Math.Min(lum1, lum2);

		var result = Math.Round((brightest + 0.05) / (darkest + 0.05), 4);

		SolidColorBrush backgroundBrush;
		if (result > 7) backgroundBrush = new() { Color = Color.FromRgb(0, 171, 78) };
		else backgroundBrush = new() { Color = Color.FromRgb(0, 171, 78) };
		if (result <= 3) backgroundBrush = new() { Color = Color.FromRgb(255, 0, 0) };
		if (result >= 3 && result <= 4.5) backgroundBrush = new() { Color = Color.FromRgb(255, 122, 41) };
		if (result >= 4.5 && result <= 7) backgroundBrush = new() { Color = Color.FromRgb(0, 127, 255) };
		return (result.ToString(), backgroundBrush);
	}
}
