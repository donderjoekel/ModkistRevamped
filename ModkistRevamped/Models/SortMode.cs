using System.ComponentModel;

namespace TNRD.Modkist.Models;

public enum SortMode
{
    [Description("Alphabetical")] Alphabetical,
    [Description("Most Popular")] MostPopular,
    [Description("Most Downloads")] MostDownloads,
    [Description("Most Subscribers")] MostSubscribers,
    [Description("Recently Added")] RecentlyAdded,
    [Description("Last Updated")] LastUpdated
}
