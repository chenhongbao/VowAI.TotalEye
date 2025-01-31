namespace VowAI.TotalEye.Models
{
    public static class Extensions
    {
        public static CentreInfoRequest Deserialize(this CentreInfoRequest instance, string plainText)
        {
            string[] lines = plainText.Split('\n');

            if (lines.Length < 5)
            {
                throw new ArgumentException("Invalid string representation.");
            }

            instance.RequestId = int.Parse(lines[0].Trim());
            instance.Name = lines[1].Trim();
            instance.Description = lines[2].Trim();
            instance.Token = lines[3].Trim();
            instance.ReplyUrl = lines[4].Trim();

            return instance;
        }

        public static string Serialize(this CentreInfoRequest instance)
        {
            return $"{instance.RequestId}\n{instance.Name}\n{instance.Description}\n{instance.Token}\n{instance.ReplyUrl}";
        }

        public static ClientControlPolicy Deserialize(this ClientControlPolicy instance, string plainText)
        {
            string[] lines = plainText.Split('\n');

            if (lines.Length < 2)
            {
                throw new ArgumentException("Invalid string representation.");
            }

            instance.PolicyId = int.Parse(lines[0].Trim());
            instance.Tag = lines[1].Trim();
            instance.Policies = new List<ControlPolicyItem>();

            for (int i = 2; i + 4 < lines.Length; i += 5)
            {
                instance.Policies.Add(new ControlPolicyItem
                {
                    ItemId = int.Parse(lines[i].Trim()),
                    FilterWords = lines[i + 1].Trim(),
                    FilterCondition = lines[i + 2].Trim(),
                    Action = lines[i + 3].Trim(),
                    ActionDescription = lines[i + 4].Trim(),
                });
            }

            return instance;
        }

        public static string Serialize(this ClientControlPolicy instance)
        {
            string text = $"{instance.PolicyId}\n{instance.Tag}";

            foreach (ControlPolicyItem item in instance.Policies)
            {
                text += $"\n{item.ItemId}\n{item.FilterWords}\n{item.FilterCondition}\n{item.Action}\n{item.ActionDescription}";
            }

            return text;
        }
    }
}
