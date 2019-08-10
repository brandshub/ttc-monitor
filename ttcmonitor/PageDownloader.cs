using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TtcMonitor
{
    public static class PageDownloader
    {


        public static async Task<List<TradeItem>> GetDataForItem(int itemId, int pageNumber)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(@"https://eu.tamrieltradecentre.com/pc/Trade/");
                var str = await client.GetStringAsync("SearchResult?ItemID=" + itemId + "&SortBy=LastSeen&Order=desc&page=" + pageNumber);

                var list = TradeItem.ParsePage(str);
                foreach (var item in list)
                    item.ItemId = itemId;

                return list;
            }
        }
    }
}
