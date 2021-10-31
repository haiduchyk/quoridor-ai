namespace Quoridor.Controller.Flow.Options
{
    using Game;

    public class Black : Option
    {
        protected override string Input => "black";

        public Black(string name) : base(name)
        {
        }

        public override GameOptions ToGameOptions()
        {
            return new GameOptions(GameMode.BotVersusBot, StartColor.Black);
        }
    }
}
