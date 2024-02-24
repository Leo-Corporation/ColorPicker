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
	public class ColorInfo(RGB rgb)
	{
		public RGB RGB { get; set; } = rgb;
		public HEX HEX { get; set; } = GetHex(rgb);
		public HSV HSV { get; set; } = ColorConverter.RgbToHsv(rgb);
		public HSL HSL { get; set; } = ColorConverter.RgbToHsl(rgb);
		public CMYK CMYK { get; set; } = ColorConverter.RgbToCmyk(rgb);
		public XYZ XYZ { get; set; } = ColorConverter.RgbToXyz(rgb);
		public YIQ YIQ { get; set; } = ColorConverter.RgbToYiq(rgb);
		public YUV YUV { get; set; } = ColorConverter.RgbToYuv(rgb);
		public DEC DEC { get; set; } = DEC.FromRgb(rgb);

		public override string ToString() => $"{Properties.Resources.RGB}: {RGB.R}{Global.Settings.RgbSeparator}{RGB.G}{Global.Settings.RgbSeparator}{RGB.B}\n" +
				$"{Properties.Resources.HEX}: {(HEX.Value.StartsWith('#') ? "" : "#")}{((Global.Settings.UseUpperCasesHex ?? false) ? HEX.Value.ToUpper() : HEX.Value.ToLower())}\n" +
				$"{Properties.Resources.HSV}: {HSV.H},{HSV.S},{HSV.V}\n" +
				$"{Properties.Resources.HSL}: {HSL.H},{HSL.S},{HSL.L}\n" +
				$"{Properties.Resources.CMYK}: {CMYK.C},{CMYK.M},{CMYK.Y},{CMYK.K}\n" +
				$"{Properties.Resources.DEC}: {DEC.Value}\n" +
				$"{Properties.Resources.XYZ}: {XYZ.X:0.00}..; {XYZ.Y:0.00}..; {XYZ.Z:0.00}..\n" +
				$"{Properties.Resources.YIQ}: {YIQ.Y:0.00}..; {YIQ.I:0.00}..; {YIQ.Q:0.00}..\n" +
				$"{Properties.Resources.YUV}: {YUV.Y:0.00}..; {YUV.U:0.00}..; {YUV.V:0.00}..";

		private static HEX GetHex(RGB rgb)
		{
			var hex = ColorConverter.RgbToHex(rgb);
			hex.Value =  (Global.Settings.UseUpperCasesHex ?? false) ? hex.Value.ToUpper() : hex.Value.ToLower();
			return hex;
		}
	}
}
