using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unchord.Editor
{
    [Serializable]
    internal class AttributeEditorWindowMember : UnchordEditorMember
    {
        [SerializeField] internal List<SerializedGameplayAttributeBase> bases;
        [SerializeField] internal List<SerializedGameplayAttributeModifier> modifiers;
        [SerializeField] internal List<SerializedLevelUpExp> expTable;
        [SerializeField] internal List<SerializedDropTableRecord> dropTable;

        // For usage toggle buttons
        internal bool useBaseValueDict;
        internal bool useModifierTable;
        internal bool useExpTable;
        internal bool useDropTable;

        internal Vector2 windowScrollPosition;

        internal MenuBar menuBar;
        internal LogField logField;
        internal PathBrowser pathBrowser;

        internal AttributeEditorWindowMember()
        {
            bases = new List<SerializedGameplayAttributeBase>(0);
            modifiers = new List<SerializedGameplayAttributeModifier>(0);
            expTable = new List<SerializedLevelUpExp>(0);
            dropTable = new List<SerializedDropTableRecord>(0);

            useBaseValueDict = false;
            useModifierTable = false;
            useExpTable = false;
            useDropTable = false;

            windowScrollPosition = Vector2.zero;

            menuBar = new MenuBar();
            logField = new LogField();
            pathBrowser = new PathBrowser(Application.streamingAssetsPath, 16);
        }
    }
}