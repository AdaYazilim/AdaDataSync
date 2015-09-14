using System;
using System.Collections.Generic;

namespace AdaDataSync.API
{
    public interface IDatabaseProxy
    {
        void BaglantilariAc();
        void BaglantilariKapat();
        List<DataTransactionInfo> BekleyenTransactionlariAl(int kayitSayisi);
        Kayit KaynaktanTekKayitAl(DataTransactionInfo transactionInfo);
        void HedeftenKayitSil(DataTransactionInfo transactionInfo);
        void HedefteInsertVeyaUpdate(Kayit kaynaktakiKayit, DataTransactionInfo transactionInfo);
        void LogKayitSil(DataTransactionInfo transactionLog);
        void LogKaydinaHataMesajiYaz(DataTransactionInfo transactionLog, Exception ex);
        void LogKaydiniSqleAktar(DataTransactionInfo transactionInfo);
    }
}