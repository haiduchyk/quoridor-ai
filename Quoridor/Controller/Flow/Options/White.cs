namespace Quoridor.Controller.Flow.Options
{
    using Game;

    public class White : Option
    {
        protected override string Input => "white";

        public White(string name) : base(name)
        {
        }

        public override GameOptions ToGameOptions()
        {
            return new GameOptions(GameMode.BotVersusBot, StartColor.White);
        }
    }
}
