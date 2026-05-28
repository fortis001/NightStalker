using LSH.Core;

namespace NightStalker.Core
{
    [TimeChannelNameEnum]
    public enum TimeChannelNameType
    {
        Game = 0,
        UI = 1
    }

    public static class TimeChannelName
    {
        public static TimeChannelReference Game =>
            TimeChannelNameUtility.From(TimeChannelNameType.Game);

        public static TimeChannelReference UI =>
            TimeChannelNameUtility.From(TimeChannelNameType.UI);

        public static readonly TimeChannelReference[] All =
        {
            Game,
            UI
        };
    }
}
