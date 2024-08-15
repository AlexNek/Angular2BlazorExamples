namespace Netlify.ApiClient.Hero
{
    internal static class Mutations
    {
        public const string CreateHeroMutation = @"
        mutation createHero($alterEgo: String!, $realName: String!) {
            createHero(data: { alterEgo: $alterEgo, realName: $realName }) {
                id
                realName
                alterEgo
                votes
                image
                public
                user {
                    id
                    email
                }
                createdAt
                updatedAt
            }
        }
    ";

        public const string DeleteHeroMutation = @"
        mutation removeHero($heroId: String!) {
            removeHero(heroId: $heroId) {
                id
            }
        }
    ";

        public const string VoteForHeroMutation = @"
        mutation voteHero($heroId: String!) {
            voteHero(heroId: $heroId) {
                id
                realName
                alterEgo
                image
                public
                user {
                    id
                    email
                }
                createdAt
                updatedAt
            }
        }
    ";
    }
}
