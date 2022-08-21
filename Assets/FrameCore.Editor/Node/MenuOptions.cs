using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrameCore.Editor
{
    internal static partial class MenuOptions
    {
        private const string UIPath = "GameObject/Omnana/UI/";
        private const string MapPath = "GameObject/Omnana/Map/";
        private const string EntityPath = "GameObject/Omnana/Entity/";

         private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            // bool explicitParentChoice = true;
            bool isnew = false;
            if (parent == null)
            {
                // explicitParentChoice = false;

                // If in Prefab Mode, Canvas has to be part of Prefab contents,
                // otherwise use Prefab root instead.
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null && !prefabStage.IsPartOfPrefabContents(parent))
                    parent = prefabStage.prefabContentsRoot;
            }

            if (parent == null)
            {
                element.transform.SetAsLastSibling();
                return;
            }

            // Setting the element to be a child of an element already in the scene should
            // be sufficient to also move the element to that scene.
            // However, it seems the element needs to be already in its destination scene when the
            // RegisterCreatedObjectUndo is performed; otherwise the scene it was created in is dirtied.
            SceneManager.MoveGameObjectToScene(element, parent.scene);

            Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);

            var target = parent;
            if (isnew) target = parent.transform.Find("Panel")?.gameObject ?? parent;

            if (element.transform.parent == null)
            {
                Undo.SetTransformParent(element.transform, target.transform, "Parent " + element.name);
            }

            GameObjectUtility.EnsureUniqueNameForSibling(element);

            // We have to fix up the undo name since the name of the object was only known after reparenting it.
            Undo.SetCurrentGroupName("Create " + element.name);

            GameObjectUtility.SetParentAndAlign(element, target);

            Selection.activeGameObject = element;
        }

        [MenuItem(UIPath + "UIPanel", false, 2)]
        public static void AddPanel(MenuCommand menuCommand)
        {
            AddUI(menuCommand, "UI/Prefabs/UIPanel.prefab");
        }

        [MenuItem(UIPath + "UINode", false, 2)]
        public static void AddUIItem(MenuCommand menuCommand)
        {
            AddUI(menuCommand, "UI/Prefabs/UINode.prefab");
        }

        [MenuItem(MapPath + "MapStage", false, 2)]
        public static void AddMapStage(MenuCommand menuCommand)
        {
            AddUI(menuCommand, "Map/Prefabs/MapStage.prefab");
        }

        [MenuItem(MapPath + "MapNode", false, 2)]
        public static void AddMapNode(MenuCommand menuCommand)
        {
            AddUI(menuCommand, "Map/Prefabs/MapNode.prefab");
        }

        [MenuItem(EntityPath + "EntityNode", false, 2)]
        public static void AddEntityNode(MenuCommand menuCommand)
        {
            AddUI(menuCommand, "Entity/Prefabs/EntityNode.prefab");
        }
        
        static void AddUI(MenuCommand menuCommand, string filePath)
        {
            var  template = IdealResource.Load<GameObject>(filePath);
            GameObject panel = Object.Instantiate(template);
            panel.name = panel.name.Replace("(Clone)", "");
            PlaceUIElementRoot(panel, menuCommand);
        }
    }
}