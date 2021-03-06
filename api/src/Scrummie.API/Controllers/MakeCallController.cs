using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Communications.Common;
using Scrummie.API.Controllers;
using Scrummie.API.Data;
using System;
using System.Threading.Tasks;
using BotObject = Scrummie.API.Bot.Bot;

namespace Scrummie.API.Http
{
    /// <summary>
    /// MakeCallController is a third-party controller (non-Bot Framework) that makes an outbound
    /// call to a target.
    /// </summary>
    public class MakeCallController : Controller
    {
        private BotObject bot;

        /// <summary>
        /// Initializes a new instance of the <see cref="MakeCallController"/> class.
        /// </summary>
        /// <param name="bot">The bot.</param>
        public MakeCallController(BotObject bot)
        {
            this.bot = bot;
        }

        // TODO: Reminder: make sure when calling this controller that the Content-Type of your request should be "application/json"

        /// <summary>
        /// The making outbound call async.
        /// </summary>
        /// <param name="makeCallBody">The making outgoing call request body.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        [Route(HttpRouteConstants.OnMakeCallRoute)]
        public async Task<IActionResult> MakeOutgoingCallAsync([FromBody] MakeCallRequestData makeCallBody)
        {
            Validator.NotNull(makeCallBody, nameof(makeCallBody));

            await this.bot.MakeCallAsync(makeCallBody, Guid.NewGuid()).ConfigureAwait(false);
            return this.Ok();
        }
    }
}