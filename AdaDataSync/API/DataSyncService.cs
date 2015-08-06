using System;
using AdaDataSync.Test;

namespace AdaDataSync.API
{
	public class DataSyncService
	{
		private readonly ISyncRepoProxy _repoProxy;
		private readonly ICalisanServisKontrolcusu _servisKontrolcusu;
		private readonly ISafetyNetLogger _ikincilLogger;

		public DataSyncService(ISyncRepoProxy repoProxy, ICalisanServisKontrolcusu servisKontrolcusu, ISafetyNetLogger ikincilLogger)
		{
			_repoProxy = repoProxy;
			_servisKontrolcusu = servisKontrolcusu;
			_ikincilLogger = ikincilLogger;
		}

		public void Sync()
		{
			if (_servisKontrolcusu.BuMakinadaBaskaServisCalisiyorMu())
				return;

			_servisKontrolcusu.MakinaBazindaKilitKoy();

			try
			{
				_repoProxy.BekleyenTransactionlariAl().ForEach(logKaydi =>
				{
					try
					{
						Kayit kaynaktakiKayit = _repoProxy.KaynaktanTekKayitAl(logKaydi);

						if (kaynaktakiKayit == null)
							_repoProxy.HedeftenKayitSil(logKaydi);
						else
							_repoProxy.HedefteInsertVeyaUpdate(kaynaktakiKayit);

						_repoProxy.TransactionLogKayitSil(logKaydi);
					}
					catch (Exception ex)
					{
						_repoProxy.TransactionLogKaydinaHataMesajiYaz(logKaydi, ex.Message);
					}
				});
			}
			catch (Exception ex)
			{
				_ikincilLogger.HataLogla(ex);
			}
		}
	}
}