namespace WebScrapper
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string url = "https://ineichen.com/auctions/past/";
            var client = new HttpClient();
            string html = await client.GetStringAsync(url);

            var scraper = new WebScraper();
            var items = scraper.ExtractItems(html);


           
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }

            var db = new DatabaseHelper("Server=localhost\\SQLEXPRESS;Database=Ineichen;Trusted_Connection=True;");

            db.SaveItems(items);
        }
    }


}

