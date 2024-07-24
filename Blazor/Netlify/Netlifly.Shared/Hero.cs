namespace Netlifly.Shared
{
    public class Hero
    {
        public string AlterEgo { get; set; }

        public string Id { get; set; }

        public string? Image { get; set; }

        public bool Public { get; set; }

        public string RealName { get; set; }

        public Hero[]? UsersVoted { get; set; }

        public Hero(Hero? hero)
        {
            if (hero != null)
            {
                Id = hero.Id;
                RealName = hero.RealName;
                AlterEgo = hero.AlterEgo;
                Image = hero.Image;
                Public = hero.Public;
                UsersVoted = hero.UsersVoted;
            }
        }
    }
}
