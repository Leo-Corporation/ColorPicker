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
using ColorPicker.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorPicker.UserControls;

/// <summary>
/// A thin wrapper around PixiEditor.ColorPicker's <see cref="global::ColorPicker.SquarePicker"/>
/// (the circular hue wheel + HSV/HSL square). The wrapper lives in THIS project's namespace so
/// XAML can reference it without clashing with the library's own "ColorPicker" assembly name.
/// It exposes <see cref="SelectedColor"/> and <see cref="ColorChanged"/> mirroring the inner picker.
/// </summary>
public class ColorWheelControl : ContentControl
{
	private readonly global::ColorPicker.SquarePicker _picker;

	public ColorWheelControl()
	{
		_picker = new global::ColorPicker.SquarePicker
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			VerticalAlignment = VerticalAlignment.Stretch,
			Margin = new Thickness(0)
		};

		Content = _picker;

		_picker.ColorChanged += (s, e) =>
		{
			ColorChanged?.Invoke(this, (ColorRoutedEventArgs)e);
		};
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
		ctrl._picker.SelectedColor = (Color)e.NewValue;
	}

	public event RoutedEventHandler ColorChanged;
}
