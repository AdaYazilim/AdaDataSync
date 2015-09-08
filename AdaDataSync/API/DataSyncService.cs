using System;
using System.Collections.Generic;

namespace AdaDataSync.API
{
    public class DataSyncService : IDataSyncService
    {
        private readonly IDatabaseProxy _dbProxy;

        public DataSyncService(IDatabaseProxy dbProxy)
        {
            _dbProxy = dbProxy;
        }

        public void Sync()
        {
            const int herSeferindeAlinacakKayitMaksSayisi = 10000;

            List<DataTransactionInfo> trInfolar = _dbProxy.BekleyenTransactionlariAl(herSeferindeAlinacakKayitMaksSayisi);
            Console.WriteLine(string.Format("Aktarılmaya çalışılacak kayıt adedi : {0}", trInfolar.Count));

            foreach (DataTransactionInfo logKaydi in trInfolar)
            {
                try
                {
                    tekLogKaydiniIsle(logKaydi);
                }
                catch (Exception ex)
                {
                    _dbProxy.TransactionLogKaydinaHataMesajiYaz(logKaydi, ex);
                }
            }
        }

        private void tekLogKaydiniIsle(DataTransactionInfo logKaydi)
        {
            Kayit kaynaktakiKayit = _dbProxy.KaynaktanTekKayitAl(logKaydi);

            if (kaynaktakiKayit == null)
                _dbProxy.HedeftenKayitSil(logKaydi);
            else
                _dbProxy.HedefteInsertVeyaUpdate(kaynaktakiKayit, logKaydi);

            _dbProxy.TransactionLogKayitSil(logKaydi);
        }
    }
}