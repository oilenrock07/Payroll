using System;

namespace Payroll.Common.Extension
{
    public static class DecimalExtension
    {
        public static string FormatAmount(this decimal amount)
        {
            return String.Format("Php {0}", amount.ToString("####,###,###.00"));
        }
    }
}
