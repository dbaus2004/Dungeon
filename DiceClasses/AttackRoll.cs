using System;
using System.Text;
using System.Collections.Generic;

namespace Dungeon.DiceClasses
{
    public class AttackRoll
    {
        public string Source;
        public string Target;
        public int D10One;
        public int D10Two;
        public int AC;
        public (int, string)[] ToHitModifers;
        public int ToHitModifierTotal;
        public (int, string)[] ToCritModifers;
        public int ToCritModifierTotal;
        public (int, string)[] ToCritMissModifers;
        public int ToCritMissModifierTotal;
        public bool IsHit;
        public string Log;
        public bool IsCrit;
        public bool IsCritFail;
        public AttackRoll(string source, string target, int ac, (int, string)[] toHitModifers)
        {
            Source = source;
            Target = target;
            D10One = Dice.D10();
            D10Two = Dice.D10();
            AC = ac;
            ToHitModifers = toHitModifers;

            ToHitModifierTotal = 0;
            foreach ((int, string) m in ToHitModifers)
            {
                ToHitModifierTotal += m.Item1;
            }
            if (AC > (D10One + D10Two + ToHitModifierTotal))
            {
                IsHit = false;
            }
            else
            {
                IsHit = true;
            }

            StringBuilder log = new StringBuilder("");
            log.Append(Source);
            if (IsHit)
            {
                log.Append(" Hit ");
            }
            else
            {
                log.Append(" Missed ");
            }
            log.Append(target + " with a roll of: " + (D10One + D10Two + ToHitModifierTotal).ToString() + " \nvs the target AC of: " + AC.ToString() + " ");
            log.Append("\nThey Rolled: " + D10One.ToString() + " (d10) + " + D10Two.ToString() + " (d10)\n");
            foreach ((int, string) m in toHitModifers)
            {
                log.Append(" + " + m.Item1 + " (" + m.Item2 + ")");
            }
            Log = log.ToString();
        }
        public AttackRoll(string source, string target, int ac, (int, string)[] toHitModifers, (int, string)[] toCritModifers, (int, string)[] toCritMissModifers)
        {
            Source = source;
            Target = target;
            D10One = Dice.D10();
            D10Two = Dice.D10();
            AC = ac;
            ToHitModifers = toHitModifers;
            ToCritModifers = toCritModifers;
            ToCritMissModifers = toCritMissModifers;

            ToHitModifierTotal = 0;
            foreach ((int, string) m in ToHitModifers)
            {
                ToHitModifierTotal += m.Item1;
            }
            ToCritModifierTotal = 0;
            foreach ((int, string) m in ToCritModifers)
            {
                ToCritModifierTotal += m.Item1;
            }
            ToCritMissModifierTotal = 0;
            foreach ((int, string) m in ToCritMissModifers)
            {
                ToCritMissModifierTotal += m.Item1;
            }

            if (AC > (D10One + D10Two + ToHitModifierTotal))
            {
                IsHit = false;
            }
            else
            {
                IsHit = true;
            }
            if (D10One + D10Two <= 2 + ToCritMissModifierTotal)
            {
                IsCritFail = true;
                IsHit = false;
            }
            if (D10One + D10Two >= 20 + ToCritModifierTotal)
            {
                IsCrit = true;
                IsHit = true;
            }

            StringBuilder log = new StringBuilder("");
            log.Append(Source);
            if (IsHit)
            {
                log.Append(" Hit ");
            }
            else
            {
                log.Append(" Missed ");
            }
            if (IsCrit)
            {
                log.Append(target + " with a roll of: " + (D10One + D10Two + ToHitModifierTotal).ToString() + " (Critical!) \nvs the target AC of: " + AC.ToString() + " ");
            }
            else if (IsCritFail)
            {
                log.Append(target + " with a roll of: " + (D10One + D10Two + ToHitModifierTotal).ToString() + " (Critical Miss!) \nvs the target AC of: " + AC.ToString() + " ");
            }
            else
            {
                log.Append(target + " with a roll of: " + (D10One + D10Two + ToHitModifierTotal).ToString() + " \nvs the target AC of: " + AC.ToString() + " ");
            }
            log.Append("They Rolled: " + D10One.ToString() + " (d10) + " + D10Two.ToString() + " (d10)\n");
            foreach ((int, string) m in toHitModifers)
            {
                log.Append("+ " + m.Item1 + " (" + m.Item2 + ")");
            }
            if (ToCritModifers.Length > 0 || ToCritMissModifers.Length > 0)
            {
                log.Append(" With the Crit Modifiers of: ");
                foreach ((int, string) m in ToCritModifers)
                {
                    log.Append("+ " + m.Item1 + " (" + m.Item2 + ")");
                }
                foreach ((int, string) m in ToCritMissModifers)
                {
                    log.Append("+ " + m.Item1 + " (" + m.Item2 + ")");
                }
            }
            Log = log.ToString();
        }
    }
}