using Android.App;
using Android.Content;
using Android.OS;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;

using System;
using System.Collections.Generic;

using Vintasoft.XamarinBarcode;
using Vintasoft.XamarinBarcode.BarcodeInfo;
using Vintasoft.XamarinBarcode.SymbologySubsets;

namespace BarcodeGeneratorDemo
{
    /// <summary>
    /// A fragment, which allows to edit a barcode value.
    /// </summary>
    public class BarcodeEditorFragment : Fragment
    {

        #region Nested classes

        /// <summary>
        /// The array adapter to show values in spinner.
        /// </summary>
        internal class EnumSpinnerArrayAdapter : ArrayAdapter<object>
        {

            #region Constructors

            /// <summary>
            /// Initializes a new instance of <see cref="EnumArrayAdapter"/> class.
            /// </summary>
            /// <param name="context">A context.</param>
            internal EnumSpinnerArrayAdapter(Context context)
                : this(context, new List<object>())
            {
            }

            /// <summary>
            /// Initializes a new instance of <see cref="EnumArrayAdapter"/> class.
            /// </summary>
            /// <param name="context">A context.</param>
            /// <param name="values">A values list.</param>
            internal EnumSpinnerArrayAdapter(Context context, IList<object> values)
                : base(context, Android.Resource.Layout.SimpleSpinnerItem, values)
            {
            }

            #endregion



            #region Methods

            /// <summary>
            /// Returns the view of single item in drop down view.
            /// </summary>
            /// <param name="position">An index.</param>
            /// <param name="convertView">A view to convert.</param>
            /// <param name="parent">A parent view group.</param>
            /// <returns>
            /// The item view.
            /// </returns>
            public override View GetDropDownView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView;
                if (view == null)
                    view = LayoutInflater.From(Context).Inflate(Android.Resource.Layout.SimpleSpinnerDropDownItem, parent, false);

                // get the first text view
                TextView textView1 = view.FindViewById<TextView>(Android.Resource.Id.Text1);
                // set single line property
                textView1.SetSingleLine(true);
                // set max length of line
                textView1.SetMaxEms(20);

                // get item by position
                object item = GetItem(position);
                string value = null;
                if (item is DataMatrixSymbolType)
                    value = Utils.DataMatrixSymbolTypeToString((DataMatrixSymbolType)item);

                if (item is HanXinCodeSymbolVersion)
                    value = Utils.HanXinCodeSymbolVersionToString((HanXinCodeSymbolVersion)item);

                if (item is HanXinCodeErrorCorrectionLevel)
                    value = Utils.HanXinCodeErrorCorrectionLevelToString((HanXinCodeErrorCorrectionLevel)item);

                if (item is QRSymbolVersion)
                    value = Utils.QRSymbolVersionToString((QRSymbolVersion)item);

                if (item is QRErrorCorrectionLevel)
                    value = Utils.QRErrorCorrectionLevelToString((QRErrorCorrectionLevel)item);

                if (item is MicroPDF417SymbolType)
                    value = Utils.MicroPDF417SymbolTypeToString((MicroPDF417SymbolType)item);

                if (item is PDF417ErrorCorrectionLevel)
                    value = Utils.PDF417ErrorCorrectionLevelToString((PDF417ErrorCorrectionLevel)item);

                if (item is int)
                {
                    if ((int)item == -1)
                        value = Context.Resources.GetString(Resource.String.auto_value);
                    else
                        value = item.ToString();
                }

                textView1.Text = value;

                return view;
            }

            /// <summary>
            /// Returns the view of single item in list view.
            /// </summary>
            /// <param name="position">An index.</param>
            /// <param name="convertView">A view to convert.</param>
            /// <param name="parent">A parent view group.</param>
            /// <returns>
            /// The item view.
            /// </returns>
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView;
                if (view == null)
                    view = LayoutInflater.From(Context).Inflate(Android.Resource.Layout.SimpleSpinnerItem, parent, false);

                // get the first text view
                TextView textView1 = view.FindViewById<TextView>(Android.Resource.Id.Text1);
                // set single line property
                textView1.SetSingleLine(true);
                // set max length of line
                textView1.SetMaxEms(20);

                // get item by position
                object item = GetItem(position);
                string value = null;
                if (item is DataMatrixSymbolType)
                    value = Utils.DataMatrixSymbolTypeToString((DataMatrixSymbolType)item);

                if (item is HanXinCodeSymbolVersion)
                    value = Utils.HanXinCodeSymbolVersionToString((HanXinCodeSymbolVersion)item);

                if (item is HanXinCodeErrorCorrectionLevel)
                    value = Utils.HanXinCodeErrorCorrectionLevelToString((HanXinCodeErrorCorrectionLevel)item);

                if (item is QRSymbolVersion)
                    value = Utils.QRSymbolVersionToString((QRSymbolVersion)item);

                if (item is QRErrorCorrectionLevel)
                    value = Utils.QRErrorCorrectionLevelToString((QRErrorCorrectionLevel)item);

                if (item is MicroPDF417SymbolType)
                    value = Utils.MicroPDF417SymbolTypeToString((MicroPDF417SymbolType)item);

                if (item is PDF417ErrorCorrectionLevel)
                    value = Utils.PDF417ErrorCorrectionLevelToString((PDF417ErrorCorrectionLevel)item);

                if (item is int)
                {
                    if ((int)item == -1)
                        value = Context.Resources.GetString(Resource.String.auto_value);
                    else
                        value = item.ToString();
                }

                textView1.Text = value;

                return view;
            }

            /// <summary>
            /// Returns index of <paramref name="value"/>.
            /// </summary>
            /// <param name="value">A value.</param>
            /// <returns>
            /// Value index - if array adapter has value; otherwise - -1.
            /// </returns>
            internal int GetPositionByValue(object value)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (value.Equals(GetItem(i)))
                        return i;
                }
                return -1;
            }

            #endregion

        }

        #endregion



        #region Fields

        /// <summary>
        /// Determines whether <see cref="_barcodeTypeName"/> is subset name.
        /// </summary>
        bool _isSubset = false;

        /// <summary>
        /// A string representation of barcode type.
        /// </summary>
        string _barcodeTypeName;

        /// <summary>
        /// The barcode value.
        /// </summary>
        string _barcodeValue = string.Empty;

        /// <summary>
        /// The barcode description.
        /// </summary>
        string _barcodeDescription;

        /// <summary>
        /// The view for viewing and editing the barcode value.
        /// </summary>
        EditText _barcodeValueEditText;

        /// <summary>
        /// The view for viewing and editing the barcode description.
        /// </summary>
        EditText _barcodeDescriptionEditText;


        /// <summary>
        /// The barcode error correction level spinner.
        /// </summary>
        Spinner _barcodeErrorCorrectionLevelSpinner;

        /// <summary>
        /// The barcode symbol size spinner.
        /// </summary>
        Spinner _barcodeSymbolSizeSpinner;

        /// <summary>
        /// The second barcode symbol size spinner.
        /// </summary>
        Spinner _barcodeSymbolSizeSpinner2;


        /// <summary>
        /// The barcode image preview.
        /// </summary>
        ImageView _barcodeImagePreview;

        /// <summary>
        /// The barcode generator error text view.
        /// </summary>
        TextView _barcodeGeneratorErrorTextView;


        /// <summary>
        /// The barcode writer settings.
        /// </summary>
        WriterSettings _barcodeWriterSettings = new WriterSettings();

        /// <summary>
        /// The barcode writer.
        /// </summary>
        BarcodeWriter _barcodeWriter = new BarcodeWriter();

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="BarcodeEditorFragment"/> class.
        /// </summary>
        internal BarcodeEditorFragment()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BarcodeEditorFragment"/> class.
        /// </summary>
        /// <param name="barcodeTypeName">A string representation of barcode type.</param>
        internal BarcodeEditorFragment(string barcodeTypeName)
            : base()
        {
            _barcodeTypeName = barcodeTypeName;
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

            // Create your fragment here
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
            // inflate view
            View view = inflater.Inflate(Resource.Layout.barcode_editor_layout, container, false);

            _barcodeValueEditText = view.FindViewById<EditText>(Resource.Id.barcode_value_edit_text);
            _barcodeValueEditText.MovementMethod = LinkMovementMethod.Instance;
            _barcodeDescriptionEditText = view.FindViewById<EditText>(Resource.Id.barcode_description_edit_text);
            _barcodeDescriptionEditText.MovementMethod = LinkMovementMethod.Instance;
            _barcodeImagePreview = view.FindViewById<ImageView>(Resource.Id.barcode_image_view);
            _barcodeGeneratorErrorTextView = view.FindViewById<TextView>(Resource.Id.barcode_generator_error_text_view);

            BarcodeType barcodeType;
            // get barcode type
            object barcodeTypeObject = Utils.BarcodeTypeNameToBarcodeTypes[_barcodeTypeName];
            bool isDigitsOnlyBarcode = false;
            // if barcode type is subset
            if (barcodeTypeObject is BarcodeSymbologySubset)
            {
                _isSubset = true;
                barcodeType = ((BarcodeSymbologySubset)barcodeTypeObject).BaseType;
                isDigitsOnlyBarcode = Utils.IsDigitsOnlyBarcode((BarcodeSymbologySubset)barcodeTypeObject);
            }
            else
            {
                barcodeType = (BarcodeType)barcodeTypeObject;
                isDigitsOnlyBarcode = Utils.IsDigitsOnlyBarcode((BarcodeType)barcodeTypeObject);
            }

            // if barcode type is type, which contains digits only
            if (isDigitsOnlyBarcode)
                _barcodeValueEditText.KeyListener = DigitsKeyListener.GetInstance("0123456789-+= ,.");

            // if barcode type is 2D barcode type
            // and barcode type is not MaxiCode
            if (Utils.Is2DBarcode(barcodeType) && barcodeType != BarcodeType.MaxiCode)
            {
                switch (barcodeType)
                {
                    // if barcode type is Aztec
                    case BarcodeType.Aztec:
                        InitAztecAdditionalOptionsView(view);
                        break;

                    // if barcode type is Data Matrix
                    case BarcodeType.DataMatrix:
                        InitDataMatrixAdditionalOptionsView(view);
                        break;

                    // if barcode type is HanXinCode
                    case BarcodeType.HanXinCode:
                        InitHanXinCodeAdditionalOptionsView(view);
                        break;

                    // if barcode type is QR
                    case BarcodeType.QR:
                        InitQRAdditionalOptionsView(view);
                        break;

                    // if barcode type is MicroQR
                    case BarcodeType.MicroQR:
                        InitMicroQRAdditionalOptionsView(view);
                        break;

                    // if barcode type is PDF417
                    case BarcodeType.PDF417:
                        InitPDF417AdditionalOptionsView(view);
                        break;

                    // if barcode type is MicroPDF417
                    case BarcodeType.MicroPDF417:
                        InitMicroPDF417AdditionalOptionsView(view);
                        break;
                }
            }

            return view;
        }

        /// <summary>
        /// Creates menu.
        /// </summary>
        /// <param name="menu">A menu.</param>
        /// <param name="inflater">An inflater.</param>
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.create_barcode_menu, menu);
            base.OnCreateOptionsMenu(menu, inflater);
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
            switch (item.ItemId)
            {
                // if accept button is pressed
                case Resource.Id.action_accept:
                    // if barcode value can be created
                    if (CheckValue(true))
                    {
                        // if parent activity is BarcodeTypeSelectorActivity
                        if (Activity is BarcodeTypeSelectorActivity)
                        {
                            Intent intent = new Intent(Activity, typeof(BarcodeViewerActivity));
                            // serialize data
                            string xmlSerialization = Utils.SerializeBarcodeWriterSettings(_barcodeWriterSettings);
                            intent.PutExtra("barcode", xmlSerialization);
                            intent.PutExtra("barcodeDescription", _barcodeDescriptionEditText.Text);
                            intent.PutExtra("barcodeValue", _barcodeValue);
                            if (_isSubset)
                                intent.PutExtra("barcodeSubset", _barcodeTypeName);

                            // start barcode viewer activity
                            StartActivity(intent);
                        }
                        // if parent activity is barcode viewer activity
                        else if (Activity is BarcodeViewerActivity)
                        {
                            // set changed values to the parent activity
                            ((BarcodeViewerActivity)Activity).BarcodeInformation.BarcodeWriterSetting = _barcodeWriterSettings.Clone();
                            ((BarcodeViewerActivity)Activity).BarcodeInformation.BarcodeDescription = _barcodeDescriptionEditText.Text;
                            ((BarcodeViewerActivity)Activity).BarcodeInformation.BarcodeValue = _barcodeValue;
                            // return to the parent activity
                            Activity.OnBackPressed();
                        }
                    }
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
        /// Called when the Fragment is visible to the user and actively running.
        /// </summary>
        public override void OnResume()
        {
            base.OnResume();

            // if barcode type is 2D barcode type
            // and barcode type is not MaxiCode
            if (Utils.Is2DBarcode(_barcodeWriterSettings.Barcode) && _barcodeWriterSettings.Barcode != BarcodeType.MaxiCode)
            {
                switch (_barcodeWriterSettings.Barcode)
                {
                    // if barcode type is Aztec
                    case BarcodeType.Aztec:
                        ArrayAdapter<int> aztecAdapter = (ArrayAdapter<int>)_barcodeErrorCorrectionLevelSpinner.Adapter;
                        int position = aztecAdapter.GetPosition(_barcodeWriterSettings.AztecErrorCorrectionDataPercent);
                        if (position != -1)
                            _barcodeErrorCorrectionLevelSpinner.SetSelection(position);

                        aztecAdapter = (ArrayAdapter<int>)_barcodeSymbolSizeSpinner.Adapter;
                        position = aztecAdapter.GetPosition(_barcodeWriterSettings.AztecDataLayers);
                        if (position != -1)
                            _barcodeSymbolSizeSpinner.SetSelection(position);
                        break;

                    // if barcode type is Data Matrix
                    case BarcodeType.DataMatrix:
                        EnumSpinnerArrayAdapter dataMatrixAdapter = (EnumSpinnerArrayAdapter)_barcodeSymbolSizeSpinner.Adapter;
                        position = dataMatrixAdapter.GetPositionByValue(_barcodeWriterSettings.DataMatrixSymbol);
                        if (position != -1)
                            _barcodeSymbolSizeSpinner.SetSelection(position);
                        break;

                    // if barcode type is HanXinCode
                    case BarcodeType.HanXinCode:
                        EnumSpinnerArrayAdapter hanXinAdapter = (EnumSpinnerArrayAdapter)_barcodeSymbolSizeSpinner.Adapter;
                        position = hanXinAdapter.GetPositionByValue(_barcodeWriterSettings.HanXinCodeSymbol);
                        if (position != -1)
                            _barcodeSymbolSizeSpinner.SetSelection(position);

                        hanXinAdapter = (EnumSpinnerArrayAdapter)_barcodeErrorCorrectionLevelSpinner.Adapter;
                        position = hanXinAdapter.GetPositionByValue(_barcodeWriterSettings.HanXinCodeErrorCorrectionLevel);
                        if (position != -1)
                            _barcodeErrorCorrectionLevelSpinner.SetSelection(position);
                        break;

                    // if barcode type is QR or MicroQR
                    case BarcodeType.QR:
                    case BarcodeType.MicroQR:
                        EnumSpinnerArrayAdapter qrAdapter = (EnumSpinnerArrayAdapter)_barcodeSymbolSizeSpinner.Adapter;
                        position = qrAdapter.GetPositionByValue(_barcodeWriterSettings.QRSymbol);
                        if (position != -1)
                            _barcodeSymbolSizeSpinner.SetSelection(position);

                        qrAdapter = (EnumSpinnerArrayAdapter)_barcodeErrorCorrectionLevelSpinner.Adapter;
                        position = qrAdapter.GetPositionByValue(_barcodeWriterSettings.QRErrorCorrectionLevel);
                        if (position != -1)
                            _barcodeErrorCorrectionLevelSpinner.SetSelection(position);
                        break;

                    // if barcode type is PDF417
                    case BarcodeType.PDF417:
                        EnumSpinnerArrayAdapter pdf417Adapter = (EnumSpinnerArrayAdapter)_barcodeSymbolSizeSpinner.Adapter;
                        position = pdf417Adapter.GetPosition(_barcodeWriterSettings.PDF417Rows);
                        if (position != -1)
                            _barcodeSymbolSizeSpinner.SetSelection(position);

                        pdf417Adapter = (EnumSpinnerArrayAdapter)_barcodeSymbolSizeSpinner2.Adapter;
                        position = pdf417Adapter.GetPosition(_barcodeWriterSettings.PDF417Columns);
                        if (position != -1)
                            _barcodeSymbolSizeSpinner2.SetSelection(position);

                        EnumSpinnerArrayAdapter pdf417EnumAdapter = (EnumSpinnerArrayAdapter)_barcodeErrorCorrectionLevelSpinner.Adapter;
                        position = pdf417EnumAdapter.GetPositionByValue(_barcodeWriterSettings.PDF417ErrorCorrectionLevel);
                        if (position != -1)
                            _barcodeErrorCorrectionLevelSpinner.SetSelection(position);
                        break;

                    // if barcode type is MicroPDF417
                    case BarcodeType.MicroPDF417:
                        EnumSpinnerArrayAdapter microPDF417Adapter = (EnumSpinnerArrayAdapter)_barcodeSymbolSizeSpinner.Adapter;
                        position = microPDF417Adapter.GetPositionByValue(_barcodeWriterSettings.MicroPDF417Symbol);
                        if (position != -1)
                            _barcodeSymbolSizeSpinner.SetSelection(position);
                        break;
                }
            }

            if (_barcodeSymbolSizeSpinner != null)
                _barcodeSymbolSizeSpinner.ItemSelected += BarcodeSettings_ItemSelected;
            if (_barcodeSymbolSizeSpinner2 != null)
                _barcodeSymbolSizeSpinner2.ItemSelected += BarcodeSettings_ItemSelected;
            if (_barcodeErrorCorrectionLevelSpinner != null)
                _barcodeErrorCorrectionLevelSpinner.ItemSelected += BarcodeSettings_ItemSelected;

            _barcodeValueEditText.TextChanged += BarcodeValueEditText_TextChanged;
            if (_barcodeValue != null)
                _barcodeValueEditText.Text = _barcodeValue;
            if (_barcodeDescription != null)
                _barcodeDescriptionEditText.Text = _barcodeDescription;

        }

        /// <summary>
        /// Fragment is paused.
        /// </summary>
        public override void OnPause()
        {
            if (_barcodeSymbolSizeSpinner != null)
                _barcodeSymbolSizeSpinner.ItemSelected -= BarcodeSettings_ItemSelected;
            if (_barcodeSymbolSizeSpinner2 != null)
                _barcodeSymbolSizeSpinner2.ItemSelected -= BarcodeSettings_ItemSelected;
            if (_barcodeErrorCorrectionLevelSpinner != null)
                _barcodeErrorCorrectionLevelSpinner.ItemSelected -= BarcodeSettings_ItemSelected;

            _barcodeGeneratorErrorTextView.TextChanged -= BarcodeValueEditText_TextChanged;

            base.OnPause();
        }

        #endregion


        #region INTERNAL

        /// <summary>
        /// Sets barcode barcode information to the fragment.
        /// </summary>
        /// <param name="barcodeInformation">A barcode information.</param>
        internal void SetBarcodeValue(Utils.BarcodeInformation barcodeInformation)
        {
            _barcodeValue = barcodeInformation.BarcodeValue;
            _barcodeWriterSettings = barcodeInformation.BarcodeWriterSetting;
            _barcodeDescription = barcodeInformation.BarcodeDescription;
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Barcode settngs are changed.
        /// </summary>
        private void BarcodeSettings_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            CheckValue(false);
        }

        /// <summary>
        /// Barcode value is changed.
        /// </summary>
        private void BarcodeValueEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            _barcodeValue = _barcodeValueEditText.Text;
            CheckValue(false);
        }

        /// <summary>
        /// Checks whether barcode can be created.
        /// </summary>
        /// <param name="isDialogShouldBeUsed">Determines whether alert dialog or barcode generator error text view should be used.</param>
        /// <returns>
        /// <b>true</b> - barcode can be created.
        /// <b>false</b> - barcode cannot be created.
        /// </returns>
        private bool CheckValue(bool isDialogShouldBeUsed)
        {
            try
            {
                // set barcode settings
                SetBarcodeSettings();
                _barcodeWriter.Settings = _barcodeWriterSettings;
                // try to get barcode
                Android.Graphics.Bitmap barcode = _barcodeWriter.GetBarcodeAsBitmap();
                _barcodeImagePreview.SetImageBitmap(barcode);
                _barcodeImagePreview.Visibility = ViewStates.Visible;
                _barcodeGeneratorErrorTextView.Visibility = ViewStates.Invisible;
            }
            catch (WriterSettingsException ex)
            {
                if (isDialogShouldBeUsed)
                {
                    // dialog creater
                    using (AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(Activity))
                    {
                        string message = ex.Message;

                        // create button
                        dialogBuilder.SetPositiveButton(Resources.GetString(Resource.String.ok_button), (EventHandler<DialogClickEventArgs>)null);

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
                _barcodeImagePreview.Visibility = ViewStates.Invisible;
                _barcodeGeneratorErrorTextView.Visibility = ViewStates.Visible;
                _barcodeGeneratorErrorTextView.Text = ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sets settings to the barcode.
        /// </summary>
        private void SetBarcodeSettings()
        {
            // get barcode type
            object barcodeTypeObject = Utils.BarcodeTypeNameToBarcodeTypes[_barcodeTypeName];
            // if barcode type is subset
            if (barcodeTypeObject is BarcodeSymbologySubset)
            {
                _isSubset = true;
                ((BarcodeSymbologySubset)barcodeTypeObject).Encode(_barcodeValue, _barcodeWriterSettings);
            }
            else
            {
                _barcodeWriterSettings.Value = _barcodeValue;
                // set barcode type
                _barcodeWriterSettings.Barcode = (BarcodeType)barcodeTypeObject;
            }

            switch (_barcodeWriterSettings.Barcode)
            {
                // if barcode type is Aztec
                case BarcodeType.Aztec:
                    _barcodeWriterSettings.AztecDataLayers = (int)_barcodeSymbolSizeSpinner.SelectedItem;
                    _barcodeWriterSettings.AztecErrorCorrectionDataPercent = (int)_barcodeErrorCorrectionLevelSpinner.SelectedItem;
                    break;

                // if barcode type is Data Matrix
                case BarcodeType.DataMatrix:
                    EnumSpinnerArrayAdapter dataMatrixAdapter = (EnumSpinnerArrayAdapter)_barcodeSymbolSizeSpinner.Adapter;
                    _barcodeWriterSettings.DataMatrixSymbol =
                        (DataMatrixSymbolType)dataMatrixAdapter.GetItem(_barcodeSymbolSizeSpinner.SelectedItemPosition);
                    break;

                // if barcode type is HanXinCode
                case BarcodeType.HanXinCode:
                    EnumSpinnerArrayAdapter hanXinAdapter = (EnumSpinnerArrayAdapter)_barcodeSymbolSizeSpinner.Adapter;
                    _barcodeWriterSettings.HanXinCodeSymbol =
                        (HanXinCodeSymbolVersion)hanXinAdapter.GetItem(_barcodeSymbolSizeSpinner.SelectedItemPosition);

                    hanXinAdapter = (EnumSpinnerArrayAdapter)_barcodeErrorCorrectionLevelSpinner.Adapter;
                    _barcodeWriterSettings.HanXinCodeErrorCorrectionLevel =
                        (HanXinCodeErrorCorrectionLevel)hanXinAdapter.GetItem(_barcodeErrorCorrectionLevelSpinner.SelectedItemPosition);
                    break;

                // if barcode type is QR or MicroQR
                case BarcodeType.QR:
                case BarcodeType.MicroQR:
                    EnumSpinnerArrayAdapter qrAdapter = (EnumSpinnerArrayAdapter)_barcodeSymbolSizeSpinner.Adapter;
                    _barcodeWriterSettings.QRSymbol =
                        (QRSymbolVersion)qrAdapter.GetItem(_barcodeSymbolSizeSpinner.SelectedItemPosition);

                    qrAdapter = (EnumSpinnerArrayAdapter)_barcodeErrorCorrectionLevelSpinner.Adapter;
                    _barcodeWriterSettings.QRErrorCorrectionLevel =
                        (QRErrorCorrectionLevel)qrAdapter.GetItem(_barcodeErrorCorrectionLevelSpinner.SelectedItemPosition);
                    break;

                // if barcode type is PDF417
                case BarcodeType.PDF417:
                    _barcodeWriterSettings.PDF417Rows = (int)_barcodeSymbolSizeSpinner.SelectedItem;
                    _barcodeWriterSettings.PDF417Columns = (int)_barcodeSymbolSizeSpinner2.SelectedItem;

                    EnumSpinnerArrayAdapter pdf417EnumAdapter = (EnumSpinnerArrayAdapter)_barcodeErrorCorrectionLevelSpinner.Adapter;
                    _barcodeWriterSettings.PDF417ErrorCorrectionLevel =
                        (PDF417ErrorCorrectionLevel)pdf417EnumAdapter.GetItem(_barcodeErrorCorrectionLevelSpinner.SelectedItemPosition);
                    break;

                // if barcode type is MicroPDF417
                case BarcodeType.MicroPDF417:
                    EnumSpinnerArrayAdapter microPDF417Adapter = (EnumSpinnerArrayAdapter)_barcodeSymbolSizeSpinner.Adapter;
                    _barcodeWriterSettings.MicroPDF417Symbol =
                        (MicroPDF417SymbolType)microPDF417Adapter.GetItem(_barcodeSymbolSizeSpinner.SelectedItemPosition);
                    break;
            }

            // get display size
            DisplayMetrics displayMetrics = new DisplayMetrics();
            Activity.WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            // get predefined max hieght of postal barcode
            float maxPostalBarcodeHeight = Resources.GetDimensionPixelSize(Resource.Dimension.max_postal_barcode_height);
            // get predefined max width of one encoded symbol of postal barcode
            float maxOneSymbolPostalBarcodeWidth = Resources.GetDimensionPixelSize(Resource.Dimension.max_one_symbol_postal_barcode_width);
            
            // barcode size
            int barcodeWidth;
            int barcodeHeight;
            // get optimal barcode size
            Utils.GetOptimalBarcodeSize(
                displayMetrics, _barcodeWriter.Settings.Barcode,
                maxPostalBarcodeHeight, maxOneSymbolPostalBarcodeWidth, _barcodeWriter.Settings.Value.Length,
                out barcodeWidth, out barcodeHeight);

            if (!Utils.IsEanOrUpcaOrUpceBarcode(_barcodeWriter.Settings.Barcode))
                _barcodeWriter.Settings.ValueVisible = false;

            // set barcode size
            _barcodeWriter.Settings.Height = barcodeHeight;
            _barcodeWriter.Settings.SetWidth(barcodeWidth);
            // set barcode text size
            _barcodeWriter.Settings.ValuePaint.TextSize = Resources.GetDimensionPixelSize(Resource.Dimension.barcode_value_bitmap_text_size);
        }


        #region InitViews

        /// <summary>
        /// Initializes the additional options view for PDF417 barcode type.
        /// </summary>
        /// <param name="view">The fragment view.</param>
        private void InitPDF417AdditionalOptionsView(View view)
        {
            // get additional option views
            LinearLayout symbolSizeLayout = view.FindViewById<LinearLayout>(Resource.Id.barcode_symbol_size_layout);
            LinearLayout errorCorrectionLevelLayout = view.FindViewById<LinearLayout>(Resource.Id.barcode_error_correction_level_layout);
            TextView symbolSizeLabel = view.FindViewById<TextView>(Resource.Id.symbol_size_text_view);
            TextView symbolSizeLabel2 = view.FindViewById<TextView>(Resource.Id.symbol_size_text_view2);
            TextView errorCorrectionLevelLabel = view.FindViewById<TextView>(Resource.Id.error_correction_level_text_view);
            // set addidional options visible
            symbolSizeLayout.Visibility = ViewStates.Visible;
            errorCorrectionLevelLayout.Visibility = ViewStates.Visible;
            symbolSizeLabel2.Visibility = ViewStates.Visible;
            // set custom labels
            symbolSizeLabel.Text = Resources.GetString(Resource.String.pdf417_rows_label);
            symbolSizeLabel2.Text = Resources.GetString(Resource.String.pdf417_cols_label);
            errorCorrectionLevelLabel.Text = Resources.GetString(Resource.String.aztec_error_correction_level_label);


            // create adapters
            EnumSpinnerArrayAdapter errorCorrectionLevelAdapter = new EnumSpinnerArrayAdapter(Activity);
            EnumSpinnerArrayAdapter symbolSizeAdapter = new EnumSpinnerArrayAdapter(Activity);
            EnumSpinnerArrayAdapter symbolSizeAdapter2 = new EnumSpinnerArrayAdapter(Activity);
            errorCorrectionLevelAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            symbolSizeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            symbolSizeAdapter2.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            // set values
            errorCorrectionLevelAdapter.AddAll(Utils.PDF417ErrorCorrectionLevelList);
            symbolSizeAdapter.AddAll(Utils.PDF417Rows);
            symbolSizeAdapter2.AddAll(Utils.PDF417Columns);


            // get picker views
            _barcodeErrorCorrectionLevelSpinner = view.FindViewById<Spinner>(Resource.Id.barcode_error_correction_level_spinner);
            _barcodeSymbolSizeSpinner = view.FindViewById<Spinner>(Resource.Id.barcode_symbol_size_spinner);
            _barcodeSymbolSizeSpinner2 = view.FindViewById<Spinner>(Resource.Id.barcode_symbol_size_spinner2);
            _barcodeSymbolSizeSpinner2.Visibility = ViewStates.Visible;
            // set adapters
            _barcodeErrorCorrectionLevelSpinner.Adapter = errorCorrectionLevelAdapter;
            _barcodeSymbolSizeSpinner.Adapter = symbolSizeAdapter;
            _barcodeSymbolSizeSpinner2.Adapter = symbolSizeAdapter2;

            // set default values
            _barcodeErrorCorrectionLevelSpinner.SetSelection(errorCorrectionLevelAdapter.GetPositionByValue(_barcodeWriterSettings.PDF417ErrorCorrectionLevel));
            _barcodeSymbolSizeSpinner.SetSelection(symbolSizeAdapter.GetPositionByValue(_barcodeWriterSettings.PDF417Rows));
            _barcodeSymbolSizeSpinner2.SetSelection(symbolSizeAdapter2.GetPositionByValue(_barcodeWriterSettings.PDF417Columns));
        }

        /// <summary>
        /// Initializes the additional options view for MicroPDF417 barcode type.
        /// </summary>
        /// <param name="view">The fragment view.</param>
        private void InitMicroPDF417AdditionalOptionsView(View view)
        {
            // get additional option view
            LinearLayout symbolSizeLayout = view.FindViewById<LinearLayout>(Resource.Id.barcode_symbol_size_layout);
            // set addidional option visible
            symbolSizeLayout.Visibility = ViewStates.Visible;

            // create adapter
            EnumSpinnerArrayAdapter symbolSizeAdapter = new EnumSpinnerArrayAdapter(Activity);
            symbolSizeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            // set values
            symbolSizeAdapter.AddAll(Utils.MicroPDF417SimbolSizes);

            // get picker view
            _barcodeSymbolSizeSpinner = view.FindViewById<Spinner>(Resource.Id.barcode_symbol_size_spinner);
            // set adapter
            _barcodeSymbolSizeSpinner.Adapter = symbolSizeAdapter;
            // set default values
            _barcodeSymbolSizeSpinner.SetSelection(symbolSizeAdapter.GetPositionByValue(_barcodeWriterSettings.MicroPDF417Symbol));
        }

        /// <summary>
        /// Initializes the additional options view for MicroQRCode barcode type.
        /// </summary>
        /// <param name="view">The fragment view.</param>
        private void InitMicroQRAdditionalOptionsView(View view)
        {
            // get additional option views
            LinearLayout symbolSizeLayout = view.FindViewById<LinearLayout>(Resource.Id.barcode_symbol_size_layout);
            LinearLayout errorCorrectionLevelLayout = view.FindViewById<LinearLayout>(Resource.Id.barcode_error_correction_level_layout);
            // set addidional options visible
            symbolSizeLayout.Visibility = ViewStates.Visible;
            errorCorrectionLevelLayout.Visibility = ViewStates.Visible;


            // create adapters
            EnumSpinnerArrayAdapter symbolSizeAdapter = new EnumSpinnerArrayAdapter(Activity);
            EnumSpinnerArrayAdapter errorCorrectionLevelAdapter = new EnumSpinnerArrayAdapter(Activity);
            errorCorrectionLevelAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            symbolSizeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            // set values
            symbolSizeAdapter.AddAll(Utils.MicroQRSymbolSizes);
            errorCorrectionLevelAdapter.AddAll(Utils.MicroQRErrorCorrectionLevel);

            // get picker views
            _barcodeErrorCorrectionLevelSpinner = view.FindViewById<Spinner>(Resource.Id.barcode_error_correction_level_spinner);
            _barcodeSymbolSizeSpinner = view.FindViewById<Spinner>(Resource.Id.barcode_symbol_size_spinner);
            // set adapters
            _barcodeErrorCorrectionLevelSpinner.Adapter = errorCorrectionLevelAdapter;
            _barcodeSymbolSizeSpinner.Adapter = symbolSizeAdapter;

            // set default values
            _barcodeErrorCorrectionLevelSpinner.SetSelection(errorCorrectionLevelAdapter.GetPositionByValue(_barcodeWriterSettings.QRErrorCorrectionLevel));
            _barcodeSymbolSizeSpinner.SetSelection(symbolSizeAdapter.GetPositionByValue(_barcodeWriterSettings.QRSymbol));
        }

        /// <summary>
        /// Initializes the additional options view for QRCode barcode type.
        /// </summary>
        /// <param name="view">The fragment view.</param>
        private void InitQRAdditionalOptionsView(View view)
        {
            // get additional option views
            LinearLayout symbolSizeLayout = view.FindViewById<LinearLayout>(Resource.Id.barcode_symbol_size_layout);
            LinearLayout errorCorrectionLevelLayout = view.FindViewById<LinearLayout>(Resource.Id.barcode_error_correction_level_layout);
            // set addidional options visible
            symbolSizeLayout.Visibility = ViewStates.Visible;
            errorCorrectionLevelLayout.Visibility = ViewStates.Visible;


            // create adapters
            EnumSpinnerArrayAdapter symbolSizeAdapter = new EnumSpinnerArrayAdapter(Activity);
            EnumSpinnerArrayAdapter errorCorrectionLevelAdapter = new EnumSpinnerArrayAdapter(Activity);
            errorCorrectionLevelAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            symbolSizeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            // set values
            symbolSizeAdapter.AddAll(Utils.QRCodeSymbolSizes);
            errorCorrectionLevelAdapter.AddAll(Utils.QRCodeErrorCorrectionLevel);

            // get picker views
            _barcodeErrorCorrectionLevelSpinner = view.FindViewById<Spinner>(Resource.Id.barcode_error_correction_level_spinner);
            _barcodeSymbolSizeSpinner = view.FindViewById<Spinner>(Resource.Id.barcode_symbol_size_spinner);
            // set adapters
            _barcodeErrorCorrectionLevelSpinner.Adapter = errorCorrectionLevelAdapter;
            _barcodeSymbolSizeSpinner.Adapter = symbolSizeAdapter;

            // set default values
            _barcodeErrorCorrectionLevelSpinner.SetSelection(errorCorrectionLevelAdapter.GetPositionByValue(_barcodeWriterSettings.QRErrorCorrectionLevel));
            _barcodeSymbolSizeSpinner.SetSelection(symbolSizeAdapter.GetPositionByValue(_barcodeWriterSettings.QRSymbol));
        }

        /// <summary>
        /// Initializes the additional options view for HanXinCode barcode type.
        /// </summary>
        /// <param name="view">The fragment view.</param>
        private void InitHanXinCodeAdditionalOptionsView(View view)
        {
            // get additional option views
            LinearLayout symbolSizeLayout = view.FindViewById<LinearLayout>(Resource.Id.barcode_symbol_size_layout);
            LinearLayout errorCorrectionLevelLayout = view.FindViewById<LinearLayout>(Resource.Id.barcode_error_correction_level_layout);
            TextView symbolSizeLabel = view.FindViewById<TextView>(Resource.Id.symbol_size_text_view);
            // set addidional options visible
            symbolSizeLayout.Visibility = ViewStates.Visible;
            errorCorrectionLevelLayout.Visibility = ViewStates.Visible;
            // set custom label
            symbolSizeLabel.Text = Resources.GetString(Resource.String.han_xin_symbol_version_label);

            // create adapters
            EnumSpinnerArrayAdapter symbolSizeAdapter = new EnumSpinnerArrayAdapter(Activity);
            EnumSpinnerArrayAdapter errorCorrectionLevelAdapter = new EnumSpinnerArrayAdapter(Activity);
            errorCorrectionLevelAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            symbolSizeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            // set values
            symbolSizeAdapter.AddAll(Utils.HanXinSymbolSizes);
            errorCorrectionLevelAdapter.AddAll(Utils.HanXinErrorCorrectionLevel);

            // get picker views
            _barcodeErrorCorrectionLevelSpinner = view.FindViewById<Spinner>(Resource.Id.barcode_error_correction_level_spinner);
            _barcodeSymbolSizeSpinner = view.FindViewById<Spinner>(Resource.Id.barcode_symbol_size_spinner);
            // set adapters            
            _barcodeErrorCorrectionLevelSpinner.Adapter = errorCorrectionLevelAdapter;
            _barcodeSymbolSizeSpinner.Adapter = symbolSizeAdapter;

            // set default values
            _barcodeErrorCorrectionLevelSpinner.SetSelection(errorCorrectionLevelAdapter.GetPositionByValue(_barcodeWriterSettings.HanXinCodeErrorCorrectionLevel));
            _barcodeSymbolSizeSpinner.SetSelection(symbolSizeAdapter.GetPositionByValue(_barcodeWriterSettings.HanXinCodeSymbol));
        }

        /// <summary>
        /// Initializes the additional options view for DataMatrix barcode type.
        /// </summary>
        /// <param name="view">The fragment view.</param>
        private void InitDataMatrixAdditionalOptionsView(View view)
        {
            // get additional option view
            LinearLayout symbolSizeLayout = view.FindViewById<LinearLayout>(Resource.Id.barcode_symbol_size_layout);
            // set addidional option visible
            symbolSizeLayout.Visibility = ViewStates.Visible;

            // create adapter
            EnumSpinnerArrayAdapter symbolSizeAdapter = new EnumSpinnerArrayAdapter(Activity);
            symbolSizeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            // set values
            symbolSizeAdapter.AddAll(Utils.DataMatrixSymbolSizes);

            // get picker view
            _barcodeSymbolSizeSpinner = view.FindViewById<Spinner>(Resource.Id.barcode_symbol_size_spinner);
            // set adapter
            _barcodeSymbolSizeSpinner.Adapter = symbolSizeAdapter;

            // set default values
            _barcodeSymbolSizeSpinner.SetSelection(symbolSizeAdapter.GetPositionByValue(_barcodeWriterSettings.DataMatrixSymbol));
        }

        /// <summary>
        /// Initializes the additional options view for Aztec barcode type.
        /// </summary>
        /// <param name="view">The fragment view.</param>
        private void InitAztecAdditionalOptionsView(View view)
        {
            // get additional option views
            LinearLayout symbolSizeLayout = view.FindViewById<LinearLayout>(Resource.Id.barcode_symbol_size_layout);
            LinearLayout errorCorrectionLevelLayout = view.FindViewById<LinearLayout>(Resource.Id.barcode_error_correction_level_layout);
            TextView symbolSizeLabel = view.FindViewById<TextView>(Resource.Id.symbol_size_text_view);
            TextView errorCorrectionLevelLabel = view.FindViewById<TextView>(Resource.Id.error_correction_level_text_view);
            // set addidional options visible
            symbolSizeLayout.Visibility = ViewStates.Visible;
            errorCorrectionLevelLayout.Visibility = ViewStates.Visible;
            // set custom labels
            symbolSizeLabel.Text = Resources.GetString(Resource.String.aztec_layers_count_label);
            errorCorrectionLevelLabel.Text = Resources.GetString(Resource.String.aztec_error_correction_level_label);

            // create adapters
            ArrayAdapter<int> errorCorrectionLevelAdapter = new ArrayAdapter<int>(Activity, Android.Resource.Layout.SimpleSpinnerItem);
            ArrayAdapter<int> symbolSizeAdapter = new ArrayAdapter<int>(Activity, Android.Resource.Layout.SimpleSpinnerItem);
            errorCorrectionLevelAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            symbolSizeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            // set values
            errorCorrectionLevelAdapter.AddAll(Utils.AztecErrorCorrectionLevel);
            symbolSizeAdapter.AddAll(Utils.AztecLayersCount);

            // get picker views
            _barcodeErrorCorrectionLevelSpinner = view.FindViewById<Spinner>(Resource.Id.barcode_error_correction_level_spinner);
            _barcodeSymbolSizeSpinner = view.FindViewById<Spinner>(Resource.Id.barcode_symbol_size_spinner);
            // set adapters
            _barcodeErrorCorrectionLevelSpinner.Adapter = errorCorrectionLevelAdapter;
            _barcodeSymbolSizeSpinner.Adapter = symbolSizeAdapter;

            // set default values
            _barcodeErrorCorrectionLevelSpinner.SetSelection(errorCorrectionLevelAdapter.GetPosition(_barcodeWriterSettings.AztecErrorCorrectionDataPercent));
            _barcodeSymbolSizeSpinner.SetSelection(symbolSizeAdapter.GetPosition(_barcodeWriterSettings.AztecDataLayers));
        }

        #endregion

        #endregion

        #endregion

    }
}