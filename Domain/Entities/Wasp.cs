using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public abstract class Wasp
    {
        protected Wasp(int initialEnergy)
        {
            Energy = initialEnergy;
        }

        public long Id { get; set; }
        public int Energy { get; set; }
        public bool IsKnocked { get; set; } = false;
    }

    public sealed class Queen : Wasp
    {
        public Queen() : base(100) { }   
    }

    public sealed class Drone : Wasp
    {
        public Drone() : base(60) { }
    }

    public sealed class Worker : Wasp
    {
        public Worker() : base(75) { }
    }
}
