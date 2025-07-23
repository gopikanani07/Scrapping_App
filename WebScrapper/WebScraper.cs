using HtmlAgilityPack;

using System.Text.RegularExpressions;


namespace WebScrapper
{
    internal class WebScraper
    {
        private static readonly HttpClient client = new HttpClient();



        public async Task<string> GetPageHtmlAsync(string url)
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

       

        public List<ScrapedItem> ExtractItems(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var items = new List<ScrapedItem>();

            var auctionNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'auction-item')]");
            if (auctionNodes == null) return items;

            foreach (var node in auctionNodes)
            {
                // Title 
                var titleNode = node.SelectSingleNode(".//h2[@class='auction-item__name']/a");
                var titleText = titleNode?.InnerText.Trim();

               

                // Image URL
                var imgSrc = node.SelectSingleNode(".//a[@class='auction-item__image']/img")?.GetAttributeValue("src", "");
                var fullImageUrl = "https://ineichen.com" + imgSrc;

                // Link
                var link = node.SelectSingleNode(".//a[@class='auction-item__image']")?.GetAttributeValue("href", "");
                var fullLink = "https://ineichen.com" + link;

                // Lot Count
                var lotText = node.SelectSingleNode(".//div[@class='auction-item__btns']/a")?.InnerText.Trim(); 

                int? lotCount = null;

                if (!string.IsNullOrEmpty(lotText))
                {
                    var match = Regex.Match(lotText, @"\d+");
                    if (match.Success && int.TryParse(match.Value, out int parsedCount))
                    {
                        lotCount = parsedCount;
                    }
                    else
                    {
                        lotCount = 0;
                    }
                }
               


                // Location
                var locationNode = node.SelectSingleNode(".//i[contains(@class, 'mdi-map-marker-outline')]/following-sibling::span/a");
                var location = locationNode?.InnerText.Trim();


                // Auction Date Range
                int startDayInt = 0, endDayInt = 0, startYearInt = 0, endYear = 0;
                string? startMonth = "", endMonth = "";
                TimeSpan startTime =TimeSpan.Zero;
                TimeSpan endTime = TimeSpan.Zero;
               
                var dateRange = node.SelectSingleNode(".//i[contains(@class, 'mdi-clock-outline')]/following-sibling::b");

                var timeNode = dateRange?.ParentNode?.SelectSingleNode("./span");
                string timeOnly = "";

                if (!string.IsNullOrWhiteSpace(timeNode?.InnerText))
                {
                    var rawTime = timeNode.InnerText.Trim(); 
                    timeOnly = rawTime.Split(' ')[0];

                    if (TimeSpan.TryParse(timeOnly, out var parsedTime))
                    {
                        startTime = parsedTime;
                        endTime = parsedTime;
                    }
                }




                var date = dateRange?.InnerText.Trim();

                if (!string.IsNullOrWhiteSpace(date))
                {
                    var monthMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["JANUARY"] = "01",
                        ["FEBRUARY"] = "02",
                        ["MARCH"] = "03",
                        ["APRIL"] = "04",
                        ["MAY"] = "05",
                        ["JUNE"] = "06",
                        ["JULY"] = "07",
                        ["AUGUST"] = "08",
                        ["SEPTEMBER"] = "09",
                        ["OCTOBER"] = "10",
                        ["NOVEMBER"] = "11",
                        ["DECEMBER"] = "12"
                    };

                    var parts = date.Split(new[] { '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                 
                    if(parts.Length == 2)
                    {
                        int.TryParse(parts[0], out startDayInt);
                        int.TryParse(parts[0], out endDayInt);

                        startMonth = parts[1];
                        endMonth=parts[1];

                    }
                    else if (parts.Length == 3)
                    {
                        //  "7 - 10 MAY"
                        int.TryParse(parts[0], out startDayInt);
                        int.TryParse(parts[1], out endDayInt);
                      
                            startMonth = endMonth = parts[2];
                    
                    }
                    else if (parts.Length == 4)
                    {
                        //  "9 - 16 OCTOBER 2024"
                        int.TryParse(parts[0], out startDayInt);
                        int.TryParse(parts[1], out endDayInt);
                       
                            startMonth = endMonth = parts[2];
                            int.TryParse(parts[3], out startYearInt);
                            endYear = startYearInt;
                        
                    }
                    else if (parts.Length == 5)
                    {
                        //  "21 AUGUST - 4 SEPTEMBER 2024"
                        int.TryParse(parts[0], out startDayInt);
                        startMonth = parts[1];


                        int.TryParse(parts[2], out endDayInt);
                        endMonth = parts[3];

                        int.TryParse(parts[4], out startYearInt);
                        endYear = startYearInt;
                    }
                }


                var item = new ScrapedItem
                {
                    Title = titleText,
                    ImageUrl = fullImageUrl,
                    Link = fullLink,
                    LotCount = lotCount,
                    StartDate = startDayInt,
                    StartMonth = startMonth,
                    StartYear = startYearInt,
                    StartTime=startTime,
                    EndTime=endTime,
                    EndDate = endDayInt,
                    EndMonth = endMonth,
                    EndYear = endYear,
                    Location = location

                };
              
                items.Add(item);
            }
            
            items = items
            .GroupBy( i=> i.Link)
            .Select(g => g.First())
            .ToList();

            return items;
        }

    }
}
