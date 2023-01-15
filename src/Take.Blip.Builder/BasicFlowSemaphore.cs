using System;
using System.Threading;
using System.Threading.Tasks;
using AsyncKeyedLock;
using Lime.Protocol;
using Take.Blip.Builder.Models;

namespace Take.Blip.Builder
{
    class BasicFlowSemaphore: IFlowSemaphore
    {
        private readonly AsyncKeyedLocker<string> _asyncKeyedLocker;

        public BasicFlowSemaphore(
            AsyncKeyedLocker<string> asyncKeyedLocker
            )
        {
            _asyncKeyedLocker = asyncKeyedLocker;
        }

        public ValueTask<IDisposable> WaitAsync(Flow flow, Message message, Identity userIdentity, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return _asyncKeyedLocker.LockAsync($"{flow.Id}:{userIdentity}", timeout, cancellationToken);
        }
    }
}
