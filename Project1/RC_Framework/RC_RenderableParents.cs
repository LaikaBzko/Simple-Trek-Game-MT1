using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Parent class for all renderables sprites and list of things to draw has active flag, color, and visible properties
    /// and draw, load contenet and update functions
    /// also mouse and keyhit functions
    /// 
    /// Recently added tag and tagInt to help making general purpose renderables 
    /// </summary>
    public class RC_Renderable
    {

        /// <summary>
        /// this variable called active is used by things like renderable_list, sprite_list and gui_list
        /// if a renderable is not active it means it can be deleted or overwritten
        /// it is used in conjunction with sprite list to manage activity of sprites in a list
        /// importantly inactive renderables should not be given Draw or update (or other UI) events
        /// </summary>
        public bool active { set; get; }

        /// <summary>
        /// This is the render colour its usually pushed into the spritebatch draw unchanged
        /// </summary>
        public Color colour { get; set; }

        /// <summary>
        /// is it visible
        /// </summary>
        public bool visible { get; set; }

        /// <summary>
        /// is it able to be used in Charles drag and drop designer ?
        /// if true it can be used
        /// it defauts to false
        /// </summary>
        public bool dragAndDrop { get; set; }

        /// <summary>
        /// only used if dragAndDrop is true
        /// current state of the control
        /// 0=Runtime
        /// 1=Design time
        /// ? 2=Editing Properties
        /// ? 3=Editing Hotspots
        /// ? 4=Editing Waypoints
        /// </summary>
        public bool dragAndDropState { get; set; }

        /// <summary>
        /// this variable called tag is available for use by user code
        /// </summary>
        public float tag { get; set; }

        /// <summary>
        /// this variable called tagInt is available for use by user code
        /// </summary>
        public int tagInt { get; set; }

        /// <summary>
        /// This variable called text is just for users to attach a string to the renderable or sprite
        /// it is used by the attached renderable AttachedText and some other renderables (eg text ones and buttons) 
        /// </summary>
        public string text = "";

        /// <summary>
        /// a default constructor
        /// </summary>
        public RC_Renderable()
        {
            active = true;
            visible = true;
            colour = Color.White;
        }

        /// <summary>
        /// a more usefull constructor
        /// </summary>
        /// <param name="activeZ"></param>
        /// <param name="visibleZ"></param>
        /// <param name="colorZ"></param>
        public RC_Renderable(bool activeZ, bool visibleZ, Color colorZ)
        {
            active = activeZ;
            visible = visibleZ;
            colour = colorZ;
            dragAndDrop = false;
        }

        /// <summary>
        /// Copy constructor dont know if it will ever be used since its a parent class
        /// </summary>
        public RC_Renderable(RC_Renderable r)
        {            
            active = r.active;
            visible = r.visible;
            colour = r.colour;
            tag = r.tag;
            tagInt = r.tagInt;
            dragAndDrop = false;
        }

        /// <summary>
        /// Sets the blend colour for the sprite if in doubt use Color.White
        /// </summary>
        /// <param name="c"></param>
        public virtual void setColor(Color c)
        {
            colour = c;
        }

        /// <summary>
        /// Gets the sprite blend colour
        /// </summary>
        /// <returns></returns>
        public Color getColor()
        {
            return colour;
        }

        /// <summary>
        /// Standard draw routine which assumes the renderable knows where it is
        /// </summary>
        /// <param name="sb"></param>
        public virtual void Draw(SpriteBatch sb)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void LoadContent()
        {
        }

        public virtual void reset()
        {
        }

        /// <summary>
        /// Set if the renderable is visible 
        /// </summary>
        /// <param name="v"></param>
        public void setVisible(bool v)
        {
            visible = v;
        }

        /// <summary>
        /// Um this should be obvious
        /// </summary>
        /// <returns></returns>
        public bool getVisible()
        {
            return visible;
        }

        /// <summary>
        /// Used to scroll the entire screen - in renderable simple but increases in complexity as the 
        /// parent child tree deepens (eg sprite3)
        /// </summary>
        /// <param name="x">the amount to scroll in x </param>
        /// <param name="y">the amount to scroll in y</param>
        /// <returns> true if successfull </returns>
        public virtual bool scrollMove(float x, float y)
        {
            return true;
        }

        /// <summary>
        /// Events in the this class can be consumed so default behavious must be to return false
        /// this will indicate that the event was not consumed
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="previousMs"></param>
        /// <returns></returns>
        public virtual bool MouseMoveEvent(MouseState ms, MouseState previousMs) { return false; }
        public virtual bool MouseDownEventLeft(float mouse_x, float mouse_y) { return false; }
        public virtual bool MouseUpEventLeft(float mouse_x, float mouse_y) { return false; }
        public virtual bool MouseDownEventRight(float mouse_x, float mouse_y) { return false; }
        public virtual bool MouseUpEventRight(float mouse_x, float mouse_y) { return false; }
        public virtual bool MouseClickEvent(float mouse_x, float mouse_y) { return false; }
        public virtual bool KeyHitEvent(Keys keyHit) { return false; }
        public virtual bool MouseOver(float mouse_x, float mouse_y) { return false; } // for tool tips and any other mouse overs

    }



    // ------------------------------------------------ RC_RenderableBounded ----------------------------------

    /// <summary>
    /// This class is a parent for a range of renderable class's with a rectangle for bounds
    /// it allows seting the bounds by routines that resize it for zooming  
    /// the bounds are definitly the render bounds not a collision box (though of course they may be the same)
    /// </summary>
    public class RC_RenderableBounded : RC_Renderable
    {
        /// <summary>
        /// bounds are a rectange property that defines the location
        /// </summary>
        public virtual Rectangle bounds
        {
            get { return boundz; }
            set { boundz = value; }
        }

        /// <summary>
        /// Used by drag and drop tools to construct game in a wysiwyg like way
        /// </summary>
        private BRHelperParent guih;

        public BRHelperParent Guih
        {
            get { return guih; }
            set
            {
                guih = value;
                guih.parent = this;
            }
        }



        protected Rectangle boundz;

        /// <summary>
        /// Set the x and y position of this renderable
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual void setPos(float x, float y)
        {
            boundz.X = (int)x;
            boundz.Y = (int)y;
        }

        /// <summary>
        /// Used to scroll the entire screen - while preserving old position 
        /// usually called in renderable List 
        /// Will have problemes with non integral scroll amounts (ie fractions)
        /// </summary>
        /// <param name="x">the amount to scroll in x </param>
        /// <param name="y">the amount to scroll in y</param>
        /// <returns> true if successfull </returns>
        public override bool scrollMove(float x, float y)
        {
            boundz.X = boundz.X + (int)Math.Round(x,0);
            boundz.Y = boundz.Y + (int)Math.Round(y,0);
            return true;
        }


        /// <summary>
        /// set the width and height
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public void setWidthHeight(int w, int h)
        {
            boundz.Width = w;
            boundz.Height = h;
        }

        /// <summary>
        /// Returns true if the renderable contains the point x,y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Contains(int x, int y)
        {
            return bounds.Contains(x, y);
        }

        /// <summary>
        /// Returns a rectangle which is the intersection of this renderable with another rectangle
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public Rectangle Intersect(Rectangle r)
        {
            return Rectangle.Intersect(bounds, r);
        }

        /// <summary>
        /// Returns true if the rectange intersects this this renderable
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool Intersects(Rectangle r)
        {
            return bounds.Intersects(r);
        }

        /// <summary>
        /// Returns Various points in the rectangle 
        /// </summary>
        /// <param name="subLocNum"></param>
        /// <returns></returns>
        public Vector2 getSubLocation(int subLocNum)
        {
            return Util.getSubLocation(bounds, subLocNum);
        }

        /// <summary>
        /// Returns true if inside or on boundary of r
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool inside(Rectangle r)
        {
            if (bounds.X < r.X) return false;
            if (bounds.X + bounds.Width > r.X + r.Width) return false;
            if (bounds.Y < r.Y) return false;
            if (bounds.Y + bounds.Height > r.Y + r.Height) return false;
            return true;
        }
    }



    // ********************************************************************* renderable GUIHelper ******************************************************
    /// <summary>
    /// A parent class for renderables attached to sprites that can be drawn after the sprite
    /// it lives off of the sprites update and draw sequences
    /// usefull for health bars and things
    /// </summary>   
    public class BRHelperParent
    {
        [Category("Positin And Size")]
        public Rectangle bounds;

        public RC_RenderableBounded parent;

        public virtual void informParent()
        {
            parent.bounds = bounds;
            // in subclass do max hit point here 
        }

    }



    /// // ********************************************************************* attached renderable ******************************************************
    /// <summary>
    /// A parent class for renderables attached to sprites that can be drawn after the sprite
    /// it lives off of the sprites update and draw sequences
    /// usefull for health bars and things
    /// </summary>
    public class RC_RenderableAttached : RC_Renderable
    {
        internal Sprite3 parent = null;

        public void setParent(Sprite3 parentZ)
        {
            parent = parentZ;
        }
    }


    }


