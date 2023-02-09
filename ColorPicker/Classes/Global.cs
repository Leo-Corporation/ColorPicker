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
using ColorPicker.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ColorPicker.Classes;
public static class Global
{
	public static SelectorPage? SelectorPage { get; set; }
	public static ChromaticWheelPage? ChromaticWheelPage { get; set; }
	public static ConverterPage? ConverterPage { get; set; }
	public static TextPage? TextPage { get; set; }

	public static Bookmarks Bookmarks { get; set; }

	public static string Version => "5.0.0.2302-pre2";

	public static string HiSentence
	{
		get
		{
			if (DateTime.Now.Hour >= 21 && DateTime.Now.Hour <= 7) // If between 9PM & 7AM
			{
				return Properties.Resources.GoodNight + ", " + Environment.UserName + "."; // Return the correct value
			}
			else if (DateTime.Now.Hour >= 7 && DateTime.Now.Hour <= 12) // If between 7AM - 12PM
			{
				return Properties.Resources.Hi + ", " + Environment.UserName + "."; // Return the correct value
			}
			else if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour <= 17) // If between 12PM - 5PM
			{
				return Properties.Resources.GoodAfternoon + ", " + Environment.UserName + "."; // Return the correct value
			}
			else if (DateTime.Now.Hour >= 17 && DateTime.Now.Hour <= 21) // If between 5PM - 9PM
			{
				return Properties.Resources.GoodEvening + ", " + Environment.UserName + "."; // Return the correct value
			}
			else
			{
				return Properties.Resources.Hi + ", " + Environment.UserName + "."; // Return the correct value
			}
		}
	}

	public static (int, int, int) GenerateRandomColor()
	{
		Random random = new();
		int r = random.Next(0, 255); int g = random.Next(0, 255); int b = random.Next(0, 255); // Generate random values
		return (r, g, b);
	}
	public static Color GetColorFromResource(string resourceName) => (Color)ColorConverter.ConvertFromString(Application.Current.Resources[resourceName].ToString());

	public static double GetLuminance(int r, int g, int b)
	{
		int[] rgb = { r, g, b };
		double[] res = new double[3];

		int i = 0;
		foreach (int val in rgb)
		{
			double v = val / 255d;
			res[i] = v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
			i++;
		}

		return res[0] * 0.2126 + res[1] * 0.7152 + res[2] * 0.0722;
	}

	public static (string, int) GetContrast(int[] rgb1, int[] rgb2)
	{
		var lum1 = GetLuminance(rgb1[0], rgb1[1], rgb1[2]);
		var lum2 = GetLuminance(rgb2[0], rgb2[1], rgb2[2]);

		var brightest = Math.Max(lum1, lum2);
		var darkest = Math.Min(lum1, lum2);

		var result = Math.Round((brightest + 0.05) / (darkest + 0.05), 4);

		int gridRow;
		if (result > 7) gridRow = 0;
		else gridRow = 0;
		if (result <= 3) gridRow = 3;
		if (result >= 3 && result <= 4.5) gridRow = 2;
		if (result >= 4.5 && result <= 7) gridRow = 1;
		return (result.ToString(), gridRow);
	}
}
