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
using ColorPicker.Enums;
using Gma.System.MouseKeyHook;
using PeyrSharp.Env;
using Synethia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

		private void InitUI()
		{
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

			UpdateOnStartChk.IsChecked = Global.Settings.CheckUpdateOnStart;
			UseKeyboardShortcutsChk.IsChecked = Global.Settings.UseKeyboardShortcuts;
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
	}
}
