using System.Collections.Generic;
using System.Text;
using System;

namespace Dungeon.DiceClasses
{
    public static class Dice
    {
        private static readonly Random random = new Random();

        public static int D4()
        {
            return random.Next(1, 5);
        }

        public static int D6()
        {
            return random.Next(1, 7);
        }

        public static int D10()
        {
            return random.Next(1, 11);
        }

        public static int D12()
        {
            return random.Next(1, 13);
        }

        public static int D20()
        {
            return random.Next(1, 21);
        }

        public static int D100()
        {
            return random.Next(1, 101);
        }
    }
}