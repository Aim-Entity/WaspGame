namespace Domain.Entities
{
    public class Game
    {
        public long Id { get; set; }

        public ICollection<Wasp> Wasps { get; set; } = new List<Wasp>();

        public bool IsGameDone { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public int KnockoutSeconds => GameRules.KnockoutSeconds;
    }
}
