namespace Scrummie.API.Data
{
    /// <summary>
    /// The meeting info class.
    /// </summary>
    public class MeetingInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether to conversation without a host is allowed.
        /// </summary>
        public bool AllowConversationWithoutHost { get; set; }

        /// <summary>
        /// Gets or sets the join URL of the meeting.
        /// </summary>
        public string JoinURL { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to remove the bot from default routing group.
        /// </summary>
        public bool RemoveFromDefaultRoutingGroup { get; set; }
    }
}