using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    public static class EffectExtension
    {
        /// <summary>
        /// Try to set the effect's current technique.
        /// </summary>
        /// <param name="effect">The effect.</param>
        /// <param name="techniqueName">The name of the technique.</param>
        public static void TrySetCurrentTechnique(this Effect effect, string techniqueName)
        {
            if (effect.CurrentTechnique.Name == techniqueName) return; // OPTIMIZATION: avoid lookup
            EffectTechnique technique = effect.Techniques[techniqueName];
            effect.TrySetCurrentTechnique(technique);
        }

        /// <summary>
        /// Try to set the effect's current technique.
        /// </summary>
        /// <param name="effect">The effect.</param>
        /// <param name="technique">The technique. May be null.</param>
        public static void TrySetCurrentTechnique(this Effect effect, EffectTechnique technique)
        {
            if (technique != null) effect.CurrentTechnique = technique;
        }
    }
}
