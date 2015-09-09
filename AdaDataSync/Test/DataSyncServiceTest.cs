﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdaDataSync.API;
using NSubstitute;
using NUnit.Framework;

namespace AdaDataSync.Test
{
    /*
     * trlog çok kayıt içeriyorsa her seferinde ilk belirli bir sayıda kayıt alınmalı
     * sql tablosunun primary keye sahip olduğu assert edilebilir.
     */
    [TestFixture]
	class DataSyncServiceTest
	{
		private IDatabaseProxy _dbProxy;
	    private DataSyncService _service;

		[SetUp]
		public void TestSetup()
		{
			_dbProxy = Substitute.For<IDatabaseProxy>();
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
        public void hedefte_islem_basariyla_yapildiktan_sonra_bu_kaydi_sqldeki_trloga_aktarir_ve_foxprodaki_trlogdan_silinir()
        {
            //given
            List<DataTransactionInfo> ornekTransactionLogKayitlari = ornekTransactionLogKayitlariYarat(1);
            _dbProxy.BekleyenTransactionlariAl(0).ReturnsForAnyArgs(ornekTransactionLogKayitlari);//kaynakta transaction logda tek kayıt var
            Kayit kaynaktakiKayit = new Kayit(null);
            _dbProxy.KaynaktanTekKayitAl(null).ReturnsForAnyArgs(kaynaktakiKayit);//kaynakta kayıt olduğunu simule ediyorum

            //when
            _service.Sync();

            //then
            DataTransactionInfo tekTrInfo = ornekTransactionLogKayitlari.Single();
            _dbProxy.Received(1).TrLogKaydiniSqleAktar(tekTrInfo);
            _dbProxy.Received(1).TransactionLogKayitSil(tekTrInfo);
        }

        [Test]
        public void hedefte_islem_basariyla_yapilmazsa_bu_kayit_sqle_aktarilmaz_ve_trlogdan_silinmez()
        {
            //given
            List<DataTransactionInfo> ornekTransactionLogKayitlari = ornekTransactionLogKayitlariYarat(1);
            _dbProxy.BekleyenTransactionlariAl(0).ReturnsForAnyArgs(ornekTransactionLogKayitlari); //kaynakta transaction logda tek kayıt var
            Kayit kaynaktakiKayit = new Kayit(null);
            _dbProxy.KaynaktanTekKayitAl(null).ReturnsForAnyArgs(kaynaktakiKayit); //kaynakta kayıt olduğunu simule ediyorum
            DataTransactionInfo tekTrInfo = ornekTransactionLogKayitlari.Single();
            _dbProxy
                .When(dbp => dbp.HedefteInsertVeyaUpdate(kaynaktakiKayit, tekTrInfo))
                .Do(x =>{throw new Exception();});
            //when
            _service.Sync();

            //then
            _dbProxy.DidNotReceive().TrLogKaydiniSqleAktar(tekTrInfo);
            _dbProxy.DidNotReceive().TransactionLogKayitSil(tekTrInfo);
        }
        
        [Test]
        public void trlog_sqle_aktarilirken_hata_atarsa_transaction_logdan_kayit_silinmez()
        {
            //given
            List<DataTransactionInfo> ornekTransactionLogKayitlari = ornekTransactionLogKayitlariYarat(1);
            _dbProxy.BekleyenTransactionlariAl(0).ReturnsForAnyArgs(ornekTransactionLogKayitlari);//kaynakta transaction logda tek kayıt var
            Kayit kaynaktakiKayit = new Kayit(null);
            _dbProxy.KaynaktanTekKayitAl(null).ReturnsForAnyArgs(kaynaktakiKayit);//kaynakta kayıt olduğunu simule ediyorum
            DataTransactionInfo tekTrInfo = ornekTransactionLogKayitlari.Single();
            _dbProxy
                .When(dbp => dbp.TrLogKaydiniSqleAktar(tekTrInfo))
                .Do(x => { throw new Exception(); });

            //when
            _service.Sync();

            //then
            _dbProxy.DidNotReceive().TransactionLogKayitSil(ornekTransactionLogKayitlari[0]);
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

        [Test]
        public void kaynaktan_transaction_log_alinirken_hata_olursa_sync_metodu_ayni_hatayi_throw_etmeli()
        {
            Exception ex = new Exception("Transaction log alınamıyor");
            _dbProxy.WhenForAnyArgs(proxy => proxy.BekleyenTransactionlariAl(0)).Do(x => { throw ex; });

            Assert.Throws<Exception>(() => _service.Sync());
        }

        [Test]
        public void hedefte_islem_yaparken_hata_alinir_bu_hata_loglanirkende_hata_alinirsa_sync_metodu_ayni_hatayi_throw_etmeli()
        {
            //given
            List<DataTransactionInfo> ornekTransactionLogKayitlari = ornekTransactionLogKayitlariYarat(1);
            _dbProxy.BekleyenTransactionlariAl(0).ReturnsForAnyArgs(ornekTransactionLogKayitlari);//kaynakta transaction logda tek kayıt var
            Kayit kaynaktakiKayit = new Kayit(null);
            _dbProxy.KaynaktanTekKayitAl(null).ReturnsForAnyArgs(kaynaktakiKayit);//kaynakta kayıt olduğunu simule ediyorum
            _dbProxy.
                When(proxy => proxy.HedefteInsertVeyaUpdate(kaynaktakiKayit, Arg.Any<DataTransactionInfo>())).
                Do(x => { throw new Exception("Hedefte güncellerken hata oluştu"); });
            _dbProxy.
                WhenForAnyArgs(proxy => proxy.TransactionLogKaydinaHataMesajiYaz(null, null)).
                Do(x => { throw new Exception("Transaction loga hata yazarken hata oluştu"); });

            Assert.Throws<Exception>(() => _service.Sync());
        }

        [Test]
        public void cari_program_guncellemeye_basladiysa_dbProxynin_bekleyen_islemleri_alinmaz_ve_sync_metodu_exception_atar()
        {
            _dbProxy.FoxproTarafindaGuncellemeYapiliyor().Returns(true);
            Assert.Throws<Exception>(() => _service.Sync());
            _dbProxy.DidNotReceiveWithAnyArgs().BekleyenTransactionlariAl(10000);
        }

		private static List<DataTransactionInfo> ornekTransactionLogKayitlariYarat(int adet)
		{
			List<DataTransactionInfo> kayitlar = new List<DataTransactionInfo>();
		    for (int i = 0; i < adet; i++)
		        kayitlar.Add(new DataTransactionInfo(5 + i, "pol", "fprkpol", 11234 + 2*i, "i", false));
			return kayitlar;
		}

		private DataTransactionInfo tekTransactionluTestOrtamiHazirla(Kayit kaynaktakiKayit)
		{
			DataTransactionInfo transactionInfo = new DataTransactionInfo(7, "pol", "fprkpol", 12345, "i", false);
			_dbProxy.BekleyenTransactionlariAl(0).ReturnsForAnyArgs(new List<DataTransactionInfo> {transactionInfo});
			_dbProxy.KaynaktanTekKayitAl(transactionInfo).Returns(kaynaktakiKayit);
			return transactionInfo;
		}
	}
}
