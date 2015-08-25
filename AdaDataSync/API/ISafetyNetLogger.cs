using System;

namespace AdaDataSync.API
{
    public interface ISafetyNetLogger
    {
        void HataLogla(Exception exception);
    }
}
