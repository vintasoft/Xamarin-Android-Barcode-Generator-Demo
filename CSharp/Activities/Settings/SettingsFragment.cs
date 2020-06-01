using Android.OS;

namespace BarcodeGeneratorDemo
{
    /// <summary>
    /// The fragment that contains user interface for application settings.
    /// </summary>
    public class SettingsFragment : Android.Support.V7.Preferences.PreferenceFragmentCompat
    {

        #region Methods

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Xml.settings_page);
        }

        #endregion

    }
}