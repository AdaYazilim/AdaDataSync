using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AdaVeriKatmani;

namespace AdaDataSync.API
{
	public class DatabaseProxy : IDatabaseProxy
	{
        private readonly ITekConnectionVeriIslemleri _kaynakVeriIslemleri;
        private readonly ITekConnectionVeriIslemleri _hedefVeriIslemleri;

        public DatabaseProxy(ITekConnectionVeriIslemleri kaynakVeriIslemleri, ITekConnectionVeriIslemleri hedefVeriIslemleri)
		{
            _kaynakVeriIslemleri = kaynakVeriIslemleri;
			_hedefVeriIslemleri = hedefVeriIslemleri;
		}

		public List<DataTransactionInfo> BekleyenTransactionlariAl(int kayitSayisi)
		{
		    DataTable dt = _kaynakVeriIslemleri.Doldur("select top " + kayitSayisi + " * from trlog order by oncekitur desc, fprktrlog2");

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
			DataTable dt = _kaynakVeriIslemleri.Doldur(komut);
			if (dt.Rows.Count == 0)
				return null;
            return new Kayit(dt.Rows[0]);
		}

		public void HedeftenKayitSil(DataTransactionInfo transactionInfo)
		{
		    string silmeKomutu = "delete from " + transactionInfo.TabloAdi + " where " + transactionInfo.PrimaryKeyKolonAdi + " = :1";
		    _hedefVeriIslemleri.SorguDisi(silmeKomutu, transactionInfo.PrimaryKeyDegeri);
		}

		public void HedefteInsertVeyaUpdate(Kayit kaynaktakiKayit, DataTransactionInfo transactionInfo)
		{
			_hedefVeriIslemleri.TekKayitGuncelle(transactionInfo.TabloAdi, transactionInfo.PrimaryKeyKolonAdi, transactionInfo.PrimaryKeyDegeri, kaynaktakiKayit.DataRow.ItemArray);
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
		    string silmeKomutu = "delete from trlog where fprktrlog2 = " + transactionLog.PrkLog;
		    _kaynakVeriIslemleri.SorguDisi(silmeKomutu);
		}

		public void TransactionLogKaydinaHataMesajiYaz(DataTransactionInfo transactionLog, Exception ex)
		{
		    string hataMesaji= ex.Message;

		    if (hataMesaji.Length > 100)
		        hataMesaji = hataMesaji.Substring(0, 100);

            string updateKomut = "update trlog set hataacikla = :1 where fprktrlog2 = " + transactionLog.PrkLog;
		    _kaynakVeriIslemleri.SorguDisi(updateKomut, hataMesaji);
		}
	}
}