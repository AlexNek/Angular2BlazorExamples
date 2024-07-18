namespace HabitTracker.Client.Models
{
   public class Habit
    {
        public Habit()
        {
            Id = Guid.NewGuid();
        }

        public string Name { get; set; } = String.Empty;

        public string Frequency { get; set; } = String.Empty;

        public string Description { get; set; } = String.Empty;

        public Habit Clone()
        {
            return (Habit)MemberwiseClone();
        }
        public Guid Id { get; init; }
    }
}
