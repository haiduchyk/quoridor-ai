namespace Quoridor.View
{
    using System;
    using Model.Options;

    public class MenuView
    {
        public void PrintOptions(Options options)
        {
            foreach (var item in options.Items)
            {
                Print(item);
            }
        }

        private void Print(OptionItem item)
        {
            Console.WriteLine($"{item.id}. {item.name}");
        }

        public void PrintErrorMessage()
        {
            Console.WriteLine($"Invalid option");
        }
    }
}
