namespace Diquis.Domain.Entities.Football.Common
{
    /// <summary>
    /// Specifies a football (soccer) player's preferred foot.
    /// </summary>
    /// <remarks>
    /// This enum is used to indicate whether a player favors their left foot,
    /// right foot, both feet equally, or has no preferred foot specified.
    /// </remarks>
    public enum Foot
    {
        /// <summary>
        /// No preferred foot specified or not applicable.
        /// </summary>
        None = 0,

        /// <summary>
        /// The player primarily uses their left foot.
        /// </summary>
        Left = 1,

        /// <summary>
        /// The player primarily uses their right foot.
        /// </summary>
        Right = 2,

        /// <summary>
        /// The player is comfortable using both feet (ambidextrous).
        /// </summary>
        Both = 3
    }
}
