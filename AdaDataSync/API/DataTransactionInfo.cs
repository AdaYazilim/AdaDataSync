using System.Data;

namespace AdaDataSync.API
{
    //public class DataTransactionInfo
    //{
    //    public static DataTransactionInfo Yarat(DataRow dr)
    //    {
    //        return new DataTransactionInfo();
    //    }

    //    public string TabloAdi { get; set; }
    //    public string PrimaryKeyKolonAdi { get; set; }
    //    public object PrimaryKeyDegeri { get; set; }
    //}

    public class DataTransactionInfo
    {
        public readonly int PrkLog;
        public readonly string TabloAdi;
        public readonly string PrimaryKeyKolonAdi;
        public readonly int PrimaryKeyDegeri;

        public DataTransactionInfo(DataRow dr)
        {
            PrkLog = (int)dr["fprktrlog2"];
            TabloAdi = dr["dosyaadi"].ToString().Trim().ToLowerInvariant();
            PrimaryKeyKolonAdi = dr["prkalanadi"].ToString().Trim().ToLowerInvariant();
            PrimaryKeyDegeri = (int)dr["prkdeger"];
        }
    }
}