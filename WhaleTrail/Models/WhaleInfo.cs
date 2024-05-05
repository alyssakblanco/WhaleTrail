using SQLite;

namespace WhaleTrail.Models
{
    public class WhaleInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Species { get; set; }

        public string Sex { get; set; }

        public string Img { get; set; }
        
        public string Bio { get; set; }
    }
}
