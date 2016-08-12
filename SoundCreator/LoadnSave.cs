using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SoundCreator {

	public struct ODMD {
		public OscillatorData[] OD;
		public MixerData MD;
	}

	class LoadnSave {

		public void SaveWave( string FileName, byte[] WavFile ) {
			File.WriteAllBytes(FileName, WavFile);
		}

		public Boolean LoadWave( string FileName, short[] RealSoundBuffer ) {
			int pos = 0;

			byte[] WaveFile = File.ReadAllBytes(FileName);
			// Endast 16 bits monoljud
			if(WaveFile[0] != 'R' || WaveFile[1] != 'I' || WaveFile[2] != 'F' || WaveFile[3] != 'F' || WaveFile[22] != 01 || WaveFile[34] != 16) {
				return false;
			}
			// Hitta data. Lite overkill....
			while (!(WaveFile[pos] == 'd' && WaveFile[pos + 1] == 'a' && WaveFile[pos + 2] == 't' && WaveFile[pos + 3] == 'a')) {
				pos++;
			}
			pos = pos + 8;

			for (Int32 i = 0; i < (WaveFile.Length - pos) / 2; i++) {
				if (i >= RealSoundBuffer.Length) break;
				RealSoundBuffer[i] = (short)(WaveFile[pos + i * 2 + 1] << 8 | WaveFile[pos + i * 2 + 0]);
			}

			return true;
		}


		public ODMD ButtonsLoad( string FileName ) {
			FileName = "ButtonSettings\\" + FileName;
			return LoadSettings(FileName);
		}


		public void SaveSettings( string FileName, OscillatorData[] OD, MixerData MD ) {
			ODMD OdMd = new ODMD();
			OdMd.OD = OD;
			OdMd.MD = MD;
			XmlSerializer writer = new XmlSerializer(OdMd.GetType());
			StreamWriter file = new StreamWriter(FileName);
			writer.Serialize(file, OdMd);
			file.Close();
		}

		public ODMD LoadSettings( string FileName ) {
			ODMD OdMd = new ODMD();
			XmlSerializer serializer = new XmlSerializer(typeof(ODMD));

			StreamReader reader = new StreamReader(FileName);

			OdMd = (ODMD)serializer.Deserialize(reader);
			reader.Close();
			return OdMd;
		}
	}
}
