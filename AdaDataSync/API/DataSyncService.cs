using System;

namespace AdaDataSync.API
{
    public class DataSyncService : IDataSyncService
    {
        private readonly IGuncellemeKontrol _guncellemeKontrol;
        //private readonly IVeritabaniIslemYapan[] _veritabaniIslemYapanlar;

        private readonly IVeritabaniIslemYapan _dataDefinitionGuncelleyen;
        private readonly IVeritabaniIslemYapan _veriAktaran;

        public DataSyncService(IGuncellemeKontrol guncellemeKontrol, IVeritabaniIslemYapan dataDefinitionGuncelleyen, IVeritabaniIslemYapan veriAktaran)
        {
            _guncellemeKontrol = guncellemeKontrol;
            //_veritabaniIslemYapanlar = veritabaniIslemYapanlar;

            _dataDefinitionGuncelleyen = dataDefinitionGuncelleyen;
            _veriAktaran = veriAktaran;
        }

        public void Sync()
        {
            //foreach (IVeritabaniIslemYapan islemYapan in _veritabaniIslemYapanlar)
            //{
            //    if (_guncellemeKontrol.SuAndaGuncellemeYapiliyor())
            //    {
            //        Console.WriteLine("Lütfen bekleyin. Veritabanı düzenlemesi yapılıyor.");
            //        return;
            //    }

            //    islemYapan.VeritabaniIslemiYap();
            //}


            if (_guncellemeKontrol.SuAndaGuncellemeYapiliyor())
            {
                Console.WriteLine("Lütfen bekleyin. Veritabanı düzenlemesi yapılıyor.");
                return;
            }

            _dataDefinitionGuncelleyen.VeritabaniIslemiYap();


            if (_guncellemeKontrol.SuAndaGuncellemeYapiliyor())
            {
                Console.WriteLine("Lütfen bekleyin. Veritabanı düzenlemesi yapılıyor.");
                return;
            }

            _veriAktaran.VeritabaniIslemiYap();
        }
    }
}