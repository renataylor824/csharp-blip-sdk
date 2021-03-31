﻿using Lime.Protocol;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Builder.Models;
using Take.Blip.Client.Extensions.ArtificialIntelligence;
using Takenet.Iris.Messaging.Resources.ArtificialIntelligence;

namespace Take.Blip.Builder.Actions.ProcessContentAssistant
{
    /// <summary>
    /// Content Assistant action
    /// </summary>
    public class ProcessContentAssistantAction : ActionBase<ProcessContentAssistantSettings>
    {
        private readonly IArtificialIntelligenceExtension _artificialIntelligenceExtension;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ProcessContentAssistantAction(IArtificialIntelligenceExtension artificialIntelligenceExtension) : base(nameof(ProcessContentAssistant))
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            _artificialIntelligenceExtension = artificialIntelligenceExtension;
        }

        /// <summary>
        /// Get content result to a given input
        /// </summary>
        /// <param name="context">bot context</param>
        /// <param name="settings">ContentAssistant settings</param 
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task ExecuteAsync(IContext context, ProcessContentAssistantSettings settings, CancellationToken cancellationToken)
        {
            var contentAssistantResource = new AnalysisRequest
            {
                Text = settings.Text,
                Score = settings.Score ?? context.Flow.BuilderConfiguration.MinimumIntentScore.Value / Constants.PERCENTAGE_DENOMINATOR
            } as Document;

            var contentResult = await _artificialIntelligenceExtension.GetContentResultAsync(
                contentAssistantResource,
                cancellationToken: cancellationToken);

            await SetContentResultAsync(context, settings, contentResult, cancellationToken);
        }

        private async Task SetContentResultAsync(
          IContext context, ProcessContentAssistantSettings settings, ContentResult contentResult, CancellationToken cancellationToken)
        {
            var combinationFound = contentResult.Combinations?.FirstOrDefault();

            var value = JsonConvert.SerializeObject(new ContentAssistantActionResponse
            {
                HasCombination = contentResult?.Result?.Content != null,
                Value = contentResult?.Result?.Content?.ToString(),
                Intent = combinationFound?.Intent,
                Entities = combinationFound?.Entities.ToList()
            });

            await context.SetVariableAsync(settings.OutputVariable, value, cancellationToken);
        }

    }
}
