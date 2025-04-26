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
using Synethia;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for ChromaticWheelPage.xaml
/// </summary>
public partial class ChromaticWheelPage : Page
{
	bool code = !Global.Settings.UseSynethia; // checks if the code as already been implemented
	ColorInfo ColorInfo { get; set; } = null!;
	readonly DetailsControl DetailsControl = new(new(new(0, 0, 0)));

	public ChromaticWheelPage()
	{
		InitializeComponent();
		InitUI();

		Loaded += (o, e) => SynethiaManager.InjectSynethiaCode(this, Global.SynethiaConfig.PagesInfo, 1, ref code); // injects the code in the page
	}

	private void InitUI()
	{
		TitleTxt.Text = $"{Properties.Resources.Picker} > {Properties.Resources.ChromaticWheel}";
		DetailsWrap.Children.Add(DetailsControl);
		CircleBtn_Click(this, null);
	}

	private void CircleBtn_Click(object sender, RoutedEventArgs? e)
	{
		UnCheckAllButtons();
		CheckButton(CircleBtn);

		BitmapImage img = new();
		img.BeginInit();
		img.UriSource = new Uri("pack://application:,,,/ColorPicker;component/Images/ColorWheel.png");
		img.EndInit();

		WheelImg.Source = img; // Set the image source
	}

	internal void DiscBtn_Click(object sender, RoutedEventArgs e)
	{
		UnCheckAllButtons();
		CheckButton(DiscBtn);
		BitmapImage img = new();
		img.BeginInit();
		img.UriSource = new Uri("pack://application:,,,/ColorPicker;component/Images/ColorDisc2.png");
		img.EndInit();

		WheelImg.Source = img; // Set the image source
		Global.SynethiaConfig.ActionsInfo[1].UsageCount++; // Increment the usage counter
	}

	private void ShadesBtn_Click(object sender, RoutedEventArgs e)
	{
		UnCheckAllButtons();
		CheckButton(ShadesBtn);
		BitmapImage img = new();
		img.BeginInit();
		img.UriSource = new Uri("pack://application:,,,/ColorPicker;component/Images/ColorDisc.png");
		img.EndInit();

		WheelImg.Source = img; // Set the image source
	}

	private void WheelImg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		System.Drawing.Bitmap bitmap = new(1, 1);
		System.Drawing.Graphics GFX = System.Drawing.Graphics.FromImage(bitmap);
		GFX.CopyFromScreen(System.Windows.Forms.Cursor.Position, new System.Drawing.Point(0, 0), bitmap.Size);
		var pixel = bitmap.GetPixel(0, 0);
		LoadDetails(new(new(pixel.R, pixel.G, pixel.B)));
		PreviewBorder.Visibility = Visibility.Visible;
	}

	private void LoadDetails(ColorInfo colorInfo)
	{
		ColorBorder.Background = new SolidColorBrush { Color = Color.FromRgb(colorInfo.RGB.R, colorInfo.RGB.G, colorInfo.RGB.B) };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = Color.FromRgb(colorInfo.RGB.R, colorInfo.RGB.G, colorInfo.RGB.B) };

		ColorInfo = colorInfo;
		DetailsControl.SetColorInfo(ColorInfo);
	}
	private void UnCheckAllButtons()
	{
		CircleBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		DiscBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		ShadesBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
	}

	internal Button CheckedButton = null!;
	internal void CheckButton(Button button) { button.Background = Global.GetColorFromResource("LightAccentColor"); CheckedButton = button; }

	public static event EventHandler<PageEventArgs> GoClick;

	private void PreviewBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Global.PalettePage.InitFromColor(ColorInfo);
		GoClick?.Invoke(this, new(AppPages.ColorPalette));
	}
}
