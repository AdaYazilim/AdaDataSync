using AdaDataSync.API;

namespace AdaDataSync
{
    public interface IDataSyncYonetici
    {
        IDataSyncService DataSyncServis{get;}
        ILogger SafetyLogger { get; }
    }
}