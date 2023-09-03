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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorPicker.Classes;
public class DEC
{
	public int Value { get; set; }

	public DEC(int value)
	{
		Value = value;
	}

	public static DEC FromRgb(RGB rgb)
	{
		if (rgb.R < 0 || rgb.R > 255 || rgb.G < 0 || rgb.G > 255 || rgb.B < 0 || rgb.B > 255)
		{
			throw new ArgumentException("RGB values must be between 0 and 255.");
		}

		int decimalValue = (rgb.R << 16) | (rgb.G << 8) | rgb.B;
		return new(decimalValue);
	}

	public RGB ToRgb()
	{
		int red = (Value >> 16) & 255;
		int green = (Value >> 8) & 255;
		int blue = Value & 255;
		return new((byte)red, (byte)green, (byte)blue);
	}
}
