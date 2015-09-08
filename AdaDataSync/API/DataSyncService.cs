using System;
using System.Collections.Generic;

namespace AdaDataSync.API
{
    public class DataSyncService : IDataSyncService
    {
        private readonly IDatabaseProxy _dbProxy;
        //private readonly ICalisanServisKontrolcusu _servisKontrolcusu;
        //private readonly ISafetyNetLogger _ikincilLogger;

        //public DataSyncService(IDatabaseProxy dbProxy, ICalisanServisKontrolcusu servisKontrolcusu, ISafetyNetLogger ikincilLogger)
        //{
        //    _dbProxy = dbProxy;
        //    _servisKontrolcusu = servisKontrolcusu;
        //    _ikincilLogger = ikincilLogger;
        //}

        public DataSyncService(IDatabaseProxy dbProxy)
        {
            _dbProxy = dbProxy;
        }

        public void Sync()
        {
            //if (_servisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu())
            //    return;

            //_servisKontrolcusu.MakinaBazindaKilitKoy();

            //try
            //{
            //    _dbProxy.BekleyenTransactionlariAl().ForEach(logKaydi =>
            //    {
            //        try
            //        {
            //            tekLogKaydiniIsle(logKaydi);
            //        }
            //        catch (Exception ex)
            //        {
            //            _dbProxy.TransactionLogKaydinaHataMesajiYaz(logKaydi, ex.Message);
            //        }
            //    });
            //}
            //catch (Exception ex)
            //{
            //    _ikincilLogger.HataLogla(ex);
            //}

            const int herSeferindeAlinacakKayitMaksSayisi = 10000;

            //_dbProxy.BekleyenTransactionlariAl(herSeferindeAlinacakKayitMaksSayisi).ForEach(logKaydi =>
            //{
            //    try
            //    {
            //        tekLogKaydiniIsle(logKaydi);
            //    }
            //    catch (Exception ex)
            //    {
            //        _dbProxy.TransactionLogKaydinaHataMesajiYaz(logKaydi, ex);
            //    }
            //});

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