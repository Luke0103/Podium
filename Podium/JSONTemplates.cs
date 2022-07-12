using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Podium
{
    public class PlayerInfo
    {
        [JsonProperty("realm")]
        public string Realm { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("rank")]
        public string Rank { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("bnet_id")]
        public string BattleTag { get; set; }

        [JsonProperty("race")]
        public string Race { get; set; }

        [JsonProperty("mmr")]
        public int MMR { get; set; }

        [JsonProperty("wins")]
        public int Win { get; set; }

        [JsonProperty("losses")]
        public int Loss { get; set; }

        [JsonProperty("clan")]
        public string Clan { get; set; }

        [JsonProperty("profile_id")]
        public int ProfileID { get; set; }

        public int GetRegionNum()
        {
            if (Region == "US")
                return 1;
            else if (Region == "EU")
                return 2;
            else if (Region == "KR")
                return 3;

            return -1;
        }
        public int GetRealmNum()
        {
            return int.Parse(Realm);
        }
    }

    class GameMatch
    {
        [JsonProperty("map")]
        public string Map { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("decision")]
        public string Result { get; set; }

        [JsonProperty("speed")]
        public string Speed { get; set; }

        [JsonProperty("date")]
        public int Date { get; set; }
    }

    class OAuthToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
