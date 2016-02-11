using System;
using AdaDataSync.API;

namespace AdaDataSync
{
    class DataSyncYonetici : IDataSyncYonetici
    {
        private IDataSyncService _dataSyncServis;
        private readonly ILogger _safetyLogger;

        public event Func<IDataSyncService> KritikHataAtti;

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
                if (KritikHataAtti != null)
                    _dataSyncServis = KritikHataAtti();

                _safetyLogger.Logla(ex.Message);
            }
        }
    }
}