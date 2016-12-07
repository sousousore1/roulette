using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace Roulette.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public ApplicationUser Creator { get; set; }
        [JsonIgnore]
        public ApplicationUser Winner { get; set; }
        [JsonIgnore]
        public DateTime Created { get; set; }
        [JsonIgnore]
        public DateTime Updated { get; set; }
        public ICollection<UserEvent> UserEvents { get; set; }
    }
}
