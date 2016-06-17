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

		public void LoadWave() {
			// Nja?
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
