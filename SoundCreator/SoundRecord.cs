using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
// finns inte på github why

namespace SoundCreator {
	class SoundRecord {
		private byte[] ByteArray = new byte[Form1.Samplerate * Form1.TimeMS / 500];
		private short[] SoundArray = new short[Form1.Samplerate * Form1.TimeMS / 1000];
		private short[] SelectedSoundArray = null;
		private byte[]  SelectedSoundByteArray = null;

		private int Count = 0;
		private int OldCount = 0;
		private bool Stopped = true;
		private int oldx,oldy;
		private int SlowerLevel = 0;
		public WaveIn WaveSrc = null;

		private Button StartButton;
		private PictureBox Level;
		private PictureBox WaveForm;
		private Bitmap Selectedbm;
		private Bitmap WaveFormbm;
		private Bitmap Levelbm;

		public SoundRecord( Button SB, PictureBox pbLevel, PictureBox pbWave ) {
			StartButton = SB;
			Level = pbLevel;
			WaveForm = pbWave;
			WaveFormbm = new Bitmap(WaveForm.Size.Width, WaveForm.Size.Height);
			Selectedbm = new Bitmap(WaveForm.Size.Width, WaveForm.Size.Height);
		}

		public void PrepairRecording() {
			if (WaveSrc == null) {

				WaveSrc = new WaveIn();
				WaveSrc.WaveFormat = new WaveFormat(Form1.Samplerate, 16, 1);
				WaveSrc.DeviceNumber = 0;
				WaveSrc.NumberOfBuffers = 3;
				WaveSrc.BufferMilliseconds = 10; //Form1.TimeMS;

				WaveSrc.DataAvailable += new EventHandler<WaveInEventArgs>(inputDevice_DataAvailable);
				WaveSrc.RecordingStopped += new EventHandler<StoppedEventArgs>(inputDevice_RecordingStopped);
				WaveSrc.StartRecording();
			}
		}

		public void StartRecord( int BufferIndex ) {
			Stopped = false;
			PrepairRecording();
			Count = 0;
			OldCount = 0;
			Array.Clear(ByteArray, 0, ByteArray.Length);
			Array.Clear(SoundArray, 0, SoundArray.Length);
		}

		public void StopRecord() {
			if (WaveSrc != null) {
				Stopped = true;
				WaveSrc.StopRecording();
				WaveSrc.Dispose();
				WaveSrc = null;
				PrepairRecording();
			}
		}

		public void StopAllRec() {
			if (WaveSrc != null) {
				Stopped = true;
				WaveSrc.StopRecording();
				WaveSrc.Dispose();
				WaveSrc = null;
			}
		}

		public short[] GetSoundArray() {
			return SoundArray;
		}

		public short[] GetSelectedSoundArray() {
			return SelectedSoundArray;
		}

		// *************************************************************************************************************************

		private void inputDevice_DataAvailable( object sender, WaveInEventArgs e ) {
			if (WaveSrc != null && Stopped == false) {


				int L = 0;
				int ML = 0;
				for (int i = 0; i < e.BytesRecorded; i += 2) {
					L = Math.Abs((short)(e.Buffer[i] | e.Buffer[i + 1] << 8));
					if (L > ML) ML = L;
				}
				ShowLevel((short)(ML));


				for (int i = 0; i < e.BytesRecorded; i++) {
					if (Count >= Form1.Samplerate * Form1.TimeMS / 500) break;
					ByteArray[Count] = e.Buffer[i];
					Count++;
				}
				DrawRecording(ByteArray, OldCount, Count);
				OldCount = Count;

				if (Stopped != true && Count >= Form1.Samplerate * Form1.TimeMS / 500) {
					Stopped = true;
					StartButton.Enabled = true;
					WaveSrc.StopRecording();
					WaveSrc.Dispose();
					WaveSrc = null;
					PrepairRecording();
				}
			}

			// Visa input level även om inspelningen är stoppad.
			if (WaveSrc != null && Stopped == true) {
				int L = 0;
				int ML = 0;
				for (int i = 0; i < e.BytesRecorded; i += 2) {
					L = Math.Abs((short)(e.Buffer[i] | e.Buffer[i + 1] << 8));
					if (L > ML) ML = L;
				}
				ShowLevel((short)(ML));
			}

		}

		private void inputDevice_RecordingStopped( object sender, StoppedEventArgs e ) {
			for (int i = 0; i < Count; i++, i++) {
				SoundArray[i / 2] = (short)(ByteArray[i] | ByteArray[i + 1] << 8);
			}
		}



		// ****************    Testa bitmap till Image *********************************************************************************************************

		private void ShowLevel( short L ) {
			if (SlowerLevel < L) {
				SlowerLevel = L;
			} else {
				SlowerLevel = SlowerLevel - 350;
				if (SlowerLevel < 0) SlowerLevel = 0;
			}
			int x = Level.Size.Width;
			int y = Level.Size.Height;
			if (oldy != y) {
				Levelbm = new Bitmap(x, y);
				oldy = y;
			} else {
				Levelbm = (Bitmap)Level.Image;
			}
			Graphics g = Graphics.FromImage(Levelbm);
			Brush b = new SolidBrush(Color.FromArgb(128,0x80,0xff,0x80));

			float MaxLevel = y / (float)Form1.MaxAmplitude;
			float SignalLevel = SlowerLevel * MaxLevel;

			g.Clear(Color.Black);
			g.FillRectangle(b, 0, y - SignalLevel, x, SignalLevel);
			Level.Image = Levelbm;
		}

		private void DrawRecording( byte[] WF, int StartWave, int StopWave ) {
			int x = WaveForm.Size.Width;
			int y = WaveForm.Size.Height;
			float yc = y/2;
			short y1;
			float MaxY1 = -32800;
			float MinY1 = 32800;
			int Oldi = 0;
			//WaveFormbm = null;

			if (StartWave == 0) {
				WaveFormbm = new Bitmap(x, y);
			}

			Graphics g = Graphics.FromImage(WaveFormbm);
			Pen PenCenter = new Pen(Color.Silver);
			Pen PenWave = new Pen(Color.FromArgb(128,0x80,0xff,0x80));

			if (StartWave == 0) {
				g.Clear(Color.Transparent);
				g.DrawLine(PenCenter, 0, yc, x, yc);
			}
			float ScaleX = x / (float)(Form1.Samplerate*Form1.TimeMS/500);
			float ScaleY = yc / (float)Form1.MaxAmplitude;

			for (int i = StartWave; i < StopWave; i += 2) {
				y1 = (short)(WF[i] | WF[i + 1] << 8);
				if (y1 > MaxY1) MaxY1 = y1;
				if (y1 < MinY1) MinY1 = y1;
				if ((int)(i * ScaleX) != (int)(Oldi * ScaleX)) {
					g.DrawLine(PenWave, i * ScaleX, MaxY1 * ScaleY + yc, i * ScaleX, MinY1 * ScaleY + yc);
					MaxY1 = -32800;
					MinY1 = 32800;
				}
				Oldi = i;
			}
			WaveForm.Image = WaveFormbm;
		}

		public void DrawSelection( int LMBStartPos, int LMBEndPos ) {
			int x = WaveForm.Size.Width;
			int y = WaveForm.Size.Height;
			if (x != oldx || y != oldy) {
				Selectedbm = new Bitmap(x, y);
				oldx = x;
				oldy = y;
			}
			Graphics g = Graphics.FromImage(Selectedbm);
			Brush b = new SolidBrush(Color.FromArgb(128,Color.Red));
			if (LMBStartPos > LMBEndPos) {
				int t = LMBStartPos;
				LMBStartPos = LMBEndPos;
				LMBEndPos = t;
			}
			g.Clear(Color.Transparent);
			g.FillRectangle(b, LMBStartPos, 0, LMBEndPos - LMBStartPos, y);
			g.DrawImage(WaveFormbm, 0, 0);
			WaveForm.Image = Selectedbm;
		}

		public void PlaySound( int StartX, int EndX, int MaxX ) {
			CreateByteArray(StartX, EndX, MaxX);

			WaveOut _waveOut = new WaveOut();
			IWaveProvider provider = new RawSourceWaveStream(new MemoryStream(SelectedSoundByteArray), new WaveFormat(44100, 16, 1));
			_waveOut.Init(provider);
			_waveOut.Play();
		}


		public void CreateByteArray( int StartX, int EndX, int MaxX ) {
			if (StartX > EndX) {
				int t = StartX;
				StartX = EndX;
				EndX = t;
			}
			float ScaleX = SoundArray.Length / MaxX;
			float WaveStart = StartX * ScaleX;
			float WaveEnd = EndX * ScaleX;

			SelectedSoundArray = new short[SoundArray.Length];
			SelectedSoundByteArray = new byte[(int)(WaveEnd - WaveStart) * 2];
			for (int i = 0; i < SelectedSoundArray.Length; i++) {
				if ((int)WaveStart + i < SoundArray.Length) {
					SelectedSoundArray[i] = SoundArray[(int)WaveStart + i];
				} else {
					SelectedSoundArray[i] = 0;
				}
			}
			for (int i = 0; i < SelectedSoundByteArray.Length / 2; i++) {
				SelectedSoundByteArray[i * 2 + 0] = (byte)(SelectedSoundArray[i] & 255);
				SelectedSoundByteArray[i * 2 + 1] = (byte)(SelectedSoundArray[i] >> 8 & 255);
			}
		}
	}
}


