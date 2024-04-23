

namespace TaskmanAPI.Model
{
    public class Notification
    {
        public int Id { get; set; }
        public String UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool Seen { get; set; }
        public User? User { get; set; }
    }
}
