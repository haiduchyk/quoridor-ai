namespace Quoridor.Controller.Flow.Options
{
    using System;
    using Game;

    public interface IOption
    {
        public string Name { get; }

        public bool IsSelected(string input);

        public GameOptions ToGameOptions();
    }

    public abstract class Option : IOption
    {
        public string Name { get; }

        protected abstract string Input { get; }

        public Option(string name)
        {
            Name = name;
        }

        public bool IsSelected(string input)
        {
            return string.Equals(Input, input, StringComparison.CurrentCultureIgnoreCase);
        }

        public abstract GameOptions ToGameOptions();
    }
}
