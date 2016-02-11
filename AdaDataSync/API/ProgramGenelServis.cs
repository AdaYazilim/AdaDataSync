using System;
using System.Diagnostics;

namespace AdaDataSync.API
{
    internal class ProgramGenelServis
    {
        private readonly ICalisanServisKontrolcusu _calisanServisKontrolcusu;
        private readonly ILogger _safetyLogger;
        private readonly int _beklemeSuresi;
        private readonly IDataSyncService[] _syncServices;

        public ProgramGenelServis(ICalisanServisKontrolcusu calisanServisKontrolcusu, ILogger safetyLogger, params IDataSyncService[] syncServices)
            :this(calisanServisKontrolcusu, safetyLogger, 5000, syncServices)
        {

        }

        public ProgramGenelServis(ICalisanServisKontrolcusu calisanServisKontrolcusu, ILogger safetyLogger, int beklemeSuresi, params IDataSyncService[] syncServices)
        {
            _calisanServisKontrolcusu = calisanServisKontrolcusu;
            _safetyLogger = safetyLogger;
            _beklemeSuresi = beklemeSuresi;
            _syncServices = syncServices;
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
            foreach (IDataSyncService dataSyncService in _syncServices)
            {
                tekServisCalistir(dataSyncService);
            }
        }

        private void tekServisCalistir(IDataSyncService dataSyncService)
        {
            try
            {
                dataSyncService.Sync();
            }
            catch (Exception ex)
            {
                _safetyLogger.Logla(ex.Message);
            }
        }
    }
}