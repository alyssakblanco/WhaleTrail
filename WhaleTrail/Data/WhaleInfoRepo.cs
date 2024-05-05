using SQLite;
using WhaleTrail.Models;

namespace WhaleTrail.Data
{
    public class WhaleInfoRepo
    {
        string _dbPath;
        private SQLiteConnection conn;

        public WhaleInfoRepo(string dbPath)
        {
            _dbPath = dbPath;
            Init();
        }

        public void Init()
        {
            conn = new SQLiteConnection(_dbPath);
            conn.CreateTable<WhaleInfo>();
        }

        public bool IsIdInDatabase(int id)
        {
            if (conn.Table<WhaleInfo>().Any(whale => whale.Id == id))
            {
                Console.WriteLine($"Item with ID {id} already exists in the database.");
                return true; 
            }
            
            Console.WriteLine("No items found with the provided IDs.");
            return false;
        }

        public void AddWhale(WhaleInfo whaleInfo)
        {
            conn = new SQLiteConnection(_dbPath);
            conn.Insert(whaleInfo);
        }

        public List<WhaleInfo> GetAllWhales()
        {
            return conn.Table<WhaleInfo>().ToList();
        }
    }
}
