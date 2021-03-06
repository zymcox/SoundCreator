﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;




namespace SoundCreator {

	public struct OscillatorData {
		public bool Active;
		public bool SoundOut;

		public int WaveType;                 // Custom, Sin, Sqr, Tri, Saw, Noice....
		public double SquareDuty;
		public double StartPhase;
		public double Volume;
		public double Attack;
		public double Decay;
		public double Sustain;
		public double Release;
		public double GateTime;
		public double Delay;
		public double Frequency;

		public int OscBitResolution;

		public int VolumeFromOsc;           // AM Amplitudemodulering
		public double AMDepth;
		public int FrequencyFromOsc;        // FM Frekvensmodulering
		public double FMDepth;
		public int PhaseFromOsc;            // PM Fasmodulation
		public double PMDepth;
		public int SquareDutyFromOsc;
		public double SDDepth;
		public int RingModulationFromOsc;
		public int SyncFromOsc;

		public double RndA;
		public double RndB;
		public double RndC;

		public double RndPA;
		public double RndPB;
		public double RndPC;
	}

	// Chromatic scale
	public struct Note {
		public double Frequency;
		public string Name1;  // 2C
		public string Name2; // C2# C Sharp
	}


	internal class Oscillator {
		private short[] RealSoundBuffer1;
		private short[] RealSoundBuffer2;
		private short[] RealSoundBuffer3;
		private short[] RealSoundBuffer4
			;

		private double Pi = Math.PI;
		private double Pi2 = Math.PI * 2;
		private double OldRandomValue = 0.0;
		private bool FirstPart = false;
		private Note[] Notes;
		private string[] strJingle;
		private int rndTakt;

		public OscillatorData[] ResetAll( OscillatorData[] OD ) {
			OD[0].Active = true;
			OD[0].SoundOut = true;

			OD[0].WaveType = 1;                 // Sinus
			OD[0].SquareDuty = 50.0;            //
			OD[0].StartPhase = 0.0;             // 0.0 - 359.9 grader
			OD[0].Volume = 65535.0;             // 0 - 65535.0
			OD[0].Attack = 0.0;                // 0.0ms - 3000.0ms
			OD[0].Decay = 500.0;                // 0.0ms - 3000.0ms
			OD[0].Sustain = 0.0;              // Volym (Tid ???????)
			OD[0].Release = 0.0;              // 0.0ms - 3000.0ms
			OD[0].GateTime = 2000.0;            // 0 - 9000.0ms
			OD[0].Delay = 0.0;                  // 0 - 3000.0ms
			OD[0].Frequency = 440;              // 0 - 20000Hz

			OD[0].OscBitResolution = 0;

			OD[0].VolumeFromOsc = -1;
			OD[0].AMDepth = 50.0;
			OD[0].FrequencyFromOsc = -1;
			OD[0].FMDepth = 50.0;
			OD[0].PhaseFromOsc = -1;
			OD[0].PMDepth = 50.0;
			OD[0].SquareDutyFromOsc = -1;
			OD[0].SDDepth = 50.0;
			OD[0].RingModulationFromOsc = -1;
			OD[0].SyncFromOsc = -1;

			OD[0].RndA = 0.75;
			OD[0].RndB = 0.75;
			OD[0].RndC = 0.75;
			OD[0].RndPA = 0.0;
			OD[0].RndPB = 0.0;
			OD[0].RndPC = 0.0;

			for (int i = 1; i < OD.Length; i++) {
				OD[i].Active = false;
				OD[i].SoundOut = false;

				OD[i].WaveType = 1;                 // Sinus
				OD[i].SquareDuty = 50.0;
				OD[i].StartPhase = 0.0;             // 0.0 - 359.9 grader
				OD[i].Volume = 65535.0;             // 0 - 65535.0
				OD[i].Attack = 0.0;                // 0.0ms - 3000.0ms
				OD[i].Decay = 500.0;                // 0.0ms - 3000.0ms
				OD[i].Sustain = 0.0;              // Volym (Tid ???????)
				OD[i].Release = 0.0;              // 0.0ms - 3000.0ms
				OD[i].GateTime = 2000.0;            // 0 - 9000.0ms
				OD[i].Delay = 0.0;                  // 0 - 3000.0ms
				OD[i].Frequency = 440;              // 0 - 20000Hz

				OD[i].OscBitResolution = 0;

				OD[i].VolumeFromOsc = -1;
				OD[i].AMDepth = 50.0;
				OD[i].FrequencyFromOsc = -1;
				OD[i].FMDepth = 50.0;
				OD[i].PhaseFromOsc = -1;
				OD[i].PMDepth = 50.0;
				OD[i].SquareDutyFromOsc = -1;
				OD[i].SDDepth = 50.0;
				OD[i].RingModulationFromOsc = -1;
				OD[i].SyncFromOsc = -1;

				OD[i].RndA = 0.75;
				OD[i].RndB = 0.75;
				OD[i].RndC = 0.75;
				OD[i].RndPA = 0.0;
				OD[i].RndPB = 0.0;
				OD[i].RndPC = 0.0;
			}
			return OD;
		}

		public Oscillator() {
			RealSoundBuffer1 = new short[Form1.Samplerate * (Form1.TimeMS / 1000)];
			RealSoundBuffer2 = new short[Form1.Samplerate * (Form1.TimeMS / 1000)];
			RealSoundBuffer3 = new short[Form1.Samplerate * (Form1.TimeMS / 1000)];
			RealSoundBuffer4 = new short[Form1.Samplerate * (Form1.TimeMS / 1000)];
			CreatePianoNotes();
		}

		public double[][] CreateWave( OscillatorData[] OD, MixerData MD ) {
			double[][] OscArray = new double[Form1.MaxOscillators][];
			Random Rnd = new Random();

			// Ocsillator för ocsillator...
			for (int Osc = 0; Osc < Form1.MaxOscillators; Osc++) {
				double DelayStart = 0.0;
				double Phase = 0.0;
				double StartPhase = 0.0;
				double PhaseFM = 0.0;
				double PhaseNow = 0.0;
				double PhaseMod = 0.0;
				double PhaseRandom = 0.0;
				double SyncAmp = 0.0;
				double OldSyncAmp = 0.0;
				double A = OD[Osc].Volume / 2.0;
				double AmpMod = 1.0;
				double AmpNow = 1.0;
				double AmpEnv = 0.0;
				double x,y;
				double SquareDuty;
				double Value = 0.0;
				double RealSoundProgress = 0.0;
				double OldRealSoundProgress = 0.0;

				OscArray[Osc] = new double[Form1.OscArraySize];

				if (OD[Osc].Active) {
					// Sätt upp envelop
					ADSR env = new ADSR();
					env.setAttackRate(Form1.Samplerate * OD[Osc].Attack / 1000.0);
					env.setDecayRate(Form1.Samplerate * OD[Osc].Decay / 1000.0);
					env.setSustainLevel(OD[Osc].Sustain / (Form1.MaxAmplitude * 2));
					env.setReleaseRate(Form1.Samplerate * OD[Osc].Release / 1000.0);
					env.setTargetRatioA(0.00000000001);
					env.setTargetRatioDR(0.00000000001);
					env.gate(true);

					// Startvinkel
					StartPhase = Pi2 * OD[Osc].StartPhase / 360.0;
					Phase = StartPhase;
					// Fördröjd start
					DelayStart = Form1.Samplerate * OD[Osc].Delay / 1000.0; // Delay i millisekunder

					for (int t = (int)DelayStart; t < Form1.OscArraySize; t++) {
						// beräkna fasvinkel.
						x = (Phase + PhaseMod + PhaseFM + PhaseRandom) / Pi2;
						PhaseNow = (x - (int)(x)) * Pi2; //Wrap till 0 - 2*pi
						RealSoundProgress = x / 440;

						// Amplitudmodulering
						if (OD[Osc].VolumeFromOsc != -1) {
							AmpMod = (((OscArray[OD[Osc].VolumeFromOsc][t] / (Form1.MaxAmplitude))) * OD[Osc].AMDepth / 200) + 0.5; // 0 - 1
						}
						// Fasmodulering
						if (OD[Osc].PhaseFromOsc != -1) {
							PhaseMod = (OD[Osc].PMDepth / 100) * Pi2 * OscArray[OD[Osc].PhaseFromOsc][t] / Form1.MaxAmplitude; // +- 2 * Pi
						}

						// Frekvensmodulering
						if (OD[Osc].FrequencyFromOsc != -1) {
							x = Math.Pow(Math.Sqrt(2), (OscArray[OD[Osc].FrequencyFromOsc][t] / Form1.MaxAmplitude) * 2.0 * (OD[Osc].FMDepth / 100) + 2.0);
							y = OD[Osc].Frequency / 2.0;
							PhaseFM = PhaseFM + (Pi2 / Form1.Samplerate) * (x * y - OD[Osc].Frequency);
							if (double.IsInfinity(PhaseFM)) {
								PhaseFM = 0.0;
							}
						}

						// Syncmodulering
						if (OD[Osc].SyncFromOsc != -1) {
							SyncAmp = OscArray[OD[Osc].SyncFromOsc][t];
							if (OldSyncAmp < 0.0 && SyncAmp > 0.0) {
								Phase = StartPhase;
							}
							OldSyncAmp = SyncAmp;
						}

						Phase = Phase + (Pi2 / Form1.Samplerate) * OD[Osc].Frequency;

						//Beräkna envelop
						AmpEnv = env.process();
						if (t - (int)DelayStart > OD[Osc].GateTime * Form1.Samplerate / 1000) env.gate(false);

						//Beräkna total amplitud
						AmpNow = AmpEnv * AmpMod * A;

						//Square Duty
						SquareDuty = OD[Osc].SquareDuty / 100.0;
						if (OD[Osc].SquareDutyFromOsc != -1) {
							SquareDuty = ((OD[Osc].SDDepth / 100.0) * OscArray[OD[Osc].SquareDutyFromOsc][t] / (Form1.MaxAmplitude * 2.0)) + (OD[Osc].SquareDuty / 100.0);
						}


						// Vågform
						switch (OD[Osc].WaveType) {
							case 1:
								Value = AmpNow * Math.Sin(PhaseNow);
								break;

							case 2:
								Value = AmpNow * GetTri(PhaseNow);
								break;

							case 3:
								Value = AmpNow * GetSaw(PhaseNow);
								break;

							case 4:
								Value = AmpNow * GetSqr(PhaseNow, SquareDuty);
								break;

							case 5:
								Value = AmpNow * GetNoice(PhaseNow, Rnd);
								break;

							case 6:
								Value = AmpNow * GetXX(PhaseNow);
								break;

							case 7:
								Value = AmpNow * Get1X(PhaseNow);
								break;

							case 8:
								Value = AmpNow * GetRNDWave(PhaseNow, OD[Osc]);
								break;

							case 9:
								Value = AmpNow * GetRealSound(RealSoundProgress, OldRealSoundProgress, RealSoundBuffer1);
								OldRealSoundProgress = RealSoundProgress;
								break;

							case 10:
								Value = AmpNow * GetRealSound(RealSoundProgress, OldRealSoundProgress, RealSoundBuffer2);
								OldRealSoundProgress = RealSoundProgress;
								break;

							case 11:
								Value = AmpNow * GetRealSound(RealSoundProgress, OldRealSoundProgress, RealSoundBuffer3);
								OldRealSoundProgress = RealSoundProgress;
								break;

							case 12:
								Value = AmpNow * GetRealSound(RealSoundProgress, OldRealSoundProgress, RealSoundBuffer4);
								OldRealSoundProgress = RealSoundProgress;
								break;

							case 13:
								Value = AmpNow * 1.0;
								break;

							case 14:
								Value = AmpNow * -1.0;
								break;

							default:
								Value = AmpNow * 1.0;
								break;
						}

						// Ringmodulering
						if (OD[Osc].RingModulationFromOsc != -1) {
							Value = Value * OscArray[OD[Osc].RingModulationFromOsc][t] / Form1.MaxAmplitude;
						}
						// Inte bra!
						if (double.IsNaN(Value)) {
							Value = 0.0;
						}
						//Oscillator bitupplösning
						if (OD[Osc].OscBitResolution != 0) {
							Value = (Math.Truncate((double)((int)Value / (1 << OD[Osc].OscBitResolution)))) * (double)(1 << OD[Osc].OscBitResolution);
						}
						//Global bitupplösning
						if (MD.BitResolution != 0) {
							Value = (Math.Truncate((double)((int)Value / (1 << MD.BitResolution)))) * (double)(1 << MD.BitResolution);
						}
						OscArray[Osc][t] = Value;
					}
				}
			}

			return OscArray;
		}

		private double GetTri( double phaseNow ) {
			double Value = 0.0;
			double x = phaseNow / Pi2;
			if (x >= 0.00 && x < 0.25) Value = x * 4.0;
			if (x >= 0.25 && x < 0.75) Value = 1.0 - (x - 0.25) * 4.0;
			if (x >= 0.75 && x < 1.00) Value = -1.0 + (x - 0.75) * 4.0;
			return Value;
		}

		private double GetSaw( double phaseNow ) {
			double Value = 0.0;
			double x =  phaseNow / Pi2;
			if (x >= 0.00 && x < 0.50) Value = x * 2.0;
			if (x >= 0.50 && x < 1.00) Value = -1.0 + (x - 0.5) * 2.0;
			return Value;
		}

		private double GetSqr( double phaseNow, double SqrDuty ) {
			double Value = -1.0;
			if (phaseNow < Pi2 * SqrDuty) Value = 1.0;
			return Value;
		}

		private double GetNoice( double phaseNow, Random Rnd ) {
			double Value;
			if (phaseNow < Pi && !FirstPart) {
				OldRandomValue = Rnd.NextDouble() * 2.0 - 1.0;
				FirstPart = true;
			}
			if (phaseNow >= Pi && FirstPart) {
				OldRandomValue = Rnd.NextDouble() * 2.0 - 1.0;
				FirstPart = false;
			}
			Value = OldRandomValue;
			return Value;
		}

		private double GetXX( double phaseNow ) {
			double Value = (phaseNow / Pi - 1.0);
			Value = Value * Value;
			Value = 2 * Value - 1.0;
			return Value;
		}

		private double Get1X( double phaseNow ) {
			double Sharpness = 0.5;
			double Value = phaseNow / Pi2 + Sharpness;
			Value = 1 / Value;
			Value = 2 * (Value / (1 / Sharpness - 1 / (Sharpness + 1)) - Sharpness) - 1.0;
			return Value;
		}

		private double GetRNDWave( double phaseNow, OscillatorData OD ) {
			double Value;
			double a = 0.0;
			double b = 0.0;
			double c = 0.0;

			double Frequency = OD.Frequency;

			// 14080 7040 3220 1760 880 440 220 110 55
			Value = Math.Sin(phaseNow);
			if (Frequency < 7040.0) {
				Value = Value + Math.Sin(OD.RndPA + phaseNow * 2.0) * OD.RndA;
				a = 1.0;
			}
			if (Frequency < 4693.333333333) {
				Value = Value + Math.Sin(OD.RndPB + phaseNow * 3.0) * OD.RndB;
				b = 1.0;
			}
			if (Frequency < 3220.0) {
				Value = Value + Math.Sin(OD.RndPC + phaseNow * 4.0) * OD.RndC;
				c = 1.0;
			}

			Value = Value / (1.0 + OD.RndA * a + OD.RndB * b + OD.RndC * c);
			return Value;
		}

		public double GetRealSound( double NewProgress, double OldProgress, short[] RealSoundBuffer ) {
			double Value = 0.0;
			double Step = 0.0;

			Step = (NewProgress - OldProgress) * Form1.Samplerate;

			NewProgress *= Form1.Samplerate;
			OldProgress *= Form1.Samplerate;
			if (NewProgress < RealSoundBuffer.Length - 1) {
				if (Step < 1.0) {
					Value = (RealSoundBuffer[(int)NewProgress] - RealSoundBuffer[(int)NewProgress + 1]) * (1 - (NewProgress - (int)NewProgress)) + RealSoundBuffer[(int)NewProgress + 1];
				} else {
					Value = RealSoundBuffer[(int)NewProgress];
				}
			}
			return Value / Form1.MaxAmplitude;
		}

		public double GetRealSoundToDisplay( double NewProgress, double OldProgress, short[] RealSoundBuffer ) {
			double Value = 0.0;
			double Step = 0.0;

			Step = (NewProgress - OldProgress) * Form1.Samplerate;

			NewProgress *= Form1.Samplerate;
			OldProgress *= Form1.Samplerate;
			if (NewProgress < RealSoundBuffer.Length - 1) {
				if (Step < 1.0) {
					Value = (RealSoundBuffer[(int)NewProgress] - RealSoundBuffer[(int)NewProgress + 1]) * (1 - (NewProgress - (int)NewProgress)) + RealSoundBuffer[(int)NewProgress + 1];
				} else {
					Value = RealSoundBuffer[(int)NewProgress];
				}
			}
			return Value / Form1.MaxAmplitude;
		}

		public OscillatorData CreateRandomSinus( OscillatorData OD ) {
			Random Rnd = new Random();
			OD.RndA = Rnd.NextDouble();
			OD.RndB = Rnd.NextDouble();
			OD.RndC = Rnd.NextDouble();

			OD.RndPA = Rnd.NextDouble() * Pi2;
			OD.RndPB = Rnd.NextDouble() * Pi2;
			OD.RndPC = Rnd.NextDouble() * Pi2;

			return OD;
		}

		public void ShowWaveForm( PictureBox pb, OscillatorData OD ) {
			Random Rnd = new Random();
			double WavesOnScreen = 3.0;

			double a;
			double P;
			double Amp = OD.Volume / 2.0;

			double PhaseNow;
			double RealSoundProgress = 0.0;
			double OldRealSoundProgress = 0.0;

			double oldx = 0.0;
			double oldy = 0.0;

			int x = pb.Size.Width;
			int y = pb.Size.Height;

			Bitmap bm = new Bitmap(x, y);
			Graphics g = Graphics.FromImage(bm);
			g.Clear(Color.Black);
			Pen PenGreen = new Pen(Color.FromArgb(0x80,0x80,0xff,0x80));
			Pen PenWhite = new Pen(Color.FromArgb(0xff,0xc0,0xc0,0xc0));

			g.DrawLine(PenWhite, 0, y / 2, x, y / 2);

			for (double i = 0.0; i < Pi2 * WavesOnScreen; i = i + (1.0 / x)) {
				P = OD.StartPhase / 360.0;
				PhaseNow = (P + i - (int)(P + i)) * Pi2; //Wrap till 0 - 2*pi
				RealSoundProgress = P + i;
                switch (OD.WaveType) {
					case 1:
						a = -Amp * Math.Sin(PhaseNow);
						break;

					case 2:
						a = -Amp * GetTri(PhaseNow);
						break;

					case 3:
						a = -Amp * GetSaw(PhaseNow);
						break;

					case 4:
						a = -Amp * GetSqr(PhaseNow, OD.SquareDuty / 100.0);
						break;

					case 5:
						a = -Amp * GetNoice(PhaseNow, Rnd);
						break;

					case 6:
						a = -Amp * GetXX(PhaseNow);
						break;

					case 7:
						a = -Amp * Get1X(PhaseNow);
						break;

					case 8:
						a = -Amp * GetRNDWave(PhaseNow, OD);
						break;

					case 9:
						a = -Amp * GetRealSoundToDisplay (RealSoundProgress, OldRealSoundProgress, RealSoundBuffer1);
						OldRealSoundProgress = RealSoundProgress;

						break;

					case 10:
						a = -Amp * GetRealSoundToDisplay(RealSoundProgress, OldRealSoundProgress, RealSoundBuffer2);
						OldRealSoundProgress = RealSoundProgress;
						break;

					case 11:
						a = -Amp * GetRealSoundToDisplay(RealSoundProgress, OldRealSoundProgress, RealSoundBuffer3);
						OldRealSoundProgress = RealSoundProgress;
						break;

					case 12:
						a = -Amp * GetRealSoundToDisplay(RealSoundProgress, OldRealSoundProgress, RealSoundBuffer4);
						OldRealSoundProgress = RealSoundProgress;
						break;

					case 13:
						a = -Amp * 1.0;
						break;

					case 14:
						a = -Amp * -1.0;
						break;

					default:
						a = -Amp * 1.0;
						break;
				}
				if (a > Form1.MaxAmplitude) a = Form1.MaxAmplitude;
				if (a <= -Form1.MaxAmplitude) a = -Form1.MaxAmplitude;

				g.DrawLine(PenGreen, (float)oldx, (float)oldy, (float)(i * x / WavesOnScreen), (float)((y - 4.0) / 2.0 + a * ((y - 4.0) / 2.0) / Form1.MaxAmplitude + 2.0));
				oldx = i * x / WavesOnScreen;
				oldy = ((y - 4.0) / 2.0 + a * ((y - 4.0) / 2.0) / Form1.MaxAmplitude) + 2.0;
			}
			pb.Image = bm;
		}












		public OscillatorData[] CreateRandomSoundCrazy( OscillatorData[] OD, RndS RS ) {
			double Pi2 = Math.PI * 2;
			Random Rnd = new Random();

			for (int i = 0; i < RS.NoRandomOsc + RS.NoRandomLFO; i++) {
				OD[i].Active = true;
				OD[i].SoundOut = (RS.NoRandomLFO <= i ? true : false);

				OD[i].WaveType = Rnd.Next(0, 8 + 1);
				OD[i].SquareDuty = Rnd.Next(5, 95);
				OD[i].StartPhase = 0.0;
				OD[i].Volume = Rnd.Next(4096, 65536);
				OD[i].Attack = Rnd.Next(0, 20);
				OD[i].Decay = Rnd.Next(200, 2000);
				OD[i].Sustain = Rnd.Next(8192, 65536);
				OD[i].Release = Rnd.Next(2000, 4000);
				OD[i].GateTime = (RS.NoRandomLFO <= i ? Rnd.Next(4000, 4500) : Form1.TimeMS);
				OD[i].Delay = 0.0;
				OD[i].Frequency = (RS.NoRandomLFO <= i ? Rnd.Next((int)RS.MinRandomFrequency, (int)RS.MaxRandomFrequency) : Rnd.Next(1, 20));
				if (Rnd.Next(0, 5) == 0)
					OD[i].VolumeFromOsc = Rnd.Next(-1, i);
				OD[i].AMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].FrequencyFromOsc = Rnd.Next(-1, i);
				OD[i].FMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].PhaseFromOsc = Rnd.Next(-1, i);
				OD[i].PMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].SquareDutyFromOsc = Rnd.Next(-1, i);
				OD[i].SDDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].RingModulationFromOsc = Rnd.Next(-1, i);
				if (Rnd.Next(0, 5) == 0)
					OD[i].SyncFromOsc = Rnd.Next(-1, i);

				OD[i].RndA = Rnd.NextDouble();
				OD[i].RndB = Rnd.NextDouble();
				OD[i].RndC = Rnd.NextDouble();

				OD[i].RndPA = Rnd.NextDouble() * Pi2;
				OD[i].RndPB = Rnd.NextDouble() * Pi2;
				OD[i].RndPC = Rnd.NextDouble() * Pi2;
			}
			return OD;
		}

		public OscillatorData[] CreateRandomSoundThis( OscillatorData[] OD, int Osc ) {
			double Pi2 = Math.PI * 2;
			Random Rnd = new Random();
			int i = Osc;

			OD[i].Active = true;
			OD[i].SoundOut = true;

			OD[i].WaveType = Rnd.Next(0, 8 + 1);
			OD[i].SquareDuty = Rnd.Next(5, 95);
			OD[i].StartPhase = (i == 0 ? 0 : Rnd.Next(0, 180));
			OD[i].Volume = Rnd.Next(32768, 65536);
			OD[i].Attack = Rnd.Next(0, 350);
			OD[i].Decay = Rnd.Next(200, 2000);
			OD[i].Sustain = Rnd.Next(8192, 65536);
			OD[i].Release = Rnd.Next(200, 2000);
			OD[i].GateTime = Rnd.Next(500, 3500);
			OD[i].Delay = (i == 0 ? 0 : Rnd.Next(0, 100));
			OD[i].Frequency = Rnd.Next(1, 2000);
			if (Rnd.Next(0, 5) == 0)
				OD[i].VolumeFromOsc = Rnd.Next(-1, i);
			OD[i].AMDepth = Rnd.Next(10, 150);
			if (Rnd.Next(0, 5) == 0)
				OD[i].FrequencyFromOsc = Rnd.Next(-1, i);
			OD[i].FMDepth = Rnd.Next(10, 150);
			if (Rnd.Next(0, 5) == 0)
				OD[i].PhaseFromOsc = Rnd.Next(-1, i);
			OD[i].PMDepth = Rnd.Next(10, 150);
			if (Rnd.Next(0, 5) == 0)
				OD[i].SquareDutyFromOsc = Rnd.Next(-1, i);
			OD[i].SDDepth = Rnd.Next(10, 150);
			if (Rnd.Next(0, 5) == 0)
				OD[i].RingModulationFromOsc = Rnd.Next(-1, i);
			if (Rnd.Next(0, 5) == 0)
				OD[i].SyncFromOsc = Rnd.Next(-1, i);

			OD[i].RndA = Rnd.NextDouble();
			OD[i].RndB = Rnd.NextDouble();
			OD[i].RndC = Rnd.NextDouble();

			OD[i].RndPA = Rnd.NextDouble() * Pi2;
			OD[i].RndPB = Rnd.NextDouble() * Pi2;
			OD[i].RndPC = Rnd.NextDouble() * Pi2;

			return OD;
		}

		public OscillatorData[] CreateRandomSoundShort( OscillatorData[] OD, RndS RS ) {
			double Pi2 = Math.PI * 2;
			Random Rnd = new Random();

			for (int i = 0; i < RS.NoRandomOsc + RS.NoRandomLFO; i++) {
				OD[i].Active = true;
				OD[i].SoundOut = (RS.NoRandomLFO <= i ? true : false);

				OD[i].WaveType = Rnd.Next(0, 8 + 1);
				OD[i].SquareDuty = Rnd.Next(5, 95);
				OD[i].StartPhase = 0;
				OD[i].Volume = Rnd.Next(4096, 65536);
				OD[i].Attack = Rnd.Next(0, 10);
				OD[i].Decay = Rnd.Next(100, 500);
				OD[i].Sustain = 0.0;
				OD[i].Release = Rnd.Next(10, 500);
				OD[i].GateTime = Form1.TimeMS;
				OD[i].Delay = 0;
				OD[i].Frequency = (RS.NoRandomLFO <= i ? Rnd.Next((int)RS.MinRandomFrequency, (int)RS.MaxRandomFrequency) : Rnd.Next(1, 20));
				if (Rnd.Next(0, 5) == 0)
					OD[i].VolumeFromOsc = Rnd.Next(-1, i);
				OD[i].AMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].FrequencyFromOsc = Rnd.Next(-1, i);
				OD[i].FMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].PhaseFromOsc = Rnd.Next(-1, i);
				OD[i].PMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].SquareDutyFromOsc = Rnd.Next(-1, i);
				OD[i].SDDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].RingModulationFromOsc = Rnd.Next(-1, i);
				if (Rnd.Next(0, 5) == 0)
					OD[i].SyncFromOsc = Rnd.Next(-1, i);

				OD[i].RndA = Rnd.NextDouble();
				OD[i].RndB = Rnd.NextDouble();
				OD[i].RndC = Rnd.NextDouble();

				OD[i].RndPA = Rnd.NextDouble() * Pi2;
				OD[i].RndPB = Rnd.NextDouble() * Pi2;
				OD[i].RndPC = Rnd.NextDouble() * Pi2;
			}
			return OD;
		}

		public OscillatorData[] CreateRandomSoundMedium( OscillatorData[] OD, RndS RS ) {
			double Pi2 = Math.PI * 2;
			Random Rnd = new Random();

			for (int i = 0; i < RS.NoRandomOsc + RS.NoRandomLFO; i++) {
				OD[i].Active = true;
				OD[i].SoundOut = (RS.NoRandomLFO <= i ? true : false);

				OD[i].WaveType = Rnd.Next(0, 8 + 1);
				OD[i].SquareDuty = Rnd.Next(5, 95);
				OD[i].StartPhase = 0;
				OD[i].Volume = Rnd.Next(4096, 65536);
				OD[i].Attack = Rnd.Next(0, 20);
				OD[i].Decay = Rnd.Next(750, 2500);
				OD[i].Sustain = 0.0;
				OD[i].Release = 0.0;
				OD[i].GateTime = Form1.TimeMS;
				OD[i].Delay = 0;
				OD[i].Frequency = (RS.NoRandomLFO <= i ? Rnd.Next((int)RS.MinRandomFrequency, (int)RS.MaxRandomFrequency) : Rnd.Next(1, 20));
				if (Rnd.Next(0, 5) == 0)
					OD[i].VolumeFromOsc = Rnd.Next(-1, i);
				OD[i].AMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].FrequencyFromOsc = Rnd.Next(-1, i);
				OD[i].FMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].PhaseFromOsc = Rnd.Next(-1, i);
				OD[i].PMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].SquareDutyFromOsc = Rnd.Next(-1, i);
				OD[i].SDDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].RingModulationFromOsc = Rnd.Next(-1, i);
				if (Rnd.Next(0, 5) == 0)
					OD[i].SyncFromOsc = Rnd.Next(-1, i);

				OD[i].RndA = Rnd.NextDouble();
				OD[i].RndB = Rnd.NextDouble();
				OD[i].RndC = Rnd.NextDouble();

				OD[i].RndPA = Rnd.NextDouble() * Pi2;
				OD[i].RndPB = Rnd.NextDouble() * Pi2;
				OD[i].RndPC = Rnd.NextDouble() * Pi2;
			}
			return OD;
		}

		public OscillatorData[] CreateRandomSoundSameADSR( OscillatorData[] OD, RndS RS ) {

			double Pi2 = Math.PI * 2;
			Random Rnd = new Random();

			double A = Rnd.Next(0, 200);
			double D = Rnd.Next(250, 1000);
			double S = Rnd.Next(8192, 65536);
			double R = Rnd.Next(250, 1000);
			double G = Rnd.Next(300, 1000); 

			for (int i = 0; i < RS.NoRandomOsc + RS.NoRandomLFO; i++) {
				OD[i].Active = true;
				OD[i].SoundOut = (RS.NoRandomLFO <= i ? true : false);

				OD[i].WaveType = Rnd.Next(0, 8 + 1);
				OD[i].SquareDuty = Rnd.Next(5, 95);
				OD[i].StartPhase = 0;
				OD[i].Volume = Rnd.Next(32768, 65536);
				OD[i].Attack = A;
				OD[i].Decay = D;
				OD[i].Sustain = S;
				OD[i].Release = R;
				OD[i].GateTime = G;
				OD[i].Delay = 0.0;
				OD[i].Frequency = (RS.NoRandomLFO <= i ? Rnd.Next((int)RS.MinRandomFrequency, (int)RS.MaxRandomFrequency) : Rnd.Next(1, 20));
				if (Rnd.Next(0, 5) == 0)
					OD[i].VolumeFromOsc = Rnd.Next(-1, i);
				OD[i].AMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].FrequencyFromOsc = Rnd.Next(-1, i);
				OD[i].FMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].PhaseFromOsc = Rnd.Next(-1, i);
				OD[i].PMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].SquareDutyFromOsc = Rnd.Next(-1, i);
				OD[i].SDDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].RingModulationFromOsc = Rnd.Next(-1, i);
				if (Rnd.Next(0, 5) == 0)
					OD[i].SyncFromOsc = Rnd.Next(-1, i);

				OD[i].RndA = Rnd.NextDouble();
				OD[i].RndB = Rnd.NextDouble();
				OD[i].RndC = Rnd.NextDouble();

				OD[i].RndPA = Rnd.NextDouble() * Pi2;
				OD[i].RndPB = Rnd.NextDouble() * Pi2;
				OD[i].RndPC = Rnd.NextDouble() * Pi2;
			}
			return OD;
		}
		public OscillatorData[] CreateRandomSoundSin( OscillatorData[] OD, RndS RS ) {

			double Pi2 = Math.PI * 2;
			Random Rnd = new Random();

			double A = Rnd.Next(0, 200);
			double D =  Rnd.Next(250, 1000);
			double S = Rnd.Next(16384, 65536);
			double R = Rnd.Next(250, 1000);


			for (int i = 0; i < RS.NoRandomOsc + RS.NoRandomLFO; i++) {
				OD[i].Active = true;
				OD[i].SoundOut = (RS.NoRandomLFO <= i ? true : false);

				OD[i].WaveType = 1;
				OD[i].SquareDuty = Rnd.Next(5, 95);
				OD[i].StartPhase = (i == 0 ? 0 : Rnd.Next(0, 180));
				OD[i].Volume = Rnd.Next(32768, 65536);
				OD[i].Attack = A;
				OD[i].Decay = D;
				OD[i].Sustain = S;
				OD[i].Release = R;
				OD[i].GateTime = (RS.NoRandomLFO <= i ? Rnd.Next(300, 1000) : Form1.TimeMS);
				OD[i].Delay = 0.0; //(i - RS.NoRandomLFO) < 0 ? 0.0 : (i - RS.NoRandomLFO) * 250;
				OD[i].Frequency = (RS.NoRandomLFO <= i ? Rnd.Next((int)RS.MinRandomFrequency, (int)RS.MaxRandomFrequency) : Rnd.Next(1, 20));
				if (Rnd.Next(0, 5) == 0)
					OD[i].VolumeFromOsc = Rnd.Next(-1, i);
				OD[i].AMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].FrequencyFromOsc = Rnd.Next(-1, i);
				OD[i].FMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].PhaseFromOsc = Rnd.Next(-1, i);
				OD[i].PMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].SquareDutyFromOsc = Rnd.Next(-1, i);
				OD[i].SDDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].RingModulationFromOsc = Rnd.Next(-1, i);
				if (Rnd.Next(0, 5) == 0)
					OD[i].SyncFromOsc = Rnd.Next(-1, i);

				OD[i].RndA = Rnd.NextDouble();
				OD[i].RndB = Rnd.NextDouble();
				OD[i].RndC = Rnd.NextDouble();

				OD[i].RndPA = Rnd.NextDouble() * Pi2;
				OD[i].RndPB = Rnd.NextDouble() * Pi2;
				OD[i].RndPC = Rnd.NextDouble() * Pi2;
			}
			return OD;
		}

		public OscillatorData[] CreateRandomSoundSqr( OscillatorData[] OD, RndS RS ) {

			double Pi2 = Math.PI * 2;
			Random Rnd = new Random();

			double A = Rnd.Next(0, 200);
			double D =  Rnd.Next(250, 1000);
			double S = Rnd.Next(16384, 65536);
			double R = Rnd.Next(250, 1000);


			for (int i = 0; i < RS.NoRandomOsc + RS.NoRandomLFO; i++) {
				OD[i].Active = true;
				OD[i].SoundOut = (RS.NoRandomLFO <= i ? true : false);

				OD[i].WaveType = (RS.NoRandomLFO <= i ? 4 : Rnd.Next(0, 8 + 1));
				OD[i].SquareDuty = Rnd.Next(5, 95);
				OD[i].StartPhase = (i == 0 ? 0 : Rnd.Next(0, 180));
				OD[i].Volume = Rnd.Next(32768, 65536);
				OD[i].Attack = A;
				OD[i].Decay = D;
				OD[i].Sustain = S;
				OD[i].Release = R;
				OD[i].GateTime = (RS.NoRandomLFO <= i ? Rnd.Next(300, 1000) : Form1.TimeMS);
				OD[i].Delay = 0.0; //(i - RS.NoRandomLFO) < 0 ? 0.0 : (i - RS.NoRandomLFO) * 250;
				OD[i].Frequency = (RS.NoRandomLFO <= i ? Rnd.Next((int)RS.MinRandomFrequency, (int)RS.MaxRandomFrequency) : Rnd.Next(1, 20));
				if (Rnd.Next(0, 5) == 0)
					OD[i].VolumeFromOsc = Rnd.Next(-1, i);
				OD[i].AMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].FrequencyFromOsc = Rnd.Next(-1, i);
				OD[i].FMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].PhaseFromOsc = Rnd.Next(-1, i);
				OD[i].PMDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 2) == 0)
					OD[i].SquareDutyFromOsc = Rnd.Next(-1, i);
				OD[i].SDDepth = Rnd.Next(10, 150);
				if (Rnd.Next(0, 5) == 0)
					OD[i].RingModulationFromOsc = Rnd.Next(-1, i);
				if (Rnd.Next(0, 5) == 0)
					OD[i].SyncFromOsc = Rnd.Next(-1, i);

				OD[i].RndA = Rnd.NextDouble();
				OD[i].RndB = Rnd.NextDouble();
				OD[i].RndC = Rnd.NextDouble();

				OD[i].RndPA = Rnd.NextDouble() * Pi2;
				OD[i].RndPB = Rnd.NextDouble() * Pi2;
				OD[i].RndPC = Rnd.NextDouble() * Pi2;
			}
			return OD;
		}

		public OscillatorData[] Jingle( OscillatorData[] OD, string[] Jingle, string[] OkNotes, bool NewJingle ) {

			OscillatorData[] OscData = new OscillatorData[Form1.MaxOscillators];

			string[] Length = new string[5];

			Length[0] = "1024"; //		   1/1
			Length[1] = " 512"; //         1/2
			Length[2] = " 256"; // * 2     1/4
			Length[3] = " 128"; // * 4     1/8
			Length[4] = "  64"; // * 8     1/16

			int MaxNoNotes = 0;
			int MaxUsedOsc = 0;

			int MaxO = Form1.MaxOscillators;

			int NoteNr = 0;
			double F = 1;
			double Delay = 0;
			string NoteName;
			Random Rnd;
			bool Ok;

			// Skapa användbara takter

			Rnd = new Random();
			int[,] Takt = {
				{1,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{2,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{3,3,2,1,0,0,0,0,0,0,0,0,0,0,0,0 },
				{1,3,3,2,0,0,0,0,0,0,0,0,0,0,0,0 },
				{2,2,3,3,3,3,0,0,0,0,0,0,0,0,0,0 },
				{3,3,2,2,3,3,0,0,0,0,0,0,0,0,0,0 },
				{3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0 },
				{3,3,3,3,2,2,0,0,0,0,0,0,0,0,0,0 },
				{2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0 },
				{2,3,3,2,3,3,0,0,0,0,0,0,0,0,0,0 },
				{1,3,3,3,3,0,0,0,0,0,0,0,0,0,0,0 },
				{3,3,3,3,3,3,3,3,0,0,0,0,0,0,0,0 },
				{1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4 },
				{3,3,2,3,3,2,0,0,0,0,0,0,0,0,0,0 },
				{3,3,2,2,2,0,0,0,0,0,0,0,0,0,0,0 }
			};
			if (NewJingle || Jingle == null) {
				rndTakt = Rnd.Next(0, 16);  // rndTakt = 0 till 15
			}
			//Hur många noter behövs för en takt
			int NotesiTakt = 16;
			for (int i = 0; i < 16; i++) {
				if (Takt[rndTakt, i] == 0) {
					NotesiTakt = i;
					break;
				}
			}
			// Hur många röster används till ett ljud, max.
			for (int i = 0; i < MaxO; i++) {
				if (OD[i].Active) MaxUsedOsc = i + 1;
			}

			MaxNoNotes = MaxO / MaxUsedOsc;
			if (MaxNoNotes > NotesiTakt) MaxNoNotes = NotesiTakt;

			// Kopiera alla noter och rätta till kopplingen mellan oscillatorer
			for (int i = 0; i < MaxNoNotes; i++) {
				for (int j = 0; j < MaxUsedOsc; j++) {
					OscData[i * MaxUsedOsc + j] = OD[j];
					if (OD[j].FrequencyFromOsc != -1) OscData[i * MaxUsedOsc + j].FrequencyFromOsc += i * MaxUsedOsc;
					if (OD[j].PhaseFromOsc != -1) OscData[i * MaxUsedOsc + j].PhaseFromOsc += i * MaxUsedOsc;
					if (OD[j].RingModulationFromOsc != -1) OscData[i * MaxUsedOsc + j].RingModulationFromOsc += i * MaxUsedOsc;
					if (OD[j].SquareDutyFromOsc != -1) OscData[i * MaxUsedOsc + j].SquareDutyFromOsc += i * MaxUsedOsc;
					if (OD[j].SyncFromOsc != -1) OscData[i * MaxUsedOsc + j].SyncFromOsc += i * MaxUsedOsc;
					if (OD[j].VolumeFromOsc != -1) OscData[i * MaxUsedOsc + j].VolumeFromOsc += i * MaxUsedOsc;
				}
			}



			// Skapa random Trudelutt eller läs strängen
			Rnd = new Random();
			Ok = false;
			string str1,str2;
			int r;
			strJingle = new string[Form1.MaxOscillators * 2];
			if (NewJingle || Jingle == null) {
				Jingle = new string[Form1.MaxOscillators * 2];
				if (OkNotes[0] == "" || OkNotes[0] == null) OkNotes[0] = "A4 ";
				for (int i = 0; i < MaxNoNotes; i++) {
					while (!Ok) {
						r = Rnd.Next(0, 108);
						str1 = Notes[r].Name1 + " ";   // Ton
						str2 = Notes[r].Name2 + " ";   // Ton
						for (int j = 0; j < OkNotes.Length; j++) {
							if (str1 == OkNotes[j] + " ") {
								Ok = true;
								Jingle[i * 2] = str1;
								break;
							}
							if (str2 == OkNotes[j] + " ") {
								Ok = true;
								Jingle[i * 2] = str2;
								break;
							}
						}
					}

					// 2,2,3,3,2
					Jingle[i * 2 + 1] = Length[Takt[rndTakt, i]];            // längd i millisekunder 0-5 512,256,128,64,32
					Ok = false;
				}
			}

			Array.Copy(Jingle, strJingle, Jingle.Length);

			for (int i = 0; i < MaxNoNotes; i++) {


				// Hitta noten i Arrayen
				NoteName = strJingle[i * 2];
				for (int k = 0; k < Notes.Length; k++) {
					if (NoteName == Notes[k].Name1 + " " || NoteName == Notes[k].Name2 + " ") {
						NoteNr = k;
						break;
					}
				}

				// Hitta första oscillatorn som skall vara grundtonen
				for (int j = 0; j < MaxUsedOsc; j++) {
					// Antag att LFO är under 20Hz. Det duger.?
					if (OscData[i * MaxUsedOsc + j].Frequency > 20) {
						F = Notes[NoteNr].Frequency / OscData[i * MaxUsedOsc + j].Frequency;
						break;
					}
				}

				for (int j = 0; j < MaxUsedOsc; j++) {
					if (OscData[i * MaxUsedOsc + j].Frequency > 20) {
						OscData[i * MaxUsedOsc + j].Frequency *= F;
					}
					OscData[i * MaxUsedOsc + j].Delay += Delay;
				}
				Delay = Delay + Convert.ToDouble(strJingle[i * 2 + 1]);
			}
			return OscData;
		}

		private void CreatePianoNotes() {
			Notes = new Note[12 * 9];
			double C = 13.75 * Math.Pow(2, 3.0 / 12.0);

			for (int Oktav = 0; Oktav < 9; Oktav++) {
				Notes[Oktav * 12 + 0].Frequency = C * Math.Pow(2, (Oktav * 12 + 0.0) / 12.0);
				Notes[Oktav * 12 + 0].Name1 = "C" + Oktav.ToString() + " ";
				Notes[Oktav * 12 + 0].Name2 = "C" + Oktav.ToString() + " ";
				Notes[Oktav * 12 + 1].Frequency = C * Math.Pow(2, (Oktav * 12 + 1.0) / 12.0);
				Notes[Oktav * 12 + 1].Name1 = "C" + Oktav.ToString() + "#";
				Notes[Oktav * 12 + 1].Name2 = "D" + Oktav.ToString() + "b";
				Notes[Oktav * 12 + 2].Frequency = C * Math.Pow(2, (Oktav * 12 + 2.0) / 12.0);
				Notes[Oktav * 12 + 2].Name1 = "D" + Oktav.ToString() + " ";
				Notes[Oktav * 12 + 2].Name2 = "D" + Oktav.ToString() + " ";
				Notes[Oktav * 12 + 3].Frequency = C * Math.Pow(2, (Oktav * 12 + 3.0) / 12.0);
				Notes[Oktav * 12 + 3].Name1 = "D" + Oktav.ToString() + "#";
				Notes[Oktav * 12 + 3].Name2 = "E" + Oktav.ToString() + "b";
				Notes[Oktav * 12 + 4].Frequency = C * Math.Pow(2, (Oktav * 12 + 4.0) / 12.0);
				Notes[Oktav * 12 + 4].Name1 = "E" + Oktav.ToString() + " ";
				Notes[Oktav * 12 + 4].Name2 = "E" + Oktav.ToString() + " ";
				Notes[Oktav * 12 + 5].Frequency = C * Math.Pow(2, (Oktav * 12 + 5.0) / 12.0);
				Notes[Oktav * 12 + 5].Name1 = "F" + Oktav.ToString() + " ";
				Notes[Oktav * 12 + 5].Name2 = "F" + Oktav.ToString() + " ";
				Notes[Oktav * 12 + 6].Frequency = C * Math.Pow(2, (Oktav * 12 + 6.0) / 12.0);
				Notes[Oktav * 12 + 6].Name1 = "F" + Oktav.ToString() + "#";
				Notes[Oktav * 12 + 6].Name2 = "G" + Oktav.ToString() + "b";
				Notes[Oktav * 12 + 7].Frequency = C * Math.Pow(2, (Oktav * 12 + 7.0) / 12.0);
				Notes[Oktav * 12 + 7].Name1 = "G" + Oktav.ToString() + " ";
				Notes[Oktav * 12 + 7].Name2 = "G" + Oktav.ToString() + " ";
				Notes[Oktav * 12 + 8].Frequency = C * Math.Pow(2, (Oktav * 12 + 8.0) / 12.0);
				Notes[Oktav * 12 + 8].Name1 = "G" + Oktav.ToString() + "#";
				Notes[Oktav * 12 + 8].Name2 = "A" + Oktav.ToString() + "b";
				Notes[Oktav * 12 + 9].Frequency = C * Math.Pow(2, (Oktav * 12 + 9.0) / 12.0);
				Notes[Oktav * 12 + 9].Name1 = "A" + Oktav.ToString() + " ";
				Notes[Oktav * 12 + 9].Name2 = "A" + Oktav.ToString() + " ";
				Notes[Oktav * 12 + 10].Frequency = C * Math.Pow(2, (Oktav * 12 + 10.0) / 12.0);
				Notes[Oktav * 12 + 10].Name1 = "A" + Oktav.ToString() + "#";
				Notes[Oktav * 12 + 10].Name2 = "B" + Oktav.ToString() + "b";
				Notes[Oktav * 12 + 11].Frequency = C * Math.Pow(2, (Oktav * 12 + 11.0) / 12.0);
				Notes[Oktav * 12 + 11].Name1 = "B" + Oktav.ToString() + " ";
				Notes[Oktav * 12 + 11].Name2 = "B" + Oktav.ToString() + " ";

			}
		}

		public string[] GetStrJingle() {
			return strJingle;
		}

		public Note[] GetNotes() {
			return Notes;
		}

		public short[] GetRealSoundBuffer1() {
			return RealSoundBuffer1;
		}

		public short[] GetRealSoundBuffer2() {
			return RealSoundBuffer2;
		}

		public short[] GetRealSoundBuffer3() {
			return RealSoundBuffer3;
		}

		public short[] GetRealSoundBuffer4() {
			return RealSoundBuffer4;
		}


		public void SetRealSoundBuffer1( short[] i ) {
			RealSoundBuffer1 = i;
		}

		public void SetRealSoundBuffer2( short[] i ) {
			RealSoundBuffer2 = i;
		}

		public void SetRealSoundBuffer3( short[] i ) {
			RealSoundBuffer3 = i;
		}

		public void SetRealSoundBuffer4( short[] i ) {
			RealSoundBuffer4 = i;
		}

	}
}