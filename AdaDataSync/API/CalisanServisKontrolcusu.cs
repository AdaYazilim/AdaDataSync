using System;
using System.Threading;

namespace AdaDataSync.API
{
    public class CalisanServisKontrolcusu : ICalisanServisKontrolcusu
    {
        // bu guidi değiştirmeyin. başka isimlerde mutexler olursa aynı anda 1'den fazla datasync çalışabilir.
        private static readonly Mutex Mutex = new Mutex(true, "{e51f3222-25ca-4213-af92-0eb0d7566460}");

        public bool BuMakinadaBaskaServisCalisiyorMu()
        {
            return !Mutex.WaitOne(TimeSpan.Zero, true);
        }
    }
}
