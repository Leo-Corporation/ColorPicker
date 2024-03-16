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
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ColorPicker.Classes
{
	public class Bookmarks
	{
		public List<string> ColorBookmarks { get; set; }
		public List<string>? ColorBookmarksNotes { get; set; }
		public List<string> PaletteBookmarks { get; set; }
		public List<Gradient> GradientBookmarks { get; set; }
		public List<BookmarkText> TextBookmarks { get; set; }
	}

	public class BookmarkText : IEquatable<BookmarkText>
	{
		public string FontFamily { get; init; }
		public string ForegroundColor { get; init; }
		public string BackgroundColor { get; init; }
		public BookmarkText()
		{
			FontFamily = "Arial"; // Fallback values
			ForegroundColor = "#000000";
			BackgroundColor = "#FFFFFF";
		}

		public BookmarkText(string font, string fore, string back)
		{
			FontFamily = font;
			ForegroundColor = fore;
			BackgroundColor = back;
		}

		public bool Equals(BookmarkText? obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return FontFamily == obj.FontFamily && ForegroundColor == obj.ForegroundColor && BackgroundColor == obj.BackgroundColor;
		}


		public override bool Equals(object obj) => Equals(obj as BookmarkText);
	}

	[XmlType("BookmarkGradientStop")]
	public record BookmarkGradientStop
	{
		// Make the properties settable
		public string Color { get; set; }
		public double Stop { get; set; }

		// Add a parameterless constructor
		public BookmarkGradientStop()
		{
			// Assign default values to the properties
			Color = "Black";
			Stop = 0.0;
		}

		// Add a constructor with two parameters
		public BookmarkGradientStop(string color, double stop)
		{
			Color = color;
			Stop = stop;
		}
	}

	public class Gradient : IEquatable<Gradient>
	{
		public List<BookmarkGradientStop> Stops { get; init; }
		public double Angle { get; init; }

		// The constructor of the class
		public Gradient(List<BookmarkGradientStop> stops, double angle)
		{
			Stops = stops;
			Angle = angle;
		}

		public Gradient()
		{
			Stops = [];
			Angle = 0;
		}

		public bool Equals(Gradient? obj)
		{
			if (obj is null || obj.Stops.Count != Stops.Count || obj.Angle != Angle) return false;
			for (int i = 0; i < obj.Stops.Count; i++)
			{
				if (obj.Stops[i] != Stops[i]) return false;
			}
			return true;
		}

		public override bool Equals(object obj) => Equals(obj as Gradient);
	}
}
