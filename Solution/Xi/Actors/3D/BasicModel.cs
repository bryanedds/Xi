using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A model that leverages XNA's BasicEffect.
    /// </summary>
    public class BasicModel : BaseModel<BasicModelSurface>
    {
        /// <summary>
        /// Create a BasicModel.
        /// </summary>
        /// <param name="game">The game.</param>
        public BasicModel(XiGame game) : base(game) { }

        /// <inheritdoc />
        protected override BasicModelSurface CreateSurface(int i, int j)
        {
            return new BasicModelSurface(Game, this, i, j);
        }
    }
}
