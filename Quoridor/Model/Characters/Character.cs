namespace Quoridor.Model.Characters
{
    public class Character
    {
        public FieldMask Position { get; }

        public int AmountOfWalls { get; }

        public Character(FieldMask position, int amountOfWalls)
        {
            Position = position;
            AmountOfWalls = amountOfWalls;
        }
    }
}
