using System;

namespace SoundCreator {

	internal class Filter {
		private static int FIRCoeffSize = 256;
		private static int FFTSize = 65536;
		//private static uint NoDFTs = 32;
		//private static uint OverLap = FFTSize - 1024;
		//private static uint LengthOfArea = FFTSize * NoDFTs - (NoDFTs - 1) * OverLap;

		private static double[] FIRCoeff;
		private static double[] InReal;
		private static double[] InImag;
		private static double[] OutReal;
		private static double[] OutImag;

		// Konstruktorn
		public Filter() {
			double[] FIRC = new double[FIRCoeffSize]; //this->WinLen
			FIRCoeff = FIRC;
			double[] inreal = new double[FFTSize];
			InReal = inreal;
			double[] inimag = new double[FFTSize];
			InImag = inimag;
			double[] outreal = new double[FFTSize];
			OutReal = outreal;
			double[] outimag = new double[FFTSize];
			OutImag = outimag;
		}

		//FIRCoeff En array med ett uträknat fönster
		//WindowLength fönstrets längd (4 - 255)
		//WaveLength insignal till filtret
		public double[] FIRfilter( double[] RawWave ) {
			uint n;
			int Top = 0;
			uint WaveLength = (uint)RawWave.Length;

			double y;

			double[] RawWaveData = RawWave;
			double[] WaveDataFiltered = new double[WaveLength];

			double[] Reg = new double[FIRCoeffSize];

			for (uint j = 0; j < FIRCoeffSize; j++) Reg[j] = 0.0;

			for (uint j = 0; j < WaveLength + (FIRCoeffSize / 2); j++) {
				if (j >= WaveLength) break;
				Reg[Top] = RawWaveData[j];
				y = 0.0;
				n = 0;

				for (int k = Top; k >= 0; k--) {
					y = y + FIRCoeff[n++] * Reg[k];
				}
				for (int k = (int)FIRCoeffSize - 1; k > Top; k--) {
					y = y + FIRCoeff[n++] * Reg[k];
				}
				if (j - FIRCoeffSize / 2 >= 0 && j > FIRCoeffSize / 2) {
					WaveDataFiltered[j - FIRCoeffSize / 2] = y;
				}
				Top++;
				if (Top >= FIRCoeffSize) Top = 0;
			}
			return WaveDataFiltered;
		}

		public void FFT( bool Inverse, double[] InReal, double[] InImag, double[] OutReal, double[] OutImag, int FFTSize ) {
			int n, i, i1, j, k, i2, l, l1, l2;
			double c1, c2, tx, ty, t1, t2, u1, u2, z;
			int s = FFTSize;

			for (i = 0; i < s; i++) {
				OutReal[i] = InReal[i];
				OutImag[i] = InImag[i];
			}

			int m = 0;
			while (s > 1 && s != 1) {
				s = s / 2;
				m++;
			}

			/* Calculate the number of points */
			n = 1;
			for (i = 0; i < m; i++) {
				n *= 2;
			}

			/* Do the bit reversal */
			i2 = n >> 1;
			j = 0;
			for (i = 0; i < n - 1; i++) {
				if (i < j) {
					tx = OutReal[i];
					ty = OutImag[i];
					OutReal[i] = OutReal[j];
					OutImag[i] = OutImag[j];
					OutReal[j] = tx;
					OutImag[j] = ty;
				}
				k = i2;
				while (k <= j) {
					j -= k;
					k >>= 1;
				}
				j += k;
			}

			/* Compute the FFT */
			c1 = -1.0;
			c2 = 0.0;
			l2 = 1;
			for (l = 0; l < m; l++) {
				l1 = l2;
				l2 <<= 1;
				u1 = 1.0;
				u2 = 0.0;
				for (j = 0; j < l1; j++) {
					for (i = j; i < n; i += l2) {
						i1 = i + l1;
						t1 = u1 * OutReal[i1] - u2 * OutImag[i1];
						t2 = u1 * OutImag[i1] + u2 * OutReal[i1];
						OutReal[i1] = OutReal[i] - t1;
						OutImag[i1] = OutImag[i] - t2;
						OutReal[i] += t1;
						OutImag[i] += t2;
					}
					z = u1 * c1 - u2 * c2;
					u2 = u1 * c2 + u2 * c1;
					u1 = z;
				}
				c2 = Math.Sqrt((1.0 - c1) / 2.0);
				if (!Inverse) {
					c2 = -c2;
				}
				c1 = Math.Sqrt((1.0 + c1) / 2.0);
			}

			/* Scaling for forward transform */
			if (!Inverse) {
				for (i = 0; i < n; i++) {
					OutReal[i] /= n;
					OutImag[i] /= n;
				}
			}
		}

		public void CreateFIRCoeffLP( int SampleRate, double Frequency1 ) {
			int i, j, k;
			int FFTSamples;
			double Amplitude;
			double HzPerFFT;
			double a, Pi = 3.141592653589793238462643383;

			HzPerFFT = (double)SampleRate / FFTSize;
			if (HzPerFFT == 0.0) HzPerFFT = 1.0;
			FFTSamples = (int)(Frequency1 / HzPerFFT);
			if (FFTSamples >= FFTSize / 2) FFTSamples = (FFTSize / 2) - 1;

			j = FFTSize - 1;
			for (i = 0; i < FFTSize / 2; i++) {
				if (FFTSamples > i) Amplitude = 1.0; else Amplitude = 0.0;
				InReal[i] = Amplitude;
				InImag[i] = 0.0;
				InReal[j] = Amplitude;
				InImag[j] = 0.0;
				j--;
			}

			FFT(false, InReal, InImag, OutReal, OutImag, FFTSize);

			j = 0;
			k = FIRCoeffSize / 2;
			for (i = 0; i < FIRCoeffSize / 2; i++) {
				FIRCoeff[FIRCoeffSize / 2 + j] = OutReal[i];
				j++;
				FIRCoeff[k - 1] = OutReal[FFTSize - i - 1];
				k--;
			}
			for (i = 0; i < FIRCoeffSize; i++) {
				//a = 0.54 - 0.46 * Math::Cos(i * 2.0 * Pi / (FIRCoeffSize - 1)); //Hamming window
				// Blackman-Nuttall window
				a = 0.3635819 - 0.4891775 * Math.Cos(i * 2.0 * Pi / (FIRCoeffSize - 1)) + 0.1365995 * Math.Cos(i * 4.0 * Pi / (FIRCoeffSize - 1)) - 0.0106411 * Math.Cos(i * 6.0 * Pi / (FIRCoeffSize - 1));
				FIRCoeff[i] = FIRCoeff[i] * a;
			}
		}

		public void CreateFIRCoeffHP( int SampleRate, double Frequency1 ) {
			int i, j, k;
			int FFTSamples;
			double Amplitude;
			double HzPerFFT;
			double a, Pi = 3.141592653589793238462643383;

			HzPerFFT = (double)SampleRate / FFTSize;
			if (HzPerFFT == 0.0) HzPerFFT = 1.0;
			FFTSamples = (int)(Frequency1 / HzPerFFT);
			if (FFTSamples >= FFTSize / 2) FFTSamples = (FFTSize / 2) - 1;

			j = FFTSize - 1;
			for (i = 0; i < FFTSize / 2; i++) {
				if (FFTSamples > i) Amplitude = 0.0; else Amplitude = 1.0;
				InReal[i] = Amplitude;
				InImag[i] = 0.0;
				InReal[j] = Amplitude;
				InImag[j] = 0.0;
				j--;
			}

			FFT(false, InReal, InImag, OutReal, OutImag, FFTSize);

			j = 0;
			k = FIRCoeffSize / 2;
			for (i = 0; i < FIRCoeffSize / 2; i++) {
				FIRCoeff[FIRCoeffSize / 2 + j] = OutReal[i];
				j++;
				FIRCoeff[k - 1] = OutReal[FFTSize - i - 1];
				k--;
			}
			for (i = 0; i < FIRCoeffSize; i++) {
				//a = 0.54 - 0.46 * Math::Cos(i * 2.0 * Pi / (FIRCoeffSize - 1)); //Hamming window
				// Blackman-Nuttall window
				a = 0.3635819 - 0.4891775 * Math.Cos(i * 2.0 * Pi / (FIRCoeffSize - 1)) + 0.1365995 * Math.Cos(i * 4.0 * Pi / (FIRCoeffSize - 1)) - 0.0106411 * Math.Cos(i * 6.0 * Pi / (FIRCoeffSize - 1));
				FIRCoeff[i] = FIRCoeff[i] * a;
			}
		}

		public void CreateFIRCoeffBP( int SampleRate, double Frequency1,  double Frequency3) {
			int i, j, k;
			int FFTSamplesLow, FFTSamplesHigh;
			double Amplitude = 0;
			double HzPerFFT;
			double a, Pi = 3.141592653589793238462643383;

			HzPerFFT = (double)SampleRate / FFTSize;
			if (HzPerFFT == 0.0) HzPerFFT = 1;
			FFTSamplesLow = (int)(Frequency1 / HzPerFFT);
			if (FFTSamplesLow >= FFTSize / 2) FFTSamplesLow = (FFTSize / 2) - 1;
			FFTSamplesHigh = (int)(Frequency3 / HzPerFFT);
			if (FFTSamplesHigh >= FFTSize / 2) FFTSamplesHigh = (FFTSize / 2) - 1;

			j = FFTSize - 1;
			for (i = 0; i < FFTSize / 2; i++) {
				if ((FFTSize / 2) - FFTSamplesLow > i) Amplitude = 0.0;
				//if ((DFTSize / 2) - FFTSamplesHigh > i) Amplitude = 1.0;
				if (FFTSamplesHigh > i) Amplitude = 1.0;
				if (FFTSamplesLow > i) Amplitude = 0.0;
				InReal[i] = Amplitude;
				InImag[i] = 0.0;
				InReal[j] = Amplitude;
				InImag[j] = 0.0;
				j--;
			}

			FFT(false, InReal, InImag, OutReal, OutImag, FFTSize);

			j = 0;
			k = FIRCoeffSize / 2;
			for (i = 0; i < FIRCoeffSize / 2; i++) {
				FIRCoeff[FIRCoeffSize / 2 + j] = OutReal[i];
				j++;
				FIRCoeff[k - 1] = OutReal[FFTSize - i - 1];
				k--;
			}
			for (i = 0; i < FIRCoeffSize; i++) {
				//a = 0.54 - 0.46 * Math::Cos(i * 2.0 * Pi / (FIRCoeffSize - 1)); //Hamming window
				// Blackman-Nuttall window
				a = 0.3635819 - 0.4891775 * Math.Cos(i * 2.0 * Pi / (FIRCoeffSize - 1)) + 0.1365995 * Math.Cos(i * 4.0 * Pi / (FIRCoeffSize - 1)) - 0.0106411 * Math.Cos(i * 6.0 * Pi / (FIRCoeffSize - 1));
				FIRCoeff[i] = FIRCoeff[i] * a;
			}
		}

		public void CreateFIRCoeffBS( int SampleRate, double Frequency1, double Frequency3 ) {
			int i, j, k;
			int FFTSamplesLow, FFTSamplesHigh;
			double Amplitude = 0;
			double HzPerFFT;
			double a, Pi = 3.141592653589793238462643383;

			HzPerFFT = (double)SampleRate / FFTSize;
			if (HzPerFFT == 0.0) HzPerFFT = 1;
			FFTSamplesLow = (int)(Frequency1 / HzPerFFT);
			if (FFTSamplesLow >= FFTSize / 2) FFTSamplesLow = (FFTSize / 2) - 1;
			FFTSamplesHigh = (int)(Frequency3 / HzPerFFT);
			if (FFTSamplesHigh >= FFTSize / 2) FFTSamplesHigh = (FFTSize / 2) - 1;

			j = FFTSize - 1;
			for (i = 0; i < FFTSize / 2; i++) {
				if ((FFTSize / 2) - FFTSamplesLow > i) Amplitude = 1.0;
				//if ((DFTSize / 2) - FFTSamplesHigh > i) Amplitude = 0.0;
				if (FFTSamplesHigh > i) Amplitude = 0.0;
				if (FFTSamplesLow > i) Amplitude = 1.0;
				InReal[i] = Amplitude;
				InImag[i] = 0.0;
				InReal[j] = Amplitude;
				InImag[j] = 0.0;
				j--;
			}

			FFT(false, InReal, InImag, OutReal, OutImag, FFTSize);

			j = 0;
			k = FIRCoeffSize / 2;
			for (i = 0; i < FIRCoeffSize / 2; i++) {
				FIRCoeff[FIRCoeffSize / 2 + j] = OutReal[i];
				j++;
				FIRCoeff[k - 1] = OutReal[FFTSize - i - 1];
				k--;
			}
			for (i = 0; i < FIRCoeffSize; i++) {
				//a = 0.54 - 0.46 * Math::Cos(i * 2.0 * Pi / (FIRCoeffSize - 1)); //Hamming window
				// Blackman-Nuttall window
				a = 0.3635819 - 0.4891775 * Math.Cos(i * 2.0 * Pi / (FIRCoeffSize - 1)) + 0.1365995 * Math.Cos(i * 4.0 * Pi / (FIRCoeffSize - 1)) - 0.0106411 * Math.Cos(i * 6.0 * Pi / (FIRCoeffSize - 1));
				FIRCoeff[i] = FIRCoeff[i] * a;
			}
		}

		public double[] GetFFT( double[] RawWave ) {
			double[] Result = new double[FFTSize];
			
			for (int i = 0; i < RawWave.Length - FFTSize; i = i + FFTSize / 2) {

				for (int j = 0; j < FFTSize; j++) {
					InReal[j] = RawWave[i + j];
					InImag[j] = 0.0;
					OutReal[j] = 0.0;
					OutImag[j] = 0.0;
                }
				FFT(false, InReal, InImag, OutReal, OutImag, FFTSize);
				for(int j=0; j< FFTSize; j++) {
					//Result[j] = Result[j] + OutReal[j];
					Result[j] = Result[j] + Math.Sqrt(OutReal[j] * OutReal[j] + OutImag[j] * OutImag[j]);
                }
			}

			return Result;
		}





		// Reverbe stuff
		// Comb filter
		// All Pass Filter
		public void FeedForwardCombFilter( double[] Input, double[] Output, double Gain, double msDelay, double NoSamples, int Samplerate ) {
			int EndPos = (int)(msDelay * Samplerate / 1000.0);
			for (int i = 0; i < EndPos; i++) Output[i] = Input[i];
			for (int i = EndPos; i < NoSamples; i++) {
				Output[i] = Input[i] + Gain * Input[i - EndPos];
			}

		}

		public void FeedBackwardCombFilter( double[] Input, double[] Output, double Gain, double msDelay, double NoSamples, int Samplerate ) {
			int EndPos = (int)(msDelay * Samplerate / 1000.0);
			for (int i = 0; i < EndPos; i++) Output[i] = Input[i];
			for (int i = EndPos; i < NoSamples; i++) {
				Output[i] = Input[i] + Gain * Output[i - EndPos];
			}
		}

		// Double comb allpasss filter
		public void AllPassFilter( double[] Input, double[] Output, double Gain, double msDelay, double NoSamples, int Samplerate ) {
			int EndPos = (int)(msDelay * Samplerate / 1000.0);
			for (int i = 0; i < EndPos; i++) Output[i] = Gain * Input[i];
			for (int i = EndPos; i < NoSamples; i++) {
				Output[i] = Gain * Input[i] + Input[i - EndPos] - Gain * Output[i - EndPos];
			}
		}

		public double[] SchroederReverbator( double[] Input, double[] Output, double[] Gain, double[] msDelay, int Samplerate ) {
			double[] Output1 = new double[Input.Length];
			double[] Output2 = new double[Input.Length];
			double[] Output3 = new double[Input.Length];
			double[] Output4 = new double[Input.Length];
			double[] Output5 = new double[Input.Length];
			double[] Output6 = new double[Input.Length];
			double[] Output7 = new double[Input.Length];

			FeedBackwardCombFilter(Input, Output1, Gain[1], msDelay[1], Input.Length, Samplerate); // 0.805 - 901 (20.4ms)
			FeedBackwardCombFilter(Input, Output2, Gain[2], msDelay[2], Input.Length, Samplerate); // 0.827 - 778 (17.6ms)
			FeedBackwardCombFilter(Input, Output3, Gain[3], msDelay[3], Input.Length, Samplerate); // 0.783 - 1011 (22.9ms)
			FeedBackwardCombFilter(Input, Output4, Gain[4], msDelay[4], Input.Length, Samplerate); // 0.764 - 1123 (25.5ms)
			for (int i = 0; i < Input.Length; i++) {
				Output[i] = (Output1[i] + Output2[i] + Output3[i] + Output4[i]) / 4;
			}
			AllPassFilter(Output, Output5, Gain[5], msDelay[5], Input.Length, Samplerate); // Gain ca 0.7
			AllPassFilter(Output5, Output6, Gain[6], msDelay[6], Input.Length, Samplerate);
			AllPassFilter(Output6, Output7, Gain[7], msDelay[7], Input.Length, Samplerate);// Delay sequence ex 125, 42, 12         2.834ms, 0.95ms, 0.27ms
			return Output7;
		}

		public int GetFFTSize() {
			return FFTSize;
		}



		public double[] Butterworth(double[] RawWave, double MainFrequency, double[] Frequency, double Depth, int Samplerate, float resonance,  bool LowPass) {
			double[] FilteredWave = new double[RawWave.Length];
			double[] inputHistory = new double[2];
			double[] outputHistory = new double[2];
			double c, a1, a2, a3, b1, b2, x, f;
			c = a1 = a2 = a3 = b1 = b2 = 0.0;

			for (int i = 0; i < RawWave.Length; i++) {

				x = Math.Pow(Math.Sqrt(2), (Frequency[i] / Form1.MaxAmplitude) * 2.0 * (Depth / 100) + 2.0);
				f = x * MainFrequency / 2.0;
				if (f > Samplerate / 2.0) f = Samplerate / 2.0 - 0.00001;
				switch (LowPass) {
					case true:
						c = 1.0f / (float)Math.Tan(Math.PI * f / Samplerate);
						a1 = 1.0f / (1.0f + resonance * c + c * c);
						a2 = 2f * a1;
						a3 = a1;
						b1 = 2.0f * (1.0f - c * c) * a1;
						b2 = (1.0f - resonance * c + c * c) * a1;
						break;
					case false:
						c = (float)Math.Tan(Math.PI * f / Samplerate);
						a1 = 1.0f / (1.0f + resonance * c + c * c);
						a2 = -2f * a1;
						a3 = a1;
						b1 = 2.0f * (c * c - 1.0f) * a1;
						b2 = (1.0f - resonance * c + c * c) * a1;
						break;
				}

				if (RawWave[i] < 0.0001 && -RawWave[i] < 0.0001) RawWave[i] = 0.0001;

				FilteredWave[i] = a1 * RawWave[i] + a2 * inputHistory[0] + a3 * inputHistory[1] - b1 * outputHistory[0] - b2 * outputHistory[1];

				inputHistory[1] = inputHistory[0];
				inputHistory[0] = RawWave[i];


				outputHistory[1] = outputHistory[0];
				outputHistory[0] = FilteredWave[i];
			}

			return FilteredWave;
		}




	



	}

	public class FilterButterworth {
		/// <summary>
		/// rez amount, from sqrt(2) to ~ 0.1
		/// </summary>
		private readonly float resonance;

		private readonly float frequency;
		private readonly int sampleRate;
		private readonly PassType passType;

		private readonly float c, a1, a2, a3, b1, b2;

		/// <summary>
		/// Array of input values, latest are in front
		/// </summary>
		private float[] inputHistory = new float[2];

		/// <summary>
		/// Array of output values, latest are in front
		/// </summary>
		private float[] outputHistory = new float[3];

		public FilterButterworth( float frequency, int sampleRate, PassType passType, float resonance ) {
			this.resonance = resonance;
			this.frequency = frequency;
			this.sampleRate = sampleRate;
			this.passType = passType;

			switch (passType) {
				case PassType.Lowpass:
					c = 1.0f / (float)Math.Tan(Math.PI * frequency / sampleRate);
					a1 = 1.0f / (1.0f + resonance * c + c * c);
					a2 = 2f * a1;
					a3 = a1;
					b1 = 2.0f * (1.0f - c * c) * a1;
					b2 = (1.0f - resonance * c + c * c) * a1;
					break;
				case PassType.Highpass:
					c = (float)Math.Tan(Math.PI * frequency / sampleRate);
					a1 = 1.0f / (1.0f + resonance * c + c * c);
					a2 = -2f * a1;
					a3 = a1;
					b1 = 2.0f * (c * c - 1.0f) * a1;
					b2 = (1.0f - resonance * c + c * c) * a1;
					break;
			}

			
		}

		public enum PassType {
			Highpass,
			Lowpass,
		}

		public void Update( float newInput ) {
			float newOutput = a1 * newInput + a2 * this.inputHistory[0] + a3 * this.inputHistory[1] - b1 * this.outputHistory[0] - b2 * this.outputHistory[1];

			this.inputHistory[1] = this.inputHistory[0];
			this.inputHistory[0] = newInput;

			this.outputHistory[2] = this.outputHistory[1];	// ?
			this.outputHistory[1] = this.outputHistory[0];
			this.outputHistory[0] = newOutput;
		}

		public float Value {
			get { return this.outputHistory[0]; }
		}
	}
}