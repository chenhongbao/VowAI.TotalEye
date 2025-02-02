using VowAI.TotalEye.ServerShared.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Client
{
    public class ConfiguredComputerSniffer: IConfiguredComputerSniffer
    {
        private readonly IClientControlPolicyProvider _policyProvider;
        private readonly TimeSpan _pollingTime = TimeSpan.FromSeconds(30);
        private bool _disposed;
        private Task _task;

        public ConfiguredComputerSniffer(IClientControlPolicyProvider policyProvider)
        {
            _policyProvider = policyProvider;
            _disposed = false;

            _task = Task.Run(() => PollProcess(null));
        }

        public void Dispose()
        {
            _disposed = true;
            _task.Wait(_pollingTime + TimeSpan.FromSeconds(1));
        }

        private async void PollProcess(object? state)
        {
            while (_disposed == false)
            {
                try
                {
                    ClientControlPolicy? policy = _policyProvider.GetPolicy("local_computer");

                    if (policy != null)
                    {
                        ApplyPolicy(policy);
                    }
                }
                catch (Exception exception)
                {
                    exception.WriteString<ServerPoller>();
                }

                try
                {
                    await Task.Delay(_pollingTime);
                }
                catch (Exception exception)
                {
                    exception.WriteString<ServerPoller>();
                }
            }
        }

        private void ApplyPolicy(ClientControlPolicy policy)
        {
            if (policy.Policies != null && policy.Policies.Any())
            {
                string[] processes = LocalComputer.RunCommand("tasklist /FO Table /NH").Split('\n');

                foreach (ControlPolicyItem item in policy.Policies)
                {
                    ApplyPolicyItem(item, processes);
                }
            }
        }

        private void ApplyPolicyItem(ControlPolicyItem item, string[] processes)
        {
            foreach (string process in processes)
            {
                ApplyPolicyProcess(item, process);
            }
        }

        private void ApplyPolicyProcess(ControlPolicyItem item, string process)
        {
            List<string> cells = process.Split(' ').ToList();
            cells.RemoveAll(x => string.IsNullOrEmpty(x) || string.IsNullOrWhiteSpace(x));

            if (cells.Count > 1)
            {
                if (item.FilterWords.Split([';', ',']).Any(word => ApplyCondition(item.FilterCondition, cells[0], word)))
                {
                    ApplyProcessAction(item, cells[1]);
                }
            }
        }

        private void ApplyProcessAction(ControlPolicyItem item, string pid)
        {
            switch(item.Action.ToLower())
            {
                case "client_taskkill":

                    LocalComputer.RunCommand($"taskkill /F /PID {pid}").WriteString<ConfiguredComputerSniffer>();
                    break;

                case "client_command":

                    LocalComputer.RunCommand(item.ActionDescription).WriteString<ConfiguredComputerSniffer>();
                    break;
            }
        }

        private bool ApplyCondition(string condition, string processName, string word)
        {
            return condition.ToLower() switch
            {
                "contain" => processName.Contains(word),
                "equal" => processName.Equals(word),
                _ => throw new ArgumentException($"Unknown filter condition '{condition}'."),
            };
        }
    }
}
