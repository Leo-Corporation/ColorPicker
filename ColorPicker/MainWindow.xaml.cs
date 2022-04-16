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
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ColorPicker
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Button CheckedButton { get; set; }

		readonly ColorAnimation colorAnimation = new()
		{
			From = (Color)ColorConverter.ConvertFromString(App.Current.Resources["AccentColor"].ToString()),
			To = (Color)ColorConverter.ConvertFromString(App.Current.Resources["Background1"].ToString()),
			Duration = new(TimeSpan.FromSeconds(0.2d))
		};
		public MainWindow()
		{
			InitializeComponent();
			InitUI();
		}

		private void InitUI()
		{
			HelloTxt.Text = Global.GetHiSentence; // Set the "Hello" message

			CheckButton(PickerTabBtn); // Check the start page button
			PageContent.Content = Global.PickerPage; // Set startup page

			Closed += (o, e) => HistoryManager.Save();
			PageContent.Navigated += (o, e) => AnimatePage();
		}

		private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized; // Minimize window
		}

		private void CloseBtn_Click(object sender, RoutedEventArgs e)
		{
			HistoryManager.Save();
			Environment.Exit(0); // Quit
		}

		private void TabEnter(object sender, MouseEventArgs e)
		{
			Button button = (Button)sender; // Create button

			button.Foreground = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["WindowButtonsHoverForeground1"].ToString()) }; // Set the foreground
		}

		private void TabLeave(object sender, MouseEventArgs e)
		{
			Button button = (Button)sender; // Create button

			if (button != CheckedButton)
			{
				button.Foreground = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["Foreground1"].ToString()) }; // Set the foreground 
				button.Background.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation); // Play animation
			}
		}

		private void CheckButton(Button button)
		{
			button.Foreground = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["WindowButtonsHoverForeground1"].ToString()) }; // Set the foreground
			button.Background = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["AccentColor"].ToString()) }; // Set the background

			CheckedButton = button; // Set the "checked" button
		}

		private void ResetAllCheckStatus()
		{
			PickerTabBtn.Foreground = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["Foreground1"].ToString()) }; // Set the foreground
			PickerTabBtn.Background = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["Background1"].ToString()) }; // Set the background

			ConverterTabBtn.Foreground = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["Foreground1"].ToString()) }; // Set the foreground
			ConverterTabBtn.Background = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["Background1"].ToString()) }; // Set the background

			PaletteTabBtn.Foreground = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["Foreground1"].ToString()) }; // Set the foreground
			PaletteTabBtn.Background = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["Background1"].ToString()) }; // Set the background

			SettingsTabBtn.Foreground = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["Foreground1"].ToString()) }; // Set the foreground
			SettingsTabBtn.Background = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString(App.Current.Resources["Background1"].ToString()) }; // Set the background
		}

		private void PickerTabBtn_Click(object sender, RoutedEventArgs e)
		{
			ResetAllCheckStatus(); // Reset the background and foreground of all buttons
			CheckButton(PickerTabBtn); // Check the "Picker" button

			PageContent.Navigate(Global.PickerPage); // Navigate
			Global.PickerPage.CopyBtn.Content = Global.ColorTypesToCopyString(Global.Settings.FavoriteColorType.Value != Enums.ColorTypes.HEX 
				? Global.Settings.FavoriteColorType.Value 
				: Enums.ColorTypes.RGB); // Set the "Copy" button text
		}

		private void ConverterTabBtn_Click(object sender, RoutedEventArgs e)
		{
			ResetAllCheckStatus(); // Reset the background and foreground of all buttons
			CheckButton(ConverterTabBtn); // Check the "Converter" button

			PageContent.Navigate(Global.ConverterPage); // Navigate

		}

		private void SettingsTabBtn_Click(object sender, RoutedEventArgs e)
		{
			ResetAllCheckStatus(); // Reset the background and foreground of all buttons
			CheckButton(SettingsTabBtn); // Check the "Settings" button

			PageContent.Navigate(Global.SettingsPage); // Navigate
		}

		private void PaletteTabBtn_Click(object sender, RoutedEventArgs e)
		{
			ResetAllCheckStatus(); // Reset the background and foreground of all buttons
			CheckButton(PaletteTabBtn); // Check the "Picker" button

			PageContent.Navigate(Global.PalettePage); // Navigate
		}

		private void PinBtn_Click(object sender, RoutedEventArgs e)
		{
			Topmost = !Topmost; // Pin/Unpin
			PinBtn.Content = Topmost ? "\uF604" : "\uF602"; // Set text
			PinToolTip.Content = Topmost ? Properties.Resources.Unpin : Properties.Resources.Pin; // Set text
		}

		private void AnimatePage()
		{
			Storyboard storyboard = new();

			ThicknessAnimationUsingKeyFrames t = new();
			t.KeyFrames.Add(new SplineThicknessKeyFrame(new(0, 30, 0, 0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
			t.KeyFrames.Add(new SplineThicknessKeyFrame(new(0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.1))));
			t.AccelerationRatio = 0.5;

			storyboard.Children.Add(t);

			Storyboard.SetTargetName(t, PageContent.Name);
			Storyboard.SetTargetProperty(t, new(Frame.MarginProperty));
			storyboard.Begin(this);
		}
	}
}
