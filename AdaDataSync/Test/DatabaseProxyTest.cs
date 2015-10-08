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
	    private DataTransactionInfo _pol12345TransactionInfo;

	    [SetUp]
        public void TestSetUp()
        {
            _kaynakVeriIslemleri = Substitute.For<ITekConnectionVeriIslemleri>();
            _hedefVeriIslemleri = Substitute.For<ITekConnectionVeriIslemleri>();
	        ILogger logger = Substitute.For<ILogger>();

            _dbProxy = new DatabaseProxy(_kaynakVeriIslemleri, _hedefVeriIslemleri, logger);

            _pol12345TransactionInfo = new DataTransactionInfo(7, "pol", "fprkpol", 12345, "i", false);
        }
        
        // Bu testi tek başına çalıştırınca geçiyor, toplu çalıştırıldığında patlıyor. Anlamadım!
        [Test]
        public void insert_veya_update_hedef_veri_islemlerinde_calistirilir()
        {
            Kayit kaynaktakiKayit = new Kayit(null);

			_dbProxy.BaglantilariAc();
            _dbProxy.HedefteInsertVeyaUpdate(kaynaktakiKayit, _pol12345TransactionInfo);
            _dbProxy.BaglantilariKapat();

			_hedefVeriIslemleri.ReceivedWithAnyArgs().TekKayitGuncelle(_pol12345TransactionInfo.TabloAdi, _pol12345TransactionInfo.PrimaryKeyKolonAdi, _pol12345TransactionInfo.PrimaryKeyDegeri, kaynaktakiKayit.DataRowItemArray);
        }

        [Test]
        public void BekleyenTransactionlariAl_metodu_cagirildiginda_foxpro_baglantisi_acik_degilse_exception_atmali()
        {
            Assert.Throws<Exception>(() => _dbProxy.BekleyenTransactionlariAl(0));
        }

        [Test]
        public void KaynaktanTekKayitAl_metodu_cagirildiginda_foxpro_baglantisi_acik_degilse_exception_atmali()
        {
            Assert.Throws<Exception>(() => _dbProxy.KaynaktanTekKayitAl(_pol12345TransactionInfo));
        }

        [Test]
        public void HedeftenKayitSil_metodu_cagirildiginda_sql_baglantisi_acik_degilse_exception_atmali()
        {
            Assert.Throws<Exception>(() => _dbProxy.HedeftenKayitSil(_pol12345TransactionInfo));
        }

        [Test]
        public void TrLogKaydiniSqleAktar_metodu_cagirildiginda_sql_baglantisi_acik_degilse_exception_atmali()
        {
            Assert.Throws<Exception>(() => _dbProxy.LogKaydiniSqleAktar(_pol12345TransactionInfo));
        }

        [Test]
        public void TransactionLogKayitSil_metodu_cagirildiginda_foxpro_baglantisi_acik_degilse_exception_atmali()
        {
            Assert.Throws<Exception>(() => _dbProxy.LogKayitSil(_pol12345TransactionInfo));
        }

        [Test]
        public void TransactionLogKaydinaHataMesajiYaz_metodu_cagirildiginda_foxpro_baglantisi_acik_degilse_exception_atmali()
        {
            Assert.Throws<Exception>(() => _dbProxy.LogKaydinaHataMesajiYaz(_pol12345TransactionInfo, new Exception()));
        }

        [Test]
        public void baglanti_acikken_tekrar_acilirsa_exception_atmali()
        {
            _dbProxy.BaglantilariAc();

            Assert.Throws<Exception>(() => _dbProxy.BaglantilariAc());
        }

        [Test]
        public void baglanti_kapali_iken_tekrar_kapatilmaya_calisilirsa_exception_atmali()
        {
            Assert.Throws<Exception>(() => _dbProxy.BaglantilariKapat());
        }
    }
}
