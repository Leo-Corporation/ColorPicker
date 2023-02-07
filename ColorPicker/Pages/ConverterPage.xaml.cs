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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for ConverterPage.xaml
/// </summary>
public partial class ConverterPage : Page
{
	public ConverterPage()
	{
		InitializeComponent();
		InitUI();
	}

	private void InitUI()
	{
		TitleTxt.Text = $"{Properties.Resources.ColorTools} > {Properties.Resources.Converter}";
		RgbBtn_Click(RgbBtn, null);
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
	}

	// Note: This event handler is used for all the choices
	private void RgbBtn_Click(object sender, RoutedEventArgs? e)
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
		DisplayText1.Text = "";
		DisplayText2.Text = "";
		DisplayText3.Text = "";
		DisplayText4.Text = "";
		DisplayText5.Text = ""; // Special textbox for hex

		B1.Visibility = Visibility.Collapsed;
		B2.Visibility = Visibility.Collapsed;
		B3.Visibility = Visibility.Collapsed;
		B4.Visibility = Visibility.Collapsed;
		B5.Visibility = Visibility.Collapsed; // Special textbox for hex
	}

	private void LoadInputUI()
	{
		HideAllInput();
		if (SelectedColorBtn != HexBtn)
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
		}
		else if (SelectedColorBtn == HexBtn)
		{
			DisplayText5.Visibility = Visibility.Visible;

			DisplayText5.Text = Properties.Resources.HEX;
			B5.Visibility = Visibility.Visible;
		}
		else if (SelectedColorBtn == HsvBtn)
		{
			DisplayText1.Text = "H";
			DisplayText2.Text = "S";
			DisplayText3.Text = "V";
		}
		else if (SelectedColorBtn == HslBtn)
		{
			DisplayText1.Text = "H";
			DisplayText2.Text = "S";
			DisplayText3.Text = "L";
		}
		else if (SelectedColorBtn == CmykBtn)
		{
			DisplayText1.Text = "C";
			DisplayText2.Text = "M";
			DisplayText3.Text = "Y";
			DisplayText4.Text = "K";
		}
		else if (SelectedColorBtn == XyzBtn)
		{
			DisplayText1.Text = "X";
			DisplayText2.Text = "Y";
			DisplayText3.Text = "Z";
		}
		else if (SelectedColorBtn == YiqBtn)
		{
			DisplayText1.Text = "Y";
			DisplayText2.Text = "I";
			DisplayText3.Text = "Q";
		}
		else if (SelectedColorBtn == YuvBtn)
		{
			DisplayText1.Text = "Y";
			DisplayText2.Text = "U";
			DisplayText3.Text = "V";
		}
	}
}
