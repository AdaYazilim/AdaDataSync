using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using AdaDataSync.API;
using AdaVeriKatmani;

namespace AdaDataSync
{
    internal class Program
    {
        const string KaynakConfigString = "KaynakBaglantiString";
        const string HedefConfigString = "HedefBaglantiString";

        private static void Main()
        {
            IDataSyncYonetici[] yoneticiler = yoneticileriAl().ToArray();
            ICalisanServisKontrolcusu calisanServisKontrolcusu = new CalisanServisKontrolcusu();
            ProgramGenelServis genelServis = new ProgramGenelServis(calisanServisKontrolcusu, yoneticiler);
            genelServis.Calistir();
        }

        private static IEnumerable<IDataSyncYonetici> yoneticileriAl()
        {
            for (int i = 1; ; i++)
            {
                string kaynakConfig = KaynakConfigString + i;
                string hedefConfig = HedefConfigString + i;

                string kaynakBaglanti = ConfigurationManager.AppSettings[kaynakConfig];
                string hedefBaglanti = ConfigurationManager.AppSettings[hedefConfig];

                if (string.IsNullOrWhiteSpace(kaynakBaglanti) && string.IsNullOrWhiteSpace(hedefBaglanti))
                    break;

                if (string.IsNullOrWhiteSpace(kaynakBaglanti) || string.IsNullOrWhiteSpace(hedefBaglanti))
                    continue;

                kaynakBaglanti = kaynakBaglanti.Trim();
                hedefBaglanti = hedefBaglanti.Trim();

                int logDosyaNo = i;

                IGuncellemeKontrol guncellemeKontrol = new FoxproGuncellemeKontrol(kaynakBaglanti);
                IDataSyncService syncServis = syncServisAl(kaynakBaglanti, hedefBaglanti, logDosyaNo);
                ILogger safetyNetLogger = new TextDosyasiLogger(string.Format("hata_{0}.txt", i));
                DataSyncYonetici dataSyncYonetici = new DataSyncYonetici(guncellemeKontrol, syncServis, safetyNetLogger);
                dataSyncYonetici.KritikHataAtti += () => syncServisAl(kaynakBaglanti, hedefBaglanti, logDosyaNo);

                yield return dataSyncYonetici;
            }

        }

        private static IDataSyncService syncServisAl(string kaynakBaglanti, string hedefBaglanti, int logDosyaNo)
        {
            OleDbConnection foxproConnection = new OleDbConnection(kaynakBaglanti);
            SqlConnection sqlConnection = new SqlConnection(hedefBaglanti);

            IAktarimScope aktarimScope = aktarimScopeHazirla();

            IVeritabaniIslemYapan hedefVeritabaniGuncelleyen = new HedefVeritabaniGuncelleyen(foxproConnection, sqlConnection, aktarimScope);
            ITekConnectionVeriIslemleri tviKaynak = new TemelVeriIslemleri(VeritabaniTipi.FoxPro, kaynakBaglanti);
            ITekConnectionVeriIslemleri tviHedef = new TemelVeriIslemleri(VeritabaniTipi.SqlServer, hedefBaglanti);
            ILogger logger = new TextDosyasiLogger("log_" + logDosyaNo + ".txt");
            IDatabaseProxy dp = new DatabaseProxy(tviKaynak, tviHedef, logger);

            IVeritabaniIslemYapan veriAktaran = new VeriAktaran(dp, aktarimScope);
            IDataSyncService retVal = new DataSyncService(hedefVeritabaniGuncelleyen, veriAktaran);
            return retVal;
        }

        private static IAktarimScope aktarimScopeHazirla()
        {
            string aktarimScopeTipi = ConfigurationManager.AppSettings["AktarimScopeTipi"] ?? string.Empty;
            string aktarimScopeTablolar = ConfigurationManager.AppSettings["AktarimScopeTablolar"] ?? string.Empty;

            switch (aktarimScopeTipi)
            {
                case "2":
                    return new DahilTablolarAktarimScope(aktarimScopeTablolar);
                case "3":
                    return new HaricTablolarAktarimScope(aktarimScopeTablolar);
                default:
                    return new ButunTablolarAktarimScope();
            }
        }
    }
}