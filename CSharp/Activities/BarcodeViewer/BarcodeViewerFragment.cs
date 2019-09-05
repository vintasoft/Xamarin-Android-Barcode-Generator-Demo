using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;

using System;
using System.IO;

using Vintasoft.XamarinBarcode;
using Vintasoft.XamarinBarcode.SymbologySubsets;

namespace BarcodeGeneratorDemo
{
    /// <summary>
    /// The barcode viewer fragment.
    /// </summary>
    public class BarcodeViewerFragment : Fragment
    {

        #region Fields

        /// <summary>
        /// The text view with barcode value.
        /// </summary>
        TextView _barcodeValueTextView;

        /// <summary>
        /// The text view with barcode description.
        /// </summary>
        TextView _barcodeDescriptionTextView;

        /// <summary>
        /// The image view with barcode.
        /// </summary>
        ImageView _barcodeValueImageView;

        /// <summary>
        /// The barcode writer.
        /// </summary>
        BarcodeWriter _barcodeWriter = new BarcodeWriter();

        /// <summary>
        /// The dialog that allows to save the barcode image to a file.
        /// </summary>
        AlertDialog _saveBarcodeImageDialog;

        /// <summary>
        /// The barcode width in pixels.
        /// </summary>
        int _barcodeWidth;

        /// <summary>
        /// The barcode height in pixels.
        /// </summary>
        int _barcodeHeight;

        #endregion



        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="BarcodeViewerFragment"/> class.
        /// </summary>
        public BarcodeViewerFragment()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="BarcodeViewerFragment"/> class.
        /// </summary>
        protected BarcodeViewerFragment(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer)
            : base(javaReference, transfer)
        { }
        
        #endregion



        #region Properties

        /// <summary>
        /// Gets the barcode information.
        /// </summary>
        internal Utils.BarcodeInformation BarcodeInformation
        {
            get
            {
                return ((BarcodeViewerActivity)Activity).BarcodeInformation;
            }
        }

        /// <summary>
        /// Gets the barcode description.
        /// </summary>
        internal string BarcodeDescription
        {
            get
            {
                return ((BarcodeViewerActivity)Activity).BarcodeInformation.BarcodeDescription;
            }
        }

        #endregion



        #region Methods

        #region PUBLIC

        /// <summary>
        /// Called to do initial creation of a fragment.
        /// </summary>
        /// <param name="savedInstanceState">The saved instance state if the fragment is being re-created from a previous saved state.</param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetHasOptionsMenu(true);
        }

        /// <summary>
        /// Called to have the fragment instantiate its user interface view.
        /// </summary>
        /// <param name="inflater">The <see cref="LayoutInflater"/> object that can be used to inflate any views in the fragment.</param>
        /// <param name="container">
        /// If non-null, this is the parent view that the fragment's UI should be attached to. 
        /// The fragment should not add the view itself, but this can be used to generate the LayoutParams of the view.
        /// </param>
        /// <param name="savedInstanceState">
        /// If non-null, this fragment is being re-constructed from a previous saved state as given here.
        /// </param>
        /// <returns>
        /// The created view.
        /// </returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.barcode_viewer_layout, container, false);

            _barcodeValueTextView = view.FindViewById<TextView>(Resource.Id.barcode_value_text_view);
            _barcodeValueTextView.MovementMethod = LinkMovementMethod.Instance;
            _barcodeDescriptionTextView = view.FindViewById<TextView>(Resource.Id.barcode_description_text_view);
            _barcodeValueImageView = view.FindViewById<ImageView>(Resource.Id.barcode_value_image_view);
            
            return view;
        }

        /// <summary>
        /// Fragment is resumed.
        /// </summary>
        public override void OnResume()
        {
            base.OnResume();
            UpdateUI();
        }

        /// <summary>
        /// Creates menu.
        /// </summary>
        /// <param name="menu">A menu.</param>
        /// <param name="inflater">An inflater.</param>
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.menu_barcode_viewer, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        /// <summary>
        /// Menu button is pressed.
        /// </summary>
        /// <param name="item">A pressed button.</param>
        /// <returns>
        /// <b>true</b> if button click is handled; otherwise, <b>false</b>.
        /// </returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
                // if edit button is pressed
                case Resource.Id.action_edit:
                    ((BarcodeViewerActivity)Activity).SwitchToBarcodeEditorFragment();
                    return true;

                // if save button is pressed
                case Resource.Id.action_save:
                    ShowSaveDialog();
                    return true;

                // if delete button is pressed
                case Resource.Id.action_delete:
                    DeleteIsPressed();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        
        #endregion


        #region INTERNAL

        /// <summary>
        /// Updates the user interface.
        /// </summary>
        internal void UpdateUI()
        {
            // format string
            string formatString;
            if (BarcodeDescription == null || BarcodeDescription == "")
                formatString = "{0}";
            else
                formatString = "{0} ({1})";

            // barcode type string
            string barcodeTypeString;
            if (BarcodeInformation.BarcodeSubsetName == null || BarcodeInformation.BarcodeSubsetName == "")
                barcodeTypeString = Utils.BarcodeTypeToString(BarcodeInformation.BarcodeWriterSetting.Barcode);
            else
                barcodeTypeString = BarcodeInformation.BarcodeSubsetName;

            // show barcode value
            _barcodeValueTextView.Text = BarcodeInformation.BarcodeValue;
            _barcodeValueTextView.ScrollTo(0, 0);
            // show barcode description
            _barcodeDescriptionTextView.Text = string.Format(formatString, barcodeTypeString, BarcodeDescription);

            _barcodeWriter.Settings = BarcodeInformation.BarcodeWriterSetting.Clone();
            if (BarcodeInformation.BarcodeSubsetName != null && BarcodeInformation.BarcodeSubsetName != "")
                ((BarcodeSymbologySubset)Utils.BarcodeTypeNameToBarcodeTypes[BarcodeInformation.BarcodeSubsetName]).Encode(BarcodeInformation.BarcodeValue, _barcodeWriter.Settings);

            try
            {
                // get display size
                DisplayMetrics displayMetrics = new DisplayMetrics();
                Activity.WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
                // get parameters for postal barcodes
                float maxPostalBarcodeHeight = Resources.GetDimensionPixelSize(Resource.Dimension.max_postal_barcode_height);
                float maxOneSymbolPostalBarcodeWidth = Resources.GetDimensionPixelSize(Resource.Dimension.max_one_symbol_postal_barcode_width);
                // get optimal barcode size
                Utils.GetOptimalBarcodeSize(
                    displayMetrics, _barcodeWriter.Settings.Barcode,
                    maxPostalBarcodeHeight, maxOneSymbolPostalBarcodeWidth, _barcodeWriter.Settings.Value.Length,
                    out _barcodeWidth, out _barcodeHeight);

                if (!Utils.IsEanOrUpcaOrUpceBarcode(_barcodeWriter.Settings.Barcode))
                    _barcodeWriter.Settings.ValueVisible = false;

                // set barcode size
                _barcodeWriter.Settings.Height = _barcodeHeight;
                _barcodeWriter.Settings.SetWidth(_barcodeWidth);
                // set barcode text size
                _barcodeWriter.Settings.ValuePaint.TextSize = Utils.GetOptimalBarcodeTextSize(_barcodeWriter.Settings.Barcode, _barcodeWidth,
                    Resources.GetDimensionPixelSize(Resource.Dimension.barcode_value_bitmap_default_text_size));
                // draw the barcode
                Bitmap barcode = _barcodeWriter.GetBarcodeAsBitmap();
                BitmapDrawable previousImage = _barcodeValueImageView.Drawable as BitmapDrawable;
                if (previousImage != null)
                    previousImage.Bitmap.Recycle();
                _barcodeValueImageView.SetImageBitmap(barcode);

                // save new sizes
                _barcodeHeight = barcode.Height;
                _barcodeWidth = barcode.Width;
            }
            catch (WriterSettingsException ex)
            {
                // dialog creater
                using (AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(Activity))
                {
                    string message = ex.Message;

                    // create button
                    dialogBuilder.SetPositiveButton(Resource.String.ok_button, (EventHandler<DialogClickEventArgs>)null);

                    // create dialog
                    AlertDialog dialog = dialogBuilder.Create();

                    // set dialog title
                    dialog.SetTitle(Resources.GetString(Resource.String.error_message_title));
                    // set dialog message
                    dialog.SetMessage(message);
                    // show on screen
                    dialog.Show();
                }
            }
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Shows the dialog that allows to save the barcode image to a file.
        /// </summary>
        private void ShowSaveDialog()
        {
            // if Android version is equal or higher than 6.0 (API 23)
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                // check WriteExternalStorage permission
                if (Activity.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Permission.Granted)
                {
                    RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage }, 0);
                }
            }

            // dialog creater
            using (AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(Activity))
            {
                dialogBuilder.SetView(LayoutInflater.From(Activity).Inflate(Resource.Layout.save_dialog_layout, null));
                // create buttons
                dialogBuilder.SetPositiveButton(Resource.String.save_button, SaveDialogButtonClicked);
                dialogBuilder.SetNegativeButton(Resource.String.cancel_button, SaveDialogButtonClicked);

                // create dialog
                _saveBarcodeImageDialog = dialogBuilder.Create();

                // set dialog title
                _saveBarcodeImageDialog.SetTitle(Resources.GetString(Resource.String.save_title));

                // show on screen
                _saveBarcodeImageDialog.Show();

                EditText filePathEditText = _saveBarcodeImageDialog.FindViewById<EditText>(Resource.Id.filepath_edit_text);
                filePathEditText.Text = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
                EditText fileNameEditText = _saveBarcodeImageDialog.FindViewById<EditText>(Resource.Id.filename_edit_text);
                // set filename to the filename edit text
                fileNameEditText.Text = GetDefaultFilename(filePathEditText.Text);

                // get width and height
                EditText widthEditText = _saveBarcodeImageDialog.FindViewById<EditText>(Resource.Id.barcode_width_edit_text);
                widthEditText.Text = _barcodeWidth.ToString();
                EditText heightEditText = _saveBarcodeImageDialog.FindViewById<EditText>(Resource.Id.barcode_height_edit_text);
                heightEditText.Text = _barcodeHeight.ToString();

                // get colors
                EditText foregroundColorEdtiText = _saveBarcodeImageDialog.FindViewById<EditText>(Resource.Id.barcode_foreground_edit_text);
                foregroundColorEdtiText.TextChanged += ColorEdtiText_TextChanged;
                ColorEdtiText_TextChanged(foregroundColorEdtiText, null);
                EditText backgroundColorEdtiText = _saveBarcodeImageDialog.FindViewById<EditText>(Resource.Id.barcode_backround_edit_text);
                backgroundColorEdtiText.TextChanged += ColorEdtiText_TextChanged;
                ColorEdtiText_TextChanged(backgroundColorEdtiText, null);
            }
        }

        /// <summary>
        /// Returns the default filename.
        /// </summary>
        /// <param name="filePath">The path to a file.</param>
        /// <returns>
        /// Default filename.
        /// </returns>
        private string GetDefaultFilename(string filePath)
        {
            int maxBarcodeValueIndex = 0;
            if (_barcodeWriter.Settings.Value.Length < 8)
                maxBarcodeValueIndex = _barcodeWriter.Settings.Value.Length;
            else
                maxBarcodeValueIndex = 8;

            for (int i = 0; ; i++)
            {
                // get filename
                string filename = string.Format("{0}_{1}_{2}",
                    Utils.BarcodeTypeToString(_barcodeWriter.Settings.Barcode),
                    _barcodeWriter.Settings.Value.Substring(0, maxBarcodeValueIndex), i);

                // if file does not exist
                if (!File.Exists(System.IO.Path.Combine(filePath, filename + ".png")))
                {
                    // return filename
                    return filename;
                }
            }
        }

        /// <summary>
        /// Sets the background color to the edit text view, according to entered value.
        /// </summary>
        private void ColorEdtiText_TextChanged(object sender, EventArgs e)
        {
            // get edit text view
            EditText colorEditText = (EditText)sender;
            try
            {
                // get color
                Color backgroundColor = Color.ParseColor(colorEditText.Text);
                // set background color
                colorEditText.SetBackgroundColor(backgroundColor);

                // get RGB average value
                double rgbAverage = (backgroundColor.R + backgroundColor.G + backgroundColor.B) / 3.0;
                if (rgbAverage > 128)
                {
                    // set font color
                    colorEditText.SetTextColor(Color.Black);
                }
                else
                {
                    // set font color
                    colorEditText.SetTextColor(Color.White);
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Save dialog button is clicked.
        /// </summary>
        private void SaveDialogButtonClicked(object sender, DialogClickEventArgs args)
        {
            // get colors edit text views
            EditText foregroundColorEdtiText = _saveBarcodeImageDialog.FindViewById<EditText>(Resource.Id.barcode_foreground_edit_text);
            EditText backgroundColorEdtiText = _saveBarcodeImageDialog.FindViewById<EditText>(Resource.Id.barcode_backround_edit_text);
            backgroundColorEdtiText.TextChanged -= ColorEdtiText_TextChanged;
            foregroundColorEdtiText.TextChanged -= ColorEdtiText_TextChanged;

            // if "Save" button clicked
            if (args.Which == (int)DialogButtonType.Positive)
            {
                // get file path
                EditText filePathEditText = _saveBarcodeImageDialog.FindViewById<EditText>(Resource.Id.filepath_edit_text);
                EditText fileNameEditText = _saveBarcodeImageDialog.FindViewById<EditText>(Resource.Id.filename_edit_text);

                if (string.IsNullOrWhiteSpace(fileNameEditText.Text))
                {
                    Toast.MakeText(Activity, Resource.String.filename_empty_message, ToastLength.Long).Show();
                    return;
                }
                if (string.IsNullOrWhiteSpace(filePathEditText.Text))
                {
                    Toast.MakeText(Activity, Resource.String.filepath_empty_message, ToastLength.Long).Show();
                    return;
                }

                // get width and height
                EditText widthEditText = _saveBarcodeImageDialog.FindViewById<EditText>(Resource.Id.barcode_width_edit_text);
                EditText heightEditText = _saveBarcodeImageDialog.FindViewById<EditText>(Resource.Id.barcode_height_edit_text);

                // save showing settings
                WriterSettings previousSettings = _barcodeWriter.Settings.Clone();

                // set colors to the writer
                _barcodeWriter.Settings.ForeColor = ((ColorDrawable)foregroundColorEdtiText.Background).Color;
                _barcodeWriter.Settings.BackColor = ((ColorDrawable)backgroundColorEdtiText.Background).Color;
                // get path to the file
                string fullPathWithExtention = System.IO.Path.Combine(filePathEditText.Text, fileNameEditText.Text) + ".png";

                try
                {
                    int width = Int32.Parse(widthEditText.Text);
                    int height = Int32.Parse(heightEditText.Text);
                    // create a file
                    using (FileStream outStream = new FileStream(fullPathWithExtention, FileMode.Create, FileAccess.Write))
                    {
                        // if barcode type is EAN or UPCA or UPCE type
                        if (Utils.IsEanOrUpcaOrUpceBarcode(_barcodeWriter.Settings.Barcode))
                        {
                            _barcodeWriter.Settings.Height = height;
                            _barcodeWriter.Settings.SetWidth(width);
                            _barcodeWriter.Settings.ValuePaint.TextSize = Resources.GetDimensionPixelSize(Resource.Dimension.barcode_value_viewer_text_size);
                            // draw the barcode
                            // create the barcode image
                            using (Bitmap barcodeImage = _barcodeWriter.GetBarcodeAsBitmap())
                            {
                                // save the barcode image to the file
                                barcodeImage.Compress(Bitmap.CompressFormat.Png, 100, outStream);
                            }
                        }
                        else
                        {
                            // create the barcode image
                            using (Bitmap barcodeImage = _barcodeWriter.GetBarcodeAsBitmap(width, height, UnitOfMeasure.Pixels))
                            {
                                // save the barcode image to the file
                                barcodeImage.Compress(Bitmap.CompressFormat.Png, 100, outStream);
                            }
                        }
                    }

                    // notify user that the barcode image is successfully saved to the file
                    string message = string.Format(Resources.GetString(Resource.String.barcode_saved_message), fullPathWithExtention);
                    Toast.MakeText(Activity, message, ToastLength.Long).Show();
                }
                catch (Exception e)
                {
                    Toast.MakeText(Activity, e.Message, ToastLength.Long).Show();
                    if (File.Exists(fullPathWithExtention))
                        File.Delete(fullPathWithExtention);
                }

                // return showing settings
                _barcodeWriter.Settings = previousSettings;
            }
        }

        /// <summary>
        /// Delete is pressed.
        /// </summary>
        private void DeleteIsPressed()
        {
            // dialog creater
            using (AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(Activity))
            {
                // create buttons
                dialogBuilder.SetPositiveButton(Resources.GetString(Resource.String.delete_button), DeleteDialogButtonClicked);
                dialogBuilder.SetNegativeButton(Resources.GetString(Resource.String.cancel_button), (EventHandler<DialogClickEventArgs>) null);

                // create dialog
                _saveBarcodeImageDialog = dialogBuilder.Create();

                // set dialog title
                _saveBarcodeImageDialog.SetTitle(Resources.GetString(Resource.String.delete_title));

                // set dialog title
                _saveBarcodeImageDialog.SetMessage(Resources.GetString(Resource.String.delete_message));

                // show on screen
                _saveBarcodeImageDialog.Show();
            }
        }

        /// <summary>
        /// Delete dialog button is clicked.
        /// </summary>
        private void DeleteDialogButtonClicked(object sender, DialogClickEventArgs args)
        {
            ((BarcodeViewerActivity)Activity).SetDeleteBarcodeFlag(true);
            Activity.OnBackPressed();
        }

        #endregion

        #endregion

    }
}