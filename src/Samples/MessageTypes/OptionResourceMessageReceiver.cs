using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
using Lime.Protocol;
using Take.Blip.Client;
using Takenet.Iris.Messaging.Contents;

namespace MessageTypes
{
    public class OptionResourceMessageReceiver : IMessageReceiver
    {
        private readonly ISender _sender;

        public OptionResourceMessageReceiver(ISender sender)
        {
            _sender = sender;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            var document = new Resource
            {
                Key = "welcome-message" //recurso previamente adicionado com extensão 'recursos' ou através do portal
            };

            await _sender.SendMessageAsync(document, message.From, cancellationToken);
        }
    }

    public class ResourceMessageReplace : IMessageReceiver
    {
        private readonly ISender _sender;

        public ResourceMessageReplace(ISender sender)
        {
            _sender = sender;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            var openWith = new Dictionary<string, string>();//using System.Collections.Generic
            openWith.Add("name",message.From.Name);//checar mais tarde <<

            var document = new Resource
            {
                Key = "welcome-message",
                Variables = openWith
            
            };

            await _sender.SendMessageAsync(document, message.From, cancellationToken);
        }
    }

}
