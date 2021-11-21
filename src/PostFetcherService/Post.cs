using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace PostFetcherService
{
    public class Post
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string Link { get; set; }

        public DateTime PublishDate { get; set; }

        public static async Task<List<Post>> FetchLast5Posts(HttpClient client)
        {
            // Get RSS feed from Wired.com
            var result = await client.GetAsync("https://wired.com/feed/rss");

            if(result.IsSuccessStatusCode)
            {
                string rss = result.Content.ReadAsStringAsync().Result;

                // Load the result for parsing
                XmlDocument doc = new();
                doc.LoadXml(rss);
                // Get first 5 posts
                var posts = doc.DocumentElement.SelectNodes("//item[position() < 6]");

                // Build a new list from posts
                List<Post> _list = new();
                for (int i = 0; i < posts.Count; i++)
                {
                    Post _post = new()
                    {
                        Title = posts[i].SelectSingleNode("./title").InnerText,
                        Link = posts[i].SelectSingleNode("./link").InnerText,
                        Description = posts[i].SelectSingleNode("./description").InnerText,
                        PublishDate = DateTime.Parse(posts[i].SelectSingleNode("./pubDate").InnerText)
                    };
                    _list.Add(_post);
                }
                return _list;
            }

            return null;
        }
    }
}
