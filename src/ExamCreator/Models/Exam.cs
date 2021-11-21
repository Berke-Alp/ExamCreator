using System;

namespace ExamCreator.Models
{
    public class Exam
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string QnA { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
