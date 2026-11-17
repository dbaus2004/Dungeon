using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Dungeon.ParticleSystem
{

    /// <summary>
    /// A class representing a generic particle system
    /// </summary>
    public abstract class ParticleSystem : DrawableGameComponent
    {
        #region Constants

        /// <summary>
        /// The draw order for particles using Alpha Blending
        /// </summary>
        /// <remarks>
        /// Particles drawn using additive blending should be drawn on top of 
        /// particles that use regular alpha blending
        /// </remarks>
        public const int AlphaBlendDrawOrder = 100;

        /// <summary>
        /// The draw order for particles using Additive Blending
        /// </summary>
        /// <remarks>
        /// Particles drawn using additive blending should be drawn on top of 
        /// particles that use regular alpha blending
        /// </remarks>
        public const int AdditiveBlendDrawOrder = 200;

        #endregion

        #region private fields

        /// <summary>
        /// The collection of particles 
        /// </summary>
        private readonly Particle[] particles;

        /// <summary>
        /// A Queue containing indices of unused particles in the Particles array
        /// </summary>
        private readonly Queue<int> freeParticles;

        /// <summary>
        /// The texture this particle system uses 
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// The origin when we're drawing textures 
        /// </summary>
        private Vector2 origin;

        private readonly SpriteBatch _spriteBatch;
        private readonly ContentManager _contentManager;

        #endregion

        #region protected fields

        /// <summary>The BlendState to use with this particle system</summary>
        protected BlendState blendState = BlendState.AlphaBlend;

        /// <summary>The filename of the texture to use for the particles</summary>
        protected string textureFilename;

        /// <summary>The minimum number of particles to add when AddParticles() is called</summary>
        protected int minNumParticles;

        /// <summary>The maximum number of particles to add when AddParticles() is called</summary>
        protected int maxNumParticles;

        #endregion

        #region public properties 

        /// <summary>
        /// The available particles in the system 
        /// </summary>
        public int FreeParticleCount => freeParticles.Count;

        #endregion

        /// <summary>
        /// Constructs a new instance of a particle system
        /// </summary>
        /// <param name="game"></param>
        public ParticleSystem(
            Game game,
            int maxParticles,
            SpriteBatch spriteBatch,
            ContentManager contentManager
        ) : base(game)
        {
            ArgumentNullException.ThrowIfNull(spriteBatch);
            ArgumentNullException.ThrowIfNull(contentManager);
            if (maxParticles <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxParticles));

            _spriteBatch = spriteBatch;
            _contentManager = contentManager;

            // Create our particles
            particles = new Particle[maxParticles];
            freeParticles = new Queue<int>(maxParticles);
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Initialize(Vector2.Zero);
                freeParticles.Enqueue(i);
            }
            // Run the InitializeConstants hook
            InitializeConstants();
        }

        #region virtual hook methods 

        /// <summary>
        /// Used to do the initial configuration of the particle engine.  The 
        /// protected constants `textureFilename`, `minNumParticles`, and `maxNumParticles`
        /// should be set in the override.
        /// </summary>
        protected abstract void InitializeConstants();

        /// <summary>
        /// InitializeParticle randomizes some properties for a particle, then
        /// calls initialize on it. It can be overridden by subclasses if they 
        /// want to modify the way particles are created.
        /// </summary>
        /// <param name="p">the particle to initialize</param>
        /// <param name="where">the position on the screen that the particle should be
        /// </param>
        protected virtual void InitializeParticle(ref Particle p, Vector2 where)
        {
            // Initialize the particle with default values
            p.Initialize(where);
        }

        /// <summary>
        /// Updates the individual particles.  Can be overridden in derived classes
        /// </summary>
        /// <param name="particle">The particle to update</param>
        /// <param name="dt">The elapsed time</param>
        protected virtual void UpdateParticle(ref Particle particle, float dt)
        {
            // Update particle's linear motion values
            particle.Velocity += particle.Acceleration * dt;
            particle.Position += particle.Velocity * dt;

            // Update the particle's angular motion values
            particle.AngularVelocity += particle.AngularAcceleration * dt;
            particle.Rotation += particle.AngularVelocity * dt;

            // Update the time the particle has been alive 
            particle.TimeSinceStart += dt;
        }

        #endregion

        #region override methods 

        /// <summary>
        /// Override the base class LoadContent to load the texture. once it's
        /// loaded, calculate the origin.
        /// </summary>
        /// <throws>A InvalidOperationException if the texture filename is not provided</throws>
        protected override void LoadContent()
        {
            EnsureContentLoaded();
            base.LoadContent();
        }

        public void EnsureContentLoaded()
        {
            if (texture != null)
                return;

            if (string.IsNullOrEmpty(textureFilename))
            {
                throw new InvalidOperationException(
                    "The particle system must set a texture filename before loading content."
                );
            }

            texture = _contentManager.Load<Texture2D>(textureFilename);
            origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
        }

        /// <summary>
        /// Overriden from DrawableGameComponent, Update will update all of the active
        /// particles.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // calculate dt, the change in the since the last frame. the particle
            // updates will use this value.
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // go through all of the particles...
            for (int i = 0; i < particles.Length; i++)
            {

                if (particles[i].Active)
                {
                    // ... and if they're active, update them.
                    UpdateParticle(ref particles[i], dt);
                    // if that update finishes them, put them onto the free particles
                    // queue.
                    if (!particles[i].Active)
                    {
                        freeParticles.Enqueue(i);
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Overriden from DrawableGameComponent, Draw will use the static 
        /// SpriteBatch to render all of the active particles.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            if (texture == null)
                throw new InvalidOperationException("Call EnsureContentLoaded before drawing the particle system.");

            _spriteBatch.Begin(blendState: blendState);

            foreach (Particle p in particles)
            {
                // skip inactive particles
                if (!p.Active)
                    continue;

                _spriteBatch.Draw(texture, p.Position, null, p.Color,
                    p.Rotation, origin, p.Scale, SpriteEffects.None, 0.0f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        #region AddParticles methods 

        /// <summary>
        /// AddParticles's job is to add an effect somewhere on the screen. If there 
        /// aren't enough particles in the freeParticles queue, it will use as many as 
        /// it can. This means that if there not enough particles available, calling
        /// AddParticles will have no effect.
        /// </summary>
        /// <param name="where">where the particle effect should be created</param>
        protected void AddParticles(Vector2 where)
        {
            // the number of particles we want for this effect is a random number
            // somewhere between the two constants specified by the subclasses.
            int numParticles =
                RandomHelper.Next(minNumParticles, maxNumParticles);

            // create that many particles, if you can.
            for (int i = 0; i < numParticles && freeParticles.Count > 0; i++)
            {
                // grab a particle from the freeParticles queue, and Initialize it.
                int index = freeParticles.Dequeue();
                InitializeParticle(ref particles[index], where);
            }
        }

        /// <summary>
        /// AddParticles's job is to add an effect somewhere on the screen. If there 
        /// aren't enough particles in the freeParticles queue, it will use as many as 
        /// it can. This means that if there not enough particles available, calling
        /// AddParticles will have no effect.
        /// </summary>
        /// <param name="where">where the particle effect should be created</param>
        protected void AddParticles(Rectangle where)
        {
            // the number of particles we want for this effect is a random number
            // somewhere between the two constants specified by the subclasses.
            int numParticles =
                RandomHelper.Next(minNumParticles, maxNumParticles);

            // create that many particles, if you can.
            for (int i = 0; i < numParticles && freeParticles.Count > 0; i++)
            {
                // grab a particle from the freeParticles queue, and Initialize it.
                int index = freeParticles.Dequeue();
                InitializeParticle(ref particles[index], RandomHelper.RandomPosition(where));
            }
        }

        #endregion

    }
}
