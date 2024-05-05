using SQLite;

namespace WhaleTrail.Models
{
    public class Sighting
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Date { get; set; }

        public double Lat { get; set; }

        public double Long { get; set; }
    }
}
