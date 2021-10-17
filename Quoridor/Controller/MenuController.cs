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
            }
        };

        private readonly IGameStarter gameStarter;
        private readonly GameController gameController;
        private readonly IInputReader inputReader;
        private readonly MenuView menuView;

        public MenuController(IGameStarter gameStarter, GameController gameController, IInputReader inputReader)
        {
            this.gameStarter = gameStarter;
            this.gameController = gameController;
            this.inputReader = inputReader;
            menuView = new MenuView();
        }

        public void StartNewGame()
        {
            menuView.PrintOptions(Options);
            var option = WaitForOption();
            var gameOptions = GetOptionsFor(option);
            gameStarter.StartNewGame(gameOptions);
            gameController.StartGame();
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

        private GameOptions GetOptionsFor(OptionItem option)
        {
            if (option == Options.Items[0])
            {
                return new GameOptions(GameMode.VersusPlayer, BotDifficulty.Easy);
            }
            return new GameOptions(GameMode.VersusBot, BotDifficulty.Easy);
        }
    }
}
