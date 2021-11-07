namespace Quoridor.Controller.Flow.Options
{
    using Game;

    public class BotVersusBot : Option
    {
        protected override string Input => "bvb";

        public BotVersusBot(string name) : base(name)
        {
        }

        public override GameOptions ToGameOptions()
        {
            return new GameOptions(GameMode.BotVersusBot, StartColor.None);
        }
    }
}
