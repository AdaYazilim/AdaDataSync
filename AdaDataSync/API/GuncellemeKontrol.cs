using System;
using System.Data.OleDb;
using System.IO;

namespace AdaDataSync.API
{
    class FoxproGuncellemeKontrol : IGuncellemeKontrol
    {
        private readonly string _dataSource;

        public FoxproGuncellemeKontrol(string connectionString)
        {
            OleDbConnection con = new OleDbConnection(connectionString);
            _dataSource = con.DataSource;
        }

        public bool SuAndaGuncellemeYapiliyor()
        {
            string pathName = Path.GetDirectoryName(_dataSource);
            if (string.IsNullOrWhiteSpace(pathName))
                throw new Exception("_kaynakVeriIslemleri.DataSource boş olmamalı.");

            string guncellemeTxtAdresi = Path.Combine(pathName, "GUNCELLEME.TXT");
            return File.Exists(guncellemeTxtAdresi);
        }
    }
}