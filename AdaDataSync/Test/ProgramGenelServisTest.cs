﻿using System;
using System.Threading;
using AdaDataSync.API;
using NSubstitute;
using NUnit.Framework;

namespace AdaDataSync.Test
{
    [TestFixture]
    class ProgramGenelServisTest
    {
        private ICalisanServisKontrolcusu _calisanServisKontrolcusu;
        private IDataSyncYonetici _dataSyncYonetici;
        private ProgramGenelServis _programGenelServis;
        private const int BeklemeSuresi = 100;

        [SetUp]
        public void TestSetup()
        {
            _calisanServisKontrolcusu = Substitute.For<ICalisanServisKontrolcusu>();
            _dataSyncYonetici = Substitute.For<IDataSyncYonetici>();
            _programGenelServis = new ProgramGenelServis(_calisanServisKontrolcusu, BeklemeSuresi, _dataSyncYonetici);
        }

        /*
         *  senkronize ederken sql metadatayı da düzelt
         *  belirli zaman aralıklarında çalışmalı                                                       ok
         *  1'den fazla veritabanı çifti için aynı program hepsi için sync etmeli                       ok
         *  configdeki bağlantılar geçerli mi?
         */

        [Test]
        public void calisan_baska_bir_servis_varsa_hic_islem_yapmaz_ve_sync_cagirilmaz()
        {
            _calisanServisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu().Returns(true);
            _programGenelServis.Calistir();
            _dataSyncYonetici.DidNotReceiveWithAnyArgs().Sync();
        }

        [Test]
        public void calisan_baska_bir_servis_yoksa_sync_cagirilir()
        {
            _calisanServisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu().Returns(false);
            _programGenelServis.Calistir(1);
            _dataSyncYonetici.Received().Sync();
        }

        [Test]
        public void calisan_baska_bir_servis_varsa_sync_cagirilmaz()
        {
            _calisanServisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu().Returns(true);
            _programGenelServis.Calistir(1);
            _dataSyncYonetici.DidNotReceive().Sync();
        }

        [Test]
        public void birden_fazla_datasyncYonetici_oldugunda_hepsinin_sync_metodu_calisir()
        {
            ICalisanServisKontrolcusu csk = Substitute.For<ICalisanServisKontrolcusu>();
            csk.BuMakinadaBaskaServisCalisiyorMu().Returns(false);
            IDataSyncYonetici syncYonetici1 = Substitute.For<IDataSyncYonetici>();
            IDataSyncYonetici syncYonetici2 = Substitute.For<IDataSyncYonetici>();

            ProgramGenelServis pgs = new ProgramGenelServis(csk, syncYonetici1, syncYonetici2);
            pgs.Calistir(1);

            syncYonetici1.Received().Sync();
            syncYonetici2.Received().Sync();
        }

        [Test]
        public void calistirma_metoduna_parametre_gonderilmezse_sync_metodu_belirli_zaman_araliklariyla_tekrar_tekrar_calismali()
        {
            Thread yeniThread = new Thread(() => _programGenelServis.Calistir());
            yeniThread.Start();

            const double katsayi = 2.5;
            const int sleepSuresi = (int)(BeklemeSuresi*katsayi);
            
            Thread.Sleep(sleepSuresi);
            int receiveSayisi = (int) Math.Floor(katsayi) + 1;
            _dataSyncYonetici.Received(receiveSayisi).Sync();
        }

        /// <summary>
        /// Sync işlem süresi, bekleme süresinden küçük ya da eşitse uzun döngülerde zaman kaybı olmamalı.
        /// Aynı sayıda çalıştırılmalı.
        /// </summary>
        [Test]
        public void beklemeSuresi_1_saniye_iken_sync_islemi_de_1_saniye_suruyorsa_sync_bittigi_gibi_yenisi_baslamali()
        {
            _dataSyncYonetici.When(ds => ds.Sync()).Do(x => Thread.Sleep(BeklemeSuresi));
            calistirma_metoduna_parametre_gonderilmezse_sync_metodu_belirli_zaman_araliklariyla_tekrar_tekrar_calismali();
        }

        //[Test]
        //public void her_turda_sync_yapmadan_once_ddlog_tablosundaki_structure_degisikliklerine_bakarak_
    }
}
