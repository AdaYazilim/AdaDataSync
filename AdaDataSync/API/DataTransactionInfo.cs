namespace AdaDataSync.API
{
    public class DataTransactionInfo
    {
        public readonly int PrkLog;
        public readonly string TabloAdi;
        public readonly string PrimaryKeyKolonAdi;
        public readonly int PrimaryKeyDegeri;
        public readonly string IslemTipi;
        public readonly bool OncekiTur;

        public DataTransactionInfo(int prkLog, string tabloAdi, string primaryKeyKolonAdi, int primaryKeyDegeri, string islemTipi, bool oncekiTur)
        {
            PrkLog = prkLog;
            TabloAdi = tabloAdi.Trim().ToLowerInvariant();
            PrimaryKeyKolonAdi = primaryKeyKolonAdi.Trim().ToLowerInvariant();
            PrimaryKeyDegeri = primaryKeyDegeri;
            IslemTipi = islemTipi.Trim();
            OncekiTur = oncekiTur;
        }
    }
}