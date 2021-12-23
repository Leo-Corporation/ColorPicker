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
using LeoCorpLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ColorPicker.Classes
{
	public class ColorContentHistory
	{
		/// <summary>
		/// Colors in history
		/// </summary>
		public List<int[]> PickerColorsRGB { get; set; }
	}

	public static class HistoryManager
	{
		/// <summary>
		/// Saves the content of the history.
		/// </summary>
		public static void Save()
		{
			string path = Env.AppDataPath + @"\Léo Corporation\ColorPicker\ColorHistory.xml"; // The path of the file

			XmlSerializer xmlSerializer = new(typeof(ColorContentHistory)); // XML Serializer

			if (!Directory.Exists(Env.AppDataPath + @"\Léo Corporation\ColorPicker")) // If the directory doesn't exist
			{
				Directory.CreateDirectory(Env.AppDataPath + @"\Léo Corporation\"); // Create the directory
				Directory.CreateDirectory(Env.AppDataPath + @"\Léo Corporation\ColorPicker"); // Create the directory
			}

			StreamWriter streamWriter = new StreamWriter(path); // The place where the file is going to be written
			xmlSerializer.Serialize(streamWriter, Global.ColorContentHistory);

			streamWriter.Dispose();
		}
	}
}
