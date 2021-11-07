namespace Quoridor.Controller.Flow
{
    using Game;
    using Io;
    using Options;

    public class MenuController
    {
        private IOption[] Options { get; } =
        {
            new VersusPlayer("vs player"),
            new VersusBot("vs bot"),
            new BotVersusBot("Bot vs bot"),
            new Black("black"),
            new White("white"),
        };

        private readonly IInputReader inputReader;

        public MenuController(IInputReader inputReader)
        {
            this.inputReader = inputReader;
        }

        public GameOptions GetGameOptions()
        {
            var option = WaitForOption();
            return option.ToGameOptions();
        }

        private IOption WaitForOption()
        {
            while (true)
            {
                var input = inputReader.ReadInput();
                if (TryGetOption(input, out var option))
                {
                    return option;
                }
            }
        }

        private bool TryGetOption(string input, out IOption selectedOption)
        {
            foreach (var option in Options)
            {
                if (option.IsSelected(input.Trim()))
                {
                    selectedOption = option;
                    return true;
                }
            }
            selectedOption = null;
            return false;
        }
    }
}
