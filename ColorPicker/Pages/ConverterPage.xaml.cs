using ColorPicker.Classes;
using ColorPicker.Enums;
using LeoCorpLibrary;
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

namespace ColorPicker.Pages
{
	/// <summary>
	/// Interaction logic for ConverterPage.xaml
	/// </summary>
	public partial class ConverterPage : Page
	{
		public ConverterPage()
		{
			InitializeComponent();
			InitUI(); // Init UI
		}

		private void InitUI()
		{
			ColorTypeComboBox.Items.Add(Global.ColorTypesToString(ColorTypes.RGB)); // Add
			ColorTypeComboBox.Items.Add(Global.ColorTypesToString(ColorTypes.HEX)); // Add
			ColorTypeComboBox.SelectedIndex = 0; // Set index
		}

		private void ColorTxt_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				if (ColorTypeComboBox.Text == Properties.Resources.RGB)
				{
					string[] rgb = ColorTxt.Text.Split(new string[] { ";" }, StringSplitOptions.None); // Split

					RGBTxt.Text = $"{Properties.Resources.RGB} {rgb[0]};{rgb[1]};{rgb[2]}"; // Set text
					HEXTxt.Text = $"{Properties.Resources.HEX} #{ColorsConverter.RGBtoHEX(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2])).Value}"; // Set text
				}
				else if (ColorTypeComboBox.Text == Properties.Resources.HEX)
				{
					var rgb = ColorsConverter.HEXtoRGB(new() { Value = ColorTxt.Text }); // Convert
					string hex = ColorTxt.Text.StartsWith("#") ? ColorTxt.Text : "#" + ColorTxt.Text; // Set

					RGBTxt.Text = $"{Properties.Resources.RGB} {rgb.R};{rgb.G};{rgb.B}"; // Set text
					HEXTxt.Text = $"{Properties.Resources.HEX} {hex}"; // Set text
				}
			}
			catch { }
		}
	}
}
