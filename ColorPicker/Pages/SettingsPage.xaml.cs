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
using Gma.System.MouseKeyHook;
using PeyrSharp.Env;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PeyrSharp.Core;

namespace ColorPicker.Pages;

/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class SettingsPage : Page
{
	bool isAvailable;
	readonly System.Windows.Forms.NotifyIcon notifyIcon = new();
	private IKeyboardMouseEvents GlobalHook;
	public SettingsPage()
	{
		InitializeComponent();
		notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.BaseDirectory + @"\ColorPicker.exe");
		notifyIcon.BalloonTipClicked += async (o, e) =>
		{
			string lastVersion = await Update.GetLastVersionAsync(Global.LastVersionLink); // Get last version
			if (MessageBox.Show(Properties.Resources.InstallConfirmMsg, $"{Properties.Resources.InstallVersion} {lastVersion}", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
			{
				Global.PickerPage.miniPicker.Close();
				Sys.ExecuteAsAdmin(Directory.GetCurrentDirectory() + @"\Xalyus Updater.exe"); // Start the updater
				Application.Current.Shutdown(); // Close
			}
		};
		GlobalHook = Hook.GlobalEvents();

		InitUI(); // Load the UI
	}

	List<string> keys = new();
	private void GlobalHook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
	{
		if (keys.Contains(e.KeyCode.ToString()))
		{
			return; // Stop
		}

		keys.Add(e.KeyCode.ToString()); // Add key to list
		if (selectShortcutInput)
		{
			SelectShortcutTxt.Text += (SelectShortcutTxt.Text.Length == 0) ? e.KeyCode.ToString() : $"+{e.KeyCode}";
		}
		else
		{
			CopyShortcutTxt.Text += (CopyShortcutTxt.Text.Length == 0) ? e.KeyCode.ToString() : $"+{e.KeyCode}";
		}
	}

	private async void InitUI()
	{
		try
		{
			if (Global.Settings.StartupPage is null) Global.Settings.StartupPage = Enums.Pages.Picker; // Set default startup page

			// Load RadioButtons
			DarkRadioBtn.IsChecked = Global.Settings.IsDarkTheme; // Change IsChecked property
			LightRadioBtn.IsChecked = !Global.Settings.IsDarkTheme; // Change IsChecked property
			SystemRadioBtn.IsChecked = Global.Settings.IsThemeSystem; // Change IsChecked property
			PickerPageRadioBtn.IsChecked = Global.Settings.StartupPage == Enums.Pages.Picker; // Change IsChecked property
			ConverterPageRadioBtn.IsChecked = Global.Settings.StartupPage == Enums.Pages.Converter; // Change IsChecked property
			PalettePageRadioBtn.IsChecked = Global.Settings.StartupPage == Enums.Pages.Palette; // Change IsChecked property

			// Borders
			if (DarkRadioBtn.IsChecked.Value)
			{
				CheckedBorder = DarkBorder; // Set
			}
			else if (LightRadioBtn.IsChecked.Value)
			{
				CheckedBorder = LightBorder; // Set
			}
			else if (SystemRadioBtn.IsChecked.Value)
			{
				CheckedBorder = SystemBorder; // Set
			}
			RefreshBorders();

			if (!Global.Settings.HEXUseUpperCase.HasValue)
			{
				Global.Settings.HEXUseUpperCase = false; // Set default value
			}

			if (!Global.Settings.EnableKeyBoardShortcuts.HasValue)
			{
				Global.Settings.EnableKeyBoardShortcuts = true; // Set default value
			}

			if (!Global.Settings.RestoreColorHistory.HasValue)
			{
				Global.Settings.RestoreColorHistory = true; // Set default value
			}

			if (!Global.Settings.RestorePaletteColorHistory.HasValue)
			{
				Global.Settings.RestorePaletteColorHistory = true; // Set default value
			}

			if (!Global.Settings.IsFirstRun.HasValue)
			{
				Global.Settings.IsFirstRun = true; // Set default value
			}

			if (!Global.Settings.FavoriteColorType.HasValue)
			{
				Global.Settings.FavoriteColorType = Enums.ColorTypes.RGB; // Set default value
			}

			if (string.IsNullOrEmpty(Global.Settings.CopyKeyboardShortcut))
			{
				Global.Settings.CopyKeyboardShortcut = "Shift+C"; // Set default value
			}

			if (string.IsNullOrEmpty(Global.Settings.SelectKeyboardShortcut))
			{
				Global.Settings.SelectKeyboardShortcut = "Shift+S"; // Set default value
			}

			if (!Global.Settings.IsPinned.HasValue)
			{
				Global.Settings.IsPinned = false; // Set default value
			}

			if (!Global.Settings.UseCompactMode.HasValue)
			{
				Global.Settings.UseCompactMode = false; // Set default value
			}

			if (string.IsNullOrEmpty(Global.Settings.TextToolFont))
			{
				Global.Settings.TextToolFont = "Arial"; // Set default value
			}

			if (!Global.Settings.TextToolFontSize.HasValue)
			{
				Global.Settings.TextToolFontSize = 16; // Set default value
			}

			if (string.IsNullOrEmpty(Global.Settings.TextToolFontColor))
			{
				Global.Settings.TextToolFontColor = "_default"; // Set default value
			}

			if (string.IsNullOrEmpty(Global.Settings.TextToolBackgroundColor))
			{
				Global.Settings.TextToolBackgroundColor = "_default"; // Set default value
			}

			// Load TextTool section
			System.Drawing.Text.InstalledFontCollection installedFonts = new();
			foreach (System.Drawing.FontFamily fontFamily in installedFonts.Families)
			{
				FontComboBox.Items.Add(fontFamily.Name);
			}
			FontComboBox.Text = FontComboBox.Items.Contains(Global.Settings.TextToolFont) ? Global.Settings.TextToolFont : "Arial"; // Set default value

			FontSizeTxt.Text = Global.Settings.TextToolFontSize.ToString(); // Set default value

			if (Global.Settings.TextToolFontColor != "_default")
			{
				var fore = ColorHelper.ColorConverter.HexToRgb(new(Global.Settings.TextToolFontColor));
				ForegroundBorder.Background = new SolidColorBrush { Color = Color.FromRgb(fore.R, fore.G, fore.B) }; // Set default value 
			}

			if (Global.Settings.TextToolBackgroundColor != "_default")
			{
				var back = ColorHelper.ColorConverter.HexToRgb(new(Global.Settings.TextToolBackgroundColor));
				BackgroundBorder.Background = new SolidColorBrush { Color = Color.FromRgb(back.R, back.G, back.B) }; // Set default value 
			}

			// Load checkboxes
			CheckUpdatesOnStartChk.IsChecked = Global.Settings.CheckUpdatesOnStart; // Set
			NotifyUpdatesChk.IsChecked = Global.Settings.NotifyUpdates; // Set

			HEXUseUpperCaseChk.IsChecked = Global.Settings.HEXUseUpperCase; // Set value
			UseKeyboardShortcutsChk.IsChecked = Global.Settings.EnableKeyBoardShortcuts; // Set value

			RestoreColorHistoryOnStartChk.IsChecked = Global.Settings.RestoreColorHistory; // Set
			RestoreColorPaletteHistoryOnStartChk.IsChecked = Global.Settings.RestorePaletteColorHistory; // Set

			CompactChk.IsChecked = Global.Settings.UseCompactMode; // Set

			// Load LangComboBox
			LangComboBox.Items.Add(Properties.Resources.Default); // Add "default"

			for (int i = 0; i < Global.LanguageList.Count; i++)
			{
				LangComboBox.Items.Add(Global.LanguageList[i]);
			}

			LangComboBox.SelectedIndex = (Global.Settings.Language == "_default") ? 0 : Global.LanguageCodeList.IndexOf(Global.Settings.Language) + 1;

			if (string.IsNullOrEmpty(Global.Settings.RGBSeparator))
			{
				Global.Settings.RGBSeparator = ";"; // Set
			}

			// Load default startup page section
			if (PickerPageRadioBtn.IsChecked.Value)
			{
				PageCheckedBorder = PickerPageBorder; // Set
			}
			else if (ConverterPageRadioBtn.IsChecked.Value)
			{
				PageCheckedBorder = ConverterPageBorder; // Set
			}
			else if (PalettePageRadioBtn.IsChecked.Value)
			{
				PageCheckedBorder = PalettePageBorder; // Set
			}

			RefreshStartupBorders();

			// Load FavoriteColorComboBox
			for (int i = 0; i < Enum.GetValues(typeof(Enums.ColorTypes)).Length; i++)
			{
				FavoriteColorComboBox.Items.Add(Global.ColorTypesToString((Enums.ColorTypes)i));
			}

			FavoriteColorComboBox.SelectedIndex = (int)Global.Settings.FavoriteColorType; // Set selected index

			RGBSeparatorTxt.Text = Global.Settings.RGBSeparator; // Set text

			LangApplyBtn.Visibility = Visibility.Hidden; // Hide
			ThemeApplyBtn.Visibility = Visibility.Hidden; // Hide
			RGBFormatApplyBtn.Visibility = Visibility.Hidden; // Hide

			// Load ShortcutTextBoxes
			CopyShortcutTxt.Text = Global.Settings.CopyKeyboardShortcut; // Set text
			SelectShortcutTxt.Text = Global.Settings.SelectKeyboardShortcut; // Set text

			// Update the UpdateStatusTxt
			if (Global.Settings.CheckUpdatesOnStart)
			{
				if (await Internet.IsAvailableAsync())
				{
					isAvailable = Update.IsAvailable(Global.Version, await Update.GetLastVersionAsync(Global.LastVersionLink));

					UpdateStatusTxt.Text = isAvailable ? Properties.Resources.AvailableUpdates : Properties.Resources.UpToDate; // Set the text
					InstallIconTxt.Text = isAvailable ? "\uF152" : "\uF191"; // Set text 
					InstallMsgTxt.Text = isAvailable ? Properties.Resources.Install : Properties.Resources.CheckUpdate; // Set text 

					if (isAvailable && Global.Settings.NotifyUpdates)
					{
						notifyIcon.Visible = true; // Show
						notifyIcon.ShowBalloonTip(5000, Properties.Resources.ColorPicker, Properties.Resources.AvailableUpdates, System.Windows.Forms.ToolTipIcon.Info);
						notifyIcon.Visible = false; // Hide
					}
				}
				else
				{
					UpdateStatusTxt.Text = Properties.Resources.UnableToCheckUpdates; // Set the text
					InstallIconTxt.Text = "\uF191"; // Set text 
					InstallMsgTxt.Text = Properties.Resources.CheckUpdate; // Set text
				}
			}
			else
			{
				UpdateStatusTxt.Text = Properties.Resources.CheckUpdatesDisabledOnStart; // Set text
				InstallMsgTxt.Text = Properties.Resources.CheckUpdate; // Set text
				InstallIconTxt.Text = "\uF191"; // Set text 
			}

			VersionTxt.Text = Global.Version; // Set text

			SettingsManager.Save(); // Save changes
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, ex.StackTrace, MessageBoxButton.OK, MessageBoxImage.Error); // Show error
		}
	}

	private async void RefreshInstallBtn_Click(object sender, RoutedEventArgs e)
	{
		if (await Internet.IsAvailableAsync()) // If there is Internet
		{
			if (isAvailable) // If there is updates
			{
				string lastVersion = await Update.GetLastVersionAsync(Global.LastVersionLink); // Get last version
				if (MessageBox.Show(Properties.Resources.InstallConfirmMsg, $"{Properties.Resources.InstallVersion} {lastVersion}", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
				{
					Sys.ExecuteAsAdmin(Directory.GetCurrentDirectory() + @"\Xalyus Updater.exe"); // Start the updater
					Application.Current.Shutdown(); // Close
				}
			}
			else
			{
				// Update the UpdateStatusTxt
				isAvailable = Update.IsAvailable(Global.Version, await Update.GetLastVersionAsync(Global.LastVersionLink));

				UpdateStatusTxt.Text = isAvailable ? Properties.Resources.AvailableUpdates : Properties.Resources.UpToDate; // Set the text
				InstallIconTxt.Text = isAvailable ? "\uF152" : "\uF191"; // Set text 
				InstallMsgTxt.Text = isAvailable ? Properties.Resources.Install : Properties.Resources.CheckUpdate; // Set text

				if (isAvailable && Global.Settings.NotifyUpdates)
				{
					notifyIcon.Visible = true; // Show
					notifyIcon.ShowBalloonTip(5000, Properties.Resources.ColorPicker, Properties.Resources.AvailableUpdates, System.Windows.Forms.ToolTipIcon.Info);
					notifyIcon.Visible = false; // Hide
				}
			}
		}
		else
		{
			UpdateStatusTxt.Text = Properties.Resources.UnableToCheckUpdates; // Set the text
			InstallIconTxt.Text = "\uF191"; // Set text 
			InstallMsgTxt.Text = Properties.Resources.CheckUpdate; // Set text
		}
	}

	/// <summary>
	/// Restarts ColorPicker.
	/// </summary>
	private static void DisplayRestartMessage()
	{
		if (MessageBox.Show(Properties.Resources.NeedRestartToApplyChanges, Properties.Resources.ColorPicker, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
		{
			Process.Start(Directory.GetCurrentDirectory() + @"\ColorPicker.exe"); // Start
			Application.Current.Shutdown(); // Close
		}
	}

	private void CheckUpdatesOnStartChk_Checked(object sender, RoutedEventArgs e)
	{
		Global.Settings.CheckUpdatesOnStart = CheckUpdatesOnStartChk.IsChecked.Value; // Set
		SettingsManager.Save(); // Save changes
	}

	private void NotifyUpdatesChk_Checked(object sender, RoutedEventArgs e)
	{
		Global.Settings.NotifyUpdates = NotifyUpdatesChk.IsChecked.Value; // Set
		SettingsManager.Save(); // Save changes
	}

	private void LightRadioBtn_Checked(object sender, RoutedEventArgs e)
	{
		ThemeApplyBtn.Visibility = Visibility.Visible; // Show the ThemeApplyBtn button
	}

	private void DarkRadioBtn_Checked(object sender, RoutedEventArgs e)
	{
		ThemeApplyBtn.Visibility = Visibility.Visible; // Show the ThemeApplyBtn button
	}

	private void ThemeApplyBtn_Click(object sender, RoutedEventArgs e)
	{
		Global.Settings.IsDarkTheme = DarkRadioBtn.IsChecked.Value; // Set the settings
		Global.Settings.IsThemeSystem = SystemRadioBtn.IsChecked; // Set the settings
		SettingsManager.Save(); // Save the changes
		ThemeApplyBtn.Visibility = Visibility.Hidden; // Hide
		DisplayRestartMessage();
	}

	private void LangComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		LangApplyBtn.Visibility = Visibility.Visible; // Show the LangApplyBtn button
	}

	private void LangApplyBtn_Click(object sender, RoutedEventArgs e)
	{
		Global.Settings.Language = LangComboBox.Text switch
		{
			"English (United States)" => Global.LanguageCodeList[0], // Set the settings value
			"Français (France)" => Global.LanguageCodeList[1], // Set the settings value
			"Italiano (Italia)" => Global.LanguageCodeList[2], // Set the settings value
			"中文（简体）" => Global.LanguageCodeList[3], // Set the settings value
			_ => "_default" // Set the settings value
		};
		SettingsManager.Save(); // Save the changes
		LangApplyBtn.Visibility = Visibility.Hidden; // Hide
		DisplayRestartMessage();
	}

	private void RGBFormatApplyBtn_Click(object sender, RoutedEventArgs e)
	{
		if (!string.IsNullOrEmpty(RGBSeparatorTxt.Text) && !string.IsNullOrWhiteSpace(RGBSeparatorTxt.Text))
		{
			Global.Settings.RGBSeparator = RGBSeparatorTxt.Text; // Set
			SettingsManager.Save();
			RGBFormatApplyBtn.Visibility = Visibility.Hidden; // Hide
		}
		else
		{
			MessageBox.Show(Properties.Resources.InvalidValueRGB, Properties.Resources.ColorPicker, MessageBoxButton.OK, MessageBoxImage.Warning); // Show	
		}
	}

	private void RGBSeparatorTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		RGBFormatApplyBtn.Visibility = Visibility.Visible; // Show
	}

	private void HEXUseUpperCaseChk_Checked(object sender, RoutedEventArgs e)
	{
		Global.Settings.HEXUseUpperCase = HEXUseUpperCaseChk.IsChecked; // Set
		SettingsManager.Save(); // Save changes
	}

	private void ImportBtn_Click(object sender, RoutedEventArgs e)
	{
		OpenFileDialog openFileDialog = new()
		{
			Filter = "XML|*.xml",
			Title = Properties.Resources.Import
		}; // Create file dialog

		if (openFileDialog.ShowDialog() ?? true)
		{
			SettingsManager.Import(openFileDialog.FileName); // Import games
		}
	}

	private void ExportBtn_Click(object sender, RoutedEventArgs e)
	{
		SaveFileDialog saveFileDialog = new()
		{
			FileName = "ColorPickerSettings.xml",
			Filter = "XML|*.xml",
			Title = Properties.Resources.Export
		}; // Create file dialog

		if (saveFileDialog.ShowDialog() ?? true)
		{
			SettingsManager.Export(saveFileDialog.FileName); // Export games
		}
	}

	private void BtnEnter(object sender, MouseEventArgs e)
	{
		Button button = (Button)sender; // Create button
		button.Foreground = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["WindowButtonsHoverForeground1"].ToString()) }; // Set the foreground
	}

	private void BtnLeave(object sender, MouseEventArgs e)
	{
		Button button = (Button)sender; // Create button
		button.Foreground = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["Foreground1"].ToString()) }; // Set the foreground 
	}

	private void UseKeyboardShortcutsChk_Checked(object sender, RoutedEventArgs e)
	{
		Global.Settings.EnableKeyBoardShortcuts = UseKeyboardShortcutsChk.IsChecked; // Set
		SettingsManager.Save(); // Save settings
	}

	private void SystemRadioBtn_Checked(object sender, RoutedEventArgs e)
	{
		ThemeApplyBtn.Visibility = Visibility.Visible; // Show
	}

	Border CheckedBorder { get; set; }
	private void LightBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		LightRadioBtn.IsChecked = true; // Set IsChecked
		CheckedBorder = LightBorder; // Set
		RefreshBorders();
	}

	private void Border_MouseEnter(object sender, MouseEventArgs e)
	{
		Border border = (Border)sender;
		border.BorderBrush = new SolidColorBrush() { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["AccentColor"].ToString()) }; // Set color
	}

	private void Border_MouseLeave(object sender, MouseEventArgs e)
	{
		Border border = (Border)sender;
		if (border != CheckedBorder && border != PageCheckedBorder)
		{
			border.BorderBrush = new SolidColorBrush() { Color = Colors.Transparent }; // Set color 
		}
	}

	private void DarkBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		DarkRadioBtn.IsChecked = true; // Set IsChecked
		CheckedBorder = DarkBorder; // Set
		RefreshBorders();
	}

	private void SystemBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		SystemRadioBtn.IsChecked = true; // Set IsChecked
		CheckedBorder = SystemBorder; // Set
		RefreshBorders();
	}

	private void RefreshBorders()
	{
		LightBorder.BorderBrush = new SolidColorBrush() { Color = Colors.Transparent }; // Set color 
		DarkBorder.BorderBrush = new SolidColorBrush() { Color = Colors.Transparent }; // Set color 
		SystemBorder.BorderBrush = new SolidColorBrush() { Color = Colors.Transparent }; // Set color 

		CheckedBorder.BorderBrush = new SolidColorBrush() { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["AccentColor"].ToString()) }; // Set color
	}

	private void ResetSettingsLink_Click(object sender, RoutedEventArgs e)
	{
		if (MessageBox.Show(Properties.Resources.ResetSettingsConfirmMsg, Properties.Resources.Settings, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
		{
			Global.Settings = new()
			{
				CheckUpdatesOnStart = false,
				IsDarkTheme = false,
				Language = "_default",
				NotifyUpdates = true,
				RGBSeparator = ";",
				HEXUseUpperCase = false,
				EnableKeyBoardShortcuts = true,
				IsThemeSystem = true,
				RestoreColorHistory = true,
				RestorePaletteColorHistory = true,
				IsFirstRun = false, // False instead of true because the user just want to reset settings, not go through the "welcome" process again.
				FavoriteColorType = Enums.ColorTypes.RGB,
				CopyKeyboardShortcut = "Shift+C",
				SelectKeyboardShortcut = "Shift+S",
				IsPinned = false,
				StartupPage = Enums.Pages.Picker,
				UseCompactMode = false,
				TextToolFont = "Arial",
				TextToolFontSize = 16,
				TextToolFontColor = "_default", // Default value depends of the theme
				TextToolBackgroundColor = "_default",
			}; // Create default settings

			SettingsManager.Save(); // Save the changes
			InitUI(); // Reload the page

			MessageBox.Show(Properties.Resources.SettingsReset, Properties.Resources.ColorPicker, MessageBoxButton.OK, MessageBoxImage.Information);
			Process.Start(Directory.GetCurrentDirectory() + @"\ColorPicker.exe");
			Application.Current.Shutdown(); // Quit
		}
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		MessageBox.Show($"{Properties.Resources.Licenses}\n\n" +
			"Fluent System Icons - MIT License - © 2020 Microsoft Corporation\n" +
			"ColorHelper - MIT License - © 2020 Artyom Gritsuk\n" +
			"globalmousekeyhook - MIT License - © 2010-2018 George Mamaladze\n" +
			"LeoCorpLibrary - MIT License - © 2020-2022 Léo Corporation\n" +
			"ColorPicker - MIT License - © 2021-2022 Léo Corporation", $"{Properties.Resources.ColorPicker} - {Properties.Resources.Licenses}", MessageBoxButton.OK, MessageBoxImage.Information);
	}

	private void RestoreColorHistoryOnStartChk_Checked(object sender, RoutedEventArgs e)
	{
		Global.Settings.RestoreColorHistory = RestoreColorHistoryOnStartChk.IsChecked; // Set
		SettingsManager.Save(); // Save changes
	}

	private void RestoreColorPaletteHistoryOnStartChk_Checked(object sender, RoutedEventArgs e)
	{
		Global.Settings.RestorePaletteColorHistory = RestoreColorPaletteHistoryOnStartChk.IsChecked; // Set
		SettingsManager.Save(); // Save changes
	}

	private void FavoriteColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		Global.Settings.FavoriteColorType = (Enums.ColorTypes)FavoriteColorComboBox.SelectedIndex; // Set
		SettingsManager.Save(); // Save changes
	}

	bool copyShortcutInput, selectShortcutInput = false;
	private void EditCopyShortcutBtn_Click(object sender, RoutedEventArgs e)
	{
		keys = new(); // Create new list
		copyShortcutInput = !copyShortcutInput; // Toggle
		selectShortcutInput = false; // Set
		PressKeys2Txt.Visibility = copyShortcutInput ? Visibility.Visible : Visibility.Collapsed; // Show/Hide
		EditCopyShortcutBtn.Content = copyShortcutInput ? "\uF295" : "\uF3DE"; // Set text

		if (copyShortcutInput)
		{
			GlobalHook.KeyDown += GlobalHook_KeyDown; // Subscribe
			CopyShortcutTxt.Text = "";
			Global.KeyBoardShortcutsAvailable = false; // Set
		}
		else
		{
			GlobalHook.KeyDown -= GlobalHook_KeyDown; // Unsubscribe
			Global.KeyBoardShortcutsAvailable = true; // Set
			if (CopyShortcutTxt.Text.Length == 0) CopyShortcutTxt.Text = Global.Settings.CopyKeyboardShortcut; // Set default

			Global.Settings.CopyKeyboardShortcut = CopyShortcutTxt.Text; // Set
			SettingsManager.Save(); // Save changes
		}
	}

	private void CreditsBtn_Click(object sender, RoutedEventArgs e)
	{
		MessageBox.Show($"{Properties.Resources.CreditsAndThanks}\n\n" +
			$"@dependabot\n" +
			$"@Leo-Peyronnet\n" +
			$"@OCTIPI1\n" +
			$"@wcxu21",
			Properties.Resources.CreditsAndThanks, MessageBoxButton.OK, MessageBoxImage.Information);
	}

	private void RefreshStartupBorders()
	{
		PickerPageBorder.BorderBrush = new SolidColorBrush() { Color = Colors.Transparent }; // Set color
		ConverterPageBorder.BorderBrush = new SolidColorBrush() { Color = Colors.Transparent }; // Set color
		PalettePageBorder.BorderBrush = new SolidColorBrush() { Color = Colors.Transparent }; // Set color		

		PageCheckedBorder.BorderBrush = new SolidColorBrush() { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["AccentColor"].ToString()) }; // Set color
	}

	Border PageCheckedBorder { get; set; }
	private void PickerPageBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		PageCheckedBorder = PickerPageBorder; // Set
		PickerPageRadioBtn.IsChecked = true;
		RefreshStartupBorders(); // Refresh

		Global.Settings.StartupPage = Enums.Pages.Picker; // Set
		SettingsManager.Save(); // Save changes
	}

	private void ConverterPageBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		PageCheckedBorder = ConverterPageBorder; // Set
		ConverterPageRadioBtn.IsChecked = true;
		RefreshStartupBorders(); // Refresh

		Global.Settings.StartupPage = Enums.Pages.Converter; // Set
		SettingsManager.Save(); // Save changes
	}

	private void PalettePageBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		PageCheckedBorder = PalettePageBorder; // Set
		PalettePageRadioBtn.IsChecked = true;
		RefreshStartupBorders(); // Refresh

		Global.Settings.StartupPage = Enums.Pages.Palette; // Set
		SettingsManager.Save(); // Save changes
	}

	private void CompactChk_Checked(object sender, RoutedEventArgs e)
	{
		Global.Settings.UseCompactMode = CompactChk.IsChecked; // Set
		SettingsManager.Save(); // Save changes
	}

	private void ResetSelectShortcutBtn_Click(object sender, RoutedEventArgs e)
	{
		SelectShortcutTxt.Text = "Shift+S"; // Set default value (Shift+S) to textbox
		Global.Settings.SelectKeyboardShortcut = "Shift+S"; // Set default value
		SettingsManager.Save(); // Save changes
	}

	private void ResetCopyShortcutBtn_Click(object sender, RoutedEventArgs e)
	{
		CopyShortcutTxt.Text = "Shift+C"; // Set default value (Shift+C) to textbox
		Global.Settings.CopyKeyboardShortcut = "Shift+C"; // Set default value
		SettingsManager.Save(); // Save changes
	}

	private void ForegroundBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		System.Windows.Forms.ColorDialog colorDialog = new()
		{
			AllowFullOpen = true,
		}; // Create color picker/dialog

		if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) // If the user selected a color
		{
			var color = new SolidColorBrush { Color = Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B) }; // Set color
			ForegroundBorder.Background = color;

			Global.Settings.TextToolFontColor = "#" + ColorHelper.ColorConverter.RgbToHex(new(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B)).Value; // Set color
			SettingsManager.Save(); // Save changes
		}
	}

	private void BackgroundBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		System.Windows.Forms.ColorDialog colorDialog = new()
		{
			AllowFullOpen = true,
		}; // Create color picker/dialog

		if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) // If the user selected a color
		{
			var color = new SolidColorBrush { Color = Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B) }; // Set color
			BackgroundBorder.Background = color;

			Global.Settings.TextToolBackgroundColor = "#" + ColorHelper.ColorConverter.RgbToHex(new(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B)).Value; // Set color
			SettingsManager.Save(); // Save changes
		}
	}

	private void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		Global.Settings.TextToolFont = FontComboBox.SelectedItem.ToString(); // Set
		SettingsManager.Save(); // Save changes
	}

	private void FontSizeTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		Regex regex = new("[^0-9]+");
		e.Handled = regex.IsMatch(e.Text);
	}

	private void FontSizeTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (!string.IsNullOrEmpty(FontSizeTxt.Text) && !string.IsNullOrWhiteSpace(FontSizeTxt.Text))
		{
			Global.Settings.TextToolFontSize = int.Parse(FontSizeTxt.Text); // Set
			SettingsManager.Save(); // Save changes 
		}
	}

	private void EditSelectShortcutBtn_Click(object sender, RoutedEventArgs e)
	{
		keys = new(); // Create new list
		selectShortcutInput = !selectShortcutInput; // Toggle
		copyShortcutInput = false; // Set
		PressKeys1Txt.Visibility = selectShortcutInput ? Visibility.Visible : Visibility.Collapsed; // Show/Hide
		EditSelectShortcutBtn.Content = selectShortcutInput ? "\uF295" : "\uF3DE"; // Set text

		if (selectShortcutInput)
		{
			GlobalHook.KeyDown += GlobalHook_KeyDown; // Subscribe
			SelectShortcutTxt.Text = "";
			Global.KeyBoardShortcutsAvailable = false; // Set
		}
		else
		{
			GlobalHook.KeyDown -= GlobalHook_KeyDown; // Unsubscribe
			Global.KeyBoardShortcutsAvailable = true; // Set
			if (SelectShortcutTxt.Text.Length == 0) SelectShortcutTxt.Text = Global.Settings.SelectKeyboardShortcut; // Set default

			Global.Settings.SelectKeyboardShortcut = SelectShortcutTxt.Text; // Set
			SettingsManager.Save(); // Save changes
		}
	}
}
