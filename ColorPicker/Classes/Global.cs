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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ColorPicker.Classes
{
	/// <summary>
	/// The <see cref="Global"/> class contains various methods and properties.
	/// </summary>
	public static class Global
	{
		/// <summary>
		/// The current version of ColorPicker.
		/// </summary>
		public static string Version => "3.0.0.2104";

		/// <summary>
		/// List of the available languages.
		/// </summary>
		public static List<string> LanguageList => new() { "English (United States)", "Français (France)" };

		/// <summary>
		/// List of the available languages codes.
		/// </summary>
		public static List<string> LanguageCodeList => new() { "en-US", "fr-FR" };

		/// <summary>
		/// The <see cref="Pages.PickerPage"/>.
		/// </summary>
		public static PickerPage PickerPage { get; set; }

		/// <summary>
		/// The <see cref="Pages.ConverterPage"/>.
		/// </summary>
		public static ConverterPage ConverterPage { get; set; }

		/// <summary>
		/// The <see cref="Pages.SettingsPage"/>.
		/// </summary>
		public static SettingsPage SettingsPage { get; set; }

		/// <summary>
		/// Settings of ColorPicker.
		/// </summary>
		public static Settings Settings { get; set; }

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
				_ => Properties.Resources.RGB
			}; // Return value
		}

		/// <summary>
		/// Changes the application's theme.
		/// </summary>
		public static void ChangeTheme()
		{
			App.Current.Resources.MergedDictionaries.Clear();
			ResourceDictionary resourceDictionary = new(); // Create a resource dictionary

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

		/// <summary>
		/// Changes the application's language.
		/// </summary>
		public static void ChangeLanguage()
		{
			switch (Global.Settings.Language) // For each case
			{
				case "_default": // No language
					break;
				case "en-US": // English (US)
					Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US"); // Change
					break;

				case "fr-FR": // French (FR)
					Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr-FR"); // Change
					break;
				default: // No language
					break;
			}
		}
	}
}
