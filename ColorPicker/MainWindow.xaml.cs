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
using PeyrSharp.Env;
using Synethia;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ColorPicker;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		InitUI();
		GC.Collect();
	}

	DoubleAnimation expandAnimation = new()
	{
		From = 0,
		To = 180,
		Duration = new Duration(TimeSpan.FromSeconds(0.2)),
	};

	DoubleAnimation collapseAnimation = new()
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
			ActionsScrollViewer.Height = ActualHeight - SideBarTop.ActualHeight - GridRow1.ActualHeight - 60;
		};
		Closed += (o, e) =>
		{
			if (!Global.Settings.UseSynethia) Global.SynethiaConfig = Global.Default;
			SynethiaManager.Save(Global.SynethiaConfig, Global.SynethiaPath);
			XmlSerializerManager.SaveToXml(Global.Bookmarks, $@"{FileSys.AppDataPath}\Léo Corporation\ColorPicker Max\Bookmarks.xml");
		};

		WindowState = Global.Settings.IsMaximized ? WindowState.Maximized : WindowState.Normal;

		PageCard.OnCardClick += PageCard_OnCardClick;
		ActionCard.OnCardClick += PageCard_OnCardClick;
		ColorItem.GoClick += PageCard_OnCardClick;
		PaletteItem.GoClick += PageCard_OnCardClick;
		GradientItem.GoClick += PageCard_OnCardClick;
		ChromaticWheelPage.GoClick += PageCard_OnCardClick;
		SelectorPage.GoClick += PageCard_OnCardClick;
		ConverterPage.GoClick += PageCard_OnCardClick;
		TextItem.GoClick += PageCard_OnCardClick;

		HelloTxt.Text = Global.HiSentence; // Show greeting message to the user

		UnCheckAllButton();
		// Show the appropriate page
		switch (Global.Settings.DefaultPage)
		{
			case AppPages.Home:
				CheckButton(HomePageBtn, true);
				break;
			case AppPages.Bookmarks:
				CheckButton(BookmarksPageBtn, true);
				break;
			case AppPages.Selector:
				Global.SynethiaConfig.PagesInfo[0].EnterUnixTime = Sys.UnixTime;
				CheckButton(SelectorPageBtn);
				break;
			case AppPages.ColorWheel:
				Global.SynethiaConfig.PagesInfo[1].EnterUnixTime = Sys.UnixTime;
				CheckButton(ChromaticPageBtn);
				break;
			case AppPages.Converter:
				Global.SynethiaConfig.PagesInfo[2].EnterUnixTime = Sys.UnixTime;
				CheckButton(ConverterPageBtn);
				break;
			case AppPages.TextTool:
				Global.SynethiaConfig.PagesInfo[3].EnterUnixTime = Sys.UnixTime;
				CheckButton(TextPageBtn);
				break;
			case AppPages.ColorPalette:
				Global.SynethiaConfig.PagesInfo[4].EnterUnixTime = Sys.UnixTime;
				CheckButton(PalettePageBtn);
				break;
			case AppPages.ColorGradient:
				Global.SynethiaConfig.PagesInfo[5].EnterUnixTime = Sys.UnixTime;
				CheckButton(GradientPageBtn);
				break;
			case AppPages.AIGeneration:
				Global.SynethiaConfig.PagesInfo[6].EnterUnixTime = Sys.UnixTime;
				CheckButton(AiCreationPageBtn);
				break;
			case AppPages.Harmonies:
				Global.SynethiaConfig.PagesInfo[7].EnterUnixTime = Sys.UnixTime;
				CheckButton(HarmoniesPageBtn);
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
			AppPages.Harmonies => Global.HarmoniesPage,
			_ => Global.HomePage
		});
	}

	private void PageCard_OnCardClick(object? sender, PageEventArgs e)
	{
		switch (e.AppPage)
		{
			case AppPages.Selector:
				UnCheckAllButton();
				CheckButton(SelectorPageBtn);

				PageDisplayer.Navigate(Global.SelectorPage);
				Global.SynethiaConfig.PagesInfo[0].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ColorWheel:
				UnCheckAllButton();
				CheckButton(ChromaticPageBtn);

				PageDisplayer.Navigate(Global.ChromaticWheelPage);
				Global.SynethiaConfig.PagesInfo[1].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.Converter:
				UnCheckAllButton();
				CheckButton(ConverterPageBtn);

				PageDisplayer.Navigate(Global.ConverterPage);
				Global.SynethiaConfig.PagesInfo[2].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.TextTool:
				UnCheckAllButton();
				CheckButton(TextPageBtn);

				PageDisplayer.Navigate(Global.TextPage);
				Global.SynethiaConfig.PagesInfo[3].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ColorPalette:
				UnCheckAllButton();
				CheckButton(PalettePageBtn);

				PageDisplayer.Navigate(Global.PalettePage);
				Global.SynethiaConfig.PagesInfo[4].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ColorGradient:
				UnCheckAllButton();
				CheckButton(GradientPageBtn);

				PageDisplayer.Navigate(Global.GradientPage);
				Global.SynethiaConfig.PagesInfo[5].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.AIGeneration:
				UnCheckAllButton();
				CheckButton(AiCreationPageBtn);

				PageDisplayer.Navigate(Global.AiGenPage);
				Global.SynethiaConfig.PagesInfo[6].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.Harmonies:
				UnCheckAllButton();
				CheckButton(HarmoniesPageBtn);

				PageDisplayer.Navigate(Global.HarmoniesPage);
				Global.SynethiaConfig.PagesInfo[7].EnterUnixTime = Sys.UnixTime;
				break;
			default:
				break;
		}
	}

	private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
	{
		WindowState = WindowState.Minimized; // Minimize the window
	}

	private void MaximizeRestoreBtn_Click(object sender, RoutedEventArgs e)
	{
		WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

		HandleWindowStateChanged();
	}

	private void CloseBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		Application.Current.Shutdown(); // Close the application
	}

	private void PinBtn_Click(object sender, RoutedEventArgs e)
	{
		Topmost = !Topmost; // Toggle
		PinBtn.Content = Topmost ? "\uF604" : "\uF602"; // Set text
	}

	private void HandleWindowStateChanged()
	{
		MaximizeRestoreBtn.Content = WindowState == WindowState.Maximized
			? "\uF670" // Restore icon
			: "\uFA40"; // Maximize icon
		MaximizeRestoreBtn.FontSize = WindowState == WindowState.Maximized
			? 18
			: 14;

		DefineMaximumSize();

		WindowBorder.Margin = WindowState == WindowState.Maximized ? new(10, 10, 0, 0) : new(10); // Set
		WindowBorder.CornerRadius = WindowState == WindowState.Maximized ? new(0) : new(5); // Set

		// Update settings information
		Global.Settings.IsMaximized = WindowState == WindowState.Maximized;
		XmlSerializerManager.SaveToXml(Global.Settings, Global.SettingsPath);
	}

	private void DefineMaximumSize()
	{
		System.Windows.Forms.Screen currentScreen = System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(this).Handle); // The current screen

		float dpiX, dpiY;
		double scaling = 100; // Default scaling = 100%

		using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
		{
			dpiX = graphics.DpiX; // Get the DPI
			dpiY = graphics.DpiY; // Get the DPI

			scaling = dpiX switch
			{
				96 => 100, // Get the %
				120 => 125, // Get the %
				144 => 150, // Get the %
				168 => 175, // Get the %
				192 => 200, // Get the % 
				_ => 100
			};
		}

		double factor = scaling / 100d; // Calculate factor

		MaxHeight = currentScreen.WorkingArea.Height / factor + 5; // Set max size
		MaxWidth = currentScreen.WorkingArea.Width / factor + 5; // Set max size
	}

	private void HomePageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		UnCheckAllButton();
		CheckButton(HomePageBtn, true);

		PageDisplayer.Navigate(Global.HomePage);
	}

	private void BookmarksPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		UnCheckAllButton();
		CheckButton(BookmarksPageBtn, true);

		PageDisplayer.Navigate(Global.BookmarksPage);
	}

	private void SettingsPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		UnCheckAllButton();
		CheckButton(SettingsPageBtn, true);

		PageDisplayer.Navigate(Global.SettingsPage);
	}

	private void PickerBtn_Click(object sender, RoutedEventArgs e)
	{
		bool expanded = PickerPanel.Visibility == Visibility.Visible;
		PickerPanel.Visibility = expanded ? Visibility.Collapsed : Visibility.Visible; // Show/hide the picker panel

		Storyboard storyboard = new(); // Create a storyboard

		storyboard.Children.Add(expanded ? collapseAnimation : expandAnimation);
		Storyboard.SetTargetName(expanded ? collapseAnimation : expandAnimation, "PickerRotator");
		Storyboard.SetTargetProperty(expanded ? collapseAnimation : expandAnimation, new(RotateTransform.AngleProperty));

		storyboard.Begin(this); // Animate the picker panel
	}

	private void SelectorPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		UnCheckAllButton();
		CheckButton(SelectorPageBtn);

		PageDisplayer.Navigate(Global.SelectorPage);
		Global.SynethiaConfig.PagesInfo[0].EnterUnixTime = Sys.UnixTime;
	}

	private void ChromaticPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		UnCheckAllButton();
		CheckButton(ChromaticPageBtn);

		PageDisplayer.Navigate(Global.ChromaticWheelPage);
		Global.SynethiaConfig.PagesInfo[1].EnterUnixTime = Sys.UnixTime;
	}

	private void CheckButton(Button button, bool isSpecial = false)
	{
		if (isSpecial)
		{
			button.Background = new SolidColorBrush(Global.GetColorFromResource("Background1"));
		}
		else
		{
			button.Background = new SolidColorBrush(Global.GetColorFromResource("AccentColor"));
			button.Foreground = new SolidColorBrush(Global.GetColorFromResource("WindowButtonsHoverForeground1"));
		}
	}

	private void UnCheckAllButton()
	{
		// Background
		HomePageBtn.Background = new SolidColorBrush(Colors.Transparent);
		BookmarksPageBtn.Background = new SolidColorBrush(Colors.Transparent);
		SettingsPageBtn.Background = new SolidColorBrush(Colors.Transparent);

		SelectorPageBtn.Background = new SolidColorBrush(Colors.Transparent);
		ChromaticPageBtn.Background = new SolidColorBrush(Colors.Transparent);
		ConverterPageBtn.Background = new SolidColorBrush(Colors.Transparent);
		TextPageBtn.Background = new SolidColorBrush(Colors.Transparent);
		PalettePageBtn.Background = new SolidColorBrush(Colors.Transparent);
		GradientPageBtn.Background = new SolidColorBrush(Colors.Transparent);
		AiCreationPageBtn.Background = new SolidColorBrush(Colors.Transparent);
		HarmoniesPageBtn.Background = new SolidColorBrush(Colors.Transparent);

		SelectorPageBtn.Foreground = new SolidColorBrush(Global.GetColorFromResource("AccentColor"));
		ChromaticPageBtn.Foreground = new SolidColorBrush(Global.GetColorFromResource("AccentColor"));
		ConverterPageBtn.Foreground = new SolidColorBrush(Global.GetColorFromResource("AccentColor"));
		TextPageBtn.Foreground = new SolidColorBrush(Global.GetColorFromResource("AccentColor"));
		PalettePageBtn.Foreground = new SolidColorBrush(Global.GetColorFromResource("AccentColor"));
		GradientPageBtn.Foreground = new SolidColorBrush(Global.GetColorFromResource("AccentColor"));
		AiCreationPageBtn.Foreground = new SolidColorBrush(Global.GetColorFromResource("AccentColor"));
		HarmoniesPageBtn.Foreground = new SolidColorBrush(Global.GetColorFromResource("AccentColor"));
	}

	private void ColorToolsBtn_Click(object sender, RoutedEventArgs e)
	{
		bool expanded = ColorToolsPanel.Visibility == Visibility.Visible;
		ColorToolsPanel.Visibility = expanded ? Visibility.Collapsed : Visibility.Visible; // Show/hide the picker panel

		Storyboard storyboard = new(); // Create a storyboard

		storyboard.Children.Add(expanded ? collapseAnimation : expandAnimation);
		Storyboard.SetTargetName(expanded ? collapseAnimation : expandAnimation, "ColorToolsRotator");
		Storyboard.SetTargetProperty(expanded ? collapseAnimation : expandAnimation, new(RotateTransform.AngleProperty));

		storyboard.Begin(this); // Animate the picker panel
	}

	private void ConverterPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		UnCheckAllButton();
		CheckButton(ConverterPageBtn);

		PageDisplayer.Navigate(Global.ConverterPage);
		Global.SynethiaConfig.PagesInfo[2].EnterUnixTime = Sys.UnixTime;
	}

	private void TextPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		UnCheckAllButton();
		CheckButton(TextPageBtn);

		PageDisplayer.Navigate(Global.TextPage);
		Global.SynethiaConfig.PagesInfo[3].EnterUnixTime = Sys.UnixTime;
	}

	private void CreationBtn_Click(object sender, RoutedEventArgs e)
	{
		bool expanded = CreationPanel.Visibility == Visibility.Visible;
		CreationPanel.Visibility = expanded ? Visibility.Collapsed : Visibility.Visible; // Show/hide the picker panel

		Storyboard storyboard = new(); // Create a storyboard

		storyboard.Children.Add(expanded ? collapseAnimation : expandAnimation);
		Storyboard.SetTargetName(expanded ? collapseAnimation : expandAnimation, "CreationRotator");
		Storyboard.SetTargetProperty(expanded ? collapseAnimation : expandAnimation, new(RotateTransform.AngleProperty));

		storyboard.Begin(this); // Animate the picker panel
	}

	private void PalettePageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		UnCheckAllButton();
		CheckButton(PalettePageBtn);

		PageDisplayer.Navigate(Global.PalettePage);
		Global.SynethiaConfig.PagesInfo[4].EnterUnixTime = Sys.UnixTime;
	}

	private void GradientPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		UnCheckAllButton();
		CheckButton(GradientPageBtn);

		PageDisplayer.Navigate(Global.GradientPage);
		Global.SynethiaConfig.PagesInfo[5].EnterUnixTime = Sys.UnixTime;
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
			case HarmoniesPage:
				Global.SynethiaConfig.PagesInfo[7].LeaveUnixTime = Sys.UnixTime;
				Global.SynethiaConfig.PagesInfo[7].TotalTimeSpent += Global.SynethiaConfig.PagesInfo[7].LeaveUnixTime - Global.SynethiaConfig.PagesInfo[7].EnterUnixTime;
				break;
			default:
				break;
		}
	}

	private void AiCreationPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		UnCheckAllButton();
		CheckButton(AiCreationPageBtn);

		PageDisplayer.Navigate(Global.AiGenPage);
		Global.SynethiaConfig.PagesInfo[6].EnterUnixTime = Sys.UnixTime;
	}

	private void HarmoniesPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		UnCheckAllButton();
		CheckButton(HarmoniesPageBtn);

		PageDisplayer.Navigate(Global.HarmoniesPage);
		Global.SynethiaConfig.PagesInfo[7].EnterUnixTime = Sys.UnixTime;
	}
}
