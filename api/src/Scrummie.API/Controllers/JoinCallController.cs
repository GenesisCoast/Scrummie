using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Communications.Common;
using Microsoft.Graph.Communications.Common.Telemetry;
using Microsoft.Graph.Communications.Core.Serialization;
using Scrummie.API.Controllers;
using Scrummie.API.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotObject = Scrummie.API.Bot.Bot;

namespace Scrummie.API.Http
{
    /// <summary>
    /// JoinCallController is a third-party controller (non-Bot Framework) that can be called in CVI
    /// scenario to trigger the bot to join a call.
    /// </summary>
    public class JoinCallController : Controller
    {
        private readonly IGraphLogger graphLogger;

        private BotObject bot;

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinCallController"/> class.
        /// </summary>
        /// <param name="bot">The bot.</param>
        public JoinCallController(BotObject bot)
        {
            this.graphLogger = bot.Client.GraphLogger.CreateShim(nameof(JoinCallController));

            this.bot = bot;
        }

        /// <summary>
        /// The join call async.
        /// </summary>
        /// <param name="joinCallBody">The join call body.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        [Route(HttpRouteConstants.OnJoinCallRoute)]
        public async Task<IActionResult> JoinCallAsync([FromBody] JoinCallRequestData joinCallBody)
        {
            Validator.NotNull(joinCallBody, nameof(joinCallBody));

            var call = await this.bot.JoinCallAsync(joinCallBody).ConfigureAwait(false);

            var callUriTemplate = new UriBuilder(this.bot.BotInstanceUri);
            callUriTemplate.Path = HttpRouteConstants.CallRoutePrefix.Replace("{callLegId}", call.Id);
            callUriTemplate.Query = this.bot.BotInstanceUri.Query.Trim('?');

            var callUri = callUriTemplate.Uri.AbsoluteUri;
            var values = new Dictionary<string, string>
                {
                    { "legId", call.Id },
                    { "scenarioId", call.ScenarioId.ToString() },
                    { "call", callUri },
                    { "logs", callUri.Replace("/calls/", "/logs/") },
                };

            var serializer = new CommsSerializer(pretty: true);
            var json = serializer.SerializeObject(values);
            return this.Ok(json);
        }

        /// <summary>
        /// End the call.
        /// </summary>
        /// <param name="callLegId">Id of the call to end.</param>
        /// <returns>The action result.</returns>
        [HttpDelete]
        [Route(HttpRouteConstants.CallRoutePrefix)]
        public async Task<IActionResult> OnEndCallAsync(string callLegId)
        {
            this.graphLogger.Info($"Ending call {callLegId}");

            await this.bot.TryDeleteCallAsync(callLegId).ConfigureAwait(false);

            return this.Ok();
        }

        /// <summary>
        /// The on get calls.
        /// </summary>
        /// <returns>The action result.</returns>
        [HttpGet]
        [Route(HttpRouteConstants.CallsPrefix + "/")]
        public ActionResult<List<Dictionary<string, string>>> OnGetCalls()
        {
            this.graphLogger.Info("Getting calls");

            if (this.bot.CallHandlers.IsEmpty)
            {
                return null;
            }

            var calls = new List<Dictionary<string, string>>();
            foreach (var callHandler in this.bot.CallHandlers.Values)
            {
                var call = callHandler.Call;
                var callUriTemplate = new UriBuilder(this.bot.BotInstanceUri);
                callUriTemplate.Path = HttpRouteConstants.CallRoutePrefix.Replace("{callLegId}", call.Id);
                callUriTemplate.Query = this.bot.BotInstanceUri.Query.Trim('?');

                var callUri = callUriTemplate.Uri.AbsoluteUri;
                var values = new Dictionary<string, string>
                {
                    { "legId", call.Id },
                    { "scenarioId", call.ScenarioId.ToString() },
                    { "call", callUri },
                    { "logs", callUri.Replace("/calls/", "/logs/") },
                };
                calls.Add(values);
            }

            return calls;
        }
    }
}