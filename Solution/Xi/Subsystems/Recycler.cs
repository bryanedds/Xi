using System;
using System.Collections.Generic;

namespace Xi
{
    /// <summary>
    /// Implements the recycling system.
    /// </summary>
    public class Recycler : Disposable
    {
        /// <summary>
        /// Create a Recycler.
        /// </summary>
        /// <param name="game">The game.</param>
        public Recycler(XiGame game)
        {
            this.game = game;
        }

        /// <summary>
        /// Allocate an object, creating one if none exist.
        /// </summary>
        /// <typeparam name="T">The minimum type of object to create.</typeparam>
        /// <param name="definition">The object definition. TODO: expand on object definition format.</param>
        public T Allocate<T>(string definition) where T : Recyclable
        {
            XiHelper.ArgumentNullCheck(definition);
            T instance;
            bool created = GetInstance<T>(definition, out instance);
            ValidateAllocation(instance, created);
            instance.Allocated = true;
            return instance;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) TearDownRecycleBins();
            base.Dispose(disposing);
        }

        internal void Deallocate(Recyclable recyclable)
        {
            XiHelper.ArgumentNullCheck(recyclable);
            ValidateDeallocation(recyclable);
            if (recyclable.RecyclingEnabled) GetRecycleBin(recyclable.RecycleBinName).Enqueue(recyclable);
            recyclable.Allocated = false;
        }

        private void TearDownRecycleBins()
        {
            foreach (Queue<Recyclable> recycleBin in recycleBins.Values) TearDownRecycleBin(recycleBin);
            recycleBins.Clear();
        }

        private void TearDownRecycleBin(Queue<Recyclable> recycleBin)
        {
            foreach (Recyclable recyclable in recycleBin) recyclable.Dispose();
            recycleBin.Clear();
        }

        private bool GetInstance<T>(string definition, out T instance) where T : Recyclable
        {
            return
                ReflectionHelper.IsFileDefinition(definition) ?
                GetInstanceFromFileDefinition(definition, out instance) :
                GetInstanceFromStringDefinition(definition, out instance);
        }

        private bool GetInstanceFromFileDefinition<T>(string fileDefinition, out T instance) where T : Recyclable
        {
            string recyclingBinName = TryExtractRecycleBinNameFromFileDefinition(fileDefinition);
            Queue<Recyclable> recycleBin;
            recycleBins.TryGetValue(recyclingBinName, out recycleBin);
            bool shouldCreateInstance = ShouldCreateInstance(recyclingBinName, recycleBin);
            instance =
                shouldCreateInstance ?
                game.CreateSerializableFromFileDefinition<T>(fileDefinition) :
                XiHelper.Cast<T>(recycleBin.Dequeue());
            instance.RecycleBinName = recyclingBinName;
            return shouldCreateInstance;
        }

        private bool GetInstanceFromStringDefinition<T>(string stringDefinition, out T instance) where T : Recyclable
        {
            string recycleBinName = TryExtractRecycleBinNameFromStringDefinition(stringDefinition);
            Queue<Recyclable> recycleBin;
            recycleBins.TryGetValue(recycleBinName, out recycleBin);
            bool shouldCreateInstance = ShouldCreateInstance(recycleBinName, recycleBin);
            instance =
                shouldCreateInstance ?
                game.CreateSerializableFromStringDefinition<T>(stringDefinition) :
                XiHelper.Cast<T>(recycleBin.Dequeue());
            instance.RecycleBinName = recycleBinName;
            return shouldCreateInstance;
        }

        private Queue<Recyclable> GetRecycleBin(string definition)
        {
            Queue<Recyclable> recycleBin;
            if (!recycleBins.TryGetValue(definition, out recycleBin))
                recycleBins.Add(definition, recycleBin = new Queue<Recyclable>());
            return recycleBin;
        }

        private static bool ShouldCreateInstance(string recycleBinName, Queue<Recyclable> recycleBin)
        {
            return
                recycleBin == null ||
                recycleBin.Count == 0 ||
                recycleBinName.Length == 0;
        }

        private static string TryExtractRecycleBinNameFromFileDefinition(string stringDefinition)
        {
            // TODO: implement
            return string.Empty;
        }

        private static string TryExtractRecycleBinNameFromStringDefinition(string stringDefinition)
        {
            int recycleBinNameOffset = stringDefinition.IndexOf(recycleBinNameKey);
            return
                recycleBinNameOffset != -1 ?
                ExtractRecycleBinNameFromStringDefinition(stringDefinition, recycleBinNameOffset) :
                string.Empty;
        }

        private static string ExtractRecycleBinNameFromStringDefinition(string stringDefinition, int recyclingGroupOffset)
        {
            // OPTIMIZATION: memoized to avoid generating garbage
            string recycleBinName;
            if (recycleBinNames.TryGetValue(stringDefinition, out recycleBinName)) return recycleBinName;
            int recycleBinNameOffsetBegin = recyclingGroupOffset + recycleBinNameKey.Length;
            // TODO: make sure '>' won't screw up the parser when used in an XML file
            int recycleBinNameOffsetEnd = stringDefinition.IndexOf('>', recyclingGroupOffset);
            recycleBinName =
                recycleBinNameOffsetEnd != -1 ?
                stringDefinition.Substring(recycleBinNameOffsetBegin, recycleBinNameOffsetEnd) :
                stringDefinition.Substring(recycleBinNameOffsetBegin);
            recycleBinNames.Add(stringDefinition, recycleBinName);
            return recycleBinName;
        }

        private static void ValidateAllocation(Recyclable recyclable, bool created)
        {
            if (!recyclable.Allocated) return;
            if (created) recyclable.Dispose(); // clean up before throwing
            throw new ArgumentException("Cannot redundantly allocate an object.");
        }

        private static void ValidateDeallocation(Recyclable recyclable)
        {
            if (!recyclable.Allocated)
                throw new ArgumentException("Cannot redundantly deallocate an object.");
        }

        private const string recycleBinNameKey = "RecycleBinName=";
        private static readonly Dictionary<string, string> recycleBinNames = new Dictionary<string, string>();
        private readonly XiGame game;
        private readonly Dictionary<string, Queue<Recyclable>> recycleBins = new Dictionary<string, Queue<Recyclable>>();
    }
}
