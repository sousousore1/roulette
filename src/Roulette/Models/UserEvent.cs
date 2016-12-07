using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Roulette.Models
{
    public class UserEvent
    {
        public int Id { get; set; }
        [JsonIgnore]
        public ApplicationUser User { get; set; }
        [JsonIgnore]
        public Event Event { get; set; }
    }
}
