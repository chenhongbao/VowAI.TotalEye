using VowAI.TotalEye.ServerShared.Models;

namespace VowAI.TotalEye.Server.Models
{
    public static class Extensions
    {
        public static ClientControlPolicySet GetPolicySet(this User user)
        {
            ClientControlPolicySet policySet = new ClientControlPolicySet();

            if (user?.Policies != null && user?.Policies.Any() == true)
            {
                foreach (ControlPolicy policy in user.Policies)
                {
                    ClientControlPolicy clientPolicy = new ClientControlPolicy
                    {
                        Description = policy.Description,
                        Tag = policy.Tag ?? "",
                        FilterWords = policy.FilterWords ?? "",
                        FilterCondition = policy.FilterCondition ?? "",
                        Action = policy.Action ?? "",
                        ActionDescription = policy.ActionDescription ?? "",
                    };

                    policySet.Policies.Add(clientPolicy);
                }
            }

            return policySet;
        }
    }
}
