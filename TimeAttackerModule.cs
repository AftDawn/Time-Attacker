using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.TimeAttacker {

    [SettingName("Time Attacker")]
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

        public override Type SettingsType => typeof(TimeAttackerSettings);
        public static TimeAttackerSettings Settings => (TimeAttackerSettings)Instance._Settings;
        
        //public override Type SaveDataType => typeof(TimeAttackerSaveData);
        //public static TimeAttackerSaveData SaveData => (TimeAttackerSaveData) Instance._SaveData;

        public override void Load()
        {
            Logger.SetLogLevel("AftDawn/TimeAttacker", LogLevel.Verbose);
            Logger.Log(LogLevel.Verbose, "AftDawn/TimeAttacker", "Hello Mt Celeste");
            
            On.Celeste.SpeedrunTimerDisplay.DrawTime += SpeedrunTimerDisplay_DrawTime;
            On.Celeste.SpeedrunTimerDisplay.CalculateBaseSizes += SpeedrunTimerDisplay_CalcuateBaseSizes;
            On.Celeste.Level.RegisterAreaComplete += SpeedrunTimerDisplay_RegisterAreaComplete;

            On.Celeste.OuiChapterPanel.Render += OuiChapterPanel_Render;
        }

        public override void Unload() {
            
            Logger.Log(LogLevel.Verbose, "AftDawn/TimeAttacker", "Goodbye Mt Celeste");
            
            On.Celeste.SpeedrunTimerDisplay.DrawTime -= SpeedrunTimerDisplay_DrawTime;
            On.Celeste.SpeedrunTimerDisplay.CalculateBaseSizes -= SpeedrunTimerDisplay_CalcuateBaseSizes;
            On.Celeste.Level.RegisterAreaComplete -= SpeedrunTimerDisplay_RegisterAreaComplete;

            On.Celeste.OuiChapterPanel.Render -= OuiChapterPanel_Render;
        }


        private void SpeedrunTimerDisplay_DrawTime(On.Celeste.SpeedrunTimerDisplay.orig_DrawTime orig, Vector2 position, string timeString, float scale, bool valid, bool finished, bool bestTime, float alpha)
        {
            if (Settings.Enable && (Engine.Scene as Level) != null)
            {
                PixelFont font = Dialog.Languages["english"].Font;
                float fontFaceSize = Dialog.Languages["english"].FontFaceSize;
                float scaleFactor = scale;
                float posX = position.X;
                float posY = position.Y;
                Color main = Color.White * alpha;
                Color secondary = Color.LightGray * alpha;
                if (!valid)
                {
                    main = Calc.HexToColor("918988") * alpha;
                    secondary = Calc.HexToColor("7a6f6d") * alpha;
                }
                else if (bestTime)
                {
                    main = Calc.HexToColor("fad768") * alpha;
                    secondary = Calc.HexToColor("cfa727") * alpha;
                }
                else if (finished)
                {
                    main = Calc.HexToColor("6ded87") * alpha;
                    secondary = Calc.HexToColor("43d14c") * alpha;
                }

                for (int i = 0; i < timeString.Length; i++)
                {
                    char currentCharacter = timeString[i];
                    if (currentCharacter == '.')
                    {
                        scaleFactor = scale * 0.7f;
                        posY -= 5f * scale;
                    }

                    Color color3 = (currentCharacter == ':' || currentCharacter == '.' || scaleFactor < scale) ? secondary : main;
                    
                    float num4 = (((currentCharacter == ':' || currentCharacter == '.') ? SpeedrunTimerDisplay.spacerWidth : SpeedrunTimerDisplay.numberWidth) + 4f) * scaleFactor;
                    
                    font.DrawOutline(fontFaceSize, currentCharacter.ToString(),
                        new Vector2(posX + num4 / 2f, posY),
                        new Vector2(0.5f, 1f), Vector2.One * scaleFactor,
                        color3, 2f, Color.Black);
                    posX += num4;
                }
            }
            else
            {
                orig(position, timeString, scale, valid, finished, bestTime, alpha);
            }
        }

        private void SpeedrunTimerDisplay_CalcuateBaseSizes(On.Celeste.SpeedrunTimerDisplay.orig_CalculateBaseSizes orig)
        {
            orig();
        }

        private void SpeedrunTimerDisplay_RegisterAreaComplete(On.Celeste.Level.orig_RegisterAreaComplete orig, Level level)
        {
            orig(level);
        }

        private void OuiChapterPanel_Render(On.Celeste.OuiChapterPanel.orig_Render orig, OuiChapterPanel self)
        {
            orig(self);
        }
        
        // small helper code and shit idk lol
        private string formatTime(int seconds)
        {
            int remainder = seconds % 60;
            string time = (seconds / 60) + ":" + (remainder < 10 ? "0" : "") + remainder;
            return time;
        }

    }
}
