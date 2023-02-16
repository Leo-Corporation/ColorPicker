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
using System;
using System.Windows.Controls;

namespace ColorPicker.UserControls;
/// <summary>
/// Interaction logic for ActionCard.xaml
/// </summary>
public partial class ActionCard : UserControl
{
	int Action { get; init; }
	public ActionCard(int actionID)
	{
		InitializeComponent();
		Action = actionID;

		InitUI();
	}

	private void InitUI()
	{
		IconTxt.Text = Global.ActionsIcons[Action]; // Set text
		PageNameTxt.Text = Global.ActionsString[Action]; // Set text
	}

	public static event EventHandler<PageEventArgs> OnCardClick;

	private void Border_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		switch (Action)
		{
			case 0:
				Global.SelectorPage.SelectBtn_Click(this, null);
				OnCardClick?.Invoke(this, new(AppPages.Selector));
				break;
			case 1:
				Global.ChromaticWheelPage.DiscBtn_Click(this, null);
				OnCardClick?.Invoke(this, new(AppPages.ColorWheel));
				break;
			case 2:
				Global.ConverterPage.RgbBtn_Click(Global.ConverterPage.RgbBtn, null);
				OnCardClick?.Invoke(this, new(AppPages.Converter));
				break;
			case 3:
				Global.TextPage.LoadConstrastUI();
				OnCardClick?.Invoke(this, new(AppPages.TextTool));
				break;
			case 4:
				Global.PalettePage.ColorBorder_MouseLeftButtonUp(this, null);
				OnCardClick?.Invoke(this, new(AppPages.ColorPalette));
				break;
			case 5:
				Global.GradientPage.GenerateRandomGradient();
				OnCardClick?.Invoke(this, new(AppPages.ColorGradient));
				break;
			default:
				break;
		}
	}
}
