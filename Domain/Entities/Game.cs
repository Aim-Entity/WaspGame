using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Game
    {
        public long Id { get; set; }
        public IEnumerable<Wasp> Wasps { get; set; }
        public bool isGameDone { get; set; } = false;
    }
}
