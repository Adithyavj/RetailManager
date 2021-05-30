using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMApi.Models
{
    public class ApplicationUserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        // Since a single person has more than one Role, we store Role Id and Name in a Dictionary 
        // Key value pair
        public Dictionary<string, string> Roles { get; set; } = new Dictionary<string, string>();
    }
}
