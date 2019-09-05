using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using System;

namespace BarcodeGeneratorDemo
{
    /// <summary>
    /// The fragment that contains user interface for application settings.
    /// </summary>
    public class SettingsFragment : PreferenceFragment
    {

        #region Methods

        /// <summary>
        /// Called to do initial creation of a fragment.
        /// </summary>
        /// <param name="savedInstanceState">The saved instance state if the fragment is being re-created from a previous saved state.</param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);            

            AddPreferencesFromResource(Resource.Xml.settings_page);
        }      

        #endregion

    }
}