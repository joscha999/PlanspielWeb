using DAL;
using DAL.Repositories;
using System;
using DAL.Models;
using System.IO;
using Newtonsoft.Json;
using Planspiel.Models;

namespace Tester {
    public static class Program {
        public static void Main(string[] args) {
            if (File.Exists("./test.sqlite"))
                File.Delete("./test.sqlite");

            var db = new Database("./test.sqlite");
            var ur = new UserRepository(db, db.PasswordHasher);
            var tr = new TeamRepository(db);
            var dr = new SaveDataRepository(db);

            for (int i = 0; i < 2; i++) {
                tr.AddOrIgnore(new Team { Name = "Team" + i, SteamID = i });
            }

            for (int i = 0; i < 4; i++) {
                ur.AddOrIgnore(new User {
                    Username = "User" + i,
                    Password = db.PasswordHasher.Hash("pw" + i),
                    TeamId = (i % 2) + 1
                });
            }

            for (int i = 0; i < 6; i++) {
                dr.AddOrIgnore(new SaveData {
                    SteamID = i % 2,
                    Date = new Date(0, 0, 0),
                    Profit = i * 100,
                    CompanyValue = i * 200,
                    DemandSatisfaction = i * 0.1d,
                    MachineUptime = i * 0.15d,
                    AbleToPayLoansBack = (i % 2) == 0,
                    AveragePollution = i * 0.05
                });
            }

            foreach (var user in ur.GetAll()) {
                Console.WriteLine($"Id: {user.Id}, Username: {user.Username}," +
                    $" Admin: {user.Admin}, Team: {user.Team?.Name ?? "None"}");
            }

            foreach (var team in tr.GetAll()) {
                Console.WriteLine("Team: " + team.Name);

                foreach (var member in team.Members) {
                    Console.WriteLine($"\tMember: {member.Username}");
                }

                foreach (var data in team.Data) {
                    Console.WriteLine($"\tSaveData: {JsonConvert.SerializeObject(data)}");
                }
            }

            Console.ReadLine();
        }
    }
}
