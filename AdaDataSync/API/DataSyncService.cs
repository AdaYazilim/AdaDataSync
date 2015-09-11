using System;
using System.Collections.Generic;

namespace AdaDataSync.API
{
    public class DataSyncService : IDataSyncService
    {
        private readonly int _syncEdilecekMaxKayitSayisi;
        private readonly IDatabaseProxy _dbProxy;

        public DataSyncService(IDatabaseProxy dbProxy, int syncEdilecekMaxKayitSayisi = 10000)
        {
            _dbProxy = dbProxy;
            _syncEdilecekMaxKayitSayisi = syncEdilecekMaxKayitSayisi;
        }

        public void Sync()
        {
            if (_dbProxy.FoxproTarafindaGuncellemeYapiliyor())
            {
                Console.WriteLine("Lütfen bekleyin. Veritabanı düzenlemesi yapılıyor.");
                return;
            }

            verilariAktar();
        }

        private void verilariAktar()
        {
            _dbProxy.BaglantilariAc();

            List<DataTransactionInfo> trInfolar = _dbProxy.BekleyenTransactionlariAl(_syncEdilecekMaxKayitSayisi);
            Console.WriteLine("Aktarılmaya çalışılacak kayıt adedi : {0}", trInfolar.Count);

            foreach (DataTransactionInfo logKaydi in trInfolar)
            {
                try
                {
                    tekLogKaydiniIsle(logKaydi);
                    _dbProxy.TrLogKaydiniSqleAktar(logKaydi);
                    _dbProxy.TransactionLogKayitSil(logKaydi);
                }
                catch (Exception ex)
                {
                    _dbProxy.TransactionLogKaydinaHataMesajiYaz(logKaydi, ex);
                }
            }

            _dbProxy.BaglantilariKapat();
        }

        private void tekLogKaydiniIsle(DataTransactionInfo logKaydi)
        {
            Kayit kaynaktakiKayit = _dbProxy.KaynaktanTekKayitAl(logKaydi);

            if (kaynaktakiKayit == null)
                _dbProxy.HedeftenKayitSil(logKaydi);
            else
                _dbProxy.HedefteInsertVeyaUpdate(kaynaktakiKayit, logKaydi);
        }
    }
}