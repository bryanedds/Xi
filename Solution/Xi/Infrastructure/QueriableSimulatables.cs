using System;
using System.Collections;
using System.Collections.Generic;

namespace Xi
{
    public class QueriableSimulatables<T> : IEnumerable<T> where T : Simulatable
    {
        public int Count { get { return simulatablePSet.Count; } }

        public T First { get { return simulatablePSet.First(); } }

        public T FirstOrDefault { get { return simulatablePSet.FirstOrDefault(); } }

        public Dictionary<T, T>.ValueCollection.Enumerator GetEnumerator()
        {
            return simulatablePSet.Values.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T simulatable)
        {
            XiHelper.ArgumentNullCheck(simulatable);
            if (simulatable.HasMeaningfulName) simulatableDictionary.Add(simulatable.Name, simulatable);
            simulatablePSet.TryAddValue(simulatable);
        }

        public bool Remove(T simulatable)
        {
            XiHelper.ArgumentNullCheck(simulatable);
            if (simulatable.HasMeaningfulName) simulatableDictionary.Remove(simulatable.Name);
            return simulatablePSet.Remove(simulatable);
        }

        public bool Contains(T simulatable)
        {
            XiHelper.ArgumentNullCheck(simulatable);
            return simulatablePSet.ContainsValue(simulatable);
        }

        public bool Contains<U>(string name) where U : class
        {
            XiHelper.ArgumentNullCheck(name);
            return simulatableDictionary[name] as U != null;
        }

        public bool Contains<U>() where U : class
        {
            return Contains<U>(typeof(U).FullName);
        }

        public void RefreshDictionary(T simulatable, string oldName)
        {
            XiHelper.ArgumentNullCheck(simulatable, oldName);
            bool removed = simulatableDictionary.Remove(oldName);
            if (oldName.Length == 0 && simulatable.HasMeaningfulName || removed)
                simulatableDictionary.TryAddValue(simulatable.Name, simulatable);
        }

        public U Get<U>() where U : class
        {
            return Get<U>(typeof(U).FullName);
        }

        public U Get<U>(string name) where U : class
        {
            XiHelper.ArgumentNullCheck(name);
            if (name.Length == 0) return null;
            T simulatable;
            simulatableDictionary.TryGetValue(name, out simulatable);
            return simulatable as U;
        }

        public U Grab<U>() where U : class
        {
            return Grab<U>(typeof(U).FullName);
        }

        public U Grab<U>(string name) where U : class
        {
            XiHelper.ArgumentNullCheck(name);
            return XiHelper.Cast<U>(simulatableDictionary[name]);
        }

        public List<U> Collect<U>(List<U> result) where U : class
        {
            XiHelper.ArgumentNullCheck(result);
            foreach (T simulatable in simulatablePSet.Values)
            {
                U simulatableU = simulatable as U;
                if (simulatableU != null) result.Add(simulatableU);
            }
            return result;
        }

        public List<U> Collect<U>(Func<U, bool> predicate, List<U> result) where U : class
        {
            XiHelper.ArgumentNullCheck(predicate, result);
            foreach (T simulatable in simulatablePSet.Values)
            {
                U simulatableU = simulatable as U;
                if (simulatableU != null && predicate(simulatableU)) result.Add(simulatableU);
            }
            return result;
        }

        private readonly Dictionary<string, T> simulatableDictionary = new Dictionary<string, T>();
        private readonly Dictionary<T, T> simulatablePSet = new Dictionary<T, T>();
    }
}
