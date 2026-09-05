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
using Betalgo.Ranul.OpenAI.Managers;
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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ColorPicker.Pages;

/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class SettingsPage : Page
{
	private readonly IKeyboardMouseEvents keyboardEvents = Hook.GlobalEvents();
	public SettingsPage()
	{
		InitializeComponent();
		InitUI();
	}

	readonly System.Windows.Forms.NotifyIcon notifyIcon = new();
	bool updatesAvailable = false;
	bool loaded = false;
	private async void InitUI()
	{
		// Select the language
		LangComboBox.SelectedIndex = (int)Global.Settings.Language;

		// Select the default theme border
		switch (Global.Settings.Theme)
		{
			case Themes.System:
				DefaultBtn.IsChecked = true;
				break;
			case Themes.Light:
				LightBtn.IsChecked = true;
				break;
			case Themes.Dark:
				DarkBtn.IsChecked = true;
				break;
		}

		// Load the color option section
		Global.Settings.RgbSeparator ??= ";";
		Global.Settings.UseUpperCasesHex ??= false;
		ColorTypeComboBox.SelectedIndex = (int)Global.Settings.DefaultColorType;
		RgbSeparatorTxt.Text = Global.Settings.RgbSeparator;
		UpperCaseHexChk.IsChecked = Global.Settings.UseUpperCasesHex;

		// Load the default page ComboBox
		PageComboBox.SelectedIndex = (int)Global.Settings.DefaultPage;

		// Load the keyboard shortcuts section
		CopyShortcutTxt.Text = Global.Settings.CopyKeyboardShortcut;
		SelectShortcutTxt.Text = Global.Settings.SelectKeyboardShortcut;

		// Load the AI section
		Global.Settings.ApiKey ??= "";
		Global.Settings.ApiEndpoint ??= "";
		Global.Settings.Model ??= "gpt-3.5-turbo";
		Global.Settings.CustomModelId ??= "";
		ApiKeyTxt.Password = Global.Settings.ApiKey;
		ApiEndpointTxt.Text = Global.Settings.ApiEndpoint;
		CustomModelIdTxt.Text = Global.Settings.CustomModelId;
		for (int i = 0; i < Global.Settings.SupportedModels.Length; i++)
		{
			ModelComboBox.Items.Add(Global.Settings.SupportedModels[i]);
		}
		ModelComboBox.SelectedItem = Global.Settings.Model;

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
		Global.Settings.LaunchOnStart ??= false;
		UpdateOnStartChk.IsChecked = Global.Settings.CheckUpdateOnStart;
		LaunchOnStartChk.IsChecked = Global.Settings.LaunchOnStart;
		loaded = true;
		UseKeyboardShortcutsChk.IsChecked = Global.Settings.UseKeyboardShortcuts;
		UseSynethiaChk.IsChecked = Global.Settings.UseSynethia;

		loaded = true;

		if (!Global.Settings.CheckUpdateOnStart) return;
		try
		{
			if (!await Internet.IsAvailableAsync()) return;
			if (!Update.IsAvailable(Global.Version, await Update.GetLastVersionAsync(Global.LastVersionLink))) return;
		}
		catch { return; }

		// If updates are available
		// Update the UI
		updatesAvailable = true;
		CheckUpdateBtn.Content = Properties.Resources.Install;
		LoadUpdateSection();

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
			updatesAvailable = true;
			LoadUpdateSection();

#if PORTABLE
			MessageBox.Show(Properties.Resources.PortableNoAutoUpdates, $"{Properties.Resources.InstallVersion} {lastVersion}", MessageBoxButton.OK, MessageBoxImage.Information);
			return;
#else
			if (MessageBox.Show(Properties.Resources.InstallConfirmMsg, $"{Properties.Resources.InstallVersion} {lastVersion}", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
			{
				return;
			}
#endif

			// If the user wants to proceed.
			SynethiaManager.Save(Global.SynethiaConfig, Global.SynethiaPath);
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
			XmlSerializerManager.SaveToXml(Global.Bookmarks, Global.BookmarksPath);

			Sys.ExecuteAsAdmin(Directory.GetCurrentDirectory() + @"\Xalyus Updater.exe"); // Start the updater
			Application.Current.Shutdown(); // Close
		}
		else
		{
			updatesAvailable = false;
			LoadUpdateSection();
		}
	}

	internal void LoadUpdateSection()
	{
		if (updatesAvailable)
		{
			UpdateTxt.Text = Properties.Resources.AvailableUpdates;
			UpdateIconTxt.Text = "\uF86A";
			UpdateTxt.Foreground = Global.GetColorFromResource("ForegroundOrange");
			UpdateIconTxt.Foreground = Global.GetColorFromResource("ForegroundOrange");
			UpdateBorder.Background = Global.GetColorFromResource("LightOrange");
			CheckUpdateBtn.Foreground = Global.GetColorFromResource("ForegroundOrange");
			CheckUpdateBtn.Content = Properties.Resources.Install;
			CheckUpdateBtn.FontFamily = new(new Uri("pack://application:,,,/"), "./Fonts/#Hauora");
			CheckUpdateBtn.FontSize = 12;
			CheckUpdateBtn.FontWeight = FontWeights.ExtraBold;
		}
		else
		{
			UpdateTxt.Text = Properties.Resources.UpToDate;
			UpdateIconTxt.Text = "\uF299";
			UpdateTxt.Foreground = Global.GetColorFromResource("ForegroundGreen");
			UpdateIconTxt.Foreground = Global.GetColorFromResource("ForegroundGreen");
			UpdateBorder.Background = Global.GetColorFromResource("LightGreen");
			CheckUpdateBtn.Foreground = Global.GetColorFromResource("ForegroundGreen");
			CheckUpdateBtn.Content = "\uF191";
			CheckUpdateBtn.FontFamily = new(new Uri("pack://application:,,,/"), "./Fonts/#FluentSystemIcons-Regular");
			CheckUpdateBtn.FontSize = 14;
			CheckUpdateBtn.FontWeight = FontWeights.Normal;
		}
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
	readonly List<string> pressedKeys = [];
	private void EditSelectShortcutBtn_Click(object sender, RoutedEventArgs e)
	{
		EditSelectShortcutBtn.Content = !selectingKeys ? "\uF295" : "\uF3DE"; // Set text

		if (selectingKeys)
		{
			keyboardEvents.KeyDown -= KeyboardEvents_KeyDown;
			EditCopyShortcutBtn.IsEnabled = true;
			fromSelect = false;
			if (pressedKeys.Count == 0 || Global.IsSameKeyboardShortcut(SelectShortcutTxt.Text, Global.Settings.CopyKeyboardShortcut))
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
			if (pressedKeys.Count == 0 || Global.IsSameKeyboardShortcut(CopyShortcutTxt.Text, Global.Settings.SelectKeyboardShortcut))
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
		Global.Settings.TextToolFont = FontComboBox.SelectedItem.ToString() ?? "Arial";
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
			Global.Settings = XmlSerializerManager.LoadFromXml<Settings>(openFileDialog.FileName) ?? new(); // Import
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
		"PeyrSharp - MIT License - © 2022-2026 Léo Corporation\n" +
		"ColorPicker - MIT License - © 2021-2026 Léo Corporation", $"{Properties.Resources.ColorPickerMax} - {Properties.Resources.Licenses}", MessageBoxButton.OK, MessageBoxImage.Information);
	}

	private void SaveAiSettings()
	{
		Global.Settings.ApiKey = ApiKeyTxt.Password;
		Global.Settings.ApiEndpoint = ApiEndpointTxt.Text;
		Global.Settings.CustomModelId = CustomModelIdTxt.Text;
		XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
		Global.SaveUserAiSettings();
	}

	private void SaveAiSettingsBtn_Click(object sender, RoutedEventArgs e)
	{
		SaveAiSettings();
		MessageBox.Show(Properties.Resources.Settings, Properties.Resources.ColorPickerMax, MessageBoxButton.OK, MessageBoxImage.Information);
	}

	private async void TestAiConnectionBtn_Click(object sender, RoutedEventArgs e)
	{
		SaveAiSettings();
		TestAiConnectionBtn.IsEnabled = false;
		TestResultBadge.Visibility = Visibility.Collapsed;

		// Start rotating animation on icon
		DoubleAnimation spinAnimation = new()
		{
			From = 0,
			To = 360,
			Duration = TimeSpan.FromSeconds(1),
			RepeatBehavior = RepeatBehavior.Forever
		};
		TestIconRotator.BeginAnimation(RotateTransform.AngleProperty, spinAnimation);

		try
		{
			var openAiService = AiGenPage.CreateOpenAIService();
			string targetModel = !string.IsNullOrWhiteSpace(Global.Settings.CustomModelId)
				? Global.Settings.CustomModelId.Trim()
				: (Global.Settings.Model ?? Betalgo.Ranul.OpenAI.ObjectModels.Models.Gpt_3_5_Turbo);

			var res = await openAiService.ChatCompletion.CreateCompletion(new Betalgo.Ranul.OpenAI.ObjectModels.RequestModels.ChatCompletionCreateRequest
			{
				Messages = [Betalgo.Ranul.OpenAI.ObjectModels.RequestModels.ChatMessage.FromUser("hi")],
				Model = targetModel,
				MaxTokens = 5
			});

			if (res.Successful)
			{
				TestStatusTitle.Foreground = new SolidColorBrush(Color.FromRgb(16, 124, 65)); // Green
				TestStatusTitle.Text = "连接成功！";
				string reply = res.Choices?.FirstOrDefault()?.Message?.Content ?? "(无返回文本)";
				TestResultTxt.Text = $"响应内容: {reply}";
			}
			else
			{
				TestStatusTitle.Foreground = new SolidColorBrush(Color.FromRgb(209, 52, 56)); // Red
				TestStatusTitle.Text = "连接失败";
				string errorMsg = res.Error?.Message ?? "未知错误";
				string errorCode = res.Error?.Code ?? "N/A";
				string errorType = res.Error?.Type ?? "N/A";
				TestResultTxt.Text = $"错误代码: {errorCode}\n错误类型: {errorType}\n详细提示: {errorMsg}";
			}
		}
		catch (Exception ex)
		{
			TestStatusTitle.Foreground = new SolidColorBrush(Color.FromRgb(209, 52, 56)); // Red
			TestStatusTitle.Text = "异常报错";
			TestResultTxt.Text = $"{ex.GetType().Name}: {ex.Message}\n{ex.InnerException?.Message}";
		}
		finally
		{
			TestIconRotator.BeginAnimation(RotateTransform.AngleProperty, null); // Stop spinning
			TestResultBadge.Visibility = Visibility.Visible;
			TestAiConnectionBtn.IsEnabled = true;
		}
	}

	private void ApiEndpointTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (!loaded) return;
		Global.Settings.ApiEndpoint = ApiEndpointTxt.Text;
		Global.SaveUserAiSettings();
	}

	private void CustomModelIdTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (!loaded) return;
		Global.Settings.CustomModelId = CustomModelIdTxt.Text;
		Global.SaveUserAiSettings();
	}

	private void ApiKeyTxt_PasswordChanged(object sender, RoutedEventArgs e)
	{
		if (!loaded) return;
		Global.Settings.ApiKey = ApiKeyTxt.Password;
		Global.SaveUserAiSettings();
	}

	private void ModelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		try
		{
			Global.Settings.Model = Global.Settings.SupportedModels[ModelComboBox.SelectedIndex];
			Global.SaveUserAiSettings();
		}
		catch { }
	}

	private async void RefreshModelsBtn_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(Global.Settings.ApiKey)) return;

		try
		{
			OpenAIService sdk = new(new() { ApiKey = Global.Settings.ApiKey });
			var modelList = await sdk.Models.ListModel();

			var sortedModels = modelList.Models.Select(m => m.Id).Where(m => m.StartsWith("gpt")).ToList();
			sortedModels.Sort();

			Global.Settings.SupportedModels = [.. sortedModels];
			XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);

			ModelComboBox.Items.Clear();
			for (int i = 0; i < Global.Settings.SupportedModels.Length; i++)
			{
				ModelComboBox.Items.Add(Global.Settings.SupportedModels[i]);
			}
			ModelComboBox.SelectedItem = Global.Settings.Model;
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, Properties.Resources.ColorPicker, MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	private void RgbSeparatorTxt_TextChanged(object sender, TextChangedEventArgs e)
	{
		Global.Settings.RgbSeparator = RgbSeparatorTxt.Text;
		XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
	}

	private void UpperCaseHexChk_Checked(object sender, RoutedEventArgs e)
	{
		Global.Settings.UseUpperCasesHex = UpperCaseHexChk.IsChecked;
		XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
	}

	private void GitHubBtn_Click(object sender, RoutedEventArgs e)
	{
		Process.Start("explorer.exe", "https://github.com/Leo-Corporation/ColorPicker");
	}

	private void LaunchOnStartChk_Checked(object sender, RoutedEventArgs e)
	{
		Global.SetStartOnWindowsStart(LaunchOnStartChk.IsChecked ?? false);
	}

	private void LightBtn_Checked(object sender, RoutedEventArgs e)
	{
		if (!loaded) return;
		Global.Settings.Theme = ((RadioButton)sender).Name switch
		{
			"DefaultBtn" => Themes.System,
			"LightBtn" => Themes.Light,
			"DarkBtn" => Themes.Dark,
			_ => Global.Settings.Theme
		};

		XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);

		SynethiaManager.Save(Global.SynethiaConfig, Global.SynethiaPath);
		XmlSerializerManager.SaveToXml(Global.Bookmarks, Global.BookmarksPath);

		Global.ChangeTheme(true);
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
