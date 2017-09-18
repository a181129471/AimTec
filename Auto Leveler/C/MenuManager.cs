using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aimtec;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util;

namespace Auto_Leveler {

    class MenuManager {

        private static readonly Menu Menu = new Menu("autoLeveler", "AutoLevelUp", true);

        private static string _championName;

        private static bool _validConfiguration;

        public static Dictionary<SpellSlot, int> LevelPriorities = new Dictionary<SpellSlot, int>();

        public static Dictionary<SpellSlot, int> LevelAts = new Dictionary<SpellSlot, int>();

        public static void Create() {
            _championName = ObjectManager.GetLocalPlayer().ChampionName;

            Menu autoLevelerMenu = new Menu(_championName, _championName) {
                new Menu("levels", "Levels") {
                    new MenuSlider("q", "Q", 2, 1, 4),
                    new MenuSlider("w", "W", 3, 1, 4),
                    new MenuSlider("e", "E", 4, 1, 4),
                    new MenuSlider("r", "R", 1, 1, 4)
                },
                new Menu("levelAt", "Level at") {
                    new MenuSlider("q", "Q", 2, 2, 18),
                    new MenuSlider("w", "W", 2, 2, 18),
                    new MenuSlider("e", "E", 2, 2, 18),
                    new MenuSlider("r", "R", 2, 2, 18)
                },
                new MenuBool("atleastOnce", "Level QWE atleast once"),
                new MenuSlider("startAt", "Start the Auto leveler at level", 2, 2, 18)
            };
            MenuBool humanizer = new MenuBool("humanizer", "Humanizer Delay");

            autoLevelerMenu["levels"].OnValueChanged += LevelsOnOnValueChanged;
            autoLevelerMenu["levelAt"].OnValueChanged += LevelAtOnOnValueChanged;

            Menu.Add(autoLevelerMenu);
            Menu.Add(humanizer);

            Menu.Attach();

            ReadLevelAts();

            if (HasValidPriorities()) {
                ReadSpellValues();
                _validConfiguration = true;
            }
        }

        public static int StartAt() {
            return Menu[_championName]["startAt"].Value;
        }

        public static bool LevelQWEAtleastOnce() {
            return Menu[_championName]["atleastOnce"].Enabled;
        }

        public static bool HumanizerEnabled() {
            return Menu["humanizer"].Enabled;
        }

        public static bool HasValidConfiguration() {
            return _validConfiguration;
        }

        private static void ReadSpellValues() {
            LevelPriorities[SpellSlot.Q] = Menu[_championName]["levels"]["q"].Value;
            LevelPriorities[SpellSlot.W] = Menu[_championName]["levels"]["w"].Value;
            LevelPriorities[SpellSlot.E] = Menu[_championName]["levels"]["e"].Value;
            LevelPriorities[SpellSlot.R] = Menu[_championName]["levels"]["r"].Value;
        }

        private static void LevelAtOnOnValueChanged(MenuComponent sender, ValueChangedArgs args) {
            LevelAts[SpellSlot.Q] = Menu[_championName]["levelAt"]["q"].Value;
            LevelAts[SpellSlot.W] = Menu[_championName]["levelAt"]["w"].Value;
            LevelAts[SpellSlot.E] = Menu[_championName]["levelAt"]["e"].Value;
            LevelAts[SpellSlot.R] = Menu[_championName]["levelAt"]["r"].Value;
        }

        private static void ReadLevelAts() {
            LevelAts[SpellSlot.Q] = Menu[_championName]["levelAt"]["q"].Value;
            LevelAts[SpellSlot.W] = Menu[_championName]["levelAt"]["w"].Value;
            LevelAts[SpellSlot.E] = Menu[_championName]["levelAt"]["e"].Value;
            LevelAts[SpellSlot.R] = Menu[_championName]["levelAt"]["r"].Value;
        }

        private static void LevelsOnOnValueChanged(MenuComponent sender, ValueChangedArgs args) {
            if (HasValidPriorities()) {
                _validConfiguration = true;
                ReadSpellValues();
            }
            else {
                _validConfiguration = false;
            }
        }

        private static bool HasValidPriorities() {
            List<int> priorities = new List<int>(new int[] {
                Menu[_championName]["levels"]["q"].Value, Menu[_championName]["levels"]["w"].Value,
                Menu[_championName]["levels"]["e"].Value, Menu[_championName]["levels"]["r"].Value
            });

            foreach (int priority in priorities) {
                if (priorities.FindAll(x => x == priority).Count > 1) {
                    return false;
                }
            }

            return true;
        }

    }

}