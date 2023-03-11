using UnityEngine;

namespace Vocario.UI
{
    [CreateAssetMenu(fileName = "UIManager_", menuName = "Scriptable/UIManager", order = 11)]
    public class UIManager : ScriptableObject
    {
        [SerializeField]
        protected ScreensMapping _screenMapping = new ScreensMapping();
        protected string _activeScreenName = null;

        public bool ShowScreen(string screenName)
        {
            if (_activeScreenName == screenName || !_screenMapping.Contains(screenName))
            {
                return false;
            }
            _screenMapping[ _activeScreenName ].Hide();
            // _screenMapping[ screenName ].Show();
            _activeScreenName = screenName;
            return true;
        }
    }

    public class ScreensMapping : SerializableDictionary<string, AController> { }
}
