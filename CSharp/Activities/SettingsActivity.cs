using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using System;

namespace BarcodeGeneratorDemo
{
    /// <summary>
    /// The activity that contains user interface for application settings.
    /// </summary>
    [Activity(
        Label = "@string/settings_barcode_button", Icon = "@mipmap/icon", Name = "activity.SettingsActivity",
        ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize,
        WindowSoftInputMode = SoftInput.AdjustResize)]
    public class SettingsActivity : PreferenceActivity, ISharedPreferencesOnSharedPreferenceChangeListener
    {

        #region Fields

        /// <summary>
        /// Previous language value.
        /// </summary>
        string _previousLanguageValue = "";

        #endregion



        #region Methods

        #region PUBLIC

        /// <summary>
        /// Called when a shared preference is changed, added, or removed.
        /// </summary>
        /// <param name="sharedPreferences">Changed shared preference.</param>
        /// <param name="key">The key of the preference that was changed, added, or removed.</param>
        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            // if language is changed
            if (key == "list_languages")
            {
                // get new language value
                string newLanguageValue = sharedPreferences.GetString(key, "auto");
                // if new language differs from the previous language
                if (newLanguageValue != _previousLanguageValue)
                {
                    // notify user that application must be restarted
                    // create the dialog builder
                    using (AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this))
                    {
                        // create a button
                        dialogBuilder.SetPositiveButton(Resource.String.ok_button, (EventHandler<DialogClickEventArgs>)null);
                        // create a dialog
                        AlertDialog dialog = dialogBuilder.Create();
                        // set dialog title
                        dialog.SetTitle(Resources.GetString(Resource.String.app_name));
                        // set dialog message
                        dialog.SetMessage(Resources.GetString(Resource.String.language_change_message));
                        // show on screen
                        dialog.Show();
                    }
                    _previousLanguageValue = newLanguageValue;
                }
            }
        }

        #endregion


        #region PROTECTED

        /// <summary>
        /// Called to do initial creation of a fragment.
        /// </summary>
        /// <param name="savedInstanceState">The saved instance state if the fragment is being re-created from a previous saved state.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Utils.SetLocaleFromPrefereces(this);

            AddPreferencesFromResource(Resource.Xml.settings_page);

            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetTitle(Resource.String.settings_barcode_button);
        }

        /// <summary>
        /// Called when the Fragment is visible to the user and actively running.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            PreferenceManager.GetDefaultSharedPreferences(this).RegisterOnSharedPreferenceChangeListener(this);
        }

        /// <summary>
        /// Called when the Fragment is no longer resumed.
        /// </summary>
        protected override void OnPause()
        {
            PreferenceManager.GetDefaultSharedPreferences(this).UnregisterOnSharedPreferenceChangeListener(this);
            base.OnPause();
        }

        /// <summary>
        /// Action bar button is pressed.
        /// </summary>
        /// <param name="item">A clicked button.</param>
        /// <returns>
        /// <b>true</b> - if button click is handled; otherwise, <b>false</b>.
        /// </returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Refreshes the preference fragment view.
        /// </summary>
        private void RefreshUI()
        {
            PreferenceScreen.RemoveAll();

            AddPreferencesFromResource(Resource.Xml.settings_page);
        }

        #endregion

        #endregion

    }
}