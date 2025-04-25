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
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ColorPicker;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow(bool isSilent = false)
	{
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
				SelectorPageBtn.IsChecked = true;
				break;
			case AppPages.ColorWheel:
				Global.SynethiaConfig.PagesInfo[1].EnterUnixTime = Sys.UnixTime;
				ChromaticPageBtn.IsChecked = true;
				break;
			case AppPages.Converter:
				Global.SynethiaConfig.PagesInfo[2].EnterUnixTime = Sys.UnixTime;
				ConverterPageBtn.IsChecked = true;
				break;
			case AppPages.TextTool:
				Global.SynethiaConfig.PagesInfo[3].EnterUnixTime = Sys.UnixTime;
				TextPageBtn.IsChecked = true;
				break;
			case AppPages.ColorPalette:
				Global.SynethiaConfig.PagesInfo[4].EnterUnixTime = Sys.UnixTime;
				PalettePageBtn.IsChecked = true;
				break;
			case AppPages.ColorGradient:
				Global.SynethiaConfig.PagesInfo[5].EnterUnixTime = Sys.UnixTime;
				GradientPageBtn.IsChecked = true;
				break;
			case AppPages.AIGeneration:
				Global.SynethiaConfig.PagesInfo[6].EnterUnixTime = Sys.UnixTime;
				AiCreationPageBtn.IsChecked = true;
				break;
			case AppPages.ImageExtractor:
				Global.SynethiaConfig.PagesInfo[8].EnterUnixTime = Sys.UnixTime;
				ImageExtractorPageBtn.IsChecked = true;
				break;
			case AppPages.ContrastGrid:
				Global.SynethiaConfig.PagesInfo[9].EnterUnixTime = Sys.UnixTime;
				ContrastGridPageBtn.IsChecked = true;
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
	}

	private void PageCard_OnCardClick(object? sender, PageEventArgs e)
	{
		switch (e.AppPage)
		{
			case AppPages.Selector:
				SelectorPageBtn.IsChecked = true;

				PageDisplayer.Navigate(Global.SelectorPage);
				Global.SynethiaConfig.PagesInfo[0].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ColorWheel:
				ChromaticPageBtn.IsChecked = true;

				PageDisplayer.Navigate(Global.ChromaticWheelPage);
				Global.SynethiaConfig.PagesInfo[1].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.Converter:
				ConverterPageBtn.IsChecked = true;

				PageDisplayer.Navigate(Global.ConverterPage);
				Global.SynethiaConfig.PagesInfo[2].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.TextTool:
				TextPageBtn.IsChecked = true;

				PageDisplayer.Navigate(Global.TextPage);
				Global.SynethiaConfig.PagesInfo[3].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ColorPalette:
				PalettePageBtn.IsChecked = true;

				PageDisplayer.Navigate(Global.PalettePage);
				Global.SynethiaConfig.PagesInfo[4].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ColorGradient:
				GradientPageBtn.IsChecked = true;

				PageDisplayer.Navigate(Global.GradientPage);
				Global.SynethiaConfig.PagesInfo[5].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.AIGeneration:
				AiCreationPageBtn.IsChecked = true;

				PageDisplayer.Navigate(Global.AiGenPage);
				Global.SynethiaConfig.PagesInfo[6].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ImageExtractor:
				ImageExtractorPageBtn.IsChecked = true;

				PageDisplayer.Navigate(Global.ImageExtractorPage);
				Global.SynethiaConfig.PagesInfo[8].EnterUnixTime = Sys.UnixTime;
				break;
			case AppPages.ContrastGrid:
				ContrastGridPageBtn.IsChecked = true;

				PageDisplayer.Navigate(Global.ContrastPage);
				Global.SynethiaConfig.PagesInfo[9].EnterUnixTime = Sys.UnixTime;
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
		PinTooltip.Content = Topmost ? Properties.Resources.Unpin : Properties.Resources.Pin;
	}

	private void HandleWindowStateChanged()
	{
		MaximizeRestoreBtn.Content = WindowState == WindowState.Maximized
			? "\uF670" // Restore icon
			: "\uFA40"; // Maximize icon
		MaximizeRestoreBtn.FontSize = WindowState == WindowState.Maximized
			? 18
			: 14;
		MaximizeTooltip.Content = WindowState == WindowState.Maximized
			? Properties.Resources.Restore // Restore icon
			: Properties.Resources.Maximize; // Maximize icon
		DefineMaximumSize();

		WindowBorder.Margin = WindowState == WindowState.Maximized ? new(10, 10, 0, 0) : new(10); // Set
		WindowBorder.CornerRadius = WindowState == WindowState.Maximized ? new(0) : new(5); // Set
		MainWindowChrome.ResizeBorderThickness = WindowState == WindowState.Maximized ? new(0) : new(10); // Set

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
		SelectorPageBtn.IsChecked = true;

		PageDisplayer.Navigate(Global.SelectorPage);
		Global.SynethiaConfig.PagesInfo[0].EnterUnixTime = Sys.UnixTime;
	}

	private void ChromaticPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		ChromaticPageBtn.IsChecked = true;

		PageDisplayer.Navigate(Global.ChromaticWheelPage);
		Global.SynethiaConfig.PagesInfo[1].EnterUnixTime = Sys.UnixTime;
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
		ConverterPageBtn.IsChecked = true;

		PageDisplayer.Navigate(Global.ConverterPage);
		Global.SynethiaConfig.PagesInfo[2].EnterUnixTime = Sys.UnixTime;
	}

	private void TextPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		TextPageBtn.IsChecked = true;

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
		PalettePageBtn.IsChecked = true;

		PageDisplayer.Navigate(Global.PalettePage);
		Global.SynethiaConfig.PagesInfo[4].EnterUnixTime = Sys.UnixTime;
	}

	private void GradientPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		GradientPageBtn.IsChecked = true;

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

	private void AiCreationPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		AiCreationPageBtn.IsChecked = true;

		PageDisplayer.Navigate(Global.AiGenPage);
		Global.SynethiaConfig.PagesInfo[6].EnterUnixTime = Sys.UnixTime;
	}

	private void ImageExtractorPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		ImageExtractorPageBtn.IsChecked = true;

		PageDisplayer.Navigate(Global.ImageExtractorPage);
		Global.SynethiaConfig.PagesInfo[8].EnterUnixTime = Sys.UnixTime;
	}

	private void ContrastGridPageBtn_Click(object sender, RoutedEventArgs e)
	{
		LeavePage();
		ContrastGridPageBtn.IsChecked = true;

		PageDisplayer.Navigate(Global.ContrastPage);
		Global.SynethiaConfig.PagesInfo[9].EnterUnixTime = Sys.UnixTime;
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
