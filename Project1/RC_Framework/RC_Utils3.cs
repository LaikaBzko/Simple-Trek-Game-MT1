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


#pragma warning disable 1591 //sadly not yet fully commented

namespace RC_Framework
{
    /// <summary>
    /// ColorTicker generates a colour that is between two colours based on ticks (timer)
    /// to get the current colour use: method currColor()
    /// dont forget to run Update() each turn
    /// ending conditions are governed by the loop parameter
    /// loop values are: 0=end (stay final colour), 1=Loop, 2=reverse
    /// </summary>   
    public class ColorTicker
    {
        Color finalColour;
        Color initColour;
        int fadeTicks;
        public int loop; // 0=end (stay final colour), 1=Loop, 2=reverse
        int ticks;
        bool reverse;
        float lerp;
        Color curColour;
        
        public ColorTicker()
        {
            ticks = 0;
            fadeTicks = 10;        
            finalColour = Color.Black;
            initColour = Color.White;        
            reverse = false;
            lerp = 0; 
            curColour = initColour;
            loop = 0;
        }

        public ColorTicker(ColorTicker c)
        {
        finalColour = c.finalColour;
        initColour = c.initColour;
        ticks = c.ticks;
        fadeTicks = c.fadeTicks;
        loop = c.loop; // 0=end (stay final colour), 1=Loop, 2=reverse
        reverse = c.reverse;
        lerp = c.lerp;
        curColour = c.curColour;
        }

        public ColorTicker(Color fromColour, Color toColour, int fadeTicksQ)
        {
            finalColour = toColour;
            initColour = fromColour;
            ticks = 0;
            fadeTicks = fadeTicksQ;
            loop = 0; // 0=end (stay final colour), 1=Loop, 2=reverse
            reverse = false;
            lerp = 0;
            curColour = initColour;
        }

        public ColorTicker(Color fromColour, Color toColour, float secondsQ, int ticksPerSecond)
        {
            finalColour = toColour;
            initColour = fromColour;
            ticks = 0;
            fadeTicks = (int)(secondsQ * ticksPerSecond);
            loop = 0; // 0=end (stay final colour), 1=Loop, 2=reverse
            reverse = false;
            lerp = 0;
            curColour = initColour;
        }

        public bool finished()
        {
            if (ticks > fadeTicks) return true;
            return false;
        }

        public void reset()
        {
            ticks = 0;
            curColour = initColour;
            reverse = false;
        }

        public Color currColor()
        {
            return curColour;
        }

        public Color currColorInverse()
        {
            lerp = (float)ticks / (float)fadeTicks;
            if (!reverse) lerp = 1 - lerp;

            Color curColourI = Color.Lerp(initColour, finalColour, lerp);
            return curColourI;
        }

        public void Update()
        {
            ticks++;
            if (ticks > fadeTicks)
            {
                if (loop == 0)
                {
                    return;
                }
                if (loop == 1)
                {
                    ticks = 0;
                    return;
                }
                if (loop == 2)
                {
                    ticks = 0;
                    reverse = !reverse;
                    return;
                }


            }
            lerp = (float)ticks / (float)fadeTicks;
            if (reverse) lerp = 1 - lerp;

            curColour = Color.Lerp(initColour, finalColour, lerp);
        }

        public void setLoop(int loopQ)
        {
            loop = loopQ;
        }
    }


    

// *********************************************** MakeTex Code Shell  *************************************************************************
    /*
    /// <summary>
    /// MakeTex a class for making square textures of a given colour - its a code shell for copy and pasting not a class
    /// </summary>
    public class MakeTex
    {
        public static Texture2D rectangleBorder(GraphicsDevice device, Rectangle r, Color c, Color borderC, int borderWidth)
        {
            Texture2D tex = new Texture2D(device, r.Width, r.Height, false, SurfaceFormat.Color);
            Color[] data = new Color[r.Width * r.Height];

            //Color cLight = Util.lighterOrDarker(c, 1.4f);
            //Color cDark = Util.lighterOrDarker(c, 0.6f);

            for (int x = 0; x < r.Width; x++)
            {
            }
            return tex;
        }

    }
    */
}
