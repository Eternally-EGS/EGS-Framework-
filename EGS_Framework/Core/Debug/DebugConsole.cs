using System;
using System.Collections.Generic;
using FlatRedBall.Input;

namespace EGS_Framework.Core.Debug
{
    public static class DebugConsole
    {
        private static DebugRect Background;
        private static DebugRect InputString;
        public static List<string> Output = new List<string>();
        public static List<string> Memory = new List<string>();
        private static int Memorycount = 0;
        private static string allLet = "";
        private static int _backDelay = 0;
        private static int _backMaxDelay = 1000;
        
        private static int BackDelay
        {
            get => _backDelay;
            set => _backDelay = Math.Clamp(value, 0, _backMaxDelay);
        }

        private static Microsoft.Xna.Framework.Color Background_Color = new Microsoft.Xna.Framework.Color(84, 84, 84, 200);
        private static Microsoft.Xna.Framework.Color InputString_Color = new Microsoft.Xna.Framework.Color(255, 0, 0, 200);

        public static void GetConsole()
        {
            float startX = FlatRedBall.Camera.Main.X - (FlatRedBall.Camera.Main.OrthogonalWidth / 2f);
            float startY = FlatRedBall.Camera.Main.Y + (FlatRedBall.Camera.Main.OrthogonalHeight / 2f);

            float ConsoleWidth = FlatRedBall.Camera.Main.OrthogonalWidth;
            float ConsoleHeight = 200;

            float InputStringWidth = ConsoleWidth;
            float InputStringHeight = ConsoleHeight / 8;

            float Base_X = (startX + ConsoleWidth / 2);
            float Base_Y = (startY - ConsoleHeight / 2);

            float BackgroundDown = (Base_Y - ConsoleHeight / 2) - InputStringHeight / 2;

            if (Background == null) { Background = new DebugRect(500, 0, 0, 0, Background_Color); }

            Background.Rect.X = Base_X;
            Background.Rect.Y = Base_Y;
            Background.Rect.Width = ConsoleWidth;
            Background.Rect.Height = ConsoleHeight;

            if (InputString == null) { InputString = new DebugRect(500, 50, 0, 0, InputString_Color); }

            InputString.Rect.Width = InputStringWidth;
            InputString.Rect.Height = InputStringHeight;
            InputString.Rect.X = Base_X;
            InputString.Rect.Y = BackgroundDown;

            string output_ = "";
            string freshTypedText = InputManager.Keyboard.GetStringTyped();

            if (freshTypedText != "" && freshTypedText != "`" && freshTypedText != "~" && freshTypedText != "ё" && freshTypedText != "Ё")
            {
                allLet += freshTypedText;
            }

            if (InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Back))
            {
                if (allLet.Length > 0) { allLet = allLet.Substring(0, allLet.Length - 1); }
            }
            
            if (InputManager.Keyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.Back))
            {
                BackDelay ++;
                if (BackDelay == _backMaxDelay) { allLet = ""; }
            } else { BackDelay --; }


            if (InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                if (Memory.Count > 0 && Memorycount > 0) { Memorycount--; allLet = Memory[Memorycount]; }
            }

            if (InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                if (Memory.Count > 0 && Memory.Count - 1 > Memorycount) { Memorycount++; allLet = Memory[Memorycount]; }
            }

            if (InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                DebugConsoleHelper.ExtractCommand(allLet);
                Memory.Add(allLet.Trim());
                Memorycount = Memory.Count;
                allLet = "";
            }

            if (Output.Count > 15)
            {
                string lastSavedLine = Output[Output.Count - 1];
                Output.Clear();
                Output.Add(lastSavedLine);
            }

            foreach (var out_ in Output) { output_ += $"{out_}\n"; }

            DebugDrawManager.DrawDebugConsoleTextOnScreen(output_, (Base_X - ConsoleWidth / 2) + 10, (Base_Y + ConsoleHeight / 2) - 10, ConsoleWidth);
            DebugDrawManager.DrawDebugConsoleTextOnScreen("->" + allLet, (Base_X - ConsoleWidth / 2) + 10, BackgroundDown + 10, ConsoleWidth);
        }

        public static void RemoveConsole()
        {
            DebugDrawManager.RemoveDebugStatOnScreen();

            if (Background != null)
            {
                FlatRedBall.SpriteManager.RemoveSprite(Background.Rect);
                Background = null;
            }

            if (InputString != null)
            {
                FlatRedBall.SpriteManager.RemoveSprite(InputString.Rect);
                InputString = null;
            }
        }
    }
}