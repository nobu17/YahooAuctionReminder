using System;
using System.Collections.Generic;
using System.Linq;
using YahooAuctionRemainder.Data;

namespace YahooAuctionRemainder.Services
{
    public interface IYahooWebService
    {
        Tuple<bool, IEnumerable<AuctionInfo>> LoadAuctionListFromHtml(string html);
        AuctionDetailInfo GetDetailedAuctionInfo(string html);
    }

    public class YahooWebService : IYahooWebService
    {


        public YahooWebService()
        {
        }

        #region prop



        #endregion

        /// <summary>
        /// HTMLページからオークション情報を取得します。
        /// </summary>
        /// <returns><c>true</c>読込データがあった<c>false</c>読込データがない</returns>
        /// <param name="html">Html.</param>
        public Tuple<bool, IEnumerable<AuctionInfo>> LoadAuctionListFromHtml(string html)
        {
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);
            var items = htmlDoc.DocumentNode.Descendants("li").Where(e => e.GetAttributeValue("class", "").Contains("acMdWatchListBox"));
            //有れば
            if (items.Any())
            {
                var aucList = items.Select(x => GetAucInfoFromHtmlNodes(x));

                return Tuple.Create(true, aucList);
            }

            return Tuple.Create(false, Enumerable.Empty<AuctionInfo>());
        }


        /// <summary>
        /// アイテム別の詳細ページからオークション情報を取得します。
        /// </summary>
        /// <returns>The detailed auction info.</returns>
        /// <param name="html">Html.</param>
        public AuctionDetailInfo GetDetailedAuctionInfo(string html)
        {
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);
            //var items = htmlDoc.DocumentNode.Descendants("li").Where(e => e.GetAttributeValue("class", "").Contains("ProductDetail__item"));
            //if(items.Any())
            //{
            //    var target = items.FirstOrDefault(x => x.Descendants("dt").FirstOrDefault().InnerText.Contains("終了日時"));
            //    if(target != null)
            //    {
            //        var time = target.Descendants("dd").FirstOrDefault().InnerText;
            //    }
            //}

            try
            {
                //var item = htmlDoc.DocumentNode.Descendants("span").FirstOrDefault(e => e.GetAttributeValue("class", "").Contains("decRemainingTimeDetail"));
                var item = htmlDoc.DocumentNode.Descendants("li").FirstOrDefault(e => e.GetAttributeValue("class", "").Contains("modDtlPInfo"));
                if (item != null)
                {
                    var target = item.Descendants("tr").FirstOrDefault(n => n.Descendants("td").FirstOrDefault().InnerText.Contains("終了日時"));
                    var endDate = target.Descendants("td").Skip(1).FirstOrDefault().InnerText;

                    //var endDate = item.InnerText;
                    var monthSp = endDate.Split('月');
                    var month = int.Parse(monthSp[0].Trim());
                    //月曜日の場合、複数に分割されるので結合する
                    var daySp = string.Join(string.Empty, monthSp.Skip(1)).Split('(');
                    var day = int.Parse(new string(daySp[0].Trim().Reverse().Skip(1).Reverse().ToArray()).Trim());

                    var hourSp = daySp[1].Split('時');
                    //var hs = hourSp[0].Split(')')[1].Trim();
                    var hs = hourSp[0].Trim().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Skip(1).First();
                    var hour = int.Parse(hs.Trim());
                    //var hour = int.Parse(new string(hourSp[0].Trim().SkipWhile(c => c == ')').Skip(1).ToArray()).Trim());

                    var minuteSP = hourSp[1].Split('分');
                    var minute = int.Parse(minuteSP[0].Trim());

                    //終了月が小さい場合は年をまたぎ
                    var nowYear = DateTime.Now.Year;
                    var nowMonth = DateTime.Now.Month;

                    if (month < nowMonth)
                    {
                        nowYear--;
                    }

                    var endDateTime = new DateTime(nowYear, month, day, hour, minute, 0);

                    return new AuctionDetailInfo() { AuctionEndDateTime = endDateTime };
                }
           
            }
            catch(Exception e)
            {
                throw e;
            }

            return null;
        }


        /// <summary>
        /// HTMLノードからオークションオブジェクトを生成
        /// </summary>
        /// <returns>The auc info from html nodes.</returns>
        /// <param name="rootNode">Root node.</param>
        private AuctionInfo GetAucInfoFromHtmlNodes(HtmlAgilityPack.HtmlNode rootNode)
        {
            var itemUrl = rootNode.Descendants("a").FirstOrDefault(e => e.GetAttributeValue("class", "").Contains("ptsLinkItem")).GetAttributeValue("href", "");
            var imgUrl = rootNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", "");

            var isnotBell = false;
            var alermNode = rootNode.Descendants("span").FirstOrDefault(e => e.GetAttributeValue("class", "").Contains("decWatchRmdBt")).GetAttributeValue("class", "");
            if(alermNode.EndsWith("Off") || alermNode.EndsWith("None"))
            {
                isnotBell = true;
            }
            //var isnotBell = rootNode.Descendants("span").FirstOrDefault(e => e.GetAttributeValue("class", "").Contains("decWatchRmdBt")).GetAttributeValue("class", "").EndsWith("Off"); ;
            var itemName = rootNode.Descendants("div").FirstOrDefault(x => x.GetAttributeValue("class", "").Contains("ptsWatchStatusBox")).Descendants("a").FirstOrDefault().InnerText;

            var curPrice = rootNode.Descendants("span").FirstOrDefault(e => e.GetAttributeValue("class", "").Contains("decWatchPrice")).InnerText;

            var curBidStutas = rootNode.Descendants("span").FirstOrDefault(e => e.GetAttributeValue("class", "").Contains("decWatchStatus")).InnerText;
            var leftTime = rootNode.Descendants("span").FirstOrDefault(e => e.GetAttributeValue("class", "").Contains("decWatchStatus")).ParentNode.InnerText.Split(new string[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x => x.Contains("残り"));
                
            return new AuctionInfo() { AuctionUrl = itemUrl, ImageUrl = imgUrl, CurrentPrice = curPrice, ItemTitle = itemName, IsReminder = !isnotBell, BidStutas = curBidStutas, LeftTimeString = leftTime };
        }
    }
}
