using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Net;
using HtmlAgilityPack;

namespace PostFetcherService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient _client;
        private readonly IConfiguration _conf;

        /// <summary>
        /// Default post fetch interval is 15 minutes
        /// </summary>
        private readonly int _fetchInterval = 15 * 60 * 1000;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _conf = configuration;
            _logger = logger;
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());

            int interval = _conf.GetValue<int>("PostFetchInterval");
            if (interval > 5000) _fetchInterval = interval;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _client = new HttpClient();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _client.Dispose();
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Get post list from RSS
                var list = await Post.FetchLast5Posts(_client);

                if (list != null)
                {
                    int added = 0;
                    foreach (Post p in list)
                    {
                        // If there is no matching post in database
                        if(!PostExistsWithLink(p.Link).Result)
                        {
                            // Then get the content of the post
                            var content = await FetchContent(p.Link);
                            
                            if (content != null)
                            {
                                p.Content = content;

                                // Finally, save the fetched post to database
                                added += await SavePost(p);
                            }
                        }
                    }

                    _logger.LogInformation($"{added} posts added to database [Time: {DateTime.Now}]");
                }
                else
                    _logger.LogError("Cannot fetch recent posts from wired.com");

                await Task.Delay(_fetchInterval, stoppingToken);
            }
        }

        #region Post Operations

        /// <summary>
        /// Checks database for a post matching with the given url
        /// </summary>
        /// <param name="link">The URL link to match</param>
        /// <returns><see langword="true"/> if exists, <see langword="false"/> if not.</returns>
        public async Task<bool> PostExistsWithLink(string link)
        {
            SqliteConnection con = new(_conf.GetConnectionString("localDatabase"));

            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM posts WHERE link = @link";
            cmd.Parameters.AddWithValue("link", link);
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            con.Close();
            await con.DisposeAsync();

            return count > 0;
        }

        private static async Task<string> FetchContent(string link)
        {
            HttpClient client = new();
            var result = await client.GetStringAsync(link);

            HtmlDocument doc = new();
            doc.LoadHtml(result);
            // Take the article
            var article = doc.DocumentNode.SelectSingleNode("//article");
            string html = "";

            if (article != null)
            {
                // Parse gallery
                if (link.Contains("gallery/"))
                {
                    var content = article.SelectSingleNode("./*[@data-attribute-verso-pattern='gallery-body']");
                    html += WebUtility.HtmlDecode(content.InnerText);
                }
                // Parse story
                else if (link.Contains("story/"))
                {
                    var content = article.SelectNodes("//*[@class='body__inner-container']");
                    foreach (var node in content)
                        html += WebUtility.HtmlDecode(node.InnerText);
                }
                // Parse other type of posts
                else
                {
                    html += WebUtility.HtmlDecode(article.InnerText);
                }
            }

            if (string.IsNullOrWhiteSpace(html)) return null;

            // Remove the adversitements and relative posts
            if (html.Contains("More Great WIRED Stories"))
            {
                html = html.Substring(0, html.IndexOf("More Great WIRED Stories"));
            }

            return html;
        }

        /// <summary>
        /// Inserts given post to database
        /// </summary>
        /// <param name="p">The <see cref="Post"/></param>
        /// <returns>Rows affected</returns>
        private async Task<int> SavePost(Post p)
        {
            SqliteConnection con = new(_conf.GetConnectionString("localDatabase"));
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "INSERT INTO posts(`publishDate`,`title`,`link`,`description`,`content`) VALUES(@added, @title, @link, @desc, @content)";
            cmd.Parameters.AddWithValue("added", p.PublishDate);
            cmd.Parameters.AddWithValue("title", p.Title);
            cmd.Parameters.AddWithValue("link", p.Link);
            cmd.Parameters.AddWithValue("desc", p.Description);
            cmd.Parameters.AddWithValue("content", p.Content);
            int affected = await cmd.ExecuteNonQueryAsync();
            cmd.Dispose();
                
            con.Close();

            await con.DisposeAsync();

            
            return affected;
        }

        #endregion
    }
}
