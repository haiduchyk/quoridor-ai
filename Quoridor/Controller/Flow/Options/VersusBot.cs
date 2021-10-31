namespace Quoridor.Controller.Flow.Options
{
    using Game;

    public class VersusBot : Option
    {
        protected override string Input => "pvb";

        public VersusBot(string name) : base(name)
        {
        }

        public override GameOptions ToGameOptions()
        {
            return new GameOptions(GameMode.VersusBot, StartColor.None);
        }
    }
}
