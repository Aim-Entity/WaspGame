namespace Domain.Entities
{
    public class Game
    {
        public const int ZapDamage = 11;

        public static readonly TimeSpan KnockoutDuration = TimeSpan.FromSeconds(40);

        public long Id { get; set; }

        public ICollection<Wasp> Wasps { get; set; } = new List<Wasp>();

        public bool IsGameDone { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public int KnockoutSeconds => (int)KnockoutDuration.TotalSeconds;

        public static Game Create()
        {
            return new Game
            {
                IsActive = true,
                Wasps = new List<Wasp>
                {
                    new Queen(),
                    new Drone(), new Drone(), new Drone(), new Drone(), new Drone(),
                    new Worker(), new Worker(), new Worker(), new Worker(), new Worker(), new Worker(), new Worker()
                }
            };
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public bool RecoverWasps(DateTimeOffset now)
        {
            if (IsGameDone)
            {
                return false;
            }

            var recovered = false;

            foreach (var wasp in Wasps)
            {
                recovered |= wasp.RecoverIfDue(now);
            }

            return recovered;
        }

        public bool Refresh(DateTimeOffset now)
        {
            var wasDone = IsGameDone;
            var recovered = RecoverWasps(now);

            UpdateGameDone();

            return recovered || wasDone != IsGameDone;
        }

        public ZapOutcome Zap(Random random, DateTimeOffset now)
        {
            var changed = RecoverWasps(now);

            if (IsGameDone)
            {
                return new ZapOutcome(null, changed);
            }

            var targets = Wasps.Where(w => !w.IsKnocked).ToList();

            if (targets.Count == 0)
            {
                return new ZapOutcome(null, changed);
            }

            var hit = targets[random.Next(targets.Count)];

            changed |= hit.TakeDamage(ZapDamage, KnockoutDuration, now);

            UpdateGameDone();

            return new ZapOutcome(hit, changed);
        }

        public void UpdateGameDone()
        {
            IsGameDone = Wasps.Any(w => w is Queen && w.IsKnocked) || Wasps.All(w => w.IsKnocked);
        }
    }

    public readonly record struct ZapOutcome(Wasp? Hit, bool StateChanged);

    public sealed class ZapResult
    {
        public ZapResult(Game game, long? hitWaspId)
        {
            Game = game;
            HitWaspId = hitWaspId;
        }

        public Game Game { get; }

        public long? HitWaspId { get; }
    }
}
