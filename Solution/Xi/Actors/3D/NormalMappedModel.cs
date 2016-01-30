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
    /// A normal mapped model.
    /// </summary>
    public class NormalMappedModel : BaseModel<NormalMappedSurface>
    {
        /// <summary>
        /// Create a NormalMappedModel.
        /// </summary>
        /// <param name="game">The game.</param>
        public NormalMappedModel(XiGame game) : base(game) { }

        /// <inheritdoc />
        protected override NormalMappedSurface CreateSurface(int i, int j)
        {
            return new NormalMappedSurface(Game, this, "Xi/3D/XiNormalMapped", i, j);
        }
    }
}
