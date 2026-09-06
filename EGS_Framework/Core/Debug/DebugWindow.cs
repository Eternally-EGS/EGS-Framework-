using FlatRedBall.Input;

namespace EGS_Framework.Core.Debug
{
    public static class DebugWindow {
        private static DebugRect Background;
        private static DebugWindowText _text;
        private static float WindowX = 0;
        private static float WindowY = 0;

        public static void GetWindow(string text) {
            float mouseX = FlatRedBall.Input.InputManager.Mouse.WorldXAt(0);
            float mouseY = FlatRedBall.Input.InputManager.Mouse.WorldYAt(0);

            float minX = -FlatRedBall.Camera.Main.OrthogonalWidth / 2f;
            float maxX =  FlatRedBall.Camera.Main.OrthogonalWidth / 2f;
            
            float minY = -FlatRedBall.Camera.Main.OrthogonalHeight / 2f;
            float maxY =  FlatRedBall.Camera.Main.OrthogonalHeight / 2f;

            float borderedX = System.Math.Clamp(mouseX, minX, maxX);
            float borderedY = System.Math.Clamp(mouseY, minY, maxY);
            
            if (Background == null) {
                Background = new DebugRect(200, 300, 100, -100, new Microsoft.Xna.Framework.Color(84, 84, 84, 100));
            }

            if (IsMouseOver() && InputManager.Mouse.ButtonDown(Mouse.MouseButtons.LeftButton)) { 
                
                WindowX = borderedX;
                WindowY = borderedY;
            }

            Background.Rect.X = WindowX;
            Background.Rect.Y = WindowY;

            if (_text == null) {
                _text = new DebugWindowText(); 
            }

        
            float textX = Background.Rect.X - (Background.Rect.Width / 2f);
            float textY = Background.Rect.Y + (Background.Rect.Height / 2f);


            _text.ResetWindowLines();

            _text.DrawDebugTextInsideWindow(text, textX, textY);

            Background.Rect.Height = _text.TextH + 5;
            Background.Rect.Width = _text.TextW + 5;

            if (IsMouseOver() && InputManager.Mouse.ButtonPushed(Mouse.MouseButtons.MiddleButton)) { 
                CloseWindow();
            }

        }

        public static bool IsMouseOver()
        {
            if (Background == null || Background.Rect == null) return false;

            float mouseX = FlatRedBall.Input.InputManager.Mouse.WorldXAt(0);
            float mouseY = FlatRedBall.Input.InputManager.Mouse.WorldYAt(0);

            float halfWidth = Background.Rect.Width / 2f;
            float halfHeight = Background.Rect.Height / 2f;

            bool insideX = mouseX >= (Background.Rect.X - halfWidth) && mouseX <= (Background.Rect.X + halfWidth);
            bool insideY = mouseY >= (Background.Rect.Y - halfHeight) && mouseY <= (Background.Rect.Y + halfHeight);

            return insideX && insideY;
        }

        public static void CloseWindow()
        {
            if (Background != null) 
            {
                FlatRedBall.SpriteManager.RemoveSprite(Background.Rect);
                Background = null; // Зануляем, чтобы при следующем вызове оно создалось заново
            }

            // 2. Стираем все буквы, которые были внутри этого окна
            if (_text != null)
            {
                _text.DestroyWindowTexts();
                _text = null; // Полностью уничтожаем текстовый замерщик
            }

            // 3. Сбрасываем координаты в дефолт
            WindowX = 0;
            WindowY = 0;
        }

    }
}