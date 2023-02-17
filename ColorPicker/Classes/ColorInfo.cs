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

namespace ColorPicker.Classes
{
	public class ColorInfo
	{
		public RGB RGB { get; set; }
		public HEX HEX { get; set; }
		public HSV HSV { get; set; }
		public HSL HSL { get; set; }
		public CMYK CMYK { get; set; }
		public XYZ XYZ { get; set; }
		public YIQ YIQ { get; set; }
		public YUV YUV { get; set; }

		public ColorInfo(RGB rgb)
		{
			RGB = rgb;
			HEX = ColorConverter.RgbToHex(rgb);
			HSV = ColorConverter.RgbToHsv(rgb);
			HSL = ColorConverter.RgbToHsl(rgb);
			CMYK = ColorConverter.RgbToCmyk(rgb);
			XYZ = ColorConverter.RgbToXyz(rgb);
			YIQ = ColorConverter.RgbToYiq(rgb);
			YUV = ColorConverter.RgbToYuv(rgb);
		}

		public override string ToString()
		{
			return $"{Properties.Resources.RGB}: {RGB.R};{RGB.G};{RGB.B}\n" +
				$"{Properties.Resources.HEX}: {HEX.Value}\n" +
				$"{Properties.Resources.HSV}: {HSV.H},{HSV.S},{HSV.V}\n" +
				$"{Properties.Resources.HSL}: {HSL.H},{HSL.S},{HSL.L}\n" +
				$"{Properties.Resources.CMYK}: {CMYK.C},{CMYK.M},{CMYK.Y},{CMYK.K}\n" +
				$"{Properties.Resources.XYZ}: {XYZ.X:0.00}..; {XYZ.Y:0.00}..; {XYZ.Z:0.00}..\n" +
				$"{Properties.Resources.YIQ}: {YIQ.Y:0.00}..; {YIQ.I:0.00}..; {YIQ.Q:0.00}..\n" +
				$"{Properties.Resources.YUV}: {YUV.Y:0.00}..; {YUV.U:0.00}..; {YUV.V:0.00}..";
		}
	}
}
