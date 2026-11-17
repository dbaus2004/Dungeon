using Microsoft.Xna.Framework;
using Dungeon.Sprites;
using Dungeon.ENUM;
using System;

namespace Dungeon.Objects
{
    public abstract class Entity
    {
        public bool IsAlive => HP > 0;

        public uint MaxHP { get; private set; } = 1;
        public uint HP { get; private set; } = 1;

        protected void SetMaxHP(uint maxHP)
        {
            if (maxHP == 0)
                throw new ArgumentOutOfRangeException(nameof(maxHP), "Maximum HP must be greater than zero.");

            MaxHP = maxHP;
            HP = maxHP;
        }

        public void TakeDamage(uint damage)
        {
            HP = damage >= HP ? 0 : HP - damage;
        }

        public void RestoreHealth(uint amount)
        {
            uint missingHealth = MaxHP - HP;
            HP += Math.Min(amount, missingHealth);
        }

        public virtual ISprite Sprite { get; set; } = new NoSprite(new Vector2(0, 0));
        public virtual string Name { get; set; } = "Nameless";
        public virtual string Description { get; set; } = "This object is beyond description";
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