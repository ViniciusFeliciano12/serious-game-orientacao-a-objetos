//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using Esper.FeelSpeak.Overworld;
using Esper.FeelSpeak.UI.UGUI;
using UnityEditor;
using UnityEngine;

namespace Esper.FeelSpeak.Editor
{
    public static class FeelSpeakEditorMenuItems
    {
        [MenuItem("GameObject/Feel Speak/Initializer", priority = 0)]
        private static void AddInitializer()
        {
            var obj = Object.FindFirstObjectByType<FeelSpeakInitializer>(FindObjectsInactive.Include);

            if (!obj)
            {
                obj = ObjectFactory.CreateGameObject("FeelSpeakInitializer", typeof(FeelSpeakInitializer)).GetComponent<FeelSpeakInitializer>();

                if (Selection.activeGameObject)
                {
                    obj.transform.SetParent(Selection.activeGameObject.transform, false);
                }

                Undo.RegisterCreatedObjectUndo(obj, "Undo Add Feel Speak Initializer");
            }

            Selection.activeGameObject = obj.gameObject;
        }

        [MenuItem("GameObject/Feel Speak/UI/Dialogue Box", priority = 1)]
        private static void AddDialogueBox()
        {
            var existing = Object.FindFirstObjectByType<DialogueBoxUGUI>();

            if (existing)
            {
                Selection.activeGameObject = existing.gameObject;
                return;
            }

            var obj = Object.Instantiate(AssetSearch.Find<DialogueBoxUGUI>("FeelSpeak", "Prefabs/uGUI/DialogueBox.prefab"));
            obj.name = "DialogueBox";

            var canvas = obj.GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;

            if (Selection.activeGameObject)
            {
                obj.transform.SetParent(Selection.activeGameObject.transform, false);
            }

            Undo.RegisterCreatedObjectUndo(obj, "Add Dialogue Box");
            Selection.activeGameObject = obj.gameObject;
        }

        [MenuItem("GameObject/Feel Speak/UI/Choices List", priority = 2)]
        private static void AddChoicesList()
        {
            var existing = Object.FindFirstObjectByType<ChoicesListUGUI>();

            if (existing)
            {
                Selection.activeGameObject = existing.gameObject;
                return;
            }

            var obj = Object.Instantiate(AssetSearch.Find<ChoicesListUGUI>("FeelSpeak", "Prefabs/uGUI/ChoicesList.prefab"));
            obj.name = "ChoicesList";

            var canvas = obj.GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;

            if (Selection.activeGameObject)
            {
                obj.transform.SetParent(Selection.activeGameObject.transform, false);
            }

            Undo.RegisterCreatedObjectUndo(obj, "Add Choices List");
            Selection.activeGameObject = obj.gameObject;
        }

        [MenuItem("GameObject/Feel Speak/UI/Choice Timer", priority = 3)]
        private static void AddChoiceTimer()
        {
            var existing = Object.FindFirstObjectByType<ChoiceTimerUGUI>();

            if (existing)
            {
                Selection.activeGameObject = existing.gameObject;
                return;
            }

            var obj = Object.Instantiate(AssetSearch.Find<ChoicesListUGUI>("FeelSpeak", "Prefabs/uGUI/ChoiceTimer.prefab"));
            obj.name = "ChoiceTimer";

            var canvas = obj.GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;

            if (Selection.activeGameObject)
            {
                obj.transform.SetParent(Selection.activeGameObject.transform, false);
            }

            Undo.RegisterCreatedObjectUndo(obj, "Add Choice Timer");
            Selection.activeGameObject = obj.gameObject;
        }

        [MenuItem("GameObject/Feel Speak/UI/Name Tag", priority = 4)]
        private static void AddNameTag()
        {
            var obj = Object.Instantiate(AssetSearch.Find<NameTag>("FeelSpeak", "Prefabs/uGUI/NameTag.prefab"));
            obj.name = "NameTag";

            var canvas = obj.GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;

            if (Selection.activeGameObject)
            {
                obj.transform.SetParent(Selection.activeGameObject.transform, false);
            }

            var speaker = obj.GetComponentInParent<Speaker>();

            if (speaker)
            {
                obj.speaker = speaker;
            }

            Undo.RegisterCreatedObjectUndo(obj, "Add Name Tag");
            Selection.activeGameObject = obj.gameObject;
        }
    }
}
#endif