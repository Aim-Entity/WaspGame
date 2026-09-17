using Domain;
using Domain.Entities;

namespace Application.Utils
{
    public static class GameLogic
    {
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

        public static bool RecoverWasps(this Game game, DateTimeOffset now)
        {
            if (game.IsGameDone)
            {
                return false;
            }

            var recovered = false;

            foreach (var wasp in game.Wasps)
            {
                recovered |= wasp.RecoverIfDue(now);
            }

            return recovered;
        }

        public static bool Refresh(this Game game, DateTimeOffset now)
        {
            var wasDone = game.IsGameDone;
            var recovered = game.RecoverWasps(now);

            game.UpdateGameDone();

            return recovered || wasDone != game.IsGameDone;
        }

        public static ZapOutcome Zap(this Game game, Random random, DateTimeOffset now)
        {
            var changed = game.RecoverWasps(now);

            if (game.IsGameDone)
            {
                return new ZapOutcome(null, changed);
            }

            var targets = game.Wasps.Where(w => !w.IsKnocked).ToList();

            if (targets.Count == 0)
            {
                return new ZapOutcome(null, changed);
            }

            var hit = targets[random.Next(targets.Count)];

            changed |= hit.TakeDamage(GameRules.ZapDamage, GameRules.KnockoutDuration, now);

            game.UpdateGameDone();

            return new ZapOutcome(hit, changed);
        }

        public static void UpdateGameDone(this Game game)
        {
            game.IsGameDone = game.Wasps.Any(w => w is Queen && w.IsKnocked) || game.Wasps.All(w => w.IsKnocked);
        }
    }

    public readonly record struct ZapOutcome(Wasp? Hit, bool StateChanged);
}
