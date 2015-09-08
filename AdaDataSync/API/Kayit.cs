using System.Data;

namespace AdaDataSync.API
{
	public class Kayit
	{
		public DataRow DataRow { get; private set; }

		public Kayit(DataRow dataRow)
		{
			DataRow = dataRow;
		}
	}
}