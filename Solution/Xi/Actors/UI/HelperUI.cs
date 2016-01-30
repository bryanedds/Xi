using System;
using System.Collections.Generic;

namespace Xi
{
    /// <summary>
    /// Sorts actor UIs by vertical distance.
    /// </summary>
    public class ActorUIVerticalDistanceSorter : IComparer<ActorUI>
    {
        /// <summary>
        /// Sort the actor UIs.
        /// </summary>
        /// <param name="actorUIs">The actor UIs to sort.</param>
        /// <param name="origin">The origin to which to compare vertical distance to.</param>
        /// <returns>The sorted actor UIs.</returns>
        public List<ActorUI> Sort(List<ActorUI> actorUIs, float origin)
        {
            this.origin = origin;
            actorUIs.Sort(this);
            return actorUIs;
        }

        /// <inheritdoc />
        int IComparer<ActorUI>.Compare(ActorUI x, ActorUI y)
        {
            return
                (x.Position.Y - origin).
                CompareAbsolute
                (y.Position.Y - origin);
        }

        private float origin;
    }

    /// <summary>
    /// Sorts actor UIs by horizontal distance.
    /// </summary>
    public class ActorUIHorizontalDistanceSorter : IComparer<ActorUI>
    {
        /// <summary>
        /// Sort the actor UIs.
        /// </summary>
        /// <param name="actorUIs">The actor UIs to sort.</param>
        /// <param name="origin">The origin to which to compare vertical distance to.</param>
        /// <returns>The sorted actor UIs.</returns>
        public List<ActorUI> Sort(List<ActorUI> actorUIs, float origin)
        {
            this.origin = origin;
            actorUIs.Sort(this);
            return actorUIs;
        }

        /// <inheritdoc />
        int IComparer<ActorUI>.Compare(ActorUI x, ActorUI y)
        {
            return
                (x.Position.X - origin).
                CompareAbsolute
                (y.Position.X - origin);
        }

        private float origin;
    }

    /// <summary>
    /// Various methods to help with UI operations.
    /// </summary>
    public static class HelperUI
    {
        /// <summary>
        /// Get the actor UIs that accept focus.
        /// </summary>
        public static List<ActorUI> GetActorUIsThatAcceptFocus(this List<ActorUI> actorUIs, List<ActorUI> result)
        {
            foreach (ActorUI actorUI in actorUIs)
                if (actorUI.AcceptFocus)
                    result.Add(actorUI);
            return result;
        }

        /// <summary>
        /// Get the actor UIs that span in a given direction.
        /// </summary>
        public static List<ActorUI> GetActorUIsInDirection(this List<ActorUI> actorUIs, List<ActorUI> result, ActorUI origin, Direction2D direction)
        {
            switch (direction)
            {
                case Direction2D.Up: return GetActorUIsUpward(actorUIs, result, origin);
                case Direction2D.Down: return GetActorUIsDownward(actorUIs, result, origin);
                case Direction2D.Left: return GetActorUIsLeftward(actorUIs, result, origin);
                case Direction2D.Right: return GetActorUIsRightward(actorUIs, result, origin);
                default: throw new ArgumentException("Invalid direction '" + direction.ToString() + "'.");
            }
        }

        /// <summary>
        /// Get the actor UIs that span upward.
        /// </summary>
        public static List<ActorUI> GetActorUIsUpward(this List<ActorUI> actorUIs, List<ActorUI> result, ActorUI origin)
        {
            foreach (ActorUI actorUI in actorUIs)
                if (actorUI != origin &&
                    actorUI.Position.Y - origin.Position.Y >= 0)
                    result.Add(actorUI);
            return result;
        }

        /// <summary>
        /// Get the actor UIs that span downward.
        /// </summary>
        public static List<ActorUI> GetActorUIsDownward(this List<ActorUI> actorUIs, List<ActorUI> result, ActorUI origin)
        {
            foreach (ActorUI actorUI in actorUIs)
                if (actorUI != origin &&
                    origin.Position.Y - actorUI.Position.Y >= 0)
                    result.Add(actorUI);
            return result;
        }

        /// <summary>
        /// Get the actor UIs that span rightward.
        /// </summary>
        public static List<ActorUI> GetActorUIsRightward(this List<ActorUI> actorUIs, List<ActorUI> result, ActorUI origin)
        {
            foreach (ActorUI actorUI in actorUIs)
                if (actorUI != origin && 
                    actorUI.Position.X - origin.Position.X >= 0)
                    result.Add(actorUI);
            return result;
        }

        /// <summary>
        /// Get the actor UIs that span leftward.
        /// </summary>
        public static List<ActorUI> GetActorUIsLeftward(this List<ActorUI> actorUIs, List<ActorUI> result, ActorUI origin)
        {
            foreach (ActorUI actorUI in actorUIs)
                if (actorUI != origin && 
                    origin.Position.X - actorUI.Position.X >= 0)
                    result.Add(actorUI);
            return result;
        }

        /// <summary>
        /// Sort the actor UIs by a given direction.
        /// </summary>
        public static List<ActorUI> SortActorUIsInDirection(this List<ActorUI> actorUIs, ActorUI origin, Direction2D direction)
        {
            if (direction == Direction2D.Up ||
                direction == Direction2D.Down)
                actorUIs.SortVerticalDistance(origin);
            else if (
                direction == Direction2D.Left ||
                direction == Direction2D.Right)
                actorUIs.SortHorizontalDistance(origin);
            else throw new ArgumentException("Invalid direction '" + direction.ToString() + "'.");
            return actorUIs;
        }

        /// <summary>
        /// Sort the actor UIs by vertical distance.
        /// </summary>
        public static List<ActorUI> SortVerticalDistance(this List<ActorUI> actorUIs, ActorUI origin)
        {
            return verticalDistanceSorter.Sort(actorUIs, origin.Position.Y);
        }

        /// <summary>
        /// Sort the actor UIs by horizontal distance.
        /// </summary>
        public static List<ActorUI> SortHorizontalDistance(this List<ActorUI> actorUIs, ActorUI origin)
        {
            return horizontalDistanceSorter.Sort(actorUIs, origin.Position.X);
        }

        private static readonly ActorUIVerticalDistanceSorter verticalDistanceSorter = new ActorUIVerticalDistanceSorter();
        private static readonly ActorUIHorizontalDistanceSorter horizontalDistanceSorter = new ActorUIHorizontalDistanceSorter();
    }
}
