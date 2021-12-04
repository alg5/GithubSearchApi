using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GithubSearchApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
 
    }
    public sealed class CurrentUser : User
    {
        private static CurrentUser currentUser = null;
        public static CurrentUser GetInstance
        {
            get
            {
                if (currentUser == null)
                {
                    currentUser = new CurrentUser();
                }    
                    
                return currentUser;
            }
        }


    }
}
