using Domain.Entities;

namespace Application.Utils
{
    public static class WaspLogic
    {
        public static bool TakeDamage(this Wasp wasp, int damage, TimeSpan knockoutDuration, DateTimeOffset now)
        {
            if (wasp.IsKnocked)
            {
                return false;
            }

            var previousEnergy = wasp.Energy;

            wasp.Energy = Math.Max(0, wasp.Energy - damage);

            if (wasp.Energy == 0)
            {
                wasp.IsKnocked = true;
                wasp.KnockedUntil = now.Add(knockoutDuration);
            }

            return wasp.Energy != previousEnergy;
        }

        public static void Recover(this Wasp wasp)
        {
            wasp.Energy = wasp.MaxEnergy;
            wasp.IsKnocked = false;
            wasp.KnockedUntil = null;
        }

        public static bool RecoverIfDue(this Wasp wasp, DateTimeOffset now)
        {
            if (!wasp.IsKnocked || wasp.KnockedUntil is null || wasp.KnockedUntil > now)
            {
                return false;
            }

            wasp.Recover();

            return true;
        }
    }
}
