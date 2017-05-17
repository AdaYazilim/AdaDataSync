using System.Data.Common;
using System.Data.SqlClient;
using AdaDataSync.API.VeriYapisiDegistirme;
using AdaVeriKatmani;

namespace AdaDataSync.API
{
    internal class MsSqlVeriTabaniGuncelleyen : IVeritabaniObjesiYaratan
    {
        private readonly string _baglantiString;

        public MsSqlVeriTabaniGuncelleyen(string baglantiString)
        {
            _baglantiString = baglantiString;
        }

        public DbConnection ConnectionYarat()
        {
            return new SqlConnection(_baglantiString);
        }

        public DbDataAdapter AdaptorYarat()
        {
            return new SqlDataAdapter();
        }

        public TemelVeriIslemleri TemelVeriIslemleriYarat()
        {
            return new TemelVeriIslemleri(VeritabaniTipi.SqlServer2, _baglantiString);
        }

        public IVeriYapisiDegistiren VeriYapisiDegistirenAl()
        {
            return new MsSqlVeriYapisiDegistiren();
        }
    }
}