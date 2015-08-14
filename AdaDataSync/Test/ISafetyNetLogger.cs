using System;
using System.IO;

namespace AdaDataSync.Test
{
	public interface ISafetyNetLogger
	{
		void HataLogla(Exception exception);
	}

	class SafetyNetLogger : ISafetyNetLogger
	{
		public void HataLogla(Exception exception)
		{
			//string hataMesaji = fPrkTrLog + " anahtarlı trlog kayıtı aktarılamadı. trlog tablosunda hataacikla alanı doldurulurken de hata oluştu. Hatamesajı: " + ex.Message;

			const string path = @"hata.txt";
			using (StreamWriter sw = new StreamWriter(path, true))
			{
				sw.WriteLine(exception.ToString());
			}
		}
	}
}