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
using ColorPicker.UserControls;
using Synethia;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ColorPicker.Pages;
/// <summary>
/// Interaction logic for HomePage.xaml
/// </summary>
public partial class HomePage : Page
{
	public HomePage()
	{
		InitializeComponent();
		InitUI();
	}

	internal void InitUI()
	{
		List<PageInfo> relevantPages = Global.SynethiaConfig.MostRelevantPages;
		for (int i = 0; i < 4; i++)
		{
			GetStartedPanel.Children.Add(new PageCard(Global.PageInfoToAppPages(relevantPages[i])));
		}
		relevantPages.RemoveRange(0, 4); // Remove already added pages; the least releavnt remains

		// Load "Discover" section
		for (int i = 0; i < relevantPages.Count; i++)
		{
			DiscoverPanel.Children.Add(new PageCard(Global.PageInfoToAppPages(relevantPages[i])));
		}

		// Keyboard hints
		KeyboardHintTxt.Text = string.Format(Properties.Resources.KeyboardSelectionHint, Global.Settings.SelectKeyboardShortcut.Replace("LControlKey", "Ctrl")
			.Replace("LShiftKey", "Shift")
			.Replace("RShiftKey", "Shift")
			.Replace("RControlKey", "Ctrl"));
	}

	private void SelectColor_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		Global.SelectorPage.SelectBtn_Click(sender, e);
	}
}
