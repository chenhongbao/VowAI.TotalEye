using VowAI.TotalEye.ServerShared.Models;

namespace VowAI.TotalEye.ServerShared
{
    public static class Extensions
    {
        public static ICollection<ClientControlPolicyItem> GetPolicyItems(this ClientControlPolicySet policySet)
        {
            List<ClientControlPolicyItem> policyItems = new();

            policySet.Policies.Select(policy => policy.Policies).All(policies =>
            {
                policyItems.AddRange(policies);
                return false;
            });

            return policyItems;
        }

    }
}
