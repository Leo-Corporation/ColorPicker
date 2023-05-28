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
using ColorPicker.Enums;
using ColorPicker.Pages;
using Microsoft.Win32;
using PeyrSharp.Enums;
using PeyrSharp.Env;
using Synethia;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace ColorPicker.Classes;
public static class Global
{
	public static SelectorPage? SelectorPage { get; set; }
	public static ChromaticWheelPage? ChromaticWheelPage { get; set; }
	public static ConverterPage? ConverterPage { get; set; }
	public static TextPage? TextPage { get; set; }
	public static PalettePage? PalettePage { get; set; }
	public static GradientPage? GradientPage { get; set; }
	public static HomePage? HomePage { get; set; }
	public static BookmarksPage? BookmarksPage { get; set; }
	public static SettingsPage? SettingsPage { get; set; }

	public static Bookmarks Bookmarks { get; set; }
	public static Settings Settings { get; set; } = XmlSerializerManager.LoadFromXml<Settings>(SettingsPath) ?? new();

	public static SynethiaConfig SynethiaConfig { get; set; } = SynethiaManager.Load(SynethiaPath, Default);

	public static SynethiaConfig Default => new()
	{
		PagesInfo = new List<PageInfo>()
		{
			new PageInfo("Selector"),
			new PageInfo("ChromaticWheel"),
			new PageInfo("Converter"),
			new PageInfo("TextTool"),
			new PageInfo("Palette"),
			new PageInfo("Gradient")
		},
		ActionsInfo = new List<ActionInfo>()
		{
			new ActionInfo(0, "Selector.SelectBtn"),
			new ActionInfo(1, "Chromatic.Disc"),
			new ActionInfo(2, "Converter.FromRgb"),
			new ActionInfo(3, "TextTool.Contrast"),
			new ActionInfo(4, "Palette.GeneratePalette"),
			new ActionInfo(5, "Gradient.GenerateGradient")
		}
	};

	internal static string SynethiaPath => $@"{FileSys.AppDataPath}\Léo Corporation\ColorPicker Max\SynethiaConfig.json";
	internal static string BookmarksPath => $@"{FileSys.AppDataPath}\Léo Corporation\ColorPicker Max\Bookmarks.xml";
	internal static string SettingsPath => $@"{FileSys.AppDataPath}\Léo Corporation\ColorPicker Max\Settings.xml";
	public static string LastVersionLink => "https://raw.githubusercontent.com/Leo-Corporation/LeoCorp-Docs/master/Liens/Update%20System/ColorPicker/5.0/Version.txt";

	public static string Version => "5.2.0.2305-rc1";

	public static string HiSentence
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

	public static Dictionary<AppPages, string> AppPagesFilledIcons => new()
	{
		{ AppPages.Home, "\uF488" },
		{ AppPages.Bookmarks, "\uF1F6" },
		{ AppPages.Settings, "\uF6B3" },
		{ AppPages.Selector, "\uF9BC" },
		{ AppPages.ColorWheel, "\uF605" },
		{ AppPages.Converter, "\uF18E" },
		{ AppPages.TextTool, "\uF7AB" },
		{ AppPages.ColorPalette, "\uF2F6" },
		{ AppPages.ColorGradient, "\uFD3F" },
	};
	public static Dictionary<AppPages, string> AppPagesName => new()
	{
		{ AppPages.Home, Properties.Resources.Home },
		{ AppPages.Bookmarks, Properties.Resources.History },
		{ AppPages.Settings, Properties.Resources.Settings },
		{ AppPages.Selector, Properties.Resources.Selector },
		{ AppPages.ColorWheel, Properties.Resources.ChromaticWheel },
		{ AppPages.Converter, Properties.Resources.Converter },
		{ AppPages.TextTool, Properties.Resources.TextTool },
		{ AppPages.ColorPalette, Properties.Resources.Palette },
		{ AppPages.ColorGradient, Properties.Resources.Gradient },
	};

	public static string[] ActionsIcons => new string[] { "\uFD48", "\uF2BF", "\uF18B", "\uFD1B", "\uF777", "\uFCBA" };
	public static string[] ActionsString => new string[] { Properties.Resources.SelectColor, Properties.Resources.SelectChomaticDisc, Properties.Resources.ConvertFromRGB, Properties.Resources.GetContrast, Properties.Resources.GeneratePalette, Properties.Resources.GenerateGradient };

	public static (int, int, int) GenerateRandomColor()
	{
		Random random = new();
		int r = random.Next(0, 255); int g = random.Next(0, 255); int b = random.Next(0, 255); // Generate random values
		return (r, g, b);
	}

	public static System.Drawing.Color GenerateRandomColorDrawing()
	{
		Random random = new();
		int r = random.Next(0, 255); int g = random.Next(0, 255); int b = random.Next(0, 255); // Generate random values
		return System.Drawing.Color.FromArgb(r, g, b);
	}
	public static Color GetColorFromResource(string resourceName) => (Color)System.Windows.Media.ColorConverter.ConvertFromString(Application.Current.Resources[resourceName].ToString());

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

	public static (string, int) GetContrast(int[] rgb1, int[] rgb2)
	{
		var lum1 = GetLuminance(rgb1[0], rgb1[1], rgb1[2]);
		var lum2 = GetLuminance(rgb2[0], rgb2[1], rgb2[2]);

		var brightest = Math.Max(lum1, lum2);
		var darkest = Math.Min(lum1, lum2);

		var result = Math.Round((brightest + 0.05) / (darkest + 0.05), 4);

		int gridRow;
		if (result > 7) gridRow = 0;
		else gridRow = 0;
		if (result <= 3) gridRow = 3;
		if (result >= 3 && result <= 4.5) gridRow = 2;
		if (result >= 4.5 && result <= 7) gridRow = 1;
		return (result.ToString(), gridRow);
	}

	public static RGB[] GetShades(HSL hsl)
	{
		RGB[] colorPalette = new RGB[10];
		for (int i = 0; i < colorPalette.Length; i++)
		{
			int saturation = (i + 1) * 10;
			HSL hslColor = new(hsl.H, (byte)saturation, hsl.L);
			colorPalette[i] = ColorHelper.ColorConverter.HslToRgb(hslColor);
		}
		return colorPalette;
	}

	public static RGB[] GetBrightness(HSL hsl)
	{
		RGB[] colorPalette = new RGB[10];
		for (int i = 0; i < colorPalette.Length; i++)
		{
			int luminosity = (i + 1) * 10;
			HSL hslColor = new(hsl.H, hsl.S, (byte)luminosity);
			colorPalette[i] = ColorHelper.ColorConverter.HslToRgb(hslColor);
		}
		return colorPalette;
	}

	public static RGB[] GetHues(HSL hsl)
	{
		RGB[] colorPalette = new RGB[10];
		for (int i = 0; i < colorPalette.Length; i++)
		{
			int hue = i * 27;
			HSL hslColor = new((byte)hue, hsl.S, hsl.L);
			colorPalette[i] = ColorHelper.ColorConverter.HslToRgb(hslColor);
		}
		return colorPalette;
	}

	public static AppPages PageInfoToAppPages(PageInfo pageInfo)
	{
		return pageInfo.Name switch
		{
			"Selector" => AppPages.Selector,
			"ChromaticWheel" => AppPages.ColorWheel,
			"Converter" => AppPages.Converter,
			"TextTool" => AppPages.TextTool,
			"Palette" => AppPages.ColorPalette,
			"Gradient" => AppPages.ColorGradient,
			_ => AppPages.Selector
		};
	}

	/// <summary>
	/// Changes the application's theme.
	/// </summary>
	public static void ChangeTheme()
	{
		App.Current.Resources.MergedDictionaries.Clear();
		ResourceDictionary resourceDictionary = new(); // Create a resource dictionary

		bool isDark = Settings.Theme == Themes.Dark;
		if (Settings.Theme == Themes.System)
		{
			isDark = IsSystemThemeDark(); // Set
		}

		if (isDark) // If the dark theme is on
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

	public static void ChangeLanguage()
	{
		switch (Settings.Language) // For each case
		{
			case Languages.Default: // No language
				break;
			case Languages.en_US: // English (US)
				Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US"); // Change
				break;

			case Languages.fr_FR: // French (FR)
				Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr-FR"); // Change
				break;

			case Languages.zh_CN: // Chinese (CN)
				Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN"); // Change
				break;
			default: // No language
				break;
		}
	}
}
