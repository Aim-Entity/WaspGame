using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Game
    {
        public long Id { get; set; }

        public ICollection<Wasp> Wasps { get; set; } = new List<Wasp>();

        public bool isGameDone { get; set; } = false;

        public static Game Create()
        {
            return new Game
            {
                Wasps = new List<Wasp>
                {
                    new Queen(),
                    new Drone(), new Drone(), new Drone(), new Drone(), new Drone(),
                    new Worker(), new Worker(), new Worker(), new Worker(), new Worker(), new Worker(), new Worker()
                }
            };
        }
    }
}
