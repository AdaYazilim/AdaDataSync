using System;

namespace AdaDataSync.API
{
    internal class ProgramGenelServis
    {
        private readonly ICalisanServisKontrolcusu _calisanServisKontrolcusu;
        private readonly ISafetyNetLogger _safetyLogger;
        private readonly IDataSyncService[] _syncService;

        public ProgramGenelServis(ICalisanServisKontrolcusu calisanServisKontrolcusu, ISafetyNetLogger safetyLogger, params IDataSyncService[] syncService)
        {
            _calisanServisKontrolcusu = calisanServisKontrolcusu;
            _safetyLogger = safetyLogger;
            _syncService = syncService;
        }

        public void Calistir()
        {
            if (_calisanServisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu())
                return;

            _calisanServisKontrolcusu.MakinaBazindaKilitKoy();

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
        }
    }
}