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
using ColorHelper;
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

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for ConverterPage.xaml
/// </summary>
public partial class ConverterPage : Page
{
	bool code = !Global.Settings.UseSynethia; // checks if the code as already been implemented
	internal ColorInfo ColorInfo { get; set; } = null!;

	internal DetailsControl DetailsControl = new(new(new(0, 0, 0))); // Details control for the converter page
	public ConverterPage()
	{
		InitializeComponent();
		InitUI();

		Loaded += (o, e) => SynethiaManager.InjectSynethiaCode(this, Global.SynethiaConfig.PagesInfo, 2, ref code); // injects the code in the page
	}

	private void InitUI()
	{
		TitleTxt.Text = $"{Properties.Resources.ColorTools} > {Properties.Resources.Converter}";
		DetailsWrap.Children.Add(DetailsControl);
		try
		{
			(int r, int g, int b) = Global.GenerateRandomColor();
			Txt1.Text = r.ToString();
			Txt2.Text = g.ToString();
			Txt3.Text = b.ToString();

			ColorInfo = new(new((byte)r, (byte)g, (byte)b));
		}
		catch { }
		RgbBtn_Click(Global.Settings.DefaultColorType switch
		{
			ColorTypes.HEX => HexBtn,
			ColorTypes.HSV => HsvBtn,
			ColorTypes.HSL => HslBtn,
			ColorTypes.CMYK => CmykBtn,
			ColorTypes.XYZ => XyzBtn,
			ColorTypes.YIQ => YiqBtn,
			ColorTypes.YUV => YuvBtn,
			ColorTypes.DEC => DecBtn,
			_ => RgbBtn
		}, null);

		LoadBookmarkMenu();
	}

	internal void LoadBookmarkMenu()
	{
		CollectionsPanel.Children.Clear();
		// Load bookmark menu
		for (int i = 0; i < Global.Bookmarks.ColorCollections.Count; i++)
		{
			bool isAddedAlready = Global.Bookmarks.ColorCollections[i].Colors.Contains(ColorInfo.HEX.Value) || Global.Bookmarks.ColorCollections[i].Colors.Contains(ColorInfo.HEX.Value[1..]);
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

	internal Button SelectedColorBtn { get; set; } = null!;
	private void UnCheckAllButtons()
	{
		RgbBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		HexBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		HsvBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		HslBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		CmykBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		XyzBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		YiqBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		YuvBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
		DecBtn.Background = new SolidColorBrush { Color = Colors.Transparent };
	}

	// Note: This event handler is used for all the choices
	internal void RgbBtn_Click(object sender, RoutedEventArgs? e)
	{
		var btn = (Button)sender;

		UnCheckAllButtons();
		CheckButton(btn);
		SelectedColorBtn = btn;
		LoadInputUI();
	}

	internal void CheckButton(Button button) => button.Background = Global.GetColorFromResource("LightAccentColor");

	private RGB ConvertToRgb()
	{
		if (SelectedColorBtn == RgbBtn) return new((byte)int.Parse(Txt1.Text),
											 (byte)int.Parse(Txt2.Text),
											 (byte)int.Parse(Txt3.Text));
		if (SelectedColorBtn == HexBtn) return ColorHelper.ColorConverter.HexToRgb(new(Txt5.Text));
		else if (SelectedColorBtn == HsvBtn) return ColorHelper.ColorConverter.HsvToRgb(new(int.Parse(Txt1.Text),
											 (byte)int.Parse(Txt2.Text),
											 (byte)int.Parse(Txt3.Text)));
		else if (SelectedColorBtn == HslBtn) return ColorHelper.ColorConverter.HslToRgb(new(int.Parse(Txt1.Text),
											 (byte)int.Parse(Txt2.Text),
											 (byte)int.Parse(Txt3.Text)));
		else return SelectedColorBtn == CmykBtn
			? ColorHelper.ColorConverter.CmykToRgb(new((byte)int.Parse(Txt1.Text),
											 (byte)int.Parse(Txt2.Text),
											 (byte)int.Parse(Txt3.Text),
											 (byte)int.Parse(Txt4.Text)))
			: SelectedColorBtn == XyzBtn
			? ColorHelper.ColorConverter.XyzToRgb(new(double.Parse(Txt1.Text),
											 double.Parse(Txt2.Text),
											 double.Parse(Txt3.Text)))
			: SelectedColorBtn == YuvBtn
			? ColorHelper.ColorConverter.YuvToRgb(new(double.Parse(Txt1.Text),
											 double.Parse(Txt2.Text),
											 double.Parse(Txt3.Text)))
			: SelectedColorBtn == DecBtn
			? new DEC(int.Parse(Txt5.Text)).ToRgb()
			: ColorHelper.ColorConverter.YiqToRgb(new(double.Parse(Txt1.Text),
											 double.Parse(Txt2.Text),
											 double.Parse(Txt3.Text)));
	}

	internal void LoadDetails(bool setColor = false)
	{
		if (!setColor) ColorInfo = new ColorInfo(ConvertToRgb());

		ColorBorder.Background = new SolidColorBrush { Color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B) };
		ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B) };

		// Show a success message to the user
		ColorValidTxt.Text = Properties.Resources.ColorValid;
		ColorValidIconTxt.Text = "\uF299";
		ColorValidIconTxt.Foreground = Global.GetColorFromResource("Green");

		LoadBookmarkMenu();
		DetailsControl.SetColorInfo(ColorInfo);

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

	private void HideAllInput()
	{
		DisplayText1.Visibility = Visibility.Collapsed;
		DisplayText2.Visibility = Visibility.Collapsed;
		DisplayText3.Visibility = Visibility.Collapsed;
		DisplayText4.Visibility = Visibility.Collapsed;
		DisplayText5.Visibility = Visibility.Collapsed; // Special textbox for hex

		// Clear text to avoid errors
		Txt1.Text = "";
		Txt2.Text = "";
		Txt3.Text = "";
		Txt4.Text = "";
		Txt5.Text = ""; // Special textbox for hex

		B1.Visibility = Visibility.Collapsed;
		B2.Visibility = Visibility.Collapsed;
		B3.Visibility = Visibility.Collapsed;
		B4.Visibility = Visibility.Collapsed;
		B5.Visibility = Visibility.Collapsed; // Special textbox for hex
	}

	private void LoadInputUI()
	{
		HideAllInput();
		if (SelectedColorBtn != HexBtn && SelectedColorBtn != DecBtn)
		{
			DisplayText1.Visibility = Visibility.Visible;
			DisplayText2.Visibility = Visibility.Visible;
			DisplayText3.Visibility = Visibility.Visible;
			DisplayText4.Visibility = SelectedColorBtn == CmykBtn ? Visibility.Visible : Visibility.Collapsed;

			B1.Visibility = Visibility.Visible;
			B2.Visibility = Visibility.Visible;
			B3.Visibility = Visibility.Visible;
			B4.Visibility = SelectedColorBtn == CmykBtn ? Visibility.Visible : Visibility.Collapsed;
		}

		if (SelectedColorBtn == RgbBtn)
		{
			DisplayText1.Text = "R";
			DisplayText2.Text = "G";
			DisplayText3.Text = "B";

			Txt1.Text = ColorInfo.RGB.R.ToString();
			Txt2.Text = ColorInfo.RGB.G.ToString();
			Txt3.Text = ColorInfo.RGB.B.ToString();
			Global.SynethiaConfig.ActionsInfo[2].UsageCount++; // Increment the usage counter

		}
		else if (SelectedColorBtn == HexBtn)
		{
			DisplayText5.Visibility = Visibility.Visible;

			DisplayText5.Text = Properties.Resources.HEX;
			B5.Visibility = Visibility.Visible;

			Txt5.Text = ColorInfo.HEX.Value;
		}
		else if (SelectedColorBtn == HsvBtn)
		{
			DisplayText1.Text = "H";
			DisplayText2.Text = "S";
			DisplayText3.Text = "V";

			Txt1.Text = ColorInfo.HSV.H.ToString();
			Txt2.Text = ColorInfo.HSV.S.ToString();
			Txt3.Text = ColorInfo.HSV.V.ToString();
		}
		else if (SelectedColorBtn == HslBtn)
		{
			DisplayText1.Text = "H";
			DisplayText2.Text = "S";
			DisplayText3.Text = "L";

			Txt1.Text = ColorInfo.HSL.H.ToString();
			Txt2.Text = ColorInfo.HSL.S.ToString();
			Txt3.Text = ColorInfo.HSL.L.ToString();
		}
		else if (SelectedColorBtn == CmykBtn)
		{
			DisplayText1.Text = "C";
			DisplayText2.Text = "M";
			DisplayText3.Text = "Y";
			DisplayText4.Text = "K";

			Txt1.Text = ColorInfo.CMYK.C.ToString();
			Txt2.Text = ColorInfo.CMYK.M.ToString();
			Txt3.Text = ColorInfo.CMYK.Y.ToString();
			Txt4.Text = ColorInfo.CMYK.K.ToString();
		}
		else if (SelectedColorBtn == XyzBtn)
		{
			DisplayText1.Text = "X";
			DisplayText2.Text = "Y";
			DisplayText3.Text = "Z";

			Txt1.Text = ColorInfo.XYZ.X.ToString();
			Txt2.Text = ColorInfo.XYZ.Y.ToString();
			Txt3.Text = ColorInfo.XYZ.Z.ToString();
		}
		else if (SelectedColorBtn == YiqBtn)
		{
			DisplayText1.Text = "Y";
			DisplayText2.Text = "I";
			DisplayText3.Text = "Q";

			Txt1.Text = ColorInfo.YIQ.Y.ToString();
			Txt2.Text = ColorInfo.YIQ.I.ToString();
			Txt3.Text = ColorInfo.YIQ.Q.ToString();
		}
		else if (SelectedColorBtn == YuvBtn)
		{
			DisplayText1.Text = "Y";
			DisplayText2.Text = "U";
			DisplayText3.Text = "V";

			Txt1.Text = ColorInfo.YUV.Y.ToString();
			Txt2.Text = ColorInfo.YUV.U.ToString();
			Txt3.Text = ColorInfo.YUV.V.ToString();
		}
		else if (SelectedColorBtn == DecBtn)
		{
			DisplayText5.Visibility = Visibility.Visible;

			DisplayText5.Text = Properties.Resources.DEC;
			B5.Visibility = Visibility.Visible;

			Txt5.Text = ColorInfo.DEC.Value.ToString();
		}
	}

	private void Txt1_TextChanged(object sender, TextChangedEventArgs e)
	{
		try
		{
			LoadDetails();
		}
		catch
		{
			// Show an error message to the user
			ColorValidTxt.Text = Properties.Resources.InvalidColor;
			ColorValidIconTxt.Text = "\uF36E";
			ColorValidIconTxt.Foreground = Global.GetColorFromResource("Red");
		}
	}

	private void Txt1_CanExecute(object sender, CanExecuteRoutedEventArgs e)
	{
		if (e.Command == ApplicationCommands.Paste)
		{
			e.CanExecute = true;
			e.Handled = true;
		}
	}

	private void Txt1_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		try
		{
			if (e.Command == ApplicationCommands.Paste)
			{
				string text = Clipboard.GetText()
					.Replace("(", "")
					.Replace(")", "")
					.Replace(" ", "");
				if (SelectedColorBtn == HsvBtn || SelectedColorBtn == HslBtn || SelectedColorBtn == CmykBtn)
				{
					var split = text.Split(",");
					Txt1.Text = split[0];
					Txt2.Text = split[1];
					Txt3.Text = split[2];
					Txt4.Text = split.Length > 3 ? split[3] : "";
				}
				else if (SelectedColorBtn == HexBtn || SelectedColorBtn == DecBtn)
				{
					Txt5.Text = text;
				}
				else
				{
					var split = text.Split(new string[] { ";", Global.Settings.RgbSeparator ?? ";" }, StringSplitOptions.None);
					Txt1.Text = split[0];
					Txt2.Text = split[1];
					Txt3.Text = split[2];
				}

				e.Handled = true;
			}
		}
		catch { }
	}

	private void BookmarkBtn_Click(object sender, RoutedEventArgs e)
	{
		CollectionsPopup.IsOpen = true;
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
}
