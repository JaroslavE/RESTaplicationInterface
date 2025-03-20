using System.Diagnostics.Metrics;
using System.Net;
using System.Text.Json;
using Newtonsoft.Json;

namespace RESTaplicationInterface
{
    internal class Program
    {
        public static readonly string baseUrl = "https://jsonmock.hackerrank.com/api/events";
        static void Main(string[] args)
        {
            VypisEventy("Japan", 1713205800000);
        }

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        public static string buildURL(string url, string country)
        {
            return url + "?country=" + country;
        }

        public static string buildURL(string url, string country, int page)
        {
            return url + "?country=" + country + "&page=" + page;
        }

        public static void VypisEventy(string country, long dateTimeMillis)
        {
            List<Page> pages = new List<Page>();
            pages.Add(getPage(buildURL(baseUrl, country)));
            if(pages.Count > 0)
            {
                if (pages[0].total_pages>1)
                {
                    for(int i = 2; i <= pages[0].total_pages; i++) pages.Add(getPage(buildURL(baseUrl, country, i)));
                }
            }


            Dictionary<DateTime,string> data = new Dictionary<DateTime,string>();
            foreach (Page page in pages) 
            {
                foreach(Event udalost in page.data)
                {
                    data.Add(udalost.Date,udalost.Name);
                }
            }

            DateTime dateTime = UnixTimeStampToDateTime(dateTimeMillis);
            List<DateTime> dates = new List<DateTime>();


            foreach (DateTime eventDate in data.Keys)
            {
                if (DateTime.Compare(eventDate, dateTime) >= 0) dates.Add(eventDate);
            }
            dates.Sort();

            Console.WriteLine(data[dates[0]]);
            Console.WriteLine(data[dates[dates.Count-1]]);
        }

       
        //Console.WriteLine();

        public static Page getPage(string url)
        {
            var json = new WebClient().DownloadString(url);
            Page page = JsonConvert.DeserializeObject<Page>(json);
            return page;
        }
        
    }

    public class Page
    {
        public int page { get; set; }
        public int per_page { get; set; }
        public int total { get; set; }
        public int total_pages { get; set; }
        public List<Event> data { get; set; } = new List<Event>();
    }
    public class Event
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Country { get; set; }
    }
}
