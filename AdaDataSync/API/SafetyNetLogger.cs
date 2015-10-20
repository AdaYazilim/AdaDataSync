using System;
using System.IO;

namespace AdaDataSync.API
{
    public class TextDosyasiLogger : ILogger
    {
        private readonly string _dosyaAdresi;
        private string _sonLogMesaji = string.Empty;

        public TextDosyasiLogger(string dosyaAdresi)
        {
            _dosyaAdresi = dosyaAdresi;
        }

        public void Logla(string logMesaji)
        {
            using (StreamWriter sw = new StreamWriter(_dosyaAdresi, true))
            {
                if (logMesaji == _sonLogMesaji) // belli bir hatayı her seferinde atmaya başladığında hata.txt dosyası büyüyor. 
                {
                    sw.WriteLine(DateTime.Now.ToString() + " / Aynısı");
                }
                else
                {
                    sw.WriteLine(DateTime.Now.ToString() + " / " + logMesaji);
                    _sonLogMesaji = logMesaji;
                }
            }
        }
    }
}
