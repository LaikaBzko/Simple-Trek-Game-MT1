using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Storage;

//
// Changed 10/3/2022 to fix error in first time use of for each loop
//
//
//
//
//

namespace RC_Framework
{
    // ***---------------------****************************** renderable list *********************--------*********************
    /// <summary>
    /// a list of renderables
    /// the idea of the active flag is is that inactive renderables can have their place re-used and are no longer needed
    /// </summary>
    public class RC_RenderableList : RC_Renderable, IEnumerator, IEnumerable
    {

        public List<RC_Renderable> rlist;

        int position=-1; // for IEnumerator, IEnumerable

        public RC_RenderableList()
        {
            rlist = new List<RC_Renderable>();
        }

        /// <summary>
        /// Overload [ and ] for ease of use
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public RC_Renderable this[int key]
        {
            get
            {
                return getRenderable(key);
            }
            set
            {
                //SetValue(key, value);
                rlist[key] = value;
            }
        }

        public bool MoveNext() // for IEnumerator
        {
            do
            {
                position++;
                if (position >= rlist.Count)
                {
                    position = -1;
                    return false;
                }
            } while (rlist[position] == null || !rlist[position].active);
            return true;  //(position < rlist.Count);
        }

        public void Reset() // for IEnumerator
        {
            position = 0;
            do
            {
                position++;
                if (position >= rlist.Count)
                {
                    position = -1; break;
                }
            } while (rlist[position] == null || !rlist[position].active);
        }

        //        public object Current // for IEnumerator
        public object Current // for IEnumerator
        {
            get
            {
                if (position != -1)
                {
                    return rlist[position];
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public void Dispose() // for IEnumerator
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator() // for IEnumerable
        {
            return (IEnumerator)this;
        }

        //RC_Renderable IEnumerator<RC_Renderable>.Current
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        /// <summary>
        /// draw the list of renderables renderables are responsible for knowing their position
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < rlist.Count; i++)
            {
                if (rlist[i].active)
                {
                    rlist[i].Draw(sb);
                }
            }
        }

        /// <summary>
        /// run update on the entire list of renderables 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < rlist.Count; i++)
            {
                if (rlist[i].active)
                {
                    rlist[i].Update(gameTime);
                }
            }
        }

        /// <summary>
        /// runs scroll move on all renderables 
        /// used to scroll the screen 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool scrollMove(float x, float y)
        {
            bool rc = true;
            for (int i = 0; i < rlist.Count; i++)
            {
                if (rlist[i].active)
                {
                    if (rlist[i].scrollMove(x, y)) rc = false;
                }
            }
            return rc;
        }

        /// <summary>
        /// For initialisation
        /// </summary>
        public override void LoadContent()
        {
            for (int i = 0; i < rlist.Count; i++)
            {
                rlist[i].LoadContent();
            }
        }

        /// <summary>
        /// This returns the index of a given renderable
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(RC_Renderable item)
        {
            return rlist.IndexOf(item);
        }

        /// <summary>
        /// This returns the index of the previous renderable
        /// in most cases it makes no senseits probably only usefull in gui applications
        /// it assumes the renderables are placed in the list an a meaningfull order
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOfPrevious(RC_Renderable item)
        {
            int retv = rlist.IndexOf(item);
            if (retv < 0) return retv;
            if (retv == 0) return 0;
            return retv - 1;
        }

        /// <summary>
        /// This returns the index of the next renderable
        /// in most cases it makes no senseits probably only usefull in gui applications
        /// it assumes the renderables are placed in the list an a meaningfull order
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOfNext(RC_Renderable item)
        {
            int retv = rlist.IndexOf(item);
            if (retv < 0) return retv;
            if (retv == rlist.Count - 1) return rlist.Count - 1;
            return retv + 1;
        }

        /// <summary>
        /// standard way to add things to the list reusing old space to 
        /// stop the list growing to long
        /// </summary>
        /// <param name="r"></param>
        public void addReuse(RC_Renderable r)
        {
            int i = findInactive();
            if (i == -1) rlist.Add(r);
            else rlist[i] = r;
        }

        /// <summary>
        /// standard way to add things to the list reusing old space to 
        /// stop the list growing to long
        /// </summary>
        /// <param name="r"></param>
        public void addSpriteReuse(RC_Renderable r)
        {
            int i = findInactive();
            if (i == -1) rlist.Add(r);
            else rlist[i] = r;
        }

        /// <summary>
        /// Adds to the list - fast but usually we would use the 'addSpriteReuse' method
        /// </summary>
        /// <param name="r"></param>
        public void addToEnd(RC_Renderable r)
        {
            rlist.Add(r);
        }

        /// <summary>
        /// internal use mainly
        /// </summary>
        /// <returns></returns>
        public int findInactive()
        {
            for (int i = 0; i < rlist.Count; i++)
            {
                if (!rlist[i].active) return i;
            }
            return -1;
        }

        /// <summary>
        /// replaces returning the old value - not especially usefull
        /// </summary>
        /// <param name="num"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public RC_Renderable replace(int num, RC_Renderable r)
        {
            RC_Renderable retv = rlist[num];
            rlist[num] = r;
            return retv;
        }

        /// <summary>
        /// gets a renderable i
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public RC_Renderable getRenderable(int i)
        {
            RC_Renderable retv = rlist[i];
            return retv;

        }

        /// <summary>
        /// Count of renderable 
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return rlist.Count;
        }

        /// <summary>
        /// If the renderable list contains RC_RenderableBounded it will return
        /// the first renderable index that this point in bounds or -1 if not found
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Contains(float x, float y)
        {
            for (int i = 0; i < rlist.Count; i++)
            {
                if (rlist[i] == null) continue;
                if (!rlist[i].active) continue;
                if (!rlist[i].visible) continue;
                if (rlist[i] is RC_RenderableBounded)
                {
                    if (((RC_RenderableBounded)(rlist[i])).bounds.Contains((int)x, (int)y))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Checks for collision with rectangle for bounded renderables only 
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public int collision(Rectangle r)
        {
            for (int i = 0; i < rlist.Count; i++)
            {
                if (rlist[i] == null) continue;
                if (!rlist[i].active) continue;
                if (!rlist[i].visible) continue;
                if (((RC_RenderableBounded)(rlist[i])).bounds.Intersects(r))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Sets colour for all active renderables
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public override void setColor(Color c)
        {
            for (int i = 0; i < rlist.Count; i++)
            {
                if (rlist[i] == null) continue;
                if (!rlist[i].active) continue;
                //if (!rlist[i].visible) continue;
                rlist[i].setColor(c);
            }
            return;
        }

        /// <summary>
        /// Counts the active renderables
        /// deprecated please use countActive
        /// </summary>
        /// <returns></returns>
        public int countOfActive()
        {

            return countActive();
        }

        public override bool MouseOver(float mouse_x, float mouse_y)
        {
            bool retv = false;
            for (int i = 0; i < rlist.Count; i++)
            {
                if (rlist[i] != null && rlist[i].active && rlist[i].visible)
                {
                    bool rc = rlist[i].MouseOver(mouse_x, mouse_y);
                    if (rc) retv = true;
                }
            }
            return retv;
        }

        /// <summary>
        /// Propagates a mouse down on left button till consumed
        /// </summary>
        /// <param name="mouse_x"></param>
        /// <param name="mouse_y"></param>
        /// <returns></returns>
        public override bool MouseDownEventLeft(float mouse_x, float mouse_y)
        {
            bool retv = false;
            for (int i = 0; i < rlist.Count; i++)
            {
                if (rlist[i] != null && rlist[i].active && rlist[i].visible)
                {
                    bool rc = rlist[i].MouseDownEventLeft(mouse_x, mouse_y);
                    if (rc)
                    {
                        retv = true;
                        return retv; // its consumed dont propagate further
                    }
                }
            }
            return retv;
        }

        /// <summary>
        /// Propagates a mouse click till consumed
        /// </summary>
        /// <param name="mouse_x"></param>
        /// <param name="mouse_y"></param>
        /// <returns></returns>
        public override bool MouseClickEvent(float mouse_x, float mouse_y)
        {
            bool retv = false;
            for (int i = 0; i < rlist.Count; i++)
            {
                if (rlist[i] != null && rlist[i].active && rlist[i].visible)
                {
                    bool rc = rlist[i].MouseClickEvent(mouse_x, mouse_y);
                    if (rc)
                    {
                        retv = true;
                        return retv; // its consumed dont propagate further
                    }
                }
            }
            return retv;
        }

        /// <summary>
        /// propagates a keyhit event till its consumed
        /// </summary>
        /// <param name="keyHit"></param>
        /// <returns></returns>
        public override bool KeyHitEvent(Keys keyHit)
        {
            bool retv = false;
            for (int i = 0; i < rlist.Count; i++)
            {
                if (rlist[i] != null && rlist[i].active && rlist[i].visible)
                {
                    bool rc = rlist[i].KeyHitEvent(keyHit);
                    if (rc)
                    {
                        retv = true;
                        return retv; // its consumed dont propagate further
                    }
                }
            }
            return retv;
        }


        /// <summary>
        /// count of active renderable
        /// </summary>
        /// <returns></returns>
        public int countActive()
        {
            int retv = 0;
            for (int i = 0; i < rlist.Count; i++)
            {
                if (rlist[i] == null) continue;
                if (rlist[i].active) retv++;
            }
            return retv;
        }

        // ************************ method methods (warning to beginners metaprogramming stay clear) ***********************************
        /// <summary>
        /// Run the method on each active renderables
        /// return true only if all functions return true or there are no active renderables
        /// the function has the formal parameters bool name(RC_Renderable s)
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public bool RunTheMethod(Func<RC_Renderable, bool> methodName)
        {
            bool retv = true;
            for (int i = 0; i < rlist.Count; i++)
            {
                if (rlist[i] == null) continue;
                if (rlist[i].active)
                {
                    bool rc = methodName(rlist[i]);
                    if (!rc) retv = false;
                }
            }
            return retv;
        }

    }

    }
