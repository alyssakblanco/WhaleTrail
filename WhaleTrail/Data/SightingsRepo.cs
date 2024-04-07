using SQLite;
using WhaleTrail.Models;

namespace WhaleTrail.Data
{
    public class SightingsRepo
    {
        string _dbPath;
        private SQLiteConnection conn;

        public SightingsRepo(string dbPath)
        {
            _dbPath = dbPath;
        }

        public void Init()
        {
            conn = new SQLiteConnection(_dbPath);
            conn.CreateTable<Sighting>();
        }

        public List<Sighting> GetAllSightings() 
        {
            Init();
            return conn.Table<Sighting>().ToList();
        }

        public void AddSighting(Sighting sighting)
        {
            conn = new SQLiteConnection(_dbPath);
            conn.Insert(sighting);
        }
    }
}
