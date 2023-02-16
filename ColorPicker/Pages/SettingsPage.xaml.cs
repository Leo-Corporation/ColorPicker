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
using ColorPicker.Enums;
using Gma.System.MouseKeyHook;
using Microsoft.Win32;
using PeyrSharp.Core;
using PeyrSharp.Env;
using Synethia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace ColorPicker.Pages
{
	/// <summary>
	/// Interaction logic for SettingsPage.xaml
	/// </summary>
	public partial class SettingsPage : Page
	{
		private IKeyboardMouseEvents keyboardEvents = Hook.GlobalEvents();
		public SettingsPage()
		{
			InitializeComponent();
			InitUI();
		}

		readonly System.Windows.Forms.NotifyIcon notifyIcon = new();
		private async void InitUI()
		{
			// About section
			VersionTxt.Text = Global.Version; // Update the current version label

			// Select the language
			LangComboBox.SelectedIndex = (int)Global.Settings.Language;

			// Select the default theme border
			ThemeSelectedBorder = Global.Settings.Theme switch
			{
				Themes.Light => LightBorder,
				Themes.Dark => DarkBorder,
				_ => SystemBorder
			};
			Border_MouseEnter(Global.Settings.Theme switch
			{
				Themes.Light => LightBorder,
				Themes.Dark => DarkBorder,
				_ => SystemBorder
			}, null);

			// Load the default color type
			ColorTypeComboBox.SelectedIndex = (int)Global.Settings.DefaultColorType;

			// Load the default page ComboBox
			PageComboBox.SelectedIndex = (int)Global.Settings.DefaultPage;

			// Load the keyboard shortcuts section
			CopyShortcutTxt.Text = Global.Settings.CopyKeyboardShortcut;
			SelectShortcutTxt.Text = Global.Settings.SelectKeyboardShortcut;

			// Load the Text Tool section
			System.Drawing.Text.InstalledFontCollection installedFonts = new();
			foreach (System.Drawing.FontFamily fontFamily in installedFonts.Families)
			{
				FontComboBox.Items.Add(fontFamily.Name);
			}
			FontComboBox.Text = Global.Settings.TextToolFont;
			FontSizeTxt.Text = Global.Settings.TextToolFontSize.ToString();
			ColorHelper.RGB foreground = ColorHelper.ColorConverter.HexToRgb(new(Global.Settings.TextToolForeground));
			ColorHelper.RGB background = ColorHelper.ColorConverter.HexToRgb(new(Global.Settings.TextToolBackground));

			ForegroundBorder.Background = new SolidColorBrush { Color = Color.FromRgb(foreground.R, foreground.G, foreground.B) };
			BackgroundBorder.Background = new SolidColorBrush { Color = Color.FromRgb(background.R, background.G, background.B) };

			// Checkboxes
			UpdateOnStartChk.IsChecked = Global.Settings.CheckUpdateOnStart;
			UseKeyboardShortcutsChk.IsChecked = Global.Settings.UseKeyboardShortcuts;
			UseSynethiaChk.IsChecked = Global.Settings.UseSynethia;

			if (!Global.Settings.CheckUpdateOnStart) return;
			try
			{
				if (!await Internet.IsAvailableAsync()) return;
				if (!Update.IsAvailable(Global.Version, await Update.GetLastVersionAsync(Global.LastVersionLink))) return;
			}
			catch { return; }

			// If updates are available
			// Update the UI
			CheckUpdateBtn.Content = Properties.Resources.Install;
			UpdateTxt.Text = Properties.Resources.AvailableUpdates;

			// Show notification
			notifyIcon.Visible = true; // Show
			notifyIcon.ShowBalloonTip(5000, Properties.Resources.ColorPickerMax, Properties.Resources.AvailableUpdates, System.Windows.Forms.ToolTipIcon.Info);
			notifyIcon.Visible = false; // Hide
		}

		private async void CheckUpdateBtn_Click(object sender, RoutedEventArgs e)
		{
			string lastVersion = await Update.GetLastVersionAsync(Global.LastVersionLink);
			if (Update.IsAvailable(Global.Version, lastVersion))
			{
				UpdateTxt.Text = Properties.Resources.AvailableUpdates;

				if (MessageBox.Show(Properties.Resources.InstallConfirmMsg, $"{Properties.Resources.InstallVersion} {lastVersion}", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
				{
					return;
				}

				// If the user wants to proceed.
				SynethiaManager.Save(Global.SynethiaConfig, Global.SynethiaPath);
				XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
				XmlSerializerManager.SaveToXml(Global.Bookmarks, Global.BookmarksPath);

				Sys.ExecuteAsAdmin(Directory.GetCurrentDirectory() + @"\Xalyus Updater.exe"); // Start the updater
				Application.Current.Shutdown(); // Close
			}
			else
			{
				UpdateTxt.Text = Properties.Resources.UpToDate;
			}
		}

		Border ThemeSelectedBorder;
		private void Border_MouseEnter(object sender, MouseEventArgs e)
		{
			((Border)sender).BorderBrush = new SolidColorBrush { Color = Global.GetColorFromResource("AccentColor") };
		}

		private void Border_MouseLeave(object sender, MouseEventArgs e)
		{
			if ((Border)sender == ThemeSelectedBorder) return;
			((Border)sender).BorderBrush = new SolidColorBrush { Color = Colors.Transparent };
		}

		private void ResetBorders()
		{
			LightBorder.BorderBrush = new SolidColorBrush { Color = Colors.Transparent };
			DarkBorder.BorderBrush = new SolidColorBrush { Color = Colors.Transparent };
			SystemBorder.BorderBrush = new SolidColorBrush { Color = Colors.Transparent };
		}

		private void LightBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ResetBorders();
			ThemeSelectedBorder = (Border)sender;
			((Border)sender).BorderBrush = new SolidColorBrush { Color = Global.GetColorFromResource("AccentColor") };
			Global.Settings.Theme = Themes.Light;
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);

			if (MessageBox.Show(Properties.Resources.NeedRestartToApplyChanges, Properties.Resources.Settings, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
			{
				return;
			}

			SynethiaManager.Save(Global.SynethiaConfig, Global.SynethiaPath);
			XmlSerializerManager.SaveToXml(Global.Bookmarks, Global.BookmarksPath);

			Process.Start(Directory.GetCurrentDirectory() + @"\ColorPicker.exe");
			Application.Current.Shutdown();
		}

		private void DarkBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ResetBorders();
			ThemeSelectedBorder = (Border)sender;
			((Border)sender).BorderBrush = new SolidColorBrush { Color = Global.GetColorFromResource("AccentColor") };
			Global.Settings.Theme = Themes.Dark;
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);

			if (MessageBox.Show(Properties.Resources.NeedRestartToApplyChanges, Properties.Resources.Settings, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
			{
				return;
			}

			SynethiaManager.Save(Global.SynethiaConfig, Global.SynethiaPath);
			XmlSerializerManager.SaveToXml(Global.Bookmarks, Global.BookmarksPath);

			Process.Start(Directory.GetCurrentDirectory() + @"\ColorPicker.exe");
			Application.Current.Shutdown();
		}

		private void SystemBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ResetBorders();
			ThemeSelectedBorder = (Border)sender;
			((Border)sender).BorderBrush = new SolidColorBrush { Color = Global.GetColorFromResource("AccentColor") };
			Global.Settings.Theme = Themes.System;
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);

			if (MessageBox.Show(Properties.Resources.NeedRestartToApplyChanges, Properties.Resources.Settings, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
			{
				return;
			}

			SynethiaManager.Save(Global.SynethiaConfig, Global.SynethiaPath);
			XmlSerializerManager.SaveToXml(Global.Bookmarks, Global.BookmarksPath);

			Process.Start(Directory.GetCurrentDirectory() + @"\ColorPicker.exe");
			Application.Current.Shutdown();
		}

		private void LangComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LangApplyBtn.Visibility = Visibility.Visible; // Show apply button
		}

		private void LangApplyBtn_Click(object sender, RoutedEventArgs e)
		{
			Global.Settings.Language = (Languages)LangComboBox.SelectedIndex;
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
			LangApplyBtn.Visibility = Visibility.Hidden; // Hide apply button

			if (MessageBox.Show(Properties.Resources.NeedRestartToApplyChanges, Properties.Resources.Settings, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
			{
				return;
			}

			SynethiaManager.Save(Global.SynethiaConfig, Global.SynethiaPath);
			XmlSerializerManager.SaveToXml(Global.Bookmarks, Global.BookmarksPath);

			Process.Start(Directory.GetCurrentDirectory() + @"\ColorPicker.exe");
			Application.Current.Shutdown();
		}

		private void ColorTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Global.Settings.DefaultColorType = (ColorTypes)ColorTypeComboBox.SelectedIndex;
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
		}

		private void PageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Global.Settings.DefaultPage = (AppPages)PageComboBox.SelectedIndex;
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
		}

		private void UpdateOnStartChk_Checked(object sender, RoutedEventArgs e)
		{
			Global.Settings.CheckUpdateOnStart = UpdateOnStartChk.IsChecked ?? true;
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
		}

		bool selectingKeys = false, fromSelect = false;
		List<string> pressedKeys = new();
		private void EditSelectShortcutBtn_Click(object sender, RoutedEventArgs e)
		{
			EditSelectShortcutBtn.Content = !selectingKeys ? "\uF295" : "\uF3DE"; // Set text

			if (selectingKeys)
			{
				keyboardEvents.KeyDown -= KeyboardEvents_KeyDown;
				EditCopyShortcutBtn.IsEnabled = true;
				fromSelect = false;
				if (pressedKeys.Count == 0)
				{
					SelectShortcutTxt.Text = Global.Settings.SelectKeyboardShortcut;
				}
				else
				{
					Global.Settings.SelectKeyboardShortcut = SelectShortcutTxt.Text;
					XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
				}
			}
			else
			{
				fromSelect = true;
				EditCopyShortcutBtn.IsEnabled = false;
				SelectShortcutTxt.Text = "";
				keyboardEvents.KeyDown += KeyboardEvents_KeyDown;
			}
			selectingKeys = !selectingKeys;
			pressedKeys.Clear();
		}

		private void KeyboardEvents_KeyDown(object? sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (pressedKeys.Contains(e.KeyCode.ToString())) return;
			pressedKeys.Add(e.KeyCode.ToString());

			if (fromSelect) SelectShortcutTxt.Text += (SelectShortcutTxt.Text.Length == 0) ? e.KeyCode.ToString() : $"+{e.KeyCode}";
			else CopyShortcutTxt.Text += (CopyShortcutTxt.Text.Length == 0) ? e.KeyCode.ToString() : $"+{e.KeyCode}";
		}

		private void ResetSelectShortcutBtn_Click(object sender, RoutedEventArgs e)
		{
			SelectShortcutTxt.Text = "Shift+S"; // Set default value (Shift+S) to textbox
			Global.Settings.SelectKeyboardShortcut = "Shift+S"; // Set default value
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
		}

		private void UseKeyboardShortcutsChk_Checked(object sender, RoutedEventArgs e)
		{
			Global.Settings.UseKeyboardShortcuts = UseKeyboardShortcutsChk.IsChecked ?? true;
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
		}

		private void EditCopyShortcutBtn_Click(object sender, RoutedEventArgs e)
		{
			fromSelect = false;
			EditCopyShortcutBtn.Content = !selectingKeys ? "\uF295" : "\uF3DE"; // Set text
			if (selectingKeys)
			{
				keyboardEvents.KeyDown -= KeyboardEvents_KeyDown;
				EditSelectShortcutBtn.IsEnabled = true;
				if (pressedKeys.Count == 0)
				{
					CopyShortcutTxt.Text = Global.Settings.CopyKeyboardShortcut;
				}
				else
				{
					Global.Settings.CopyKeyboardShortcut = CopyShortcutTxt.Text;
					XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
				}
			}
			else
			{
				CopyShortcutTxt.Text = "";
				keyboardEvents.KeyDown += KeyboardEvents_KeyDown;
				EditSelectShortcutBtn.IsEnabled = false;
			}
			selectingKeys = !selectingKeys;
			pressedKeys.Clear();
		}

		private void ResetCopyShortcutBtn_Click(object sender, RoutedEventArgs e)
		{
			CopyShortcutTxt.Text = "Shift+C"; // Set default value (Shift+C) to textbox
			Global.Settings.CopyKeyboardShortcut = "Shift+C"; // Set default value
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
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
				Global.Settings.TextToolForeground = ColorHelper.ColorConverter.RgbToHex(new(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B)).Value;
				XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
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
				Global.Settings.TextToolBackground = ColorHelper.ColorConverter.RgbToHex(new(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B)).Value;
				XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
			}
		}

		private void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Global.Settings.TextToolFont = FontComboBox.SelectedItem.ToString();
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
		}

		private void FontSizeTxt_TextChanged(object sender, TextChangedEventArgs e)
		{
			Global.Settings.TextToolFontSize = int.Parse(FontSizeTxt.Text);
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
		}

		private void FontSizeTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
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
				Global.Settings = XmlSerializerManager.LoadFromXml<Settings>(openFileDialog.FileName); // Import
				XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
				MessageBox.Show(Properties.Resources.SettingsImportedMsg, Properties.Resources.ColorPickerMax, MessageBoxButton.OK, MessageBoxImage.Information); // Show error message

				// Restart app
				Process.Start(Directory.GetCurrentDirectory() + @"\ColorPicker.exe"); // Start app
				Environment.Exit(0); // Quit
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
				XmlSerializerManager.SaveToXml(Global.Settings, saveFileDialog.FileName); // Export games
				MessageBox.Show(Properties.Resources.SettingsExportedSucessMsg, Properties.Resources.ColorPickerMax, MessageBoxButton.OK, MessageBoxImage.Information); // Show message
			}
		}


		private void ResetSettingsLink_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show(Properties.Resources.ResetSettingsConfirmation, Properties.Resources.Settings, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
			{
				return;
			}

			Global.Settings = new() { IsFirstRun = false };
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);


			if (MessageBox.Show(Properties.Resources.NeedRestartToApplyChanges, Properties.Resources.Settings, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
			{
				return;
			}

			SynethiaManager.Save(Global.SynethiaConfig, Global.SynethiaPath);
			XmlSerializerManager.SaveToXml(Global.Bookmarks, Global.BookmarksPath);

			Process.Start(Directory.GetCurrentDirectory() + @"\ColorPicker.exe");
			Application.Current.Shutdown();
		}

		private void UseSynethiaChk_Checked(object sender, RoutedEventArgs e)
		{
			Global.Settings.UseSynethia = UseSynethiaChk.IsChecked ?? true;
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
		}

		private void SeeLicensesBtn_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show($"{Properties.Resources.Licenses}\n\n" +
			"Fluent System Icons - MIT License - © 2020 Microsoft Corporation\n" +
			"ColorHelper - MIT License - © 2020 Artyom Gritsuk\n" +
			"globalmousekeyhook - MIT License - © 2010-2018 George Mamaladze\n" +
			"PeyrSharp - MIT License - © 2022-2023 Léo Corporation\n" +
			"ColorPicker - MIT License - © 2021-2023 Léo Corporation", $"{Properties.Resources.ColorPickerMax} - {Properties.Resources.Licenses}", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void ResetSynethiaLink_Click(object sender, RoutedEventArgs e)
		{
			// Ask the user a confirmation
			if (MessageBox.Show(Properties.Resources.SynethiaDeleteMsg, Properties.Resources.Settings, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
			{
				return;
			}

			// If the user wants to proceed, reset Syenthia config file.
			Global.SynethiaConfig = Global.Default;
			SynethiaManager.Save(Global.SynethiaConfig, Global.SynethiaPath);

			// Ask the user if they want to restart the application to apply changes.
			if (MessageBox.Show(Properties.Resources.NeedRestartToApplyChanges, Properties.Resources.Settings, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
			{
				return;
			}

			// If the user wants to restart the app, save and quit the app
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
			XmlSerializerManager.SaveToXml(Global.Bookmarks, Global.BookmarksPath);

			Process.Start(Directory.GetCurrentDirectory() + @"\ColorPicker.exe"); // Start a new instance
			Application.Current.Shutdown(); // Quit this current instance
		}
	}
}
