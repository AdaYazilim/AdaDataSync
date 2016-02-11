using System;
using AdaDataSync.API;

namespace AdaDataSync
{
    class DataSyncYonetici : IDataSyncYonetici
    {
        private readonly IDataSyncService _dataSyncServis;
        private readonly ILogger _safetyLogger;

        public DataSyncYonetici(IDataSyncService dataSyncServis, ILogger safetyLogger)
        {
            _dataSyncServis = dataSyncServis;
            _safetyLogger = safetyLogger;
        }

        public void Sync()
        {
            try
            {
                _dataSyncServis.Sync();
            }
            catch (Exception ex)
            {
                _safetyLogger.Logla(ex.Message);
            }
        }
    }
}