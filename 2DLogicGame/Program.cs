using System;

namespace _2DLogicGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new LogicGame())
                game.Run();
        }
    }
}
