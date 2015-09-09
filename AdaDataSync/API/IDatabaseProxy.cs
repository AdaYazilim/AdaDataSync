using System;
using System.Collections.Generic;

namespace AdaDataSync.API
{
	public interface IDatabaseProxy
	{
	    bool FoxproTarafindaGuncellemeYapiliyor();
		List<DataTransactionInfo> BekleyenTransactionlariAl(int kayitSayisi);
		Kayit KaynaktanTekKayitAl(DataTransactionInfo transactionInfo);
		void HedeftenKayitSil(DataTransactionInfo transactionInfo);
		void HedefteInsertVeyaUpdate(Kayit kaynaktakiKayit, DataTransactionInfo transactionInfo);
		void TransactionLogKayitSil(DataTransactionInfo transactionLog);
		void TransactionLogKaydinaHataMesajiYaz(DataTransactionInfo transactionLog, Exception ex);
        void TrLogKaydiniSqleAktar(DataTransactionInfo transactionInfo);
	}
}