namespace Quoridor.Logic
{
    public class QuoridorModel
    {
        private ushort blueCharacterWalls;
        private ushort redCharacterWalls;

        public QuoridorModel()
        {
            CreateSimplePlayerMovesMasks();
            CreateSimplePlayerMoves();
            PutPlayersOnStartPosition();

            SetupSimpleCorridor();

            walls.ToStr(blueCharacterStart, redCharacterStart).Log();

            for (var i = 0; i < 100; i++)
            {
                MakeRandomRedMove();
                MakeRandomBlueMove();
                walls.ToStr(blueCharacter, redCharacter).Log();
            }

            walls.ToStr(blueCharacterStart, redCharacterStart).Log();
            // var a = (long)~0 >> (QuoridorModel.BitsBlockSize - 9 - 1);
            redCharacterStart.ToStr().Log();
        }

        private void PutPlayersOnStartPosition()
        {
            redCharacter = redCharacterStart;
            blueCharacter = blueCharacterStart;
        }

        private void SetupSimpleCorridor()
        {
            PlaceWall(1, 1, WallOrientation.Horizontal);
            PlaceWall(1, 3, WallOrientation.Horizontal);
            PlaceWall(1, 5, WallOrientation.Horizontal);
            PlaceWall(1, 7, WallOrientation.Horizontal);
            PlaceWall(1, 9, WallOrientation.Horizontal);
            PlaceWall(1, 11, WallOrientation.Horizontal);
            PlaceWall(1, 13, WallOrientation.Horizontal);
        }

        private void MakeRandomRedMove()
        {
            redCharacter = MakeRandomMove(redCharacter);
        }

        private void MakeRandomBlueMove()
        {
            blueCharacter = MakeRandomMove(blueCharacter);
        }

        
    }
}
