using System.Collections.Generic;

namespace OAuthServer.Models
{
    public class User
    {
        public string UserName { get; set; }
        
        public string Password { get; set; }
        
    }
    public static class Users
    {
        public static List<User> GetAll()
        {
            return new List<User>{
                new User
                {
                    UserName = "kdelac",
                    Password = "Passw0rd"
                }
            };
        }
    }
}