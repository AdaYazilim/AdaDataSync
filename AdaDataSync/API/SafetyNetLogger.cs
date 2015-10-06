using System;
using System.IO;

namespace AdaDataSync.API
{
    class SafetyNetLogger : ISafetyNetLogger
    {
        private string _sonHataMesaji = string.Empty;

        public void HataLogla(Exception exception)
        {

            const string path = @"hata.txt";
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                string hataMesaji = exception.ToString();

                if (hataMesaji == _sonHataMesaji) // belli bir hatayı her seferinde atmaya başladığında hata.txt dosyası büyüyor. 
                {
                    sw.WriteLine(DateTime.Now.ToString() + " / Aynısı");
                }
                else
                {
                    sw.WriteLine(DateTime.Now.ToString() + " / " + hataMesaji);
                    _sonHataMesaji = hataMesaji;
                }
            }
        }
    }
}
