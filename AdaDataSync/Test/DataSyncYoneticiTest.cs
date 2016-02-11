using System;
using AdaDataSync.API;
using NSubstitute;
using NUnit.Framework;

namespace AdaDataSync.Test
{
    [TestFixture]
    public class DataSyncYoneticiTest
    {
        private IDataSyncService _dataSyncServis;
        private ILogger _safetyLogger;
        private DataSyncYonetici _dataSyncYonetici;

        [SetUp]
        public void TestSetup()
        {
            _dataSyncServis = Substitute.For<IDataSyncService>();
            _safetyLogger = Substitute.For<ILogger>();

            _dataSyncYonetici = new DataSyncYonetici(_dataSyncServis, _safetyLogger);
        }

        [Test]
        public void servisin_synci_patlamazsa_safetyloggera_kayit_atilmamali()
        {
            _dataSyncYonetici.Sync();
            _safetyLogger.DidNotReceiveWithAnyArgs().Logla("");
        }

        [Test]
        public void servisin_synci_patlarsa_yoneticinin_synci_patlamamali_ve_safetyloggera_kayit_atilmali()
        {
            Exception ex = new Exception("gdfgdfgd");
            _dataSyncServis.When(ds => ds.Sync()).Do(x => { throw ex; });

            Assert.DoesNotThrow(() => _dataSyncYonetici.Sync());
            _safetyLogger.Received().Logla(ex.Message);
        }
    }
}
