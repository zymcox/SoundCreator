using System;
using System.IO;
using System.Media;

namespace SoundCreator {

	public struct MixerData {
		public double MasterVolume;
		public bool AGC;

		public bool Reverb;
		public double Delay1;
		public double Delay2;
		public double Delay3;
		public double Delay4;
		public double Delay5;
		public double Delay6;
		public double Delay7;
		public double Gain1;
		public double Gain2;
		public double Gain3;
		public double Gain4;
		public double Gain5;
		public double Gain6;
		public double Gain7;

		public int FilterType;
		public double FilterFrequency1;
		public double FilterFrequency2;
		public int FilterOrder;
		public int FilterFrequencyFromOsc;
		public double FFDepth;

		public bool MovingAverageFilter;
		public bool RemoveDC;

	}

	internal class Mixer {
		private Byte[] SoundDotWav;
		private double[] RawSoundData;

		public MixerData Reset( MixerData MD ) {
			MD.MasterVolume = 65536;
			MD.AGC = false;

			MD.Reverb = false;
			MD.Delay1 = 20;
			MD.Delay2 = 18;
			MD.Delay3 = 23;
			MD.Delay4 = 26;
			MD.Delay5 = 283;
			MD.Delay6 = 95;
			MD.Delay7 = 27;
			MD.Gain1 = 80;
			MD.Gain2 = 83;
			MD.Gain3 = 78;
			MD.Gain4 = 76;
			MD.Gain5 = 70;
			MD.Gain6 = 70;
			MD.Gain7 = 70;

			MD.FilterType = 0;
			MD.FilterFrequency1 = 220.0;
			MD.FilterFrequency2 = 230.0;
			MD.FilterOrder = 2;
			MD.FilterFrequencyFromOsc = -1;
			MD.FFDepth = 50.0;

			MD.MovingAverageFilter = false;
			MD.RemoveDC = false;

			return MD;
		}

		public void CreateSoundWav( MixerData MixData, OscillatorData[] OscData, double[][] OscArray ) {
			Filter objFilter = new Filter();
			double[] SoundArray = new double[Form1.OscArraySize];
			double[] EchoSoundArray = new double[Form1.OscArraySize + 1];
			Int16[] SoundArray16bit = new Int16[Form1.OscArraySize];
			Array.Clear(SoundArray, 0, SoundArray.Length);
			Byte[] WaveFile = new Byte[Form1.OscArraySize * 2 + 44];

			int UsedSoundingWaves = 0;

			// Addera ihop alla vågformer
			for (int i = 0; i < Form1.MaxOscillators; i++) {
				if (OscData[i].SoundOut && OscData[i].Active) {
					UsedSoundingWaves++;
					for (int j = 0; j < Form1.OscArraySize; j++) {
						SoundArray[j] = SoundArray[j] + OscArray[i][j];
					}
				}
			}

			if (UsedSoundingWaves == 0) return;

			//Addera alla oscillatorer
			for (int i = 0; i < Form1.OscArraySize; i++) {
				double x;
				x = SoundArray[i] / UsedSoundingWaves;
				if (x > Form1.MaxAmplitude - 1.0) x = Form1.MaxAmplitude - 1.0;
				if (x < -Form1.MaxAmplitude) x = -Form1.MaxAmplitude;
				SoundArray[i] = x;
			}

			// Filtrera allt
			//switch (MixData.FilterType) {
			//	case 1:
			//		objFilter.CreateFIRCoeffLP(Form1.Samplerate, MixData.FilterFrequency1);
			//		SoundArray = objFilter.FIRfilter(SoundArray);
			//		break;

			//	case 2:
			//		objFilter.CreateFIRCoeffHP(Form1.Samplerate, MixData.FilterFrequency2);
			//		SoundArray = objFilter.FIRfilter(SoundArray);
			//		break;

			//	case 3:
			//		objFilter.CreateFIRCoeffBP(Form1.Samplerate, MixData.FilterFrequency1, MixData.FilterFrequency2);
			//		SoundArray = objFilter.FIRfilter(SoundArray);
			//		break;

			//	case 4:
			//		objFilter.CreateFIRCoeffBS(Form1.Samplerate, MixData.FilterFrequency1, MixData.FilterFrequency2);
			//		SoundArray = objFilter.FIRfilter(SoundArray);
			//		break;
			//}

			if (MixData.FilterType != 0) {
				objFilter.IIRFilter(SoundArray, MixData.FilterFrequency1, MixData.FilterFrequency2, OscArray[0], MixData.FFDepth, Form1.Samplerate, 0f, MixData.FilterOrder, MixData.FilterType);
			}

			// Automatisk gain control AGC
			if (MixData.AGC) {
				double Max = 0.5;
				for (int i = 0; i < Form1.OscArraySize; i++) {
					if (Math.Abs(SoundArray[i]) > Max) Max = Math.Abs(SoundArray[i]);
				}
				double mul = Form1.MaxAmplitude / Max;
				for (int i = 0; i < Form1.OscArraySize; i++) {
					SoundArray[i] = SoundArray[i] * mul;
				}
			}

			//Skapa reverb effekt
			if (MixData.Reverb) {
				double[] Output = new double[SoundArray.Length];
				double[] Gain = new double[8];
				double[] msDelay = new double[8];
				Gain[1] = MixData.Gain1 / 100;
				Gain[2] = MixData.Gain2 / 100;
				Gain[3] = MixData.Gain3 / 100;
				Gain[4] = MixData.Gain4 / 100;
				Gain[5] = MixData.Gain5 / 100;
				Gain[6] = MixData.Gain6 / 100;
				Gain[7] = MixData.Gain7 / 100;
				msDelay[1] = MixData.Delay1;
				msDelay[2] = MixData.Delay2;
				msDelay[3] = MixData.Delay3;
				msDelay[4] = MixData.Delay4;
				msDelay[5] = MixData.Delay5 / 100;
				msDelay[6] = MixData.Delay6 / 100;
				msDelay[7] = MixData.Delay7 / 100;
				SoundArray = objFilter.SchroederReverbator(SoundArray, Output, Gain, msDelay, Form1.Samplerate);
			}

			// (Samplerate/2)Hz LP-filter
			if (MixData.MovingAverageFilter) {
				for (int i = 0; i < Form1.OscArraySize - 1; i++) {
					SoundArray[i] = (SoundArray[i] + SoundArray[i + 1]) / 2;
				}
			}

			// Ta bort likströms komponenten
			if (MixData.RemoveDC) {
				// y(n) = x(n) - x(n-1) + 0.995*y(n-1)
				double[] y = new double[Form1.OscArraySize];
				y[0] = SoundArray[0];
				for (int n = 1; n < Form1.OscArraySize - 1; n++) {
					y[n] = SoundArray[n] - SoundArray[n - 1] + 0.998 * y[n - 1];
				}
				SoundArray = y;
			}

			// Automatisk gain control AGC
			if (MixData.AGC) {
				double Max = 0.5;
				for (int i = 0; i < Form1.OscArraySize; i++) {
					if (Math.Abs(SoundArray[i]) > Max) Max = Math.Abs(SoundArray[i]);
				}
				double mul = Form1.MaxAmplitude / Max;
				for (int i = 0; i < Form1.OscArraySize; i++) {
					SoundArray[i] = SoundArray[i] * mul;
				}
			}

			RawSoundData = SoundArray;

			// Double till 16bit
			for (int i = 0; i < Form1.OscArraySize; i++) {
				double x;
				x = SoundArray[i];
				if (x > Form1.MaxAmplitude - 1.0) x = Form1.MaxAmplitude - 1.0;
				if (x < -Form1.MaxAmplitude) x = -Form1.MaxAmplitude;
				SoundArray16bit[i] = (Int16)(x + 0.5);
			}

			// Bygg .Wav fil
			WaveFile[0] = 0x52; //R
			WaveFile[1] = 0x49; //I
			WaveFile[2] = 0x46; //F
			WaveFile[3] = 0x46; //F
			WaveFile[4] = GetByte(0, Form1.OscArraySize * 2 + 36);
			WaveFile[5] = GetByte(1, Form1.OscArraySize * 2 + 36);
			WaveFile[6] = GetByte(2, Form1.OscArraySize * 2 + 36);
			WaveFile[7] = GetByte(3, Form1.OscArraySize * 2 + 36);
			WaveFile[8] = 0x57; //W
			WaveFile[9] = 0x41; //A
			WaveFile[10] = 0x56; //V
			WaveFile[11] = 0x45; //E
			WaveFile[12] = 0x66; //f
			WaveFile[13] = 0x6d; //m
			WaveFile[14] = 0x74; //t
			WaveFile[15] = 0x20; //

			//_BinaryWriter->Write(this->Subchunk1Size);
			WaveFile[16] = 16;
			WaveFile[17] = 00;
			WaveFile[18] = 00;
			WaveFile[19] = 00;
			//_BinaryWriter->Write(this->AudioFormat);
			WaveFile[20] = 01;
			WaveFile[21] = 00;
			//_BinaryWriter->Write(this->NumChannels);
			WaveFile[22] = 01;
			WaveFile[23] = 00;
			//_BinaryWriter->Write(this->SampleRate);
			WaveFile[24] = GetByte(0, Form1.Samplerate);
			WaveFile[25] = GetByte(1, Form1.Samplerate);
			WaveFile[26] = GetByte(2, Form1.Samplerate);
			WaveFile[27] = GetByte(3, Form1.Samplerate);
			//_BinaryWriter->Write(this->ByteRate);
			WaveFile[28] = GetByte(0, Form1.Samplerate * 2);
			WaveFile[29] = GetByte(1, Form1.Samplerate * 2);
			WaveFile[30] = GetByte(2, Form1.Samplerate * 2);
			WaveFile[31] = GetByte(3, Form1.Samplerate * 2);
			//_BinaryWriter->Write(this->BlockAlign);
			WaveFile[32] = 02;
			WaveFile[33] = 00;
			//_BinaryWriter->Write(this->BitsPerSample);
			WaveFile[34] = 16;
			WaveFile[35] = 00;
			//_BinaryWriter->Write(this->Subchunk2ID);
			WaveFile[36] = 0x64; //d
			WaveFile[37] = 0x61; //a
			WaveFile[38] = 0x74; //t
			WaveFile[39] = 0x61; //a
								 //_BinaryWriter->Write(this->Subchunk2Size);
			WaveFile[40] = GetByte(0, Form1.OscArraySize * 2);
			WaveFile[41] = GetByte(1, Form1.OscArraySize * 2);
			WaveFile[42] = GetByte(2, Form1.OscArraySize * 2);
			WaveFile[43] = GetByte(3, Form1.OscArraySize * 2);
			for (Int32 i = 0; i < Form1.OscArraySize; i++) {
				WaveFile[44 + i * 2 + 0] = (Byte)(SoundArray16bit[i] & 255);
				WaveFile[44 + i * 2 + 1] = (Byte)((SoundArray16bit[i] >> 8) & 255);
			}

			SoundDotWav = WaveFile;
			// Play
			MemoryStream ms = new MemoryStream(WaveFile);
			SoundPlayer simpleSound = new SoundPlayer(ms);
			simpleSound.Play();
		}

		public void PlaySound( Byte[] WaveFile ) {
			if (WaveFile != null) {
				MemoryStream ms = new MemoryStream(WaveFile);
				SoundPlayer simpleSound = new SoundPlayer(ms);
				simpleSound.Play();
			}
		}

		private Byte GetByte( Int32 BytePos, Int32 Value ) {
			BytePos = BytePos & 3;
			return (Byte)((Value >> (BytePos * 8)) & 255);
		}

		public Byte[] GetSoundDotWave() {
			return SoundDotWav;
		}

		public double[] GetRawSoundData() {
			return RawSoundData;
		}



		public MixerData CreateRandomGlobal( MixerData MD, RndS RS ) {
			Random Rnd = new Random();
			MD.MasterVolume = 65536;
			if (!RS.ReverbLock) {
				MD.AGC = true;
				MD.Reverb = Rnd.Next(0, 2) == 0;
				MD.Delay1 = Rnd.Next(10, 100);
				MD.Delay2 = Rnd.Next(10, 100);
				MD.Delay3 = Rnd.Next(10, 100);
				MD.Delay4 = Rnd.Next(10, 100);
				MD.Delay5 = Rnd.Next(100, 400);
				MD.Delay6 = Rnd.Next(50, 250);
				MD.Delay7 = Rnd.Next(25, 200);
				MD.Gain1 = Rnd.Next(50, 95);
				MD.Gain2 = Rnd.Next(50, 95);
				MD.Gain3 = Rnd.Next(50, 95);
				MD.Gain4 = Rnd.Next(50, 95);
				MD.Gain5 = Rnd.Next(50, 95);
				MD.Gain6 = Rnd.Next(50, 95);
				MD.Gain7 = Rnd.Next(50, 95);
			}
			if (!RS.FilterLock) {
				MD.FilterType = Rnd.Next(0, 4 + 1);
				MD.FilterFrequency1 = Rnd.Next(20, 500);
				MD.FilterFrequency2 = Rnd.Next(550, 3000);
				MD.FilterOrder = Rnd.Next(1, 4);

				//MD.MovingAverageFilter = true;
				MD.RemoveDC = true;
			}
			return MD;
		}

		public MixerData CreateRandomReverb( MixerData MD ) {
			Random Rnd = new Random();

			MD.MasterVolume = 65536;
			MD.AGC = true;

			MD.Reverb = true;
			MD.Delay1 = Rnd.Next(10, 100);
			MD.Delay2 = Rnd.Next(10, 100);
			MD.Delay3 = Rnd.Next(10, 100);
			MD.Delay4 = Rnd.Next(10, 100);
			MD.Delay5 = Rnd.Next(100, 400);
			MD.Delay6 = Rnd.Next(50, 250);
			MD.Delay7 = Rnd.Next(25, 200);
			MD.Gain1 = Rnd.Next(50, 95);
			MD.Gain2 = Rnd.Next(50, 95);
			MD.Gain3 = Rnd.Next(50, 95);
			MD.Gain4 = Rnd.Next(50, 95);
			MD.Gain5 = Rnd.Next(50, 95);
			MD.Gain6 = Rnd.Next(50, 95);
			MD.Gain7 = Rnd.Next(50, 95);

			//MD.MovingAverageFilter = true;
			MD.RemoveDC = true;

			return MD;
		}


		public MixerData CreateRandomFilter( MixerData MD ) {
			Random Rnd = new Random();

			MD.MasterVolume = 65536;
			MD.AGC = true;

			MD.FilterType = Rnd.Next(1, 4 + 1);
			MD.FilterFrequency1 = Rnd.Next(20, 2500);
			MD.FilterFrequency2 = Rnd.Next((int)MD.FilterFrequency1, 5000);
			MD.FilterOrder = Rnd.Next(1, 4);

			//MD.MovingAverageFilter = true;
			MD.RemoveDC = true;

			return MD;
		}


	}
}