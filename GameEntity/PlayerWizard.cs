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
    public class PlayerWizard: Entity
    {
        public override ISprite Sprite {get; set;} = new WizardSprite(new Vector2(0,0));
        public PlayerWizard(Vector2 position)
        {
            Sprite.Position = position;
            Name = "Player character";
            HP = 10;
            Discription = "This is the player character for the demo";
        }
    }
}