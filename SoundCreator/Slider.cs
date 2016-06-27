using System;
using System.Drawing;
using System.Windows.Forms;

namespace SoundCreator {

	public struct NL {
		public string Text;
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
		public int Orientation;     // 0 = X, 1 = Y
		public bool PrintText;
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
			nl.Text = DisplayValue.ToString();
			nl.PrintText = false;
		}

		// Linjär
		public Slider( PictureBox PixBox, double DisplayValue, double MinValue, double MaxValue ) {
			double MidValue = (MaxValue - MinValue) / 2.0 - 0.000001;
			nl = new NL();
			nl = NonLinear(MinValue, MaxValue, MidValue, nl);
			pb = PixBox;
			nl.DisplayValue = DisplayValue;
			nl.Text = DisplayValue.ToString();
			nl.PrintText = false;
		}

		// Olinjär
		public Slider( PictureBox PixBox, double DisplayValue, double MinValue, double MaxValue, double MidValue, string Text ) {
			nl = new NL();
			nl = NonLinear(MinValue, MaxValue, MidValue, nl);
			pb = PixBox;
			if (pb.Size.Width < pb.Size.Height) nl.Orientation = 1; else nl.Orientation = 0;
			nl.DisplayValue = DisplayValue;
			nl.Text = Text;
			nl.PrintText = true;
		}

		// Linjär
		public Slider( PictureBox PixBox, double DisplayValue, double MinValue, double MaxValue, string Text ) {
			double MidValue = (MaxValue - MinValue) / 2.0 - 0.000001;
			nl = new NL();
			nl = NonLinear(MinValue, MaxValue, MidValue, nl);
			pb = PixBox;
			if (pb.Size.Width < pb.Size.Height) nl.Orientation = 1; else nl.Orientation = 0;
			nl.DisplayValue = DisplayValue;
			nl.Text = Text;
			nl.PrintText = true;
		}

		public void Draw( double DisplayValue ) {
			int x = pb.Size.Width;
			int y = pb.Size.Height;
			double SliderValue;

			nl.DisplayValue = DisplayValue;

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

			if (nl.Orientation == 0) {
				SliderValue = (Math.Log((DisplayValue - nl.A) / nl.B) / nl.C) * x;
				nl.SliderValue = SliderValue;
				rec.X = 0;
				rec.Y = 1;
				rec.Height = y - 2;
				rec.Width = (int)SliderValue;

				g.FillRectangle(BrushPos, rec);
				g.DrawLine(PenYellow, (int)SliderValue, 0, (int)SliderValue, y);

				g.DrawLine(PenWhite, 0, 0, 0, rec.Height);
				g.DrawLine(PenWhite, x - 1, 0, x - 1, rec.Height);
				if (!nl.PrintText) nl.Text = DisplayValue.ToString();
				Font myFont = new Font("Arial", 7);
				g.DrawString(nl.Text, myFont, Brushes.Yellow, new PointF(0, 0));
			} else {
				SliderValue = (Math.Log((DisplayValue - nl.A) / nl.B) / nl.C) * y;
				nl.SliderValue = SliderValue;
				rec.X = 1;
				rec.Y = y - (int)SliderValue;
				rec.Width = x - 2;
				rec.Height = (int)SliderValue;

				g.FillRectangle(BrushPos, rec);
				g.DrawLine(PenYellow, 0, y - (int)SliderValue, y, y - (int)SliderValue);

				g.DrawLine(PenWhite, 0, 0, x, 0);
				g.DrawLine(PenWhite, 0, y - 1, x, y - 1);
				if (!nl.PrintText) nl.Text = DisplayValue.ToString();
				Font myFont = new Font("Arial", 7);
				g.TranslateTransform(1, y - 1);
				g.RotateTransform(-90);
				g.DrawString(nl.Text, myFont, Brushes.Yellow, new PointF(0, 0));


			}

			pb.Image = bm;
		}

		public double MouseDown( MouseEventArgs e ) {
			int Size;
			if (nl.Orientation == 0) {
				Size = pb.Size.Width;
				nl.SliderValue = e.X;
				nl.ClickedSliderValue = e.X;
			} else {
				Size = pb.Size.Height;
				nl.SliderValue = -1 + Size - e.Y;
				nl.ClickedSliderValue = -1 + Size - e.Y;
			}
			nl.OldSliderValue = (Math.Log((nl.DisplayValue - nl.A) / nl.B) / nl.C) * Size;
			if (e.Button.ToString() == "Left") {
				nl.DisplayValue = Move(nl.SliderValue);
			}
			return nl.DisplayValue;
		}

		public double MouseMove( MouseEventArgs e, double Speed ) {
			int Movement;
			if (nl.Orientation == 0) Movement = e.X; else Movement = -1 + pb.Size.Height - e.Y;
			if (e.Button.ToString() == "Left") {
				nl.SliderValue = Movement;
				nl.DisplayValue = Move(nl.SliderValue);
			} else if (e.Button.ToString() == "Right") {
				nl.SliderValue = nl.OldSliderValue + ((Movement - nl.ClickedSliderValue) / Speed);
				nl.DisplayValue = Move(nl.SliderValue);
			}
			return nl.DisplayValue;
		}

		public double Move( double SliderValue ) {
			int Size;
			if (nl.Orientation == 0) Size = pb.Size.Width; else Size = pb.Size.Height;
			double DisplayValue = nl.A + nl.B * Math.Exp(nl.C * (SliderValue  / Size));
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

	// Exempelkod
	//namespace Lekstuga2 {
	//	public partial class Form1 : Form {
	//		private Slider SliderGain1;
	//		private double ValueGain1;
	//		public Form1() {
	//			InitializeComponent();
	//			SliderGain1 = new Slider(pbGain1, 50.0, 0.0, 100.0);
	//		}

	//		private void pbGain1_MouseDown( object sender, MouseEventArgs e ) {
	//			double DisplayValue = SliderGain1.MouseDown(e);
	//			ValueGain1 = DisplayValue;
	//		}

	//		private void pbGain1_MouseMove( object sender, MouseEventArgs e ) {
	//			if (e.Button.ToString() != "None") {
	//				double DisplayValue = SliderGain1.MouseMove(e, 5.0);
	//				ValueGain1 = DisplayValue;
	//			}
	//		}
	//	}
	//}
