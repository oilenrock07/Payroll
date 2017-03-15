using System;

namespace Payroll.Common.Extension
{
    public static class DecimalExtension
    {
        public static string FormatAmount(this decimal amount)
        {
            if (amount < 0)
                return String.Format("Php ({0})", amount.ToString("####,###,##0.00"));

            return String.Format("Php {0}", amount.ToString("####,###,##0.00"));
        }
    }
}
