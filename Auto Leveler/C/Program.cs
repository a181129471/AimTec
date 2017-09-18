using System;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Aimtec;
using Aimtec.SDK.Events;
using Aimtec.SDK.Util;

namespace Auto_Leveler {

    public class Program {

        private static readonly int BaseY = (int) (Render.Height - Render.Height / 4);
        private static readonly int BaseX = (int) (Render.Width / 2.0);


        private static readonly Random Random = new Random();

        private static bool _needToLevelCurrent;
        private static int _currentOldlevel;
        private static SpellSlot _currentSpell;

        public static void Main(string[] args) {
            GameEvents.GameStart += GameEvents_GameStart;
        }

        private static void GameEvents_GameStart() {
            MenuManager.Create();

            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnLevelUp += ObjAiBaseOnOnLevelUp;
        }

        private static void Game_OnUpdate() {
            if (_needToLevelCurrent) {
                ObjectManager.GetLocalPlayer().SpellBook.LevelSpell(_currentSpell);

                if (ObjectManager.GetLocalPlayer().SpellBook.GetSpell(_currentSpell).Level != _currentOldlevel) {
                    _needToLevelCurrent = false;
                }
            }
        }


        private static void ObjAiBaseOnOnLevelUp(Obj_AI_Base objAi, Obj_AI_BaseLevelUpEventArgs e) {
            if (e.NewLevel < MenuManager.StartAt()) {
                return;
            }

            if (objAi.GetType() != typeof(Obj_AI_Hero)) {
                return;
            }

            Obj_AI_Hero hero = (Obj_AI_Hero) objAi;
            if (hero.Name == null || !hero.Name.Equals(ObjectManager.GetLocalPlayer().Name)) {
                return;
            }

            List<KeyValuePair<SpellSlot, int>> pairs = MenuManager.LevelPriorities.ToList();
            pairs.Sort((p1, p2) => p1.Value.CompareTo(p2.Value));

            foreach (KeyValuePair<SpellSlot, int> pair in pairs) {
                Spell spell = hero.SpellBook.GetSpell(pair.Key);

                if (MenuManager.LevelQWEAtleastOnce() && e.NewLevel <= 3) {
                    if (spell.Level == 0 && LevelSpell(pair.Key, e.NewLevel)) {
                        return;
                    }
                }
                else {
                    if (e.NewLevel >= MenuManager.LevelAts[pair.Key]) {
                        if (LevelSpell(pair.Key, e.NewLevel)) {
                            return;
                        }
                    }
                }
            }
        }

        private static bool CanLevelUpSpell(SpellSlot spell, int combatLevel) {
            Obj_AI_Hero player = ObjectManager.GetLocalPlayer();
            int spellLevel = player.SpellBook.GetSpell(spell).Level;
            string champion = player.ChampionName;

            if (spell.Equals(SpellSlot.R) && !champion.Equals("Udyr")) {
                // ulti level 1
                if (combatLevel >= 6 && combatLevel < 11 && spellLevel == 0) {
                    return true;
                }

                // ulti level 2
                if (combatLevel >= 11 && combatLevel < 16 && spellLevel <= 1) {
                    return true;
                }

                // ulti level 3
                if (combatLevel >= 16 && spellLevel <= 2) {
                    // ryze r is max 2
                    if (champion.Equals("Ryze") && spellLevel == 2) {
                        return false;
                    }
                    return true;
                }
            }
            else {
                // level 1-2, max level is 1
                if (combatLevel >= 1 && combatLevel < 3 && spellLevel == 0) {
                    return true;
                }

                // level 3-4, max level is 2
                if (combatLevel >= 3 && combatLevel < 5 && spellLevel <= 1) {
                    return true;
                }

                // level 5-6, max level is 3
                if (combatLevel >= 5 && combatLevel < 7 && spellLevel <= 2) {
                    return true;
                }

                // level 7-8, max level is 4
                if (combatLevel >= 7 && combatLevel < 9 && spellLevel <= 3) {
                    return true;
                }

                // level >= 9, max level is 5
                if (combatLevel >= 9 && spellLevel <= 4) {
                    return true;
                }

                // level >= 11 for ryze, max level for q is 6
                if (champion.Equals("Ryze") && spell.Equals(SpellSlot.Q) && combatLevel >= 11 && spellLevel <= 5) {
                    return true;
                }
            }

            return false;
        }

        private static bool LevelSpell(SpellSlot spellSlot, int combatLevel) {
            if (CanLevelUpSpell(spellSlot, combatLevel)) {
                _needToLevelCurrent = true;
                _currentSpell = spellSlot;
                _currentOldlevel = ObjectManager.GetLocalPlayer().SpellBook.GetSpell(spellSlot).Level;
                return true;
            }
            return false;
        }

    }

}