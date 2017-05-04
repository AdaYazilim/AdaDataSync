using AdaDataSync.API;
using NUnit.Framework;

namespace AdaDataSync.Test
{
    [TestFixture]
    class EpostaTest
    {
        [Ignore]
		[Test]
		public void kaynakta_kayit_yoksa_hedeften_siler()
		{
            EPosta.Gonder("deneme", "içerik");
        }
	}
}
