using System.Collections.Generic;
using System.Configuration;
using AdaDataSync.API;
using AdaVeriKatmani;

namespace AdaDataSync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ICalisanServisKontrolcusu calisanServisKontrolcusu = new CalisanServisKontrolcusu();
            ISafetyNetLogger safetyNetLogger = new SafetyNetLogger();
            List<IDataSyncService> syncServisler = new List<IDataSyncService>();

            const string kaynakConfigString = "KaynakBaglantiString";
            const string hedefConfigString = "HedefBaglantiString";

            for (int i = 0; ; i++)
            {
                string kaynakConfig = kaynakConfigString + i;
                string hedefConfig = hedefConfigString + i;

                string kaynakBaglanti = ConfigurationManager.AppSettings[kaynakConfig].Trim();
                string hedefBaglanti = ConfigurationManager.AppSettings[hedefConfig].Trim();

                if (string.IsNullOrWhiteSpace(kaynakBaglanti) && string.IsNullOrWhiteSpace(hedefBaglanti))
                    break;

                if (string.IsNullOrWhiteSpace(kaynakBaglanti) || string.IsNullOrWhiteSpace(hedefBaglanti))
                    continue;

                ITekConnectionVeriIslemleri tviKaynak = new TemelVeriIslemleri(VeritabaniTipi.FoxPro, kaynakBaglanti);
                ITekConnectionVeriIslemleri tviHedef = new TemelVeriIslemleri(VeritabaniTipi.SqlServer, hedefBaglanti);

                DatabaseProxy dp = new DatabaseProxy(tviKaynak, tviHedef);
                DataSyncService syncServis = new DataSyncService(dp);
                syncServisler.Add(syncServis);
            }

            ProgramGenelServis genelServis = new ProgramGenelServis(calisanServisKontrolcusu, safetyNetLogger, syncServisler.ToArray());
            genelServis.Calistir();
        }
    }
}