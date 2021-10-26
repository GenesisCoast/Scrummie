using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Communications.Common;
using Scrummie.API.Controllers;
using Scrummie.Data;
using System;
using System.Threading.Tasks;
using BotObject = Scrummie.API.Bot.Bot;

namespace Scrummie.API.Http
{
    /// <summary>
    /// AddParticipantController is a third-party controller (non-Bot Framework) that can be called
    /// to trigger a transfer.
    /// </summary>
    public class AddParticipantController : Controller
    {
        private BotObject bot;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddParticipantController"/> class.
        /// </summary>
        /// <param name="bot">The bot.</param>
        public AddParticipantController(BotObject bot)
        {
            this.bot = bot;
        }

        /// <summary>
        /// The add participants async.
        /// </summary>
        /// <param name="callLegId">The call to add participants to.</param>
        /// <param name="addParticipantBody">The add participant request body.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        [Route(HttpRouteConstants.OnAddParticipantRoute)]
        public async Task<IActionResult> AddParticipantAsync(string callLegId, [FromBody] AddParticipantRequestData addParticipantBody)
        {
            Validator.IsTrue(Guid.TryParse(callLegId, out Guid result), nameof(callLegId), "call leg id must be a valid guid.");
            Validator.NotNull(addParticipantBody, nameof(addParticipantBody));

            await this.bot.AddParticipantAsync(callLegId, addParticipantBody).ConfigureAwait(false);
            return this.Ok();
        }
    }
}