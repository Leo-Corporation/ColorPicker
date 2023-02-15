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
		public SettingsPage()
		{
			InitializeComponent();
			InitUI();
		}

		private void InitUI()
		{
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

	}
}
