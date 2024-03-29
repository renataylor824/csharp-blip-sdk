﻿using System;
using Lime.Protocol;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol.Network;
using Take.Blip.Client.Content;

namespace Take.Blip.Client.Extensions.Tunnel
{
    /// <summary>
    /// Allows forwarding envelopes to an identity.
    /// </summary>
    public interface ITunnelExtension
    {
        Task<Takenet.Iris.Messaging.Resources.Tunnel> GetTunnelAsync(Identity tunnelIdentity, CancellationToken cancellationToken);
    
        Task<Node> ForwardMessageAsync(Message message, Identity destination, CancellationToken cancellationToken);

        Task<Node> ForwardNotificationAsync(Notification notification, Identity destination, CancellationToken cancellationToken);
    }

    public static class TunnelExtensionExtensions
    {
        public static async Task<Takenet.Iris.Messaging.Resources.Tunnel> TryGetTunnelAsync<T>(
            this ITunnelExtension tunnelExtension,
            T envelope,
            CancellationToken cancellationToken)
            where T : Envelope
        {
            if (envelope == null) throw new ArgumentNullException(nameof(envelope));

            var fromNode = envelope.From;
            var isInputExpiration = (envelope is Message && (envelope as Message).Content is InputExpiration);

            if (envelope.Metadata != null && envelope.Metadata.ContainsKey(Constants.ORIGINAL_SUBFLOW_REDIRECT_FROM))
            {
                envelope.Metadata.TryGetValue(Constants.ORIGINAL_SUBFLOW_REDIRECT_FROM, out string originalFrom);
                fromNode = Identity.Parse(originalFrom).ToNode();
            }
            else if (isInputExpiration)
            {
                fromNode = ((envelope as Message).Content as InputExpiration).Identity.ToNode();
            }

            if (fromNode?.Domain == null ||
                !fromNode.Domain.Equals(TunnelExtension.TunnelAddress.Domain, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (envelope.TryGetTunnelFromEnvelope(out var tunnel))
            {
                return tunnel;
            }

            try
            {
                return await tunnelExtension.GetTunnelAsync(fromNode.ToIdentity(), cancellationToken);
            }
            catch (LimeException ex) when (ex.Reason.Code == ReasonCodes.COMMAND_RESOURCE_NOT_FOUND)
            {
                return null;
            }
        }
    }
}
