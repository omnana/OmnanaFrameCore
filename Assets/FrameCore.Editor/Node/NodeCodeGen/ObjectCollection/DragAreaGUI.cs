using System;
using UnityEditor;
using UnityEngine;

namespace FrameCore.Editor
{
    public class DragAreaGUI
    {
        public static UnityEngine.Object ObjectField(Rect position, Type objType, Type[] blackTypes = null)
        {
            return ObjectField(position, objType, null, null, null, blackTypes);
        }

        /// <summary>
        /// DragArea
        /// </summary>
        /// <param name="objectType">限定拖拽进来的类型</param>
        /// <param name="meg">显示信息</param>
        /// <returns>返回拖拽进来的Object，不符合条件为空</returns>
        public static UnityEngine.Object ObjectField(Rect position, Type objectType,
            GUIContent title = null, GUIStyle style = null, string meg = null, Type[] blackTypes = null)
        {
            Event aEvent;
            aEvent = Event.current;
            UnityEngine.Object temp = null;
            var dragArea = position;

            if (title == null)
            {
                if (string.IsNullOrEmpty(meg))
                {
                    meg = "Drag here to add";
                }

                // title = new GUIContent(meg + $"\n({objectType.Name})");
                title = new GUIContent(meg);
            }

            if (style == null)
            {
                style = new GUIStyle("Button")
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.BoldAndItalic,
                    fontSize = 12,
                };
            }

            GUI.Box(dragArea, title, style);
            switch (aEvent.type)
            {
                case EventType.DragUpdated:
                    if (!dragArea.Contains(aEvent.mousePosition))
                    {
                        return null;
                    }

                    if (Selection.activeObject)
                    {
                        UnityEngine.Object judge =
                            TryReadComponentFromGameObject(DragAndDrop.objectReferences[0], objectType, blackTypes);
                        if (judge == null)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.None;
                        }
                        else
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        }

                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }

                    break;
                case EventType.DragPerform:
                    if (!dragArea.Contains(aEvent.mousePosition))
                    {
                        return null;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (aEvent.type == EventType.DragPerform)
                    {
                        if (Selection.activeObject)
                        {
                            DragAndDrop.AcceptDrag();
                            temp = TryReadComponentFromGameObject(DragAndDrop.objectReferences[0], objectType,
                                blackTypes);
                        }
                    }

                    Event.current.Use();
                    break;
                default:
                    break;
            }

            return temp;
        }

        private static UnityEngine.Object TryReadComponentFromGameObject(UnityEngine.Object obj, Type objectType,
            Type[] blackTypes)
        {
            var go = obj as GameObject;
            if (blackTypes != null)
            {
                var comps = go.GetComponents(typeof(Component));
                foreach (var comp in comps)
                {
                    foreach (var item in blackTypes)
                    {
                        if (item.IsAssignableFrom(comp.GetType()))
                        {
                            return null;
                        }
                    }
                }
            }

            if (go != null && objectType == typeof(GameObject))
            {
                return go;
            }

            if (go != null && objectType != null && objectType.IsSubclassOf(typeof(Component)))
            {
                var comps = go.GetComponents(typeof(Component));
                foreach (var comp in comps)
                {
                    if (objectType.IsAssignableFrom(comp.GetType()))
                    {
                        return comp;
                    }
                }
            }

            return go;
        }
    }
}