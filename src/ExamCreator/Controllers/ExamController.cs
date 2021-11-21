using ExamCreator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ExamCreator.Controllers
{
    public class ExamController : Controller
    {
        private JsonResult Error(string message) => Json(new
        {
            status = "error",
            message
        });

        [HttpGet("Exam/{id}")]
        public IActionResult Index(int id)
        {
            Exam exam = DBOps.GetExam(id);
            if (exam == null) return Json(new
            {
                status = "error",
                message = "Bu numara ile eşleşen bir sınav bulunamadı."
            });

            return View(exam);
        }

        [HttpDelete]
        public IActionResult Delete(int examId)
        {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("username")))
                return Forbid();

            Exam exam = DBOps.GetExam(examId);
            if (exam == null) return Error("Sınav bulunamadı");

            DBOps.DeleteExam(exam.Id);

            return Json(new
            {
                status = "ok"
            });
        }

        [HttpPost]
        public IActionResult Finish([FromBody] FinishForm form)
        {
            Exam exam = DBOps.GetExam(form.ExamId);
            if (exam == null) return Error("Sınav bulunamadı.");

            List<Question> questions = JsonConvert.DeserializeObject<List<Question>>(exam.QnA);
            if (form.Answers.Length != questions.Count) return Error("Tüm soruları cevaplamalısınız.");

            bool[] answers = new bool[questions.Count];

            for (int i = 0; i < questions.Count; i++)
                answers[i] = questions[i].RightAnswer == form.Answers[i];

            return Json(new {
                status = "ok",
                answers
            });
        }
    }

    public class FinishForm
    {
        public int ExamId { get; set; }

        public int[] Answers { get; set; }
    }
}
