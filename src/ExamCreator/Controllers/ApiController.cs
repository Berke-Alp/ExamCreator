using ExamCreator.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using HtmlAgilityPack;
using System.Net;

namespace ExamCreator.Controllers
{
    public class ApiController : Controller
    {
        [HttpGet("Api/Post/Details/{id}")]
        public IActionResult GetPostDetail(int id)
        {
            var error = new
            {
                status = "error",
                message = "Yazı bulunamadı, sayfa yenileniyor...",
            };

            Post p = DBOps.GetPost(id);

            if(p != null)
            {
                if(string.IsNullOrWhiteSpace(p.Content))
                {
                    HttpClient client = new();
                    var result = client.GetStringAsync(p.Link).Result;

                    HtmlDocument doc = new();
                    doc.LoadHtml(result);
                    var article = doc.DocumentNode.SelectSingleNode("//article");
                    string html = "";

                    if (article != null)
                    {
                        // Parse gallery
                        if(p.Link.Contains("gallery/"))
                        {
                            var content = article.SelectSingleNode("./*[@data-attribute-verso-pattern='gallery-body']");
                            html += WebUtility.HtmlDecode(content.InnerText);
                        }
                        else if(p.Link.Contains("story/"))
                        {
                            var content = article.SelectNodes("//*[@class='body__inner-container']");
                            foreach (var node in content)
                                html += WebUtility.HtmlDecode(node.InnerText);
                        }
                        else
                        {
                            html += WebUtility.HtmlDecode(article.InnerText);
                        }
                    }
                    
                    if(string.IsNullOrWhiteSpace(html)) return Json(error);

                    if (html.Contains("More Great WIRED Stories"))
                    {
                        html = html.Substring(0, html.IndexOf("More Great WIRED Stories"));
                    }

                    p.Content = html;
                    DBOps.UpdatePost(p);
                }

                return Json(new
                {
                    status = "ok",
                    title = p.Title,
                    content = p.Content,
                    link = p.Link,
                    publishedDate = p.PublishDate
                });
            }

            return Json(error);
        }
    }
}
