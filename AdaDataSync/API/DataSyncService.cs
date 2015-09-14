using System;

namespace AdaDataSync.API
{
    public class DataSyncService : IDataSyncService
    {
        private readonly IGuncellemeKontrol _guncellemeKontrol;
        private readonly IVeritabaniIslemYapan[] _veritabaniIslemYapanlar;

        public DataSyncService(IGuncellemeKontrol guncellemeKontrol, params IVeritabaniIslemYapan[] veritabaniIslemYapanlar)
        {
            _guncellemeKontrol = guncellemeKontrol;
            _veritabaniIslemYapanlar = veritabaniIslemYapanlar;
        }

        public void Sync()
        {
            foreach (IVeritabaniIslemYapan islemYapan in _veritabaniIslemYapanlar)
            {
                if (_guncellemeKontrol.SuAndaGuncellemeYapiliyor())
                {
                    Console.WriteLine("Lütfen bekleyin. Veritabanı düzenlemesi yapılıyor.");
                    return;
                }

                islemYapan.VeritabaniIslemiYap();
            }
        }
    }
}