using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RC_Framework;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Project1
{
    public class Game1 : Game
    {
        // define vars
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        // ints
        int ticks = 0;
        int ticker = 0;
        int gameHeight = 720;
        int gameWidth = 1280;
        int voyagerXSpeed = 4;
        int voyagerYSpeed = 4;
        int gameState = 0; // 0 for standby, 1 for ingame, 2 for lost, 3 for win, 4 for working behind the scenes
        int gameScore = 0;
        // strings
        string HUDText => "SCORE: " + gameScore;
        // floats
        // bools
        bool showBB = false;
        bool showDB = false;
        bool photonAway = false;
        // textures
        Texture2D texBack = null;
        Texture2D texVoyager = null;
        Texture2D texPhotonTorpedo = null;
        Texture2D texBirdOfPrey = null;
        Texture2D texBoom = null;
        Texture2D texMidas = null;
        // sprites
        Sprite3 sVoyager = null;
        Sprite3 sPhoton = null;
        Sprite3 sBirdOfPrey = null;
        Sprite3 sMidasArray = null;
        SpriteList slBooms = null;
        // text
        SpriteFont fonty;
        SpriteFont fontHud;
        SpriteFont fontHudBig;
        TextRenderableFlash tStandby = null;
        TextRenderable tHUD = null;
        TextRenderable tGameOver = null;
        TextRenderable tWinnerWinner = null;
        // vectors
        Vector2 voyagerPos = new Vector2(0, 0);
        Vector2 photonPos = new Vector2(0, 0);
        // rectangles
        Rectangle ptBB; //photon 
        Rectangle voyBB; //voyager
        Rectangle enBB; //enemy
        Rectangle tnBB; //terrain
        // misc
        List<SoundEffect> soundEffects;
        KeyboardState k;
        KeyboardState prevk;
        HitState hsPTEnemy;
        HitState hsVoyEnemy;
        Random rnd = new Random();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = gameHeight;
            _graphics.PreferredBackBufferWidth = gameWidth;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            voyagerPos.X = 10;
            voyagerPos.Y = gameHeight / 2 ;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            LineBatch.init(GraphicsDevice);
            // load misc
            fonty = Content.Load<SpriteFont>("File");
            fontHud = Content.Load<SpriteFont>("HUD");
            fontHudBig = Content.Load<SpriteFont>("HUDBig");
            soundEffects = new List<SoundEffect>
            {
                Content.Load<SoundEffect>("Torpedo_fire"),
                Content.Load<SoundEffect>("redAlert"),
                Content.Load<SoundEffect>("battlestations"),
                Content.Load<SoundEffect>("explosion"),
                Content.Load<SoundEffect>("CCVerified")
            };
            SoundEffect.MasterVolume = 0.4f;

            //load anims
            slBooms = new SpriteList();

            // load textures
            texBack = Util.texFromFile(GraphicsDevice, @"\school\year 3 sem 1\GPT\MT1\Project1\assets\ESDBG.jpg");
            texVoyager = Util.texFromFile(GraphicsDevice, @"\school\year 3 sem 1\GPT\MT1\Project1\assets\voyager.png");
            texPhotonTorpedo = Util.texFromFile(GraphicsDevice, @"\school\year 3 sem 1\GPT\MT1\Project1\assets\photon.png");
            texBirdOfPrey = Util.texFromFile(GraphicsDevice, @"\school\year 3 sem 1\GPT\MT1\Project1\assets\BOP.png");
            texBoom = Util.texFromFile(GraphicsDevice, @"\school\year 3 sem 1\GPT\MT1\Project1\assets\Boom3.png");
            texMidas = Util.texFromFile(GraphicsDevice, @"\school\year 3 sem 1\GPT\MT1\Project1\assets\midas.png");

            // load sprites
            sVoyager = new Sprite3(true, texVoyager, voyagerPos.X, voyagerPos.Y);
            sVoyager.setBBToTexture();

            sPhoton = new Sprite3(false, texPhotonTorpedo, sVoyager.getPosX() + texVoyager.Width / 2, sVoyager.getPosY() + sVoyager.getHeight() / 2);
            sPhoton.setBBToTexture();

            sBirdOfPrey = new Sprite3(true, texBirdOfPrey, gameWidth + 100, rnd.Next(1, (int)(gameHeight - texBirdOfPrey.Height)));
            sBirdOfPrey.setBBToTexture();

            sMidasArray = new Sprite3(true, texMidas, gameWidth + 250, rnd.Next(1, (int)(gameHeight - texMidas.Height)));
            sMidasArray.setBBToTexture();

            // load text
            tStandby = new TextRenderableFlash("STANDBY", new Vector2(10, 10), fontHud ,Color.SkyBlue, (int) 4);
            tGameOver = new TextRenderable("MISSION FAILED", new Vector2((gameWidth / 2) - 250, 0), fontHudBig, Color.IndianRed);
            tWinnerWinner = new TextRenderable("MISSION SUCCESS", new Vector2((gameWidth / 2) - 250, 0), fontHudBig, Color.LawnGreen);


            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevk = k;
            k = Keyboard.GetState();

            // updating shit
            ticks++;
            photonPos.X = sVoyager.getPosX() + texVoyager.Width / 2 + 5;
            photonPos.Y = sVoyager.getPosY() + sVoyager.getHeight() / 2 + 5;
            ptBB = sPhoton.getBoundingBoxAA();
            voyBB = sVoyager.getBoundingBoxAA();
            enBB = sBirdOfPrey.getBoundingBoxAA();

            // handle misc keybindings
            if (k.IsKeyDown(Keys.B) && prevk.IsKeyUp(Keys.B)) showBB = !showBB;

            if (k.IsKeyDown(Keys.D) && prevk.IsKeyUp(Keys.D)) showDB = !showDB;

            if (k.IsKeyDown(Keys.R) && prevk.IsKeyUp(Keys.R))
            {
                soundEffects[4].Play();
                resetGame();
            }

            if (k.IsKeyDown(Keys.C) && prevk.IsKeyUp(Keys.C))
            {
                soundEffects[4].Play();
                chickenDinner();
            }

            // handle in game logic
            if (gameState == 1) 
            {

                // handling game sprite object logic
                if (!photonAway)
                {
                    sPhoton.visible = false;
                    sPhoton.setPos(photonPos);
                }
                if (photonAway)
                {
                    if (sPhoton.getPosX() > gameWidth || !sPhoton.visible)
                    {
                        photonAway = false;
                    }
                    sPhoton.visible = true;
                    sPhoton.setPosX(sPhoton.getPosX() + voyagerXSpeed + 5);
                }
                if (!sVoyager.visible)
                {
                    gameOver();
                }
                if (!sBirdOfPrey.visible || sBirdOfPrey.getPosX() < (0 - sBirdOfPrey.getWidth()))
                {
                    if (sBirdOfPrey.getPosX() < (0 - sBirdOfPrey.getWidth())) 
                    {
                        gameScore--;
                        soundEffects[1].Play();
                    }

                    if (!sBirdOfPrey.visible && sBirdOfPrey.getPosX() > (0 - sBirdOfPrey.getWidth()))
                    {
                        gameScore++;
                    }
                    sBirdOfPrey.setPos(new Vector2(gameWidth + rnd.Next(40, 200), rnd.Next(1, (int)(gameHeight - sBirdOfPrey.getHeight()))));

                    if (sBirdOfPrey.getPosY() + sBirdOfPrey.getHeight() >= sMidasArray.getPosY() && sBirdOfPrey.getPosY() <= sMidasArray.getPosY() + sMidasArray.getHeight())
                    {
                        sBirdOfPrey.setPos(new Vector2(gameWidth + rnd.Next(40, 200), rnd.Next(1, (int)(gameHeight - sBirdOfPrey.getHeight()))));
                    }

                    sBirdOfPrey.visible = true;
                }
                if (sBirdOfPrey.visible)
                {
                    sBirdOfPrey.setPosX(sBirdOfPrey.getPosX() - 3);
                }
                if (sMidasArray.visible)
                {
                    sMidasArray.setPosX(sMidasArray.getPosX() - 1);
                }
                if (!sMidasArray.visible || sMidasArray.getPosX() < (0 - sMidasArray.getWidth()))
                {
                    if (!sMidasArray.visible && sMidasArray.getPosX() > (0))
                    {
                        soundEffects[1].Play();
                        gameScore--;
                    }
                    sMidasArray.setPos(new Vector2(gameWidth + rnd.Next(150, 400), rnd.Next(1, (int)(gameHeight - sMidasArray.getHeight()))));
                    sMidasArray.visible = true;
                }
                if (gameScore >= 10) gameState = 3;
                if (gameScore < 0) gameState = 2;

                // handle hit checking

                hsPTEnemy = checkHit(ptBB, enBB);
                hsVoyEnemy = checkHit(voyBB, enBB);
                handleHitsSpritesMAD(sVoyager, sBirdOfPrey);
                handleHitsSpritesMAD(sVoyager, sMidasArray);
                handleHitsSpritesMAD(sPhoton, sBirdOfPrey);
                handleHitsSpritesMAD(sPhoton, sMidasArray);

                // handle inputs 
                    // handle movement
                        // left and right
                if (k.IsKeyDown(Keys.Left) && sVoyager.getPosX() > 0)
                {
                    sVoyager.setPosX(sVoyager.getPosX() - voyagerXSpeed);
                }
                if (k.IsKeyDown(Keys.Right) && sVoyager.getPosX() < gameWidth - sVoyager.getWidth())
                {
                    sVoyager.setPosX(sVoyager.getPosX() + voyagerXSpeed);
                }

                        // move up and down
                if (k.IsKeyDown(Keys.Up) && sVoyager.getPosY() > 0)
                {
                    sVoyager.setPosY(sVoyager.getPosY() - voyagerYSpeed);
                }
                if (k.IsKeyDown(Keys.Down) && sVoyager.getPosY() < gameHeight - texVoyager.Height)
                {
                    sVoyager.setPosY(sVoyager.getPosY() + voyagerYSpeed);
                }

                // fire photon torpedo
                if (ticks > ticker + 30 && k.IsKeyDown(Keys.Space) && !photonAway)
                {
                    soundEffects[0].Play();
                    photonAway = true;
                }

            }
            if (gameState == 2 || gameState == 3)
            {
                if (k.IsKeyDown(Keys.Space) && prevk.IsKeyUp(Keys.Space))
                {
                    resetGame();
                }
            }


            if (gameState == 0 && k.IsKeyDown(Keys.Space))
            {
                soundEffects[2].Play();
                gameState = 1;
                ticker = ticks;
            }
            slBooms.animationTick(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // draw textures
            _spriteBatch.Draw(texBack,GraphicsDevice.Viewport.Bounds, Color.White);

            // draw sprites
            sVoyager.Draw(_spriteBatch);
            sPhoton.Draw(_spriteBatch);
            sBirdOfPrey.Draw(_spriteBatch);
            sMidasArray.Draw(_spriteBatch);

            // draw anims
            slBooms.Draw(_spriteBatch);

            // draw HUD
            if (gameState == 0)
            {
                tStandby.Draw(_spriteBatch);
            }

            if (gameState != 0)
            {
                tHUD = new TextRenderable(HUDText, new Vector2(10, 10), fontHud, Color.Orange);
                tHUD.Draw(_spriteBatch);
            }

            if (gameState == 2)
            {
                tGameOver.Draw(_spriteBatch);
            }
            if (gameState == 3)
            {
                tWinnerWinner.Draw(_spriteBatch);
            }

            // debugging
            if (showDB)
            {
                _spriteBatch.DrawString(fonty, "X speed: " + voyagerXSpeed, new Vector2(gameWidth - 150, 20), Color.HotPink);
                _spriteBatch.DrawString(fonty, "Y speed: " + voyagerYSpeed, new Vector2(gameWidth - 150, 40), Color.HotPink);
                _spriteBatch.DrawString(fonty, "Photon away: " + photonAway, new Vector2(gameWidth - 150, 60), Color.HotPink);
                _spriteBatch.DrawString(fonty, "Photon speed: " + (voyagerXSpeed + 5), new Vector2(gameWidth - 150, 80), Color.HotPink);
                _spriteBatch.DrawString(fonty, "Gamestate: " + gameState, new Vector2(gameWidth - 150, 100), Color.HotPink);
                _spriteBatch.DrawString(fonty, "Score: " + gameScore, new Vector2(gameWidth - 150, 120), Color.HotPink);
                if (hsPTEnemy.hit || hsVoyEnemy.hit)
                {
                    int tempTimer = ticks;
                    if (tempTimer + 3000 > ticks)
                    {
                        if (hsPTEnemy.hitTop) _spriteBatch.DrawString(fonty, "HitTop", new Vector2(gameWidth - 250, 20), Color.LawnGreen);
                        if (hsPTEnemy.hitBottom) _spriteBatch.DrawString(fonty, "HitBottom", new Vector2(gameWidth - 250, 40), Color.LawnGreen);
                        if (hsPTEnemy.hitLeft) _spriteBatch.DrawString(fonty, "HitLeft", new Vector2(gameWidth - 250, 60), Color.LawnGreen);
                        if (hsPTEnemy.hitRight) _spriteBatch.DrawString(fonty, "HitRight", new Vector2(gameWidth - 250, 80), Color.LawnGreen);
                        if (hsVoyEnemy.hitTop) _spriteBatch.DrawString(fonty, "HitTop", new Vector2(gameWidth - 350, 20), Color.BlueViolet);
                        if (hsVoyEnemy.hitBottom) _spriteBatch.DrawString(fonty, "HitBottom", new Vector2(gameWidth - 350, 40), Color.BlueViolet);
                        if (hsVoyEnemy.hitLeft) _spriteBatch.DrawString(fonty, "HitLeft", new Vector2(gameWidth - 350, 60), Color.BlueViolet);
                        if (hsVoyEnemy.hitRight) _spriteBatch.DrawString(fonty, "HitRight", new Vector2(gameWidth - 350, 80), Color.BlueViolet);
                    }
                }
            }

            if (showBB)
            {
                sVoyager.drawBB(_spriteBatch, Color.DarkBlue);
                sVoyager.drawHS(_spriteBatch, Color.SkyBlue);
                sPhoton.drawBB(_spriteBatch, Color.DarkGreen);
                sPhoton.drawHS(_spriteBatch, Color.ForestGreen);
                sBirdOfPrey.drawBB(_spriteBatch, Color.Red);
                sBirdOfPrey.drawHS(_spriteBatch, Color.DarkRed);
                sMidasArray.drawBB(_spriteBatch, Color.Orange);
                sMidasArray.drawHS(_spriteBatch, Color.DarkOrange);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        // custom methods below

        public void resetGame()
        {
            gameState = 4;
            sVoyager.setPos(voyagerPos);
            sVoyager.visible = true;
            sMidasArray.visible = false;
            sBirdOfPrey.visible = false;
            gameScore = 0;
            gameState = 0;
        }

        public void chickenDinner()
        {
            gameState = 3;
        }
        void gameOver() {
            gameState = 2;
            createExplosion((int)sVoyager.getPosX(), (int)sVoyager.getPosY());       
        }

        void handleHitsSpritesMAD(Sprite3 s1, Sprite3 s2)
        {
            Rectangle BB1, BB2;
            HitState tempHS;
            BB1 = s1.getBoundingBoxAA();
            BB2 = s2.getBoundingBoxAA();

            tempHS = checkHit(BB1, BB2);
            if (tempHS.hit)
            {
                s1.visible = false;
                s2.visible = false;
                // this stays here because i made this monstrosity and im goddamn proud of it. It'll either disgust or impress someone looking at my GH and i think that'd be funny. 
                //createExplosion((int)(((s1.getPosX() + (s1.getWidth() / 2)) + (s2.getPosX() + (s2.getWidth() / 2))) / 2), (int)(((s1.getPosY() + (s1.getHeight() / 2)) + (s2.getPosY() + (s2.getHeight() / 2))) / 2));
                createExplosion((int)s2.getPosX(), (int)s2.getPosY());
            }

        }
        // thanks Rob!
        void createExplosion(int x, int y) 
        {

            Sprite3 s = new Sprite3(true, texBoom, x, y);
            float scale = 2f;
            s.setXframes(7);
            s.setYframes(3);
            s.setWidthHeight(896 / 7 * scale, 384 / 3 * scale);

            Vector2[] anim = new Vector2[21];
            anim[0].X = 0; anim[0].Y = 0;
            anim[1].X = 1; anim[1].Y = 0;
            anim[2].X = 2; anim[2].Y = 0;
            anim[3].X = 3; anim[3].Y = 0;
            anim[4].X = 4; anim[4].Y = 0;
            anim[5].X = 5; anim[5].Y = 0;
            anim[6].X = 6; anim[6].Y = 0;
            anim[7].X = 0; anim[7].Y = 1;
            anim[8].X = 1; anim[8].Y = 1;
            anim[9].X = 2; anim[9].Y = 1;
            anim[10].X = 3; anim[10].Y = 1;
            anim[11].X = 4; anim[11].Y = 1;
            anim[12].X = 5; anim[12].Y = 1;
            anim[13].X = 6; anim[13].Y = 1;
            anim[14].X = 0; anim[14].Y = 2;
            anim[15].X = 1; anim[15].Y = 2;
            anim[16].X = 2; anim[16].Y = 2;
            anim[17].X = 3; anim[17].Y = 2;
            anim[18].X = 4; anim[18].Y = 2;
            anim[19].X = 5; anim[19].Y = 2;
            anim[20].X = 6; anim[20].Y = 2;
            s.setAnimationSequence(anim, 0, 20, 4);
            s.setAnimFinished(2); // make it inactive and invisible
            s.animationStart();
            soundEffects[3].Play();
            slBooms.addSpriteReuse(s); // add the sprite

        }

        //check hitstate - from Rob
        public HitState checkHit(Rectangle rect1, Rectangle rect2) 
        {
            HitState retv;
            retv.hit = false;
            retv.hitTop = false;
            retv.hitBottom = false;
            retv.hitLeft = false;
            retv.hitRight = false;
            retv.hitInside = false;
            retv.hitExact = false;

            Rectangle temp1 = Rectangle.Intersect(rect1, rect2);

            if (temp1.Width == 0 && temp1.Height == 0)
            {
                return retv;
            }

            retv.hit = true;
            if (temp1.Y == rect1.Y) retv.hitTop = true;
            if (temp1.Y + temp1.Height == rect1.Y + rect1.Height) retv.hitBottom = true;
            if (temp1.X == rect1.X) retv.hitLeft = true;
            if (temp1.X + temp1.Width == rect1.X + rect1.Width) retv.hitRight = true;

            return retv;
        }

    }
    // also from Rob
    public struct HitState 
    {
        public bool hit; // true if hit
        public bool hitTop; // true if hit top
        public bool hitBottom; // true if hit bottom 
        public bool hitLeft; // true if hit left
        public bool hitRight; // true if hit right
        public bool hitInside; // true if one inside other
        public bool hitExact; // only true if both bounding boxes exact size
    }

}

// voyager by Alexander Klemm https://www.artstation.com/artwork/9e9moy
// torpedo https://www.deviantart.com/bagera3005/art/Photon-Torpedo-mark-VI-348752012
// torpedo sfx http://soundfxcenter.com/download-sound/star-trek-enterprise-photon-torpedo-sound-effect/
// bird of prey https://www.ex-astris-scientia.org/schematics/discovery_klingon.htm
// explosion https://www.youtube.com/shorts/5_2AteP2r6w
// red alert https://www.mediacollege.com/downloads/sound-effects/star-trek/tos/
// battle stations https://www.mediacollege.com/downloads/sound-effects/star-trek/voy/
// command codes https://www.trekcore.com/audio/