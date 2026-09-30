using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Gui;
using FlatRedBall.Math;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Localization;
using Microsoft.Xna.Framework;

using EGS_Framework.Entities;
using EGS_Framework.Core.Debug;
using EGS_Framework.Services.UI;


namespace EGS_Framework.Screens
{
    public partial class Main_Menu
    {
        private void CustomInitialize()
        {
            MenuManager.MenuInit(GumScreen);
        }

        private void CustomActivity(bool firstTimeCalled)
        {
            
        }

        private void CustomDestroy()
        {
            // ОБЯЗАТЕЛЬНО НАХУЙ НЕ ПОСТАВИШ ИГРЕ ПИЗДА ПОЛНАЯ 
            DebugCleanup.Clear();    
        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {
            
        }
    }
}
