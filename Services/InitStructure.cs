namespace FPSHome.Services
{
    public partial class InitializationService
    {

        //public Dictionary<string, User> Userdata = new();

        public class User
        {
            public string Password { get; set; }
            public int TotalKills { get; set; }
            public int TotalDeaths { get; set; }
            public List<KD> Kds { get; set; }
            public Favourites Favourites { get; set; }
            public int HoursPlayed { get; set; }
        }

        public class KD
        {
            public string Op { get; set; }
            public int Deaths { get; set; }
            public List<Kill> Kills { get; set; }
            public string Map { get; set; }
            public double HoursPlayedAtSubmission { get; set; }
        }

        public class Kill
        {
            public string Weapon { get; set; }
            public bool Attacking { get; set; }
        }

        public class Favourites
        {
            public List<string> Operators { get; set; }
            public List<string> Maps { get; set; }
        }
    }
}
