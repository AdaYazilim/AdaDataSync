using System;
using AdaDataSync.API;

namespace AdaDataSync
{
    class DataSyncYonetici : IDataSyncYonetici
    {
        private readonly IGuncellemeKontrol _guncellemeKontrol;
        private readonly ITumDosyalariKullanmaMotoru _dosyalariKullanmaMotoru;
        private IDataSyncService _dataSyncServis;
        private readonly ILogger _safetyLogger;

        public event Func<IDataSyncService> KritikHataAtti;

        public DataSyncYonetici(IGuncellemeKontrol guncellemeKontrol, ITumDosyalariKullanmaMotoru dosyalariKullanmaMotoru, IDataSyncService dataSyncServis, ILogger safetyLogger)
        {
            _guncellemeKontrol = guncellemeKontrol;
            _dosyalariKullanmaMotoru = dosyalariKullanmaMotoru;
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
                    _dosyalariKullanmaMotoru.ButunDosyalariSerbestBirak();
                    return;
                }

                _dosyalariKullanmaMotoru.ButunDosyalariKullan();

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