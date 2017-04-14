using System.Data.Common;
using AdaVeriKatmani;

namespace AdaDataSync.API
{
    internal interface IVeritabaniObjesiYaratan
    {
        DbConnection ConnectionYarat();
        DbDataAdapter AdaptorYarat();
        TemelVeriIslemleri TemelVeriIslemleriYarat();
    }
}