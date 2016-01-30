using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Xna.Framework;
using Xi;

namespace XiEditor
{
    // TODO: factor out functional duplication from FacetContext and ActorContext.
    public class FacetContext : ControllerContext
    {
        public FacetContext(XiGame game, EditorController controller) : base(game, controller) { }

        protected override bool CreateHook(string typeName)
        {
            base.CreateHook(typeName);
            Facet facetTarget = FacetTarget;
            if (facetTarget != null) TryCreateFacet(facetTarget.ActorBase, typeName);
            else foreach (Actor actor in ActorSelection) TryCreateFacet(actor, typeName);
            return true;
        }

        protected override void ChangeTypeHook()
        {
            if (ActorTarget == null) return;
            string typeName = PromptTypeName();
            if (typeName.Length == 0) return;
            XmlDocument document = new XmlDocument();
            XmlNode rootNode = document.CreateElement("Root");
            document.AppendChild(rootNode);
            Facet oldFacet = ActorTarget.Selection.OfType<Facet>().First();
            oldFacet.WriteProperties(document, rootNode, false);
            ActorTarget.RemoveFacet(oldFacet);
            Facet newFacet = ActorTarget.CreateFacet<Facet>(typeName);
            newFacet.ReadProperties(rootNode);
            newFacet.Selected = true;
        }

        protected override void DeleteHook()
        {
            Actor actorTarget = ActorTarget;
            if (actorTarget == null) return;
            Facet facet = FacetSelection.FirstOrDefault();
            if (facet == null) return;
            actorTarget.RemoveFacet(facet);
        }

        protected override XmlDocument CopyHook()
        {
            if (ActorTarget == null) return null;
            XmlDocument document = new XmlDocument();
            XmlNode rootNode = document.CreateElement("Root");
            document.AppendChild(rootNode);
            XmlNode instanceNode = document.CreateElement("Serializable");
            rootNode.AppendChild(instanceNode);
            ActorTarget.Selection.OfType<Facet>().First().Write(document, instanceNode);
            return document;
        }

        protected override void PasteHook(XmlDocument document)
        {
            XmlNode instanceNode = document.SelectSingleNode("Root").SelectSingleNode("Serializable");
            Facet facet = ActorTarget.CreateFacetFromDocument<Facet>(instanceNode);
            facet.SelectedExclusively = true;
        }

        protected override void PasteHook(XmlDocument document, Vector2 canvasPosition)
        {
            Paste(document);
        }

        protected override IEnumerable<Simulatable> GetItemsHook()
        {
            foreach (Simulatable item in ActorGroup.SimulatableChildren)
                foreach (Simulatable item2 in item.SimulatableChildren)
                    yield return item2;
        }

        protected override void SelectSameTypeHook()
        {
            base.SelectSameTypeHook();
            Facet facetTarget = FacetTarget;
            if (facetTarget == null) return;
            Simulatable facetParent = facetTarget.SimulatableParent;
            if (facetParent == null) return;
            IEnumerable<Simulatable> peersOfSameType =
                facetParent.
                SimulatableChildren.
                Where(x => x.GetType() == facetTarget.GetType()).
                Skip(1);
            foreach (Simulatable item in peersOfSameType) item.Selected = true;
        }

        private Actor ActorTarget
        {
            get
            {
                Actor singleSelectedActor = ActorSelection.SingleOrDefault();
                if (singleSelectedActor != null) return singleSelectedActor;
                Facet firstSelectedFacet = FacetSelection.FirstOrDefault();
                return firstSelectedFacet != null ? firstSelectedFacet.ActorBase : null;
            }
        }

        private IEnumerable<Actor> ActorSelection
        {
            get { return ActorGroup.SelectionBottom.OfType<Actor>(); }
        }

        private Facet FacetTarget
        {
            get { return FacetSelection.SingleOrDefault(); }
        }

        private IEnumerable<Facet> FacetSelection
        {
            get { return ActorGroup.SelectionBottom.OfType<Facet>(); }
        }

        private IEnumerable<Type> FacetTypes
        {
            get
            {
                return Game.GetTypes().Where(x =>
                    x.IsSubclassOf(typeof(Facet)) &&
                    !x.IsGenericType &&
                    !x.IsAbstract);
            }
        }

        private void TryCreateFacet(Actor actor, string typeName)
        {
            try
            {
                if (typeName != "From File...") actor.CreateFacet<Facet>(typeName);
                else
                {
                    OpenFileDialog opener = new OpenFileDialog { RestoreDirectory = true };
                    if (opener.ShowDialog() == DialogResult.OK) actor.CreateFacetFromFile<Facet>(opener.FileName);
                }
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message, "Invalid Facet Type");
            }
        }

        private string PromptTypeName()
        {
            using (SelectTypeForm selectTypeForm = new SelectTypeForm(FacetTypes.ToArray()))
            {
                selectTypeForm.ShowDialog();
                return selectTypeForm.SelectedTypeName;
            }
        }
    }
}
