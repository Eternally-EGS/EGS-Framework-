using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FlatRedBall.Input;
using RenderingLibrary;

namespace EGS_Framework.Core.Debug
{
    public static class DebugConsole {
        private static DebugRect Background;
        private static DebugRect InputString;
        private static List<string> AllLeters = new List<string>();
        public static List<string> Output = new List<string>();
        private static string allLet = "";

        private static Microsoft.Xna.Framework.Color Background_Color = new Microsoft.Xna.Framework.Color(84, 84, 84, 100);
        private static Microsoft.Xna.Framework.Color InputString_Color = new Microsoft.Xna.Framework.Color(255, 0, 0, 100);
        
        public static void GetConsole()
        {
            // Cord on screen
            float startX = FlatRedBall.Camera.Main.X - (FlatRedBall.Camera.Main.OrthogonalWidth / 2f);
            float startY = FlatRedBall.Camera.Main.Y + (FlatRedBall.Camera.Main.OrthogonalHeight / 2f);

            float ConsoleWidth = FlatRedBall.Camera.Main.OrthogonalWidth;
            float ConsoleHeight = 200;

            float InputStringWidth = ConsoleWidth;
            float InputStringHeight = ConsoleHeight / 8;

            float Base_X = (startX + ConsoleWidth / 2);
            float Base_Y = (startY - ConsoleHeight / 2);

            float BackgroundDown = (Base_Y - ConsoleHeight/2) - InputStringHeight/2;

            //float width ширина, float height высота, float xoffset, float yoffset, Microsoft.Xna.Framework.Color color
            
            // DRAW background
            if (Background == null) { Background = new DebugRect(500,0,0,0,Background_Color); }

            Background.Rect.X = Base_X;
            Background.Rect.Y = Base_Y;
            Background.Rect.Width = ConsoleWidth;
            Background.Rect.Height = ConsoleHeight;

            // DRAW input string
            if (InputString == null) { InputString = new DebugRect(500,50,0,0,InputString_Color); }

            InputString.Rect.Width = InputStringWidth;
            InputString.Rect.Height = InputStringHeight;
            InputString.Rect.X = Base_X;
            InputString.Rect.Y = BackgroundDown;
            
            // TEMP 
            string output_ = "";
            string freshTypedText = InputManager.Keyboard.GetStringTyped();

            // Convert and check errors
            if (freshTypedText != "" && freshTypedText != "`" && freshTypedText != "~" && freshTypedText != "ё" && freshTypedText != "Ё") {  
                AllLeters.Add(freshTypedText);  
            }

            // Backspase function
            if (FlatRedBall.Input.InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Back)) {
               if (AllLeters.Count > 0) { AllLeters.RemoveAt(AllLeters.Count - 1); }
            }

            // !!! Reset from 1 symvol
            allLet = ""; 
            // LIst -> string
            foreach (var AllL in AllLeters) { allLet += AllL; }

            // Command execute 
            if (FlatRedBall.Input.InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Enter)) {
                DebugConsoleHelper.ExtractCommand(allLet);
                Clear();
            }
            
            // Border
            if (Output.Count > 15) { string lastSavedLine = Output[Output.Count - 1];
                Output.Clear();
                Output.Add(lastSavedLine); }

            // Output convert
            foreach (var out_ in Output) { output_ += $"{out_}\n"; }


            // Main console textbox
            DebugDrawManager.DrawDebugConsoleTextOnScreen(output_,(Base_X - ConsoleWidth / 2) + 10,(Base_Y + ConsoleHeight/2) - 10,ConsoleWidth);
           
            // Input string
            DebugDrawManager.DrawDebugConsoleTextOnScreen("->" + allLet,(Base_X - ConsoleWidth / 2) + 10,BackgroundDown + 10,ConsoleWidth);

        }

        // Clear 
        public static void RemoveConsole()
        {
            DebugDrawManager.RemoveDebugStatOnScreen();

            if (Background != null) {
                FlatRedBall.SpriteManager.RemoveSprite(Background.Rect);
                Background = null;
            }

            if (InputString != null) {
                FlatRedBall.SpriteManager.RemoveSprite(InputString.Rect);
                InputString = null;
            }
        }

        // Reset
        private static void Clear() {
            allLet = "";
            AllLeters.Clear();
        }

    }
}