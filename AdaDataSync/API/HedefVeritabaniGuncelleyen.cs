using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using AdaPublicGenel.Extensions;

namespace AdaDataSync.API
{
    internal class HedefVeritabaniGuncelleyen : IVeritabaniIslemYapan
    {
        private readonly OleDbConnection _foxproConnection;
        private readonly SqlConnection _sqlConnection;

        public HedefVeritabaniGuncelleyen(OleDbConnection foxproConnection, SqlConnection sqlConnection)
        {
            _foxproConnection = foxproConnection;
            _sqlConnection = sqlConnection;
        }
		//
        public void VeritabaniIslemiYap()
        {
            const string selectKomut = "select * from ddlog";
            DataTable dt = new DataTable();
            new OleDbDataAdapter(selectKomut, _foxproConnection).Fill(dt);

            if (dt.Rows.Count == 0)
                return; // gereksiz bağlantı açıp kapatmasın

            try
            {
                baglantilariAc();

                DataTable fpKolonlar = _foxproConnection.GetSchema("Columns");

                var ddiler = dt.Rows.Cast<DataRow>()
                    .Select(dr =>
                        new DataDefinitionInfo(
                            (int) dr["fprkddlog"],
                            dr["dosyaadi"].ToString().ToLowerInvariant().Trim(),
                            dr["alanAdi"].ToString().ToLowerInvariant().Trim()))
                    .GroupBy(ddi => ddi.TabloAdi);


                foreach (IGrouping<string, DataDefinitionInfo> g in ddiler)
                {
                    string tabloAdi = g.Key;

                    IEnumerable<DataDefinitionInfo> tabloDdiler = g;
                    DataRow[] tabloKolonlari = fpKolonlar.Select("table_name='" + tabloAdi + "'");
                    tablonunIslemleriniYap(tabloAdi, tabloDdiler, tabloKolonlari);
                }
            }
            finally
            {
                baglantilariKapat();
            }
        }

        private void baglantilariAc()
        {
            _foxproConnection.Open();
            _sqlConnection.Open();
        }

        private void baglantilariKapat()
        {
            _foxproConnection.Close();
            _sqlConnection.Close();
        }

        private void tablonunIslemleriniYap(string tabloAdi, IEnumerable<DataDefinitionInfo> ddiler, DataRow[] kaynakTabloKolonlari)
        {
            string[] hedefKolonlar = null;
            foreach (DataDefinitionInfo ddi in ddiler)
            {
                // trlog'un ddi'sinin sql'e aktarılmasına gerek yok. Çünkü yine bu program sql'deki trlog'a işini bitirdiklerini log atıyor. 
                // sql'deki trlog'da burası sebepli otomatik bir structure değişikliği olursa oraya kayıt atan class otomatik değişmeyeceği için 
                // hata oluşur. Foxpro'daki trlog değişirse elle müdahale gerekecek.
                // w_exists_tbl ise structure olarak aktarılmalı.
                if (ddi.TabloAdi.ToLowerInvariant() != "ddlog" && ddi.TabloAdi.ToLowerInvariant() != "trlog")
                {
                    if (string.IsNullOrWhiteSpace(ddi.DegisenAlanAdi))
                    {
                        tabloyuTumuyleGuncelle(tabloAdi, kaynakTabloKolonlari);
                    }
                    else
                    {
                        if (hedefKolonlar==null)
                        {
                            SqlCommand command = _sqlConnection.CreateCommand();
                            command.CommandText = "select * from " + tabloAdi + " where 1=2";
                            DataTable dtHedefKolonlar = new DataTable();
                            new SqlDataAdapter(command).Fill(dtHedefKolonlar);
                            hedefKolonlar = dtHedefKolonlar.Columns.Cast<DataColumn>().Select(dc => dc.ColumnName.ToLowerInvariant()).ToArray();
                        }
                        tabloAlaniniGuncelle(ddi, kaynakTabloKolonlari, hedefKolonlar);
                    }
                }

                ddlogKaydiniSil(ddi);
            }
        }

        private void tabloyuTumuyleGuncelle(string tabloAdi, DataRow[] tabloKolonlari)
        {
            string kolonlarKomutString = "";
            bool primaryKeyEklendi = false;
            foreach (DataRow drKolon in tabloKolonlari)
            {
                string kolonAdi = drKolon["column_name"].ToString().Trim();
                kolonlarKomutString += ",[" + kolonAdi + "] " + kolonTipiniAl(drKolon, ref primaryKeyEklendi);
            }

            kolonlarKomutString = kolonlarKomutString.Substring(1);
            string komut = "create table " + tabloAdi + " (" + kolonlarKomutString + ");";
            SqlCommand command = _sqlConnection.CreateCommand();
            command.CommandText = komut;
            command.ExecuteNonQuery();
        }

        private void tabloAlaniniGuncelle(DataDefinitionInfo ddi, DataRow[] kaynakTabloKolonlari, string[] hedefKolonlar)
        {
            DataRow ilgiliKolonBilgisi = kaynakTabloKolonlari.SingleOrDefault(dr => string.Equals(dr["column_name"].ToString().Trim(), ddi.DegisenAlanAdi, StringComparison.InvariantCultureIgnoreCase));

            string komut;
            if (ilgiliKolonBilgisi == null) // Kolon kaynak tabloda yok. Yani silinmiş.
            {
                komut = "alter table " + ddi.TabloAdi + " drop column " + ddi.DegisenAlanAdi;
            }
            else
            {
                bool tablodaPrimaryKeyVar = true;
                string kolonTipi = kolonTipiniAl(ilgiliKolonBilgisi, ref tablodaPrimaryKeyVar);

                if(hedefKolonlar.Contains(ddi.DegisenAlanAdi.ToLowerInvariant()))   // kolon zaten varsa
                    komut = "alter table " + ddi.TabloAdi + " alter column " + ddi.DegisenAlanAdi + " " + kolonTipi;
                else
                    komut = "alter table " + ddi.TabloAdi + " add " + ddi.DegisenAlanAdi + " " + kolonTipi;
            }

            SqlCommand command = _sqlConnection.CreateCommand();
            command.CommandText = komut;
            command.ExecuteNonQuery();
        }

        private void ddlogKaydiniSil(DataDefinitionInfo ddi)
        {
            string silmeKomutu = "delete from ddlog where fprkddlog = " + ddi.FprkDdLog;
            OleDbCommand command = _foxproConnection.CreateCommand();
            command.CommandText = silmeKomutu;
            command.ExecuteNonQuery();
        }

        private string kolonTipiniAl(DataRow drKolon, ref bool tablodaPrimaryKeyVar)
        {
            string retVal;

            if (drKolon["DATA_TYPE"].ToString() == "3") //int
            {
                bool columnHasDefault = (bool)drKolon["COLUMN_HASDEFAULT"];
                if (columnHasDefault && !tablodaPrimaryKeyVar)
                //if (columnHasDefault)
                {
                    tablodaPrimaryKeyVar = true;
                    retVal = "[int] PRIMARY KEY";
                }
                else
                {
                    retVal = "[int]";
                }
            }
            else if (drKolon["DATA_TYPE"].ToString() == "129" && drKolon["CHARACTER_MAXIMUM_LENGTH"].ToInt() < 5000) //string
                retVal = "[varchar] (" + drKolon["CHARACTER_MAXIMUM_LENGTH"] + ")";
            else if (drKolon["DATA_TYPE"].ToString() == "129") //string
                retVal = "[text]";
            else if (drKolon["DATA_TYPE"].ToString() == "5") //double  todo: burası problemli
            {
                //int prec = Araclar.ParseInt(drKolon["NUMERIC_PRECISION"].ToString());
                //int scal = Araclar.ParseInt(drKolon["NUMERIC_SCALE"].ToString());
                //if (scal == 0)
                //{
                //    prec += 3;
                //    scal = 2;
                //}
                //return "[decimal] (" + prec + "," + scal + ") default 0";

                retVal = "[float]";
            }
            //return "[decimal] (18,2) default 0";
            else if (drKolon["DATA_TYPE"].ToString() == "131") //double
            {

                int prec = drKolon["NUMERIC_PRECISION"].ToInt();
                int scal = drKolon["NUMERIC_SCALE"].ToInt();

                prec += scal + 1;  // sebebini bilmiyorum. Buraya olduğundan eksik geliyor.

                retVal = "[decimal] (" + prec + "," + scal + ") default 0";
            }
            else if (drKolon["DATA_TYPE"].ToString() == "11") //bool
            {
                //return "[int]";
                retVal = "[bit]";
            }
            else if (drKolon["DATA_TYPE"].ToString() == "133") //tarih
                retVal = "[date]";
            else if (drKolon["DATA_TYPE"].ToString() == "135") //tarih saat
                retVal = "[datetime2]";
            else if (drKolon["DATA_TYPE"].ToString() == "128") //varbinary
            {
                if (drKolon["CHARACTER_MAXIMUM_LENGTH"].ToInt() < 1500)
                    retVal = "[varbinary] (" + drKolon["CHARACTER_MAXIMUM_LENGTH"] + ")";
                else
                    retVal = "[varbinary] (4000)";
            }
            else
                retVal= "[nvarchar](max)";

            //bool nullable = (bool)drKolon["is_nullable"];
            //string nullableEklentisi = nullable ? " NULL" : " NOT NULL";

            //retVal += nullableEklentisi;
            return retVal;
        }
    }
}