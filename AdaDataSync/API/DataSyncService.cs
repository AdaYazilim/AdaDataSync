using System;
using AdaDataSync.Test;

namespace AdaDataSync.API
{
	public class DataSyncService
	{
		private readonly IDatabaseProxy _dbProxy;
		private readonly ICalisanServisKontrolcusu _servisKontrolcusu;
		private readonly ISafetyNetLogger _ikincilLogger;

		public DataSyncService(IDatabaseProxy dbProxy, ICalisanServisKontrolcusu servisKontrolcusu, ISafetyNetLogger ikincilLogger)
		{
			_dbProxy = dbProxy;
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
				_dbProxy.BekleyenTransactionlariAl().ForEach(logKaydi =>
				{
					try
					{
						tekLogKaydiniIsle(logKaydi);
					}
					catch (Exception ex)
					{
						_dbProxy.TransactionLogKaydinaHataMesajiYaz(logKaydi, ex.Message);
					}
				});
			}
			catch (Exception ex)
			{
				_ikincilLogger.HataLogla(ex);
			}
		}

		private void tekLogKaydiniIsle(DataTransactionInfo logKaydi)
		{
			Kayit kaynaktakiKayit = _dbProxy.KaynaktanTekKayitAl(logKaydi);

			if (kaynaktakiKayit == null)
				_dbProxy.HedeftenKayitSil(logKaydi);
			else
				_dbProxy.HedefteInsertVeyaUpdate(kaynaktakiKayit, logKaydi);

			_dbProxy.TransactionLogKayitSil(logKaydi);
		}
	}
}