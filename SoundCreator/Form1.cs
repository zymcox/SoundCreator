using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

// Start 160317 
// Game sound builder
// Game Sound Builder   *.gsb  Settings fil (xml)


// Undo-Redo
// Save .wav                               OK
// Save Load alla inställningar            OK

namespace SoundCreator {

	public struct RndS {
		public bool FilterLock;
		public bool ReverbLock;
		public int NoRandomOsc;
		public int NoRandomLFO;
		public double MaxRandomFrequency;
		public double MinRandomFrequency;
	};

	public partial class Form1 : Form {

		// Init
		public static int MaxOscillators = 128;

		public static int Samplerate = 44100;
		public static int TimeMS = 6000;
		public static int OscArraySize = (Samplerate * TimeMS) / 1000;
		public static double MaxAmplitude = 32768.0;

		private static RndS RndSettings = new RndS();


		private static double MaxSquareDuty =          100.0;
		private static double MinSquareDuty =            0.0;
		private static double MaxFrequency =          7040.0;
		private static double MidFrequency =           880.0;
		private static double MinFrequency =             1.0;
		private static double MaxStartPhase =          360.0;
		private static double MinStartPhase =            0.0;
		private static double MaxVolume =           655360.0;
		private static double MidVolume =            65536.0;
		private static double MinVolume =                0.0;
		private static double MaxAttack =             TimeMS;
		private static double MinAttack =                0.0;
		private static double MaxDecay =              TimeMS;
		private static double MinDecay =                 0.0;
		private static double MaxSustain =           65536.0;
		private static double MidSustain =            2048.0;
		private static double MinSustain =               0.0;
		private static double MaxRelease =            TimeMS;
		private static double MinRelease =               0.0;
		private static double MaxGateTime =           TimeMS;
		private static double MinGateTime =              0.0;
		private static double MaxDelay =          TimeMS/2.0;
		private static double MinDelay =                 0.0;

		private static double MaxAMDepth =             200.0;
		private static double MinAMDepth =               0.0;
		private static double MaxFMDepth =             400.0;
		private static double MinFMDepth =               0.0;
		private static double MaxPMDepth =             400.0;
		private static double MinPMDepth =               0.0;
		private static double MaxSDDepth =             200.0;
		private static double MinSDDepth =               0.0;

		private static double MaxZoom =                100.0;
		private static double MidZoom =                 10.0;
		private static double MinZoom =                  0.05;
		private static double MaxPosition =            100.0;
		private static double MinPosition =              0.0;
		private static double OscilloscopeZoom =        5.0;
		private static double OscilloscopePosition =    0.0;

		private static double MaxFilterFrequency =   20000.0;
		private static double MidFilterFrequency =    4000.0;
		private static double MinFilterFrequency =      10.0;

		private static double MaxFFDepth =             400.0;
		private static double MinFFDepth =               0.0;

		private static double MaxStereoDelay = Form1.Samplerate/10;
		private static double MinStereoDelay =          10.0;

		private int OscillatorNumber = 0;
		private double[][] OscArray;


		private OscillatorData[] OscData = new OscillatorData[MaxOscillators];
		private OscillatorData[] OscDataJingle = new OscillatorData[MaxOscillators];
		private string[] strJingle;
		private string[] OkNotes = new string[8];

		private Oscillator OscillatorObj = new Oscillator();
		private MixerData MixData = new MixerData();
		private Mixer MixerObj = new Mixer();
		private LoadnSave LSObj = new LoadnSave();
		private cUndo UndoObj = new cUndo();

		private bool InsertNewData = true;

		private static Slider SliderSquareDutyObj;
		private static Slider SliderFrequencyObj;
		private static Slider SliderPhaseObj;
		private static Slider SliderVolumeObj;
		private static Slider SliderAttackObj;
		private static Slider SliderDecayObj;
		private static Slider SliderSustainObj;
		private static Slider SliderReleaseObj;
		private static Slider SliderGateTimeObj;
		private static Slider SliderDelayObj;

		private static Slider SliderAMDepthObj;
		private static Slider SliderFMDepthObj;
		private static Slider SliderPMDepthObj;
		private static Slider SliderSDDepthObj;

		private static Slider SliderZoomObj;
		private static Slider SliderPositionObj;

		private static Slider SliderFilterFrequencyObj1;
		private static Slider SliderFilterFrequencyObj2;
		private static Slider SliderFilterFrequencyDepthObj;

		private static Slider SliderStereoDelayObj;

		private static Oscilloscope OscilloscopeObj;
		// Init end

		public Form1() {
			InitializeComponent();

			DoubleBuffered = true;
			OscData = OscillatorObj.ResetAll(OscData);
			OscDataJingle = OscillatorObj.ResetAll(OscData);
			MixData = MixerObj.Reset(MixData);
			RndSettings = ResetRndSettings(RndSettings);

			SliderSquareDutyObj = new Slider(pbSquareDutySlider, OscData[OscillatorNumber].SquareDuty, MinSquareDuty, MaxSquareDuty);
			SliderFrequencyObj = new Slider(pbFrequencySlider, OscData[OscillatorNumber].Frequency, MinFrequency, MaxFrequency, MidFrequency);
			SliderPhaseObj = new Slider(pbPhaseSlider, OscData[OscillatorNumber].StartPhase, MinStartPhase, MaxStartPhase);
			SliderVolumeObj = new Slider(pbVolumeSlider, OscData[OscillatorNumber].Volume, MinVolume, MaxVolume, MidVolume);
			SliderAttackObj = new Slider(pbAttackSlider, OscData[OscillatorNumber].Attack, MinAttack, MaxAttack);
			SliderDecayObj = new Slider(pbDecaySlider, OscData[OscillatorNumber].Decay, MinDecay, MaxDecay);
			SliderSustainObj = new Slider(pbSustainSlider, OscData[OscillatorNumber].Sustain, MinSustain, MaxSustain, MidSustain);
			SliderReleaseObj = new Slider(pbReleaseSlider, OscData[OscillatorNumber].Release, MinRelease, MaxRelease);
			SliderGateTimeObj = new Slider(pbGateTimeSlider, OscData[OscillatorNumber].GateTime, MinGateTime, MaxGateTime);
			SliderDelayObj = new Slider(pbDelaySlider, OscData[OscillatorNumber].Delay, MinDelay, MaxDelay);

			SliderAMDepthObj = new Slider(pbAMDepthSlider, OscData[OscillatorNumber].AMDepth, MinAMDepth, MaxAMDepth);
			SliderFMDepthObj = new Slider(pbFMDepthSlider, OscData[OscillatorNumber].FMDepth, MinFMDepth, MaxFMDepth);
			SliderPMDepthObj = new Slider(pbPMDepthSlider, OscData[OscillatorNumber].PMDepth, MinPMDepth, MaxPMDepth);
			SliderSDDepthObj = new Slider(pbSDDepthSlider, OscData[OscillatorNumber].SDDepth, MinSDDepth, MaxSDDepth);

			SliderZoomObj = new Slider(pbZoomSlider, OscilloscopeZoom, MinZoom, MaxZoom, MidZoom);
			SliderPositionObj = new Slider(pbPositionSlider, OscilloscopePosition, MinPosition, MaxPosition);

			SliderFilterFrequencyObj1 = new Slider(pbFilterFrequencySlider1, MixData.FilterFrequency1, MinFilterFrequency, MaxFilterFrequency, MidFilterFrequency);
			SliderFilterFrequencyObj2 = new Slider(pbFilterFrequencySlider2, MixData.FilterFrequency2, MinFilterFrequency, MaxFilterFrequency, MidFilterFrequency);
			SliderFilterFrequencyDepthObj = new Slider(pbFilterFrequencyDepth, MixData.FFDepth, MinFFDepth, MaxFFDepth);

			SliderStereoDelayObj = new Slider(pbSteroDelay, MixData.StereoDelay, MinStereoDelay, MaxStereoDelay);

			OscilloscopeObj = new Oscilloscope(pbOscilloscope, pbFFT);
		}

		private void PresentData( OscillatorData[] OD, int ON ) {
			cbWaveForm.SelectedIndex = OD[ON].WaveType;
			cbActive.Checked = OD[ON].Active;
			cbSoundOn.Checked = OD[ON].SoundOut;

			udAmplitudeModulationFromOsc.Value = OD[ON].VolumeFromOsc;
			udFrequencyModulatonFromOsc.Value = OD[ON].FrequencyFromOsc;
			udPhaseModulationFromOsc.Value = OD[ON].PhaseFromOsc;
			udRingModulationFromOsc.Value = OD[ON].RingModulationFromOsc;

			udSquareDutyFromOsc.Value = OD[ON].SquareDutyFromOsc;
			udSyncFromOsc.Value = OD[ON].SyncFromOsc;
			udOscBitResulotion.Value = 16 - OD[ON].OscBitResolution;

			tbPhase.Text = OD[ON].StartPhase.ToString();
			tbVolume.Text = OD[ON].Volume.ToString();
			tbAttack.Text = OD[ON].Attack.ToString();
			tbDecay.Text = OD[ON].Decay.ToString();
			tbSustain.Text = OD[ON].Sustain.ToString();
			tbRelease.Text = OD[ON].Release.ToString();
			tbFrequency.Text = OD[ON].Frequency.ToString();
			tbGateTime.Text = OD[ON].GateTime.ToString();
			tbDelay.Text = OD[ON].Delay.ToString();

			cbFilterType.SelectedIndex = MixData.FilterType;
			tbFilterFrequency1.Text = MixData.FilterFrequency1.ToString();
			tbFilterFrequency2.Text = MixData.FilterFrequency2.ToString();

			SliderSquareDutyObj.Draw(OD[ON].SquareDuty);
			SliderFrequencyObj.Draw(OD[ON].Frequency);
			SliderPhaseObj.Draw(OD[ON].StartPhase);
			SliderVolumeObj.Draw(OD[ON].Volume);
			SliderAttackObj.Draw(OD[ON].Attack);
			SliderDecayObj.Draw(OD[ON].Decay);
			SliderSustainObj.Draw(OD[ON].Sustain);
			SliderReleaseObj.Draw(OD[ON].Release);
			SliderGateTimeObj.Draw(OD[ON].GateTime);
			SliderDelayObj.Draw(OD[ON].Delay);

			SliderAMDepthObj.Draw(OD[ON].AMDepth);
			SliderFMDepthObj.Draw(OD[ON].FMDepth);
			SliderPMDepthObj.Draw(OD[ON].PMDepth);
			SliderSDDepthObj.Draw(OD[ON].SDDepth);

			SliderZoomObj.Draw(OscilloscopeZoom);
			SliderPositionObj.Draw(OscilloscopePosition);

			SliderFilterFrequencyObj1.Draw(MixData.FilterFrequency1);
			SliderFilterFrequencyObj2.Draw(MixData.FilterFrequency2);
			SliderFilterFrequencyDepthObj.Draw(MixData.FFDepth);

			SliderStereoDelayObj.Draw(MixData.StereoDelay);

			cbAGC.Checked = MixData.AGC;
			cbRemoveDC.Checked = MixData.RemoveDC;
			cbMovingAverageFilter.Checked = MixData.MovingAverageFilter;

			cbReverb.Checked = MixData.Reverb;
			udDelay1.Value = (int)MixData.Delay1;
			udGain1.Value = (int)MixData.Gain1;
			udDelay2.Value = (int)MixData.Delay2;
			udGain2.Value = (int)MixData.Gain2;
			udDelay3.Value = (int)MixData.Delay3;
			udGain3.Value = (int)MixData.Gain3;
			udDelay4.Value = (int)MixData.Delay4;
			udGain4.Value = (int)MixData.Gain4;
			udDelay5.Value = (int)MixData.Delay5;
			udGain5.Value = (int)MixData.Gain5;
			udDelay6.Value = (int)MixData.Delay6;
			udGain6.Value = (int)MixData.Gain6;
			udDelay7.Value = (int)MixData.Delay7;
			udGain7.Value = (int)MixData.Gain7;

			udFilterOrder.Value = MixData.FilterOrder;
			udFFModulationFromOsc.Value = MixData.FilterFrequencyFromOsc;
			udBitResulotion.Value = 16 - MixData.BitResolution;
			udDelay.Value = MixData.StereoDelay;
			cbStereo.Checked = MixData.Stereo;

			OscillatorObj.ShowWaveForm(pbWaveForm, OD[ON]);

			btnUndo.Enabled = UndoObj.GetUndoButton();
			btnRedo.Enabled = UndoObj.GetRedoButton();

		}

		private RndS ResetRndSettings( RndS RS ) {
			RS.FilterLock = false;
			RS.ReverbLock = false;
			RS.NoRandomLFO = 0;
			RS.NoRandomOsc = 2;
			RS.MinRandomFrequency = 1;
			RS.MaxRandomFrequency = 7040;
			return RS;
		}

		private void CalcAndPlay(bool PlayJingle) {
			if (PlayJingle) {
				OscArray = OscillatorObj.CreateWave(OscDataJingle, MixData);
				MixerObj.CreateSoundWav(MixData, OscDataJingle, OscArray);
				OscilloscopeObj.SetView(MixerObj.GetRawSoundData(), OscilloscopePosition, OscilloscopeZoom);
				OscilloscopeObj.DrawFFT(MixerObj.GetRawSoundData());
				
			} else {
				OscArray = OscillatorObj.CreateWave(OscData, MixData);
				MixerObj.CreateSoundWav(MixData, OscData, OscArray);
				OscilloscopeObj.SetView(MixerObj.GetRawSoundData(), OscilloscopePosition, OscilloscopeZoom);
				OscilloscopeObj.DrawFFT(MixerObj.GetRawSoundData());
				if (InsertNewData) {
					Items a = new Items();
					OscillatorData[] Od = new OscillatorData[OscData.Length];
					Array.Copy(OscData, Od, OscData.Length);
					a.OD = Od;
					a.MD = MixData;
					UndoObj.InsertNew(a);
					btnUndo.Enabled = UndoObj.GetUndoButton();
					btnRedo.Enabled = UndoObj.GetRedoButton();
				}
				InsertNewData = true;
			}
		}

		private void btnTest_Click( object sender, EventArgs e ) {
			CalcAndPlay(false);
		}

		private void btnPrevOsc_Click( object sender, EventArgs e ) {
			OscillatorNumber = OscillatorNumber - 1;
			if (OscillatorNumber <= 0) OscillatorNumber = 0;
			if (OscillatorNumber >= 10) {
				labelOscNumber.Text = OscillatorNumber.ToString();
			} else {
				labelOscNumber.Text = "0" + OscillatorNumber.ToString();
			}
			PresentData(OscData, OscillatorNumber);
		}

		private void btnNextOsc_Click( object sender, EventArgs e ) {
			OscillatorNumber = OscillatorNumber + 1;
			if (OscillatorNumber >= MaxOscillators - 1) OscillatorNumber = MaxOscillators - 1;
			if (OscillatorNumber >= 10) {
				labelOscNumber.Text = OscillatorNumber.ToString();
			} else {
				labelOscNumber.Text = "0" + OscillatorNumber.ToString();
			}
			PresentData(OscData, OscillatorNumber);
		}

		private void cbWaveForm_SelectedIndexChanged( object sender, EventArgs e ) {
			OscData[OscillatorNumber].WaveType = cbWaveForm.SelectedIndex;
			OscillatorObj.ShowWaveForm(pbWaveForm, OscData[OscillatorNumber]);
			//CalcAndPlay();
		}

		private void cbFilterType_SelectedIndexChanged( object sender, EventArgs e ) {
			MixData.FilterType = cbFilterType.SelectedIndex;
			//CalcAndPlay();
		}

		private void cbActive_CheckedChanged( object sender, EventArgs e ) {
			OscData[OscillatorNumber].Active = cbActive.Checked;
		}

		private void cbSoundOn_CheckedChanged( object sender, EventArgs e ) {
			OscData[OscillatorNumber].SoundOut = cbSoundOn.Checked;
		}

		private void udAmplitudeModulationFromOsc_ValueChanged( object sender, EventArgs e ) {
			int newvalue = (int)udAmplitudeModulationFromOsc.Value;
			int oldvalue = OscData[OscillatorNumber].VolumeFromOsc;
			if (newvalue < OscillatorNumber) {
				oldvalue = newvalue;
			} else {
				newvalue = oldvalue;
			}
			udAmplitudeModulationFromOsc.Value = newvalue;
			OscData[OscillatorNumber].VolumeFromOsc = oldvalue;
		}

		private void udFrequencyModulatonFromOsc_ValueChanged( object sender, EventArgs e ) {
			int newvalue = (int)udFrequencyModulatonFromOsc.Value;
			int oldvalue = OscData[OscillatorNumber].FrequencyFromOsc;
			if (newvalue < OscillatorNumber) {
				oldvalue = newvalue;
			} else {
				newvalue = oldvalue;
			}
			udFrequencyModulatonFromOsc.Value = newvalue;
			OscData[OscillatorNumber].FrequencyFromOsc = oldvalue;
		}

		private void udPhaseModulationFromOsc_ValueChanged( object sender, EventArgs e ) {
			int newvalue = (int)udPhaseModulationFromOsc.Value;
			int oldvalue = OscData[OscillatorNumber].PhaseFromOsc;
			if (newvalue < OscillatorNumber) {
				oldvalue = newvalue;
			} else {
				newvalue = oldvalue;
			}
			udPhaseModulationFromOsc.Value = newvalue;
			OscData[OscillatorNumber].PhaseFromOsc = oldvalue;
		}

		private void udRingModulationFromOsc_ValueChanged( object sender, EventArgs e ) {
			int newvalue = (int)udRingModulationFromOsc.Value;
			int oldvalue = OscData[OscillatorNumber].RingModulationFromOsc;
			if (newvalue < OscillatorNumber) {
				oldvalue = newvalue;
			} else {
				newvalue = oldvalue;
			}
			udRingModulationFromOsc.Value = newvalue;
			OscData[OscillatorNumber].RingModulationFromOsc = oldvalue;
		}

		private void udSquareDutyFromOsc_ValueChanged( object sender, EventArgs e ) {
			int newvalue = (int)udSquareDutyFromOsc.Value;
			int oldvalue = OscData[OscillatorNumber].SquareDutyFromOsc;
			if (newvalue < OscillatorNumber) {
				oldvalue = newvalue;
			} else {
				newvalue = oldvalue;
			}
			udSquareDutyFromOsc.Value = newvalue;
			OscData[OscillatorNumber].SquareDutyFromOsc = oldvalue;
		}

		private void udSyncFromOsc_ValueChanged( object sender, EventArgs e ) {
			int newvalue = (int)udSyncFromOsc.Value;
			int oldvalue = OscData[OscillatorNumber].SyncFromOsc;
			if (newvalue < OscillatorNumber) {
				oldvalue = newvalue;
			} else {
				newvalue = oldvalue;
			}
			udSyncFromOsc.Value = newvalue;
			OscData[OscillatorNumber].SyncFromOsc = oldvalue;
		}

		private void tbPhase_TextChanged( object sender, EventArgs e ) {
			Double.TryParse(tbPhase.Text, out OscData[OscillatorNumber].StartPhase);
			SliderPhaseObj.Draw(OscData[OscillatorNumber].StartPhase);
		}

		private void tbVolume_TextChanged( object sender, EventArgs e ) {
			Double.TryParse(tbVolume.Text, out OscData[OscillatorNumber].Volume);
			SliderVolumeObj.Draw(OscData[OscillatorNumber].Volume);
		}

		private void tbAttack_TextChanged( object sender, EventArgs e ) {
			Double.TryParse(tbAttack.Text, out OscData[OscillatorNumber].Attack);
			SliderAttackObj.Draw(OscData[OscillatorNumber].Attack);
		}

		private void tbDecay_TextChanged( object sender, EventArgs e ) {
			Double.TryParse(tbDecay.Text, out OscData[OscillatorNumber].Decay);
			SliderDecayObj.Draw(OscData[OscillatorNumber].Decay);
		}

		private void tbSustain_TextChanged( object sender, EventArgs e ) {
			Double.TryParse(tbSustain.Text, out OscData[OscillatorNumber].Sustain);
			SliderSustainObj.Draw(OscData[OscillatorNumber].Sustain);
		}

		private void tbRelease_TextChanged( object sender, EventArgs e ) {
			double.TryParse(tbRelease.Text, out OscData[OscillatorNumber].Release);
			SliderReleaseObj.Draw(OscData[OscillatorNumber].Release);
		}

		private void tbFrequency_TextChanged( object sender, EventArgs e ) {
			Double.TryParse(tbFrequency.Text, out OscData[OscillatorNumber].Frequency);
			SliderFrequencyObj.Draw(OscData[OscillatorNumber].Frequency);
		}

		private void tbGateTime_TextChanged( object sender, EventArgs e ) {
			Double.TryParse(tbGateTime.Text, out OscData[OscillatorNumber].GateTime);
			SliderGateTimeObj.Draw(OscData[OscillatorNumber].GateTime);
		}

		private void tbDelay_TextChanged( object sender, EventArgs e ) {
			Double.TryParse(tbDelay.Text, out OscData[OscillatorNumber].Delay);
			SliderDelayObj.Draw(OscData[OscillatorNumber].Delay);
		}

		private void btnCopyToNext_Click( object sender, EventArgs e ) {
			if (OscillatorNumber < MaxOscillators - 1) {
				OscData[OscillatorNumber + 1] = OscData[OscillatorNumber];
			}
		}

		private void btnResetAll_Click( object sender, EventArgs e ) {
			OscillatorNumber = 0;
			OscData = OscillatorObj.ResetAll(OscData);
			MixData = MixerObj.Reset(MixData);
			labelOscNumber.Text = "00";
			OscilloscopeZoom = 5.0;
			OscilloscopePosition = 0.0;
			OscilloscopeObj.ResetView();
			PresentData(OscData, OscillatorNumber);
			CalcAndPlay(false);
		}

		private void tbFilterFrequency1_TextChanged( object sender, EventArgs e ) {
			Double.TryParse(tbFilterFrequency1.Text, out MixData.FilterFrequency1);
			SliderFilterFrequencyObj1.Draw(MixData.FilterFrequency1);
		}

		private void tbFilterFrequency2_TextChanged( object sender, EventArgs e ) {
			Double.TryParse(tbFilterFrequency2.Text, out MixData.FilterFrequency2);
			SliderFilterFrequencyObj2.Draw(MixData.FilterFrequency2);
		}

		private void btnGlobalRndSin_Click( object sender, EventArgs e ) {
			OscData[OscillatorNumber] = OscillatorObj.CreateRandomSinus(OscData[OscillatorNumber]);
			OscillatorObj.ShowWaveForm(pbWaveForm, OscData[OscillatorNumber]);
			CalcAndPlay(false);
		}

		private void pbSquareDutySlider_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderSquareDutyObj.MouseDown(e);
			OscData[OscillatorNumber].SquareDuty = DisplayValue;
			OscillatorObj.ShowWaveForm(pbWaveForm, OscData[OscillatorNumber]);
		}

		private void pbSquareDutySlider_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderSquareDutyObj.MouseMove(e, 5.0);
				OscData[OscillatorNumber].SquareDuty = DisplayValue;
				OscillatorObj.ShowWaveForm(pbWaveForm, OscData[OscillatorNumber]);
			}
		}

		private void pbFrequencySlider_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderFrequencyObj.MouseDown(e);
			OscData[OscillatorNumber].Frequency = DisplayValue;
			tbFrequency.Text = DisplayValue.ToString();
		}

		private void pbFrequencySlider_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderFrequencyObj.MouseMove(e, 5.0);
				OscData[OscillatorNumber].Frequency = DisplayValue;
				tbFrequency.Text = DisplayValue.ToString();
			}
		}

		private void pbPhaseSlider_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderPhaseObj.MouseDown(e);
			OscData[OscillatorNumber].StartPhase = DisplayValue;
			tbPhase.Text = DisplayValue.ToString();
			OscillatorObj.ShowWaveForm(pbWaveForm, OscData[OscillatorNumber]);
		}

		private void pbPhaseSlider_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderPhaseObj.MouseMove(e, 5.0);
				OscData[OscillatorNumber].StartPhase = DisplayValue;
				tbPhase.Text = DisplayValue.ToString();
				OscillatorObj.ShowWaveForm(pbWaveForm, OscData[OscillatorNumber]);
			}
		}

		private void pbVolumeSlider_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderVolumeObj.MouseDown(e);
			OscData[OscillatorNumber].Volume = DisplayValue;
			tbVolume.Text = DisplayValue.ToString();
			OscillatorObj.ShowWaveForm(pbWaveForm, OscData[OscillatorNumber]);
		}

		private void pbVolumeSlider_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderVolumeObj.MouseMove(e, 5.0);
				OscData[OscillatorNumber].Volume = DisplayValue;
				tbVolume.Text = DisplayValue.ToString();
				OscillatorObj.ShowWaveForm(pbWaveForm, OscData[OscillatorNumber]);
			}
		}

		private void pbAttackSlider_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderAttackObj.MouseDown(e);
			OscData[OscillatorNumber].Attack = DisplayValue;
			tbAttack.Text = DisplayValue.ToString();
		}

		private void pbAttackSlider_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderAttackObj.MouseMove(e, 5.0);
				OscData[OscillatorNumber].Attack = DisplayValue;
				tbAttack.Text = DisplayValue.ToString();
			}
		}

		private void pbDecaySlider_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderDecayObj.MouseDown(e);
			OscData[OscillatorNumber].Decay = DisplayValue;
			tbDecay.Text = DisplayValue.ToString();
		}

		private void pbDecaySlider_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderDecayObj.MouseMove(e, 5.0);
				OscData[OscillatorNumber].Decay = DisplayValue;
				tbDecay.Text = DisplayValue.ToString();
			}
		}

		private void pbSustainSlider_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderSustainObj.MouseDown(e);
			OscData[OscillatorNumber].Sustain = DisplayValue;
			tbSustain.Text = DisplayValue.ToString();
		}

		private void pbSustainSlider_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderSustainObj.MouseMove(e, 5.0);
				OscData[OscillatorNumber].Sustain = DisplayValue;
				tbSustain.Text = DisplayValue.ToString();
			}
		}

		private void pbReleaseSlider_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderReleaseObj.MouseDown(e);
			OscData[OscillatorNumber].Release = DisplayValue;
			tbRelease.Text = DisplayValue.ToString();
		}

		private void pbReleaseSlider_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderReleaseObj.MouseMove(e, 5.0);
				OscData[OscillatorNumber].Release = DisplayValue;
				tbRelease.Text = DisplayValue.ToString();
			}
		}

		private void pbGateTimeSlider_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderGateTimeObj.MouseDown(e);
			OscData[OscillatorNumber].GateTime = DisplayValue;
			tbGateTime.Text = DisplayValue.ToString();
		}

		private void pbGateTimeSlider_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderGateTimeObj.MouseMove(e, 5.0);
				OscData[OscillatorNumber].GateTime = DisplayValue;
				tbGateTime.Text = DisplayValue.ToString();
			}
		}

		private void pbDelaySlider_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderDelayObj.MouseDown(e);
			OscData[OscillatorNumber].Delay = DisplayValue;
			tbDelay.Text = DisplayValue.ToString();
		}

		private void pbDelaySlider_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderDelayObj.MouseMove(e, 5.0);
				OscData[OscillatorNumber].Delay = DisplayValue;
				tbDelay.Text = DisplayValue.ToString();
			}
		}

		private void pbFilterFrequencySlider1_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderFilterFrequencyObj1.MouseDown(e);
			MixData.FilterFrequency1 = DisplayValue;
			tbFilterFrequency1.Text = DisplayValue.ToString();
			if (MixData.FilterFrequency1 > MixData.FilterFrequency2) {
				MixData.FilterFrequency2 = DisplayValue;
				SliderFilterFrequencyObj2.Draw(DisplayValue);
				tbFilterFrequency2.Text = DisplayValue.ToString();
			}
		}

		private void pbFilterFrequencySlider1_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderFilterFrequencyObj1.MouseMove(e, 5.0);
				MixData.FilterFrequency1 = DisplayValue;
				tbFilterFrequency1.Text = DisplayValue.ToString();
				if (MixData.FilterFrequency1 > MixData.FilterFrequency2) {
					MixData.FilterFrequency2 = DisplayValue;
					SliderFilterFrequencyObj2.Draw(DisplayValue);
					tbFilterFrequency2.Text = DisplayValue.ToString();
				}
			}
		}

		private void pbFilterFrequencySlider2_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderFilterFrequencyObj2.MouseDown(e);
			MixData.FilterFrequency2 = DisplayValue;
			tbFilterFrequency2.Text = DisplayValue.ToString();
			if (MixData.FilterFrequency1 > MixData.FilterFrequency2) {
				MixData.FilterFrequency1 = DisplayValue;
				SliderFilterFrequencyObj1.Draw(DisplayValue);
				tbFilterFrequency1.Text = DisplayValue.ToString();
			}
		}

		private void pbFilterFrequencySlider2_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderFilterFrequencyObj2.MouseMove(e, 5.0);
				MixData.FilterFrequency2 = DisplayValue;
				tbFilterFrequency2.Text = DisplayValue.ToString();
				if (MixData.FilterFrequency1 > MixData.FilterFrequency2) {
					MixData.FilterFrequency1 = DisplayValue;
					SliderFilterFrequencyObj1.Draw(DisplayValue);
					tbFilterFrequency1.Text = DisplayValue.ToString();
				}
			}
		}

		private void udFilterOrder_ValueChanged( object sender, EventArgs e ) {
			MixData.FilterOrder = (int)udFilterOrder.Value;
		}

		private void Form1_Shown( object sender, EventArgs e ) {
			PresentData(OscData, OscillatorNumber);
		}

		private void btPlay_Click( object sender, EventArgs e ) {
			MixerObj.PlaySound(MixerObj.GetSoundDotWave());
		}

		private void pbAMDepthSlider_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderAMDepthObj.MouseDown(e);
			OscData[OscillatorNumber].AMDepth = DisplayValue;
		}

		private void pbAMDepthSlider_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderAMDepthObj.MouseMove(e, 5.0);
				OscData[OscillatorNumber].AMDepth = DisplayValue;
			}
		}

		private void pbFMDepthSlider_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderFMDepthObj.MouseDown(e);
			OscData[OscillatorNumber].FMDepth = DisplayValue;
		}

		private void pbFMDepthSlider_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderFMDepthObj.MouseMove(e, 5.0);
				OscData[OscillatorNumber].FMDepth = DisplayValue;
			}
		}

		private void pbPMDepthSlider_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderPMDepthObj.MouseDown(e);
			OscData[OscillatorNumber].PMDepth = DisplayValue;
		}

		private void pbPMDepthSlider_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderPMDepthObj.MouseMove(e, 5.0);
				OscData[OscillatorNumber].PMDepth = DisplayValue;
			}
		}

		private void pbSDDepthSlider_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderSDDepthObj.MouseDown(e);
			OscData[OscillatorNumber].SDDepth = DisplayValue;
		}

		private void pbSDDepthSlider_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderSDDepthObj.MouseMove(e, 5.0);
				OscData[OscillatorNumber].SDDepth = DisplayValue;
			}
		}

		private void pbZoomSlider_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderZoomObj.MouseDown(e);
			OscilloscopeZoom = DisplayValue;
			OscilloscopeObj.SetView(MixerObj.GetRawSoundData(), OscilloscopePosition, OscilloscopeZoom);
		}

		private void pbZoomSlider_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderZoomObj.MouseMove(e, 5.0);
				OscilloscopeZoom = DisplayValue;
				OscilloscopeObj.SetView(MixerObj.GetRawSoundData(), OscilloscopePosition, OscilloscopeZoom);
			}
		}

		private void pbPositionSlider_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderPositionObj.MouseDown(e);
			OscilloscopePosition = DisplayValue;
			OscilloscopeObj.SetView(MixerObj.GetRawSoundData(), OscilloscopePosition, OscilloscopeZoom);
		}

		private void pbPositionSlider_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderPositionObj.MouseMove(e, 25.0);
				OscilloscopePosition = DisplayValue;
				OscilloscopeObj.SetView(MixerObj.GetRawSoundData(), OscilloscopePosition, OscilloscopeZoom);
			}
		}

		private void cbAGC_CheckedChanged( object sender, EventArgs e ) {
			MixData.AGC = cbAGC.Checked;
		}

		private void udDelay1_ValueChanged( object sender, EventArgs e ) {
			MixData.Delay1 = (int)udDelay1.Value;
		}

		private void udGain1_ValueChanged( object sender, EventArgs e ) {
			MixData.Gain1 = (int)udGain1.Value;
		}

		private void udDelay2_ValueChanged( object sender, EventArgs e ) {
			MixData.Delay2 = (int)udDelay2.Value;
		}

		private void udGain2_ValueChanged( object sender, EventArgs e ) {
			MixData.Gain2 = (int)udGain2.Value;
		}

		private void udDelay3_ValueChanged( object sender, EventArgs e ) {
			MixData.Delay3 = (int)udDelay3.Value;
		}

		private void udGain3_ValueChanged( object sender, EventArgs e ) {
			MixData.Gain3 = (int)udGain3.Value;
		}

		private void udDelay4_ValueChanged( object sender, EventArgs e ) {
			MixData.Delay4 = (int)udDelay4.Value;
		}

		private void udGain4_ValueChanged( object sender, EventArgs e ) {
			MixData.Gain4 = (int)udGain4.Value;
		}

		private void udDelay5_ValueChanged( object sender, EventArgs e ) {
			MixData.Delay5 = (int)udDelay5.Value;
		}

		private void udGain5_ValueChanged( object sender, EventArgs e ) {
			MixData.Gain5 = (int)udGain5.Value;
		}

		private void udDelay6_ValueChanged( object sender, EventArgs e ) {
			MixData.Delay6 = (int)udDelay6.Value;
		}

		private void udGain6_ValueChanged( object sender, EventArgs e ) {
			MixData.Gain6 = (int)udGain6.Value;
		}

		private void udDelay7_ValueChanged( object sender, EventArgs e ) {
			MixData.Delay7 = (int)udDelay7.Value;
		}

		private void udGain7_ValueChanged( object sender, EventArgs e ) {
			MixData.Gain7 = (int)udGain7.Value;
		}

		private void cbReverb_CheckedChanged( object sender, EventArgs e ) {
			MixData.Reverb = cbReverb.Checked;
		}

		private void btnDrawFFT_Click( object sender, EventArgs e ) {

			OscilloscopeObj.DrawFFT(MixerObj.GetRawSoundData());
		}

		private void cbRemoveDC_CheckedChanged( object sender, EventArgs e ) {
			MixData.RemoveDC = cbRemoveDC.Checked;
		}

		private void cbMovingAverageFilter_CheckedChanged( object sender, EventArgs e ) {
			MixData.MovingAverageFilter = cbMovingAverageFilter.Checked;
		}

		private void saveSettingsToolStripMenuItem_Click( object sender, EventArgs e ) {
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();

			saveFileDialog1.Filter = "gsb files (*.gsb)|*.gsb";
			saveFileDialog1.Title = "Save settings.";
			saveFileDialog1.FilterIndex = 2;
			saveFileDialog1.RestoreDirectory = true;

			if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
				LSObj.SaveSettings(saveFileDialog1.FileName, OscData, MixData);
			}
		}

		private void loadSeettingsToolStripMenuItem_Click( object sender, EventArgs e ) {
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
			openFileDialog1.Filter = "gsb files (*.gsb)|*.gsb";
			openFileDialog1.Title = "Load settings.";

			if (openFileDialog1.ShowDialog() == DialogResult.OK) {
				ODMD OdMd = LSObj.LoadSettings(openFileDialog1.FileName);
				OscData = OdMd.OD;
				MixData = OdMd.MD;
			}
			PresentData(OscData, OscillatorNumber);
			CalcAndPlay(false);
		}

		private void exitToolStripMenuItem_Click( object sender, EventArgs e ) {
			Application.Exit();
		}

		private void saveWavToolStripMenuItem_Click( object sender, EventArgs e ) {
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();

			saveFileDialog1.Filter = "wav files (*.wav)|*.wav";
			saveFileDialog1.Title = "Save .wav.";
			saveFileDialog1.FilterIndex = 2;
			saveFileDialog1.RestoreDirectory = true;

			if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
				LSObj.SaveWave(saveFileDialog1.FileName, MixerObj.GetSoundDotWave());
			}
		}

		private void btnRandom_Click_1( object sender, EventArgs e ) {
			OscData = OscillatorObj.ResetAll(OscData);
			OscData = OscillatorObj.CreateRandomSoundCrazy(OscData, RndSettings);
			MixData = MixerObj.CreateRandomGlobal(MixData, RndSettings);
			PresentData(OscData, OscillatorNumber);
			CalcAndPlay(false);
		}

		private void btnRandomFilter_Click( object sender, EventArgs e ) {
			if (!RndSettings.FilterLock) {
				MixData = MixerObj.CreateRandomFilter(MixData);
				PresentData(OscData, OscillatorNumber);
				CalcAndPlay(false);
			}
		}

		private void btnRandomReverb_Click( object sender, EventArgs e ) {
			if (!RndSettings.ReverbLock) {
				MixData = MixerObj.CreateRandomReverb(MixData);
				PresentData(OscData, OscillatorNumber);
				CalcAndPlay(false);
			}
		}

		private void cbFilterLock_CheckedChanged( object sender, EventArgs e ) {
			RndSettings.FilterLock = cbFilterLock.Checked;
		}

		private void cbReverbLock_CheckedChanged( object sender, EventArgs e ) {
			RndSettings.ReverbLock = cbReverbLock.Checked;
		}

		private void udLFO_ValueChanged( object sender, EventArgs e ) {
			RndSettings.NoRandomLFO = (int)udLFO.Value;
		}

		private void udNoRandomOsc_ValueChanged( object sender, EventArgs e ) {
			RndSettings.NoRandomOsc = (int)udNoRandomOsc.Value;
		}

		private void btnRandomThisOsc_Click( object sender, EventArgs e ) {
			OscData = OscillatorObj.CreateRandomSoundThis(OscData, OscillatorNumber);
			MixData = MixerObj.CreateRandomGlobal(MixData, RndSettings);
			PresentData(OscData, OscillatorNumber);
			CalcAndPlay(false);
		}

		private void btnShortRnd_Click( object sender, EventArgs e ) {
			OscData = OscillatorObj.ResetAll(OscData);
			OscData = OscillatorObj.CreateRandomSoundShort(OscData, RndSettings);
			MixData = MixerObj.CreateRandomGlobal(MixData, RndSettings);
			PresentData(OscData, OscillatorNumber);
			CalcAndPlay(false);
		}

		private void btnMediumRnd_Click( object sender, EventArgs e ) {
			OscData = OscillatorObj.ResetAll(OscData);
			OscData = OscillatorObj.CreateRandomSoundMedium(OscData, RndSettings);
			MixData = MixerObj.CreateRandomGlobal(MixData, RndSettings);
			PresentData(OscData, OscillatorNumber);
			CalcAndPlay(false);
		}

		private void btnSameADSR_Click( object sender, EventArgs e ) {
			OscData = OscillatorObj.ResetAll(OscData);
			OscData = OscillatorObj.CreateRandomSoundSameADSR(OscData, RndSettings);
			MixData = MixerObj.CreateRandomGlobal(MixData, RndSettings);
			PresentData(OscData, OscillatorNumber);
			CalcAndPlay(false);
		}

		private void btnNotes_Click( object sender, EventArgs e ) {
			OscData = OscillatorObj.ResetAll(OscData);
			OscData = OscillatorObj.CreateRandomSoundSin(OscData, RndSettings);
			MixData = MixerObj.CreateRandomGlobal(MixData, RndSettings);
			PresentData(OscData, OscillatorNumber);
			CalcAndPlay(false);
		}

		private void udMaxRandomFrequency_ValueChanged( object sender, EventArgs e ) {
			RndSettings.MaxRandomFrequency = (double)udMaxRandomFrequency.Value;
		}

		private void udMinRandomFrequency_ValueChanged( object sender, EventArgs e ) {
			RndSettings.MinRandomFrequency = (double)udMinRandomFrequency.Value;
		}

		private void btnUndo_Click( object sender, EventArgs e ) {
			Items OdMd = UndoObj.Undo();
			OscData = new OscillatorData[MaxOscillators];
			Array.Copy(OdMd.OD, OscData, OscData.Length);
			MixData = OdMd.MD;
			InsertNewData = false;
			CalcAndPlay(false);
			PresentData(OscData, OscillatorNumber);
		}

		private void btnRedo_Click( object sender, EventArgs e ) {
			Items OdMd = UndoObj.Redo();
			OscData = new OscillatorData[MaxOscillators];
			Array.Copy(OdMd.OD, OscData, OscData.Length);
			MixData = OdMd.MD;
			InsertNewData = false;
			CalcAndPlay(false);
			PresentData(OscData, OscillatorNumber);
		}

		private void pbFilterFrequencyDepth_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderFilterFrequencyDepthObj.MouseDown(e);
			MixData.FFDepth = DisplayValue;
		}

		private void pbFilterFrequencyDepth_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderFilterFrequencyDepthObj.MouseMove(e, 5.0);
				MixData.FFDepth = DisplayValue;
			}
		}

		private void udFFModulationFromOsc_ValueChanged( object sender, EventArgs e ) {
			MixData.FilterFrequencyFromOsc = (int)udFFModulationFromOsc.Value;
		}

		private void aboutToolStripMenuItem_Click( object sender, EventArgs e ) {
			MessageBox.Show(" Game Sound Builder V0.1 \n\n Copyright © Ulf Mandorff 2016 \n\n\n Freeware \n\n http://gsb.zymcox.com \n\n ");
		}

		private void udBitResulotion_ValueChanged( object sender, EventArgs e ) {
			MixData.BitResolution = 16 - (int)udBitResulotion.Value;
		}

		private void cbStereo_CheckedChanged( object sender, EventArgs e ) {
			MixData.Stereo = cbStereo.Checked;
		}

		private void udDelay_ValueChanged( object sender, EventArgs e ) {
			MixData.StereoDelay = (int)udDelay.Value;
			SliderStereoDelayObj.Draw(MixData.StereoDelay);
		}

		private void udOscBitResulotion_ValueChanged( object sender, EventArgs e ) {
			OscData[OscillatorNumber].OscBitResolution = 16 - (int)udOscBitResulotion.Value;
		}

		private void pbSteroDelay_MouseDown( object sender, MouseEventArgs e ) {
			double DisplayValue = SliderStereoDelayObj.MouseDown(e);
			MixData.StereoDelay = (int)DisplayValue;
			udDelay.Value = (int)DisplayValue;
		}

		private void pbSteroDelay_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() != "None") {
				double DisplayValue = SliderStereoDelayObj.MouseMove(e, 5.0);
				MixData.StereoDelay = (int)DisplayValue;
				udDelay.Value = (int)DisplayValue;
			}
		}

		private void btnRndJingel_Click( object sender, EventArgs e ) {
			// Ny slumpmässig jingle
			OscDataJingle = OscillatorObj.Jingle(OscData, strJingle, OkNotes, true);
			CalcAndPlay(true);
		}

		private bool CheckOkNotes( string s ) {
			bool Ok = false;
			string SpacesOnEnd = "";
			Note[] Notes = OscillatorObj.GetNotes();
			while (s.Length + SpacesOnEnd.Length < 3) SpacesOnEnd = SpacesOnEnd + " ";
			for (int i = 0; i < Notes.Length; i++) {
				if (Notes[i].Name1 == s + SpacesOnEnd || Notes[i].Name2 == s + SpacesOnEnd) {
					Ok = true;
					break;
				}
			}
			return Ok;
		}

		private void tbOkNote0_TextChanged( object sender, EventArgs e ) {
			string s = tbOkNote0.Text;
			if (!CheckOkNotes(s)) {
				tbOkNote0.ForeColor = Color.Red;
				OkNotes[0] = "C5 ";
			} else {
				tbOkNote0.ForeColor = Color.Black;
				if(s.Length < 3) s = s + " ";
				OkNotes[0] = s;
            }
		}

		private void tbOkNote1_TextChanged( object sender, EventArgs e ) {
			string s = tbOkNote1.Text;
			if (!CheckOkNotes(s)) {
				tbOkNote1.ForeColor = Color.Red;
				OkNotes[1] = "C5 ";
			} else {
				tbOkNote1.ForeColor = Color.Black;
				if (s.Length < 3) s = s + " ";
				OkNotes[1] = s;
			}
		}

		private void tbOkNote2_TextChanged( object sender, EventArgs e ) {
			string s = tbOkNote2.Text;
			if (!CheckOkNotes(s)) {
				tbOkNote2.ForeColor = Color.Red;
				OkNotes[2] = "C5 ";
			} else {
				tbOkNote2.ForeColor = Color.Black;
				if (s.Length < 3) s = s + " ";
				OkNotes[2] = s;
			}
		}

		private void tbOkNote3_TextChanged( object sender, EventArgs e ) {
			string s = tbOkNote3.Text;
			if (!CheckOkNotes(s)) {
				tbOkNote3.ForeColor = Color.Red;
				OkNotes[3] = "C5 ";
			} else {
				tbOkNote3.ForeColor = Color.Black;
				if (s.Length < 3) s = s + " ";
				OkNotes[3] = s;
			}
		}

		private void tbOkNote4_TextChanged( object sender, EventArgs e ) {
			string s = tbOkNote4.Text;
			if (!CheckOkNotes(s)) {
				tbOkNote4.ForeColor = Color.Red;
				OkNotes[4] = "C5 ";
			} else {
				tbOkNote4.ForeColor = Color.Black;
				if (s.Length < 3) s = s + " ";
				OkNotes[4] = s;
			}
		}

		private void tbOkNote5_TextChanged( object sender, EventArgs e ) {
			string s = tbOkNote5.Text;
			if (!CheckOkNotes(s)) {
				tbOkNote5.ForeColor = Color.Red;
				OkNotes[5] = "C5 ";
			} else {
				tbOkNote5.ForeColor = Color.Black;
				if (s.Length < 3) s = s + " ";
				OkNotes[5] = s;
			}
		}

		private void tbOkNote6_TextChanged( object sender, EventArgs e ) {
			string s = tbOkNote6.Text;
			if (!CheckOkNotes(s)) {
				tbOkNote6.ForeColor = Color.Red;
				OkNotes[6] = "C5 ";
			} else {
				tbOkNote6.ForeColor = Color.Black;
				if (s.Length < 3) s = s + " ";
				OkNotes[6] = s;
			}
		}

		private void tbOkNote7_TextChanged( object sender, EventArgs e ) {
			string s = tbOkNote7.Text;
			if (!CheckOkNotes(s)) {
				tbOkNote7.ForeColor = Color.Red;
				OkNotes[7] = "C5 ";
			} else {
				tbOkNote7.ForeColor = Color.Black;
				if (s.Length < 3) s = s + " ";
				OkNotes[7] = s;
			}
		}
	}
}