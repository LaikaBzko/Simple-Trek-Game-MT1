using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Storage;
//using Microsoft.Xna.Framework.GamerServices;

namespace RC_Framework
{

    /// <summary>
    /// a sort of menu system that maintains a state called switchVal (int) and or val (float)
    /// It uses tag and tagint in renderable
    /// Its importent to call update before you pass in keyhit and mouseevents or the clickedThisFrame will not work
    /// Its a kind of half way to a full GUI 
    ///    this suggests that renderables will ultimately replace GUI objects in this framework
    /// </summary>
    class RC_GuiMenuRenderable : RC_Renderable 
    {

        float val = -1;
        int switchVal = -1;
        int currentFocusIndex = -1; // the currently selected one
        bool initalized = false;

        /// <summary>
        /// true if a keyhit was clicked by cr (enter) or mouse to activate the button this turn
        /// </summary>
        public bool clickedThisFrame = false;

        /// <summary>
        /// true if a keyhit was clicked by cr (enter) or mouse to select the button this turn
        /// </summary>
        public bool selectedThisFrame = false;



        public Color  highlightColor = Color.Black;

        RC_RenderableList rlist = new RC_RenderableList();

        public RC_GuiMenuRenderable()
        {

        }

        public void add(RC_RenderableBounded ren, int switchValQ, float valQ)
        {
            rlist.addToEnd(ren);
            ren.tag = valQ;
            ren.tagInt = switchValQ;
            if (!initalized)
            {
                val = valQ;
                switchVal = switchValQ;
                currentFocusIndex = 0; // the currently selected one
                initalized = true;
            }
        }

        public void addBelow(RC_RenderableBounded ren, int switchValQ, float valQ, int gapInPixels)
        {
            RC_RenderableBounded lastOne = (RC_RenderableBounded)rlist[rlist.Count() - 1];
            add(ren, switchValQ, valQ);
            // now set bounds
            Rectangle r = lastOne.bounds;
            r.Y = r.Y + gapInPixels + r.Height;
            ren.bounds = r;
        }

        public void addBeside(RC_RenderableBounded ren, int switchValQ, float valQ, int gapInPixels)
        {
            RC_RenderableBounded lastOne = (RC_RenderableBounded)rlist[rlist.Count() - 1];
            add(ren, switchValQ, valQ);
            // now set bounds
            Rectangle r = lastOne.bounds;
            r.X = r.X + gapInPixels + r.Width;
            ren.bounds = r;
        }

        public override void Draw(SpriteBatch sb)
        {
            rlist.Draw(sb);
            Rectangle bounds = ((RC_RenderableBounded)(rlist[currentFocusIndex])).bounds;
            LineBatch.drawLineRectangleOuter(sb, bounds, highlightColor,3);
        }

        public override void Update(GameTime gameTime)
        {
            clickedThisFrame = false;
            selectedThisFrame = false;
            rlist.Update(gameTime);
        }

        public override  bool MouseClickEvent(float mouse_x, float mouse_y)
        {
            foreach (RC_Renderable ren in rlist)
            {

                //RC_RenderableBounded rb = ((RC_RenderableBounded)(rlist[currentFocusIndex]));
                bool c = ((RC_RenderableBounded)ren).Contains((int)mouse_x, (int)mouse_y);
                if (c)
                {
                    int i = rlist.IndexOf(ren);
                    if (i == currentFocusIndex) { clickedThisFrame = true; return true; }
                    currentFocusIndex = i;
                    selectedThisFrame = true;
                    return true;
                }
            }
            return false;
        }

        public override bool KeyHitEvent(Keys keyHit)
        {
            if (keyHit == Keys.Down)
            {
                RC_RenderableBounded rb = ((RC_RenderableBounded)(rlist[currentFocusIndex]));
                int i = rlist.IndexOfNext(rb);
                currentFocusIndex = i;
                selectedThisFrame = true;
                return true;
            }

            if (keyHit == Keys.Up)
            {
                RC_RenderableBounded rb = ((RC_RenderableBounded)(rlist[currentFocusIndex]));
                int i = rlist.IndexOfPrevious(rb);
                currentFocusIndex = i;
                selectedThisFrame = true;
                return true;
            }

            if (keyHit == Keys.Enter || keyHit == Keys.Space)
            {
                clickedThisFrame = true;
            }

            return false;
        }


        //public override void Reset()
        //{
        //}

    }
}
