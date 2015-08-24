using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdaVeriKatmani;
using NSubstitute;
using NUnit.Framework;

namespace AdaDataSync.API
{
	[TestFixture]
	public class DatabaseProxyTest
	{
		private ITemelVeriIslemleri _kaynakVeriIslemleri;
		private ITemelVeriIslemleri _hedefVeriIslemleri;

		[Test]
		public void insert_veya_update_hedef_veri_islemlerinde_calistirilir()
		{
			_kaynakVeriIslemleri = Substitute.For<ITemelVeriIslemleri>();
			_hedefVeriIslemleri = Substitute.For<ITemelVeriIslemleri>();

			DatabaseProxy dbProxy = new DatabaseProxy(_kaynakVeriIslemleri, _hedefVeriIslemleri);

			Kayit kaynaktakiKayit = new Kayit();
			DataTransactionInfo transactionInfo = new DataTransactionInfo();
			
			dbProxy.HedefteInsertVeyaUpdate(kaynaktakiKayit, transactionInfo);

			_hedefVeriIslemleri.ReceivedWithAnyArgs().TekKayitGuncelle(transactionInfo.TabloAdi, transactionInfo.PrimaryKeyKolonAdi, transactionInfo.PrimaryKeyDegeri, kaynaktakiKayit.DataRow);

			//_hedefVeriIslemleri.Received().SelectTekKolondan(transactionInfo.TabloAdi, transactionInfo.PrimaryKeyKolonAdi, transactionInfo.PrimaryKeyDegeri);
			//_hedefVeriIslemleri.ReceivedWithAnyArgs().DataAdaptorUpdateCagir();

		}

	}
	
	public class DatabaseProxy : IDatabaseProxy
	{
		private readonly ITemelVeriIslemleri _kaynakVeriIslemleri;
		private readonly ITemelVeriIslemleri _hedefVeriIslemleri;

		public DatabaseProxy(ITemelVeriIslemleri kaynakVeriIslemleri, ITemelVeriIslemleri hedefVeriIslemleri)
		{
			_kaynakVeriIslemleri = kaynakVeriIslemleri;
			_hedefVeriIslemleri = hedefVeriIslemleri;
		}

		public List<DataTransactionInfo> BekleyenTransactionlariAl()
		{
			DataTable dt = _kaynakVeriIslemleri.SelectTop("trlog", 10000);
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

		public void HedefteInsertVeyaUpdate(Kayit kaynaktakiKayit, DataTransactionInfo transactionInfo)
		{
			_hedefVeriIslemleri.TekKayitGuncelle(transactionInfo.TabloAdi, transactionInfo.PrimaryKeyKolonAdi, transactionInfo.PrimaryKeyDegeri, kaynaktakiKayit.DataRow);
			//new SqlDataAdapter()
			//DataRow drKayit = kaynaktakiKayit.DataRow;
			//SqlDataAdapter adaptor = (SqlDataAdapter) _hedefVeriIslemleri.DataAdapterAl("select * from " + transactionInfo.TabloAdi + " where " + transactionInfo.PrimaryKeyKolonAdi + " = " + transactionInfo.PrimaryKeyDegeri);
			//DataTable dtSql = new DataTable();
			//adaptor.Fill(dtSql);

			//if (dtSql.Rows.Count == 0)
			//	dtSql.Rows.Add(drKayit.ItemArray);
			//else
			//	dtSql.Rows[0].ItemArray = drKayit.ItemArray;

			//var updateOlabilmesiIcinDummyNesne = new SqlCommandBuilder(adaptor);
			//adaptor.Update(dtSql);

			////foxproTablosundanSil(foxproCon, fPrkTrLog);
		}

		public void TransactionLogKayitSil(DataTransactionInfo transactionLog)
		{
			throw new NotImplementedException();
		}

		public void TransactionLogKaydinaHataMesajiYaz(DataTransactionInfo transactionLog, string hataMesaji)
		{
			throw new NotImplementedException();
		}
	}
}