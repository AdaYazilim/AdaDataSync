using System;
using System.Collections.Generic;

namespace AdaDataSync.API
{
    class VeriAktaran : IVeritabaniIslemYapan
    {
        private readonly IDatabaseProxy _dbProxy;
        private readonly int _syncEdilecekMaxKayitSayisi;

        public VeriAktaran(IDatabaseProxy dbProxy, int syncEdilecekMaxKayitSayisi = 10000)
        {
            _dbProxy = dbProxy;
            _syncEdilecekMaxKayitSayisi = syncEdilecekMaxKayitSayisi;
        }

        public void VeritabaniIslemiYap()
        {
            _dbProxy.BaglantilariAc();

            List<DataTransactionInfo> trInfolar = _dbProxy.BekleyenTransactionlariAl(_syncEdilecekMaxKayitSayisi);
            Console.WriteLine("Aktarılmaya çalışılacak kayıt adedi : {0}", trInfolar.Count);

            foreach (DataTransactionInfo logKaydi in trInfolar)
            {
                try
                {
                    if (logKaydi.TabloAdi.ToLowerInvariant() != "ddlog" && logKaydi.TabloAdi.ToLowerInvariant() != "w_exists_tbl")        // ddlog kayıtları senkronize edilmeyecek.
                    {
                        tekLogKaydiniIsle(logKaydi);
                        _dbProxy.LogKaydiniSqleAktar(logKaydi);    
                    }
                    
                    _dbProxy.LogKayitSil(logKaydi);
                }
                catch (Exception ex)
                {
                    _dbProxy.LogKaydinaHataMesajiYaz(logKaydi, ex);
                }
            }

            _dbProxy.BaglantilariKapat();
        }

        private void tekLogKaydiniIsle(DataTransactionInfo logKaydi)
        {
            if (logKaydi.TabloAdi.ToLowerInvariant() == "ddlog")
                return;

            Kayit kaynaktakiKayit = _dbProxy.KaynaktanTekKayitAl(logKaydi);

            if (kaynaktakiKayit == null)
                _dbProxy.HedeftenKayitSil(logKaydi);
            else
                _dbProxy.HedefteInsertVeyaUpdate(kaynaktakiKayit, logKaydi);
        }
    }
}