using System;
namespace YahooAuctionRemainder.DService
{
    public class LocalNotifyData
    {
        public LocalNotifyData()
        {

        }

        public string Key
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Body
        {
            get;
            set;
        }

        public DateTime ReserveDate
        {
            get;
            set;
        }
    }
}
