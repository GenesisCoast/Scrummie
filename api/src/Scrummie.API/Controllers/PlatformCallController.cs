using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Communications.Common.Telemetry;
using Scrummie.API.Controllers;
using Scrummie.Common.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BotObject = Scrummie.API.Bot.Bot;

namespace Scrummie.API.Http
{
    /// <summary>
    /// Entry point for handling call-related web hook requests from the stateful client.
    /// </summary>
    public class PlatformCallController : Controller
    {
        private readonly BotObject bot;
        private readonly IGraphLogger graphLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformCallController"/> class.
        /// </summary>
        /// <param name="bot">The bot.</param>
        public PlatformCallController(BotObject bot)
        {
            this.graphLogger = bot.Client.GraphLogger.CreateShim(nameof(PlatformCallController));

            this.bot = bot;
        }

        /// <summary>
        /// Get the callback logs.
        /// </summary>
        /// <param name="maxCount">The maximum count of log lines.</param>
        /// <returns>The logs.</returns>
        [HttpGet]
        [Route("log/callback")]
        public async Task<IEnumerable<string>> GetCallbackLogsAsync(int maxCount = 1000)
        {
            return await Task.FromResult(this.bot.GetCallbackLogs(maxCount)).ConfigureAwait(false);
        }

        /// <summary>
        /// Handle a callback for an existing call.
        /// </summary>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        [HttpPost]
        [Route(HttpRouteConstants.OnIncomingRequestRoute)]
        public async Task<IActionResult> OnIncomingRequestAsync()
        {
            //var requestMessage = this.Request.HttpContext.Request;

            //var requestUri = requestMessage.GetUri;

            //// Process the incoming request for current web instance.
            //this.bot.AddCallbackLog($"Process incoming request {requestUri}");

            //var token = requestMessage?.Headers["Authorization"];

            //this.bot.AddCallbackLog($"Token: {token ?? "null"}");

            //this.graphLogger.Info($"Received HTTP {this.Request.Method}, {requestUri}");

            //// Pass the incoming message to the sdk. The sdk takes care of what to do with it.
            //var response = await this.bot.Client.ProcessNotificationAsync().ConfigureAwait(false);

            //// Convert the status code, content of HttpResponseMessage to IActionResult, and copy
            //// the headers from response to HttpContext.Response.Headers.
            return await this.GetActionResultAsync(null).ConfigureAwait(false);
        }
    }
}