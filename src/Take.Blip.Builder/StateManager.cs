﻿using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Lime.Protocol.Network;
using Take.Blip.Client.Extensions.Context;

namespace Take.Blip.Builder
{
    public class StateManager : IStateManager
    {
        private const string STATE_ID_KEY = "stateId";

        private readonly IContextExtension _contextExtension;

        public StateManager(IContextExtension contextExtension)
        {
            _contextExtension = contextExtension;
        }

        public async Task<string> GetStateIdAsync(string flowId, Identity user, CancellationToken cancellationToken)
        {
            try
            {
                return await _contextExtension.GetTextVariableAsync(user, $"{STATE_ID_KEY}@{flowId}", cancellationToken);
            }
            catch (LimeException ex) when (ex.Reason.Code == ReasonCodes.COMMAND_RESOURCE_NOT_FOUND)
            {
                return null;
            }
        }
        public Task SetStateIdAsync(string flowId, Identity user, string stateId, CancellationToken cancellationToken)
            => _contextExtension.SetTextVariableAsync(user, $"{STATE_ID_KEY}@{flowId}", stateId, cancellationToken);

        public async Task DeleteStateIdAsync(string flowId, Identity user, CancellationToken cancellationToken)
        {
            try
            {
                await _contextExtension.DeleteVariableAsync(user, $"{STATE_ID_KEY}@{flowId}", cancellationToken);
            }
            catch (LimeException ex) when (ex.Reason.Code == ReasonCodes.COMMAND_RESOURCE_NOT_FOUND) { }
        }
    }
}