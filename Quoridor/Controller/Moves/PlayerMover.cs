namespace Quoridor.Controller.Moves
{
    using System;
    using Io;
    using Model;
    using Model.Moves;
    using Model.Players;

    public class PlayerMover
    {
        public bool ShouldPrint => !player.ShouldWaitForMove();

        private readonly Field field;
        private readonly Player player;
        private readonly IMoveConverter moveConverter;
        private readonly IIoWorker ioWorker;

        public PlayerMover(Field field, Player player, IMoveConverter moveConverter, IIoWorker ioWorker)
        {
            this.field = field;
            this.player = player;
            this.moveConverter = moveConverter;
            this.ioWorker = ioWorker;
        }

        public (IMove move, string code) WaitForMove()
        {
            var move = player.ShouldWaitForMove() ? ReadMoveFromConsole() : player.FindMove(field);
            var code = moveConverter.GetCode(field, player, move);
            return (move, code);
        }

        private IMove ReadMoveFromConsole()
        {
            var input = ioWorker.ReadInput();
            return moveConverter.ParseMove(field, player, input);
        }
    }
}
