using System;
using Microsoft.Xna.Framework;
using Xi;

namespace XiEditor
{
    public abstract class Dragger
    {
        public Dragger(XiGame game, double dragBeginDelay)
        {
            XiHelper.ArgumentNullCheck(game);
            if (dragBeginDelay < 0) throw new ArgumentException("Drag begin delay should be positive.");
            this.game = game;
            this.dragBeginDelay = dragBeginDelay;
        }

        public void Update(GameTime gameTime)
        {
            XiHelper.ArgumentNullCheck(gameTime);
            UpdateButtonDownTime(gameTime);
            UpdateModifier();
        }

        public void HandleButtonDown(Vector2 dragPosition)
        {
            modifier = game.KeyboardState.GetModifier();
            PrepareDrag(dragPosition);
            ButtonDown = true;
        }

        public void HandleButtonDrag(Vector2 dragPosition)
        {
            if (ReadyToDrag && !dragging) BeginDrag(dragPosition);
            if (modifierChanged) ModifyDrag(dragPosition);
            if (dragging) UpdateDrag(dragPosition);
        }

        public void HandleButtonUp(Vector2 dragPosition)
        {
            if (ReadyToDrag && dragging) EndDrag(dragPosition);
            ButtonDown = false;
        }

        protected XiGame Game { get { return game; } }

        protected KeyboardModifier Modifier { get { return modifier; } }

        protected abstract void PrepareDragHook(Vector2 dragPosition);

        protected abstract void BeginDragHook(Vector2 dragPosition);

        protected abstract void UpdateDragHook(Vector2 dragPosition);

        protected abstract void EndDragHook(Vector2 dragPosition);

        private bool ReadyToDrag { get { return ButtonDown && buttonDownTime >= dragBeginDelay; } }

        private bool ButtonDown
        {
            get { return _buttonDown; }
            set
            {
                _buttonDown = value;
                buttonDownTime = 0;
            }
        }

        private void UpdateButtonDownTime(GameTime gameTime)
        {
            buttonDownTime += gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void UpdateModifier()
        {
            KeyboardModifier modifier = game.KeyboardState.GetModifier();
            if (this.modifier == modifier) return;
            this.modifier = modifier;
            modifierChanged = true;
        }

        private void PrepareDrag(Vector2 dragPosition)
        {
            PrepareDragHook(dragPosition);
        }

        private void BeginDrag(Vector2 dragPosition)
        {
            BeginDragHook(dragPosition);
            dragging = true;
        }

        private void UpdateDrag(Vector2 dragPosition)
        {
            UpdateDragHook(dragPosition);
        }

        private void EndDrag(Vector2 dragPosition)
        {
            EndDragHook(dragPosition);
            dragging = false;
        }

        private void ModifyDrag(Vector2 dragPosition)
        {
            if (dragging) EndDrag(dragPosition);
            PrepareDrag(dragPosition);
            if (dragging) BeginDrag(dragPosition);
            modifierChanged = false;
        }

        private readonly XiGame game;
        private readonly double dragBeginDelay;
        private KeyboardModifier modifier;
        private double buttonDownTime;
        private bool modifierChanged;
        private bool dragging;
        private bool _buttonDown;
    }
}
