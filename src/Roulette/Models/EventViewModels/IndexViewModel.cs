using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roulette.Models.EventViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<EventViewModel> Events { get; set; }

        public class EventViewModel
        {
            public bool IsCreated { get; set; }
            public Event Event { get; set; }
        }
    }
}
