namespace Quoridor.Model
{
    using System;

    public enum Direction
    {
        Up = -17,
        Down = 17,
        Right = 1,
        Left = -1
    }

    public static class DirectionExtension
    {
        public static Direction Opposite(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Right => Direction.Left,
                Direction.Left => Direction.Right,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
    }
}
