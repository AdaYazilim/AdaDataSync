using System;
using System.Collections.Generic;
using AdaDataSync.API;
using NSubstitute;
using NUnit.Framework;

namespace AdaDataSync.Test
{
	[TestFixture]
	class DataSyncServiceTest
	{
		private IDatabaseProxy _dbProxy;
	    private DataSyncService _service;

		[SetUp]
		public void TestSetup()
		{
			_dbProxy = Substitute.For<IDatabaseProxy>();
			Substitute.For<ICalisanServisKontrolcusu>();
			Substitute.For<ISafetyNetLogger>();
            _service = new DataSyncService(_dbProxy);
		}

		[Test]
		public void kaynakta_kayit_yoksa_hedeften_siler()
		{
			DataTransactionInfo transaction = tekTransactionluTestOrtamiHazirla(null);

			_service.Sync();

			_dbProxy.Received().HedeftenKayitSil(transaction);
		}

        [Test]
        public void kaynakta_kayit_varsa_hedefe_kaydi_gonder()
        {
            Kayit kaynaktakiKayit = new Kayit(null);
            tekTransactionluTestOrtamiHazirla(kaynaktakiKayit);

            _service.Sync();

            _dbProxy.Received().HedefteInsertVeyaUpdate(kaynaktakiKayit, Arg.Any<DataTransactionInfo>());
        }

		[Test]
		public void hedefte_islem_basariyla_yapildiktan_sonra_transaction_logdan_kaydi_siler()//insert veya update
		{
			//given
			List<DataTransactionInfo> ornekTransactionLogKayitlari = ornekTransactionLogKayitlariYarat(1);
			_dbProxy.BekleyenTransactionlariAl(0).ReturnsForAnyArgs(ornekTransactionLogKayitlari);//kaynakta transaction logda tek kayıt var
			Kayit kaynaktakiKayit = new Kayit(null);
			_dbProxy.KaynaktanTekKayitAl(null).ReturnsForAnyArgs(kaynaktakiKayit);//kaynakta kayıt olduğunu simule ediyorum

			//when
			_service.Sync();

			//then
			_dbProxy.Received(1).TransactionLogKayitSil(ornekTransactionLogKayitlari[0]);
		}

		[Test]
		public void hedefte_islem_basarisiz_olursa_transaction_log_kaydi_silinmez_hata_mesaji_eklenir()//insert veya update
		{
			//given
			List<DataTransactionInfo> ornekTransactionLogKayitlari = ornekTransactionLogKayitlariYarat(1);
			_dbProxy.BekleyenTransactionlariAl(0).ReturnsForAnyArgs(ornekTransactionLogKayitlari);//kaynakta transaction logda tek kayıt var
			Kayit kaynaktakiKayit = new Kayit(null);
			_dbProxy.KaynaktanTekKayitAl(null).ReturnsForAnyArgs(kaynaktakiKayit);//kaynakta kayıt olduğunu simule ediyorum
			_dbProxy.
				When(proxy => proxy.HedefteInsertVeyaUpdate(kaynaktakiKayit, Arg.Any<DataTransactionInfo>())).
				Do(x => { throw new Exception("Hedefte güncellerken hata oluştu"); });

			//when
			_service.Sync();

			//then
			_dbProxy.ReceivedWithAnyArgs(1).TransactionLogKaydinaHataMesajiYaz(ornekTransactionLogKayitlari[0], new Exception());
		}

		private static List<DataTransactionInfo> ornekTransactionLogKayitlariYarat(int adet)
		{

			List<DataTransactionInfo> kayitlar = new List<DataTransactionInfo>();
		    for (int i = 0; i < adet; i++)
		        kayitlar.Add(new DataTransactionInfo(5 + i, "pol", "fprkpol", 11234 + 2*i));
			return kayitlar;
		}

		private DataTransactionInfo tekTransactionluTestOrtamiHazirla(Kayit kaynaktakiKayit)
		{
			DataTransactionInfo transactionInfo = new DataTransactionInfo(7, "pol", "fprkpol", 12345);
			_dbProxy.BekleyenTransactionlariAl(0).ReturnsForAnyArgs(new List<DataTransactionInfo> {transactionInfo});
			_dbProxy.KaynaktanTekKayitAl(transactionInfo).Returns(kaynaktakiKayit);
			return transactionInfo;
		}
	}
}
