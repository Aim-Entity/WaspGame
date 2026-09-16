namespace Domain.Entities
{
    public class Game
    {
        public const int ZapDamage = 11;

        public static readonly TimeSpan KnockoutDuration = TimeSpan.FromSeconds(40);

        public long Id { get; set; }

        public ICollection<Wasp> Wasps { get; set; } = new List<Wasp>();

        public bool IsGameDone { get; set; } = false;

        /// <summary>How long a knocked out wasp stays down. The client caps its countdown at this.</summary>
        public int KnockoutSeconds => (int)KnockoutDuration.TotalSeconds;

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

        public bool RecoverWasps()
        {
            if (IsGameDone)
            {
                return false;
            }

            var recovered = false;

            foreach (var wasp in Wasps)
            {
                recovered |= wasp.RecoverIfDue();
            }

            return recovered;
        }

        public bool Refresh()
        {
            var wasDone = IsGameDone;
            var recovered = RecoverWasps();

            UpdateGameDone();

            return recovered || wasDone != IsGameDone;
        }

        public Wasp? Zap(Random random)
        {
            RecoverWasps();

            if (IsGameDone)
            {
                return null;
            }

            var targets = Wasps.Where(w => !w.IsKnocked).ToList();

            if (targets.Count == 0)
            {
                return null;
            }

            var hit = targets[random.Next(targets.Count)];

            hit.TakeDamage(ZapDamage, KnockoutDuration);

            UpdateGameDone();

            return hit;
        }

        public void UpdateGameDone()
        {
            IsGameDone = Wasps.Any(w => w is Queen && w.IsKnocked) || Wasps.All(w => w.IsKnocked);
        }
    }

    public sealed record ZapResult(Game Game, long? HitWaspId);
}
