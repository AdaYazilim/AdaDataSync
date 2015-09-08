namespace AdaDataSync.API
{
    public class DataTransactionInfo
    {
        public readonly int PrkLog;
        public readonly string TabloAdi;
        public readonly string PrimaryKeyKolonAdi;
        public readonly int PrimaryKeyDegeri;

        public DataTransactionInfo(int prkLog, string tabloAdi, string primaryKeyKolonAdi, int primaryKeyDegeri)
        {
            PrkLog = prkLog;
            TabloAdi = tabloAdi;
            PrimaryKeyKolonAdi = primaryKeyKolonAdi;
            PrimaryKeyDegeri = primaryKeyDegeri;
        }
    }
}