using System;
using System.Drawing;
using System.Windows.Forms;

namespace SoundCreator {

	public struct NL {
		public double A;
		public double B;
		public double C;
		public double MinValue;
		public double MidValue;
		public double MaxValue;
		public double SliderValue;
		public double ClickedSliderValue;
		public double OldSliderValue;
		public double DisplayValue;
	}

	internal class Slider {
		private NL nl;
		private PictureBox pb;

		// Olinjär
		public Slider( PictureBox PixBox, double DisplayValue, double MinValue, double MaxValue, double MidValue ) {
			nl = new NL();
			nl = NonLinear(MinValue, MaxValue, MidValue, nl);
			pb = PixBox;
			nl.DisplayValue = DisplayValue;
		}

		// Linjär
		public Slider( PictureBox PixBox, double DisplayValue, double MinValue, double MaxValue ) {
			double MidValue = (MaxValue - MinValue) / 2.0 - 0.000001;
			nl = new NL();
			nl = NonLinear(MinValue, MaxValue, MidValue, nl);
			pb = PixBox;
			nl.DisplayValue = DisplayValue;
		}

		public void Draw( double DisplayValue ) {
			int x = pb.Size.Width;
			int y = pb.Size.Height;
			nl.DisplayValue = DisplayValue;

			double SliderValue = (Math.Log((DisplayValue - nl.A) / nl.B) / nl.C) * x;
			nl.SliderValue = SliderValue;

			Bitmap bm = new Bitmap(pb.Size.Width, pb.Size.Height);
			Graphics g = Graphics.FromImage(bm);

			g.Clear(Color.Black);
			Pen PenWhite = new Pen(Color.White);
			Pen PenYellow = new Pen(Color.Yellow);

			g.DrawLine(PenWhite, 0, y / 2, x, y / 2);
			g.DrawLine(PenWhite, x / 2, 0 + 2, x / 2, y - 2);

			Rectangle rec = new Rectangle();
			Brush BrushPos = new SolidBrush(Color.FromArgb(0xC0, 0x40, 0x48, 0xFF));
			Brush BrushNeg = new SolidBrush(Color.FromArgb(0xC0, 0xFF, 0x40, 0x40));

			rec.X = 0;
			rec.Y = 1;
			rec.Height = y-2;
			rec.Width = (int)SliderValue;
			g.FillRectangle(BrushPos, rec);
			g.DrawLine(PenYellow, (int)SliderValue, 0, (int)SliderValue, y);

			g.DrawLine(PenWhite, 0, 0, 0, y);
			g.DrawLine(PenWhite, x - 1, 0, x - 1, y);

			pb.Image = bm;
		}

		public double MouseDown( MouseEventArgs e ) {
			nl.SliderValue = e.X;
			nl.ClickedSliderValue = e.X;
			nl.OldSliderValue = (Math.Log((nl.DisplayValue - nl.A) / nl.B) / nl.C) * pb.Size.Width;
			if (e.Button.ToString() == "Left") {
				nl.DisplayValue = Move(nl.SliderValue);
			}
			return nl.DisplayValue;
		}

		public double MouseMove( MouseEventArgs e, double Speed ) {
			if (e.Button.ToString() == "Left") {
				nl.SliderValue = e.X;
				nl.DisplayValue = Move(nl.SliderValue);
			} else if (e.Button.ToString() == "Right") {
				nl.SliderValue = nl.OldSliderValue + ((e.X - nl.ClickedSliderValue) / Speed);
				nl.DisplayValue = Move(nl.SliderValue);
			}
			return nl.DisplayValue;
		}

		public double Move( double SliderValue ) {
			double DisplayValue = nl.A + nl.B * Math.Exp(nl.C * (SliderValue  / pb.Size.Width));
			if (DisplayValue > nl.MaxValue) DisplayValue = nl.MaxValue;
			if (DisplayValue < nl.MinValue) DisplayValue = nl.MinValue;
			DisplayValue = Math.Round(DisplayValue, 3);
			Draw(DisplayValue);
			nl.DisplayValue = DisplayValue;
			return DisplayValue;
		}

		public NL NonLinear( double MinValue, double MaxValue, double MidValue, NL nl ) {
			double x = MinValue;
			double y = MidValue;
			double z = MaxValue;
			double a;

			// Max skall vara största värdet
			if (x > z) {
				a = x;
				x = z;
				z = a;
			}

			if (z > y && y > x && (x - 2 * y + z) != 0.0) {
				nl.A = (x * z - y * y) / (x - 2 * y + z);
				nl.B = ((y - x) * (y - x)) / (x - 2 * y + z);
				nl.C = 2 * Math.Log((z - y) / (y - x));
			} else {
				nl.A = 0.0;
				nl.B = 0.0;
				nl.C = 0.0;
			}
			nl.MinValue = x;
			nl.MidValue = y;
			nl.MaxValue = z;
			return nl;
		}
	}
}