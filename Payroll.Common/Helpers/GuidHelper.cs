using System;

namespace Payroll.Common.Helpers
{
    public class GuidHelper
    {
        public static long GetNewId()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode());
        }
    }
}
