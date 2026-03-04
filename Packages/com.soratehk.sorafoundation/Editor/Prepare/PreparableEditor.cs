using SaintsField.Editor;
using SoraTehk.Prepare;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoraTehk.Editor {
    public abstract class PreparableEditor : SaintsEditor {
        public override VisualElement CreateInspectorGUI() {
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            var root = base.CreateInspectorGUI() ?? new VisualElement();

            if (target is not IPreparable) return root;

            root.Add(
                new Button(OnPrepareButtonPressed) {
                    text = "Prepare",
                    style = {
                        marginTop = 8
                    }
                }
            );

            return root;
        }

        private void OnPrepareButtonPressed() {
            PreparableSystem.Prepare(target);
        }
    }

    [CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
    public sealed class PreparableMonoBehaviourEditor : PreparableEditor {
    }

    [CustomEditor(typeof(ScriptableObject), true), CanEditMultipleObjects]
    public sealed class PreparableScriptableObjectEditor : PreparableEditor {
    }
}