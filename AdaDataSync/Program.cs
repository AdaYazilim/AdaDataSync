using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using AdaDataSync.API;
using AdaVeriKatmani;

namespace AdaDataSync
{
    internal class Program
    {
        private static void Main()
        {
            ICalisanServisKontrolcusu calisanServisKontrolcusu = new CalisanServisKontrolcusu();
            ISafetyNetLogger safetyNetLogger = new SafetyNetLogger();
            List<IDataSyncService> syncServisler = new List<IDataSyncService>();

            const string kaynakConfigString = "KaynakBaglantiString";
            const string hedefConfigString = "HedefBaglantiString";

            for (int i = 1; ; i++)
            {
                string kaynakConfig = kaynakConfigString + i;
                string hedefConfig = hedefConfigString + i;

                string kaynakBaglanti = ConfigurationManager.AppSettings[kaynakConfig];
                string hedefBaglanti = ConfigurationManager.AppSettings[hedefConfig];

                if (string.IsNullOrWhiteSpace(kaynakBaglanti) && string.IsNullOrWhiteSpace(hedefBaglanti))
                    break;

                if (string.IsNullOrWhiteSpace(kaynakBaglanti) || string.IsNullOrWhiteSpace(hedefBaglanti))
                    continue;

                kaynakBaglanti = kaynakBaglanti.Trim();
                hedefBaglanti = hedefBaglanti.Trim();

                ITekConnectionVeriIslemleri tviKaynak = new TemelVeriIslemleri(VeritabaniTipi.FoxPro, kaynakBaglanti);
                ITekConnectionVeriIslemleri tviHedef = new TemelVeriIslemleri(VeritabaniTipi.SqlServer, hedefBaglanti);
                IDatabaseProxy dp = new DatabaseProxy(tviKaynak, tviHedef);
                IVeritabaniIslemYapan veriAktaran = new VeriAktaran(dp);
                
                IGuncellemeKontrol guncellemeKontrol = new FoxproGuncellemeKontrol(kaynakBaglanti);

                OleDbConnection foxproConnection = new OleDbConnection(kaynakBaglanti);
                SqlConnection sqlConnection = new SqlConnection(hedefBaglanti);
                IVeritabaniIslemYapan hedefVeritabaniGuncelleyen = new HedefVeritabaniGuncelleyen(foxproConnection, sqlConnection);

                IDataSyncService syncServis = new DataSyncService(guncellemeKontrol, hedefVeritabaniGuncelleyen, veriAktaran);
                syncServisler.Add(syncServis);
            }

            ProgramGenelServis genelServis = new ProgramGenelServis(calisanServisKontrolcusu, safetyNetLogger, syncServisler.ToArray());
            genelServis.Calistir();
        }
    }
}