using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.TimeAttacker {

    [SettingName("Time Attacker - By AftDawn")]
    public class TimeAttackerSettings : EverestModuleSettings
    {
        public bool Enable { get; set; } = false;
    }
    
    public class TimeAttackerModule : EverestModule {
        
        public static TimeAttackerModule Instance;

        public TimeAttackerModule()
        {
            Instance = this;
        }

        public static TimeAttackerSettings Settings => (TimeAttackerSettings)Instance._Settings;
        
        //public override Type SaveDataType => typeof(TimeAttackerSaveData);
        //public static TimeAttackerSaveData SaveData => (TimeAttackerSaveData) Instance._SaveData;

        public override void Load()
        {
            On.Celeste.SpeedrunTimerDisplay.DrawTime += DrawTime;
            //On.Celeste.Level.RegisterAreaComplete +=
        }

        public override void Unload() {
        }


        private void DrawTime(On.Celeste.SpeedrunTimerDisplay.orig_DrawTime orig, Vector2 position, string timeString, float scale, bool valid, bool finished, bool bestTime, float alpha)
        {
            // if the mod is activated and in a level
            if (Settings.Enable && (Engine.Scene as Level) != null)
            {
                Level level = Engine.Scene as Level;
                Session session = level.Session;
                TimeSpan timeSpan = TimeSpan.FromTicks(session.Time);
                int time = (int) timeSpan.TotalSeconds;
            }
            else
            {
                orig(position, timeString, scale, valid, finished, bestTime, alpha);
            }

        }
        // small helper code and shit idk lol
        private string formatTime(int seconds)
        {
            int remainer = seconds % 60;
            string time = (seconds / 60) + ":" + (remainer < 10 ? "0" : "") + remainer;
            return time;
        }
        
    }
}
