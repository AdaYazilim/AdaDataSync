using System;
using System.Diagnostics;

namespace AdaDataSync.API
{
    internal class ProgramGenelServis
    {
        private readonly ICalisanServisKontrolcusu _calisanServisKontrolcusu;
        //private readonly ILogger _safetyLogger;
        private readonly int _beklemeSuresi;
        private readonly IDataSyncYonetici[] _syncYoneticiler;

        //public ProgramGenelServis(ICalisanServisKontrolcusu calisanServisKontrolcusu, ILogger safetyLogger, params IDataSyncService[] syncServices)
        //    :this(calisanServisKontrolcusu, safetyLogger, 5000, syncServices)
        public ProgramGenelServis(ICalisanServisKontrolcusu calisanServisKontrolcusu, params IDataSyncYonetici[] syncYoneticiler)
            : this(calisanServisKontrolcusu, 5000, syncYoneticiler)
        {

        }

        //public ProgramGenelServis(ICalisanServisKontrolcusu calisanServisKontrolcusu, ILogger safetyLogger, int beklemeSuresi, params IDataSyncService[] syncServices)
        public ProgramGenelServis(ICalisanServisKontrolcusu calisanServisKontrolcusu, int beklemeSuresi, params IDataSyncYonetici[] syncYoneticiler)
        {
            _calisanServisKontrolcusu = calisanServisKontrolcusu;
            //_safetyLogger = safetyLogger;
            _beklemeSuresi = beklemeSuresi;
            _syncYoneticiler = syncYoneticiler;
        }

        public void Calistir(int calistirmaSayisi = 0)
        {
            if (_calisanServisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu())
                return;

            //_calisanServisKontrolcusu.MakinaBazindaKilitKoy();

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
            try
            {
                dataSyncYonetici.DataSyncServis.Sync();
            }
            catch (Exception ex)
            {
                //_safetyLogger.Logla(ex.Message);
                //dataSyncService.SafetyLogla(ex.Message);
                dataSyncYonetici.SafetyLogger.Logla(ex.Message);
            }
        }
    }
}