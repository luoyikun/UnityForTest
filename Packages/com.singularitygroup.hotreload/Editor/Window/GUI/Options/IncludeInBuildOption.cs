using UnityEditor;

namespace SingularityGroup.HotReload.Editor {
    internal class IncludeInBuildOption : ProjectOptionBase, ISerializedProjectOption {
        public override string ShortSummary => "Include Hot Reload in player builds";
        public override string Summary => ShortSummary;

        public override string ObjectPropertyName =>
            nameof(HotReloadSettingsObject.IncludeInBuild);

        public override void InnerOnGUI(SerializedObject so) {
            string description;
            if (GetValue(so)) {
                description = "The Hot Reload runtime is included in development builds that use the Mono scripting backend.";
            } else {
                description = "The Hot Reload runtime will not be included in any build. Use this option to disable HotReload without removing it from your project.";
            }
            description += " Hot Reload is always available in the Unity Editor's Playmode.";
            EditorGUILayout.LabelField(description, HotReloadWindowStyles.WrapStyle);
        }
    }
}