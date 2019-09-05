using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;

namespace BarcodeGeneratorDemo
{
    /// <summary>
    /// The barcode type selector activity.
    /// </summary>
    [Activity(
        Label = "@string/title_barcode_type_selector", Icon = "@mipmap/icon", Name = "activity.BarcodeTypeSelectorActivity",
        ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize,
        WindowSoftInputMode = SoftInput.AdjustResize)]
    public class BarcodeTypeSelectorActivity : AppCompatActivity
    {

        #region Methods

        /// <summary>
        /// Action bar button is pressed.
        /// </summary>
        /// <param name="item">A clicked button.</param>
        /// <returns>
        /// <b>true</b> - if button click is handled; otherwise - <b>false</b>.
        /// </returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }


        /// <summary>
        /// Called when the activity is starting.
        /// </summary>
        /// <param name="savedInstanceState">
        /// If the activity is being re-initialized after previously being shut down then
        /// this <see cref="Bundle"/> contains the data it most recently supplied in 
        /// <see cref="Android.App.Activity.OnSaveInstanceState(Android.OS.Bundle)"/>.
        /// Note: Otherwise it is <b>null</b>.
        /// </param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Utils.SetLocaleFromPrefereces(this);
            SetContentView(Resource.Layout.main);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetTitle(Resource.String.title_barcode_type_selector);

            // create a new transaction
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            // add fragment to the container
            transaction.Replace(Resource.Id.mainContentFrame, new BarcodeTypeListFragment(Resources.GetStringArray(Resource.Array.barcode_list), false));
            // commit the transaction
            transaction.Commit();
        }

        #endregion

    }
}