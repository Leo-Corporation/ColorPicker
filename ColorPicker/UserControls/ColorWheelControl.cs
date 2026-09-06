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
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorPicker.UserControls;

/// <summary>
/// A thin wrapper around PixiEditor.ColorPicker's <c>SquarePicker</c> (circular hue wheel + HSV/HSL square).
/// The picker is instantiated via reflection so we avoid the assembly-name clash with this project's own
/// "ColorPicker" assembly (which would otherwise bind <c>SquarePicker</c> to the wrong assembly at runtime).
/// Exposes <see cref="SelectedColor"/> and <see cref="ColorChanged"/>. The inner library's
/// <c>ColorRoutedEventArgs</c> is forwarded unchanged, so consumers can read its <c>.Color</c> property.
/// </summary>
public class ColorWheelControl : ContentControl
{
	private static readonly Assembly LibAssembly = LoadLibrary();
	private static readonly Type PickerType = LibAssembly.GetType("ColorPicker.SquarePicker")!;

	private readonly object _picker;

	public ColorWheelControl()
	{
		_picker = Activator.CreateInstance(PickerType)!;
		if (_picker is FrameworkElement fe)
		{
			fe.HorizontalAlignment = HorizontalAlignment.Stretch;
			fe.VerticalAlignment = VerticalAlignment.Stretch;
			fe.Margin = new Thickness(0);
		}

		Content = _picker;

		var colorChanged = PickerType.GetEvent("ColorChanged")!;
		var handler = new RoutedEventHandler(OnPickerColorChanged);
		colorChanged.AddEventHandler(_picker, handler);
	}

	private void OnPickerColorChanged(object sender, RoutedEventArgs e)
	{
		// Forward the library's ColorRoutedEventArgs (which carries .Color) to our own event.
		ColorChanged?.Invoke(this, e);
	}

	public static readonly DependencyProperty SelectedColorProperty =
		DependencyProperty.Register(
			nameof(SelectedColor),
			typeof(Color),
			typeof(ColorWheelControl),
			new PropertyMetadata(Colors.Black, OnSelectedColorChanged));

	public Color SelectedColor
	{
		get => (Color)GetValue(SelectedColorProperty);
		set => SetValue(SelectedColorProperty, value);
	}

	private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var ctrl = (ColorWheelControl)d;
		var prop = PickerType.GetProperty("SelectedColor")!;
		prop.SetValue(ctrl._picker, (Color)e.NewValue);
	}

	private static Assembly LoadLibrary()
	{
		try
		{
			// The library's assembly is also named "ColorPicker" (same as this app's exe), so we must
			// load it by its full strong name (with version + PublicKeyToken) to disambiguate it from
			// this project's own "ColorPicker" assembly.
			return Assembly.Load("ColorPicker, Version=3.4.1.0, Culture=neutral, PublicKeyToken=1c61eec504ce2276");
		}
		catch
		{
			string dir = AppDomain.CurrentDomain.BaseDirectory;
			return Assembly.LoadFrom(System.IO.Path.Combine(dir, "ColorPicker.dll"));
		}
	}

	public event RoutedEventHandler ColorChanged;
}
