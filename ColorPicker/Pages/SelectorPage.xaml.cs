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
using ColorPicker.UserControls;
using ColorPicker.Windows;
using Gma.System.MouseKeyHook;
using Synethia;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for SelectorPage.xaml
/// </summary>
public partial class SelectorPage : Page
{
	bool code = !Global.Settings.UseSynethia; // checks if the code as already been implemented
	readonly DispatcherTimer timer = new();
	private IKeyboardMouseEvents keyboardEvents = Hook.GlobalEvents();
	internal MiniPicker miniPicker = new(); // MiniPicker window
	ColorInfo ColorInfo { get; set; } = null!;

	DetailsControl DetailsControl { get; set; } = new(new(new(0, 0, 0))); // Details control
	public SelectorPage()
	{
		InitializeComponent();
		InitUI();

		Loaded += (o, e) => SynethiaManager.InjectSynethiaCode(this, Global.SynethiaConfig.PagesInfo, 0, ref code); // injects the code in the page
	}

	private void InitUI()
	{
		TitleTxt.Text = $"{Properties.Resources.Picker} > {Properties.Resources.Selector}";
		DetailsWrap.Children.Add(DetailsControl); // Add details control to the page	
		(RedSlider.Value, GreenSlider.Value, BlueSlider.Value) = Global.GenerateRandomColor();
		LoadDetails();

		timer.Interval = new(0, 0, 0, 0, 1); // Interval
		timer.Tick += (o, e) =>
		{
			// Get the pixel from the screen
			System.Drawing.Bitmap bitmap = new(1, 1); // Create a bitmap where the color of the pixel is going to be copied
			System.Drawing.Graphics GFX = System.Drawing.Graphics.FromImage(bitmap);
			GFX.CopyFromScreen(System.Windows.Forms.Cursor.Position, new System.Drawing.Point(0, 0), bitmap.Size); // Get the color of the pixel at the mouse position
			var pixel = bitmap.GetPixel(0, 0); // Copy to the bitmap

			switch (ColorTypeComboBox.SelectedIndex)
			{
				case 1:
					var hsv = ColorHelper.ColorConverter.RgbToHsv(new(pixel.R, pixel.G, pixel.B));
					RedSlider.Value = hsv.H;
					GreenSlider.Value = hsv.S;
					BlueSlider.Value = hsv.V;
					break;
				case 2:
					var hsl = ColorHelper.ColorConverter.RgbToHsl(new(pixel.R, pixel.G, pixel.B));
					RedSlider.Value = hsl.H;
					GreenSlider.Value = hsl.S;
					BlueSlider.Value = hsl.L;
					break;
				case 3:
					var cmyk = ColorHelper.ColorConverter.RgbToCmyk(new(pixel.R, pixel.G, pixel.B));
					RedSlider.Value = cmyk.C;
					GreenSlider.Value = cmyk.M;
					BlueSlider.Value = cmyk.Y;
					KSlider.Value = cmyk.K;
					break;
				default:
					RedSlider.Value = pixel.R; // Set value
					GreenSlider.Value = pixel.G; // Set value
					BlueSlider.Value = pixel.B; // Set value
					break;
			}

			LoadDetails();

			// MiniPicker
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
			miniPicker.Left = System.Windows.Forms.Cursor.Position.X / factor; // Define position
			miniPicker.Top = System.Windows.Forms.Cursor.Position.Y / factor + 5; // Define position
		};

		// Register Keyboard Shortcuts
		try
		{
			keyboardEvents = Hook.GlobalEvents();
			Hook.GlobalEvents().OnCombination(new Dictionary<Combination, Action>
			{
				{ Combination.FromString(Global.Settings.CopyKeyboardShortcut), HandleCopyKeyboard },
				{ Combination.FromString(Global.Settings.SelectKeyboardShortcut), HandleSelectKeyboard }
			});
			ColorTypeComboBox.SelectedIndex = Global.Settings.DefaultColorType switch
			{
				ColorTypes.HSV => 1,
				ColorTypes.HSL => 2,
				ColorTypes.CMYK => 3,
				_ => 0
			};
		}
		catch { }

		LoadBookmarkMenu();
	}

	internal void LoadBookmarkMenu()
	{
		CollectionsPanel.Children.Clear();
		// Load bookmark menu
		for (int i = 0; i < Global.Bookmarks.ColorCollections.Count; i++)
		{
			bool isAddedAlready = Global.Bookmarks.ColorCollections[i].Colors.Contains(ColorInfo.HEX.Value);
			Button button = new()
			{
				Content = isAddedAlready ? string.Format(Properties.Resources.RemoveFrom, Global.Bookmarks.ColorCollections[i].Name) : string.Format(Properties.Resources.AddTo, Global.Bookmarks.ColorCollections[i].Name),
				HorizontalAlignment = HorizontalAlignment.Stretch,
				HorizontalContentAlignment = HorizontalAlignment.Left,
				Background = new SolidColorBrush() { Color = Colors.Transparent },
				FontWeight = FontWeights.Bold,
				Style = (Style)FindResource("DefaultButton"),
				Foreground = Global.GetColorFromResource("Foreground1"),
			};
			int j = i; // Avoid index out of range issues
			button.Click += (o, e) =>
			{
				if (Global.Bookmarks.ColorCollections[j].Colors.Contains(ColorInfo.HEX.Value))
				{
					Global.Bookmarks.ColorCollections[j].Colors.Remove(ColorInfo.HEX.Value);
					button.Content = string.Format(Properties.Resources.AddTo, Global.Bookmarks.ColorCollections[j].Name);
				}
				else
				{
					Global.Bookmarks.ColorCollections[j].Colors.Add(ColorInfo.HEX.Value);
					button.Content = string.Format(Properties.Resources.RemoveFrom, Global.Bookmarks.ColorCollections[j].Name);
				}
			};

			CollectionsPanel.Children.Add(button);
		}
	}

	private void HandleSelectKeyboard()
	{
		if (!Global.Settings.UseKeyboardShortcuts) return;

		selecting = !selecting;
		UpdateSelectionState(selecting);
		Global.SynethiaConfig.ActionsInfo[0].UsageCount++; // Increment the usage counter
	}

	readonly List<string> RecentColors = [];
	private void HandleCopyKeyboard()
	{
		try
		{
			if (!Global.Settings.UseKeyboardShortcuts) return;

			Clipboard.SetText(Global.Settings.DefaultColorType switch
			{
				ColorTypes.HEX => $"#{ColorInfo.HEX.Value}",
				ColorTypes.HSL => $"{ColorInfo.HSL.H}, {ColorInfo.HSL.S}, {ColorInfo.HSL.L}",
				ColorTypes.HSV => $"{ColorInfo.HSV.H}, {ColorInfo.HSV.S}, {ColorInfo.HSV.V}",
				ColorTypes.CMYK => $"{ColorInfo.CMYK.C}, {ColorInfo.CMYK.M}, {ColorInfo.CMYK.Y}, {ColorInfo.CMYK.K}",
				ColorTypes.XYZ => $"{ColorInfo.XYZ.X}; {ColorInfo.XYZ.Y}; {ColorInfo.XYZ.Z}",
				ColorTypes.YIQ => $"{ColorInfo.YIQ.Y}; {ColorInfo.YIQ.I}; {ColorInfo.YIQ.Q}",
				ColorTypes.YUV => $"{ColorInfo.YUV.Y}; {ColorInfo.YUV.U}; {ColorInfo.YUV.V}",
				_ => $"{ColorInfo.RGB.R}{Global.Settings.RgbSeparator}{ColorInfo.RGB.G}{Global.Settings.RgbSeparator}{ColorInfo.RGB.B}"
			});

			if (RecentColors.Contains(ColorInfo.HEX.ToString())) return;
			RecentColors.Add(ColorInfo.HEX.ToString());

			Border border = new()
			{
				Height = 25,
				Width = 25,
				CornerRadius = new(15),
				Cursor = Cursors.Hand,
				Margin = new(2),
				Background = new SolidColorBrush { Color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B) }
			};

			border.MouseLeftButtonUp += (o, e) =>
			{
				var c = ((SolidColorBrush)border.Background).Color;
				ColorInfo = new(new(c.R, c.G, c.B));
				LoadSliders();
			};

			RecentColorsPanel.Children.Add(border);
		}
		catch { }
	}

	private static Color GetRgb(int h, int s, int v, bool isHsl = false)
	{
		if (isHsl)
		{
			var rgb = ColorHelper.ColorConverter.HslToRgb(new(h, (byte)s, (byte)v));
			return Color.FromRgb(rgb.R, rgb.G, rgb.B);
		}
		var c = ColorHelper.ColorConverter.HsvToRgb(new(h, (byte)s, (byte)v));
		return Color.FromRgb(c.R, c.G, c.B);
	}

	private static Color GetRgb(byte c, byte m, byte y, byte k)
	{
		var rgb = ColorHelper.ColorConverter.CmykToRgb(new(c, m, y, k));
		return Color.FromRgb(rgb.R, rgb.G, rgb.B);
	}

	private void RedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		Color color = ColorTypeComboBox.SelectedIndex switch
		{
			1 => GetRgb((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value),
			2 => GetRgb((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value, true),
			3 => GetRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value, (byte)KSlider.Value),
			_ => Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value)
		};

		ColorBorder.Background = new SolidColorBrush { Color = color };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = color };
		RedValueTxt.Text = RedSlider.Value.ToString(); // Set text
		LoadDetails();
	}

	private void GreenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		Color color = ColorTypeComboBox.SelectedIndex switch
		{
			1 => GetRgb((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value),
			2 => GetRgb((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value, true),
			3 => GetRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value, (byte)KSlider.Value),
			_ => Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value)
		};

		ColorBorder.Background = new SolidColorBrush { Color = color };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = color };
		GreenValueTxt.Text = GreenSlider.Value.ToString(); // Set text
		LoadDetails();
	}

	private void BlueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		Color color = ColorTypeComboBox.SelectedIndex switch
		{
			1 => GetRgb((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value),
			2 => GetRgb((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value, true),
			3 => GetRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value, (byte)KSlider.Value),
			_ => Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value)
		};

		ColorBorder.Background = new SolidColorBrush { Color = color };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = color };
		BlueValueTxt.Text = BlueSlider.Value.ToString(); // Set text
		LoadDetails();
	}

	bool selecting = false;
	internal void SelectBtn_Click(object sender, RoutedEventArgs e)
	{
		selecting = !selecting;
		UpdateSelectionState(selecting);
		Global.SynethiaConfig.ActionsInfo[0].UsageCount++; // Increment the usage counter
	}

	private void ColorBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		(RedSlider.Value, GreenSlider.Value, BlueSlider.Value) = Global.GenerateRandomColor();
		LoadDetails();
	}

	internal void LoadDetails()
	{
		// Load the details section
		ColorInfo = ColorTypeComboBox.SelectedIndex switch
		{
			1 => new ColorInfo(ColorHelper.ColorConverter.HsvToRgb(new((int)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value))),
			2 => new ColorInfo(ColorHelper.ColorConverter.HslToRgb(new((int)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value))),
			3 => new ColorInfo(ColorHelper.ColorConverter.CmykToRgb(new((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value, (byte)KSlider.Value)))
			{ CMYK = new((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value, (byte)KSlider.Value) },
			_ => new ColorInfo(new((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value))
		};

		DetailsControl.SetColorInfo(ColorInfo);
		LoadBookmarkMenu();

		// Load the bookmark icon
		if (!Global.Bookmarks.ColorBookmarks.Contains($"#{ColorInfo.HEX.Value}"))
		{
			BookmarkBtn.Content = "\uF1F6";
			BookmarkToolTip.Content = Properties.Resources.AddBookmark;
			AddRemoveBookmarkBtn.Content = Properties.Resources.AddBookmark;
			return;
		}
		BookmarkBtn.Content = "\uF1F8";
		BookmarkToolTip.Content = Properties.Resources.RemoveBookmark;
		AddRemoveBookmarkBtn.Content = Properties.Resources.RemoveBookmark;
	}

	private void BookmarkBtn_Click(object sender, RoutedEventArgs e)
	{
		CollectionsPopup.IsOpen = true;
	}

	private void UpdateSelectionState(bool selectionOn)
	{
		if (selectionOn)
		{
			timer.Start();
			miniPicker.timer.Start(); // Start
			miniPicker.Show();
		}
		else
		{
			timer.Stop();
			miniPicker.timer.Stop(); // Stop
			miniPicker.Hide();
		}
	}

	private void ColorTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		LoadSliders();
	}

	private void LoadSliders()
	{
		var current = ColorInfo;
		KSlider.Visibility = Visibility.Collapsed;
		KValueTxt.Visibility = Visibility.Collapsed;
		switch (ColorTypeComboBox.SelectedIndex)
		{
			case 1:
				RedSlider.Foreground = Global.GetColorFromResource("AccentColor");
				GreenSlider.Foreground = Global.GetColorFromResource("AccentColor");
				BlueSlider.Foreground = Global.GetColorFromResource("AccentColor");

				RedSlider.Maximum = 360;
				GreenSlider.Maximum = 100;
				BlueSlider.Maximum = 100;

				RedSlider.Value = current.HSV.H;
				GreenSlider.Value = current.HSV.S;
				BlueSlider.Value = current.HSV.V;
				break;
			case 2:
				RedSlider.Foreground = Global.GetColorFromResource("AccentColor");
				GreenSlider.Foreground = Global.GetColorFromResource("AccentColor");
				BlueSlider.Foreground = Global.GetColorFromResource("AccentColor");

				RedSlider.Maximum = 360;
				GreenSlider.Maximum = 100;
				BlueSlider.Maximum = 100;

				RedSlider.Value = current.HSL.H;
				GreenSlider.Value = current.HSL.S;
				BlueSlider.Value = current.HSL.L;
				break;
			case 3:
				RedSlider.Foreground = Global.GetColorFromResource("AccentColor");
				GreenSlider.Foreground = Global.GetColorFromResource("AccentColor");
				BlueSlider.Foreground = Global.GetColorFromResource("AccentColor");
				KSlider.Foreground = Global.GetColorFromResource("AccentColor");

				RedSlider.Maximum = 100;
				GreenSlider.Maximum = 100;
				BlueSlider.Maximum = 100;
				KSlider.Maximum = 100;

				RedSlider.Value = current.CMYK.C;
				GreenSlider.Value = current.CMYK.M;
				BlueSlider.Value = current.CMYK.Y;
				KSlider.Value = current.CMYK.K;

				KSlider.Visibility = Visibility.Visible;
				KValueTxt.Visibility = Visibility.Visible;
				break;

			default: // RGB
				RedSlider.Foreground = Global.GetColorFromResource("SliderRed");
				GreenSlider.Foreground = Global.GetColorFromResource("SliderGreen");
				BlueSlider.Foreground = Global.GetColorFromResource("SliderBlue");

				RedSlider.Maximum = 255;
				GreenSlider.Maximum = 255;
				BlueSlider.Maximum = 255;

				RedSlider.Value = current.RGB.R;
				GreenSlider.Value = current.RGB.G;
				BlueSlider.Value = current.RGB.B;
				break;
		}
	}
	public static event EventHandler<PageEventArgs> GoClick;

	private void PaletteBtn_Click(object sender, RoutedEventArgs e)
	{
		Global.PalettePage.InitFromColor(ColorInfo);
		GoClick?.Invoke(this, new(AppPages.ColorPalette));
	}


	private void AddRemoveBookmarkBtn_Click(object sender, RoutedEventArgs e)
	{
		if (Global.Bookmarks.ColorBookmarks.Contains($"#{ColorInfo.HEX.Value}"))
		{
			int i = Global.Bookmarks.ColorBookmarks.IndexOf($"#{ColorInfo.HEX.Value}");
			Global.Bookmarks.ColorBookmarks.RemoveAt(i);
			Global.Bookmarks.ColorBookmarksNotes.RemoveAt(i); // Add note
			BookmarkBtn.Content = "\uF1F6";
			AddRemoveBookmarkBtn.Content = Properties.Resources.AddBookmark;
			BookmarkToolTip.Content = Properties.Resources.AddBookmark;

			return;
		}
		Global.Bookmarks.ColorBookmarks.Add($"#{ColorInfo.HEX.Value}"); // Add to color bookmarks
		Global.Bookmarks.ColorBookmarksNotes.Add(""); // Add note
		BookmarkBtn.Content = "\uF1F8";
		AddRemoveBookmarkBtn.Content = Properties.Resources.RemoveBookmark;
		BookmarkToolTip.Content = Properties.Resources.RemoveBookmark;
	}

	private void KSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		Color color = ColorTypeComboBox.SelectedIndex switch
		{
			1 => GetRgb((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value),
			2 => GetRgb((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value, true),
			3 => GetRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value, (byte)KSlider.Value),
			_ => Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value)
		};

		ColorBorder.Background = new SolidColorBrush { Color = color };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = color };
		KValueTxt.Text = KSlider.Value.ToString(); // Set text
		LoadDetails();
	}
}
