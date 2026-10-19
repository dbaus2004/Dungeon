using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Dungeon.Collisions;
using Dungeon.Sprites;
using Dungeon.ENUM;
using System;

namespace Dungeon.Objects
{
    public abstract class Entity
    {
        public bool IsAlive => HP > 0;

        public void TakeTrueDamage(uint damage)
        {
            HP = damage >= HP ? 0 : HP - damage;
        }
        public virtual ISprite Sprite { get; set; } = new NoSprite(new Vector2(0, 0));
        public virtual string Name { get; set; } = "Nameless";
        public virtual string Discription { get; set; } = "This object is beyond description";
        public virtual uint HP { get; set; } = 1;
        public virtual int AC {get; set;}
        public virtual DamageResistanceLevel Normal { get; set; } = DamageResistanceLevel.Normal;
        public virtual DamageResistanceLevel Magical { get; set; } = DamageResistanceLevel.Normal;
        public virtual DamageResistanceLevel True { get; set; } = DamageResistanceLevel.Immune;
        public virtual DamageResistanceLevel Bludgeoning { get; set; } = DamageResistanceLevel.Normal;
        public virtual DamageResistanceLevel Piercing { get; set; } = DamageResistanceLevel.Normal;
        public virtual DamageResistanceLevel Slashing { get; set; } = DamageResistanceLevel.Normal;
        public virtual DamageResistanceLevel Heat { get; set; } = DamageResistanceLevel.Normal;
        public virtual DamageResistanceLevel Cold { get; set; } = DamageResistanceLevel.Normal;
        public virtual DamageResistanceLevel Energy { get; set; } = DamageResistanceLevel.Normal;
        public virtual DamageResistanceLevel Necrotic { get; set; } = DamageResistanceLevel.Normal;
        public virtual DamageResistanceLevel Psychic { get; set; } = DamageResistanceLevel.Normal;
        public virtual int PhysicalPercent { get; set; } = 0;
        public virtual int MagicalPercent { get; set; } = 0;
        public virtual int BludgeoningPercent { get; set; } = 0;
        public virtual int PiercingPercent { get; set; } = 0;
        public virtual int SlashingPercent { get; set; } = 0;
        public virtual int HeatPercent { get; set; } = 0;
        public virtual int ColdPercent { get; set; } = 0;
        public virtual int EnergyPercent { get; set; } = 0;
        public virtual int NecroticPercent { get; set; } = 0;
        public virtual int PsychicPercent { get; set; } = 0;
    }
}