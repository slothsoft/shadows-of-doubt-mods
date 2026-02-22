using System.Collections.Generic;
using System.Linq;
using SOD.Common.Extensions;

namespace LiveYourLife.Common;

public static class NewAddressExtensions
{
    public static IEnumerable<NewAddress> WhereIsResidential(this IEnumerable<NewAddress> addresses)
    {
        return addresses.Where(a => a.inhabitants.Count > 0);
    }
    
    public static float CalculateFamilyIncome(this NewAddress address)
    {
        return address.inhabitants.Select(c => c.job.salary).Sum();
    }
}