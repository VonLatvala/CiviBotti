﻿using System;
using System.Collections.Generic;

namespace CiviBotti {
    public class UserData
    {
        private static readonly List<UserData> Users = new List<UserData>();

        public long Id;
        public string SteamId;
        public string AuthKey;

        private string _name = string.Empty;
        public string Name {
            get {
                if (_name == string.Empty) {
                    _name = Program.Bot.GetChat(Id)?.Username;
                }
                return _name;
            }
        }

        public List<SubData> Subs = new List<SubData>();

        public bool InsertDatabase(bool open) {
            var result = false;

            var sql = $"INSERT INTO users (id, steamid, authkey) values ({Id}, '{SteamId}', '{AuthKey}')";
            Console.WriteLine(sql);
            var rows = Program.Database.ExecuteNonQuery(sql);

            if (rows == 1) {
                result = true;
            }
                
            return result;
        }

        public static bool CheckDatabase(long id) {
            var sql = $"SELECT * FROM users WHERE id = {id}";
            var reader = Program.Database.ExecuteReader(sql);
            var result = reader.HasRows;
            reader.Close();
            return result;
        }

        public static UserData Get(long id) {
            var check = Users.Find(_ => _.Id == id);
            if (check != null) return check;

            var sql = $"SELECT * FROM users WHERE id = {id}";
            Console.WriteLine(sql);
            var reader = Program.Database.ExecuteReader(sql);


            UserData user = null;
            if (reader.Read()) {
                user = new UserData {
                    Id = reader.GetInt64(0),
                    SteamId = reader.GetString(1),
                    AuthKey = reader.GetString(2),
                    Subs = SubData.Get(reader.GetInt64(0))
                };
            }

            reader.Close();
            return user;
        }
        
        public static UserData GetBySteamId(string steamid) {
            var sql = $"SELECT * FROM users WHERE steamid = '{steamid}'";
            var reader = Program.Database.ExecuteReader(sql);


            UserData user = null;
            if (reader.Read()) {
                var check = Users.Find(_ => _.Id == reader.GetInt64(0));
                if (check != null) {
                    user = check;
                } else {
                    user = new UserData {
                        Id = reader.GetInt64(0),
                        SteamId = reader.GetString(1),
                        AuthKey = reader.GetString(2),
                        Subs = SubData.Get(reader.GetInt64(0))
                    };
                }
            }

            reader.Close();
            return user;
        }

        public override string ToString() => $"User: {Name}";
    }
}