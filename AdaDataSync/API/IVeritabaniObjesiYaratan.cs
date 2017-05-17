using System.Data.Common;
using AdaDataSync.API.VeriYapisiDegistirme;
using AdaVeriKatmani;

namespace AdaDataSync.API
{
    internal interface IVeritabaniObjesiYaratan
    {
        DbConnection ConnectionYarat();
        DbDataAdapter AdaptorYarat();
        TemelVeriIslemleri TemelVeriIslemleriYarat();
        IVeriYapisiDegistiren VeriYapisiDegistirenAl();
    }
}