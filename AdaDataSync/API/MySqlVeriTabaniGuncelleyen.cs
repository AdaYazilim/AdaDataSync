using System.Data.Common;
using System.Data.OleDb;
using MySql.Data.MySqlClient;

namespace AdaDataSync.API
{
    internal class MySqlVeriTabaniGuncelleyen : HedefVeritabaniGuncelleyen
    {
        internal MySqlVeriTabaniGuncelleyen(OleDbConnection foxproConnection, MySqlConnection sqlConnection, IAktarimScope aktarimScope)
            : base(foxproConnection, sqlConnection, aktarimScope)
        {
        }

        protected override DbDataAdapter AdapterAl()
        {
            return new MySqlDataAdapter();
        }
    }
}