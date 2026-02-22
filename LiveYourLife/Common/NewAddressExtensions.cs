using System.Collections.Generic;
using System.Linq;
using SOD.Common.Extensions;

namespace LiveYourLife.Common;

public static class NewAddressExtensions
{
    public static IEnumerable<NewAddress> WhereIsResidential(this IEnumerable<NewAddress> addresses)
    {
        return addresses.Where(a => a.residence is not null);
    }
    
    public static IEnumerable<NewAddress> WhereIsComercial(this IEnumerable<NewAddress> addresses)
    {
        return addresses.Where(a => a.company is not null);
    }
    
    public static float CalculateFamilyIncome(this NewAddress address)
    {
        return address.inhabitants.Select(c => c.job.salary).Sum();
    }
}