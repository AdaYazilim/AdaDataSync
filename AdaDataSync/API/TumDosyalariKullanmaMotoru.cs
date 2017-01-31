using System.Data;
using System.Data.OleDb;

namespace AdaDataSync.API
{
    public class TumDosyalariKullanmaMotoru:ITumDosyalariKullanmaMotoru
    {
        private readonly OleDbConnection _connection;
        private OleDbTransaction _transaction;

        public TumDosyalariKullanmaMotoru(OleDbConnection connection)
        {
            _connection = connection;
        }

        public void ButunDosyalariKullan()
        {
            if (_transaction != null)
            {
                return;
            }

            _connection.Open();
            _transaction = _connection.BeginTransaction();

            DataTable dtTablolar = _connection.GetSchema("Tables");

            // aşağıdaki koda gerek yok. _connection.GetSchema("Tables") komutu zaten connectionu açık tutuyor sürekli.

            //List<string> tablolar =
            //    dtTablolar
            //        .Rows
            //        .Cast<DataRow>()
            //        .Select(dr => dr[2].ToString())
            //        .ToList();

            //OleDbCommand command = _connection.CreateCommand();
            //command.Transaction = _transaction;

            //foreach (string tablo in tablolar)
            //{
            //    string dummyKomut = string.Format("delete from {0} where 1=2", tablo);
            //    command.CommandText = dummyKomut;
            //    command.ExecuteNonQuery();
            //}
        }
        
        public void ButunDosyalariSerbestBirak()
        {
            if (_transaction == null)
            {
                return;
            }

            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
            _connection.Close();
        }
    }
}
