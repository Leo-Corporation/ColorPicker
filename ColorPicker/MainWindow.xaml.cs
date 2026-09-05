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
using ColorPicker.Pages;
using ColorPicker.UserControls;
using MicaWPF.Lite.Controls;
using PeyrSharp.Env;
using Synethia;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ColorPicker;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MicaWindow
{
	/// <summary>Allows pages (e.g. HomePage) to reach back to the host window's Frame and other members.</summary>
	public static MainWindow? Current { get; private set; }

	public MainWindow(bool isSilent = false)
	{
		Current = this;
		InitializeComponent();
		if (isSilent) Hide();
		InitUI();
		GC.Collect();
	}

	readonly DoubleAnimation expandAnimation = new()
	{
		From = 0,
		To = 180,
		Duration = new Duration(TimeSpan.FromSeconds(0.2)),
	};

	readonly DoubleAnimation collapseAnimation = new()
	{
		From = 180,
		To = 0,
		Duration = new Duration(TimeSpan.FromSeconds(0.2)),
	};

	private void InitUI()
	{
#if PORTABLE
		VersionTxt.Text = Global.Version + " (Portable)";
#else
		VersionTxt.Text = Global.Version;
#endif

		StateChanged += (o, e) => HandleWindowStateChanged();
		Loaded += (o, e) => HandleWindowStateChanged();
		LocationChanged += (o, e) => HandleWindowStateChanged();
		SizeChanged += (o, e) =>
		{
			PageScroller.Height = (ActualHeight - (GridRow1.ActualHeight + 68) > 0) ? ActualHeight - (GridRow1.ActualHeight + 68) : 0; // Set the scroller height
		};
		Closed += (o, e) =>
		{
			if (!Global.Settings.UseSynethia) Global.SynethiaConfig = Global.Default;
			SynethiaManager.Save(Global.SynethiaConfig, Global.SynethiaPath);
			XmlSerializerManager.SaveToXml(Global.Bookmarks, $@"{FileSys.AppDataPath}\Léo Corporation\ColorPicker Max\Bookmarks.xml");
			LeavePage();
			Application.Current.Shutdown(); // Close the application
		};

		WindowState = Global.Settings.IsMaximized ? WindowState.Maximized : WindowState.Normal;

		PageCard.OnCardClick += PageCard_OnCardClick;
		ColorItem.GoClick += PageCard_OnCardClick;
		PaletteItem.GoClick += PageCard_OnCardClick;
		GradientItem.GoClick += PageCard_OnCardClick;
		ChromaticWheelPage.GoClick += PageCard_OnCardClick;
		SelectorPage.GoClick += PageCard_OnCardClick;
		ConverterPage.GoClick += PageCard_OnCardClick;
		TextItem.GoClick += PageCard_OnCardClick;
		CollectionItem.GoClick += PageCard_OnCardClick;

		HelloTxt.Text = Global.HiSentence; // Show greeting message to the user

		// Show the appropriate page
		switch (Global.Settings.DefaultPage)
		{
			case AppPages.Home:
				HomePageBtn.IsChecked = true;
				break;
			case AppPages.Bookmarks:
				BookmarksPageBtn.IsChecked = true;
				break;
			case AppPages.Selector:
				Global.SynethiaConfig.PagesInfo[0].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ColorWheel:
				Global.SynethiaConfig.PagesInfo[1].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.Converter:
				Global.SynethiaConfig.PagesInfo[2].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.TextTool:
				Global.SynethiaConfig.PagesInfo[3].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ColorPalette:
				Global.SynethiaConfig.PagesInfo[4].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ColorGradient:
				Global.SynethiaConfig.PagesInfo[5].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.AIGeneration:
				Global.SynethiaConfig.PagesInfo[6].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ImageExtractor:
				Global.SynethiaConfig.PagesInfo[8].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ContrastGrid:
				Global.SynethiaConfig.PagesInfo[9].EnterUnixTime = Sys.UnixTime;
				break;
			default:
				break;
		}


		PageDisplayer.Navigate(Global.Settings.DefaultPage switch
		{
			AppPages.Selector => Global.SelectorPage,
			AppPages.ColorWheel => Global.ChromaticWheelPage,
			AppPages.Converter => Global.ConverterPage,
			AppPages.TextTool => Global.TextPage,
			AppPages.ColorPalette => Global.PalettePage,
			AppPages.ColorGradient => Global.GradientPage,
			AppPages.AIGeneration => Global.AiGenPage,
			AppPages.ImageExtractor => Global.ImageExtractorPage,
			AppPages.ContrastGrid => Global.ContrastPage,
			_ => Global.HomePage
		});
		PinTooltip.Content = Topmost ? Properties.Resources.Unpin : Properties.Resources.Pin;
		NavSelectColorShortcutTxt.Text = Global.Settings.SelectKeyboardShortcut
			.Replace("LControlKey", "Ctrl")
			.Replace("LShiftKey", "Shift")
			.Replace("RShiftKey", "Shift")
			.Replace("RControlKey", "Ctrl");
		UpdateThemeToggleIcon();
	}

	/// <summary>Computes whether the effective (resolved) theme is currently dark.</summary>
	private static bool IsEffectiveThemeDark()
	{
		if (Global.Settings.Theme == Themes.System) return Global.IsSystemThemeDark();
		return Global.Settings.Theme == Themes.Dark;
	}

	/// <summary>Updates the nav-bar theme toggle icon to match the current effective theme:
	/// sun for light mode, moon for dark mode (same images used on the Settings page).</summary>
	private void UpdateThemeToggleIcon()
	{
		ThemeToggleIcon.Source = new BitmapImage(new Uri(IsEffectiveThemeDark() ? "/Images/moon.png" : "/Images/sun.png", UriKind.Relative));
	}

	private void ThemeToggleBtn_Click(object sender, RoutedEventArgs e)
	{
		Global.Settings.Theme = IsEffectiveThemeDark() ? Themes.Light : Themes.Dark;
		Global.ChangeTheme();
		UpdateThemeToggleIcon();
	}

	private void PageCard_OnCardClick(object? sender, PageEventArgs e)
	{
		switch (e.AppPage)
		{
			case AppPages.Selector:
				LeavePage();
				PageDisplayer.Navigate(Global.SelectorPage);
				Global.SynethiaConfig.PagesInfo[0].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ColorWheel:
				LeavePage();
				PageDisplayer.Navigate(Global.ChromaticWheelPage);
				Global.SynethiaConfig.PagesInfo[1].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.Converter:
				LeavePage();
				PageDisplayer.Navigate(Global.ConverterPage);
				Global.SynethiaConfig.PagesInfo[2].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.TextTool:
				LeavePage();
				PageDisplayer.Navigate(Global.TextPage);
				Global.SynethiaConfig.PagesInfo[3].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ColorPalette:
				LeavePage();
				PageDisplayer.Navigate(Global.PalettePage);
				Global.SynethiaConfig.PagesInfo[4].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ColorGradient:
				LeavePage();
				PageDisplayer.Navigate(Global.GradientPage);
				Global.SynethiaConfig.PagesInfo[5].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.AIGeneration:
				LeavePage();
				PageDisplayer.Navigate(Global.AiGenPage);
				Global.SynethiaConfig.PagesInfo[6].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ImageExtractor:
				LeavePage();
				PageDisplayer.Navigate(Global.ImageExtractorPage);
				Global.SynethiaConfig.PagesInfo[8].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ContrastGrid:
				LeavePage();
				PageDisplayer.Navigate(Global.ContrastPage);
				Global.SynethiaConfig.PagesInfo[9].EnterUnixTime = Sys.UnixTime;
				break;
			default:
				break;
		}
	}

	private void PinBtn_Click(object sender, RoutedEventArgs e)
	{
		Topmost = !Topmost; // Toggle
		PinBtn.Content = Topmost ? "\uF604" : "\uF602"; // Set text
		PinTooltip.Content = Topmost ? Properties.Resources.Unpin : Properties.Resources.Pin;
	}

	/// <summary>
	/// Public helper invoked by tool radio buttons that now live in HomePage (moved out of the sidebar).
	/// Navigates the PageDisplayer to the given tool page and updates Synethia usage timestamps.
	/// </summary>
	public void NavigateToTool(AppPages page, int synethiaIndex)
	{
		LeavePage();
		PageDisplayer.Navigate(page switch
		{
			AppPages.Selector => Global.SelectorPage,
			AppPages.ColorWheel => Global.ChromaticWheelPage,
			AppPages.Converter => Global.ConverterPage,
			AppPages.TextTool => Global.TextPage,
			AppPages.ColorPalette => Global.PalettePage,
			AppPages.ColorGradient => Global.GradientPage,
			AppPages.AIGeneration => Global.AiGenPage,
			AppPages.ImageExtractor => Global.ImageExtractorPage,
			AppPages.ContrastGrid => Global.ContrastPage,
			_ => Global.HomePage
		});
		Global.SynethiaConfig.PagesInfo[synethiaIndex].EnterUnixTime = Sys.UnixTime;
	}

	private void HandleWindowStateChanged()
	{
		// Update settings information
		Global.Settings.IsMaximized = WindowState == WindowState.Maximized;
		XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
	}


	private void HomePageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		HomePageBtn.IsChecked = true;

		PageDisplayer.Navigate(Global.HomePage);
	}

	private void BookmarksPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		BookmarksPageBtn.IsChecked = true;

		PageDisplayer.Navigate(Global.BookmarksPage);
	}

	private void SettingsPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		SettingsPageBtn.IsChecked = true;

		PageDisplayer.Navigate(Global.SettingsPage);
	}

	private void NavSelectColorBtn_Click(object sender, RoutedEventArgs e)
	{
		Global.HomePage?.Nav_OpenColorSelector();
	}

	private void NavContrastBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		HomePageBtn.IsChecked = true;
		PageDisplayer.Navigate(Global.HomePage);
		Global.HomePage?.Nav_OpenContrastPopup();
	}

	private void NavPaletteBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		HomePageBtn.IsChecked = true;
		PageDisplayer.Navigate(Global.HomePage);
		Global.HomePage?.Nav_OpenPalettePopup();
	}

	private void LeavePage()
	{
		if (!Global.Settings.UseSynethia) return;
		switch (PageDisplayer.Content)
		{
			case SelectorPage:
				Global.SynethiaConfig.PagesInfo[0].LeaveUnixTime = Sys.UnixTime;
				Global.SynethiaConfig.PagesInfo[0].TotalTimeSpent += Global.SynethiaConfig.PagesInfo[0].LeaveUnixTime - Global.SynethiaConfig.PagesInfo[0].EnterUnixTime;
				break;
			case ChromaticWheelPage:
				Global.SynethiaConfig.PagesInfo[1].LeaveUnixTime = Sys.UnixTime;
				Global.SynethiaConfig.PagesInfo[1].TotalTimeSpent += Global.SynethiaConfig.PagesInfo[1].LeaveUnixTime - Global.SynethiaConfig.PagesInfo[1].EnterUnixTime;
				break;
			case ConverterPage:
				Global.SynethiaConfig.PagesInfo[2].LeaveUnixTime = Sys.UnixTime;
				Global.SynethiaConfig.PagesInfo[2].TotalTimeSpent += Global.SynethiaConfig.PagesInfo[2].LeaveUnixTime - Global.SynethiaConfig.PagesInfo[2].EnterUnixTime;
				break;
			case TextPage:
				Global.SynethiaConfig.PagesInfo[3].LeaveUnixTime = Sys.UnixTime;
				Global.SynethiaConfig.PagesInfo[3].TotalTimeSpent += Global.SynethiaConfig.PagesInfo[3].LeaveUnixTime - Global.SynethiaConfig.PagesInfo[3].EnterUnixTime;
				break;
			case PalettePage:
				Global.SynethiaConfig.PagesInfo[4].LeaveUnixTime = Sys.UnixTime;
				Global.SynethiaConfig.PagesInfo[4].TotalTimeSpent += Global.SynethiaConfig.PagesInfo[4].LeaveUnixTime - Global.SynethiaConfig.PagesInfo[4].EnterUnixTime;
				break;
			case GradientPage:
				Global.SynethiaConfig.PagesInfo[5].LeaveUnixTime = Sys.UnixTime;
				Global.SynethiaConfig.PagesInfo[5].TotalTimeSpent += Global.SynethiaConfig.PagesInfo[5].LeaveUnixTime - Global.SynethiaConfig.PagesInfo[5].EnterUnixTime;
				break;
			case AiGenPage:
				Global.SynethiaConfig.PagesInfo[6].LeaveUnixTime = Sys.UnixTime;
				Global.SynethiaConfig.PagesInfo[6].TotalTimeSpent += Global.SynethiaConfig.PagesInfo[6].LeaveUnixTime - Global.SynethiaConfig.PagesInfo[6].EnterUnixTime;
				break;
			case ImageExtractorPage:
				Global.SynethiaConfig.PagesInfo[8].LeaveUnixTime = Sys.UnixTime;
				Global.SynethiaConfig.PagesInfo[8].TotalTimeSpent += Global.SynethiaConfig.PagesInfo[8].LeaveUnixTime - Global.SynethiaConfig.PagesInfo[8].EnterUnixTime;
				break;
			case ContrastPage:
				Global.SynethiaConfig.PagesInfo[9].LeaveUnixTime = Sys.UnixTime;
				Global.SynethiaConfig.PagesInfo[9].TotalTimeSpent += Global.SynethiaConfig.PagesInfo[9].LeaveUnixTime - Global.SynethiaConfig.PagesInfo[9].EnterUnixTime;
				break;
			default:
				break;
		}
	}

	private void ShowMenu_Click(object sender, RoutedEventArgs e)
	{
		Show();
	}

	private void QuitMenu_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		Application.Current.Shutdown(); // Close the application
	}

	private void HideMenu_Click(object sender, RoutedEventArgs e)
	{
		Hide();
	}
}
