using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TtcMonitor
{
    public class TradeItem
    {
        public long Id { get; set; }
        public int ItemId { get; set; }
        public string Name { get; set; }
        public int MinutesAgo { get; set; }
        public DateTime PostedDate { get; set; }
        public string Location { get; set; }
        public string GuildName { get; set; }
        public float ItemPrice { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get { return Math.Round(ItemPrice * Quantity); } }

        public static TradeItem Parse(HtmlNode tr)
        {

            TradeItem item = new TradeItem();
            item.Id = long.Parse(tr.Attributes["data-on-click-link"].Value.Substring(tr.Attributes["data-on-click-link"].Value.LastIndexOf('/') + 1));

            var cells = tr.SelectNodes("td");
            for (int i = 0; i < cells.Count; i++)
            {
                var current = cells[i];
                if (i == 0)
                {
                    item.Name = TrimRemove(current.SelectSingleNode("div").InnerText);
                }
                else if (i == 2)
                {
                    var nodes = current.SelectNodes("div");
                    item.Location = TrimRemove(nodes[0].InnerText);
                    item.GuildName = TrimRemove(nodes[1].InnerText);
                }
                else if (i == 3)
                {
                    var nodes = current.ChildNodes;
                    var perItem = TrimRemove(nodes[2].InnerText.Replace(",", "").Replace(".", ","));
                    var quantityItem = TrimRemove(nodes[6].InnerText);
                    float price;
                    if (float.TryParse(perItem, System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture, out price))
                        item.ItemPrice = price;

                    item.ItemPrice = float.Parse(perItem);
                    item.Quantity = int.Parse(quantityItem);

                }
                else if (i == 4)
                {
                    item.MinutesAgo = int.Parse(current.Attributes["data-mins-elapsed"].Value);
                    item.PostedDate = DateTime.Now.AddMinutes(-item.MinutesAgo);
                }
            }

            return item;
        }

        private static string TrimRemove(string s)
        {
            return s.Replace("\r\n", "").Replace("&#39;", "'").Trim();
        }

        public static List<TradeItem> ParsePage(string html)
        {
            var list = new List<TradeItem>();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes("//table[@class='trade-list-table max-width']/tr[@data-on-click-link]");
            return nodes.Select(s => Parse(s)).ToList();
        }


        public override string ToString()
        {
            return string.Format("{0} | {1} | {2} > {3} | {4:F2} x {5} = {6} | {7} minutes ago", Id, Name, Location, GuildName, ItemPrice, Quantity, TotalPrice, MinutesAgo);
        }

    }
}
