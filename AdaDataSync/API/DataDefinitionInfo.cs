namespace AdaDataSync.API
{
    public class DataDefinitionInfo
    {
        public readonly int FprkDdLog;
        public readonly string TabloAdi;
        public readonly string DegisenAlanAdi;

        public DataDefinitionInfo(int fprkDdLog, string tabloAdi, string degisenAlanAdi)
        {
            FprkDdLog = fprkDdLog;
            TabloAdi = tabloAdi;
            DegisenAlanAdi = degisenAlanAdi;
        }
    }
}