using System;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Screens;
using Microsoft.Xna.Framework;

namespace EGS_Framework.Core.Debug
{
    public static class DebugDashBoard
    {
        private static DebugGraph _frametimeGraph = new DebugGraph(new Color(255,0,0,200));
        private static DebugGraph _memoryGraph = new DebugGraph(new Color(0,255,0,200));
        private static DebugGraph _fpsGraph = new DebugGraph(new Color(0,0,255,200));
        // Show game info
        public static void GetDashBoardInfo()
        {
            // Get parametress
            float pading = 10;
            float deltaTime = TimeManager.SecondDifference;
            float fps = (deltaTime > 0) ? 1f / deltaTime : 0;
            float memory = GC.GetTotalMemory(true) / 1024;

            int spritesCount = SpriteManager.AutomaticallyUpdatedSprites.Count; 
            int textsCount = TextManager.AutomaticallyUpdatedTexts.Count;     
            int entitiesCount = SpriteManager.ManagedPositionedObjects.Count;
            int totalObjects = spritesCount + textsCount + entitiesCount;
            string screen = ScreenManager.CurrentScreen.ToString();
            string currentscreen = screen;
            float frameTime = FlatRedBall.TimeManager.SecondDifference * 1000f;

            // Drawing
            DebugDrawManager.DrawDebugTextOnScreen($"FPS: {fps:F0} TotalMemory: {memory:F0} kb",pading,-20);
            DebugDrawManager.DrawDebugTextOnScreen($"TotalObjectOnScreen: {totalObjects:F0}",pading,-20);
            DebugDrawManager.DrawDebugTextOnScreen($"Sprites: {spritesCount:F0}",pading,-20);
            DebugDrawManager.DrawDebugTextOnScreen($"Text: {textsCount:F0}",pading,-20);
            DebugDrawManager.DrawDebugTextOnScreen($"Entity: {entitiesCount:F0}",pading,-20);
            DebugDrawManager.DrawDebugTextOnScreen($"CurrentScreen: {currentscreen}",pading,-20);
            DebugDrawManager.DrawDebugTextOnScreen($"FrameTime: {frameTime:F2}",pading,-20);
            DebugDrawManager.DrawDebugTextOnScreen($"FrameTime                         Memory                             Fps",pading,-20);
            _frametimeGraph.Draw(frameTime,86,-220,200f);
            _memoryGraph.Draw(memory,250,-220,102400f);
            _fpsGraph.Draw(fps,414,-220,250f);     
        }

        // Remove
        public static void DashBoardClear() {
            _frametimeGraph.Clear();
            _memoryGraph.Clear();
            _fpsGraph.Clear();
        }

        // Reset
        public static void DashBoardAllClear() 
        {
            _frametimeGraph.Clear(); 
            _memoryGraph.Clear(); 
            _fpsGraph.Clear(); 
            DebugDrawManager.RemoveDebugStatOnScreen();
        }
    }
}