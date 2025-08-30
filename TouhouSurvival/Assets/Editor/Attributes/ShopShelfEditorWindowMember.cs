using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unchord.Editor
{
    [Serializable]
    internal class ShopShelfEditorWindowMember : UnchordEditorMember
    {
        [SerializeField] internal List<SerializedShopItem> paths;

        internal Vector2 windowScrollPosition;

        internal MenuBar menuBar;
        internal LogField logField;
        internal PathBrowser pathBrowser;

        internal ShopShelfEditorWindowMember()
        {
            paths = new List<SerializedShopItem>(0);

            windowScrollPosition = Vector2.zero;

            menuBar = new MenuBar();
            logField = new LogField();
            pathBrowser = new PathBrowser(Application.streamingAssetsPath, 16);
        }
    }
}