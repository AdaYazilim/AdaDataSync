using System;
using System.Collections.Generic;

namespace AdaDataSync.API
{
    class VeriAktaran : IVeritabaniIslemYapan
    {
        private readonly IDatabaseProxy _dbProxy;
        private readonly IAktarimScope _aktarimScope;
        private readonly int _syncEdilecekMaxKayitSayisi;

        public VeriAktaran(IDatabaseProxy dbProxy, IAktarimScope aktarimScope, int syncEdilecekMaxKayitSayisi = 10000)
        {
            _dbProxy = dbProxy;
            _aktarimScope = aktarimScope;
            _syncEdilecekMaxKayitSayisi = syncEdilecekMaxKayitSayisi;
        }

        public void VeritabaniIslemiYap()
        {
            try
            {
                _dbProxy.BaglantilariAc();

                List<DataTransactionInfo> trInfolar = _dbProxy.BekleyenTransactionlariAl(_syncEdilecekMaxKayitSayisi);
                Console.WriteLine("Aktarılmaya çalışılacak kayıt adedi : {0}", trInfolar.Count);

                foreach (DataTransactionInfo logKaydi in trInfolar)
                {
                    try
                    {
                        // ddlog kayıtları senkronize edilmeyecek.

                        if (_aktarimScope.TabloAktarimaDahil(logKaydi.TabloAdi))
                        {
                            if (logKaydi.TabloAdi != "ddlog" && logKaydi.TabloAdi != "w_exists_tbl")
                            {
                                tekLogKaydiniIsle(logKaydi);
                                _dbProxy.LogKaydiniSqleAktar(logKaydi);
                            }    
                        }

                        _dbProxy.LogKayitSil(logKaydi);
                    }
                    catch (Exception ex)
                    {
                        _dbProxy.LogKaydinaHataMesajiYaz(logKaydi, ex);
                    }
                }
            }
            finally
            {
                _dbProxy.BaglantilariKapat();
            }
        }

        private void tekLogKaydiniIsle(DataTransactionInfo logKaydi)
        {
            Kayit kaynaktakiKayit = _dbProxy.KaynaktanTekKayitAl(logKaydi);

            if (kaynaktakiKayit == null)
                _dbProxy.HedeftenKayitSil(logKaydi);
            else
                _dbProxy.HedefteInsertVeyaUpdate(kaynaktakiKayit, logKaydi);
        }
    }
}