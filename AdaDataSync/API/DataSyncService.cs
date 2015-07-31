namespace AdaDataSync.API
{
	public class DataSyncService
	{
		private readonly ISyncRepoProxy _repoProxy;
		private readonly ICalisanServisKontrolcusu _servisKontrolcusu;

		public DataSyncService(ISyncRepoProxy repoProxy, ICalisanServisKontrolcusu servisKontrolcusu)
		{
			_repoProxy = repoProxy;
			_servisKontrolcusu = servisKontrolcusu;
		}

		public void Sync()
		{
			if (_servisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu())
				return;

			_servisKontrolcusu.MakinaBazindaKilitKoy();

			_repoProxy.BekleyenTransactionlariAl().ForEach(logKaydi =>
			{
				Kayit kaynaktakiKayit = _repoProxy.KaynaktanTekKayitAl(logKaydi);
				if (kaynaktakiKayit == null)
					_repoProxy.HedeftenKayitSil(logKaydi);
				else
					_repoProxy.HedefteInsertVeyaUpdate(kaynaktakiKayit);
			});
		}
	}
}