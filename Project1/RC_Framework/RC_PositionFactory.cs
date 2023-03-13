using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

#pragma warning disable 1591 //sadly not yet fully commented

namespace RC_Framework
{
    /// <summary>
    /// parent class for all position factories 
    /// </summary>
    public abstract class RC_PositionFactory
    {
        public abstract Vector2 getNextPos(); // return the next position
        public virtual void init() { }
        public virtual void reset() { }
        public virtual void draw(SpriteBatch sb, Color col) { }
     
    }

    // ----------------------------- RC_posInrectangle -- also on vertical or horizontal line with width or height 1 ---------------------------------

    /// <summary>
    /// Creates positions inside the rectangle inclusive of top and left
    /// exclusive of right and bottom
    /// NOTE Also usefull for vertical or horizontal line with width or height 1
    /// </summary>
    public class RC_posInrectangle : RC_PositionFactory
    {
        Random rnd = null;
        Rectangle rect;

        public RC_posInrectangle(Rectangle r, Random rndZ)
        {
            rect = Util.newRectangle(r);
            rnd = rndZ;
        }

        public override Vector2 getNextPos()
        {
            Vector2 retv = new Vector2();
            retv.X = rnd.Next(rect.X, rect.X + rect.Width);
            retv.Y = rnd.Next(rect.Y, rect.Y + rect.Height);
            return retv;
        }

        public override void draw(SpriteBatch sb, Color col)
        {
            LineBatch.drawLineRectangle(sb, rect, col);
        }
    }


    // ----------------------------- RC_posInCircle ---------------------------------------------------------

    /// <summary>
    /// Creates positions inside the rectangle inclusive of top and left
    /// exclusive of right and bottom
    /// </summary>
    public class RC_posInCircle : RC_PositionFactory
    {
        Random rnd = null;
        Vector2 pos = new Vector2(0, 0);
        float radiusMin = 10;
        float radiusMax = 10;

        public RC_posInCircle(Vector2 posZ, float radiusMinZ, float radiusMaxZ, Random rndZ)
        {
            pos = new Vector2(posZ.X, posZ.Y);
            rnd = rndZ;
            radiusMin = radiusMinZ;
            radiusMax = radiusMaxZ;
        }

        public override Vector2 getNextPos()
        {
            double angle = rnd.NextDouble() * 2 * Math.PI;
            double radius = radiusMin + (radiusMax - radiusMin) * rnd.NextDouble();
            Vector2 retv = Util.moveByAngleDist(pos, (float)angle, (float)radius);
            return retv;
        }

        public override void draw(SpriteBatch sb, Color col)
        {
            LineBatch.drawCircle(sb, col, pos, radiusMin, 19, 1);
            LineBatch.drawCircle(sb, col, pos, radiusMax, 19, 1);
        }
    }

    // ----------------------------- RC_posXY ---------------------------------------------------------

    /// <summary>
    /// Creates positions which are just 1 position one x,y on the screen
    /// </summary>
    public class RC_posXY : RC_PositionFactory
    {
        public Vector2 poss = new Vector2(100, 100); // truly insane default but you should be able to see it

        public RC_posXY(Vector2 posQ)
        {
            poss = posQ;
        }

        public override Vector2 getNextPos()
        {
            Vector2 retv = new Vector2(poss.X, poss.Y);

            return retv;
        }

        public override void draw(SpriteBatch sb, Color col)
        {
            LineBatch.drawCross(sb, poss.X, poss.Y, 7, col, col);
        }
    }

    // ----------------------------- RC_posRandomLine ---------------------------------------------------------

    /// <summary>
    /// Creates positions which are just random on an  arbitary line inclusive (includes both end points)
    /// </summary>
    public class RC_posRandomLine : RC_PositionFactory
    {
        public Vector2 posFrom = new Vector2(100, 100); // truly insane default but you should be able to see it
        public Vector2 posTo = new Vector2(200, 200); // truly insane default but you should be able to see it
        Random rnd;

        public RC_posRandomLine(Vector2 fromQ, Vector2 toQ, Random rndZ)
        {
            posFrom = fromQ;
            posTo = toQ;
            rnd = rndZ;
        }

        public override Vector2 getNextPos()
        {
            int minX = (int)Math.Min(posFrom.X,posTo.X);
            int maxX = (int)Math.Max(posFrom.X,posTo.X)+1;
            int minY = (int)Math.Min(posFrom.Y,posTo.Y);
            int maxY = (int)Math.Max(posFrom.Y,posTo.Y)+1;
            Vector2 retv = new Vector2(rnd.Next(minX,maxX),rnd.Next(minY,maxY));
            return retv;
        }

        public override void draw(SpriteBatch sb, Color col)
        {
            LineBatch.drawCross(sb, posFrom.X, posTo.Y, 7, col, col);
        }
    }

    // ----------------------------- RC_posWaypointList ---------------------------------------------------------

    /// <summary>
    /// A class that takes a waypoint list and returns a set of positions
    /// </summary>
    public class RC_posWaypointList : RC_PositionFactory
    {

        public WayPointList wList; 

        public RC_posWaypointList(WayPointList wListQ)
        {
            wList = wListQ;
            //wList.wayFinished = 0; // typically you want this to re-loop back to start
        }

        public override Vector2 getNextPos()
        {
            WayPoint wp = wList.currentWaypoint();
            Vector2 pos = new Vector2();
            pos.X = wp.pos.X;
            pos.Y = wp.pos.Y;
            bool rc = wList.nextLeg();
            return pos;
        }

        public override void init()
        {
            reset();
        }

        public override void reset()
        {
            wList.setLeg(0);
        }

        public override void draw(SpriteBatch sb, Color col)
        {
            wList.Draw(sb, col, col);
        }

    }
}