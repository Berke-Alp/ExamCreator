using ExamCreator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ExamCreator.Controllers
{
    public class AccountController : Controller
    {
        private JsonResult Error(string message) => Json(new
        {
            status = "error",
            message
        });


        public IActionResult Index()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("username")))
            {
                ViewBag.Exams = DBOps.GetExams();
                return View();
            }
            else return Redirect("/");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }

        public IActionResult CreateExam()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("username")))
            {
                ViewBag.Posts = DBOps.GetPostsWithOutExams();
                return View();
            }
            else return Redirect("/");
        }

        [HttpPost]
        public IActionResult CreateExam([FromBody] CreateExamForm form)
        {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("username")))
                return Forbid();

            if (form.Questions.Count != 4) return Error("Soru sayısı 4 olmalıdır.");

            Post p = DBOps.GetPost(form.PostId);
            Exam e = DBOps.GetExam(form.PostId);

            if (p == null) return Error("Seçilen yazı veritabanında bulunamadı.");
            if (e != null) return Error("Seçilen yazı hakkında zaten bir sınav oluşturulmuş.");

            Exam exam = new()
            {
                Id = p.Id,
                Content = p.Content,
                CreatedAt = DateTime.Now,
                Title = p.Title,
                QnA = JsonConvert.SerializeObject(form.Questions)
            };

            DBOps.CreateExam(exam);

            return Json(new
            {
                status = "ok",
            });
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginForm form)
        {
            var verify = DBOps.CheckUser(form.Username, form.Password, out User u);

            if (verify)
            {
                HttpContext.Session.SetInt32("id", u.Id);
                HttpContext.Session.SetString("username", u.Username);
                HttpContext.Session.SetString("name", u.Name);
                HttpContext.Session.SetString("surname", u.Surname);
            }

            return Json(new
            {
                status = verify ? "ok" : "error",
                message = verify ? "Başarıyla giriş yaptınız, panele yönlendiriliyorsunuz..." : "Kullanıcı adı veya şifre yanlış."
            });
        }

        public IActionResult Login()
        {
            return View("~/Views/Home/Index.cshtml");
        }
    }

    public class CreateExamForm
    {
        [JsonProperty("postId")]
        public int PostId { get; set; }

        [JsonProperty("questions")]
        public List<Question> Questions { get; set; }
    }

    public class LoginForm
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
