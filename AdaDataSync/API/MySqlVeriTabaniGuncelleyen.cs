using System.Data.Common;
using AdaVeriKatmani;
using MySql.Data.MySqlClient;

namespace AdaDataSync.API
{
    internal class MySqlVeriTabaniGuncelleyen : IVeritabaniObjesiYaratan
    {
        private readonly string _baglantiString;

        public MySqlVeriTabaniGuncelleyen(string baglantiString)
        {
            _baglantiString = baglantiString;
        }

        public DbConnection ConnectionYarat()
        {
            return new MySqlConnection(_baglantiString);
        }

        public DbDataAdapter AdaptorYarat()
        {
            return new MySqlDataAdapter();
        }

        public TemelVeriIslemleri TemelVeriIslemleriYarat()
        {
            return new TemelVeriIslemleri(VeritabaniTipi.MySql, _baglantiString);
        }
    }
}