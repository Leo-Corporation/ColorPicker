	private void WheelImg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Bitmap bitmap = new(1, 1);
		Graphics GFX = Graphics.FromImage(bitmap);
		GFX.CopyFromScreen(System.Windows.Forms.Cursor.Position, new System.Drawing.Point(0, 0), bitmap.Size);
		var pixel = bitmap.GetPixel(0, 0);
		LoadDetails(new(new(pixel.R, pixel.G, pixel.B)));
	}

	private void LoadDetails(ColorInfo colorInfo)
	{
		ColorInfo = ColorInfo;
		RgbTxt.Text = $"{colorInfo.RGB.R}; {colorInfo.RGB.G}; {colorInfo.RGB.B}";
		HexTxt.Text = $"#{colorInfo.HEX.Value}";
		HsvTxt.Text = $"{colorInfo.HSV.H}, {colorInfo.HSV.S}, {colorInfo.HSV.V}";
		HslTxt.Text = $"{colorInfo.HSL.H}, {colorInfo.HSL.S}, {colorInfo.HSL.L}";
		CmykTxt.Text = $"{colorInfo.CMYK.C}, {colorInfo.CMYK.M}, {colorInfo.CMYK.Y}, {colorInfo.CMYK.K}";
		XyzTxt.Text = $"{colorInfo.XYZ.X:0.00}.., {colorInfo.XYZ.Y:0.00}.., {colorInfo.XYZ.Z:0.00}..";
		YiqTxt.Text = $"{colorInfo.YIQ.Y:0.00}.., {colorInfo.YIQ.I:0.00}.., {colorInfo.YIQ.Q:0.00}..";
		YuvTxt.Text = $"{colorInfo.YUV.Y:0.00}.., {colorInfo.YUV.U:0.00}.., {colorInfo.YUV.V:0.00}..";
	}
	ColorInfo ColorInfo { get; set; }
