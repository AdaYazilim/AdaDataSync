using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaDataSync.API;
using AdaDataSync.Test;
using AdaVeriKatmani;

namespace AdaDataSync
{
	class Program
	{
		static void Main(string[] args)
		{
            //
            string foxproString = ConfigurationManager.AppSettings["VfpBaglantiString"];
			string sqlString = ConfigurationManager.AppSettings["SqlBaglantiString"];
			TemelVeriIslemleri tviKaynak = new TemelVeriIslemleri(VeritabaniTipi.FoxPro, foxproString);
			TemelVeriIslemleri tviHedef = new TemelVeriIslemleri(VeritabaniTipi.SqlServer, sqlString);

			DatabaseProxy dp= new DatabaseProxy(tviKaynak,tviHedef);
			CalisanServisKontrolcusu csk = new CalisanServisKontrolcusu();
			SafetyNetLogger sl = new SafetyNetLogger();
			DataSyncService sync = new DataSyncService(dp,csk,sl);
			
			while (true)
			{
				sync.Sync();

				System.Threading.Thread.Sleep(5000);
			}
		}
	}

	public class CalisanServisKontrolcusu:ICalisanServisKontrolcusu
	{
		public bool BuMakinadaBaskaServisCalisiyorMu()
		{
			return false;
		}

		public void MakinaBazindaKilitKoy()
		{
			
		}
	}
}
