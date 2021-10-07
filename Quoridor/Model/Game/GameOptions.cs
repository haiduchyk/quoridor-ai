namespace Quoridor.Model
{
    public class GameOptions
    {
        public readonly GameMode gameMode;
        public readonly BotDifficulty botDifficulty;

        public GameOptions(GameMode gameMode, BotDifficulty botDifficulty)
        {
            this.gameMode = gameMode;
            this.botDifficulty = botDifficulty;
        }
    }

    public enum BotDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public enum GameMode
    {
        VersusBot,
        VersusPlayer
    }
}
