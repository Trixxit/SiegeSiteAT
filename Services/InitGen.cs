using Bogus;
using Microsoft.JSInterop;
using System;
using System.Text.Json;
using static FPSHome.Services.InitializationService;
using static System.Net.WebRequestMethods;

namespace FPSHome.Services
{
    public partial class InitializationService
    {
        private static readonly List<string> Maps = new List<string>
        {
            "Bank", "Lair", "Nighthaven Labs", "Chalet", "Clubhouse", "Coastline", "Consulate",
            "Oregon", "Outback", "Skyscraper", "Emerald Plains", "Theme Park", "Villa", "Kafe Dostoyevsky", "Kanal"
        };

        private static Dictionary<string, Operator> OperatorsData;

        private class Operator
        {
            public bool attacker { get; set; }
            public string affiliation { get; set; }
            public int yearintroduced { get; set; }
            public List<string> weaponsused { get; set; }
            public string Name { get; set; }
        }

        public async Task GenerateUserData(int numUsers)
        {
            var json = await _httpClient.GetStringAsync("/JSON/rsix.json");
            OperatorsData = JsonSerializer.Deserialize<Dictionary<string, Operator>>(json);
            var faker = new Faker();

            // Calculate total number of KDs to be generated
            var totalKDs = 0;
            var userKDs = new Dictionary<string, int>();
            for (int i = 0; i < numUsers; i++)
            {
                var hoursPlayed = faker.Random.Int(20, 500);
                var numKds = (int)(hoursPlayed * (faker.Random.Int(1, 3) + (faker.Random.Double() * 10 / 10)));
                totalKDs += numKds;
                userKDs.Add(faker.Internet.UserName(), numKds);
            }

            var kdsGenerated = 0;
            foreach (var kvp in userKDs)
            {
                var username = kvp.Key;
                var numKds = kvp.Value;

                var user = new User
                {
                    Password = faker.Internet.Password(),
                    TotalKills = 0,
                    TotalDeaths = 0,
                    Kds = new List<KD>(),
                    Favourites = GenerateFavourites(),
                    HoursPlayed = faker.Random.Int(20, 500)
                };

                for (int j = 0; j < numKds; j++)
                {
                    var kd = GenerateKD();
                    user.Kds.Add(kd);
                    user.TotalKills += kd.Kills.Count;
                    user.TotalDeaths += kd.Deaths;
                    kdsGenerated++;
                    OnProgressChanged?.Invoke((int)(((double)kdsGenerated / totalKDs) * 100));
                }

                Userdata.TryAdd(username, user);
                await _jsRuntime.InvokeVoidAsync("logMessage", $"Generated user: {username}");
            }

            return;
        }

        private static KD GenerateKD()
        {
            var faker = new Faker();
            var operatorName = faker.PickRandom<string>(OperatorsData.Keys);
            var mapName = faker.PickRandom(Maps);
            var killsCount = faker.Random.Int(0, 10);
            var deaths = faker.Random.Int(0, 8);

            var killsList = new List<Kill>();
            for (int i = 0; i < killsCount; i++)
            {
                var weapon = faker.PickRandom(OperatorsData[operatorName].weaponsused);
                var attacking = OperatorsData[operatorName].attacker;
                killsList.Add(new Kill { Weapon = weapon, Attacking = attacking });
            }

            return new KD
            {
                Op = operatorName,
                Deaths = deaths,
                Kills = killsList,
                Map = mapName,
                HoursPlayedAtSubmission = Math.Round(faker.Random.Double(0.5, 5), 1)
            };
        }

        private static Favourites GenerateFavourites()
        {
            var faker = new Faker();
            return new Favourites
            {
                Operators = faker.PickRandom(OperatorsData.Keys, 3).ToList(),
                Maps = faker.PickRandom(Maps, 3).ToList()
            };
        }
    }
}
