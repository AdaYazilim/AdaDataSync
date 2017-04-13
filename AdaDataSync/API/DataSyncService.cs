namespace AdaDataSync.API
{
    public class DataSyncService : IDataSyncService
    {
        private readonly IVeritabaniGuncelleyen _dataDefinitionGuncelleyen;
        private readonly IVeriAktaran _veriAktaran;

        public DataSyncService(IVeritabaniGuncelleyen dataDefinitionGuncelleyen, IVeriAktaran veriAktaran)
        {
            _dataDefinitionGuncelleyen = dataDefinitionGuncelleyen;
            _veriAktaran = veriAktaran;
        }

        public void Sync()
        {
            _dataDefinitionGuncelleyen.Guncelle();
            _veriAktaran.AktarimYap();
        }
    }
}