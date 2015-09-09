using System;
using System.IO;

namespace AdaDataSync.API
{
    class SafetyNetLogger : ISafetyNetLogger
    {
        public void HataLogla(Exception exception)
        {
            const string path = @"hata.txt";
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(DateTime.Now.ToString() + " / " + exception.ToString());
            }
        }
    }
}
