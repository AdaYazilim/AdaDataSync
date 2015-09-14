using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AdaVeriKatmani;

namespace AdaDataSync.API
{
	public class DatabaseProxy:IDatabaseProxy
	{
        private readonly ITekConnectionVeriIslemleri _kaynakVeriIslemleri;
        private readonly ITekConnectionVeriIslemleri _hedefVeriIslemleri;
        private bool _kaynakBaglantiAcik;
        private bool _hedefBaglantiAcik;

        public DatabaseProxy(ITekConnectionVeriIslemleri kaynakVeriIslemleri, ITekConnectionVeriIslemleri hedefVeriIslemleri)
		{
            _kaynakVeriIslemleri = kaynakVeriIslemleri;
			_hedefVeriIslemleri = hedefVeriIslemleri;
            _kaynakBaglantiAcik = false;
            _hedefBaglantiAcik = false;
		}

	    public void BaglantilariAc()
	    {
	        if (_kaynakBaglantiAcik)
	            throw new Exception("Kaynak bağlantı açık iken tekrar açılamaz.");

            if (_hedefBaglantiAcik)
                throw new Exception("Hedef bağlantı açık iken tekrar açılamaz.");

	        _kaynakVeriIslemleri.BaglantiAc();
            _kaynakBaglantiAcik = true;

	        _hedefVeriIslemleri.BaglantiAc();
            _hedefBaglantiAcik = true;
	    }

	    public void BaglantilariKapat()
        {
            if (!_kaynakBaglantiAcik)
                throw new Exception("Kaynak bağlantı kapalı iken tekrar kapatılamaz.");

            if (!_hedefBaglantiAcik)
                throw new Exception("Hedef bağlantı kapalı iken tekrar kapatılamaz.");

            _kaynakVeriIslemleri.BaglantiKapat();
            _kaynakBaglantiAcik = false;

            _hedefVeriIslemleri.BaglantiKapat();
            _hedefBaglantiAcik = false;
        }

	    public List<DataTransactionInfo> BekleyenTransactionlariAl(int kayitSayisi)
		{
            if (!_kaynakBaglantiAcik)
                throw new Exception("Kaynak bağlantı açılmış olmalı.");

            DataTable dt = _kaynakVeriIslemleri.Doldur("select top " + kayitSayisi + " * from trlog order by oncekitur desc, fprktrlog2");

			return (from DataRow dr in dt.Rows 
                    select new DataTransactionInfo
                        (
                        (int)dr["fprktrlog2"],
                        dr["dosyaadi"].ToString().Trim().ToLowerInvariant(),
                        dr["prkalanadi"].ToString().Trim().ToLowerInvariant(),
                        (int)dr["prkdeger"],
                        dr["islemtipi"].ToString().Trim(),
                        dr["oncekitur"] != DBNull.Value && (bool)dr["oncekitur"]
                        )
                   ).ToList();
        }

		public Kayit KaynaktanTekKayitAl(DataTransactionInfo transactionInfo)
		{
            if (!_kaynakBaglantiAcik)
                throw new Exception("Kaynak bağlantı açılmış olmalı.");

		    string komut = "select * from " + transactionInfo.TabloAdi + " where " + transactionInfo.PrimaryKeyKolonAdi + "=" + transactionInfo.PrimaryKeyDegeri;
			DataTable dt = _kaynakVeriIslemleri.Doldur(komut);
			if (dt.Rows.Count == 0)
				return null;
            return new Kayit(dt.Rows[0].ItemArray);
        }

		public void HedeftenKayitSil(DataTransactionInfo transactionInfo)
		{
            if (!_hedefBaglantiAcik)
                throw new Exception("Hedef bağlantı açılmış olmalı.");

		    string silmeKomutu = "delete from " + transactionInfo.TabloAdi + " where " + transactionInfo.PrimaryKeyKolonAdi + " = :1";
		    _hedefVeriIslemleri.SorguDisi(silmeKomutu, transactionInfo.PrimaryKeyDegeri);
        }

	    public void HedefteInsertVeyaUpdate(Kayit kaynaktakiKayit, DataTransactionInfo transactionInfo)
		{
            if (!_hedefBaglantiAcik)
                throw new Exception("Hedef bağlantı açılmış olmalı.");

			_hedefVeriIslemleri.TekKayitGuncelle(transactionInfo.TabloAdi, transactionInfo.PrimaryKeyKolonAdi, transactionInfo.PrimaryKeyDegeri, kaynaktakiKayit.DataRowItemArray);
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

	    public void LogKaydiniSqleAktar(DataTransactionInfo transactionInfo)
	    {
            if (!_hedefBaglantiAcik)
                throw new Exception("Hedef bağlantı açılmış olmalı.");

	        const string insertKomutu = "insert into trlog (dosyaadi,prkalanadi,prkdeger,islemtipi,fprktrlog2,oncekitur,tarihsaat) values(:1,:2,:3,:4,:5,:6,:7)";
            try
            {
                _hedefVeriIslemleri.SorguDisi(insertKomutu, transactionInfo.TabloAdi, transactionInfo.PrimaryKeyKolonAdi, transactionInfo.PrimaryKeyDegeri, transactionInfo.IslemTipi, transactionInfo.PrkLog, transactionInfo.OncekiTur, DateTime.Now);
            }
            catch (Exception ex)
            {
                if (!ex.Message.StartsWith("Violation of PRIMARY KEY constraint "))
                    throw;
            }
	    }

	    public void LogKayitSil(DataTransactionInfo transactionLog)
		{
            if (!_kaynakBaglantiAcik)
                throw new Exception("Kaynak bağlantı açılmış olmalı.");

            string silmeKomutu = "delete from trlog where fprktrlog2 = " + transactionLog.PrkLog;
		    _kaynakVeriIslemleri.SorguDisi(silmeKomutu);
        }

	    public void LogKaydinaHataMesajiYaz(DataTransactionInfo transactionLog, Exception ex)
		{
            if (!_kaynakBaglantiAcik)
                throw new Exception("Kaynak bağlantı açılmış olmalı.");

            string hataMesaji = ex.Message;

		    if (hataMesaji.Length > 100)
		        hataMesaji = hataMesaji.Substring(0, 100);

            string updateKomut = "update trlog set hataacikla = :1 where fprktrlog2 = " + transactionLog.PrkLog;
		    _kaynakVeriIslemleri.SorguDisi(updateKomut, hataMesaji);
		}
	}
}