using System.Data;

namespace AdaDataSync.API
{
	public class DataTransactionInfo
	{
		public static DataTransactionInfo Yarat(DataRow dr)
		{
			return new DataTransactionInfo();
		}

		public string TabloAdi { get; set; }
		public string PrimaryKeyKolonAdi { get; set; }
		public object PrimaryKeyDegeri { get; set; }
	}
}