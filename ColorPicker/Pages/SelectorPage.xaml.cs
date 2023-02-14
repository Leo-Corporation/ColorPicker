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
using ColorPicker.Windows;
using Gma.System.MouseKeyHook;
using Synethia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for SelectorPage.xaml
/// </summary>
public partial class SelectorPage : Page
{
	bool code = false; // checks if the code as already been implemented
	readonly DispatcherTimer timer = new();
	private readonly IKeyboardMouseEvents keyboardEvents;
	internal MiniPicker miniPicker = new(); // MiniPicker window

	public SelectorPage()
	{
		InitializeComponent();
		keyboardEvents = Hook.GlobalEvents();


		InitUI();

		Loaded += (o, e) => SynethiaManager.InjectSynethiaCode(this, Global.SynethiaConfig.PagesInfo, 0, ref code); // injects the code in the page
	}

	private void InitUI()
	{
		TitleTxt.Text = $"{Properties.Resources.Picker} > {Properties.Resources.Selector}";
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

			RedSlider.Value = pixel.R; // Set value
			GreenSlider.Value = pixel.G; // Set value
			BlueSlider.Value = pixel.B; // Set value
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

		Hook.GlobalEvents().OnCombination(new Dictionary<Combination, Action>
		{
			{ Combination.FromString("Control+Shift+C"), HandleCopyKeyboard },
			{ Combination.FromString("Control+Shift+S"), HandleSelectKeyboard }
		});
	}

	private void HandleSelectKeyboard()
	{
		selecting = !selecting;
		UpdateSelectionState(selecting);
		Global.SynethiaConfig.ActionsInfo[0].UsageCount++; // Increment the usage counter
	}

	private void HandleCopyKeyboard()
	{
		Clipboard.SetText(RgbTxt.Text);
	}

	private void RedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		ColorBorder.Background = new SolidColorBrush { Color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value) };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value) };
		RedValueTxt.Text = RedSlider.Value.ToString(); // Set text
		LoadDetails();
	}

	private void GreenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		ColorBorder.Background = new SolidColorBrush { Color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value) };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value) };
		GreenValueTxt.Text = GreenSlider.Value.ToString(); // Set text
		LoadDetails();
	}

	private void BlueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		ColorBorder.Background = new SolidColorBrush { Color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value) };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value) };
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

	ColorInfo ColorInfo { get; set; }
	internal void LoadDetails()
	{
		// Load the details section
		ColorInfo = new ColorInfo(new((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value));
		RgbTxt.Text = $"{ColorInfo.RGB.R}; {ColorInfo.RGB.G}; {ColorInfo.RGB.B}";
		HexTxt.Text = $"#{ColorInfo.HEX.Value}";
		HsvTxt.Text = $"{ColorInfo.HSV.H}, {ColorInfo.HSV.S}, {ColorInfo.HSV.V}";
		HslTxt.Text = $"{ColorInfo.HSL.H}, {ColorInfo.HSL.S}, {ColorInfo.HSL.L}";
		CmykTxt.Text = $"{ColorInfo.CMYK.C}, {ColorInfo.CMYK.M}, {ColorInfo.CMYK.Y}, {ColorInfo.CMYK.K}";
		XyzTxt.Text = $"{ColorInfo.XYZ.X:0.00}..; {ColorInfo.XYZ.Y:0.00}..; {ColorInfo.XYZ.Z:0.00}..";
		YiqTxt.Text = $"{ColorInfo.YIQ.Y:0.00}..; {ColorInfo.YIQ.I:0.00}..; {ColorInfo.YIQ.Q:0.00}..";
		YuvTxt.Text = $"{ColorInfo.YUV.Y:0.00}..; {ColorInfo.YUV.U:0.00}..; {ColorInfo.YUV.V:0.00}..";

		// Load the bookmark icon
		if (!Global.Bookmarks.ColorBookmarks.Contains(HexTxt.Text))
		{
			BookmarkBtn.Content = "\uF1F6";
			return;
		}
		BookmarkBtn.Content = "\uF1F8";
	}

	private void CopyYiqBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText($"{ColorInfo.YIQ.Y}; {ColorInfo.YIQ.I}; {ColorInfo.YIQ.Q}");
	}

	private void CopyXyzBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText($"{ColorInfo.XYZ.X}; {ColorInfo.XYZ.Y}; {ColorInfo.XYZ.Z}");
	}

	private void CopyCmykBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText($"{ColorInfo.CMYK.C}, {ColorInfo.CMYK.M}, {ColorInfo.CMYK.Y}, {ColorInfo.CMYK.K}");
	}

	private void CopyYuvBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText($"{ColorInfo.YUV.Y}; {ColorInfo.YUV.U}; {ColorInfo.YUV.V}");
	}

	private void CopyHslBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(HslTxt.Text);
	}

	private void CopyHsvBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(HsvTxt.Text);
	}

	private void CopyHexBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(HexTxt.Text);
	}

	private void CopyRgbBtn_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(RgbTxt.Text);
	}

	private void BookmarkBtn_Click(object sender, RoutedEventArgs e)
	{
		if (Global.Bookmarks.ColorBookmarks.Contains(HexTxt.Text))
		{
			Global.Bookmarks.ColorBookmarks.Remove(HexTxt.Text);
			BookmarkBtn.Content = "\uF1F6";
			return;
		}
		Global.Bookmarks.ColorBookmarks.Add(HexTxt.Text); // Add to color bookmarks
		BookmarkBtn.Content = "\uF1F8";
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
}
