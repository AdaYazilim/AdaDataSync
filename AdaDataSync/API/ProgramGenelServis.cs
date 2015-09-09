using System;
using System.Diagnostics;

namespace AdaDataSync.API
{
    internal class ProgramGenelServis
    {
        private readonly ICalisanServisKontrolcusu _calisanServisKontrolcusu;
        private readonly ISafetyNetLogger _safetyLogger;
        private readonly int _beklemeSuresi;
        private readonly IDataSyncService[] _syncService;

        public ProgramGenelServis(ICalisanServisKontrolcusu calisanServisKontrolcusu, ISafetyNetLogger safetyLogger, params IDataSyncService[] syncService)
            :this(calisanServisKontrolcusu, safetyLogger, 5000, syncService)
        {

        }

        public ProgramGenelServis(ICalisanServisKontrolcusu calisanServisKontrolcusu, ISafetyNetLogger safetyLogger, int beklemeSuresi, params IDataSyncService[] syncService)
        {
            _calisanServisKontrolcusu = calisanServisKontrolcusu;
            _safetyLogger = safetyLogger;
            _beklemeSuresi = beklemeSuresi;
            _syncService = syncService;
        }

        public void Calistir(int calistirmaSayisi = 0)
        {
            if (_calisanServisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu())
                return;

            _calisanServisKontrolcusu.MakinaBazindaKilitKoy();

            int calistirmaNo = 0;
            while (true)
            {
                Stopwatch sw = Stopwatch.StartNew();

                foreach (IDataSyncService dataSyncService in _syncService)
                {
                    try
                    {
                        dataSyncService.Sync();
                    }
                    catch (Exception ex)
                    {
                        _safetyLogger.HataLogla(ex);
                    }
                }

                calistirmaNo++;

                sw.Stop();
                if (calistirmaNo >= calistirmaSayisi && calistirmaSayisi > 0)
                {
                    break;
                }
                else
                {
                    if(sw.ElapsedMilliseconds<_beklemeSuresi)
                        System.Threading.Thread.Sleep(_beklemeSuresi - (int)sw.ElapsedMilliseconds);
                }
            }
        }
    }
}