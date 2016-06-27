using System;
using System.Drawing;
using System.Windows.Forms;

namespace SoundCreator {

	internal class Oscilloscope {
		private PictureBox pb;
		private PictureBox pbFFT;

		private double[] WaveData;

		public Oscilloscope( PictureBox picbox, PictureBox picboxFFT ) {
			pb = picbox;
			pbFFT = picboxFFT;
		}

		private void Draw() {
			int x = pb.Size.Width;
			int y = pb.Size.Height;
			int j;
			double wd1,wd2;

			Bitmap bm = new Bitmap(x, y);
			Graphics g = Graphics.FromImage(bm);

			g.Clear(Color.Black);
			Pen PenWhite = new Pen(Color.FromArgb(0xff,0xc0,0xc0,0xc0));
			Pen PenGreen = new Pen(Color.FromArgb(128,0x80,0xff,0x80));

			g.DrawLine(PenWhite, 0, y / 2, x, y / 2);

			for (int i = 0; i < x; i++) {
				j = i;
				wd1 = WaveData[i * 2];
				wd2 = WaveData[i * 2 + 1];
				if ((int)wd1 == (int)wd2) j = i + 1;
				if (wd1 > 1000) wd1 = 1000.0;
				if (wd1 < -1000) wd1 = -1000.0;
				if (wd2 > 1000) wd2 = 1000.0;
				if (wd2 < -1000) wd2 = -1000.0;
				g.DrawLine(PenGreen, i, (int)wd1, j, (int)wd2);
			}

			pb.Image = bm;
		}

		public void ResetView() {
			int x = pb.Size.Width;
			int y = pb.Size.Height;
			Bitmap bm = new Bitmap(x, y);
			Graphics g = Graphics.FromImage(bm);
			g.Clear(Color.Black);
			Pen PenWhite = new Pen(Color.FromArgb(0xff,0xc0,0xc0,0xc0));
			g.DrawLine(PenWhite, 0, y / 2, x, y / 2);
			pb.Image = bm;

			x = pbFFT.Size.Width;
			y = pbFFT.Size.Height;
			bm = new Bitmap(x, y);
			g = Graphics.FromImage(bm);
			g.Clear(Color.Black);
			pbFFT.Image = bm;
		}

		// zoom = 0 - 100%
		public void SetView( double[] RawWave, double Start, double Zoom ) {
			int StartSample = 0;
			if (RawWave != null) {
				Start = Start * RawWave.Length / 100;
				if (Start != 0.0) {
					//Hitta Synk för start värde
					double oldstartvalue = 0.0;
					double startvalue = 0.0;
					int pos = (int)Start;
					bool StartFound = false;

					for (int i = (int)Start; i < RawWave.Length; i++) {
						pos = i;
						startvalue = RawWave[i];
						if (startvalue > 0.0 && oldstartvalue < 0.0) {
							StartFound = true;
							break;
						}
						oldstartvalue = startvalue;
					}
					StartSample = pos;
					if (!StartFound) StartSample = (int)Start;
				}

				//Beräkna fönsterlängd
				int VisibleWaveLength = (int)(RawWave.Length * Zoom / 100.0);
				int x = pb.Size.Width;
				int x2 = x * 2;
				int y = pb.Size.Height;
				double ScaleFactorX = (double)x / (double)VisibleWaveLength;
				double ScaleFactorY = y / (Form1.MaxAmplitude * 2.0);
				double[] WD = new double[x2];
				double j = 0.0;
				double OldJ = 0.0;
				int k = 0;
				double TempWD = 0.0;
				double MaxWD = -100000.0;
				double MinWD = 100000.0;

				if (StartSample + VisibleWaveLength > RawWave.Length) {
					StartSample = (RawWave.Length - VisibleWaveLength);
				}
				if (ScaleFactorX > 1.0) {
					VisibleWaveLength = x;
					ScaleFactorX = 1.0;
				}
				for (int i = 0; i < VisibleWaveLength; i++) {
					TempWD = -RawWave[StartSample + i] * ScaleFactorY + y / 2;
					if (MaxWD < TempWD) MaxWD = TempWD;
					if (MinWD >= TempWD) MinWD = TempWD;

					OldJ = j;

					j = j + ScaleFactorX;
					if ((int)OldJ != (int)j) {
						WD[k] = MaxWD;
						WD[k + 1] = MinWD;
						k = k + 2;
						//Wow smart!
						TempWD = MinWD;
						MinWD = MaxWD;
						MaxWD = TempWD;
					}
				}

				WaveData = WD;
				Draw();
			}
		}

		// Visa ljudspectrum
		public void DrawFFT( double[] RawWave ) {
			Filter FilterObj = new Filter();
			int FFTSize = FilterObj.GetFFTSize();

			double[] FFT = new double[FFTSize];


			FFT = FilterObj.GetFFT(RawWave);

			int x = pbFFT.Size.Width;
			double[] FFTwindow = new double[x];

			int y = pbFFT.Size.Height;


			Bitmap bm = new Bitmap(x, y);
			Graphics g = Graphics.FromImage(bm);

			g.Clear(Color.Black);
			Pen PenWhite = new Pen(Color.FromArgb(0xff,0xc0,0xc0,0xc0));
			Pen PenGreen = new Pen(Color.FromArgb(128,0x80,0xff,0x80));

			float ii = (float)x / (FFTSize / 2);
			float counter = 0;
			float OldC = 0;
			int k = 0;

			for (int i = 0; i < FFTSize / 2; i++) {
				if ((int)counter >= x) break;
				FFTwindow[(int)counter] = FFTwindow[(int)counter] + FFT[i];
				OldC = counter;
				counter = counter + ii;
				if ((int)counter != (int)OldC) {
					FFTwindow[(int)OldC] = FFTwindow[(int)OldC] / k;
                    k = 0;
				}
				k++;
			}

			double Max = -100000;
			for (int j = 1; j < x; j++) {
				if (FFTwindow[j] > Max) Max = FFTwindow[j];
			}
			for (int j = 1; j < x; j++) {
				FFTwindow[j] = FFTwindow[j] / Max;
			}



			for (int i = 1; i < x; i++) {
				g.DrawLine(PenGreen, i, (float)(1 - FFTwindow[(int)(i)]) * y, i, y);
			}


			pbFFT.Image = bm;
		}



	}
}