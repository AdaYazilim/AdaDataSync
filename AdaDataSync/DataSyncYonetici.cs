using System;
using AdaDataSync.API;

namespace AdaDataSync
{
    class DataSyncYonetici : IDataSyncYonetici
    {
        private readonly IGuncellemeKontrol _guncellemeKontrol;
        private IDataSyncService _dataSyncServis;
        private readonly ILogger _safetyLogger;

        public event Func<IDataSyncService> KritikHataAtti;

        public DataSyncYonetici(IGuncellemeKontrol guncellemeKontrol, IDataSyncService dataSyncServis, ILogger safetyLogger)
        {
            _guncellemeKontrol = guncellemeKontrol;
            _dataSyncServis = dataSyncServis;
            _safetyLogger = safetyLogger;
        }

        public void Sync()
        {
            try
            {
                if (_guncellemeKontrol.SuAndaGuncellemeYapiliyor())
                {
                    Console.WriteLine("Lütfen bekleyin. Veritabanı düzenlemesi yapılıyor.");
                    return;
                }

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