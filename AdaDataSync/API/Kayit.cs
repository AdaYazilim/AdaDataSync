namespace AdaDataSync.API
{
	public class Kayit
	{
		public object[] DataRowItemArray { get; private set; }

		public Kayit(object[] dataRowItemArray)
		{
			DataRowItemArray = dataRowItemArray;
		}
	}
}