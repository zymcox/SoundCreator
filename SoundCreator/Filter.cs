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

		public void CreateFIRCoeffBP( int SampleRate, double Frequency1, double Frequency3 ) {
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
				for (int j = 0; j < FFTSize; j++) {
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

		// Double comb allpass filter
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



		public double[] ButterworthX( double[] RawWave, double MainFrequency, double[] Frequency, double Depth, int Samplerate, float resonance, bool LowPass ) {
			double[] FilteredWave = new double[RawWave.Length];
			double[] inputHistory = new double[2];
			double[] outputHistory = new double[2];
			double c, a1, a2, a3, b1, b2, x, fc;
			c = a1 = a2 = a3 = b1 = b2 = 0.0;

			for (int i = 0; i < RawWave.Length; i++) {

				x = Math.Pow(Math.Sqrt(2), (Frequency[i] / Form1.MaxAmplitude) * 2.0 * (Depth / 100) + 2.0);
				fc = x * MainFrequency / 2.0;
				if (fc > Samplerate / 2.0) fc = Samplerate / 2.0 - 0.00001;
				switch (LowPass) {
					case true: // Lowpass
						c = 1.0f / (float)Math.Tan(Math.PI * fc / Samplerate);
						a1 = 1.0f / (1.0f + resonance * c + c * c);  //   1 / s^2 + sqr(2)*s + 1
						a2 = 2f * a1;
						a3 = a1;

						b1 = 2.0f * (1.0f - c * c) * a1;
						b2 = (1.0f - resonance * c + c * c) * a1;

						break;
					case false: //HiPass
						c = (float)Math.Tan(Math.PI * fc / Samplerate);
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



		public double[] IIRFilter( double[] RawWave, double Frequency1, double Frequency2, double[] Frequency, double Depth, int Samplerate, float resonance, int Order, int PassType ) {

			//Filtrera bara om det behövs
			int RWLength = RawWave.Length;
			for(int i=RawWave.Length-1; i>=0; --i) {
				if (RawWave[i] > 0.1) {
					RWLength = i;
					break;
				}
			}
			if (RWLength < 1000) RWLength = 1000;


			double[] FilteredWave = new double[RawWave.Length];
			double[] inputHistory = new double[2];
			double[] outputHistory = new double[2];
			double[] inputHistory2 = new double[2];
			double[] outputHistory2 = new double[2];
			double[] inputHistory3 = new double[2];
			double[] outputHistory3 = new double[2];
			double[] inputHistory4 = new double[2];
			double[] outputHistory4 = new double[2];

			double FW1,FW2;

			double c,a0, a1, a2, b0, b1, b2, x, fc;
			c = a0 = a1 = a2 = b0 = b1 = b2 = 0.0;

			//			This is bilinear transform code for IIR filters.
			//			In order to use these equations, you must define the variables A, B, C, D, E, and F
			//			which are the coefficients for the low pass prototype function H(s).
			//			H(s) = (D * s ^ 2 + E * s + F) / (A * s ^ 2 + B * s + C)
			//          s = 2(z – 1) / (z + 1)

			//
			// z = insignalsample




			double MPI2 = Math.PI/2;
			double OmegaC, OmegaC2;
			double T1,T2;
			double A,B,C,D,E,F,Arg;

			double[,] Coefficients = CalcButterworthCoeff(Order, Order);


			for (int j = 0; j < Order / 2; j++) {
				A = 1.0;
				B = -Coefficients[j, 1];
				C = 1.0;
				D = 1.0;        // Endast för allpass annars 0.0
				E = -B;         // Endast för allpass annars 0.0
				F = 1.0;

				inputHistory[0] = 0.0;
				inputHistory[1] = 0.0;
				inputHistory2[0] = 0.0;
				inputHistory2[1] = 0.0;
				inputHistory3[0] = 0.0;
				inputHistory3[1] = 0.0;
				inputHistory4[0] = 0.0;
				inputHistory4[1] = 0.0;
				outputHistory[0] = 0.0;
				outputHistory[1] = 0.0;
				outputHistory2[0] = 0.0;
				outputHistory2[1] = 0.0;
				outputHistory3[0] = 0.0;
				outputHistory3[1] = 0.0;
				outputHistory4[0] = 0.0;
				outputHistory4[1] = 0.0;

				for (int i = 0; i < RWLength; i++) {
					x = 1.0;
					if (Frequency != null) {
						x = Math.Pow(Math.Sqrt(2.0), (Frequency[i] / Form1.MaxAmplitude) * 2.0 * (Depth / 100.0) + 2.0); // x = sqrt(2)^ (0 till 4) = 1 till 4
					}
					x = x / 2.0; // 0.5 - 2.0

					fc = x * Frequency1;                  // fc = 0.5 till 2.0  * frekvens (Depth = 100.0 => +- 1 oktav)
					OmegaC = fc / (Samplerate / 2.0);              // 0 till 20000Hz => OmegaC = 0 till 1    Cut Off Frequency
					fc = x * Frequency2;
					OmegaC2 = fc / (Samplerate / 2.0);
					T1 = 2.0 * Math.Tan(OmegaC * MPI2);           // * Pi/2
					T2 = 2.0 * Math.Tan(OmegaC2 * MPI2);

					switch (PassType) {
						case 1:    // 2 poles Lowpass
							Arg = (4.0 + 2.0 * B * T1 + T1 * T1);
							a2 = (4.0 - 2.0 * B * T1 + T1 * T1) / Arg;
							a1 = (2.0 * T1 * T1 - 8.0) / Arg;

							b2 = T1 * T1 / Arg;
							b1 = 2.0 * b2;  //(2.0 * T * T) / Arg;
							b0 = b2;

							FilteredWave[i] = b0 * RawWave[i] + b1 * inputHistory[0] + b2 * inputHistory[1] - a1 * outputHistory[0] - a2 * outputHistory[1];
							inputHistory[1] = inputHistory[0];
							inputHistory[0] = RawWave[i];
							outputHistory[1] = outputHistory[0];
							outputHistory[0] = FilteredWave[i];
							break;
						case 2:     // 2 poles Highpass
							Arg = T2 * T2 + 4.0 + 2.0 * B * T2;
							a2 = (T2 * T2 + 4.0 - 2.0 * B * T2) / Arg;
							a1 = (2.0 * T2 * T2 - 8.0) / Arg;

							b2 = 4.0 / Arg;
							b1 = -2.0 * b2;
							b0 = b2;

							FilteredWave[i] = b0 * RawWave[i] + b1 * inputHistory[0] + b2 * inputHistory[1] - a1 * outputHistory[0] - a2 * outputHistory[1];
							inputHistory[1] = inputHistory[0];
							inputHistory[0] = RawWave[i];
							outputHistory[1] = outputHistory[0];
							outputHistory[0] = FilteredWave[i];
							break;

						case 3: // Bandpass
								// LP
							Arg = (4.0 + 2.0 * B * T2 + T2 * T2);
							a2 = (4.0 - 2.0 * B * T2 + T2 * T2) / Arg;
							a1 = (2.0 * T2 * T2 - 8.0) / Arg;

							b2 = T2 * T2 / Arg;
							b1 = 2.0 * b2;  //(2.0 * T2 * T2) / Arg;
							b0 = b2;

							FW1 = b0 * RawWave[i] + b1 * inputHistory[0] + b2 * inputHistory[1] - a1 * outputHistory[0] - a2 * outputHistory[1];
							inputHistory[1] = inputHistory[0];
							inputHistory[0] = RawWave[i];
							outputHistory[1] = outputHistory[0];
							outputHistory[0] = FW1;

							// HP
							Arg = T1 * T1 + 4.0 + 2.0 * B * T1;
							a2 = (T1 * T1 + 4.0 - 2.0 * B * T1) / Arg;
							a1 = (2.0 * T1 * T1 - 8.0) / Arg;

							b2 = 4.0 / Arg;
							b1 = -2.0 * b2;
							b0 = b2;

							FilteredWave[i] = b0 * FW1 + b1 * inputHistory2[0] + b2 * inputHistory2[1] - a1 * outputHistory2[0] - a2 * outputHistory2[1];
							inputHistory2[1] = inputHistory2[0];
							inputHistory2[0] = FW1; //  RawWave[i];
							outputHistory2[1] = outputHistory2[0];
							outputHistory2[0] = FilteredWave[i];


							break;
						case 4:
							// LP
							Arg = (4.0 + 2.0 * B * T1 + T1 * T1);
							a2 = (4.0 - 2.0 * B * T1 + T1 * T1) / Arg;
							a1 = (2.0 * T1 * T1 - 8.0) / Arg;

							b2 = T1 * T1 / Arg;
							b1 = 2.0 * b2;  //(2.0 * T2 * T2) / Arg;
							b0 = b2;

							FW1 = b0 * RawWave[i] + b1 * inputHistory[0] + b2 * inputHistory[1] - a1 * outputHistory[0] - a2 * outputHistory[1];
							inputHistory[1] = inputHistory[0];
							inputHistory[0] = RawWave[i];
							outputHistory[1] = outputHistory[0];
							outputHistory[0] = FW1;

							// HP
							Arg = T2 * T2 + 4.0 + 2.0 * B * T2;
							a2 = (T2 * T2 + 4.0 - 2.0 * B * T2) / Arg;
							a1 = (2.0 * T2 * T2 - 8.0) / Arg;

							b2 = 4.0 / Arg;
							b1 = -2.0 * b2;
							b0 = b2;

							//FilteredWave[i]
							FW2 = b0 * RawWave[i] + b1 * inputHistory2[0] + b2 * inputHistory2[1] - a1 * outputHistory2[0] - a2 * outputHistory2[1];
							inputHistory2[1] = inputHistory2[0];
							inputHistory2[0] = RawWave[i];
							outputHistory2[1] = outputHistory2[0];
							outputHistory2[0] = FW2;
							FilteredWave[i] = FW1 + FW2;

							// Allpass LP
							//Arg = (4.0 * A + 2.0 * B * T2 + C * T2 * T2);
							//a2 = (4.0 * A - 2.0 * B * T2 + C * T2 * T2) / Arg;
							//a1 = (2.0 * C * T2 * T2 - 8.0 * A) / Arg;
							//a0 = 1.0;

							//b2 = (4.0 * D - 2.0 * E * T2 + F * T2 * T2) / Arg * C / F;
							//b1 = (2.0 * F * T2 * T2 - 8.0 * D) / Arg * C / F;
							//b0 = (4 * D + F * T2 * T2 + 2.0 * E * T2) / Arg * C / F;


							//FW1 = b0 * RawWave[i] + b1 * inputHistory3[0] + b2 * inputHistory3[1] - a1 * outputHistory3[0] - a2 * outputHistory3[1];
							//inputHistory3[1] = inputHistory3[0];
							//inputHistory3[0] = RawWave[i];
							//outputHistory3[1] = outputHistory3[0];
							//outputHistory3[0] = FW1;

							//// Allpass HP
							//Arg = A * T1 * T1 + 4.0 * C + 2.0 * B * T1;
							//a2 = (A * T1 * T1 + 4.0 * C - 2.0 * B * T1) / Arg;
							//a1 = (2.0 * A * T1 * T1 - 8.0 * C) / Arg;
							//a0 = 1.0;

							//b2 = (D * T1 * T1 - 2.0 * E * T1 + 4.0 * F) / Arg * C / F;
							//b1 = (2.0 * D * T1 * T1 - 8.0 * F) / Arg * C / F;
							//b0 = (D * T1 * T1 + 4.0 * F + 2.0 * E * T1) / Arg * C / F;


							//FW2 = b0 * FW1 + b1 * inputHistory4[0] + b2 * inputHistory4[1] - a1 * outputHistory4[0] - a2 * outputHistory4[1];
							//inputHistory4[1] = inputHistory4[0];
							//inputHistory4[0] = FW1;
							//outputHistory4[1] = outputHistory4[0];
							//outputHistory4[0] = FW2;




							//FilteredWave[i] = FW2 -FilteredWave[i];


							break;

						default:
							break;
					}

					// Direct transpose realization    x(n) =>  B(z) => 1/A(z) => y(n)


				}
				Array.Copy(FilteredWave, RawWave, RawWave.Length);
			}

			return FilteredWave;
		}

		//2.nd order poly coefficients
		private double[,] CalcButterworthCoeff( int Order, int Poles ) {
			int n = Order;

			double PI = Math.PI;
			double[,] Coefficients = new double[Poles,3];
			for (int k = 0; k < Poles; k++) {
				Coefficients[k, 0] = 1.0;
				Coefficients[k, 1] = -2.0 * Math.Cos((2 * k + 1) * PI / (2.0 * n));
				Coefficients[k, 2] = 1.0;
			}
			return Coefficients;
		}
	}



}
/*
function test_bp1
fc1 = 1e3; fc2 = 10e3; fs = 48e3;
[b, a] =  butter2_bp_direct( fc1, fc2, fs)
figure, freqz( b, a)
[b, a] =  butter(2, [fc1, fc2]*2/fs)
figure,freqz( b, a)
function[b1, a1] =  butter2_bp_direct( fc1, fc2, fs)
q1 = sqrt(2);
fc = abs( fc2 - fc1 );% equivalent cut-off frequency of lowpass filter is (fc2-fc1)
k = tan( pi* fc / fs);
r = cos( 2 * pi* ( fc1 + fc2 ) / 2 / fs ) / cos( 2 * pi* fc / 2 / fs);
b1 = k^2 *[1, 0, -2, 0, 1];
a10 =   k^4 + k*q1  + 1;
a11 =   -2*k *q1*r    - 4*r;
a12 =   -2 *k*k  +4*r*r  + 2;
a13 =   2*k *q1*r       -    4*r;
a14 =  k^2-k*q1+1;
a1 = [a10, a11, a12, a13, a14]/a10;
b1 = b1/a10;




Evaluating  -2\cos((2k+1)\pi/2n) for k=3, k=4, and k=5, we get the coefficients of the three first order terms  -2\cos(7\pi/12)=0.5176, -2\cos(9\pi/12)=1.4142, and  -2\cos(9\pi/12)=1.319. 
\begin{displaymath}
H(s)=\frac{1}{(s^2+0.5176 s+1)(s^2+1.4142 s+1)(s^2+1.9319 s+1)}
\end{displaymath}


	
August 3, 2015

If you find a problem with this code, please leave us a note on:
http://www.iowahills.com/feedbackcomments.html

Please note that the code in this file is not stand alone code.
In particular, all the code needed to get the 4th order bandpass and notch
sections to 2nd order setions is not in this file.

For more complete code, see our Code Kit on this page. It also contains 
the required 4th order root solver needed for IIR band pass and notch filters.

http://www.iowahills.com/A7ExampleCodePage.html


This is bilinear transform code for IIR filters.
In order to use these equations, you must define the variables A, B, C, D, E, and F
which are the coefficients for the low pass prototype function H(s).
H(s) = ( D*s^2 + E*s + F ) / (A*s^2 + B*s + C)

For example, if you are doing a 6 pole Butterworth, then D = E = 0 and F = 1
and the 3 denominator polynomials are.
1*s^2 +  0.5176380902 s  +  1.0000000000
1*s^2 +  1.4142135624 s  +  1.0000000000
1*s^2 +  1.9318516526 s  +  1.0000000000

OmegaC and BW are in terms of Nyquist. For example, if the sampling frequency = 20 kHz
and the 3 dB corner frquncy is 1.5 kHz, then OmegaC = 0.15

These define T and Q. Q is only used for the bandpass and notch filters.
M_PI_2 and M_PI_4 are Pi/2 and Pi/4 respectively, and should already be defined in math.h
The Q correction given here was derived from a curve fit of bandwidth error using an uncorrected Q.

 T = 2.0 * tan(OmegaC * M_PI_2);
 Q = 1.0 + OmegaC;
 if(Q > 1.95)Q = 1.95;       // Q must be < 2
 Q = 0.8 * tan(Q * M_PI_4);  // This is the correction factor.
 Q = OmegaC / BW / Q;        // This is the corrected Q.


This code calculates the a's and b's for H(z).
b's are the numerator  a's are the denominator

  if(Filt.PassType == LPF)
   {
	if(A == 0.0 && D == 0.0) // 1 pole case
	 {
	  Arg = (2.0*B + C*T);
	  a2[j] = 0.0;
	  a1[j] = (-2.0*B + C*T) / Arg;
	  a0[j] = 1.0;

	  b2[j] = 0.0;
	  b1[j] = (-2.0*E + F*T) / Arg * C/F;
	  b0[j] = ( 2.0*E + F*T) / Arg * C/F;
	 }
	else // 2 poles
	 {
	  Arg = (4.0*A + 2.0*B*T + C*T*T);
	  a2[j] = (4.0*A - 2.0*B*T + C*T*T) / Arg;
	  a1[j] = (2.0*C*T*T - 8.0*A) / Arg;
	  a0[j] = 1.0;

	  b2[j] = (4.0*D - 2.0*E*T + F*T*T) / Arg * C/F;
	  b1[j] = (2.0*F*T*T - 8.0*D) / Arg * C/F;
	  b0[j] = (4*D + F*T*T + 2.0*E*T) / Arg * C/F;
	 }
   }

  if(Filt.PassType == HPF)
   {
	if(A == 0.0 && D == 0.0) // 1 pole
	 {
	  Arg = 2.0*C + B*T;
	  a2[j] = 0.0;
	  a1[j] = (B*T - 2.0*C) / Arg;
	  a0[j] = 1.0;

	  b2[j] = 0.0;
	  b1[j] = (E*T - 2.0*F) / Arg * C/F;
	  b0[j] = (E*T + 2.0*F) / Arg * C/F;
	 }
	else  // 2 poles
	 {
	  Arg = A*T*T + 4.0*C + 2.0*B*T;
	  a2[j] = (A*T*T + 4.0*C - 2.0*B*T) / Arg;
	  a1[j] = (2.0*A*T*T - 8.0*C) / Arg;
	  a0[j] = 1.0;

	  b2[j] = (D*T*T - 2.0*E*T + 4.0*F) / Arg * C/F;
	  b1[j] = (2.0*D*T*T - 8.0*F) / Arg * C/F;
	  b0[j] = (D*T*T + 4.0*F + 2.0*E*T) / Arg * C/F;
	 }
   }

  if(Filt.PassType == BPF)
   {
	if(A == 0.0 && D == 0.0) // 1 pole
	 {
	  Arg = 4.0*B*Q + 2.0*C*T + B*Q*T*T;
	  a2[k] = (B*Q*T*T + 4.0*B*Q - 2.0*C*T) / Arg;
	  a1[k] = (2.0*B*Q*T*T - 8.0*B*Q) / Arg ;
	  a0[k] = 1.0;

	  b2[k] = (E*Q*T*T + 4.0*E*Q - 2.0*F*T) / Arg * C/F;
	  b1[k] = (2.0*E*Q*T*T - 8.0*E*Q) / Arg * C/F;
	  b0[k] = (4.0*E*Q + 2.0*F*T + E*Q*T*T) / Arg * C/F;
	  k++;
	 }
	else //2 Poles
	 {
	  a4[j] = (16.0*A*Q*Q + A*Q*Q*T*T*T*T + 8.0*A*Q*Q*T*T - 2.0*B*Q*T*T*T - 8.0*B*Q*T + 4.0*C*T*T) * F;
	  a3[j] = (4.0*T*T*T*T*A*Q*Q - 4.0*Q*T*T*T*B + 16.0*Q*B*T - 64.0*A*Q*Q) * F;
	  a2[j] = (96.0*A*Q*Q - 16.0*A*Q*Q*T*T + 6.0*A*Q*Q*T*T*T*T - 8.0*C*T*T) * F;
	  a1[j] = (4.0*T*T*T*T*A*Q*Q + 4.0*Q*T*T*T*B - 16.0*Q*B*T - 64.0*A*Q*Q) * F;
	  a0[j] = (16.0*A*Q*Q + A*Q*Q*T*T*T*T + 8.0*A*Q*Q*T*T + 2.0*B*Q*T*T*T + 8.0*B*Q*T + 4.0*C*T*T) * F;

	  b4[j] = (8.0*D*Q*Q*T*T - 8.0*E*Q*T + 16.0*D*Q*Q - 2.0*E*Q*T*T*T + D*Q*Q*T*T*T*T + 4.0*F*T*T) * C;
	  b3[j] = (16.0*E*Q*T - 4.0*E*Q*T*T*T - 64.0*D*Q*Q + 4.0*D*Q*Q*T*T*T*T) * C;
	  b2[j] = (96.0*D*Q*Q - 8.0*F*T*T + 6.0*D*Q*Q*T*T*T*T - 16.0*D*Q*Q*T*T) * C;
	  b1[j] = (4.0*D*Q*Q*T*T*T*T - 64.0*D*Q*Q + 4.0*E*Q*T*T*T - 16.0*E*Q*T) * C;
	  b0[j] = (16.0*D*Q*Q + 8.0*E*Q*T + 8.0*D*Q*Q*T*T + 2.0*E*Q*T*T*T + 4.0*F*T*T + D*Q*Q*T*T*T*T) * C;
	 }
   }

  if(Filt.PassType == NOTCH)
   {
	if(A == 0.0 && D == 0.0) // 1 pole
	 {
	  Arg = 2.0*B*T + C*Q*T*T + 4.0*C*Q;
	  a2[k] = (4.0*C*Q - 2.0*B*T + C*Q*T*T) / Arg;
	  a1[k] = (2.0*C*Q*T*T - 8.0*C*Q) / Arg;
	  a0[k] = 1.0;

	  b2[k] = (4.0*F*Q - 2.0*E*T + F*Q*T*T) / Arg * C/F;
	  b1[k] = (2.0*F*Q*T*T - 8.0*F*Q) / Arg *C/F;
	  b0[k] = (2.0*E*T + F*Q*T*T +4.0*F*Q) / Arg *C/F;
	  k++;
	 }
	else
	 {
	  a4[j] = (4.0*A*T*T - 2.0*B*T*T*T*Q + 8.0*C*Q*Q*T*T - 8.0*B*T*Q + C*Q*Q*T*T*T*T + 16.0*C*Q*Q) * -F;
	  a3[j] = (16.0*B*T*Q + 4.0*C*Q*Q*T*T*T*T - 64.0*C*Q*Q - 4.0*B*T*T*T*Q) * -F;
	  a2[j] = (96.0*C*Q*Q - 8.0*A*T*T - 16.0*C*Q*Q*T*T + 6.0*C*Q*Q*T*T*T*T) * -F;
	  a1[j] = (4.0*B*T*T*T*Q - 16.0*B*T*Q - 64.0*C*Q*Q + 4.0*C*Q*Q*T*T*T*T) * -F;
	  a0[j] = (4.0*A*T*T + 2.0*B*T*T*T*Q + 8.0*C*Q*Q*T*T + 8.0*B*T*Q + C*Q*Q*T*T*T*T + 16.0*C*Q*Q) * -F;

	  b4[j] = (2.0*E*T*T*T*Q - 4.0*D*T*T - 8.0*F*Q*Q*T*T + 8.0*E*T*Q - 16.0*F*Q*Q - F*Q*Q*T*T*T*T) * C;
	  b3[j] = (64.0*F*Q*Q + 4.0*E*T*T*T*Q - 16.0*E*T*Q - 4.0*F*Q*Q*T*T*T*T) * C;
	  b2[j] = (8.0*D*T*T - 96.0*F*Q*Q + 16.0*F*Q*Q*T*T - 6.0*F*Q*Q*T*T*T*T) * C;
	  b1[j] = (16.0*E*T*Q - 4.0*E*T*T*T*Q + 64.0*F*Q*Q - 4.0*F*Q*Q*T*T*T*T) * C;
	  b0[j] = (-4.0*D*T*T - 2.0*E*T*T*T*Q - 8.0*E*T*Q - 8.0*F*Q*Q*T*T - F*Q*Q*T*T*T*T - 16.0*F*Q*Q) * C;
     }
   }



*/
