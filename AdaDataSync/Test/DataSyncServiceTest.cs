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
		private ICalisanServisKontrolcusu _servisKontrolcusu;
		private ISafetyNetLogger _ikincilLogger;
		private DataSyncService _service;

		[SetUp]
		public void TestSetup()
		{
			_dbProxy = Substitute.For<IDatabaseProxy>();
			_servisKontrolcusu = Substitute.For<ICalisanServisKontrolcusu>();
			_ikincilLogger = Substitute.For<ISafetyNetLogger>();
			_service = new DataSyncService(_dbProxy, _servisKontrolcusu, _ikincilLogger);
		}

		[Test]
		public void kaynakta_kayit_yoksa_hedeften_siler()
		{
			var transaction = tekTransactionluTestOrtamiHazirla();

			_service.Sync();

			_dbProxy.Received().HedeftenKayitSil(transaction);
		}


		[Test]
		public void kaynakta_kayit_varsa_hedefe_kaydi_gonder()
		{
			Kayit kaynaktakiKayit = new Kayit();
			tekTransactionluTestOrtamiHazirla(kaynaktakiKayit);

			_service.Sync();

			_dbProxy.Received().HedefteInsertVeyaUpdate(kaynaktakiKayit);
		}

		[Test]		
		public void calisan_baska_bir_servis_varsa_hic_islem_yapmaz()
		{
			tekTransactionluTestOrtamiHazirla();

			_servisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu().Returns(true);

			_service.Sync();

			_dbProxy.DidNotReceiveWithAnyArgs().BekleyenTransactionlariAl();
		}

		[Test]
		public void calisan_baska_bir_servis_yoksa_global_lock_koyar()
		{
			tekTransactionluTestOrtamiHazirla();

			_servisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu().Returns(false);

			_service.Sync();

			_servisKontrolcusu.Received().MakinaBazindaKilitKoy();
		}

		[Test]
		public void hedefte_islem_basariyla_yapildiktan_sonra_transaction_logdan_kaydi_siler()//insert veya update
		{
			//given
			List<DataTransactionInfo> ornekTransactionLogKayitlari = ornekTransactionLogKayitlariYarat(1);
			_dbProxy.BekleyenTransactionlariAl().Returns(ornekTransactionLogKayitlari);//kaynakta transaction logda tek kayıt var
			Kayit kaynaktakiKayit = new Kayit();
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
			_dbProxy.BekleyenTransactionlariAl().Returns(ornekTransactionLogKayitlari);//kaynakta transaction logda tek kayıt var
			Kayit kaynaktakiKayit = new Kayit();
			_dbProxy.KaynaktanTekKayitAl(null).ReturnsForAnyArgs(kaynaktakiKayit);//kaynakta kayıt olduğunu simule ediyorum
			_dbProxy.
				When(proxy => proxy.HedefteInsertVeyaUpdate(kaynaktakiKayit)).
				Do(x => { throw new Exception("Hedefte güncellerken hata oluştu"); });

			//when
			_service.Sync();

			//then
			_dbProxy.ReceivedWithAnyArgs(1).TransactionLogKaydinaHataMesajiYaz(ornekTransactionLogKayitlari[0], "");
		}

		[Test]
		public void kaynaktan_transaction_log_alinirken_hata_olursa_dosya_sistemine_logla()
		{
			_dbProxy.When(proxy => proxy.BekleyenTransactionlariAl()).Do(x => { throw new Exception("Transaction log alınamıyor"); });

			_service.Sync();

			_ikincilLogger.ReceivedWithAnyArgs(1).HataLogla(null);
		}

		[Test]
		public void hedefte_islem_yaparken_hata_alinir_bu_hata_loglanirkende_hata_alinirsa_dosya_sistemine_logla()
		{
			//given
			List<DataTransactionInfo> ornekTransactionLogKayitlari = ornekTransactionLogKayitlariYarat(1);
			_dbProxy.BekleyenTransactionlariAl().Returns(ornekTransactionLogKayitlari);//kaynakta transaction logda tek kayıt var
			Kayit kaynaktakiKayit = new Kayit();
			_dbProxy.KaynaktanTekKayitAl(null).ReturnsForAnyArgs(kaynaktakiKayit);//kaynakta kayıt olduğunu simule ediyorum
			_dbProxy.
				When(proxy => proxy.HedefteInsertVeyaUpdate(kaynaktakiKayit)).
				Do(x => { throw new Exception("Hedefte güncellerken hata oluştu"); });
			_dbProxy.
				WhenForAnyArgs(proxy => proxy.TransactionLogKaydinaHataMesajiYaz(null, null)).
				Do(x => { throw new Exception("Transaction loga hata yazarken hata oluştu"); });

			//when
			_service.Sync();

			//then
			_ikincilLogger.ReceivedWithAnyArgs(1).HataLogla(null);
		}

		private static List<DataTransactionInfo> ornekTransactionLogKayitlariYarat(int adet)
		{

			List<DataTransactionInfo> kayitlar = new List<DataTransactionInfo>();
			for (int i = 0; i < adet; i++)
				kayitlar.Add(new DataTransactionInfo());
			return kayitlar;
		}

		private DataTransactionInfo tekTransactionluTestOrtamiHazirla(Kayit kaynaktakiKayit = null)
		{
			DataTransactionInfo transactionInfo = new DataTransactionInfo();
			_dbProxy.BekleyenTransactionlariAl().Returns(new List<DataTransactionInfo> {transactionInfo});
			_dbProxy.KaynaktanTekKayitAl(transactionInfo).Returns(kaynaktakiKayit);
			return transactionInfo;
		}
	}
}
