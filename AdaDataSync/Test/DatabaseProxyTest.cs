using System;
using AdaDataSync.API;
using AdaVeriKatmani;
using NSubstitute;
using NUnit.Framework;

namespace AdaDataSync.Test
{
    [TestFixture]
    public class DatabaseProxyTest
    {
        private ITekConnectionVeriIslemleri _kaynakVeriIslemleri;
        private ITekConnectionVeriIslemleri _hedefVeriIslemleri;
        private DatabaseProxy _dbProxy;

        [SetUp]
        public void TestSetUp()
        {
            _kaynakVeriIslemleri = Substitute.For<ITekConnectionVeriIslemleri>();
            _hedefVeriIslemleri = Substitute.For<ITekConnectionVeriIslemleri>();

            _dbProxy = new DatabaseProxy(_kaynakVeriIslemleri, _hedefVeriIslemleri);
        }
        
        // Bu testi tek başına çalıştırınca geçiyor, toplu çalıştırıldığında patlıyor. Anlamadım!
        [Test]
        public void insert_veya_update_hedef_veri_islemlerinde_calistirilir()
        {
            Kayit kaynaktakiKayit = new Kayit(null);
            DataTransactionInfo transactionInfo = new DataTransactionInfo(7, "pol", "fprkpol", 12345, "i", false);
            _dbProxy.HedefteInsertVeyaUpdate(kaynaktakiKayit, transactionInfo);
            _hedefVeriIslemleri.ReceivedWithAnyArgs().TekKayitGuncelle(transactionInfo.TabloAdi, transactionInfo.PrimaryKeyKolonAdi, transactionInfo.PrimaryKeyDegeri, kaynaktakiKayit.DataRowItemArray);
        }

        //[Test]
        //public void FoxproTarafindaGuncellemeYapiliyor_metodu_cagirildiginda_foxpro_baglantisi_acik_degilse_exception_atmali()
        //{
        //    Assert.Throws<Exception>(() => _dbProxy.FoxproTarafindaGuncellemeYapiliyor());
        //}

        //[Test]
        //public void BekleyenTransactionlariAl_metodu_cagirildiginda_foxpro_baglantisi_acik_degilse_exception_atmali()
        //{
        //    Assert.Throws<Exception>(() => _dbProxy.BekleyenTransactionlariAl(Arg.Any<int>()));
        //}

        //[Test]
        //public void KaynaktanTekKayitAl_metodu_cagirildiginda_foxpro_baglantisi_acik_degilse_exception_atmali()
        //{
        //    Assert.Throws<Exception>(() => _dbProxy.KaynaktanTekKayitAl(Arg.Any<DataTransactionInfo>()));
        //}

        //[Test]
        //public void HedeftenKayitSil_metodu_cagirildiginda_sql_baglantisi_acik_degilse_exception_atmali()
        //{
        //    Assert.Throws<Exception>(() => _dbProxy.HedeftenKayitSil(Arg.Any<DataTransactionInfo>()));
        //}

        //[Test]
        //public void TrLogKaydiniSqleAktar_metodu_cagirildiginda_sql_baglantisi_acik_degilse_exception_atmali()
        //{
        //    Assert.Throws<Exception>(() => _dbProxy.TrLogKaydiniSqleAktar(Arg.Any<DataTransactionInfo>()));
        //}

        //[Test]
        //public void TransactionLogKayitSil_metodu_cagirildiginda_foxpro_baglantisi_acik_degilse_exception_atmali()
        //{
        //    Assert.Throws<Exception>(() => _dbProxy.TransactionLogKayitSil(Arg.Any<DataTransactionInfo>()));
        //}

        //[Test]
        //public void TransactionLogKaydinaHataMesajiYaz_metodu_cagirildiginda_foxpro_baglantisi_acik_degilse_exception_atmali()
        //{
        //    Assert.Throws<Exception>(() => _dbProxy.TransactionLogKaydinaHataMesajiYaz(Arg.Any<DataTransactionInfo>(), Arg.Any<Exception>()));
        //}

        //[Test]
        //public void baglanti_acikken_tekrar_acilirsa_exception_atmali()
        //{
        //    _dbProxy.BaglantilariAc();

        //    Assert.Throws<Exception>(() => _dbProxy.BaglantilariAc());
        //}

        //[Test]
        //public void baglanti_kapali_iken_tekrar_kapatilmaya_calisilirsa_exception_atmali()
        //{
        //    Assert.Throws<Exception>(() => _dbProxy.BaglantilariKapat());
        //}
    }
}
