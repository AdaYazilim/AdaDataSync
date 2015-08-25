using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AdaVeriKatmani;

namespace AdaDataSync.API
{
    //[TestFixture]
    //public class DatabaseProxyTest
    //{
    //    private ITemelVeriIslemleri _kaynakVeriIslemleri;
    //    private ITemelVeriIslemleri _hedefVeriIslemleri;

    //    [Test]
    //    public void insert_veya_update_hedef_veri_islemlerinde_calistirilir()
    //    {
    //        _kaynakVeriIslemleri = Substitute.For<ITemelVeriIslemleri>();
    //        _hedefVeriIslemleri = Substitute.For<ITemelVeriIslemleri>();

    //        DatabaseProxy dbProxy = new DatabaseProxy(_kaynakVeriIslemleri, _hedefVeriIslemleri);

    //        Kayit kaynaktakiKayit = new Kayit(null);
    //        DataTransactionInfo transactionInfo = new DataTransactionInfo(null);
			
    //        dbProxy.HedefteInsertVeyaUpdate(kaynaktakiKayit, transactionInfo);

    //        _hedefVeriIslemleri.ReceivedWithAnyArgs().TekKayitGuncelle(transactionInfo.TabloAdi, transactionInfo.PrimaryKeyKolonAdi, transactionInfo.PrimaryKeyDegeri, kaynaktakiKayit.DataRow);

    //        //_hedefVeriIslemleri.Received().SelectTekKolondan(transactionInfo.TabloAdi, transactionInfo.PrimaryKeyKolonAdi, transactionInfo.PrimaryKeyDegeri);
    //        //_hedefVeriIslemleri.ReceivedWithAnyArgs().DataAdaptorUpdateCagir();

    //    }

    //}
	
	public class DatabaseProxy : IDatabaseProxy
	{
		private readonly ITemelVeriIslemleri _kaynakVeriIslemleri;
		private readonly ITemelVeriIslemleri _hedefVeriIslemleri;

		public DatabaseProxy(ITemelVeriIslemleri kaynakVeriIslemleri, ITemelVeriIslemleri hedefVeriIslemleri)
		{
			_kaynakVeriIslemleri = kaynakVeriIslemleri;
			_hedefVeriIslemleri = hedefVeriIslemleri;
		}

		public List<DataTransactionInfo> BekleyenTransactionlariAl(int kayitSayisi)
		{
            //DataTable dt = _kaynakVeriIslemleri.SelectTop("trlog", 10000);
		    DataTable dt = _kaynakVeriIslemleri.TransactionIciDoldur("select top " + kayitSayisi + " * from trlog order by oncekitur desc, fprktrlog2");

			return (from DataRow dr in dt.Rows 
                    select new DataTransactionInfo
                        (
                        (int)dr["fprktrlog2"],
                        dr["dosyaadi"].ToString().Trim().ToLowerInvariant(),
                        dr["prkalanadi"].ToString().Trim().ToLowerInvariant(),
                        (int)dr["prkdeger"]
                        )
                   ).ToList();
		}

		public Kayit KaynaktanTekKayitAl(DataTransactionInfo transactionInfo)
		{
		    string komut = "select * from " + transactionInfo.TabloAdi + " where " + transactionInfo.PrimaryKeyKolonAdi + "=" + transactionInfo.PrimaryKeyDegeri;
			DataTable dt = _kaynakVeriIslemleri.TransactionIciDoldur(komut);
			if (dt.Rows.Count == 0)
				return null;
			return new Kayit(dt.Rows[0]);
		}

		public void HedeftenKayitSil(DataTransactionInfo transactionInfo)
		{
            // delete fonksiyonu hatalı
            //_hedefVeriIslemleri.Delete(transactionInfo.TabloAdi, transactionInfo.PrimaryKeyKolonAdi, transactionInfo.PrimaryKeyDegeri);

		    string silmeKomutu = "delete from " + transactionInfo.TabloAdi + " where " + transactionInfo.PrimaryKeyKolonAdi + " = :1";
		    _hedefVeriIslemleri.TransactionIciSorguDisi(silmeKomutu, transactionInfo.PrimaryKeyDegeri);
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
            //_kaynakVeriIslemleri.Delete("trlog", "fprktrlog2", transactionLog.PrkLog);

		    string silmeKomutu = "delete from trlog where fprktrlog2 = " + transactionLog.PrkLog;
		    _kaynakVeriIslemleri.TransactionIciSorguDisi(silmeKomutu);
		}

		public void TransactionLogKaydinaHataMesajiYaz(DataTransactionInfo transactionLog, Exception ex)
		{
		    string hataMesaji= ex.Message;

		    if (hataMesaji.Length > 100)
		        hataMesaji = hataMesaji.Substring(0, 100);

            string updateKomut = "update trlog set hataacikla = :1 where fprktrlog2 = " + transactionLog.PrkLog;
		    _kaynakVeriIslemleri.TransactionIciSorguDisi(updateKomut, hataMesaji);
		}
	}
}