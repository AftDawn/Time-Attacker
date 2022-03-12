using System;
using System.Reflection;
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
        public static TimeAttackerSettings privateSettings => (TimeAttackerSettings)Instance._Settings;
        
        //public override Type SaveDataType => typeof(TimeAttackerSaveData);
        //public static TimeAttackerSaveData SaveData => (TimeAttackerSaveData) Instance._SaveData;

        public override void Load()
        {
            Logger.SetLogLevel("AftDawn/", LogLevel.Verbose);
            Logger.Log(LogLevel.Verbose, "AftDawn/", "Hello Mt Celeste");
            
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
            if (privateSettings.Enable && (Engine.Scene as Level) != null)
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

                for (int index = 0; index < timeString.Length; index++)
                {
                    char currentCharacter = timeString[index];
                    if (currentCharacter == '.')
                    {
                        scaleFactor = scale * 0.7f;
                        posY -= 5f * scale;
                    }

                    Color altCharacterColour = (currentCharacter == ':' || currentCharacter == '.' || scaleFactor < scale) ? secondary : main;
                    
                    float altCharacterPos = (((currentCharacter == ':' || currentCharacter == '.') ? spacerWidth : numberWidth) + 4f) * scaleFactor;
                    
                    font.DrawOutline(fontFaceSize, currentCharacter.ToString(), new Vector2(posX + altCharacterPos / 2f, posY), new Vector2(0.5f, 1f), Vector2.One * scaleFactor, altCharacterColour, 2f, Color.Black);
                    posX += altCharacterPos;
                }
            }
            else
            {
                orig(position, timeString, scale, valid, finished, bestTime, alpha);
            }
        }

        private float numberWidth;
        private float spacerWidth;
        
        private void SpeedrunTimerDisplay_CalcuateBaseSizes(On.Celeste.SpeedrunTimerDisplay.orig_CalculateBaseSizes orig)
        {
            orig();
            FieldInfo nw = typeof(SpeedrunTimerDisplay).GetField("numberWidth", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo sw = typeof(SpeedrunTimerDisplay).GetField("spacerWidth", BindingFlags.NonPublic | BindingFlags.Static);
            numberWidth = (float)nw.GetValue(null);
            spacerWidth = (float)sw.GetValue(null);
        }

        private void SpeedrunTimerDisplay_RegisterAreaComplete(On.Celeste.Level.orig_RegisterAreaComplete orig, Level level)
        {
            orig(level);
        }

        private void OuiChapterPanel_Render(On.Celeste.OuiChapterPanel.orig_Render orig, OuiChapterPanel self)
        {
            orig(self);
        }
    }
}
