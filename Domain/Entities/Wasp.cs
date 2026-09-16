namespace Domain.Entities
{
    public abstract class Wasp
    {
        protected Wasp(int initialEnergy)
        {
            Energy = initialEnergy;
            MaxEnergy = initialEnergy;
        }

        public long Id { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }
        public bool IsKnocked { get; set; } = false;

        public DateTimeOffset? KnockedUntil { get; set; }

        public string Type => GetType().Name;

        public void TakeDamage(int damage, TimeSpan knockoutDuration)
        {
            if (IsKnocked)
            {
                return;
            }

            Energy = Math.Max(0, Energy - damage);

            if (Energy == 0)
            {
                IsKnocked = true;
                KnockedUntil = DateTimeOffset.UtcNow.Add(knockoutDuration);
            }
        }

        public void Recover()
        {
            Energy = MaxEnergy;
            IsKnocked = false;
            KnockedUntil = null;
        }

        public bool RecoverIfDue()
        {
            if (!IsKnocked || KnockedUntil is null || KnockedUntil > DateTimeOffset.UtcNow)
            {
                return false;
            }

            Recover();

            return true;
        }
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
