using System;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// Spawns 3D objects over time.
    /// </summary>
    public class TimedSpawner : Facet<Actor3D>
    {
        /// <summary>
        /// Create a TimedSpawner3D.
        /// </summary>
        /// <param name="game">The game.</param>
        public TimedSpawner(XiGame game) : base(game, false) { }

        /// <summary>
        /// The offset from the parent actor where spawning occurs.
        /// </summary>
        public Vector3 SpawnOffset
        {
            get { return spawnOffset; }
            set { spawnOffset = value; }
        }

        /// <summary>
        /// The time delay between each spawn.
        /// </summary>
        public double SpawnDelay
        {
            get { return spawnDelay; }
            set { spawnDelay = value; }
        }

        /// <summary>
        /// The spawn timer.
        /// </summary>
        public double SpawnTimer
        {
            get { return spawnTimer; }
            set { spawnTimer = value; }
        }

        /// <summary>
        /// The spawned object definition.
        /// TODO: expand on object definition format.
        /// </summary>
        public string SpawnDefinition
        {
            get { return spawnDefinition; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                spawnDefinition = value;
            }
        }

        /// <inheritdoc />
        protected override void PlayHook(GameTime gameTime)
        {
            base.PlayHook(gameTime);
            AdvanceSpawnTime(gameTime);
            if (ShouldSpawn) TrySpawn();
        }

        private bool ShouldSpawn
        {
            get { return spawnTimer >= spawnDelay; }
        }

        private bool CanSpawn
        {
            get { return Actor.ActorGroup != null && spawnDefinition.Length != 0; }
        }

        private void AdvanceSpawnTime(GameTime gameTime)
        {
            spawnTimer += gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void TrySpawn()
        {
            if (CanSpawn) Spawn();
            DecrementSpawnTime();
        }

        private void Spawn()
        {
            try
            {
                Actor3D spawn = Actor.ActorGroup.CreateActorFromDefinition<Actor3D>(spawnDefinition);
                spawn.OrientationQuaternion = Actor.OrientationQuaternion;
                spawn.Position = Actor.Position + Vector3.Transform(spawnOffset, spawn.OrientationQuaternion);
            }
            catch (ArgumentException)
            {
                // swallow argument exception
            }
        }

        private void DecrementSpawnTime()
        {
            spawnTimer -= spawnDelay;
        }

        private double spawnDelay = 1.0;
        private double spawnTimer;
        private string spawnDefinition = string.Empty;
        private Vector3 spawnOffset;
    }
}
