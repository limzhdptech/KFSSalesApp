using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QHSalesApp
{
    class BsnValidationTriggerAction : TriggerAction<Entry>
    {
        private string _prevValue = string.Empty;

        protected override void Invoke(Entry entry)
        {
            int n;
            var isNumeric = int.TryParse(entry.Text, out n);

            if (!string.IsNullOrWhiteSpace(entry.Text) && (entry.Text.Length > 9 || !isNumeric))
            {
                entry.Text = _prevValue;
                return;
            }

            _prevValue = entry.Text;
        }
    }
}
