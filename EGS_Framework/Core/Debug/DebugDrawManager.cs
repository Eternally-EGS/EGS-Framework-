using System.Collections.Generic;
using System.Threading.Tasks;
using ExCSS;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EGS_Framework.Core.Debug
{
    
    public static class DebugDrawManager {
        private static List<Text> _debugTexts = new List<Text>();
        private static List<Text> _debugConsoleTexts = new List<Text>();
        private static List<Sprite> _debugRects = new List<Sprite>();
        private static int _currentRectIndex = 0;
        private static int _currentLineIndex = 0;
        private static int _currentLineIndex_0 = 0;
        private static DebugRect Background;
        
        // Graphic aditional
        private static List<DebugRect> _myBars = new List<DebugRect>();
        private static double _lastSpawnTime = 0;

        // Draw text function 
        public static void DrawDebugTextOnScreen(string _text,float xoffset,float yoffset) {
            // Local text
            Text currentText;

            // New object
            if (_currentLineIndex >= _debugTexts.Count)
            {
                currentText = TextManager.AddText(_text);
                currentText.TextureScale = 1f;
                _debugTexts.Add(currentText);
            }
            else
            {
                currentText = _debugTexts[_currentLineIndex];
            }

            currentText.Visible = true;
            currentText.DisplayText = _text;
            currentText.SetColor(new Microsoft.Xna.Framework.Color(84, 84, 84, 200));

            float lineSpacing = _currentLineIndex * 20f;

            currentText.X = Camera.Main.X - (Camera.Main.OrthogonalWidth / 2f) + xoffset;
            currentText.Y = Camera.Main.Y + (Camera.Main.OrthogonalHeight / 2f) + yoffset - lineSpacing;

            _currentLineIndex++;
        }

        // Main text
        public static void DrawDebugConsoleTextOnScreen(string _text,float xoffset,float yoffset,float WText) {
            Text currentText;

            if (_currentLineIndex_0 >= _debugConsoleTexts.Count)
            {
                currentText = TextManager.AddText(_text);
                currentText.TextureScale = 1f;
                _debugConsoleTexts.Add(currentText);
            }
            else
            {
                currentText = _debugConsoleTexts[_currentLineIndex_0];
            }

            currentText.Visible = true;
            currentText.DisplayText = _text;
            currentText.MaxWidth = WText;
            currentText.MaxWidthBehavior = MaxWidthBehavior.Wrap;
            currentText.HorizontalAlignment = FlatRedBall.Graphics.HorizontalAlignment.Left;
            currentText.VerticalAlignment = FlatRedBall.Graphics.VerticalAlignment.Top;


            currentText.X = xoffset;
            currentText.Y = yoffset;

            _currentLineIndex_0++;
        }

        // Clean screen 
        public static void RemoveDebugStatOnScreen()
        {
            foreach (var text in _debugTexts)
            {
                if (text != null) FlatRedBall.Graphics.TextManager.RemoveText(text);
            }
            _debugTexts.Clear();
            _currentLineIndex = 0;

            foreach (var text in _debugConsoleTexts)
            {
                if (text != null) FlatRedBall.Graphics.TextManager.RemoveText(text);
            }
            _debugConsoleTexts.Clear();
            _currentLineIndex_0 = 0;

            foreach (var rect in _debugRects)
            {
                if (rect != null) FlatRedBall.SpriteManager.RemoveSprite(rect);
            }
            _debugRects.Clear();
            _currentRectIndex = 0;
            
            foreach (var bar in _myBars)
            {
                if (bar != null && bar.Rect != null) FlatRedBall.SpriteManager.RemoveSprite(bar.Rect);
            }
            _myBars.Clear();

            if (Background != null) {
                FlatRedBall.SpriteManager.RemoveSprite(Background.Rect);
                Background = null;
            }

            _lastSpawnTime = 0;
        }

        // Update
        public static void Reset() {
            _currentLineIndex = 0;
            _currentRectIndex = 0;
            _currentLineIndex_0 = 0;
        }

    }

    public class DebugRect
    {
        public Sprite Rect;

        // Draw Rectangle
        public DebugRect(float width, float height, float xoffset, float yoffset, Microsoft.Xna.Framework.Color color)
        {
            var device = FlatRedBall.FlatRedBallServices.GraphicsDevice;
            Texture2D solidTexture = new Texture2D(device, 1, 1);
            Microsoft.Xna.Framework.Color[] colorData = new Microsoft.Xna.Framework.Color[] { color };
            solidTexture.SetData(colorData);

            float startX = Camera.Main.X - (Camera.Main.OrthogonalWidth / 2f) + xoffset;
            float startY = Camera.Main.Y + (Camera.Main.OrthogonalHeight / 2f) + yoffset;

            Rect = SpriteManager.AddSprite(solidTexture);
            Rect.Visible = true;
            Rect.Width = width;
            Rect.Height = height;
            Rect.X = startX;
            Rect.Y = startY;
            Rect.Color = color;
        }
    }

    public class DebugGraph
    {
        // Draw graphic function 
        public DebugRect Background;
        private List<DebugRect> _myBars = new List<DebugRect>();
        private double _lastSpawnTime = 0;
        private Microsoft.Xna.Framework.Color _barColor;

        public DebugGraph(Microsoft.Xna.Framework.Color barColor)
        {
            _barColor = barColor;
        }

        public void Draw(float value, float xoffset, float yoffset, float maxvalue)
        {
            if (Background == null) {
                Background = new DebugRect(150, 100, xoffset, yoffset, new Microsoft.Xna.Framework.Color(84, 84, 84, 200));
            }

            float halfWidthBackground = FlatRedBall.Camera.Main.OrthogonalWidth / 2f;
            Background.Rect.X = FlatRedBall.Camera.Main.X - halfWidthBackground + xoffset;
            Background.Rect.Y = FlatRedBall.Camera.Main.Y + (FlatRedBall.Camera.Main.OrthogonalHeight / 2f) + yoffset;

            double currentTime = FlatRedBall.TimeManager.CurrentTime;
            if (currentTime - _lastSpawnTime >= 0.5)
            {
                float inputMax = maxvalue; 
                float barMaxHeight = 90f; 
                int MaxBar = 17;

                float calculatedHeight = (value / inputMax) * barMaxHeight;
                calculatedHeight = System.Math.Clamp(calculatedHeight, 2f, barMaxHeight);

                DebugRect newBar = new DebugRect(5, calculatedHeight, xoffset + 65, yoffset, _barColor);
                
                _myBars.Insert(0, newBar);
                _lastSpawnTime = currentTime;

                if (_myBars.Count > MaxBar)
                {
                    int lastIndex = _myBars.Count - 1;
                    FlatRedBall.SpriteManager.RemoveSprite(_myBars[lastIndex].Rect);
                    _myBars.RemoveAt(lastIndex);
                }
            }

            for (int i = 0; i < _myBars.Count; i++)
            {
                float halfWidth = FlatRedBall.Camera.Main.OrthogonalWidth / 2f;
                _myBars[i].Rect.X = FlatRedBall.Camera.Main.X - halfWidth + (xoffset + 65) - (i * 8);
                _myBars[i].Rect.Y = FlatRedBall.Camera.Main.Y + (FlatRedBall.Camera.Main.OrthogonalHeight / 2f) + yoffset;
            }
        }

        public void Clear()
        {
            if (Background != null) {
                FlatRedBall.SpriteManager.RemoveSprite(Background.Rect);
                Background = null;
            }
            foreach (var bar in _myBars) {
                if (bar != null && bar.Rect != null) FlatRedBall.SpriteManager.RemoveSprite(bar.Rect);
            }
            _myBars.Clear();
            _lastSpawnTime = 0;
        }
    }


    public class DebugWindowText
    {
        private readonly List<Text> _windowTexts = new List<Text>();
        
        private int _currentWindowLineIndex = 0;
        public float TextW = 0;
        public float TextH = 0;

        // Draw text function inside Window
        public void DrawDebugTextInsideWindow(string _text, float xoffset, float yoffset)
        {
            Text currentText;

            if (_currentWindowLineIndex >= _windowTexts.Count)
            {
                currentText = TextManager.AddText(_text);
                currentText.TextureScale = 1f; 
                
                currentText.HorizontalAlignment = FlatRedBall.Graphics.HorizontalAlignment.Left;
                currentText.VerticalAlignment = FlatRedBall.Graphics.VerticalAlignment.Top;
                currentText.Z = 12f; 

                _windowTexts.Add(currentText);
            }
            else
            {
                currentText = _windowTexts[_currentWindowLineIndex];
            }

            currentText.Visible = true;
            currentText.DisplayText = _text;
            currentText.SetPixelPerfectScale(Camera.Main);

            currentText.X = xoffset;
            currentText.Y = yoffset;

            TextW = currentText.Width;
            TextH = currentText.Height;

            _currentWindowLineIndex++;
        }

        public void ResetWindowLines()
        {
            _currentWindowLineIndex = 0;
        }

        public void DestroyWindowTexts()
        {
            foreach (var text in _windowTexts)
            {
                if (text != null) TextManager.RemoveText(text);
            }
            _windowTexts.Clear();
            _currentWindowLineIndex = 0;
        }
    }
}