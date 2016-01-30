using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xi;
using SysRectangle = System.Drawing.Rectangle;

namespace XiEditor
{
    public class EditorController : Disposable
    {
        public EditorController(XiGame game)
        {
            XiHelper.ArgumentNullCheck(game);
            this.game = game;
            actors = CreateActors(game);
            SetUpContexts();
            ResetSettings();
        }

        public ActorGroup ActorGroup { get { return actors; } }

        public Viewport CanvasViewport
        {
            get
            {
                return new Viewport()
                {
                    Height = canvasTransform.Height,
                    Width = canvasTransform.Width,
                    X = canvasTransform.Left,
                    Y = canvasTransform.Top,
                    MinDepth = game.Camera.NearPlane,
                    MaxDepth = game.Camera.FarPlane
                };
            }
        }

        /// <summary>
        /// The canvas transform.
        /// </summary>
        public SysRectangle CanvasTransform
        {
            get { return canvasTransform; }
            set { canvasTransform = value; }
        }

        /// <summary>
        /// The camera's position.
        /// </summary>
        public Vector3 CameraPosition
        {
            get { return _cameraPosition; }
            set
            {
                _cameraPosition = value;
                RefreshCameraTransform();
            }
        }

        /// <summary>
        /// The camera's left vector.
        /// </summary>
        public Vector3 CameraLeft { get { return _cameraOrientation.Left; } }

        /// <summary>
        /// The camera's right vector.
        /// </summary>
        public Vector3 CameraRight { get { return _cameraOrientation.Right; } }

        /// <summary>
        /// The camera's up vector.
        /// </summary>
        public Vector3 CameraUp { get { return _cameraOrientation.Up; } }

        /// <summary>
        /// The camera's down vector.
        /// </summary>
        public Vector3 CameraDown { get { return _cameraOrientation.Down; } }

        /// <summary>
        /// The camera's forward vector.
        /// </summary>
        public Vector3 CameraForward { get { return _cameraOrientation.Forward; } }

        /// <summary>
        /// The camera's backward vector.
        /// </summary>
        public Vector3 CameraBackward { get { return _cameraOrientation.Backward; } }

        /// <summary>
        /// The camera's eular x angle.
        /// </summary>
        public float CameraAngleX
        {
            get { return _cameraOrientation.Angle1; }
            set
            {
                // NOTE: property prevents the camera from looking directly (or beyond) up and down
                const float upEpsilon = 0.001f;
                if (value <= -MathHelper.PiOver2) value = -MathHelper.PiOver2 + upEpsilon;
                else if (value >= MathHelper.PiOver2) value = MathHelper.PiOver2 - upEpsilon;
                _cameraOrientation.Angle1 = value;
                RefreshCameraTransform();
            }
        }

        /// <summary>
        /// The camera's eular y angle.
        /// </summary>
        public float CameraAngleY
        {
            get { return _cameraOrientation.Angle2; }
            set
            {
                _cameraOrientation.Angle2 = value;
                RefreshCameraTransform();
            }
        }

        /// <summary>
        /// The axis in which object manipulation is constrained.
        /// </summary>
        public AxisConstraint AxisConstraint
        {
            get { return axisConstraint; }
            set { axisConstraint = value; }
        }

        public float TranslationSnap
        {
            get { return translationSnap; }
            set { translationSnap = value; }
        }

        public float AngleSnap
        {
            get { return angleSnap; }
            set { angleSnap = value; }
        }

        public float CreationDepth
        {
            get { return creationDepth; }
            set { creationDepth = value; }
        }

        public bool Focused
        {
            get { return focused; }
            set { focused = value; }
        }

        public bool MouseOver
        {
            get { return mouseOver; }
            set { mouseOver = value; }
        }

        public bool AllowMouse { get { return mouseOver && focused; } }

        public bool AllowKeyboard { get { return focused; } }

        public void AddContext(ControllerContext context)
        {
            contexts.AddFirst(context);
        }

        public bool RemoveContext(ControllerContext context)
        {
            return contexts.Remove(context);
        }

        public void Advance(GameTime gameTime)
        {
            if (actors.Enabled) AdvanceActors(gameTime);
            AdvanceContexts(gameTime);
        }

        public void Visualize(GameTime gameTime)
        {
            if (actors.Visible) actors.Visualize(gameTime);
            VisualizeContexts(gameTime);
        }

        public void ResetSettings()
        {
            translationSnap = defaultPositionSnap;
            angleSnap = defaultOrientationSnap;
            creationDepth = defaultCreationDepth;
            ResetCamera();
        }

        public void New()
        {
            actors.ClearActors();
            ResetCamera();
        }

        public void Open(string fileName)
        {
            XiHelper.ArgumentNullCheck(fileName);
            actors.Dispose();
            try { actors = game.ReadRecyclable<ActorGroup>(fileName); }
            catch { actors = new ActorGroup(game); throw; }
            game.RaiseSimulationStructureChanged();
        }

        public void Save(string fileName)
        {
            XiHelper.ArgumentNullCheck(fileName);
            actors.Write(fileName);
        }

        public void CreateActor(string typeName)
        {
            foreach (ControllerContext context in contexts)
                if (context != facetContext && context.Create(typeName))
                    break;
        }

        public void CreateActor(string typeName, Vector2 canvasPosition)
        {
            foreach (ControllerContext context in contexts)
                if (context != facetContext && context.Create(typeName, canvasPosition))
                    break;
        }

        public void CreateFacet(string typeName)
        {
            facetContext.Create(typeName);
        }

        public void ChangeType()
        {
            CurrentContext.ChangeType();
        }

        public void Delete()
        {
            CurrentContext.Delete();
        }

        public void Cut()
        {
            Copy();
            Delete();
        }

        public void Copy()
        {
            clipboardContext = CurrentContext;
            clipboardContent = clipboardContext.Copy() ?? clipboardContent;
        }

        public void Paste()
        {
            if (clipboardContext != null) clipboardContext.Paste(clipboardContent);
        }

        public void Paste(Vector2 canvasPosition)
        {
            if (clipboardContext != null) clipboardContext.Paste(clipboardContent, canvasPosition);
        }

        public void SelectAll()
        {
            ActorGroup.SelectAll();
        }

        public void SelectSameType()
        {
            CurrentContext.SelectSameType();
        }

        public void ImportProperties()
        {
            if (!ActorGroup.Selection.HasOne()) return;
            OpenFileDialog importDialog = new OpenFileDialog()
            {
                DefaultExt = "xiprop",
                Filter = "Xi Property documents|*xiprop",
                RestoreDirectory = true
            };
            if (importDialog.ShowDialog() == DialogResult.OK)
            {
                XmlDocument document = game.XmlDocumentCache.GetXmlDocument(importDialog.FileName);
                XmlNode rootNode = document.SelectSingleNode("Root");
                ActorGroup.SelectionBottom.First().ReadProperties(rootNode);
            }
            importDialog.Dispose();
        }

        public void ExportSelection()
        {
            if (!ActorGroup.Selection.HasOne()) return;
            SaveFileDialog exportDialog = new SaveFileDialog()
            {
                DefaultExt = "xisel",
                Filter = "Xi Selection documents|*xisel",
                RestoreDirectory = true
            };
            if (exportDialog.ShowDialog() == DialogResult.OK) ActorGroup.SelectionBottom.First().Write(exportDialog.FileName);
            exportDialog.Dispose();
        }

        public void ExportProperties()
        {
            if (!ActorGroup.Selection.HasOne()) return;
            SaveFileDialog exportDialog = new SaveFileDialog()
            {
                DefaultExt = "xiprop",
                Filter = "Xi Property documents|*xiprop",
                RestoreDirectory = true
            };
            if (exportDialog.ShowDialog() == DialogResult.OK) ActorGroup.SelectionBottom.First().WriteProperties(exportDialog.FileName, true);
            exportDialog.Dispose();
        }

        public void Look(Direction3D direction)
        {
            ResetCameraAngles();
            switch (direction)
            {
                case Direction3D.Up: CameraAngleX = MathHelper.PiOver2; break;
                case Direction3D.Down: CameraAngleX = -MathHelper.PiOver2; break;
                case Direction3D.Left: CameraAngleY = MathHelper.PiOver2; break;
                case Direction3D.Right: CameraAngleY = -MathHelper.PiOver2; break;
                case Direction3D.Backward: CameraAngleY = MathHelper.Pi; break;
            }
        }

        public bool IsCurrentContext(ControllerContext context)
        {
            return CurrentContext == context;
        }

        public Simulatable GetItem(int hashCode)
        {
            foreach (ControllerContext context in contexts)
            {
                Simulatable item = context.GetItem(hashCode);
                if (item != null) return item;
            }
            return null;
        }

        public IEnumerable<Simulatable> GetItems()
        {
            foreach (ControllerContext context in contexts)
                foreach (Simulatable item in context.GetItems())
                    yield return item;
        }

        public void HandleMouseButtonDown(MouseButtons button, Vector2 mousePosition)
        {
            if (AllowMouse)
                foreach (ControllerContext context in contexts)
                    if (context.HandleMouseButtonDown(button, mousePosition))
                        break;
        }

        public void HandleMouseButtonUp(MouseButtons button, Vector2 mousePosition)
        {
            if (AllowMouse)
                foreach (ControllerContext context in contexts)
                    if (context.HandleMouseButtonUp(button, mousePosition))
                        break;
        }

        public void HandleMouseButtonDrag(MouseButtons button, Vector2 mousePosition)
        {
            if (AllowMouse)
                foreach (ControllerContext context in contexts)
                    if (context.HandleMouseButtonDrag(button, mousePosition))
                        break;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TearDownContexts();
                actors.Dispose();
            }
            base.Dispose(disposing);
        }

        private ControllerContext CurrentContext
        {
            get
            {
                Simulatable first = actors.SelectionBottom.FirstOrDefault();
                if (first == null) return actor3DContext; // use actor 3D context by default
                foreach (ControllerContext context in contexts)
                    if (context.Contains(first))
                        return context;
                throw new InvalidOperationException("First selected item not found in any controller context.");
            }
        }

        private static ActorGroup CreateActors(XiGame game)
        {
            ActorGroup actors = new ActorGroup(game);
            actors.RefreshOverlaidProperties();
            return actors;
        }

        private void SetUpContexts()
        {
            AddContext(actor3DContext = new Actor3DContext(game, this));
            AddContext(actor2DContext = new Actor2DContext(game, this));
            AddContext(actorUIContext = new ActorUIContext(game, this));
            AddContext(facetContext = new FacetContext(game, this));
            AddContext(new CameraContext(game, this));
        }

        private void TearDownContexts()
        {
            foreach (ControllerContext context in contexts) context.Dispose();
            contexts.Clear();
        }

        private void AdvanceActors(GameTime gameTime)
        {
            actors.Update(gameTime);
            if (game.Editing) actors.Edit(gameTime);
            else actors.Play(gameTime);
        }

        private void AdvanceContexts(GameTime gameTime)
        {
            foreach (ControllerContext context in contexts)
                context.Advance(gameTime);
        }

        private void ResetCamera()
        {
            ResetCameraAngles();
            ResetCameraPosition();
        }

        private void ResetCameraAngles()
        {
            _cameraOrientation.ClearAngles();
            RefreshCameraTransform();
        }

        private void ResetCameraPosition()
        {
            CameraPosition = CameraBackward * creationDepth;
        }

        private void RefreshCameraTransform()
        {
            game.Camera.SetTransformByLookForward(CameraPosition, Vector3.Up, CameraForward);
        }

        private void VisualizeContexts(GameTime gameTime)
        {
            foreach (ControllerContext context in contexts)
                context.Visualize(gameTime);
        }

        private const float defaultPositionSnap = 1;
        private const float defaultOrientationSnap = 5;
        private const float defaultCreationDepth = 50;
        private readonly XiGame game;
        private readonly LinkedList<ControllerContext> contexts = new LinkedList<ControllerContext>();
        private readonly EularOrientation _cameraOrientation = new EularOrientation();
        private ActorGroup actors;
        /// <summary>May be null.</summary>
        private XmlDocument clipboardContent;
        /// <summary>May be null.</summary>
        private ControllerContext clipboardContext;
        private Actor3DContext actor3DContext;
        private Actor2DContext actor2DContext;
        private ActorUIContext actorUIContext;
        private FacetContext facetContext;
        private SysRectangle canvasTransform;
        private AxisConstraint axisConstraint;
        private float translationSnap;
        private float angleSnap;
        private float creationDepth;
        private bool focused;
        private bool mouseOver;
        private Vector3 _cameraPosition;
    }
}
