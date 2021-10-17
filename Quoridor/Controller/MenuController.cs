namespace Quoridor.Controller
{
    using System.Linq;
    using Model;
    using Model.Options;
    using View;

    public class MenuController
    {
        private Options Options { get; } = new()
        {
            Items = new OptionItem[]
            {
                new() { id = 1, name = "Game versus another player" },
                new() { id = 2, name = "Game versus bot" },
                new() { id = 3, name = "Quit" },
            }
        };

        private readonly IInputReader inputReader;
        private readonly MenuView menuView;

        public MenuController(IInputReader inputReader)
        {
            this.inputReader = inputReader;
            menuView = new MenuView();
        }

        public bool TryGetGameOptions(out GameOptions gameOptions)
        {
            menuView.PrintOptions(Options);
            var option = WaitForOption();
            if (ShouldQuit(option))
            {
                gameOptions = null;
                return false;
            }
            gameOptions = GetGameOptionsFor(option);
            return true;
        }

        private OptionItem WaitForOption()
        {
            while (true)
            {
                var input = inputReader.ReadInput();
                if (TryGetOption(input, out var option))
                {
                    return option;
                }
                menuView.PrintErrorMessage();
            }
        }

        private bool TryGetOption(string input, out OptionItem optionItem)
        {
            if (int.TryParse(input.Trim(), out var number))
            {
                optionItem = Options.Items.FirstOrDefault(item => item.id == number);
                return optionItem != null;
            }
            optionItem = null;
            return false;
        }

        private bool ShouldQuit(OptionItem option)
        {
            return option == Options.Items[2];
        }

        private GameOptions GetGameOptionsFor(OptionItem option)
        {
            if (option == Options.Items[0])
            {
                return new GameOptions(GameMode.VersusPlayer, BotDifficulty.Easy);
            }
            return new GameOptions(GameMode.VersusBot, BotDifficulty.Easy);
        }
    }
}
