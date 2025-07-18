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

using ColorHelper;
using ColorPicker.Classes;
using ColorPicker.UserControls;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using Synethia;
using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for AiGenPage.xaml
/// </summary>
public partial class AiGenPage : Page
{
	bool code = !Global.Settings.UseSynethia; // checks if the code as already been implemented
	ColorInfo ColorInfo { get; set; } = null!;
	DetailsControl DetailsControl { get; set; } = new(new(new(0, 0, 0))); // Details control

	public AiGenPage()
	{
		InitializeComponent();
		InitUI();
		Loaded += (o, e) => SynethiaManager.InjectSynethiaCode(this, Global.SynethiaConfig.PagesInfo, 4, ref code); // injects the code in the page
		ColorBtn.IsChecked = true;
	}

	private void InitUI()
	{
		DetailsWrap.Children.Add(DetailsControl); // Add the details control to the page
		if (!string.IsNullOrEmpty(Global.Settings.ApiKey))
		{
			ColorPanel.Visibility = Visibility.Visible;
			ApiPlaceholder.Visibility = Visibility.Collapsed;
			NavGrid.Visibility = Visibility.Visible;
			return;
		}
	}

	internal void LoadDetails()
	{
		ColorBorder.Background = new SolidColorBrush { Color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B) };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B) };
		ColorBorder.Visibility = Visibility.Visible;
		EmptyPlaceholder.Visibility = Visibility.Collapsed;

		// Load the details
		DetailsControl.SetColorInfo(ColorInfo);

		// Load the bookmark icon
		BookmarkBtn.Content = Global.Bookmarks.ColorBookmarks.Contains(ColorInfo.HEX.Value) ? "\uF1F8" : "\uF1F6";
		BookmarkBtn.Visibility = Visibility.Visible;

		DetailsHeader.Visibility = Visibility.Visible;
		DetailsWrap.Visibility = Visibility.Visible;
	}

	private void LoadBorders(string[] colors)
	{
		try
		{
			RGB[] convertedColors = new RGB[5];
			for (int i = 0; i < colors.Length; i++)
			{
				convertedColors[i] = ColorHelper.ColorConverter.HexToRgb(new(colors[i]));
			}

			C1.Background = new SolidColorBrush { Color = Color.FromRgb(convertedColors[0].R, convertedColors[0].G, convertedColors[0].B) };
			C2.Background = new SolidColorBrush { Color = Color.FromRgb(convertedColors[1].R, convertedColors[1].G, convertedColors[1].B) };
			C3.Background = new SolidColorBrush { Color = Color.FromRgb(convertedColors[2].R, convertedColors[2].G, convertedColors[2].B) };
			C4.Background = new SolidColorBrush { Color = Color.FromRgb(convertedColors[3].R, convertedColors[3].G, convertedColors[3].B) };
			C5.Background = new SolidColorBrush { Color = Color.FromRgb(convertedColors[4].R, convertedColors[4].G, convertedColors[4].B) };

			C1Shadow.Color = Color.FromRgb(convertedColors[0].R, convertedColors[0].G, convertedColors[0].B);
			C2Shadow.Color = Color.FromRgb(convertedColors[1].R, convertedColors[1].G, convertedColors[1].B);
			C3Shadow.Color = Color.FromRgb(convertedColors[2].R, convertedColors[2].G, convertedColors[2].B);
			C4Shadow.Color = Color.FromRgb(convertedColors[3].R, convertedColors[3].G, convertedColors[3].B);
			C5Shadow.Color = Color.FromRgb(convertedColors[4].R, convertedColors[4].G, convertedColors[4].B);

			PaletteBordersPanel.Visibility = Visibility.Visible;
			EmptyPalettePlaceholder.Visibility = Visibility.Collapsed;
		}
		catch (Exception e)
		{
			MessageBox.Show(e.Message, Properties.Resources.AIGeneration, MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	private async void GenerateBtn_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(Global.Settings.ApiKey) || string.IsNullOrWhiteSpace(Global.Settings.ApiKey))
		{
			MessageBox.Show(Properties.Resources.ProvideAPIKey, Properties.Resources.AIGenerateColor, MessageBoxButton.OK, MessageBoxImage.Information);
			return;
		}

		if (string.IsNullOrEmpty(PromptTxt.Text) || string.IsNullOrWhiteSpace(PromptTxt.Text))
		{
			MessageBox.Show(Properties.Resources.ProvideAPromptMsg, Properties.Resources.AIGenerateColor, MessageBoxButton.OK, MessageBoxImage.Information);
			return;
		}
		Global.SynethiaConfig.ActionsInfo[6].UsageCount++; // Increment the usage counter
		try
		{
			var openAiService = new OpenAIService(new OpenAiOptions()
			{
				ApiKey = Global.Settings.ApiKey ?? ""
			});
			var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
			{
				Messages =
				[
					ChatMessage.FromSystem("GOAL: You are a color generator assistant. The user gives you a prompt to generate a SINGLE color. RESPONDE FORMAT: Only the color is in hexadecimal format, i.e.: #FFFFFF. "),
					ChatMessage.FromUser("Generate a color from this prompt: " + PromptTxt.Text)
				],
				Model = Global.Settings.Model ?? Models.Gpt_3_5_Turbo,
			});

			if (completionResult.Successful)
			{
				ColorInfo = new(ColorHelper.ColorConverter.HexToRgb(new(completionResult.Choices.First().Message.Content)));
				LoadDetails();
			}
		}
		catch { }
	}

	private async void PaletteGenerateBtn_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(Global.Settings.ApiKey) || string.IsNullOrWhiteSpace(Global.Settings.ApiKey))
		{
			MessageBox.Show(Properties.Resources.ProvideAPIKey, Properties.Resources.AIGenerateColor, MessageBoxButton.OK, MessageBoxImage.Information);
			return;
		}

		if (string.IsNullOrEmpty(PalettePromptTxt.Text) || string.IsNullOrWhiteSpace(PalettePromptTxt.Text))
		{
			MessageBox.Show(Properties.Resources.ProvideAPromptMsg, Properties.Resources.AIGenerateColor, MessageBoxButton.OK, MessageBoxImage.Information);
			return;
		}
		Global.SynethiaConfig.ActionsInfo[6].UsageCount++; // Increment the usage counter
		try
		{
			var openAiService = new OpenAIService(new OpenAiOptions()
			{
				ApiKey = Global.Settings.ApiKey ?? ""
			});
			var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
			{
				Messages =
				[
					ChatMessage.FromSystem("GOAL: You are a color palette assistant. The user gives you a prompt to generate a EXACLY FIVE colors. FORMAT: ONLY ANWSER LIKE THIS (with colors instead of \"...\"): [\"#FFFFFF\", \"#000000\", \"...\", \"...\", \"...\"]"),
					ChatMessage.FromUser(PalettePromptTxt.Text)
				],
				Model = Models.Gpt_3_5_Turbo,
			});

			if (completionResult.Successful)
			{
				var colors = JsonSerializer.Deserialize<string[]>(completionResult.Choices.First().Message.Content ?? "");
				LoadBorders(colors ?? []);
			}
		}
		catch { }
	}

	private void C1_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		var bg = (SolidColorBrush)((Border)sender).Background;
		ColorInfo = new(new(bg.Color.R, bg.Color.G, bg.Color.B));
		LoadDetails();
	}

	private void ApiApplyBtn_Click(object sender, RoutedEventArgs e)
	{
		Global.Settings.ApiKey = ApiKeyTxt.Password;
		XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
		InitUI();
	}

	private void BookmarkBtn_Click(object sender, RoutedEventArgs e)
	{
		var bg = (SolidColorBrush)ColorBorder.Background;
		string hex = $"#{new ColorInfo(new(bg.Color.R, bg.Color.G, bg.Color.B)).HEX.Value}";
		if (Global.Bookmarks.ColorBookmarks.Contains(hex))
		{
			int i = Global.Bookmarks.ColorBookmarks.IndexOf(hex);
			Global.Bookmarks.ColorBookmarks.RemoveAt(i);
			Global.Bookmarks.ColorBookmarksNotes.RemoveAt(i); // Add note
			BookmarkBtn.Content = "\uF1F6";
			BookmarkToolTip.Content = Properties.Resources.AddBookmark;
			return;
		}
		Global.Bookmarks.ColorBookmarks.Add(hex); // Add to color bookmarks
		Global.Bookmarks.ColorBookmarksNotes.Add(""); // Add note

		BookmarkBtn.Content = "\uF1F8";
		BookmarkToolTip.Content = Properties.Resources.RemoveBookmark;
	}

	private void IdeaBtn_Click(object sender, RoutedEventArgs e)
	{
		PromptTxt.Text = Global.GetRandomAiPrompt();
	}

	private void IdeaPaletteBtn_Click(object sender, RoutedEventArgs e)
	{
		PalettePromptTxt.Text = Global.GetRandomAiPrompt();
	}

	private void ColorBtn_Checked(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(Global.Settings.ApiKey)) return;
		if (ColorInfo is null) BookmarkBtn.Visibility = Visibility.Collapsed;
		ColorPanel.Visibility = Visibility.Visible;
		PalettePanel.Visibility = Visibility.Collapsed;
		DescPanel.Visibility = Visibility.Collapsed;
	}

	private void PaletteBtn_Checked(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(Global.Settings.ApiKey)) return;
		if (ColorInfo is null) BookmarkBtn.Visibility = Visibility.Collapsed;
		ColorPanel.Visibility = Visibility.Collapsed;
		PalettePanel.Visibility = Visibility.Visible;
		DescPanel.Visibility = Visibility.Collapsed;
	}

	private void DescribeBtn_Checked(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(Global.Settings.ApiKey)) return;
		if (ColorInfo is null) BookmarkBtn.Visibility = Visibility.Collapsed;
		ColorPanel.Visibility = Visibility.Collapsed;
		PalettePanel.Visibility = Visibility.Collapsed;
		DescPanel.Visibility = Visibility.Visible;
	}
	string _colorName = "";
	private async void DescGenerateBtn_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(Global.Settings.ApiKey) || string.IsNullOrWhiteSpace(Global.Settings.ApiKey))
		{
			MessageBox.Show(Properties.Resources.ProvideAPIKey, Properties.Resources.AIGenerateColor, MessageBoxButton.OK, MessageBoxImage.Information);
			return;
		}

		if (string.IsNullOrEmpty(DescPromptTxt.Text) || string.IsNullOrWhiteSpace(DescPromptTxt.Text))
		{
			MessageBox.Show(Properties.Resources.ProvideAPromptMsg, Properties.Resources.AIGenerateColor, MessageBoxButton.OK, MessageBoxImage.Information);
			return;
		}
		Global.SynethiaConfig.ActionsInfo[6].UsageCount++; // Increment the usage counter
		try
		{
			var openAiService = new OpenAIService(new OpenAiOptions()
			{
				ApiKey = Global.Settings.ApiKey ?? ""
			});
			var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
			{
				Messages =
				[
					ChatMessage.FromSystem($"GOAL: You are a color name generator assistant. The user gives you a prompt to generate a SINGLE name for a specified color. RESPONDE FORMAT: Only the color name. LANGUAGE: {CultureInfo.CurrentUICulture.Name}"),
					ChatMessage.FromUser("Generate the name of the color from this prompt: " + DescPromptTxt.Text)
				],
				Model = Global.Settings.Model ?? Models.Gpt_3_5_Turbo,
			});

			if (completionResult.Successful)
			{
				_colorName = completionResult.Choices.First().Message.Content ?? "";
				NameTxt.Text = $"{Properties.Resources.Name}: {_colorName}";
			}
		}
		catch { }
	}

	private void NameTxt_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		Clipboard.SetDataObject(_colorName);
	}
}
