using System;

namespace _2DLogicGame
{
    /// <summary>
    /// Main trieda programu, ma za ulohu vytvorit instanciu triedy hry.
    /// </summary>
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
