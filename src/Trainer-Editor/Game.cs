namespace Hopeless
{
    class Game
    {
        public GameTrainers Trainers { get; set; }
        public GamePokemon Pokemon { get; set; }
        public GameItems Items { get; set; }
        public GameAttacks Attacks { get; set; }
    }

    class GameTrainers
    {
        public int Count { get; set; }
        public int Data { get; set; }
        public GameTrainersClasses Classes { get; set; }
        public GameTrainersSprites Sprites { get; set; }
    }

    class GameTrainersClasses
    {
        public int Count { get; set; }
        public int Data { get; set; }
        public int Names { get; set; }
    }

    class GameTrainersSprites
    {
        public int Count { get; set; }
        public int Dat { get; set; }
        public int Palettes { get; set; }
    }

    class GamePokemon
    {
        public int Count { get; set; }
        public int Names { get; set; }
        public int Sprites { get; set; }
        public int Palettes { get; set; }
    }

    class GameItems
    {
        public int Count { get; set; }
        public int Data { get; set; }
    }

    class GameAttacks
    {
        public int Count { get; set; }
        public int Names { get; set; }
    }
}
