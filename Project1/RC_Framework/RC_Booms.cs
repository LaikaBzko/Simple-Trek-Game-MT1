using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

#pragma warning disable 1591 //sadly not yet fully commented

namespace RC_Framework
{
   
    public class RC_Booms
    {

        public const int numImages=12;
        Texture2D[] boomImages=null;
        GCG_SpriteSheet[] boomSpriteSheets=null;
        //Texture2DSequence[] frameSeqs = null;

        //Sprite5 boom = null; // for the creation


        public RC_Booms(GraphicsDevice gd, string dir_BoomsImages)
        {
            boomImages = new Texture2D[numImages];
            boomImages[0] = Util.texFromFile(gd,dir_BoomsImages+"Boom0.png");
            boomImages[1] = Util.texFromFile(gd, dir_BoomsImages + "Boom1.png");
            boomImages[2] = Util.texFromFile(gd, dir_BoomsImages + "Boom2.png");
            boomImages[3] = Util.texFromFile(gd, dir_BoomsImages + "Boom3.png");
            boomImages[4] = Util.texFromFile(gd, dir_BoomsImages + "Boom4.png");
            boomImages[5] = Util.texFromFile(gd, dir_BoomsImages + "Boom5.png");
            boomImages[6] = Util.texFromFile(gd, dir_BoomsImages + "Boom6.png");
            boomImages[7] = Util.texFromFile(gd, dir_BoomsImages + "Boom7.png");
            boomImages[8] = Util.texFromFile(gd, dir_BoomsImages + "Boom8.png");
            boomImages[9] = Util.texFromFile(gd, dir_BoomsImages + "Boom9.png");
            boomImages[10] = Util.texFromFile(gd, dir_BoomsImages + "Boom10.png");
            boomImages[11] = Util.texFromFile(gd, dir_BoomsImages + "Boom11.png");

            boomSpriteSheets = new GCG_SpriteSheet[numImages]; 
//          boomSpriteSheets[0] = GCG_SpriteSheet(boomImages[0],XframesZ, YframesZ)
            boomSpriteSheets[0] = new GCG_SpriteSheet(boomImages[0],7, 3);
            boomSpriteSheets[1] = new GCG_SpriteSheet(boomImages[1], 4, 1);
            boomSpriteSheets[2] = new GCG_SpriteSheet(boomImages[2],6, 1);
            boomSpriteSheets[3] = new GCG_SpriteSheet(boomImages[3],8, 1);
            boomSpriteSheets[4] = new GCG_SpriteSheet(boomImages[4],10, 2);
            boomSpriteSheets[5] = new GCG_SpriteSheet(boomImages[5],7, 3);
            boomSpriteSheets[6] = new GCG_SpriteSheet(boomImages[6],7, 3);
            boomSpriteSheets[7] = new GCG_SpriteSheet(boomImages[7],7, 3);
            boomSpriteSheets[8] = new GCG_SpriteSheet(boomImages[8],10, 2);
            boomSpriteSheets[9] = new GCG_SpriteSheet(boomImages[9],10, 2);
            boomSpriteSheets[10] = new GCG_SpriteSheet(boomImages[10],7, 2);
            boomSpriteSheets[11] = new GCG_SpriteSheet(boomImages[11],10, 2);

            //frameSeqs = new Texture2DSequence[numImages];
            //frameSeqs[0] = new Texture2DSequence(boomSpriteSheets[0]);
            //frameSeqs[1] = new Texture2DSequence(boomSpriteSheets[1]);
            //frameSeqs[2] = new Texture2DSequence(boomSpriteSheets[2]);
            //frameSeqs[3] = new Texture2DSequence(boomSpriteSheets[3]);
            //frameSeqs[4] = new Texture2DSequence(boomSpriteSheets[4]);
            //frameSeqs[5] = new Texture2DSequence(boomSpriteSheets[5]);
            //frameSeqs[6] = new Texture2DSequence(boomSpriteSheets[6]);
            //frameSeqs[7] = new Texture2DSequence(boomSpriteSheets[7]);
            //frameSeqs[8] = new Texture2DSequence(boomSpriteSheets[8]);
            //frameSeqs[9] = new Texture2DSequence(boomSpriteSheets[9]);
            //frameSeqs[10] = new Texture2DSequence(boomSpriteSheets[10]);
            //frameSeqs[11] = new Texture2DSequence(boomSpriteSheets[11]);

            //frameSeqs[0].setAnimationSequence(0, 19, 2);
            //frameSeqs[1].setAnimationSequence(0, 3, 5);
            //frameSeqs[2].setAnimationSequence(0, 5, 4);

            //frameSeqs[0].setAnimationSequence(0, 14, 4);
            //frameSeqs[1].setAnimationSequence(0, 3, 8);
            //frameSeqs[2].setAnimationSequence(0, 5, 6);
            //frameSeqs[3].setAnimationSequence(0, 7, 6);
            //frameSeqs[4].setAnimationSequence(0, 19, 3);
            //frameSeqs[5].setAnimationSequence(0, 14, 4);
            //frameSeqs[6].setAnimationSequence(0, 20, 3);
            //frameSeqs[7].setAnimationSequence(0, 20, 3);
            //frameSeqs[8].setAnimationSequence(0, 19, 3);
            //frameSeqs[9].setAnimationSequence(0, 19, 3);
            //frameSeqs[10].setAnimationSequence(0, 13, 4);
            //frameSeqs[11].setAnimationSequence(0, 19, 3); 

        }

        /// <summary>
        /// Make a boom sprite if boomn size == 0 then default size is used
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="boomKind"></param>
        /// <param name="boomSize"></param>
        /// <returns></returns>
        public Sprite3 makeBoom(int x, int y, int boomKind, float boomSize)
        {
            //Texture2DSequence fs = new Texture2DSequence(boomSpriteSheets[boomKind]);
            //fs.setAnimationSequence(frameSeqs[boomKind].firstFrame,frameSeqs[boomKind].lastFrame,frameSeqs[boomKind].ticksBetweenFrames);
            Sprite3 boom = new Sprite3(true, boomImages[boomKind], x, y);
            GCG_SpriteSheet ss = boomSpriteSheets[boomKind];
            boom.setXframes(ss.getXframes());
            boom.setYframes(ss.getYframes());
            int q = ss.getXframes() * ss.getYframes();
            Vector2[] animSeq = new Vector2[q];
            int ctr = 0;
            for (int yy = 0; yy < ss.getYframes(); yy++)
                for (int xx = 0; xx < ss.getXframes(); xx++)
                {
                    animSeq[ctr].X = xx;
                    animSeq[ctr].Y = yy;
                    ctr++;
                }
            boom.setAnimationSequence(animSeq, 0, q - 1, 4);
            //boom.setFrameSource(boomSpriteSheets[boomKind], true);
            if (boomSize > 0) boom.setWidthHeight(boomSize, boomSize);  
            boom.animationStart();
            //boom.animInfo = fs;
            boom.setAnimFinished(2);
            return boom;
        }

    }

  
    // --------------------------------- GCG_SpriteSheet --------------------------------------

    /// <summary>
    /// A frame source that is itself a frame (frames rule) the default order is horizontal then vertical
    /// eg for a 12 step animation frame numbers are (0) (1) (2)
    ///                                              (3) (4) (5)
    ///                                              (6) (7) (8)
    ///                                              (9 )(10)(11)
    /// </summary>
    public class GCG_SpriteSheet 
    {
        /// <summary>
        /// The source of the aniamtion
        /// </summary>
        Texture2D sourceFrame = null;

        /// <summary>
        /// Animation information - number of horizontal frames
        /// </summary>
        protected int Xframes = 1; // number of x frames
        internal int XframeWidth;
        internal int XframeGap = 0;
        public bool hotspotMiddle = true; //note set after source middle or topleft


        /// <summary>
        ///  Animation information - number of vertical frames
        /// </summary>
        protected int Yframes = 1; // number of y frames
        internal int YframeHeight;
        internal int YframeGap = 0; // gap between frames

        /// <summary>
        /// Animation information - current horizontal frame number
        /// frame numbers start at 0
        /// </summary>
        protected int Xframe = 0; // number of the current x frame

        /// <summary>
        /// Animation information - current vertical frame number
        /// frame numbers start at 0
        /// </summary>
        protected int Yframe = 0; // number of the current y frame

        ///// <summary>
        ///// the frame withing sequence 'animationSeq'
        ///// </summary>
        //protected int frame;


        public GCG_SpriteSheet(Texture2D f, int XframesZ, int YframesZ)
        {
            sourceFrame = f;
            Xframes = XframesZ;
            Yframes = YframesZ;
            computeFrameParams();
        }

        /// <summary>
        /// set the inter frame gap in source pixels
        /// </summary>
        /// <param name="XframeGapZ"></param>
        /// <param name="YframeGapZ"></param>
        public void setFrames(int XframesZ, int YframesZ)
        {
            Xframes = XframesZ;
            Yframes = YframesZ;
            computeFrameParams();
        }

        public void setFrameGaps(int XframeGapZ, int YframeGapZ)
        {
            XframeGap = XframeGapZ;
            YframeGap = YframeGapZ;
            computeFrameParams();
        }

        private void computeFrameParams()
        {
            if (Xframes != 0 && Yframes != 0)
            {
                XframeWidth = (int)((sourceFrame.Width + XframeGap) / Xframes);
                YframeHeight = (int)((sourceFrame.Height + YframeGap) / Yframes);
            }
        }


        /// <summary>
        /// Set the number of horizontal frames of animation
        /// </summary>
        /// <param name="f"></param>
        public void setXframes(int f) // 
        {
            Xframes = f;
            computeFrameParams();
        }

        /// <summary>
        /// Set the number of vertical frames of animation
        /// </summary>
        /// <param name="f"></param>
        public void setYframes(int f) // 
        {
            Yframes = f;
            computeFrameParams();
        }

        /// <summary>
        /// get the number of vertical frames of animation
        /// </summary>
        /// <returns></returns>
        public int getYframes() // 
        {
            return Yframes;
        }

        /// <summary>
        /// set the number of vertical frames of animation
        /// </summary>
        /// <returns></returns>
        public int getXframes() // 
        {
            return Xframes;
        }

        /// <summary>
        /// get the current horizontal frame
        /// </summary>
        /// <returns></returns>
        public int getXframe() // 
        {
            return Xframe;
        }

        /// <summary>
        /// get the current vertical frame
        /// </summary>
        /// <returns></returns>
        public int getYframe() // 
        {
            return Yframe;
        }

        /// <summary>
        /// set the current vertical frame
        /// </summary>
        /// <param name="f"></param>
        public void setYframe(int f) // 
        {
            Yframe = f;
        }

        /// <summary>
        /// Set the current horizontal frame
        /// </summary>
        /// <param name="f"></param>
        public void setXframe(int f) // 
        {
            Xframe = f;
        }

        //public override Texture2D getFrameNum(int frameNum)
        //{
        //    Xframe = frameNum % Xframes;
        //    Yframe = frameNum / Xframes;
        //    Rectangle r = new Rectangle(Xframe * (XframeWidth + XframeGap) + sourceFrame.source.X, Yframe * (YframeHeight + YframeGap) + sourceFrame.source.Y, XframeWidth, YframeHeight);
        //    Texture2D retv = new Texture2D(sourceFrame.tex, r);
        //    if (hotspotMiddle) retv.hotSpot = new Point(r.X + r.Width / 2, r.Y + r.Height / 2);
        //    else retv.hotSpot = new Point(r.X, r.Y);
        //    return retv;
        //}

    }

}
