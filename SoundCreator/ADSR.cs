using System;

namespace SoundCreator {

	internal class ADSR {
		private int state;
		private double output;

		private double attackRate;
		private double attackCoef;
		private double attackBase;

		private double decayRate;
		private double decayCoef;
		private double decayBase;

		private double sustainLevel;

		private double releaseRate;
		private double releaseCoef;
		private double releaseBase;

		private double targetRatioA;
		private double targetRatioDR;

		private const int env_idle = 0;
		private const int env_attack = 1;
		private const int env_decay = 2;
		private const int env_sustain = 3;
		private const int env_release = 4;

		public double process() {
			switch (state) {
				case env_idle:
					break;

				case env_attack:
					output = attackBase + output * attackCoef;
					if (output >= 1.0) {
						output = 1.0;
						state = env_decay;
					}
					break;

				case env_decay:
					output = decayBase + output * decayCoef;
					if (output <= sustainLevel) {
						output = sustainLevel;
						state = env_sustain;
					}
					break;

				case env_sustain:
					break;

				case env_release:
					output = releaseBase + output * releaseCoef;
					if (output <= 0.0) {
						output = 0.0;
						state = env_idle;
					}
					break;
			}
			return output;
		}

		public void gate( bool gate ) {
			if (gate)
				state = env_attack;
			else if (state != env_idle)
				state = env_release;
		}

		public int getState() {
			return state;
		}

		public void reset() {
			state = env_idle;
			output = 0.0;
		}

		public double getOutput() {
			return output;
		}

		public void setAttackRate( double rate ) {
			attackRate = rate;
			attackCoef = calcCoef(rate, targetRatioA);
			attackBase = (1.0 + targetRatioA) * (1.0 - attackCoef);
		}

		public void setDecayRate( double rate ) {
			decayRate = rate;
			decayCoef = calcCoef(rate, targetRatioDR);
			decayBase = (sustainLevel - targetRatioDR) * (1.0 - decayCoef);
		}

		public void setReleaseRate( double rate ) {
			releaseRate = rate;
			releaseCoef = calcCoef(rate, targetRatioDR);
			releaseBase = -targetRatioDR * (1.0 - releaseCoef);
		}

		private double calcCoef( double rate, double targetRatio ) {
			return Math.Exp(-Math.Log((1.0 + targetRatio) / targetRatio) / rate);
		}

		public void setSustainLevel( double level ) {
			sustainLevel = level;
			decayBase = (sustainLevel - targetRatioDR) * (1.0 - decayCoef);
		}

		public void setTargetRatioA( double targetRatio ) {
			if (targetRatio < 0.000000001)
				targetRatio = 0.000000001;  // -180 dB
			targetRatioA = targetRatio;
			attackCoef = calcCoef(attackRate, targetRatioA);
			attackBase = (1.0 + targetRatioA) * (1.0 - attackCoef);
		}

		public void setTargetRatioDR( double targetRatio ) {
			if (targetRatio < 0.000000001)
				targetRatio = 0.000000001;  // -180 dB
			targetRatioDR = targetRatio;
			decayCoef = calcCoef(decayRate, targetRatioDR);
			releaseCoef = calcCoef(releaseRate, targetRatioDR);
			decayBase = (sustainLevel - targetRatioDR) * (1.0 - decayCoef);
			releaseBase = -targetRatioDR * (1.0 - releaseCoef);
		}
	}
}