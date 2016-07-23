using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZzukBot.Engines.CustomClass;
using ZzukBot.Engines.CustomClass.Objects;

namespace AnonClasses {
    public class EnhancementDeux : CustomClass {
        public string HPot = "Superior Healing Potion";

        public override byte DesignedForClass {
            get { return PlayerClass.Shaman; }
        }

        public override string CustomClassName {
            get { return "EnhancementDeux"; }
        }

        public override bool Buff() {
            this.Player.Buff("Lightning Shield");
            WeaponEnchant();
            return true;
        }

        public override void PreFight() {
            this.SetCombatDistance(26);

            if (this.Target.DistanceToPlayer <= 29)
                this.Player.Buff("Grace of Air Totem");

            if (this.Target.DistanceToPlayer <= 27) {
                this.SetCombatDistance(29);
                this.Player.UseSkillWait("Lightning Bolt", 5000);
            }  
        }

        public override void Fight() {
            if (this.Player.HealthPercent <= 5 && this.Player.IsCasting == "" && this.Player.VerifyItem(HPot))
                this.Player.UseItem(HPot);

            this.Player.Attack();
            this.Player.Buff("Lightning Shield");

            if (this.Target.DistanceToPlayer >= 21 && this.Player.VerifySkill("Earth Shock") && this.Target.Casting()) {
                this.SetCombatDistance(19);
                return;
            }

            if (this.Target.DistanceToPlayer <= 20 && this.Player.VerifySkill("Earth Shock") && this.Target.Casting()) {
                this.Player.Cast("Earth Shock");
                return;
            }

            if (this.Target.DistanceToPlayer <= 8 || this.Target.Casting() || this.Target.Channeling())
                this.SetCombatDistance(4);

            if (this.Player.HealthPercent <= 20 && this.Player.ManaPercent >= 25)
                this.Player.UseSkill("Stoneclaw Totem");

            if (this.Player.HealthPercent <= 35 && ((this.Target.HealthPercent + 5) >= this.Player.HealthPercent || this.Attackers.Multiple()))
                if (this.Player.VerifySkill("Lesser Healing Wave")) {
                    this.Player.CastWait("Lesser Healing Wave", 500);
                    return;
                }
                else {
                    this.Player.UseSkillWait("Healing Wave", 500);
                    return;
                }
            
            if (!this.Player.VerifySkill("Stormstrike")) {
                if (!this.Target.GotDebuff("Flame Shock") && this.Target.HealthPercent >= 40) {
                    this.Player.UseSkill("Flame Shock");
                    return;
                }
                
                if (this.Player.ManaPercent >= 15)
                    this.Player.UseSkill("Earth Shock");
            }
            else {
                if (this.Target.HealthPercent >= 35)
                    this.Player.UseSkill("Stormstrike");

                if (this.Player.ManaPercent >= 15 && this.Target.GotDebuff("Stormstrike"))
                    this.Player.UseSkill("Earth Shock");
            }

            if (this.Attackers.Multiple())
                this.Player.UseSkill("Stoneclaw Totem");

            WeaponEnchant();

            if (this.Player.ManaPercent <= 40)
                this.Player.Buff("Mana Spring Totem");
        }

        public override void Rest()
        {
            if (this.Player.HealthPercent <= 50 && !this.Player.GotBuff("Drink") && this.Player.ManaPercent >= 20)
                 this.Player.UseSkillWait("Healing Wave", 500);

            if (this.Player.NeedToDrink)
                this.Player.Drink();
            
            if (this.Player.NeedToEat)
                this.Player.Eat();
        }

        public void WeaponEnchant() {
            if (!this.Player.IsMainhandEnchanted()) {
                if (this.Player.VerifySkill("Windfury Weapon"))
                    this.Player.Cast("Windfury Weapon");
                else
                    this.Player.UseSkill("Rockbiter Weapon");
            }
        }
    }

    /// A bunch of helper extension methods for clean code
    public static class HelperFns {

        /// Verifies player has a skill and is able to use it
        public static bool VerifySkill(this _Player player, string skill) {
            if (player.GetSpellRank(skill) != 0 && player.CanUse(skill))
                return true;
            return false;
        }

        /// Verifies a player has a buff and can use buff before using it
        public static void Buff(this _Player player, string buff) {
            if (player.VerifySkill(buff) && !player.GotBuff(buff))
                player.Cast(buff);
        }

        /// Verifies player can use a skill and if so, uses it
        public static void UseSkill(this _Player player, string skill) {
            if (player.VerifySkill(skill))
                player.Cast(skill);
        }

        /// Verifies player can use a skill and if so, uses it and waits
        public static void UseSkillWait(this _Player player, string skill, int wait) {
            if (player.VerifySkill(skill))
                player.CastWait(skill, wait);
        }

        /// Verifies player has an item and can use it
        public static bool VerifyItem(this _Player player, string item) {
            if (player.ItemCount(item) >= 1 && player.CanUse(item))
                return true;
            return false;
        }

        /// Returns true if the target is casting a spell
        public static bool Casting(this _Target target) {
            if (target.IsCasting != "") return true;
            else return false;
        }

        /// Returns true if the target is channeling a spell
        public static bool Channeling(this _Target target) {
            if (target.IsChanneling != "") return true;
            else return false;
        }

        /// Returns true if there is more than one attacker
        public static bool Multiple(this IReadOnlyList<_Unit> attackers) {
            if (attackers.Count >= 2) return true;
            else return false;
        }
    }
}
