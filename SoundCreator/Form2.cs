using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundCreator {
	public partial class Form2 : Form {

		public int BufferIndex;
		//public short[] SoundArray;
		public short[] SelectedSoundArray;
		public bool Ok = false;

		private int LMBStartPos;
		private int LMBEndPos;
		private bool isMouseUp = true;

		SoundRecord SoundRecordObj;
		public Form2(int BI) {
			InitializeComponent();
			SoundRecordObj = new SoundRecord(StartRec, pbRecLevel, pbRecWave);
			SoundRecordObj.PrepairRecording();
			LMBStartPos = 0;
			LMBEndPos = pbRecWave.Size.Width;
			BufferIndex = BI;
		}

		private void ExitRec_Click( object sender, EventArgs e ) {
			SoundRecordObj.CreateByteArray(LMBStartPos, LMBEndPos, pbRecWave.Size.Width);
			SelectedSoundArray = SoundRecordObj.GetSelectedSoundArray();
			Ok = true;
			SoundRecordObj.StopAllRec();
			Close();
		}

		private void StopRec_Click( object sender, EventArgs e ) {
			StartRec.Enabled = true;
			SoundRecordObj.StopRecord();
		}

		private void StartRec_Click( object sender, EventArgs e ) {
			StartRec.Enabled = false;
			LMBStartPos = 0;
			LMBEndPos = pbRecWave.Size.Width;
			SoundRecordObj.StartRecord(BufferIndex);
		}

		private void pbRecWave_MouseDown( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() == "Left" && isMouseUp) {
				isMouseUp = false;
				LMBStartPos = e.X;
				LMBEndPos = e.X;
			}
		}

		private void pbRecWave_MouseMove( object sender, MouseEventArgs e ) {
			if (e.Button.ToString() == "Left" && !isMouseUp) {
				
				LMBEndPos = e.X;
				SoundRecordObj.DrawSelection(LMBStartPos, LMBEndPos);
			}
		}

		private void pbRecWave_MouseUp( object sender, MouseEventArgs e ) {
			isMouseUp = true;
			SoundRecordObj.CreateByteArray(LMBStartPos, LMBEndPos, pbRecWave.Size.Width);
		}

		private void PlayRec_Click( object sender, EventArgs e ) {
			SoundRecordObj.PlaySound(LMBStartPos, LMBEndPos, pbRecWave.Size.Width);
		}

		private void CancelRec_Click( object sender, EventArgs e ) {
			SoundRecordObj.StopAllRec();
			Ok = false;
			Close();
		}
	}
}
