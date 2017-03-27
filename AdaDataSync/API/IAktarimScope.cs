using System.Collections.Generic;
using System.Linq;

namespace AdaDataSync.API
{
    public interface IAktarimScope
    {
        bool TabloAktarimaDahil(string tabloAdi);
    }

    class ButunTablolarAktarimScope : IAktarimScope
    {
        public bool TabloAktarimaDahil(string tabloAdi)
        {
            return true;
        }
    }

    class DahilTablolarAktarimScope : IAktarimScope
    {
        private readonly HashSet<string> _dahilTablolar; 

        public DahilTablolarAktarimScope(string dahilTablolarString)
        {
            IEnumerable<string> dahilTablolar =
                (dahilTablolarString ?? string.Empty)
                    .Split(new[] {','}, System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(tblAdi => tblAdi.Trim().ToLowerInvariant());

            _dahilTablolar = new HashSet<string>(dahilTablolar);
        }

        public bool TabloAktarimaDahil(string tabloAdi)
        {
            return _dahilTablolar.Contains(tabloAdi);
        }
    }

    class HaricTablolarAktarimScope : IAktarimScope
    {
        private readonly HashSet<string> _haricTablolar; 

        public HaricTablolarAktarimScope(string dahilTablolarString)
        {
            IEnumerable<string> haricTablolar =
                (dahilTablolarString ?? string.Empty)
                    .Split(new[] {','}, System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(tblAdi => tblAdi.Trim().ToLowerInvariant());

            _haricTablolar = new HashSet<string>(haricTablolar);
        }

        public bool TabloAktarimaDahil(string tabloAdi)
        {
            return !_haricTablolar.Contains(tabloAdi);
        }
    }
}
