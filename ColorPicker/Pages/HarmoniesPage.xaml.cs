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
using ColorPicker.Windows;
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

namespace ColorPicker.Pages
{
    /// <summary>
    /// Interaction logic for HarmoniesPage.xaml
    /// </summary>
    public partial class HarmoniesPage : Page
    {
        public HarmoniesPage()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
			TitleTxt.Text = $"{Properties.Resources.Creation} > {Properties.Resources.Harmonies}";
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
		}

		internal void InitHarmonies()
		{
			ColorInfo = new ColorInfo(ConvertToRgb());
			var color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B);
			ColorBorder.Background = new SolidColorBrush { Color = color };
			ColorBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = color };

			// Complementary
			var complementary = Global.GetComplementaryColor(color);
			ComplementaryBorder.Background = new SolidColorBrush { Color = complementary };
			ComplementaryBorder.Effect = new DropShadowEffect() { BlurRadius = 15, ShadowDepth = 0, Color = complementary };
		}

		internal ColorInfo ColorInfo { get; set; }

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
			else if (SelectedColorBtn == CmykBtn) return ColorHelper.ColorConverter.CmykToRgb(new((byte)int.Parse(Txt1.Text),
												 (byte)int.Parse(Txt2.Text),
												 (byte)int.Parse(Txt3.Text),
												 (byte)int.Parse(Txt4.Text)));
			else if (SelectedColorBtn == XyzBtn) return ColorHelper.ColorConverter.XyzToRgb(new(double.Parse(Txt1.Text),
												 double.Parse(Txt2.Text),
												 double.Parse(Txt3.Text)));
			else if (SelectedColorBtn == YuvBtn) return ColorHelper.ColorConverter.YuvToRgb(new(double.Parse(Txt1.Text),
												 double.Parse(Txt2.Text),
												 double.Parse(Txt3.Text)));
			else if (SelectedColorBtn == DecBtn) return new DEC(int.Parse(Txt5.Text)).ToRgb();
			else return ColorHelper.ColorConverter.YiqToRgb(new(double.Parse(Txt1.Text),
												 double.Parse(Txt2.Text),
												 double.Parse(Txt3.Text)));
		}

		Button SelectedColorBtn { get; set; }
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

		private void CheckButton(Button button) => button.Background = new SolidColorBrush { Color = Global.GetColorFromResource("LightAccentColor") };

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
				InitHarmonies();
			}
			catch { }
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
						var split = text.Split(";");
						Txt1.Text = split[0];
						Txt2.Text = split[1];
						Txt3.Text = split[2];
					}

					e.Handled = true;
				}
			}
			catch { }
		}

		internal void ColorBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			try
			{
				(int r, int g, int b) = Global.GenerateRandomColor();
				ColorInfo = new(new((byte)r, (byte)g, (byte)b));
				Global.SynethiaConfig.ActionsInfo[4].UsageCount++; // Increment the usage counter
			}
			catch { }
			RgbBtn_Click(SelectedColorBtn, null);
		}

		private void ComplementaryBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			new ColorDetailsWindow(new()
			{
				Color = Color.FromRgb(ColorInfo.RGB.R, ColorInfo.RGB.G, ColorInfo.RGB.B)
			}).Show();
		}
	}
}
