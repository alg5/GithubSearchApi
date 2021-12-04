using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GithubSearchApi.Models
{


    public class Repo
    {
        public ulong id { get; set; }
        public string name { get; set; }
        public bool bookmarked { get; set; }
        public Owner owner { get; set; }
    }
    public class Owner
    {
        public string login { get; set; }
        public string avatar_url { get; set; }
    }
}
