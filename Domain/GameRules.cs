namespace Domain
{
    public static class GameRules
    {
        public const int ZapDamage = 11;

        public static readonly TimeSpan KnockoutDuration = TimeSpan.FromSeconds(40);

        public static int KnockoutSeconds => (int)KnockoutDuration.TotalSeconds;
    }
}
