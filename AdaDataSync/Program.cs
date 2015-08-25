using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using AdaDataSync.API;
using AdaDataSync.Test;
using AdaVeriKatmani;

namespace AdaDataSync
{
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        //
    //        string foxproString = ConfigurationManager.AppSettings["VfpBaglantiString"];
    //        string sqlString = ConfigurationManager.AppSettings["SqlBaglantiString"];
    //        TemelVeriIslemleri tviKaynak = new TemelVeriIslemleri(VeritabaniTipi.FoxPro, foxproString);
    //        TemelVeriIslemleri tviHedef = new TemelVeriIslemleri(VeritabaniTipi.SqlServer, sqlString);

    //        DatabaseProxy dp= new DatabaseProxy(tviKaynak,tviHedef);
    //        CalisanServisKontrolcusu csk = new CalisanServisKontrolcusu();
    //        SafetyNetLogger sl = new SafetyNetLogger();
    //        DataSyncService sync = new DataSyncService(dp,csk,sl);

    //        while (true)
    //        {
    //            sync.Sync();

    //            System.Threading.Thread.Sleep(5000);
    //        }
    //    }
    //}

    internal class Program
    {
        private static void Main(string[] args)
        {
            SafetyNetLogger safetyLogger = new SafetyNetLogger();
            CalisanServisKontrolcusu calisanServisKontrolcusu = new CalisanServisKontrolcusu();

            const string kaynakConfigString = "KaynakBaglantiString";
            const string hedefConfigString = "HedefBaglantiString";

            while (true)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                for (int i = 1;; i++)
                {
                    if (calisanServisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu())
                        continue;

                    calisanServisKontrolcusu.MakinaBazindaKilitKoy();

                    string kaynakConfig = kaynakConfigString + i;
                    string hedefConfig = hedefConfigString + i;

                    string kaynakBaglanti = ConfigurationManager.AppSettings[kaynakConfig].Trim();
                    string hedefBaglanti = ConfigurationManager.AppSettings[hedefConfig].Trim();

                    if (string.IsNullOrWhiteSpace(kaynakBaglanti) && string.IsNullOrWhiteSpace(hedefBaglanti))
                        break;

                    if (string.IsNullOrWhiteSpace(kaynakBaglanti) || string.IsNullOrWhiteSpace(hedefBaglanti))
                        continue;

                    try
                    {
                        syncYap(kaynakBaglanti, hedefBaglanti);
                    }
                    catch (Exception ex)
                    {
                        //string hataMesaji = "İlk veritabanları ile ilgili hata oluştu. HataMesajı: " + ex.Message;
                        safetyLogger.HataLogla(ex);
                    }
                }

                sw.Stop();
                if (sw.ElapsedMilliseconds < 5000)
                    Thread.Sleep(5000 - (int)sw.ElapsedMilliseconds);
            }
        }

        private static void syncYap(string kaynakBaglanti, string hedefBaglanti)
        {
            TemelVeriIslemleri tviKaynak = new TemelVeriIslemleri(VeritabaniTipi.FoxPro, kaynakBaglanti);
            TemelVeriIslemleri tviHedef = new TemelVeriIslemleri(VeritabaniTipi.SqlServer, hedefBaglanti);

            DatabaseProxy dp = new DatabaseProxy(tviKaynak, tviHedef);
            DataSyncService sync = new DataSyncService(dp);
            sync.Sync();
        }
    }
}