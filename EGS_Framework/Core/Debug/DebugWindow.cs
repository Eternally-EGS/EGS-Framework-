using FlatRedBall.Input;
using System;

namespace EGS_Framework.Core.Debug
{
    public class DebugWindow 
    {
        private DebugRect _background;
        private DebugWindowText _text;
        private float _windowX = 0;
        private static DebugWindow _draggingWindow = null;
        private float _windowY = 0;
        private Func<string> _textFunc; 
        private static readonly System.Collections.Generic.List<DebugWindow> _allWindows 
        = new System.Collections.Generic.List<DebugWindow>();

        public DebugWindow(Func<string> textFunc) 
        {
            _background = new DebugRect(200, 300, 100, -100, new Microsoft.Xna.Framework.Color(84, 84, 84, 200));
            _text = new DebugWindowText(); 
            _textFunc = textFunc;
        }

        public void UpdateAndDraw() 
        {
            if (_background == null) return;

            float mouseX = InputManager.Mouse.WorldXAt(0);
            float mouseY = InputManager.Mouse.WorldYAt(0);

            float minX = -FlatRedBall.Camera.Main.OrthogonalWidth / 2f;
            float maxX =  FlatRedBall.Camera.Main.OrthogonalWidth / 2f;
            
            float minY = -FlatRedBall.Camera.Main.OrthogonalHeight / 2f;
            float maxY =  FlatRedBall.Camera.Main.OrthogonalHeight / 2f;

            float borderedX = Math.Clamp(mouseX, minX, maxX);
            float borderedY = Math.Clamp(mouseY, minY, maxY);

            bool mouseDown = InputManager.Mouse.ButtonDown(Mouse.MouseButtons.LeftButton);

            if (_draggingWindow == null && mouseDown && IsMouseOver())
            {
                _draggingWindow = this;
            }

            if (_draggingWindow == this)
            {
                _windowX = borderedX;
                _windowY = borderedY;

                if (!mouseDown)
                {
                    _draggingWindow = null;
                }
            }

            _background.Rect.X = _windowX;
            _background.Rect.Y = _windowY;

            float textX = _background.Rect.X - (_background.Rect.Width / 2f);
            float textY = _background.Rect.Y + (_background.Rect.Height / 2f);

            _text.ResetWindowLines();
            
            string liveText = _textFunc != null ? _textFunc() : "No Data";
            _text.DrawDebugTextInsideWindow(liveText, textX, textY);

            _background.Rect.Height = _text.TextH + 5;
            _background.Rect.Width = _text.TextW + 5;

            if (IsMouseOver() && InputManager.Mouse.ButtonPushed(Mouse.MouseButtons.MiddleButton)) 
            { 
                Close();
            }
        }

        public bool IsMouseOver()
        {
            if (_background == null || _background.Rect == null) return false;

            float mouseX = InputManager.Mouse.WorldXAt(0);
            float mouseY = InputManager.Mouse.WorldYAt(0);

            float halfWidth = _background.Rect.Width / 2f;
            float halfHeight = _background.Rect.Height / 2f;

            bool insideX = mouseX >= (_background.Rect.X - halfWidth) && mouseX <= (_background.Rect.X + halfWidth);
            bool insideY = mouseY >= (_background.Rect.Y - halfHeight) && mouseY <= (_background.Rect.Y + halfHeight);

            return insideX && insideY;
        }

        public void Close()
        {
            if (_background != null) 
            {
                FlatRedBall.SpriteManager.RemoveSprite(_background.Rect);
                _background = null; 
            }

            if (_text != null)
            {
                _text.DestroyWindowTexts();
                _text = null; 
            }

            _windowX = 0;
            _windowY = 0;
        }

         public static void ClearAll()
        {
            var copy = new System.Collections.Generic.List<DebugWindow>(_allWindows);
            foreach (var w in copy)
            {
                try { w.Close(); } catch { }
            }
            _allWindows.Clear();
            _draggingWindow = null;
        }
    }
}
