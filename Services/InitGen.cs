using Bogus;
using Bogus.DataSets;
using Microsoft.JSInterop;
using System;
using System.Text.Json;
using static FPSHome.Services.InitializationService;

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

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public bool IsTaskRunning { get; private set; } = false;

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
            await _jsRuntime.InvokeVoidAsync("logMessage", "Generating user data...");
            var json = await _httpClient.GetStringAsync("/JSON/rsix.json");
            await _jsRuntime.InvokeVoidAsync("logMessage", "Downloaded rsix json");
            OperatorsData = JsonSerializer.Deserialize<Dictionary<string, Operator>>(json);
            await _jsRuntime.InvokeVoidAsync("logMessage", "Deserialized Operators");
            var faker = new Faker();

            int totalKDs = 0;
            var userKDs = new Dictionary<string, int>();
            for (int i = 0; i < numUsers; i++)
            {
                var hoursPlayed = faker.Random.Int(10, 50);
                var numKds = (int)(hoursPlayed * (faker.Random.Int(1, 2) + (faker.Random.Double())));
                await _jsRuntime.InvokeVoidAsync("logMessage", $"Generated KD #{i + 1} with {numKds} kills");
                totalKDs += numKds;
                userKDs.Add(faker.Internet.UserName(), numKds);
                if (_cancellationTokenSource.IsCancellationRequested)
                    return;
            }

            var kdsGenerated = 0;
            int ij = 0;
            foreach (var kvp in userKDs)
            {
                await _jsRuntime.InvokeVoidAsync("logMessage", $"Generating user {ij++}");
                var username = kvp.Key;
                var numKds = kvp.Value;

                if (_cancellationTokenSource.IsCancellationRequested)
                    return;

                List<int> nums;

                var user = new User
                {
                    Password = faker.Internet.Password(),
                    TotalKills = 0,
                    TotalDeaths = 0,
                    Kds = new List<KD>(),
                    Favourites = GenerateFavourites(),
                    HoursPlayed = faker.Random.Int(10, 100)
                };
                nums = GenerateRange(faker, 0, user.HoursPlayed, numKds);
                for (int j = 0; j < numKds; j++)
                {
                    var kd = GenerateKD(nums[j], user.HoursPlayed);
                    user.Kds.Add(kd);
                    user.TotalKills += kd.Kills.Count;
                    user.TotalDeaths += kd.Deaths;
                    kdsGenerated++;
                    int progress = (int)(((double)kdsGenerated / totalKDs) * 100);
                    await _jsRuntime.InvokeVoidAsync("logMessage", $"Progress: {progress}");
                    OnProgressChanged?.Invoke(progress);
                    await Task.Yield();
                }

                Userdata.TryAdd(username, user);
                await _jsRuntime.InvokeVoidAsync("logMessage", $"Generated user: {username}");
            }

            return;
        }

        private static List<int> GenerateRange(Faker faker, double min, double max, int amount)
        {
            if (amount < 2)
            {
                throw new ArgumentException("Amount must be at least 2 to generate a range.");
            }

            List<int> values = new List<int>();
            values.Add((int)min);
            values.Add((int)max);
            for (int i = 2; i < amount; i++)
            {
                double fraction = (double)i / (amount - 1);
                double spread = fraction + (faker.Random.Double() - 0.5) * 0.2;
                double value = min + (max - min) * spread;
                value = Math.Clamp(value, min, max);
                values.Add((int)value);
            }
            values.Sort();
            return values;
        }

        private static KD GenerateKD(double hoursplayed, double maxHours)
        {
            var faker = new Faker();
            var operatorName = faker.PickRandom<string>(OperatorsData.Keys);
            var mapName = faker.PickRandom(Maps);
            var killsCount = Math.Round(Generate(faker, 0, 15, Normalize(hoursplayed, maxInput: maxHours)));
            int deaths = 8 - (int)Math.Round(Generate(faker, 0, 8, Normalize(hoursplayed, maxInput: maxHours, maxOutput:8)));

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
                HoursPlayedAtSubmission = hoursplayed
            };
        }

        public static double Generate(Faker faker, double min, double max, double weight)
        {
            double randomValue = faker.Random.Double(0, 1);
            double weightedValue = 1 - Math.Pow(1 - randomValue, weight);
            return min + (max - min) * weightedValue;
        }

        public static double Normalize(double value, double minInput = 1, double maxInput = 50, double minOutput = 1, double maxOutput = 15)
        {
            return (value - minInput) / (maxInput - minInput) * (maxOutput - minOutput) + minOutput;
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