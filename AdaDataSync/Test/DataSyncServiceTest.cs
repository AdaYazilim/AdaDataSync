using System.Collections.Generic;
using AdaDataSync.API;
using NSubstitute;
using NUnit.Framework;

namespace AdaDataSync.Test
{
	[TestFixture]
	class DataSyncServiceTest
	{
		private ISyncRepoProxy _repoProxy;
		private ICalisanServisKontrolcusu _servisKontrolcusu;
		private DataSyncService _service;

		[SetUp]
		public void TestSetup()
		{
			_repoProxy = Substitute.For<ISyncRepoProxy>();
			_servisKontrolcusu = Substitute.For<ICalisanServisKontrolcusu>();
			_service = new DataSyncService(_repoProxy, _servisKontrolcusu);
		}

		[Test]
		public void kaynakta_kayit_yoksa_hedeften_siler()
		{
			var transaction = tekTransactionluTestOrtamiHazirla();

			_service.Sync();

			_repoProxy.Received().HedeftenKayitSil(transaction);
		}


		[Test]
		public void kaynakta_kayit_varsa_hedefe_kaydi_gonder()
		{
			Kayit kaynaktakiKayit = new Kayit();
			tekTransactionluTestOrtamiHazirla(kaynaktakiKayit);

			_service.Sync();

			_repoProxy.Received().HedefteInsertVeyaUpdate(kaynaktakiKayit);
		}

		[Test]		
		public void calisan_baska_bir_servis_varsa_hic_islem_yapmaz()
		{
			tekTransactionluTestOrtamiHazirla();

			_servisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu().Returns(true);

			_service.Sync();

			_repoProxy.DidNotReceiveWithAnyArgs().BekleyenTransactionlariAl();
		}

		[Test]
		public void calisan_baska_bir_servis_yoksa_global_lock_koyar()
		{
			tekTransactionluTestOrtamiHazirla();

			_servisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu().Returns(false);

			_service.Sync();

			_servisKontrolcusu.Received().MakinaBazindaKilitKoy();
		}

		//[Test]
		//public void senkronizasyon_sonrasi_transaction_log_silinir()
		//{
		//	var transaction = tekTransactionluTestOrtamiHazirla();

		//	_service.Sync();

		//	_repoProxy.Received().KaynaktanTransactionSil(transaction);
		//}

		private DataTransactionInfo tekTransactionluTestOrtamiHazirla(Kayit kaynaktakiKayit = null)
		{
			DataTransactionInfo transactionInfo = new DataTransactionInfo();
			_repoProxy.BekleyenTransactionlariAl().Returns(new List<DataTransactionInfo> {transactionInfo});
			_repoProxy.KaynaktanTekKayitAl(transactionInfo).Returns(kaynaktakiKayit);
			return transactionInfo;
		}
	}
}
