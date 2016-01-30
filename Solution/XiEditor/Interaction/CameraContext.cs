using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xi;
using XnaKeys = Microsoft.Xna.Framework.Input.Keys;

namespace XiEditor
{
    public class CameraContext : ControllerContext
    {
        public CameraContext(XiGame game, EditorController controller) : base(game, controller)
        {
            cameraDragger = new CameraDragger(game, controller);
        }

        protected override void AdvanceHook(GameTime gameTime)
        {
            base.AdvanceHook(gameTime);
            UpdateCameraDragger(gameTime);
            UpdateCameraMovement();
        }

        protected override bool OnMouseButtonDown(MouseButtons button, Vector2 mousePosition)
        {
            if (IgnoreMouseButton(button)) return false;
            cameraDragger.HandleButtonDown(mousePosition);
            return true;
        }

        protected override bool OnMouseButtonUp(MouseButtons button, Vector2 mousePosition)
        {
            if (IgnoreMouseButton(button)) return false;
            cameraDragger.HandleButtonUp(mousePosition);
            return true;
        }

        protected override bool OnMouseButtonDrag(MouseButtons button, Vector2 mousePosition)
        {
            if (IgnoreMouseButton(button)) return false;
            cameraDragger.HandleButtonDrag(mousePosition);
            return true;
        }

        private void UpdateCameraDragger(GameTime gameTime)
        {
            cameraDragger.Update(gameTime);
        }

        private void UpdateCameraMovement()
        {
            if (!Controller.AllowKeyboard) return;
            KeyboardState keyboard = Game.KeyboardState;
            if (keyboard.GetModifier() != KeyboardModifier.None) return;
            if (keyboard.IsKeyDown(XnaKeys.LeftAlt) || keyboard.IsKeyDown(XnaKeys.RightAlt)) return;
            if (keyboard.IsKeyDown(XnaKeys.W)) Controller.CameraPosition += Controller.CameraForward * moveSpeed;
            if (keyboard.IsKeyDown(XnaKeys.S)) Controller.CameraPosition -= Controller.CameraForward * moveSpeed;
            if (keyboard.IsKeyDown(XnaKeys.A)) Controller.CameraAngleY += turnSpeed;
            if (keyboard.IsKeyDown(XnaKeys.D)) Controller.CameraAngleY -= turnSpeed;
            if (keyboard.IsKeyDown(XnaKeys.Left) || keyboard.IsKeyDown(XnaKeys.Q)) Controller.CameraPosition -= Controller.CameraRight * moveSpeed;
            if (keyboard.IsKeyDown(XnaKeys.Right) || keyboard.IsKeyDown(XnaKeys.E)) Controller.CameraPosition += Controller.CameraRight * moveSpeed;
            if (keyboard.IsKeyDown(XnaKeys.Up) || keyboard.IsKeyDown(XnaKeys.R)) Controller.CameraPosition += Controller.CameraUp * moveSpeed;
            if (keyboard.IsKeyDown(XnaKeys.Down) || keyboard.IsKeyDown(XnaKeys.F)) Controller.CameraPosition -= Controller.CameraUp * moveSpeed;
        }

        private static bool IgnoreMouseButton(MouseButtons button)
        {
            return button != MouseButtons.Middle;
        }

        private const float moveSpeed = 5;
        private const float turnSpeed = 0.125f;
        private readonly CameraDragger cameraDragger;
    }
}
