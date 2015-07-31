using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AdaVeriKatmani;

namespace AdaDataSync.API
{
	public class SyncRepoProxy : ISyncRepoProxy
	{
		private readonly ITemelVeriIslemleri _kaynakVeriIslemleri;
		private readonly ITemelVeriIslemleri _hedefVeriIslemleri;

		public SyncRepoProxy(ITemelVeriIslemleri kaynakVeriIslemleri, ITemelVeriIslemleri hedefVeriIslemleri)
		{
			_kaynakVeriIslemleri = kaynakVeriIslemleri;
			_hedefVeriIslemleri = hedefVeriIslemleri;
		}

		public List<DataTransactionInfo> BekleyenTransactionlariAl()
		{
			DataTable dt = _kaynakVeriIslemleri.SelectTop("trlog", 1000);
			return (from DataRow dr in dt.Rows select DataTransactionInfo.Yarat(dr)).ToList();
		}

		public Kayit KaynaktanTekKayitAl(DataTransactionInfo transactionInfo)
		{
			DataTable dt = _kaynakVeriIslemleri.Select(transactionInfo.TabloAdi, transactionInfo.PrimaryKeyKolonAdi, transactionInfo.PrimaryKeyDegeri);
			if (dt.Rows.Count == 0)
				return null;
			return new Kayit(dt.Rows[0]);
		}

		public void HedeftenKayitSil(DataTransactionInfo transactionInfo)
		{
			_hedefVeriIslemleri.Delete(transactionInfo.TabloAdi, transactionInfo.PrimaryKeyKolonAdi, transactionInfo.PrimaryKeyDegeri);
		}

		public void HedefteInsertVeyaUpdate(Kayit kaynaktakiKayit)
		{
			throw new NotImplementedException();
		}
	}
}