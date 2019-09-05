using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;

using Vintasoft.XamarinBarcode;

namespace BarcodeGeneratorDemo
{
    /// <summary>
    /// The barcode viewer activity.
    /// </summary>
    [Activity(
        Label = "@string/title_barcode_viewer", Icon = "@mipmap/icon", Name = "activity.BarcodeViewerActivity",
        ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize,
        WindowSoftInputMode = SoftInput.AdjustResize)]
    public class BarcodeViewerActivity : AppCompatActivity
    {

        #region Fields

        /// <summary>
        /// The barcode editor fragment.
        /// </summary>
        BarcodeEditorFragment _barcodeEditorFragment;
        
        /// <summary>
        /// The barcode viewer fragment.
        /// </summary>
        BarcodeViewerFragment _barcodeViewerFragment;

        /// <summary>
        /// Determines whether barcode should be deleted.
        /// </summary>
        bool _isBarcodeShouldBeDeleted = false;

        #endregion



        #region Propeties

        Utils.BarcodeInformation _barcode;
        /// <summary>
        /// Gets the barcode information.
        /// </summary>
        internal Utils.BarcodeInformation BarcodeInformation
        {
            get
            {
                return _barcode;
            }
        }
        
        #endregion



        #region Methods

        #region PUBLIC

        /// <summary>
        /// Back button is pressed.
        /// </summary>
        public override void OnBackPressed()
        {
            // if barcode editor fragment is visible
            if (_barcodeEditorFragment != null && _barcodeEditorFragment.IsVisible)
            {
                base.OnBackPressed();
                return;
            }
            BackToMainActivity();
        }

        /// <summary>
        /// Action bar button is pressed.
        /// </summary>
        /// <param name="item">A clicked button.</param>
        /// <returns>
        /// <b>true</b> if button click is handled; otherwise, <b>false</b>.
        /// </returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
                // if home button is pressed
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
        /// Called by the system when the device configuration changes while your activity is running.
        /// </summary>
        /// <param name="newConfig">The new device configuration.</param>
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            _barcodeViewerFragment.UpdateUI();
        }

        #endregion


        #region PROTECTED

        /// <summary>
        /// Called to do initial creation of a fragment.
        /// </summary>
        /// <param name="savedInstanceState">If the fragment is being re-created from a previous saved state, 
        /// this is the state.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Utils.SetLocaleFromPrefereces(this);
            SetContentView(Resource.Layout.main);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetTitle(Resource.String.title_barcode_viewer);

            string xmlSerialization = Intent.GetStringExtra("barcode");
            WriterSettings barcodeWriterSettings = Utils.DeserializeBarcodeWriterSettings(xmlSerialization);
            string barcodeDescription = Intent.GetStringExtra("barcodeDescription");
            string barcodeSubset = Intent.GetStringExtra("barcodeSubset");
            string barcodeValue = Intent.GetStringExtra("barcodeValue");
            _barcode = new Utils.BarcodeInformation(barcodeWriterSettings, barcodeValue, barcodeDescription, barcodeSubset);
                        
            _barcodeViewerFragment = new BarcodeViewerFragment();

            // create a new transaction
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            // add fragment to the container
            transaction.Replace(Resource.Id.mainContentFrame, _barcodeViewerFragment);
            // commit the transaction
            transaction.Commit();
        }

        #endregion


        #region INTERNAL

        /// <summary>
        /// Switches to the barcode editor fragment.
        /// </summary>
        internal void SwitchToBarcodeEditorFragment()
        {
            if (_barcodeEditorFragment == null)
            {
                if (_barcode.BarcodeSubsetName != null && _barcode.BarcodeSubsetName != "")
                    _barcodeEditorFragment = new BarcodeEditorFragment(_barcode.BarcodeSubsetName);
                else
                    _barcodeEditorFragment = new BarcodeEditorFragment(Utils.BarcodeTypeToString(_barcode.BarcodeWriterSetting.Barcode));
            }

            // create a new transaction
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            // add fragment to the container
            transaction.Replace(Resource.Id.mainContentFrame, _barcodeEditorFragment);
            // add transaction to back stack
            transaction.AddToBackStack(null);
            // commit the transaction
            transaction.Commit();

            // set current barcode value to the editor
            _barcodeEditorFragment.SetBarcodeValue(BarcodeInformation);
        }

        /// <summary>
        /// Sets a value, which determines whether barcode should be deleted.
        /// </summary>
        internal void SetDeleteBarcodeFlag(bool flag)
        {
            _isBarcodeShouldBeDeleted = flag;
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Returns to the main activity.
        /// </summary>
        private void BackToMainActivity()
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            string xmlSerialization = Utils.SerializeBarcodeWriterSettings(_barcode.BarcodeWriterSetting);
            intent.PutExtra("barcode", xmlSerialization);
            intent.PutExtra("barcodeDescription", _barcode.BarcodeDescription);
            intent.PutExtra("barcodeSubset", _barcode.BarcodeSubsetName);
            intent.PutExtra("barcodeValue", _barcode.BarcodeValue);
            intent.PutExtra("delete", _isBarcodeShouldBeDeleted);
            intent.AddFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
        }

        #endregion

        #endregion

    }
}