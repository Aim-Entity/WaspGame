using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Game
    {
        public Game()
        {
            IEnumerable<Wasp> GeneratedWasps = new List<Wasp>() { 
                new Queen(),
                new Drone(), new Drone(), new Drone(), new Drone(), new Drone(),
                new Worker(), new Worker(), new Worker(), new Worker(), new Worker(), new Worker(), new Worker()
            };

            Wasps = GeneratedWasps;
        }

        public long Id { get; set; }
        public IEnumerable<Wasp> Wasps { get; set; }
        public bool isGameDone { get; set; } = false;
    }
}
