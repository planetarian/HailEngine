#region Using Statements

using System;
using System.Text;
using Artemis;
using Artemis.Manager;
using Artemis.System;
using Hail.GraupelSemantics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Hail.Core
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class HailGame : Game
    {
        private readonly GraphicsDeviceManager graphics;
        //private int minHeight = 240;
        //private int minWidth = 320;
        private SpriteBatch spriteBatch;
        private EntityWorld world;

        private readonly StringBuilder titleBuilder = new StringBuilder();

        public HailGame()
        {
            graphics = new GraphicsDeviceManager(this)
                           {
#if WINDOWS_PHONE || WINRT
                               SupportedOrientations =
                                   DisplayOrientation.LandscapeLeft |
                                   DisplayOrientation.LandscapeRight,
#elif XBOX
#else
                               IsFullScreen = false,
                               PreferredBackBufferWidth = 1280,
                               PreferredBackBufferHeight = 720,
                               SynchronizeWithVerticalRetrace = true,
                               PreferMultiSampling = true,
#endif
                           };

            IsFixedTimeStep = false;
            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";
        }
        

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            world = new EntityWorld();

            EntitySystem.BlackBoard.SetEntry("ContentManager", Content);
            EntitySystem.BlackBoard.SetEntry("GraphicsDeviceManager", graphics);
            EntitySystem.BlackBoard.SetEntry("GraphicsDevice", GraphicsDevice);
            EntitySystem.BlackBoard.SetEntry("SpriteBatch", spriteBatch);
            

            world.InitializeAll(true);
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            
            // Load Graupel file
            const string path = "Content\\Data\\";
            const string ext = ".txt";
            //const string file = path + "entitydef" + ext;
            //const string file = path + "entitytest" + ext;
            const string file = path + "entitytest" + ext;
            var loader = new GraupelLoader();
            loader.Load(file);
            loader.SpawnEntities(world, "simpletest");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
#if !WINDOWS_PHONE
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
#endif

            // TODO: Add your update logic here

            world.Update(Math.Min(60,gameTime.ElapsedGameTime.Milliseconds));

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            
            spriteBatch.Begin();
            world.Draw();
            spriteBatch.End();

            titleBuilder.Clear();
            titleBuilder.Append("Hail Engine (");
            titleBuilder.Append(world.Delta == 0 ? 0 : (int) (1000/world.Delta));
            titleBuilder.Append(" FPS)");

            var hovered = EntitySystem.BlackBoard.GetEntry<string>("HoveredEntity");
            var selected = EntitySystem.BlackBoard.GetEntry<string>("SelectedEntity");
            titleBuilder.Append(" Hover: ");
            titleBuilder.Append(hovered);
            titleBuilder.Append(" Selected: ");
            titleBuilder.Append(selected);
            
            Window.Title = titleBuilder.ToString();

            base.Draw(gameTime);
        }
    }
}