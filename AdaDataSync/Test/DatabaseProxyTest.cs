using AdaDataSync.API;
using AdaVeriKatmani;
using NSubstitute;
using NUnit.Framework;

namespace AdaDataSync.Test
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

            Kayit kaynaktakiKayit = new Kayit(null);
            DataTransactionInfo transactionInfo = new DataTransactionInfo(7, "pol", "fprkpol", 12345);

            dbProxy.HedefteInsertVeyaUpdate(kaynaktakiKayit, transactionInfo);

            _hedefVeriIslemleri.ReceivedWithAnyArgs().TekKayitGuncelle(transactionInfo.TabloAdi, transactionInfo.PrimaryKeyKolonAdi, transactionInfo.PrimaryKeyDegeri, kaynaktakiKayit.DataRow);

            //_hedefVeriIslemleri.Received().SelectTekKolondan(transactionInfo.TabloAdi, transactionInfo.PrimaryKeyKolonAdi, transactionInfo.PrimaryKeyDegeri);
            //_hedefVeriIslemleri.ReceivedWithAnyArgs().DataAdaptorUpdateCagir();

        }

    }
}
