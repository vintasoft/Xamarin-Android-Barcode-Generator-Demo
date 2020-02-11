using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;

using Java.Util;

using System;
using System.Collections.Generic;
using System.IO;

using Vintasoft.XamarinBarcode;
using Vintasoft.XamarinBarcode.SymbologySubsets;

namespace BarcodeGeneratorDemo
{
    /// <summary>
    /// The MainActivity class.
    /// </summary>
    [Activity(
        Label = "@string/app_name", Icon = "@mipmap/icon", Name = "activity.MainActivity",
        Theme = "@style/MainActivityTheme",
        MainLauncher = true,
        AlwaysRetainTaskState = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize,
        LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : AppCompatActivity
    {

        #region Constants

        /// <summary>
        /// The barcode generator fragment tag.
        /// </summary>
        const string BARCODE_GENERATOR_FRAGMENT_TAG = "BARCODEGENERATORFRAGMENT";

        /// <summary>
        /// Request code for granting the WriteExternalStorage permission.
        /// </summary>
        const int WRITE_EXTERNAL_STORAGE_PERMISSION_REQUEST_CODE = 100;

        #endregion



        #region Fields

        /// <summary>
        /// The barcode generator fragment.
        /// </summary>
        BarcodeGeneratorFragment _barcodeGeneratorFragment;

        /// <summary>
        /// The info dialog.
        /// </summary>
        Android.Support.V7.App.AlertDialog _infoDialog = null;

        /// <summary>
        /// The path to the history file.
        /// </summary>
        string _historyFilePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "history.data");

        /// <summary>
        /// Determines that the WriteExternalStorage permission is granted.
        /// </summary>
        bool _isWriteExternalStorageGranted = false;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="MainActivity"/> class.
        /// </summary>
        public MainActivity()
            : base()
        {
            VintasoftBarcode.VintasoftXamarinBarcodeLicense.Register();
        }

        #endregion



        #region Methods

        #region PUBLIC

        /// <summary>
        /// Action bar button is pressed.
        /// </summary>
        /// <param name="item">A clicked button.</param>
        /// <returns>
        /// <b>true</b> - if button click is handled; otherwise, <b>false</b>.
        /// </returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(Resources.GetString(Resource.String.vintasoft_barcodes_url)));
                    StartActivity(intent);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion


        #region PROTECTED

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
            // subscribe to the unhandled exception events
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            base.OnCreate(savedInstanceState);

            List<Utils.BarcodeInformation> values = LoadHistory();

            // set our view from the "main" layout resource
            Utils.SetLocaleFromPrefereces(this);
            SetContentView(Resource.Layout.main);

            _barcodeGeneratorFragment = (BarcodeGeneratorFragment)FragmentManager.FindFragmentByTag(BARCODE_GENERATOR_FRAGMENT_TAG);
            if (_barcodeGeneratorFragment == null)
                _barcodeGeneratorFragment = new BarcodeGeneratorFragment(values);

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Show();

            // create a new transaction
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            // add fragment to the container
            transaction.Replace(Resource.Id.mainContentFrame, _barcodeGeneratorFragment, BARCODE_GENERATOR_FRAGMENT_TAG);
            // commit the transaction
            transaction.Commit();

            // if Android version is equal or higher than 6.0 (API 23)
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                if (CheckSelfPermission(Android.Manifest.Permission.WriteExternalStorage) != Permission.Granted)
                {
                    RequestPermissions(new string[] { Android.Manifest.Permission.WriteExternalStorage }, WRITE_EXTERNAL_STORAGE_PERMISSION_REQUEST_CODE);
                }
                else
                {
                    _isWriteExternalStorageGranted = true;
                }
            }
            // if Android version is less than 6.0 (API 23)
            else
            {
                _isWriteExternalStorageGranted = true;
            }
        }

        /// <summary>
        /// Called when request permissions result occured.
        /// </summary>
        /// <param name="requestCode">The request code.</param>
        /// <param name="permissions">The permissions.</param>
        /// <param name="grantResults">The grant results.</param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == WRITE_EXTERNAL_STORAGE_PERMISSION_REQUEST_CODE)
            {
                if (grantResults[0] == Permission.Granted)
                {
                    _isWriteExternalStorageGranted = true;
                }
                else
                {
                    // finish the application
                    Finish();
                }
            }
        }

        /// <summary>
        /// Performs any final cleanup before an activity is destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            SaveHistory();
            base.OnDestroy();
        }

        /// <summary>
        /// This is called for activities that set launchMode to "singleTop" in their package,
        /// or if a client used the <see cref="Android.Content.ActivityFlags.SingleTop"/> flag when calling
        /// <see cref="Android.Content.ContextWrapper.StartActivity(Android.Content.Intent)"/>.
        /// </summary>
        /// <param name="intent">The new intent that was started for the activity.</param>
        protected override void OnNewIntent(Intent intent)
        {
            bool isBarcodeShouldBeDeleted = intent.GetBooleanExtra("delete", false);
            // if barcode should be deleted
            if (isBarcodeShouldBeDeleted)
            {
                // if clicked item index is correct
                if (_barcodeGeneratorFragment.ClickedItemIndex != -1)
                {
                    // get adapter
                    ArrayAdapter<Utils.BarcodeInformation> adapter = _barcodeGeneratorFragment.ListAdapter as ArrayAdapter<Utils.BarcodeInformation>;
                    // remove barcode
                    adapter.Remove(adapter.GetItem(_barcodeGeneratorFragment.ClickedItemIndex));
                    adapter.NotifyDataSetChanged();
                }
            }
            else
            {
                // get barcode writer settings
                string xmlSerialization = intent.GetStringExtra("barcode");
                if (xmlSerialization != null)
                {
                    WriterSettings barcodeWriterSettings = Utils.DeserializeBarcodeWriterSettings(xmlSerialization);
                    // get barcode description
                    string barcodeDescription = intent.GetStringExtra("barcodeDescription");

                    // get barcode subset
                    string barcodeSubset = intent.GetStringExtra("barcodeSubset");
                    string barcodeValue = intent.GetStringExtra("barcodeValue");

                    ArrayAdapter<Utils.BarcodeInformation> adapter = _barcodeGeneratorFragment.ListAdapter as ArrayAdapter<Utils.BarcodeInformation>;

                    // if barcode writer settings is not empty
                    if (barcodeWriterSettings != null)
                    {
                        // if clicked item index is correct
                        if (_barcodeGeneratorFragment.ClickedItemIndex != -1)
                        {
                            // update list item
                            Utils.BarcodeInformation item = adapter.GetItem(_barcodeGeneratorFragment.ClickedItemIndex);
                            item.BarcodeWriterSetting = barcodeWriterSettings;
                            item.BarcodeValue = barcodeValue;
                            item.BarcodeDescription = barcodeDescription;
                            item.BarcodeSubsetName = barcodeSubset;
                        }
                        else
                        {
                            // add value to the list
                            adapter.Add(new Utils.BarcodeInformation(barcodeWriterSettings, barcodeValue, barcodeDescription, barcodeSubset));
                        }
                        adapter.NotifyDataSetChanged();
                    }
                }
            }

            SaveHistory();

            base.OnNewIntent(intent);
        }

        /// <summary>
        /// Called when an activity you launched exits, giving you the requestCode you started
        /// it with, the resultCode it returned, and any additional data from it.
        /// </summary>
        /// <param name="requestCode">
        /// The integer request code originally supplied to <see cref="StartActivityForResult()"/>, allowing
        /// you to identify who this result came from.
        /// </param>
        /// <param name="resultCode">The integer result code returned by the child activity through its <see cref="SetResult()"/>.</param>
        /// <param name="data">
        /// An Intent, which can return result data to the caller (various data can be attached to Intent "extras").
        /// </param>
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            // if result is OK
            if (resultCode == Result.Ok)
            {
                // get barcode value
                string barcodeValue = data.GetStringExtra("SCAN_RESULT");
                // get barcode type string
                string barcodeType = data.GetStringExtra("SCAN_RESULT_FORMAT");

                // get adapter of list view
                ArrayAdapter<Utils.BarcodeInformation> adapter = _barcodeGeneratorFragment.ListAdapter as ArrayAdapter<Utils.BarcodeInformation>;

                // create new item
                Utils.BarcodeInformation newItem = new Utils.BarcodeInformation();
                // get barcode type
                object barcodeTypeObject;
                bool isSupported = Utils.BarcodeTypeNameToBarcodeTypes.TryGetValue(barcodeType, out barcodeTypeObject);

                // if barcode type is supported
                if (isSupported)
                {
                    // set barcode value
                    newItem.BarcodeValue = barcodeValue;

                    // if barcode type is subset
                    if (barcodeTypeObject is BarcodeSymbologySubset)
                    {
                        // set subset name
                        newItem.BarcodeSubsetName = barcodeType;
                        ((BarcodeSymbologySubset)barcodeTypeObject).Encode(barcodeValue, newItem.BarcodeWriterSetting);
                    }
                    else
                    {
                        // set barcode type
                        newItem.BarcodeWriterSetting.Barcode = (BarcodeType)barcodeTypeObject;
                        newItem.BarcodeWriterSetting.Value = barcodeValue;
                    }

                    // add to the list
                    adapter.Add(newItem);
                    adapter.NotifyDataSetChanged();

                    Toast.MakeText(this, Resources.GetString(Resource.String.barcode_added_message), ToastLength.Short).Show();

                    _barcodeGeneratorFragment.SetLastItemIndexToClickedItemIndex();
                    SwitchToBarcodeViewer(this, newItem);
                }
                else
                {
                    ShowInfoDialog(
                        Resources.GetString(Resource.String.app_name),
                        string.Format(Resources.GetString(Resource.String.barcode_type_unsupported_message), barcodeType));
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        #endregion


        #region INTERNAL

        /// <summary>
        /// Starts barcode scanning.
        /// </summary>
        internal void ScanBarcode()
        {
            Intent scanIntent = new Intent("com.vintasoft.barcodescanner.SCAN");
            try
            {
                // open the Vintasoft Barcode Scanner application
                StartActivityForResult(scanIntent, 0);
            }
            // if Vintasoft Barcode Scanner application is not found
            catch (ActivityNotFoundException ex)
            {
                using (Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this))
                {
                    builder.SetPositiveButton(Resource.String.ok_button, ScannerDialogPositiveButton_Clicked);
                    builder.SetNegativeButton(Resource.String.cancel_button, (EventHandler<DialogClickEventArgs>)null);
                    builder.SetTitle(Resource.String.app_name);
                    builder.SetMessage(Resource.String.vintasoft_scanner_not_found_message);

                    // create dialog
                    Android.Support.V7.App.AlertDialog dialog = builder.Create();
                    // show on screen
                    dialog.Show();
                }
            }
        }

        /// <summary>
        /// Switches from current activity to the <see cref="BarcodeViewerActivity"/> instance.
        /// </summary>
        /// <param name="context">A context.</param>
        /// <param name="item">A barcode item.</param>
        internal void SwitchToBarcodeViewer(Context context, Utils.BarcodeInformation item)
        {
            Intent intent = new Intent(context, typeof(BarcodeViewerActivity));
            // serialize data
            string xmlSerialization = Utils.SerializeBarcodeWriterSettings(item.BarcodeWriterSetting);
            intent.PutExtra("barcode", xmlSerialization);
            intent.PutExtra("barcodeDescription", item.BarcodeDescription);
            intent.PutExtra("barcodeSubset", item.BarcodeSubsetName);
            intent.PutExtra("barcodeValue", item.BarcodeValue);
            // start barcode viewer activity
            StartActivity(intent);
        }


        /// <summary>
        /// Deletes saved history.
        /// </summary>
        internal void DeleteSavedHistory()
        {
            File.Delete(_historyFilePath);
        }

        /// <summary>
        /// Shows a dialog with information.
        /// </summary>
        /// <param name="title">A dialog title.</param>
        /// <param name="value">A dialog value.</param>
        internal void ShowInfoDialog(string title, string value)
        {
            // create the dialog builder
            using (Android.Support.V7.App.AlertDialog.Builder dialogBuilder = new Android.Support.V7.App.AlertDialog.Builder(this))
            {
                // create a button
                dialogBuilder.SetPositiveButton(Resource.String.ok_button, (EventHandler<DialogClickEventArgs>)null);

                // create a dialog
                _infoDialog = dialogBuilder.Create();

                // set dialog title
                _infoDialog.SetTitle(title);
                // set dialog message
                _infoDialog.SetMessage(value);
                // show on screen
                _infoDialog.Show();

                // get display size
                DisplayMetrics displayMetrics = new DisplayMetrics();
                WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
                int height = (int)(displayMetrics.HeightPixels * 3 / 4);

                // if dialog content height is greater than 3/4 of screen height
                if (_infoDialog.Window.Attributes.Height > height)
                    _infoDialog.Window.SetLayout(_infoDialog.Window.Attributes.Width, height);

                TextView dialogTextView = _infoDialog.FindViewById<TextView>(Android.Resource.Id.Message);
                // allow to select dialog text
                dialogTextView.SetTextIsSelectable(true);
                // allow to click links
                dialogTextView.MovementMethod = LinkMovementMethod.Instance;
                dialogTextView.LinksClickable = true;
                // add links
                Utils.MyLinkify.AddLinks(dialogTextView, Patterns.EmailAddress, null);
                Utils.MyLinkify.AddLinks(dialogTextView, Patterns.WebUrl, null, new Utils.MyLinkify(), null);
            }
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Opens URL "www.vintasoft.com" in the browser.
        /// </summary>
        private void TitleButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(Resources.GetString(Resource.String.vintasoft_barcodes_url)));
            StartActivity(intent);
        }

        /// <summary>
        /// "Scanner dialog" button is clicked.
        /// </summary>
        private void ScannerDialogPositiveButton_Clicked(object sender, DialogClickEventArgs args)
        {
            try
            {
                // open the Vintasoft Barcode Scanner application page in in Google Play Market
                OpenVintasoftBarcodeScannerPageInPlayMarket();
            }
            catch (ActivityNotFoundException)
            {
                // open the Vintasoft Barcode Scanner application web page in play.google.com
                OpenVintasoftBarcodeScannerPageInBrowser();
            }
        }

        /// <summary>
        /// Opens the Vintasoft Barcode Scanner application page in Google Play Market.
        /// </summary>
        private void OpenVintasoftBarcodeScannerPageInPlayMarket()
        {
            Intent playStoreIntent = new Intent(Intent.ActionView);
            playStoreIntent.SetData(Android.Net.Uri.Parse("market://details?id=com.vintasoft.barcodescanner"));
            // open the Vintasoft Barcode Scanner application page in Google Play Market
            StartActivity(playStoreIntent);
        }

        /// <summary>
        /// Opens the Vintasoft Barcode Scanner application web page in play.google.com.
        /// </summary>
        private void OpenVintasoftBarcodeScannerPageInBrowser()
        {
            Intent playStoreIntentLink = new Intent(Intent.ActionView);
            playStoreIntentLink.SetData(Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=com.vintasoft.barcodescanner"));
            // open the Vintasoft Barcode Scanner application page in Google Play Market in browser
            StartActivity(playStoreIntentLink);
        }


        #region Save/Load history

        /// <summary>
        /// Saves history.
        /// </summary>
        private void SaveHistory()
        {
            // adapter of list view
            ArrayAdapter<Utils.BarcodeInformation> adapter = _barcodeGeneratorFragment.ListAdapter as ArrayAdapter<Utils.BarcodeInformation>;

            if (adapter != null)
            {
                try
                {
                    // open file stream to save history
                    using (FileStream fileStream = new FileStream(_historyFilePath, FileMode.Create, FileAccess.Write))
                    {
                        // open java object stream, which allows to seralize objects to a file
                        using (Java.IO.ObjectOutputStream outStream = new Java.IO.ObjectOutputStream(fileStream))
                        {
                            // temp list to get serialized items
                            List<ArrayList> list = new List<ArrayList>();

                            // for each list item
                            for (int index = 0; index < adapter.Count; index++)
                            {
                                // get item
                                Utils.BarcodeInformation item = adapter.GetItem(index);
                                // serialize writer settings
                                string xmlBarcode = Utils.SerializeBarcodeWriterSettings(item.BarcodeWriterSetting);
                                // create java list for four strings
                                ArrayList serializedStruct = new ArrayList();
                                serializedStruct.Add(xmlBarcode);
                                serializedStruct.Add(item.BarcodeDescription);
                                serializedStruct.Add(item.BarcodeSubsetName);
                                serializedStruct.Add(item.BarcodeValue);
                                // add serialized object to the temp list
                                list.Add(serializedStruct);
                            }
                            // create java list
                            using (ArrayList arrayList = new ArrayList(list))
                            {
                                // write java serializable objects to the file
                                outStream.WriteObject(arrayList);
                            }
                            outStream.Close();
                        }
                        fileStream.Close();
                    }
                }
                catch (Exception e)
                {
                    Toast.MakeText(this, string.Format("Save history error: {0}", e.Message), ToastLength.Long).Show();
                    LogUnhandledException(e);
                }
            }
        }

        /// <summary>
        /// Loads history.
        /// </summary>
        /// <returns>
        /// A history list.
        /// </returns>
        private List<Utils.BarcodeInformation> LoadHistory()
        {
            List<Utils.BarcodeInformation> values = new List<Utils.BarcodeInformation>();

            if (File.Exists(_historyFilePath))
            {
                try
                {
                    // open file stream
                    using (FileStream fileStream = new FileStream(_historyFilePath, FileMode.Open, FileAccess.Read))
                    {
                        // open java object stream, which allows to deseralize objects from a file
                        using (Java.IO.ObjectInputStream inStream = new Java.IO.ObjectInputStream(fileStream))
                        {
                            // read java list
                            JavaList arrayList = (JavaList)inStream.ReadObject();

                            for (int index = 0; index < arrayList.Count; index++)
                            {
                                // read four items: barcode writer settings + barcode value + barcode description + barcode subset name
                                JavaList serializedStruct = arrayList[index] as JavaList;
                                Utils.BarcodeInformation barcodeInformation = new Utils.BarcodeInformation();
                                barcodeInformation.BarcodeWriterSetting = Utils.DeserializeBarcodeWriterSettings((string)serializedStruct[0]);
                                barcodeInformation.BarcodeDescription = (string)serializedStruct[1];
                                barcodeInformation.BarcodeSubsetName = (string)serializedStruct[2];
                                if (serializedStruct.Count > 3)
                                    barcodeInformation.BarcodeValue = (string)serializedStruct[3];
                                else
                                    barcodeInformation.BarcodeValue = barcodeInformation.BarcodeWriterSetting.Value;

                                values.Add(barcodeInformation);
                            }
                            inStream.Close();
                        }
                        fileStream.Close();
                    }
                }
                catch (Exception e)
                {
                    Toast.MakeText(this, string.Format("Load history error: {0}", e.Message), ToastLength.Long).Show();
                    LogUnhandledException(e);
                }
            }

            return values;
        }

        #endregion


        #region Unhandled exceptions

        /// <summary>
        /// Handles an Unhandled exception, which occured when managed exception was translated into an Android throwable.
        /// </summary>
        private void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            LogUnhandledException(e.Exception);
        }

        /// <summary>
        /// Handles an Unhandled exception.
        /// </summary>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogUnhandledException((Exception)e.ExceptionObject);
        }

        /// <summary>
        /// Saves log file with unhandled exception info.
        /// </summary>
        /// <param name="exception">An exception.</param>
        private void LogUnhandledException(Exception exception)
        {
            string errorMessage = string.Format("Time: {0}\r\nError: UnhandledExceptionMessage\r\n{1}\r\nStackTrace: {2}",
                DateTime.Now, exception.Message, exception.StackTrace);

            try
            {
                // copy to clipboard if possible

                // get clipboard
                ClipboardManager clipboard = (ClipboardManager)GetSystemService(Android.Content.Context.ClipboardService);
                // create data for clipboard
                ClipData clip = ClipData.NewPlainText("VintasoftBarcodeDemoError", errorMessage);
                // put data to clipboard
                clipboard.PrimaryClip = clip;
            }
            catch (Exception e)
            {
                Toast.MakeText(Application.ApplicationContext, string.Format("Logging Exception {0}", e.Message), ToastLength.Short).Show();
            }
        }

        #endregion

        #endregion

        #endregion

    }
}

