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
    // ************************ class RC_Frame ************************************************************************ //

    public class RC_Frame : FrameSource
    {
        public Rectangle source;
        public Texture2D tex;

        /// <summary>
        /// Bounding Box rectange in source pixels
        /// x and y are offset from the HOTSPOT
        /// Warning if the hotspot is in the middle of the image/frame then x,y will be negative
        /// For users to use (posibbly in update if you need it) currently sprite3 ignores this 
        /// </summary>
        public Rectangle bb;

        /// <summary>
        /// hotspot offset x and y from top left corner of
        /// either the image - or single frame if its an animation
        /// hotspot offet is in pixels in the souce image 
        /// For users to use (posibbly in update if you need it) currently sprite3 ignores this 
        /// </summary>
        public Vector2 hotSpotOffset; //

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="sourceZ"></param>
        /// <param name="texZ"></param>
        public RC_Frame(Rectangle sourceZ, Texture2D texZ)
        {
            source = sourceZ;
            tex = texZ;
        }

        /// <summary>
        /// create frame from file 
        /// </summary>
        /// <param name="sourceZ"></param>
        /// <param name="texZ"></param>
        public RC_Frame(GraphicsDevice gd, string dir, string fileName)
        {
            tex = Util.texFromFile(gd, dir + fileName);
            source.X = 0;
            source.Y = 0;
            source.Width = tex.Width;
            source.Width = tex.Height;            
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="f"></param>
        public RC_Frame(RC_Frame f)
        {
            source = f.source;
            tex = f.tex;
        }

        /// <summary>
        /// so that a single frame can be a frame source
        /// it ignores the frame number (use 0 if it really matters to you) eg getFrame(0)
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public override RC_Frame getFrame(int num)
        {
            return this;
        }

        /// <summary>
        /// so that a single frame can be a frame source
        /// it ignores the frame number (use 0 if it really matters to you) eg getFrame()
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public override RC_Frame getFrame()
        {
            return this;
        }

        /// <summary>
        ///  draw it at a given 2d loaction 
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="pos"></param>
        /// <param name="col"></param>
        public void draw(SpriteBatch sb, Vector2 pos, Color col )
        {
            sb.Draw(tex,new Rectangle((int)pos.X,(int)pos.Y,source.Width,source.Height),source,col);
        }

        /// <summary>
        ///  draw it at a given 2d loaction 
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="pos"></param>
        /// <param name="col"></param>
        public void draw(SpriteBatch sb, Rectangle rect, Color col )
        {
            sb.Draw(tex,rect,source,col);
        }
    }

    // ************************ class RC_FrameSource ************************************************************************ //

    /// <summary>
    /// Abstract class to get a numbered frame 
    /// </summary>
    public abstract class FrameSource
    {
        /// <summary>
        ///  this one method must be overridden and return the current frame or null
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public abstract RC_Frame getFrame(); 

        /// <summary>
        ///  this method is usually overridden and returns a numbered frame for any given number (or null)
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public abstract RC_Frame getFrame(int num);

        /// <summary>
        /// Update so the frames can advance if necessary
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime) {} // default do nothing
        
    }

    
    // ************************ class FrameList ************************************************************************ //

    public class FrameList : FrameSource
    {
        public List<RC_Frame> flist;

        /// <summary>
        /// Overload [ and ] for ease of use
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public RC_Frame this[int key]
        {
            get
            {
                return getFrame(key);
            }
            set
            {
                setFrame(key, value);
            }
        }

        public FrameList()
        {
            flist = new List<RC_Frame>();
        }

        /// <summary>
        /// Adds a frame to the list 
        /// 
        /// </summary>
        /// <param name="r"></param>
        public void addToEnd(RC_Frame f)
        {
            flist.Add(f);
        }

        /// <summary>
        /// return a specific frame based on number
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public override RC_Frame getFrame(int num)
        {
            if (num < 0) return null;
            if (num < flist.Count()) return flist[num];
            return null;
        }

        /// <summary>
        /// Just a usefull default behaviour so everything compiles 
        /// will generally not get used
        /// </summary>
        /// <returns></returns>
        public override RC_Frame getFrame()
        {
            if ( flist.Count == 0) return null;
            return flist[0];
        }

        public bool setFrame(int num, RC_Frame f)
        {
            if (num < 0) return false;
            if (num < flist.Count())
            {
                flist[num] = new RC_Frame(f);
                return true; 
            } 
            return false;
        }

        public int Count()
        {
            return flist.Count();
        }
    }

    // ************************ class AnimationFrameList ************************************************************************ //

    public class AnimationFrameList : FrameSource
    {

        /// <summary>
        /// A complicated animation class using a timer of class called AnimationTicker 
        /// and a list of frames of class called FrameList 
        /// 
        /// 
        /// To use this class its normal to call it as follows:
        /// 
        /// // first here set up the frame list ....
        /// frList FrameList = new FrameList();
        /// frList.add(tex, new Rectangle(0,0,100,100);
        /// frList.add(tex, new Rectangle(0,0,200,100);
        /// frList.add(tex2, new Rectangle(0,0,100,100);
        ///
        /// // now set up the animation ticker
        /// int aseq[] = new aseq[3];
        /// AnimationTicker anTicker = new AnimationTicker(aSeq, 0, 3, 9, 0);
        /// 
        /// // now combine them to what you want 
        /// AnimationFrameList afl = new AnimationFrameList(frList, anTicker);
        /// afl.animationTicker.animationStart();
        /// 
        /// // finally put them in the sprite or renderable
        /// sprite.frameSource = afl; 
        /// 
        /// // finally start the animation using the animation ticker 
        /// afl.animationTicker.animationStart();
        /// 
        /// </summary>
        public AnimationTicker animationTicker=null;
        protected FrameList frameList=null; // used only at setup
        public FrameSource frameSource=null;

        public AnimationFrameList(FrameList frameListZ, AnimationTicker animTickerZ)
        {
            frameList = frameListZ;
            frameSource = frameListZ;
            animationTicker = animTickerZ;
        }

        public AnimationFrameList(FrameSource frameSrc, AnimationTicker animTickerZ)
        {
            frameList = null;
            frameSource = frameSrc;
            animationTicker = animTickerZ;
        }


        /// <summary>
        ///  This one method must be overridden and return a numbered frame for any given number (or null)
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public override RC_Frame getFrame(int num)
        {
            return frameSource.getFrame(num);
        }        
        
        /// <summary>
        ///  This  method must be overridden and return the current frame (or null)
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public override RC_Frame getFrame()
        {
            return frameSource.getFrame(animationTicker.getFrameNum());
        }

        /// <summary>
        /// Update so the frames can advance if necessary
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            animationTicker.Update(gameTime);
            frameSource.Update(gameTime);
        }

        /// <summary>
        /// Wrapper for animation ticker running flag
        /// </summary>
        /// <param name="runningQ"></param>
        public void  setRunning(bool runningQ)
        {
            animationTicker.running = runningQ;
        }

    }
    
    
    
    // ************************ class Animation Ticker ************************************************************************ //

    /// <summary>
    /// This class just manages a counter to tell the current frame number
    /// </summary>
    public class AnimationTicker  
    {
        int ticks = 0;
        int frame = 0;
        int frameIndex = 0;
        int[] animationSeq = null;
        int firstFrame=0;
        int lastFrame=0;
        int ticksBetweenFrames=-1; // -1 means do nothing

        /// <summary>
        /// make it tick or not
        /// </summary>
        public bool running = false;
        
        /// <summary>
        /// A flag telling what to do when the animation finishes
        /// 0 = loop
        /// 1 = set first frame  and set running = false
        /// 2 = set last frame   and set running = false
        /// 3 = set frame = 0    and set running = false
        /// 
        /// </summary>
        int animFinished;

        /// <summary>
        /// create an animation ticker
        /// </summary>
        /// <param name="animSeq"></param>
        /// <param name="firstFrameZ"></param>
        /// <param name="lastFrameZ"></param>
        /// <param name="ticksBetweenFramesZ"></param>
        /// <param name="animFinishedZ">0=loop, 1=firstframe, 2=lastframe, 3=frame0</param>
        public AnimationTicker(int[] animSeq, int firstFrameZ, int lastFrameZ, int ticksBetweenFramesZ, int animFinishedZ) // the frame sequence
        {

            animationSeq = animSeq;
            firstFrame = firstFrameZ;
            lastFrame = lastFrameZ;
            ticksBetweenFrames = ticksBetweenFramesZ;
            animFinished = animFinishedZ;
        }

        public void resetAnimationTicker(int[] animSeq, int firstFrameZ, int lastFrameZ, int ticksBetweenFramesZ, int animFinishedZ) // the frame sequence
        {

            animationSeq = animSeq;
            firstFrame = firstFrameZ;
            lastFrame = lastFrameZ;
            ticksBetweenFrames = ticksBetweenFramesZ;
            animFinished = animFinishedZ;
        }

        /// <summary>
        /// Set the start parameters for the animation sequence
        /// </summary>
        public void animationStart()
        {
            frameIndex = firstFrame;
            ticks = 0;
            running = true;
            frame = animationSeq[frameIndex];
        }

        /// <summary>
        /// A flag telling what to do when the animation finishes
        /// 0 = loop
        /// 1 = set first frame 
        /// 2 = set last frame 
        /// 3 = set frame = 0 
        /// </summary>
        /// <param name="whatToDo"></param>
        public void setAnimFinished(int whatToDo)
        {
            animFinished = whatToDo;
        }

        /// <summary>
        /// gets the current frame number - if update is called then all should be good
        /// 
        /// </summary>
        /// <returns></returns>
        public int getFrameNum()
        {
            return frame;
        }

        /// <summary>
        /// can be used to temporarily pause an animation
        /// </summary>
        /// <param name="state"></param>
        public void pauseAnimation(bool state)
        {
            running = state;
        }

        /// <summary>
        /// generally you wont need this but in theory it can reset an animation to a new point 
        /// </summary>
        /// <param name="frameZ"></param>
        public void setFrameIndex(int frameIndexZ)
        {
            frameIndex = frameIndexZ;
        }

        /// <summary>
        /// generally you wont need this and you will need to set running to false for it to stop animation
        /// </summary>
        /// <param name="frameZ"></param>
        public void setFrame(int frameZ)
        {
            frame = frameZ;
        }

        /// <summary>
        /// Update the animation sequence
        /// This routine is usually run in the update routine
        /// this routine has gameTime as a parameter
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (animationSeq == null) return;
            if (ticksBetweenFrames == -1) return; // not initialised
            if (!running) return;

            ticks++;
            if (ticksBetweenFrames != 0)
                if (ticks % ticksBetweenFrames == 0)
                {
                    frameIndex = frameIndex + 1;
                    if (frameIndex > lastFrame)
                    {
                        if (animFinished == 0)
                        {
                            frameIndex = firstFrame;
                            frame = animationSeq[frameIndex];
                        }
                        if (animFinished == 1)
                        {
                            frameIndex = firstFrame;
                            frame = animationSeq[frameIndex];
                            running = false;
                        }
                        if (animFinished == 2)
                        {
                            frameIndex = lastFrame;
                            frame = animationSeq[frameIndex];
                            running = false;
                        }
                        if (animFinished == 3)
                        {
                            frame = 0;
                            running = false;
                        }

                    }
                    else
                    {
                        frame = animationSeq[frameIndex];
                    }
                }
        }
    }

    // ************************ class RenderableFrame ************************************************************************ //

    /// <summary>
    /// A renderable frame similar in most respects to the class ImageBackground
    /// </summary>
    public class RenderableFrame : RC_RenderableBounded
    {
        RC_Frame frame;

        public RenderableFrame(RC_Frame frameZ, Rectangle destZ, Color colourZ)
        {
            frame = new RC_Frame(frameZ);
            bounds = new Rectangle(destZ.X, destZ.Y, destZ.Width, destZ.Height);
            colour = colourZ;
        }

        public RenderableFrame(RC_Frame frameZ, Color colourZ, GraphicsDevice g)
        {
            frame = new RC_Frame(frameZ);
            bounds = new Rectangle(0, 0, g.Viewport.Width, g.Viewport.Height);
            colour = colourZ;
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(frame.tex, bounds, frame.source, colour);
        }
    }


}
