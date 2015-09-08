﻿using System;
using AdaDataSync.API;
using NSubstitute;
using NUnit.Framework;

namespace AdaDataSync.Test
{
    [TestFixture]
    class ProgramGenelServisTest
    {
        private ICalisanServisKontrolcusu _calisanServisKontrolcusu;
        private ISafetyNetLogger _safetyLogger;
        private IDataSyncService _dataSyncServis;
        private ProgramGenelServis _programGenelServis;

        [SetUp]
        public void TestSetup()
        {
            _calisanServisKontrolcusu = Substitute.For<ICalisanServisKontrolcusu>();
            _safetyLogger = Substitute.For<ISafetyNetLogger>();
            _dataSyncServis = Substitute.For<IDataSyncService>();
            _programGenelServis = new ProgramGenelServis(_calisanServisKontrolcusu, _safetyLogger, _dataSyncServis);
        }

        /*
            senkronize ederken sql metadatayı da düzelt
            belirli zaman aralıklarında çalışmalı
            trlog çok kayıt içeriyorsa her seferinde ilk belirli bir sayıda kayıt alınmalı
            sql tablosunun primary keye sahip olduğu assert edilebilir.
            1'den fazla veritabanı çifti için aynı program hepsi için sync etmeli
         * configdeki bağlantılar geçerli mi?
         * 
         */

        [Test]
        public void calisan_baska_bir_servis_varsa_hic_islem_yapmaz_ve_sync_cagirilmaz()
        {
            _calisanServisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu().Returns(true);
            _programGenelServis.Calistir();
            _dataSyncServis.DidNotReceiveWithAnyArgs().Sync();
        }

        [Test]
        public void calisan_baska_bir_servis_yoksa_global_lock_koyar()
        {
            _calisanServisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu().Returns(false);
            _programGenelServis.Calistir();
            _calisanServisKontrolcusu.Received().MakinaBazindaKilitKoy();
        }

        [Test]
        public void sync_esnasinda_herhangi_bir_unhandled_exception_alinirsa_program_patlamamali_ve_ikincil_loga_kayit_atilmali()
        {
            Exception ex = new Exception();
            _dataSyncServis.When(ds => ds.Sync()).Do(x => { throw ex; });

            Assert.DoesNotThrow(() => _programGenelServis.Calistir());
            _safetyLogger.Received().HataLogla(ex);
        }

        //[Test]
        //public void birden_fazla_databaseproxy_ile_sync_yapılabilmeli()
        //{

        //}
    }
}