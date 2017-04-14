using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace AdaDataSync.API
{
    internal class MsSqlVeriTabaniGuncelleyen : HedefVeritabaniGuncelleyen
    {
        internal MsSqlVeriTabaniGuncelleyen(OleDbConnection foxproConnection, SqlConnection sqlConnection, IAktarimScope aktarimScope)
            :base(foxproConnection, sqlConnection, aktarimScope)
        {
        }

        protected override DbDataAdapter AdapterAl()
        {
            return new SqlDataAdapter();
        }
    }
}