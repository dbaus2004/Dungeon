using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Dungeon.Collisions;
using Dungeon.Sprites;
using Dungeon.ENUM;
using Dungeon.DiceClasses;
using System;


namespace Dungeon.Objects
{
    public class EvilWizard: Entity
    {
        public override ISprite Sprite {get; set;} = new EvilWizardSprite(new Vector2(0,0));
        public EvilWizard(Vector2 position)
        {
            Name = "Evil Wizard";
            HP = (uint)(Dice.D10() + Dice.D10()  + Dice.D10()  + Dice.D10()  + Dice.D10()  + Dice.D10()  + Dice.D10()  + Dice.D10()  + Dice.D10()  + Dice.D10());
            Discription = "This is the arch rival of the player for the demo";
            PhysicalPercent = RandomHelper.Next(-90, 89);
            MagicalPercent = RandomHelper.Next(-90, 89);
            BludgeoningPercent = RandomHelper.Next(-90, 89);
            PiercingPercent = RandomHelper.Next(-90, 89);
            SlashingPercent = RandomHelper.Next(-90, 89);
            HeatPercent = RandomHelper.Next(-90, 89);
            ColdPercent = RandomHelper.Next(-90, 89);
            EnergyPercent = RandomHelper.Next(-90, 89);
            NecroticPercent = RandomHelper.Next(-90, 89);
            PsychicPercent = RandomHelper.Next(-90, 89);
            True = DamageResistanceLevel.Normal;
            Sprite.Position = position;
            AC = 20;
        }
    }
}