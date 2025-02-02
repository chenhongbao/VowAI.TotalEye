using VowAI.TotalEye.ServerShared;
using VowAI.TotalEye.ServerShared.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Client
{
    public class ConfiguredComputerSniffer : IConfiguredComputerSniffer
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
                    ClientControlPolicySet? policySet = _policyProvider.GetPolicy("local_computer");

                    if (policySet != null)
                    {
                        ApplyPolicySet(policySet);
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

        private void ApplyPolicySet(ClientControlPolicySet policySet)
        {
            if (policySet.Policies.Any())
            {
                string[] processes = LocalComputer.RunCommand("tasklist /FO Table /NH").Split('\n');

                foreach (ClientControlPolicy policy in policySet.Policies)
                {
                    ApplyPolicy(policy, processes);
                }
            }
        }

        private void ApplyPolicy(ClientControlPolicy policy, string[] processes)
        {
            foreach (string process in processes)
            {
                ApplyPolicyProcess(policy, process);
            }
        }

        private void ApplyPolicyProcess(ClientControlPolicy policy, string process)
        {
            List<string> cells = process.Split(' ').ToList();
            cells.RemoveAll(x => string.IsNullOrEmpty(x) || string.IsNullOrWhiteSpace(x));

            if (cells.Count > 1)
            {
                if (policy.FilterWords.Split([';', ',']).Any(word => ApplyCondition(policy.FilterCondition, cells[0], word)))
                {
                    ApplyProcessAction(policy, cells[1]);
                }
            }
        }

        private void ApplyProcessAction(ClientControlPolicy policy, string pid)
        {
            switch (policy.Action.ToLower())
            {
                case "client_taskkill":

                    LocalComputer.RunCommand($"taskkill /F /PID {pid}").WriteString<ConfiguredComputerSniffer>();
                    break;

                case "client_command":

                    LocalComputer.RunCommand(policy.ActionDescription).WriteString<ConfiguredComputerSniffer>();
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
