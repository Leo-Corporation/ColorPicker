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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for AiGenPage.xaml
/// </summary>
public partial class AiGenPage : Page
{
	bool code = Global.Settings.UseSynethia ? false : true; // checks if the code as already been implemented

	public AiGenPage()
	{
		InitializeComponent();
		Loaded += (o, e) => SynethiaManager.InjectSynethiaCode(this, Global.SynethiaConfig.PagesInfo, 4, ref code); // injects the code in the page

	}

	ColorInfo ColorInfo { get; set; }
	internal void LoadDetails()
	{
		// Load the details section		
		RgbTxt.Text = $"{ColorInfo.RGB.R}; {ColorInfo.RGB.G}; {ColorInfo.RGB.B}";
		HexTxt.Text = $"#{ColorInfo.HEX.Value}";
		HsvTxt.Text = $"{ColorInfo.HSV.H}, {ColorInfo.HSV.S}, {ColorInfo.HSV.V}";
		HslTxt.Text = $"{ColorInfo.HSL.H}, {ColorInfo.HSL.S}, {ColorInfo.HSL.L}";
		CmykTxt.Text = $"{ColorInfo.CMYK.C}, {ColorInfo.CMYK.M}, {ColorInfo.CMYK.Y}, {ColorInfo.CMYK.K}";
		XyzTxt.Text = $"{ColorInfo.XYZ.X:0.00}..; {ColorInfo.XYZ.Y:0.00}..; {ColorInfo.XYZ.Z:0.00}..";
		YiqTxt.Text = $"{ColorInfo.YIQ.Y:0.00}..; {ColorInfo.YIQ.I:0.00}..; {ColorInfo.YIQ.Q:0.00}..";
		YuvTxt.Text = $"{ColorInfo.YUV.Y:0.00}..; {ColorInfo.YUV.U:0.00}..; {ColorInfo.YUV.V:0.00}..";
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

	private void GenerateBtn_Click(object sender, RoutedEventArgs e)
	{

	}
}
