namespace Quoridor.Controller.Flow.Options
{
    using Game;

    public class VersusPlayer : Option
    {
        protected override string Input => "pvp";

        public VersusPlayer(string name) : base(name)
        {
        }

        public override GameOptions ToGameOptions()
        {
            return new GameOptions(GameMode.VersusPlayer, StartColor.None);
        }
    }
}
