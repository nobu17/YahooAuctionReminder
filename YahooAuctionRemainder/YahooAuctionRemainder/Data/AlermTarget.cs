using System;
namespace YahooAuctionRemainder.Data
{
    public class AlermTarget
    {
        public AlermTarget()
        {
        }

        public AlermTarget(AuctionInfo info)
        {
            if(info != null)
            {
                AuctionId = info.AuctionId;
                AuctionUrl = info.AuctionUrl;
                ItemTitle = info.ItemTitle;
                AuctionEndDateTime = info.AuctionDetail.AuctionEndDateTime;
            }
        }

        public string AuctionId
        {
            get;
            set;
        }


        public string ItemTitle
        {
            get;
            set;
        }


        public string AuctionUrl
        {
            get;
            set;
        }

        public DateTime AuctionEndDateTime
        {
            get;
            set;         
        }

        public int Id
        {
            get;
            set;
        }
    }
}
