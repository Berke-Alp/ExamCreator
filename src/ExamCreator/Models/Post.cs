using System;

namespace ExamCreator.Models
{
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

        public DateTime PublishDate { get; set; }
    }
}
