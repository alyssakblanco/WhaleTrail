using SQLite;

namespace WhaleTrail.Models
{
    public class Sighting
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public double Lat { get; set; }

        public double Long { get; set; }
    }
}
