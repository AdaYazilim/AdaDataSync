using AdaDataSync.API;

namespace AdaDataSync
{
    class DataSyncYonetici : IDataSyncYonetici
    {
        private readonly IDataSyncService _dataSyncServis;
        private readonly ILogger _safetyLogger;

        public IDataSyncService DataSyncServis
        {
            get { return _dataSyncServis; }
        }

        public ILogger SafetyLogger
        {
            get { return _safetyLogger; }
        }

        public DataSyncYonetici(IDataSyncService dataSyncServis, ILogger safetyLogger)
        {
            _dataSyncServis = dataSyncServis;
            _safetyLogger = safetyLogger;
        }
    }
}