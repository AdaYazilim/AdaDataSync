using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using AdaPublicGenel.Cesitli;

namespace AdaDataSync.API
{
    internal abstract class HedefVeritabaniGuncelleyen : IVeritabaniGuncelleyen
    {
        private readonly OleDbConnection _foxproConnection;
        private readonly IAktarimScope _aktarimScope;
        private readonly DbConnection _sqlConnection;

        protected HedefVeritabaniGuncelleyen(OleDbConnection foxproConnection, DbConnection sqlConnection, IAktarimScope aktarimScope)
        {
            _foxproConnection = foxproConnection;
            _aktarimScope = aktarimScope;
            _sqlConnection = sqlConnection;
        }

        protected abstract DbDataAdapter AdapterAl();
		
        public void Guncelle()
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
                            dr["dosyaadi"].ToString(),
                            dr["alanAdi"].ToString()))
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
            // trlog'un ddi'sinin sql'e aktarılmasına gerek yok. Çünkü yine bu program sql'deki trlog'a işini bitirdiklerini log atıyor. 
            // sql'deki trlog'da burası sebepli otomatik bir structure değişikliği olursa oraya kayıt atan class otomatik değişmeyeceği için 
            // hata oluşur. Foxpro'daki trlog değişirse elle müdahale gerekecek.
            // w_exists_tbl ise structure olarak aktarılmalı.
            if (tabloAdi == "ddlog" || tabloAdi == "trlog")
            {
                // güncellemeye gerek yok. ddlog kayıtlarını sil.
                ddlogKayitlariniSil(ddiler);
                return;
            }

            if (!_aktarimScope.TabloAktarimaDahil(tabloAdi))
            {
                ddlogKayitlariniSil(ddiler);
                return;
            }

            if (!hedefteTabloVarMi(tabloAdi))
            {
                tabloyuTumuyleYenidenYarat(tabloAdi, kaynakTabloKolonlari);
                ddlogKayitlariniSil(ddiler);
                return;
            }

            foreach (DataDefinitionInfo ddi in ddiler)
            {
                tabloGuncellemesiniYap(tabloAdi, kaynakTabloKolonlari, ddi);
                ddlogKaydiniSil(ddi);
            }
        }

        private bool hedefteTabloVarMi(string tabloAdi)
        {
            DbCommand command = _sqlConnection.CreateCommand();
            command.CommandText = "select * from " + tabloAdi + " where 1=2";
            DataTable dt = new DataTable();
            try
            {
                DbDataAdapter adapter = AdapterAl();
                adapter.SelectCommand = command;
                adapter.Fill(dt);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void tabloGuncellemesiniYap(string tabloAdi, DataRow[] kaynakTabloKolonlari, DataDefinitionInfo ddi)
        {
            if (string.IsNullOrWhiteSpace(ddi.DegisenAlanAdi))
                return;

            DbCommand command = _sqlConnection.CreateCommand();
            command.CommandText = "select * from " + tabloAdi + " where 1=2";
            DataTable dtHedefKolonlar = new DataTable();
            DbDataAdapter adapter = AdapterAl();
            adapter.SelectCommand = command;
            adapter.Fill(dtHedefKolonlar);
            string[] hedefKolonlar = dtHedefKolonlar.Columns.Cast<DataColumn>().Select(dc => dc.ColumnName.ToLowerInvariant()).ToArray();
            tabloAlaniniGuncelle(ddi, kaynakTabloKolonlari, hedefKolonlar);
        }

        private void tabloyuTumuyleYenidenYarat(string tabloAdi, DataRow[] tabloKolonlari)
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
            DbCommand command = _sqlConnection.CreateCommand();
            command.CommandText = komut;
            command.ExecuteNonQuery();
        }

        private void tabloAlaniniGuncelle(DataDefinitionInfo ddi, DataRow[] kaynakTabloKolonlari, string[] hedefKolonlar)
        {
            DataRow ilgiliKolonBilgisi = kaynakTabloKolonlari.SingleOrDefault(dr => string.Equals(dr["column_name"].ToString().Trim(), ddi.DegisenAlanAdi, StringComparison.InvariantCultureIgnoreCase));

            string komut;
            if (ilgiliKolonBilgisi == null) // Kolon kaynak tabloda yok. Yani silinmiş.
            {
                if (!hedefKolonlar.Contains(ddi.DegisenAlanAdi.ToLowerInvariant())) // kolon zaten yoksa birşey yapma
                {
                    return;
                }

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

            DbCommand command = _sqlConnection.CreateCommand();
            command.CommandTimeout = 180;
            command.CommandText = komut;
            command.ExecuteNonQuery();
        }

        private void ddlogKayitlariniSil(IEnumerable<DataDefinitionInfo> ddiler)
        {
            foreach (DataDefinitionInfo ddi in ddiler)
            {
                ddlogKaydiniSil(ddi);
            }
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
            // buradaki kodu adapublice aldım. Toplu aktarım programında da kullanılıyor.
            return FoxproAlanTipindenSqlAlanTipiYaratan.SqlKolonTipiniAl(drKolon, false, ref tablodaPrimaryKeyVar);
        }
    }
}