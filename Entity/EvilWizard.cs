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
            Random random = new Random();
            PhysicalPercent = random.Next(-90, 89);
            MagicalPercent = random.Next(-90, 89);
            BludgeoningPercent = random.Next(-90, 89);
            PiercingPercent = random.Next(-90, 89);
            SlashingPercent = random.Next(-90, 89);
            HeatPercent = random.Next(-90, 89);
            ColdPercent = random.Next(-90, 89);
            EnergyPercent = random.Next(-90, 89);
            NecroticPercent = random.Next(-90, 89);
            PsychicPercent = random.Next(-90, 89);
            True = DamageResistanceLevel.Normal;
            Sprite.Position = position;
            AC = 20;
        }
    }
}