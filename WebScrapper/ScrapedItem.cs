using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper
{
    internal  class ScrapedItem
    {
        public string Title { get; set; }
        public string Description { get; set; } 
        public string ImageUrl { get; set; }
        public string Link { get; set; }
        public int? LotCount { get; set; }
        public int StartDate { get; set; }
        public string StartMonth { get; set; }
        public int StartYear { get; set; }
        public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
        public int EndDate { get; set; }
        public string EndMonth { get; set; }
        public int EndYear { get; set; }
        public TimeSpan EndTime { get; set; } = TimeSpan.Zero;
        public string Location { get; set; }

        public override string ToString()
        {
            return $"Title: {Title}, Link: {Link}, LotCount: {LotCount}, Start: {StartDate}-{StartMonth}-{StartYear} {StartTime}, End: {EndDate}-{EndMonth}-{EndYear} {EndTime}, Location: {Location}";
        }

    }


}
