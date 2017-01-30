namespace AdaDataSync.API
{
    public class DataSyncService : IDataSyncService
    {
        private readonly IVeritabaniIslemYapan _dataDefinitionGuncelleyen;
        private readonly IVeritabaniIslemYapan _veriAktaran;

        public DataSyncService(IVeritabaniIslemYapan dataDefinitionGuncelleyen, IVeritabaniIslemYapan veriAktaran)
        {
            _dataDefinitionGuncelleyen = dataDefinitionGuncelleyen;
            _veriAktaran = veriAktaran;
        }

        public void Sync()
        {
            _dataDefinitionGuncelleyen.VeritabaniIslemiYap();
            _veriAktaran.VeritabaniIslemiYap();
        }
    }
}