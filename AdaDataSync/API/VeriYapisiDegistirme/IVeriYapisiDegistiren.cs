using System.Data;
using AdaPublicGenel.Cesitli;

namespace AdaDataSync.API.VeriYapisiDegistirme
{
    interface IVeriYapisiDegistiren
    {
        string KolonEklemeKomutunuAl(string tabloAdi, string kolonAdi, string kolonTipi);
        string KolonTipiDegistirmeKomutunuAl(string tabloAdi, string kolonAdi, string kolonTipi);
        string KolonSilmeKomutunuAl(string tabloAdi, string kolonAdi);
        string KolonTipiniAl(DataRow ilgiliKolonBilgisi, ref bool tablodaPrimaryKeyVar);
    }

    class MsSqlVeriYapisiDegistiren : IVeriYapisiDegistiren
    {
        public string KolonEklemeKomutunuAl(string tabloAdi, string kolonAdi, string kolonTipi)
        {
            return string.Format("alter table {0} add {1} {2}", tabloAdi, kolonAdi, kolonTipi);
        }

        public string KolonTipiDegistirmeKomutunuAl(string tabloAdi, string kolonAdi, string kolonTipi)
        {
            return string.Format("alter table {0} alter column {1} {2}", tabloAdi, kolonAdi, kolonTipi);
        }

        public string KolonSilmeKomutunuAl(string tabloAdi, string kolonAdi)
        {
            return string.Format("alter table {0} drop column {1}", tabloAdi, kolonAdi);
        }

        public string KolonTipiniAl(DataRow ilgiliKolonBilgisi, ref bool tablodaPrimaryKeyVar)
        {
            return FoxproAlanTipindenSqlAlanTipiYaratan.SqlKolonTipiniAl(ilgiliKolonBilgisi, false, ref tablodaPrimaryKeyVar);
        }
    }

    class MySqlVeriYapisiDegistiren : IVeriYapisiDegistiren
    {
        public string KolonEklemeKomutunuAl(string tabloAdi, string kolonAdi, string kolonTipi)
        {
            return string.Format("alter table {0} add {1} {2}", tabloAdi, kolonAdi, kolonTipi);
        }

        public string KolonTipiDegistirmeKomutunuAl(string tabloAdi, string kolonAdi, string kolonTipi)
        {
            return string.Format("alter table {0} change column {1} {1} {2}", tabloAdi, kolonAdi, kolonTipi);
        }

        public string KolonSilmeKomutunuAl(string tabloAdi, string kolonAdi)
        {
            return string.Format("alter table {0} drop column {1}", tabloAdi, kolonAdi);
        }

        public string KolonTipiniAl(DataRow ilgiliKolonBilgisi, ref bool tablodaPrimaryKeyVar)
        {
            return FoxproAlanTipindenSqlAlanTipiYaratan.MySqlKolonTipiniAl(ilgiliKolonBilgisi, ref tablodaPrimaryKeyVar);
        }
    }
}
