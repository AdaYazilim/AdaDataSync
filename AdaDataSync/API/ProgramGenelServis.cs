using System.Diagnostics;

namespace AdaDataSync.API
{
    internal class ProgramGenelServis
    {
        private readonly ICalisanServisKontrolcusu _calisanServisKontrolcusu;
        private readonly int _beklemeSuresi;
        private readonly IDataSyncYonetici[] _syncYoneticiler;

        public ProgramGenelServis(ICalisanServisKontrolcusu calisanServisKontrolcusu, params IDataSyncYonetici[] syncYoneticiler)
            : this(calisanServisKontrolcusu, 5000, syncYoneticiler)
        {

        }

        public ProgramGenelServis(ICalisanServisKontrolcusu calisanServisKontrolcusu, int beklemeSuresi, params IDataSyncYonetici[] syncYoneticiler)
        {
            _calisanServisKontrolcusu = calisanServisKontrolcusu;
            _beklemeSuresi = beklemeSuresi;
            _syncYoneticiler = syncYoneticiler;
        }

        public void Calistir(int calistirmaSayisi = 0)
        {
            if (_calisanServisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu())
                return;

            int calistirmaNo = 0;
            while (true)
            {
                Stopwatch sw = Stopwatch.StartNew();
                butunServisleriCalistir();
                sw.Stop();
                
                calistirmaNo++;

                if (calistirmaNo >= calistirmaSayisi && calistirmaSayisi > 0)
                    break;
                
                gerekliBeklemeyiYap(sw);
            }
        }

        private void gerekliBeklemeyiYap(Stopwatch sw)
        {
            if (sw.ElapsedMilliseconds < _beklemeSuresi)
                System.Threading.Thread.Sleep(_beklemeSuresi - (int) sw.ElapsedMilliseconds);
        }

        private void butunServisleriCalistir()
        {
            foreach (IDataSyncYonetici dataSyncYonetici in _syncYoneticiler)
            {
                tekServisCalistir(dataSyncYonetici);
            }
        }

        private void tekServisCalistir(IDataSyncYonetici dataSyncYonetici)
        {
            dataSyncYonetici.Sync();
        }
    }
}