using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Preferences;
using Android.Text.Util;
using Android.Util;
using Java.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using Vintasoft.XamarinBarcode;
using Vintasoft.XamarinBarcode.BarcodeInfo;
using Vintasoft.XamarinBarcode.SymbologySubsets;
using Vintasoft.XamarinBarcode.SymbologySubsets.GS1;

namespace BarcodeGeneratorDemo
{
    /// <summary>
    /// Contains auxiliary methods and classes.
    /// </summary>
    internal class Utils
    {

        #region Nested classes

        /// <summary>
        /// Contains the barcode writer settings and the barcode description.
        /// </summary>
        internal class BarcodeInformation
        {

            #region Constructors

            /// <summary>
            /// Initializes a new instance of <see cref="BarcodeInformation"/> class.
            /// </summary>
            internal BarcodeInformation()
            {
                _barcodeWriterSettings = new WriterSettings();
                _barcodeDescription = "";
                _barcodeValue = "";
            }

            /// <summary>
            /// Initializes a new instance of <see cref="BarcodeInformation"/> class.
            /// </summary>
            /// <param name="barcodeWriterSettings">A barcode information.</param>
            /// <param name="barcodeValue">A barcode value.</param>
            /// <param name="barcodeDescription">A barcode description.</param>
            /// <param name="subset">A barcode subset name.</param>
            internal BarcodeInformation(WriterSettings barcodeWriterSettings, string barcodeValue, string barcodeDescription, string subset)
            {
                _barcodeWriterSettings = barcodeWriterSettings;
                _barcodeValue = barcodeValue;
                _barcodeDescription = barcodeDescription;
                _barcodeSubsetName = subset;
            }

            #endregion



            #region Properties

            WriterSettings _barcodeWriterSettings;
            /// <summary>
            /// Gets or sets the settings, which are necessary for barcode generation.
            /// </summary>
            internal WriterSettings BarcodeWriterSetting
            {
                get
                {
                    return _barcodeWriterSettings;
                }
                set
                {
                    _barcodeWriterSettings = value;
                }
            }

            string _barcodeDescription;
            /// <summary>
            /// Gets or sets the barcode description.
            /// </summary>
            internal string BarcodeDescription
            {
                get
                {
                    return _barcodeDescription;
                }
                set
                {
                    _barcodeDescription = value;
                }
            }

            string _barcodeSubsetName;
            /// <summary>
            /// Gets or sets the barcode subset name.
            /// </summary>
            internal string BarcodeSubsetName
            {
                get
                {
                    return _barcodeSubsetName;
                }
                set
                {
                    _barcodeSubsetName = value;
                }
            }

            string _barcodeValue;
            /// <summary>
            /// Gets or sets the barcode value.
            /// </summary>
            internal string BarcodeValue
            {
                get
                {
                    return _barcodeValue;
                }
                set
                {
                    _barcodeValue = value;
                }
            }

            #endregion

        }

        /// <summary>
        /// Linkify take a piece of text and a regular expression and turns all of the regex matches in the text into clickable links.
        /// </summary>
        internal class MyLinkify : Linkify, Linkify.IMatchFilter
        {

            /// <summary>
            /// Determines if substring  of <paramref name="s"/> should be turned into a link.
            /// </summary>
            /// <param name="s">A string.</param>
            /// <param name="start">A start index.</param>
            /// <param name="end">An end index.</param>
            /// <returns>
            /// <b>true</b> - if substring between start and end indexes should be turned into a link; otherwise - <b>false</b>.
            /// </returns>
            public bool AcceptMatch(Java.Lang.ICharSequence s, int start, int end)
            {
                if (Patterns.IpAddress.Matcher(s.SubSequenceFormatted(start, end)).Matches())
                    return false;
                return true;
            }

        }

        #endregion



        #region Fields

        #region BarcodeTypes

        /// <summary>
        /// Dictionary: barcode type name => unsupported barcode type.
        /// </summary>
        internal static Dictionary<string, object> BarcodeTypeNameToUnsupportedBarcodeTypes = new Dictionary<string, object>();

        /// <summary>
        /// Dictionary: barcode type name => barcode type.
        /// </summary>
        internal static Dictionary<string, object> BarcodeTypeNameToBarcodeTypes = new Dictionary<string, object>();

        /// <summary>
        /// The array of popular barcode type names.
        /// </summary>
        internal static string[] SortedPopularBarcodeTypeNames;

        /// <summary>
        /// The array of sorted 2D barcode type names.
        /// </summary>
        internal static string[] SortedBarcode2DTypeNames;

        /// <summary>
        /// The array of sorted GS1 barcode type names.
        /// </summary>
        internal static string[] SortedBarcodeGS1TypeNames;

        /// <summary>
        /// The array of sorted 1D barcode type names.
        /// </summary>
        internal static string[] SortedBarcode1DTypeNames;

        /// <summary>
        /// The array of sorted 1D postal barcode type names.
        /// </summary>
        internal static string[] SortedBarcode1DPostalTypeNames;

        #endregion


        #region Aztec

        /// <summary>
        /// List of Aztec layers count.
        /// </summary>
        internal static List<int> AztecLayersCount = new List<int>();

        /// <summary>
        /// List of Aztec error correction levels.
        /// </summary>
        internal static List<double> AztecErrorCorrectionLevel = new List<double>();

        #endregion


        #region DataMatrix

        /// <summary>
        /// List of DataMatrix symbol sizes.
        /// </summary>
        internal static List<DataMatrixSymbolType> DataMatrixSymbolSizes = new List<DataMatrixSymbolType>();

        #endregion


        #region HanXinCode

        /// <summary>
        /// List of HanXinCode symbol sizes.
        /// </summary>
        internal static List<HanXinCodeSymbolVersion> HanXinSymbolSizes = new List<HanXinCodeSymbolVersion>();

        /// <summary>
        /// List of HanXinCode error correction level.
        /// </summary>
        internal static List<HanXinCodeErrorCorrectionLevel> HanXinErrorCorrectionLevel = new List<HanXinCodeErrorCorrectionLevel>();

        #endregion


        #region PDF417

        /// <summary>
        /// List of PDF417 rows.
        /// </summary>
        internal static List<int> PDF417Rows = new List<int>();

        /// <summary>
        /// List of PDF417 columns.
        /// </summary>
        internal static List<int> PDF417Columns = new List<int>();

        /// <summary>
        /// List of PDF417 error correction level.
        /// </summary>
        internal static List<PDF417ErrorCorrectionLevel> PDF417ErrorCorrectionLevelList = new List<PDF417ErrorCorrectionLevel>();

        #endregion


        #region MicroPDF417

        /// <summary>
        /// List of MicroPDF417 rows.
        /// </summary>
        internal static List<MicroPDF417SymbolType> MicroPDF417SimbolSizes = new List<MicroPDF417SymbolType>();

        #endregion


        #region QRCode

        /// <summary>
        /// List of QR symbol sizes.
        /// </summary>
        internal static List<QRSymbolVersion> QRCodeSymbolSizes = new List<QRSymbolVersion>();

        /// <summary>
        /// List of QR error correction level.
        /// </summary>
        internal static List<QRErrorCorrectionLevel> QRCodeErrorCorrectionLevel = new List<QRErrorCorrectionLevel>();

        #endregion


        #region MicroQRCode

        /// <summary>
        /// List of MicroQR symbol sizes.
        /// </summary>
        internal static List<QRSymbolVersion> MicroQRSymbolSizes = new List<QRSymbolVersion>();

        /// <summary>
        /// List of MicroQR error correction level.
        /// </summary>
        internal static List<QRErrorCorrectionLevel> MicroQRErrorCorrectionLevel = new List<QRErrorCorrectionLevel>();

        #endregion

        #endregion



        #region Constructors

        /// <summary>
        /// A static constructor, which initializes all static fields of <see cref="Utils"/> class.
        /// </summary>
        static Utils()
        {
            InitBarcodeTypes();
            InitAztecLists();
            InitDataMatrixLists();
            InitHanXinLists();
            InitQRCodeLists();
            InitMicroQRLists();
            InitPDF417Lists();
            InitMicroPDF417Lists();
        }

        #endregion



        #region Methods

        #region INTERNAL

        /// <summary>
        /// Sets locale from preferences.
        /// </summary>
        internal static void SetLocaleFromPrefereces(Activity activity)
        {
            // set the default values from an XML preference file
            PreferenceManager.SetDefaultValues(activity, Resource.Xml.settings_page, false);

            // get preferences
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(activity);
            // get the language name
            string languageToLoad = preferences.GetString("list_languages", "auto");
            // if language name is not auto
            if (languageToLoad != "auto")
            {
                // set choosen locale
                Locale locale = new Locale(languageToLoad);
                Locale.Default = locale;
                Configuration config = new Configuration();
                config.Locale = locale;
                activity.BaseContext.Resources.UpdateConfiguration(config, activity.BaseContext.Resources.DisplayMetrics);
            }
        }

        /// <summary>
        /// Returns optimal barcode size.
        /// </summary>
        /// <param name="displayMetrics">The display metrics.</param>
        /// <param name="barcodeType">The barcode type.</param>
        /// <param name="maxPostalBarcodeHeight">The maximum height of postal barcode.</param>
        /// <param name="maxOneSymbolPostalBarcodeWidth">The maximum width of one symbol encoded in postal barcode.</param>
        /// <param name="symbolCount">The symbol count of barcode value.</param>
        /// <param name="barcodeWidth">The barcode width.</param>
        /// <param name="barcodeHeight">The barcode height.</param>
        internal static void GetOptimalBarcodeSize(
            DisplayMetrics displayMetrics,
            BarcodeType barcodeType,
            float maxPostalBarcodeHeight,
            float maxOneSymbolPostalBarcodeWidth,
            int symbolCount,
            out int barcodeWidth,
            out int barcodeHeight)
        {
            // barcode size
            if (displayMetrics.WidthPixels < displayMetrics.HeightPixels)
            {
                barcodeWidth = displayMetrics.WidthPixels;
                barcodeHeight = barcodeWidth / 2;
            }
            else
            {
                barcodeWidth = displayMetrics.WidthPixels;
                barcodeHeight = displayMetrics.HeightPixels / 3;
            }

            // if barcode is not 2D barcode
            if (!Utils.Is2DBarcode(barcodeType))
            {

                // if barcode is postal barcode
                if (Utils.IsPostalBarcode(barcodeType))
                {
                    int postalBarcodeWidth = (int)(maxOneSymbolPostalBarcodeWidth * symbolCount);
                    if (barcodeHeight > maxPostalBarcodeHeight)
                        barcodeHeight = (int)maxPostalBarcodeHeight;
                    if (barcodeWidth > postalBarcodeWidth)
                        barcodeWidth = postalBarcodeWidth;
                }
            }
        }

        /// <summary>
        /// Returns optimal barcode value text size.
        /// </summary>
        /// <param name="barcodeType">The barcode type.</param>
        /// <param name="barcodeWidth">The barcode width.</param>
        /// <param name="defaultTextSize">The default text size.</param>
        /// <returns></returns>
        internal static int GetOptimalBarcodeTextSize(
            BarcodeType barcodeType,
            int barcodeWidth,
            int defaultTextSize)
        {
            // the text size
            int textSize = defaultTextSize;

            // the barcode narrow bar count
            int narrowBarCount = 0;

            // the narrow bar counts
            int ean13NarrowBars = 95;
            int spaceNarrowBars = 12;
            int plus2NarrowBars = 20;
            int plus5NarrowBars = 47;
            int ean8NarrowBars = 67;
            int upceNarrowBars = 51;

            // get width in narrow bars for barcode type
            switch (barcodeType)
            {
                case BarcodeType.EAN13:
                case BarcodeType.UPCA:
                    narrowBarCount = ean13NarrowBars;
                    break;

                case BarcodeType.EAN13Plus2:
                case BarcodeType.UPCAPlus2:
                    narrowBarCount = ean13NarrowBars + spaceNarrowBars + plus2NarrowBars;
                    break;

                case BarcodeType.EAN13Plus5:
                case BarcodeType.UPCAPlus5:
                    narrowBarCount = ean13NarrowBars + spaceNarrowBars + plus5NarrowBars;
                    break;

                case BarcodeType.EAN8:
                    narrowBarCount = ean8NarrowBars;
                    break;

                case BarcodeType.EAN8Plus2:
                    narrowBarCount = ean8NarrowBars + spaceNarrowBars + plus2NarrowBars;
                    break;

                case BarcodeType.EAN8Plus5:
                    narrowBarCount = ean8NarrowBars + spaceNarrowBars + plus5NarrowBars;
                    break;

                case BarcodeType.UPCE:
                    narrowBarCount = upceNarrowBars;
                    break;

                case BarcodeType.UPCEPlus2:
                    narrowBarCount = upceNarrowBars + spaceNarrowBars + plus2NarrowBars;
                    break;

                case BarcodeType.UPCEPlus5:
                    narrowBarCount = upceNarrowBars + spaceNarrowBars + plus5NarrowBars;
                    break;
            }

            // if narrow bar count is not 0
            if (narrowBarCount != 0)
            {
                // calculate text size
                textSize = (int)(4.0f * barcodeWidth / narrowBarCount);
            }

            return textSize;
        }


        #region Serialization

        /// <summary>
        /// Serializes the barcode writer settings.
        /// </summary>
        /// <param name="settings">The barcode writer settings.</param>
        /// <returns>An Xml representation of <see cref="WriterSettings"/>.</returns>
        internal static string SerializeBarcodeWriterSettings(WriterSettings settings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WriterSettings));
            System.Text.StringBuilder xmlSerialization = new System.Text.StringBuilder();
            using (StringWriter stream = new StringWriter(xmlSerialization))
                serializer.Serialize(stream, settings);

            return xmlSerialization.ToString();
        }

        /// <summary>
        /// Deserializes the barcode writer settings.
        /// </summary>
        /// <param name="xmlSerialization">An Xml representation of barcode writer settings.</param>
        /// <returns>The <see cref="WriterSettings"/>.</returns>
        internal static WriterSettings DeserializeBarcodeWriterSettings(string xmlSerialization)
        {
            WriterSettings barcodeSettings;
            XmlSerializer serializer = new XmlSerializer(typeof(WriterSettings));
            using (StringReader stream = new StringReader(xmlSerialization))
                barcodeSettings = (WriterSettings)serializer.Deserialize(stream);

            return barcodeSettings;
        }

        #endregion


        #region CheckBarcodeCategory

        /// <summary>
        /// Determines whether barcode type is 2D barcode.
        /// </summary>
        /// <param name="barcodeType">A barcode type.</param>
        /// <returns>
        /// <b>true</b> - if barcode type is 2D barcode type; otherwise, <b>false</b>.
        /// </returns>
        internal static bool Is2DBarcode(BarcodeType barcodeType)
        {
            return
                barcodeType == BarcodeType.Aztec ||
                barcodeType == BarcodeType.DataMatrix ||
                barcodeType == BarcodeType.QR ||
                barcodeType == BarcodeType.MicroQR ||
                barcodeType == BarcodeType.MaxiCode ||
                barcodeType == BarcodeType.PDF417 ||
                barcodeType == BarcodeType.PDF417Compact ||
                barcodeType == BarcodeType.MicroPDF417 ||
                barcodeType == BarcodeType.HanXinCode;
        }

        /// <summary>
        /// Determines whether barcode type is the matrix stacked barcode type.
        /// </summary>
        /// <param name="barcodeType">A barcode type.</param>
        /// <returns>
        /// <b>true</b> - if barcode type is the matrix stacked barcode type; otherwise, <b>false</b>.
        /// </returns>
        internal static bool IsMatrixStackedBarcode(BarcodeType barcodeType)
        {
            return
                barcodeType == BarcodeType.PDF417 ||
                barcodeType == BarcodeType.PDF417Compact ||
                barcodeType == BarcodeType.MicroPDF417 ||
                barcodeType == BarcodeType.Code16K ||
                barcodeType == BarcodeType.RSS14Stacked ||
                barcodeType == BarcodeType.RSSExpandedStacked;
        }

        /// <summary>
        /// Determines whether barcode type is the postal barcode type.
        /// </summary>
        /// <param name="barcodeType">A barcode type.</param>
        /// <returns>
        /// <b>true</b> - if barcode type is the postal barcode type; otherwise, <b>false</b>.
        /// </returns>
        internal static bool IsPostalBarcode(BarcodeType barcodeType)
        {
            return
                barcodeType == BarcodeType.Planet ||
                barcodeType == BarcodeType.Postnet ||
                barcodeType == BarcodeType.AustralianPost ||
                barcodeType == BarcodeType.DutchKIX ||
                barcodeType == BarcodeType.IntelligentMail ||
                barcodeType == BarcodeType.Mailmark4StateC ||
                barcodeType == BarcodeType.Mailmark4StateL ||
                barcodeType == BarcodeType.RoyalMail;
        }

        /// <summary>
        /// Determines whether the barcode type is the type, which can contain digits only.
        /// </summary>
        /// <param name="barcodeType">A barcode type.</param>
        /// <returns>
        /// <b>true</b> - if barcode type is type, which can contain digits only; otherwise, <b>false</b>.
        /// </returns>
        internal static bool IsDigitsOnlyBarcode(BarcodeType barcodeType)
        {
            switch (barcodeType)
            {
                // postal barcodes
                case BarcodeType.AustralianPost:
                case BarcodeType.IntelligentMail:
                case BarcodeType.Planet:
                case BarcodeType.Postnet:
                // 1D barcodes
                case BarcodeType.Code11:
                case BarcodeType.EAN13:
                case BarcodeType.EAN13Plus2:
                case BarcodeType.EAN13Plus5:
                case BarcodeType.EAN8:
                case BarcodeType.EAN8Plus2:
                case BarcodeType.EAN8Plus5:
                case BarcodeType.IATA2of5:
                case BarcodeType.Interleaved2of5:
                case BarcodeType.Matrix2of5:
                case BarcodeType.MSI:
                case BarcodeType.Pharmacode:
                case BarcodeType.RSS14:
                case BarcodeType.RSS14Stacked:
                case BarcodeType.RSSLimited:
                case BarcodeType.Standard2of5:
                case BarcodeType.UPCA:
                case BarcodeType.UPCAPlus2:
                case BarcodeType.UPCAPlus5:
                case BarcodeType.UPCE:
                case BarcodeType.UPCEPlus2:
                case BarcodeType.UPCEPlus5:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the barcode type is the type, which can contain digits only.
        /// </summary>
        /// <param name="barcodeSubset">A barcode subset.</param>
        /// <returns>
        /// <b>true</b> - if barcode subset is subset, which can contain digits only; otherwise, <b>false</b>.
        /// </returns>
        internal static bool IsDigitsOnlyBarcode(BarcodeSymbologySubset barcodeSubset)
        {
            if (barcodeSubset is Code32BarcodeSymbology)
                return true;

            if (barcodeSubset is DeutschePostIdentcodeBarcodeSymbology)
                return true;

            if (barcodeSubset is DeutschePostLeitcodeBarcodeSymbology)
                return true;

            if (barcodeSubset is DhlAwbBarcodeSymbology)
                return true;

            if (barcodeSubset is FedExGround96BarcodeSymbology)
                return true;

            if (barcodeSubset is EanVelocityBarcodeSymbology)
                return true;

            if (barcodeSubset is Interleaved2of5ChecksumISO16390)
                return true;

            if (barcodeSubset is Interleaved2of5ChecksumMod10C)
                return true;

            if (barcodeSubset is IsbnBarcodeSymbology)
                return true;

            if (barcodeSubset is IssnBarcodeSymbology)
                return true;

            if (barcodeSubset is JanBarcodeSymbology)
                return true;

            if (barcodeSubset is NumlyNumberBarcodeSymbology)
                return true;

            if (barcodeSubset is OpcBarcodeSymbology)
                return true;

            if (barcodeSubset is PznBarcodeSymbology)
                return true;

            if (barcodeSubset is Sscc18BarcodeSymbology)
                return true;

            if (barcodeSubset is SwissPostParcelBarcodeSymbology)
                return true;

            if (barcodeSubset is VicsBolBarcodeSymbology)
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether barcode type is EAN or UPCA or UPCE type.
        /// </summary>
        /// <param name="barcodeType">A barcode type.</param>
        /// <returns>
        /// <b>true</b> - if barcode type is  EAN or UPCA or UPCE type; otherwise - <b>false</b>.
        /// </returns
        internal static bool IsEanOrUpcaOrUpceBarcode(BarcodeType barcodeType)
        {
            switch (barcodeType)
            {
                case BarcodeType.EAN13:
                case BarcodeType.EAN13Plus2:
                case BarcodeType.EAN13Plus5:
                case BarcodeType.EAN8:
                case BarcodeType.EAN8Plus2:
                case BarcodeType.EAN8Plus5:
                case BarcodeType.UPCA:
                case BarcodeType.UPCAPlus2:
                case BarcodeType.UPCAPlus5:
                case BarcodeType.UPCE:
                case BarcodeType.UPCEPlus2:
                case BarcodeType.UPCEPlus5:
                    return true;
            }

            return false;
        }

        #endregion


        #region ToStringMethods

        /// <summary>
        /// Returns a string representation of <see cref="BarcodeType"/> enum value.
        /// <summary>
        /// <param name="item">An enum value.</param>
        /// <returns>A string representation of enum value.</returns>
        internal static string BarcodeTypeToString(BarcodeType item)
        {
            switch (item)
            {
                case BarcodeType.None:
                    return "None";

                case BarcodeType.Codabar:
                    return "Codabar";

                case BarcodeType.Code11:
                    return "Code11";

                case BarcodeType.Code128:
                    return "Code128";

                case BarcodeType.Standard2of5:
                    return "Standard2of5";

                case BarcodeType.Interleaved2of5:
                    return "Interleaved2of5";

                case BarcodeType.Code39:
                    return "Code39";

                case BarcodeType.Code93:
                    return "Code93";

                case BarcodeType.RSS14Stacked:
                    return "RSS14Stacked";

                case BarcodeType.RSSExpandedStacked:
                    return "RSSExpandedStacked";

                case BarcodeType.UPCE:
                    return "UPCE";

                case BarcodeType.Telepen:
                    return "Telepen";

                case BarcodeType.Postnet:
                    return "Postnet";

                case BarcodeType.Planet:
                    return "Planet";

                case BarcodeType.RoyalMail:
                    return "RoyalMail";

                case BarcodeType.AustralianPost:
                    return "AustralianPost";

                case BarcodeType.PatchCode:
                    return "PatchCode";

                case BarcodeType.PDF417:
                    return "PDF417";

                case BarcodeType.PDF417Compact:
                    return "PDF417Compact";

                case BarcodeType.EAN13:
                    return "EAN13";

                case BarcodeType.EAN8:
                    return "EAN8";

                case BarcodeType.UPCA:
                    return "UPCA";

                case BarcodeType.Plus5:
                    return "Plus5";

                case BarcodeType.UPCEPlus5:
                    return "UPCEPlus5";

                case BarcodeType.EAN13Plus5:
                    return "EAN13Plus5";

                case BarcodeType.EAN8Plus5:
                    return "EAN8Plus5";

                case BarcodeType.UPCAPlus5:
                    return "UPCAPlus5";

                case BarcodeType.Plus2:
                    return "Plus2";

                case BarcodeType.UPCEPlus2:
                    return "UPCEPlus2";

                case BarcodeType.EAN13Plus2:
                    return "EAN13Plus2";

                case BarcodeType.EAN8Plus2:
                    return "EAN8Plus2";

                case BarcodeType.UPCAPlus2:
                    return "UPCAPlus2";

                case BarcodeType.DataMatrix:
                    return "DataMatrix";

                case BarcodeType.QR:
                    return "QR";

                case BarcodeType.IntelligentMail:
                    return "IntelligentMail";

                case BarcodeType.RSS14:
                    return "RSS14";

                case BarcodeType.RSSLimited:
                    return "RSSLimited";

                case BarcodeType.RSSExpanded:
                    return "RSSExpanded";

                case BarcodeType.Aztec:
                    return "Aztec";

                case BarcodeType.Pharmacode:
                    return "Pharmacode";

                case BarcodeType.MSI:
                    return "MSI";

                case BarcodeType.UnknownLinear:
                    return "UnknownLinear";

                case BarcodeType.MicroQR:
                    return "MicroQR";

                case BarcodeType.MaxiCode:
                    return "MaxiCode";

                case BarcodeType.DutchKIX:
                    return "DutchKIX";

                case BarcodeType.MicroPDF417:
                    return "MicroPDF417";

                case BarcodeType.Mailmark4StateC:
                    return "Mailmark4StateC";

                case BarcodeType.Mailmark4StateL:
                    return "Mailmark4StateL";

                case BarcodeType.IATA2of5:
                    return "IATA2of5";

                case BarcodeType.Matrix2of5:
                    return "Matrix2of5";

                case BarcodeType.Code16K:
                    return "Code16K";

                case BarcodeType.HanXinCode:
                    return "HanXinCode";

                case BarcodeType.DotCode:
                    return "DotCode";

            }
            return null;
        }


        #region DataMatrix

        /// <summary>
        /// Returns a string representation of <see cref="DataMatrixSymbolType"/> enum value.
        /// <summary>
        /// <param name="item">Enum value.</param>
        /// <returns>A string representation of enum value.</returns>
        internal static string DataMatrixSymbolTypeToString(DataMatrixSymbolType item)
        {
            switch (item)
            {
                case DataMatrixSymbolType.Undefined:
                    return "Undefined";

                case DataMatrixSymbolType.Row10Col10:
                    return "Row10Col10";

                case DataMatrixSymbolType.Row12Col12:
                    return "Row12Col12";

                case DataMatrixSymbolType.Row14Col14:
                    return "Row14Col14";

                case DataMatrixSymbolType.Row16Col16:
                    return "Row16Col16";

                case DataMatrixSymbolType.Row18Col18:
                    return "Row18Col18";

                case DataMatrixSymbolType.Row20Col20:
                    return "Row20Col20";

                case DataMatrixSymbolType.Row22Col22:
                    return "Row22Col22";

                case DataMatrixSymbolType.Row24Col24:
                    return "Row24Col24";

                case DataMatrixSymbolType.Row26Col26:
                    return "Row26Col26";

                case DataMatrixSymbolType.Row32Col32:
                    return "Row32Col32";

                case DataMatrixSymbolType.Row36Col36:
                    return "Row36Col36";

                case DataMatrixSymbolType.Row40Col40:
                    return "Row40Col40";

                case DataMatrixSymbolType.Row44Col44:
                    return "Row44Col44";

                case DataMatrixSymbolType.Row48Col48:
                    return "Row48Col48";

                case DataMatrixSymbolType.Row52Col52:
                    return "Row52Col52";

                case DataMatrixSymbolType.Row64Col64:
                    return "Row64Col64";

                case DataMatrixSymbolType.Row72Col72:
                    return "Row72Col72";

                case DataMatrixSymbolType.Row80Col80:
                    return "Row80Col80";

                case DataMatrixSymbolType.Row88Col88:
                    return "Row88Col88";

                case DataMatrixSymbolType.Row96Col96:
                    return "Row96Col96";

                case DataMatrixSymbolType.Row104Col104:
                    return "Row104Col104";

                case DataMatrixSymbolType.Row120Col120:
                    return "Row120Col120";

                case DataMatrixSymbolType.Row132Col132:
                    return "Row132Col132";

                case DataMatrixSymbolType.Row144Col144:
                    return "Row144Col144";

                case DataMatrixSymbolType.Row8Col18:
                    return "Row8Col18";

                case DataMatrixSymbolType.Row8Col32:
                    return "Row8Col32";

                case DataMatrixSymbolType.Row12Col26:
                    return "Row12Col26";

                case DataMatrixSymbolType.Row12Col36:
                    return "Row12Col36";

                case DataMatrixSymbolType.Row16Col36:
                    return "Row16Col36";

                case DataMatrixSymbolType.Row16Col48:
                    return "Row16Col48";

            }
            return null;
        }

        #endregion


        #region HanXinCode

        /// <summary>
        /// Returns a string representation of <see cref="HanXinCodeErrorCorrectionLevel"/> enum value.
        /// <summary>
        /// <param name="item">Enum value.</param>
        /// <returns>A string representation of enum value.</returns>
        internal static string HanXinCodeErrorCorrectionLevelToString(HanXinCodeErrorCorrectionLevel item)
        {
            switch (item)
            {
                case HanXinCodeErrorCorrectionLevel.L1:
                    return "L1";

                case HanXinCodeErrorCorrectionLevel.L2:
                    return "L2";

                case HanXinCodeErrorCorrectionLevel.L3:
                    return "L3";

                case HanXinCodeErrorCorrectionLevel.L4:
                    return "L4";

            }
            return null;
        }

        /// <summary>
        /// Returns a string representation of <see cref="HanXinCodeSymbolVersion"/> enum value.
        /// <summary>
        /// <param name="item">Enum value.</param>
        /// <returns>A string representation of enum value.</returns>
        internal static string HanXinCodeSymbolVersionToString(HanXinCodeSymbolVersion item)
        {
            switch (item)
            {
                case HanXinCodeSymbolVersion.Version1:
                    return "Version1";

                case HanXinCodeSymbolVersion.Version2:
                    return "Version2";

                case HanXinCodeSymbolVersion.Version3:
                    return "Version3";

                case HanXinCodeSymbolVersion.Version4:
                    return "Version4";

                case HanXinCodeSymbolVersion.Version5:
                    return "Version5";

                case HanXinCodeSymbolVersion.Version6:
                    return "Version6";

                case HanXinCodeSymbolVersion.Version7:
                    return "Version7";

                case HanXinCodeSymbolVersion.Version8:
                    return "Version8";

                case HanXinCodeSymbolVersion.Version9:
                    return "Version9";

                case HanXinCodeSymbolVersion.Version10:
                    return "Version10";

                case HanXinCodeSymbolVersion.Version11:
                    return "Version11";

                case HanXinCodeSymbolVersion.Version12:
                    return "Version12";

                case HanXinCodeSymbolVersion.Version13:
                    return "Version13";

                case HanXinCodeSymbolVersion.Version14:
                    return "Version14";

                case HanXinCodeSymbolVersion.Version15:
                    return "Version15";

                case HanXinCodeSymbolVersion.Version16:
                    return "Version16";

                case HanXinCodeSymbolVersion.Version17:
                    return "Version17";

                case HanXinCodeSymbolVersion.Version18:
                    return "Version18";

                case HanXinCodeSymbolVersion.Version19:
                    return "Version19";

                case HanXinCodeSymbolVersion.Version20:
                    return "Version20";

                case HanXinCodeSymbolVersion.Version21:
                    return "Version21";

                case HanXinCodeSymbolVersion.Version22:
                    return "Version22";

                case HanXinCodeSymbolVersion.Version23:
                    return "Version23";

                case HanXinCodeSymbolVersion.Version24:
                    return "Version24";

                case HanXinCodeSymbolVersion.Version25:
                    return "Version25";

                case HanXinCodeSymbolVersion.Version26:
                    return "Version26";

                case HanXinCodeSymbolVersion.Version27:
                    return "Version27";

                case HanXinCodeSymbolVersion.Version28:
                    return "Version28";

                case HanXinCodeSymbolVersion.Version29:
                    return "Version29";

                case HanXinCodeSymbolVersion.Version30:
                    return "Version30";

                case HanXinCodeSymbolVersion.Version31:
                    return "Version31";

                case HanXinCodeSymbolVersion.Version32:
                    return "Version32";

                case HanXinCodeSymbolVersion.Version33:
                    return "Version33";

                case HanXinCodeSymbolVersion.Version34:
                    return "Version34";

                case HanXinCodeSymbolVersion.Version35:
                    return "Version35";

                case HanXinCodeSymbolVersion.Version36:
                    return "Version36";

                case HanXinCodeSymbolVersion.Version37:
                    return "Version37";

                case HanXinCodeSymbolVersion.Version38:
                    return "Version38";

                case HanXinCodeSymbolVersion.Version39:
                    return "Version39";

                case HanXinCodeSymbolVersion.Version40:
                    return "Version40";

                case HanXinCodeSymbolVersion.Version41:
                    return "Version41";

                case HanXinCodeSymbolVersion.Version42:
                    return "Version42";

                case HanXinCodeSymbolVersion.Version43:
                    return "Version43";

                case HanXinCodeSymbolVersion.Version44:
                    return "Version44";

                case HanXinCodeSymbolVersion.Version45:
                    return "Version45";

                case HanXinCodeSymbolVersion.Version46:
                    return "Version46";

                case HanXinCodeSymbolVersion.Version47:
                    return "Version47";

                case HanXinCodeSymbolVersion.Version48:
                    return "Version48";

                case HanXinCodeSymbolVersion.Version49:
                    return "Version49";

                case HanXinCodeSymbolVersion.Version50:
                    return "Version50";

                case HanXinCodeSymbolVersion.Version51:
                    return "Version51";

                case HanXinCodeSymbolVersion.Version52:
                    return "Version52";

                case HanXinCodeSymbolVersion.Version53:
                    return "Version53";

                case HanXinCodeSymbolVersion.Version54:
                    return "Version54";

                case HanXinCodeSymbolVersion.Version55:
                    return "Version55";

                case HanXinCodeSymbolVersion.Version56:
                    return "Version56";

                case HanXinCodeSymbolVersion.Version57:
                    return "Version57";

                case HanXinCodeSymbolVersion.Version58:
                    return "Version58";

                case HanXinCodeSymbolVersion.Version59:
                    return "Version59";

                case HanXinCodeSymbolVersion.Version60:
                    return "Version60";

                case HanXinCodeSymbolVersion.Version61:
                    return "Version61";

                case HanXinCodeSymbolVersion.Version62:
                    return "Version62";

                case HanXinCodeSymbolVersion.Version63:
                    return "Version63";

                case HanXinCodeSymbolVersion.Version64:
                    return "Version64";

                case HanXinCodeSymbolVersion.Version65:
                    return "Version65";

                case HanXinCodeSymbolVersion.Version66:
                    return "Version66";

                case HanXinCodeSymbolVersion.Version67:
                    return "Version67";

                case HanXinCodeSymbolVersion.Version68:
                    return "Version68";

                case HanXinCodeSymbolVersion.Version69:
                    return "Version69";

                case HanXinCodeSymbolVersion.Version70:
                    return "Version70";

                case HanXinCodeSymbolVersion.Version71:
                    return "Version71";

                case HanXinCodeSymbolVersion.Version72:
                    return "Version72";

                case HanXinCodeSymbolVersion.Version73:
                    return "Version73";

                case HanXinCodeSymbolVersion.Version74:
                    return "Version74";

                case HanXinCodeSymbolVersion.Version75:
                    return "Version75";

                case HanXinCodeSymbolVersion.Version76:
                    return "Version76";

                case HanXinCodeSymbolVersion.Version77:
                    return "Version77";

                case HanXinCodeSymbolVersion.Version78:
                    return "Version78";

                case HanXinCodeSymbolVersion.Version79:
                    return "Version79";

                case HanXinCodeSymbolVersion.Version80:
                    return "Version80";

                case HanXinCodeSymbolVersion.Version81:
                    return "Version81";

                case HanXinCodeSymbolVersion.Version82:
                    return "Version82";

                case HanXinCodeSymbolVersion.Version83:
                    return "Version83";

                case HanXinCodeSymbolVersion.Version84:
                    return "Version84";

                case HanXinCodeSymbolVersion.Undefined:
                    return "Undefined";

            }
            return null;
        }

        #endregion


        #region QRCode

        /// <summary>
        /// Returns a string representation of <see cref="QRErrorCorrectionLevel"/> enum value.
        /// <summary>
        /// <param name="item">Enum value.</param>
        /// <returns>A string representation of enum value.</returns>
        internal static string QRErrorCorrectionLevelToString(QRErrorCorrectionLevel item)
        {
            switch (item)
            {
                case QRErrorCorrectionLevel.L:
                    return "L";

                case QRErrorCorrectionLevel.M:
                    return "M";

                case QRErrorCorrectionLevel.Q:
                    return "Q";

                case QRErrorCorrectionLevel.H:
                    return "H";

            }
            return null;
        }

        /// <summary>
        /// Returns a string representation of <see cref="QRSymbolVersion"/> enum value.
        /// <summary>
        /// <param name="item">Enum value.</param>
        /// <returns>A string representation of enum value.</returns>
        internal static string QRSymbolVersionToString(QRSymbolVersion item)
        {
            switch (item)
            {
                case QRSymbolVersion.Version1:
                    return "Version1";

                case QRSymbolVersion.Version2:
                    return "Version2";

                case QRSymbolVersion.Version3:
                    return "Version3";

                case QRSymbolVersion.Version4:
                    return "Version4";

                case QRSymbolVersion.Version5:
                    return "Version5";

                case QRSymbolVersion.Version6:
                    return "Version6";

                case QRSymbolVersion.Version7:
                    return "Version7";

                case QRSymbolVersion.Version8:
                    return "Version8";

                case QRSymbolVersion.Version9:
                    return "Version9";

                case QRSymbolVersion.Version10:
                    return "Version10";

                case QRSymbolVersion.Version11:
                    return "Version11";

                case QRSymbolVersion.Version12:
                    return "Version12";

                case QRSymbolVersion.Version13:
                    return "Version13";

                case QRSymbolVersion.Version14:
                    return "Version14";

                case QRSymbolVersion.Version15:
                    return "Version15";

                case QRSymbolVersion.Version16:
                    return "Version16";

                case QRSymbolVersion.Version17:
                    return "Version17";

                case QRSymbolVersion.Version18:
                    return "Version18";

                case QRSymbolVersion.Version19:
                    return "Version19";

                case QRSymbolVersion.Version20:
                    return "Version20";

                case QRSymbolVersion.Version21:
                    return "Version21";

                case QRSymbolVersion.Version22:
                    return "Version22";

                case QRSymbolVersion.Version23:
                    return "Version23";

                case QRSymbolVersion.Version24:
                    return "Version24";

                case QRSymbolVersion.Version25:
                    return "Version25";

                case QRSymbolVersion.Version26:
                    return "Version26";

                case QRSymbolVersion.Version27:
                    return "Version27";

                case QRSymbolVersion.Version28:
                    return "Version28";

                case QRSymbolVersion.Version29:
                    return "Version29";

                case QRSymbolVersion.Version30:
                    return "Version30";

                case QRSymbolVersion.Version31:
                    return "Version31";

                case QRSymbolVersion.Version32:
                    return "Version32";

                case QRSymbolVersion.Version33:
                    return "Version33";

                case QRSymbolVersion.Version34:
                    return "Version34";

                case QRSymbolVersion.Version35:
                    return "Version35";

                case QRSymbolVersion.Version36:
                    return "Version36";

                case QRSymbolVersion.Version37:
                    return "Version37";

                case QRSymbolVersion.Version38:
                    return "Version38";

                case QRSymbolVersion.Version39:
                    return "Version39";

                case QRSymbolVersion.Version40:
                    return "Version40";

                case QRSymbolVersion.VersionM1:
                    return "VersionM1";

                case QRSymbolVersion.VersionM2:
                    return "VersionM2";

                case QRSymbolVersion.VersionM3:
                    return "VersionM3";

                case QRSymbolVersion.VersionM4:
                    return "VersionM4";

                case QRSymbolVersion.Model1Version1:
                    return "Model1Version1";

                case QRSymbolVersion.Model1Version2:
                    return "Model1Version2";

                case QRSymbolVersion.Model1Version3:
                    return "Model1Version3";

                case QRSymbolVersion.Model1Version4:
                    return "Model1Version4";

                case QRSymbolVersion.Model1Version5:
                    return "Model1Version5";

                case QRSymbolVersion.Model1Version6:
                    return "Model1Version6";

                case QRSymbolVersion.Model1Version7:
                    return "Model1Version7";

                case QRSymbolVersion.Model1Version8:
                    return "Model1Version8";

                case QRSymbolVersion.Model1Version9:
                    return "Model1Version9";

                case QRSymbolVersion.Model1Version10:
                    return "Model1Version10";

                case QRSymbolVersion.Model1Version11:
                    return "Model1Version11";

                case QRSymbolVersion.Model1Version12:
                    return "Model1Version12";

                case QRSymbolVersion.Model1Version13:
                    return "Model1Version13";

                case QRSymbolVersion.Model1Version14:
                    return "Model1Version14";

                case QRSymbolVersion.Undefined:
                    return "Undefined";

            }
            return null;
        }

        #endregion


        #region PDF417

        /// <summary>
        /// Returns a string representation of <see cref="MicroPDF417SymbolType"/> enum value.
        /// <summary>
        /// <param name="item">Enum value.</param>
        /// <returns>A string representation of enum value.</returns>
        internal static string MicroPDF417SymbolTypeToString(MicroPDF417SymbolType item)
        {
            switch (item)
            {
                case MicroPDF417SymbolType.Undefined:
                    return "Undefined";

                case MicroPDF417SymbolType.Col1Row11:
                    return "Col1Row11";

                case MicroPDF417SymbolType.Col1Row14:
                    return "Col1Row14";

                case MicroPDF417SymbolType.Col1Row17:
                    return "Col1Row17";

                case MicroPDF417SymbolType.Col1Row20:
                    return "Col1Row20";

                case MicroPDF417SymbolType.Col1Row24:
                    return "Col1Row24";

                case MicroPDF417SymbolType.Col1Row28:
                    return "Col1Row28";

                case MicroPDF417SymbolType.Col2Row8:
                    return "Col2Row8";

                case MicroPDF417SymbolType.Col2Row11:
                    return "Col2Row11";

                case MicroPDF417SymbolType.Col2Row14:
                    return "Col2Row14";

                case MicroPDF417SymbolType.Col2Row17:
                    return "Col2Row17";

                case MicroPDF417SymbolType.Col2Row20:
                    return "Col2Row20";

                case MicroPDF417SymbolType.Col2Row23:
                    return "Col2Row23";

                case MicroPDF417SymbolType.Col2Row26:
                    return "Col2Row26";

                case MicroPDF417SymbolType.Col3Row6:
                    return "Col3Row6";

                case MicroPDF417SymbolType.Col3Row8:
                    return "Col3Row8";

                case MicroPDF417SymbolType.Col3Row10:
                    return "Col3Row10";

                case MicroPDF417SymbolType.Col3Row12:
                    return "Col3Row12";

                case MicroPDF417SymbolType.Col3Row15:
                    return "Col3Row15";

                case MicroPDF417SymbolType.Col3Row20:
                    return "Col3Row20";

                case MicroPDF417SymbolType.Col3Row26:
                    return "Col3Row26";

                case MicroPDF417SymbolType.Col3Row32:
                    return "Col3Row32";

                case MicroPDF417SymbolType.Col3Row38:
                    return "Col3Row38";

                case MicroPDF417SymbolType.Col3Row44:
                    return "Col3Row44";

                case MicroPDF417SymbolType.Col4Row4:
                    return "Col4Row4";

                case MicroPDF417SymbolType.Col4Row6:
                    return "Col4Row6";

                case MicroPDF417SymbolType.Col4Row8:
                    return "Col4Row8";

                case MicroPDF417SymbolType.Col4Row10:
                    return "Col4Row10";

                case MicroPDF417SymbolType.Col4Row12:
                    return "Col4Row12";

                case MicroPDF417SymbolType.Col4Row15:
                    return "Col4Row15";

                case MicroPDF417SymbolType.Col4Row20:
                    return "Col4Row20";

                case MicroPDF417SymbolType.Col4Row26:
                    return "Col4Row26";

                case MicroPDF417SymbolType.Col4Row32:
                    return "Col4Row32";

                case MicroPDF417SymbolType.Col4Row38:
                    return "Col4Row38";

                case MicroPDF417SymbolType.Col4Row44:
                    return "Col4Row44";

            }
            return null;
        }

        /// <summary>
        /// Returns a string representation of <see cref="PDF417ErrorCorrectionLevel"/> enum value.
        /// <summary>
        /// <param name="item">Enum value.</param>
        /// <returns>A string representation of enum value.</returns>
        internal static string PDF417ErrorCorrectionLevelToString(PDF417ErrorCorrectionLevel item)
        {
            switch (item)
            {
                case PDF417ErrorCorrectionLevel.Level0:
                    return "Level0";

                case PDF417ErrorCorrectionLevel.Level1:
                    return "Level1";

                case PDF417ErrorCorrectionLevel.Level2:
                    return "Level2";

                case PDF417ErrorCorrectionLevel.Level3:
                    return "Level3";

                case PDF417ErrorCorrectionLevel.Level4:
                    return "Level4";

                case PDF417ErrorCorrectionLevel.Level5:
                    return "Level5";

                case PDF417ErrorCorrectionLevel.Level6:
                    return "Level6";

                case PDF417ErrorCorrectionLevel.Level7:
                    return "Level7";

                case PDF417ErrorCorrectionLevel.Level8:
                    return "Level8";

                case PDF417ErrorCorrectionLevel.Undefined:
                    return "Undefined";

            }
            return null;
        }

        #endregion

        #endregion

        #endregion


        #region PRIVATE

        #region Init

        /// <summary>
        /// Initializes the barcode type arrays and dictionaries.
        /// </summary>
        private static void InitBarcodeTypes()
        {
            // 1D barcodes
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.Code128), BarcodeType.Code128);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.SSCC18.Name.ToString(), BarcodeSymbologySubsets.SSCC18);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.SwissPostParcel.Name.ToString(), BarcodeSymbologySubsets.SwissPostParcel);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.FedExGround96.Name.ToString(), BarcodeSymbologySubsets.FedExGround96);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.VicsBol.Name.ToString(), BarcodeSymbologySubsets.VicsBol);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.VicsScacPro.Name.ToString(), BarcodeSymbologySubsets.VicsScacPro);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.Code16K), BarcodeType.Code16K);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.Code93), BarcodeType.Code93);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.Code39), BarcodeType.Code39);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.Code39Extended.Name.ToString(), BarcodeSymbologySubsets.Code39Extended);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.Code32.Name.ToString(), BarcodeSymbologySubsets.Code32);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.VIN.Name.ToString(), BarcodeSymbologySubsets.VIN);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.PZN.Name.ToString(), BarcodeSymbologySubsets.PZN);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.NumlyNumber.Name.ToString(), BarcodeSymbologySubsets.NumlyNumber);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.DhlAwb.Name.ToString(), BarcodeSymbologySubsets.DhlAwb);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.Code11), BarcodeType.Code11);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.Codabar), BarcodeType.Codabar);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.EAN13), BarcodeType.EAN13);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.EAN13Plus2), BarcodeType.EAN13Plus2);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.EAN13Plus5), BarcodeType.EAN13Plus5);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.JAN13.Name.ToString(), BarcodeSymbologySubsets.JAN13);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.JAN13Plus2.Name.ToString(), BarcodeSymbologySubsets.JAN13Plus2);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.JAN13Plus5.Name.ToString(), BarcodeSymbologySubsets.JAN13Plus5);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.EAN8), BarcodeType.EAN8);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.EAN8Plus2), BarcodeType.EAN8Plus2);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.EAN8Plus5), BarcodeType.EAN8Plus5);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.JAN8.Name.ToString(), BarcodeSymbologySubsets.JAN8);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.JAN8Plus2.Name.ToString(), BarcodeSymbologySubsets.JAN8Plus2);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.JAN8Plus5.Name.ToString(), BarcodeSymbologySubsets.JAN8Plus5);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.EANVelocity.Name.ToString(), BarcodeSymbologySubsets.EANVelocity);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.ISBN.Name.ToString(), BarcodeSymbologySubsets.ISBN);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.ISBNPlus2.Name.ToString(), BarcodeSymbologySubsets.ISBNPlus2);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.ISBNPlus5.Name.ToString(), BarcodeSymbologySubsets.ISBNPlus5);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.ISMN.Name.ToString(), BarcodeSymbologySubsets.ISMN);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.ISMNPlus2.Name.ToString(), BarcodeSymbologySubsets.ISMNPlus2);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.ISMNPlus5.Name.ToString(), BarcodeSymbologySubsets.ISMNPlus5);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.ISSN.Name.ToString(), BarcodeSymbologySubsets.ISSN);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.ISSNPlus2.Name.ToString(), BarcodeSymbologySubsets.ISSNPlus2);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.ISSNPlus5.Name.ToString(), BarcodeSymbologySubsets.ISSNPlus5);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.Interleaved2of5), BarcodeType.Interleaved2of5);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.Interleaved2of5ChecksumISO16390.Name.ToString(), BarcodeSymbologySubsets.Interleaved2of5ChecksumISO16390);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.OPC.Name.ToString(), BarcodeSymbologySubsets.OPC);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.DeutschePostIdentcode.Name.ToString(), BarcodeSymbologySubsets.DeutschePostIdentcode);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.DeutschePostLeitcode.Name.ToString(), BarcodeSymbologySubsets.DeutschePostLeitcode);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.MSI), BarcodeType.MSI);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.PatchCode), BarcodeType.PatchCode);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.Pharmacode), BarcodeType.Pharmacode);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.RSS14), BarcodeType.RSS14);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.RSS14Stacked), BarcodeType.RSS14Stacked);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.RSSLimited), BarcodeType.RSSLimited);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.RSSExpanded), BarcodeType.RSSExpanded);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.RSSExpandedStacked), BarcodeType.RSSExpandedStacked);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.Standard2of5), BarcodeType.Standard2of5);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.IATA2of5), BarcodeType.IATA2of5);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.Matrix2of5), BarcodeType.Matrix2of5);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.Telepen), BarcodeType.Telepen);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.UPCA), BarcodeType.UPCA);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.UPCAPlus2), BarcodeType.UPCAPlus2);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.UPCAPlus5), BarcodeType.UPCAPlus5);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.UPCE), BarcodeType.UPCE);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.UPCEPlus2), BarcodeType.UPCEPlus2);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.UPCEPlus5), BarcodeType.UPCEPlus5);

            SortedBarcode1DTypeNames = new string[BarcodeTypeNameToBarcodeTypes.Count];
            int index = 0;
            foreach (string item in BarcodeTypeNameToBarcodeTypes.Keys)
            {
                SortedBarcode1DTypeNames[index] = item;
                index++;
            }
            Array.Sort(SortedBarcode1DTypeNames);


            // 1D postal barcodes
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.AustralianPost), BarcodeType.AustralianPost);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.IntelligentMail), BarcodeType.IntelligentMail);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.Planet), BarcodeType.Planet);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.Postnet), BarcodeType.Postnet);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.DutchKIX), BarcodeType.DutchKIX);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.RoyalMail), BarcodeType.RoyalMail);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.Mailmark4StateC), BarcodeType.Mailmark4StateC);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.Mailmark4StateL), BarcodeType.Mailmark4StateL);

            SortedBarcode1DPostalTypeNames = new string[]
            {
                BarcodeTypeToString(BarcodeType.AustralianPost),
                BarcodeTypeToString(BarcodeType.IntelligentMail),
                BarcodeTypeToString(BarcodeType.Planet),
                BarcodeTypeToString(BarcodeType.Postnet),
                BarcodeTypeToString(BarcodeType.DutchKIX),
                BarcodeTypeToString(BarcodeType.RoyalMail),
                BarcodeTypeToString(BarcodeType.Mailmark4StateC),
                BarcodeTypeToString(BarcodeType.Mailmark4StateL)
            };
            Array.Sort(SortedBarcode1DPostalTypeNames);


            // 2D barcodes
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.Aztec), BarcodeType.Aztec);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.DataMatrix), BarcodeType.DataMatrix);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.DotCode), BarcodeType.DotCode);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.PDF417), BarcodeType.PDF417);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.MicroPDF417), BarcodeType.MicroPDF417);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.HanXinCode), BarcodeType.HanXinCode);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.QR), BarcodeType.QR);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.MicroQR), BarcodeType.MicroQR);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeTypeToString(BarcodeType.MaxiCode), BarcodeType.MaxiCode);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.XFACompressedAztec.Name.ToString(), BarcodeSymbologySubsets.XFACompressedAztec);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.XFACompressedDataMatrix.Name.ToString(), BarcodeSymbologySubsets.XFACompressedDataMatrix);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.XFACompressedPDF417.Name.ToString(), BarcodeSymbologySubsets.XFACompressedPDF417);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.XFACompressedQRCode.Name.ToString(), BarcodeSymbologySubsets.XFACompressedQRCode);

            SortedBarcode2DTypeNames = new string[]
            {
                BarcodeTypeToString(BarcodeType.Aztec),
                BarcodeTypeToString(BarcodeType.DataMatrix),
                BarcodeTypeToString(BarcodeType.DotCode),
                BarcodeTypeToString(BarcodeType.PDF417),
                BarcodeTypeToString(BarcodeType.MicroPDF417),
                BarcodeTypeToString(BarcodeType.HanXinCode),
                BarcodeTypeToString(BarcodeType.QR),
                BarcodeTypeToString(BarcodeType.MicroQR),
                BarcodeTypeToString(BarcodeType.MaxiCode),

                BarcodeSymbologySubsets.XFACompressedAztec.Name.ToString(),
                BarcodeSymbologySubsets.XFACompressedDataMatrix.Name.ToString(),
                BarcodeSymbologySubsets.XFACompressedPDF417.Name.ToString(),
                BarcodeSymbologySubsets.XFACompressedQRCode.Name.ToString()
            };
            Array.Sort(SortedBarcode2DTypeNames);

            // unsupported
            BarcodeTypeNameToUnsupportedBarcodeTypes.Add(BarcodeSymbologySubsets.MailmarkCmdmType7.Name.ToString(), BarcodeSymbologySubsets.MailmarkCmdmType7);
            BarcodeTypeNameToUnsupportedBarcodeTypes.Add(BarcodeSymbologySubsets.MailmarkCmdmType9.Name.ToString(), BarcodeSymbologySubsets.MailmarkCmdmType9);
            BarcodeTypeNameToUnsupportedBarcodeTypes.Add(BarcodeSymbologySubsets.MailmarkCmdmType29.Name.ToString(), BarcodeSymbologySubsets.MailmarkCmdmType29);
            BarcodeTypeNameToUnsupportedBarcodeTypes.Add(BarcodeSymbologySubsets.PPN.Name.ToString(), BarcodeSymbologySubsets.PPN);

            // GS1 barcodes
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.GS1Aztec.Name.ToString(), BarcodeSymbologySubsets.GS1Aztec);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.GS1DataMatrix.Name.ToString(), BarcodeSymbologySubsets.GS1DataMatrix);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.GS1DotCode.Name.ToString(), BarcodeSymbologySubsets.GS1DotCode);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.GS1QR.Name.ToString(), BarcodeSymbologySubsets.GS1QR);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.GS1DataBar.Name.ToString(), BarcodeSymbologySubsets.GS1DataBar);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.GS1_128.Name.ToString(), BarcodeSymbologySubsets.GS1_128);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.GS1DataBarStacked.Name.ToString(), BarcodeSymbologySubsets.GS1DataBarStacked);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.GS1DataBarLimited.Name.ToString(), BarcodeSymbologySubsets.GS1DataBarLimited);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.GS1DataBarExpanded.Name.ToString(), BarcodeSymbologySubsets.GS1DataBarExpanded);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.GS1DataBarExpandedStacked.Name.ToString(), BarcodeSymbologySubsets.GS1DataBarExpandedStacked);
            BarcodeTypeNameToBarcodeTypes.Add(BarcodeSymbologySubsets.ITF14.Name.ToString(), BarcodeSymbologySubsets.ITF14);

            SortedBarcodeGS1TypeNames = new string[]
            {
                BarcodeSymbologySubsets.GS1Aztec.Name.ToString(),
                BarcodeSymbologySubsets.GS1DataMatrix.Name.ToString(),
                BarcodeSymbologySubsets.GS1DotCode.Name.ToString(),
                BarcodeSymbologySubsets.GS1QR.Name.ToString(),
                BarcodeSymbologySubsets.GS1DataBar.Name.ToString(),
                BarcodeSymbologySubsets.GS1_128.Name.ToString(),
                BarcodeSymbologySubsets.GS1DataBarStacked.Name.ToString(),
                BarcodeSymbologySubsets.GS1DataBarLimited.Name.ToString(),
                BarcodeSymbologySubsets.GS1DataBarExpanded.Name.ToString(),
                BarcodeSymbologySubsets.GS1DataBarExpandedStacked.Name.ToString(),
                BarcodeSymbologySubsets.ITF14.Name.ToString(),
            };
            Array.Sort(SortedBarcode2DTypeNames);

            // popular barcodes
            SortedPopularBarcodeTypeNames = new string[]
            {
                BarcodeTypeToString(BarcodeType.QR),
                BarcodeTypeToString(BarcodeType.DataMatrix),
                BarcodeTypeToString(BarcodeType.PDF417),
                BarcodeTypeToString(BarcodeType.Aztec),
                BarcodeTypeToString(BarcodeType.EAN8),
                BarcodeTypeToString(BarcodeType.EAN13),
                BarcodeTypeToString(BarcodeType.Code39),
                BarcodeTypeToString(BarcodeType.Code128)
            };
        }

        /// <summary>
        /// Initializes the Aztec layers count and Aztec error correction level lists.
        /// </summary>
        private static void InitAztecLists()
        {
            for (int layersCount = 0; layersCount <= 32; layersCount++)
            {
                AztecLayersCount.Add(layersCount);
            }

            for (int errorCorrection = 5; errorCorrection <= 65; errorCorrection += 10)
            {
                AztecErrorCorrectionLevel.Add(errorCorrection);
            }
        }

        /// <summary>
        /// Initializes a DataMatrix symbol size list.
        /// </summary>
        private static void InitDataMatrixLists()
        {
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Undefined);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row10Col10);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row12Col12);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row14Col14);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row16Col16);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row18Col18);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row20Col20);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row22Col22);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row24Col24);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row26Col26);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row32Col32);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row36Col36);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row40Col40);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row44Col44);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row48Col48);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row52Col52);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row64Col64);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row72Col72);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row80Col80);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row88Col88);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row96Col96);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row104Col104);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row120Col120);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row132Col132);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row144Col144);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row8Col18);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row8Col32);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row12Col26);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row12Col36);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row16Col36);
            DataMatrixSymbolSizes.Add(DataMatrixSymbolType.Row16Col48);
        }

        /// <summary>
        /// Initializes the HanXin symbol size and HanXin error correction level lists.
        /// </summary>
        private static void InitHanXinLists()
        {
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Undefined);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version1);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version2);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version3);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version4);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version5);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version6);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version7);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version8);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version9);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version10);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version11);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version12);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version13);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version14);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version15);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version16);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version17);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version18);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version19);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version20);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version21);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version22);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version23);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version24);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version25);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version26);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version27);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version28);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version29);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version30);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version31);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version32);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version33);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version34);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version35);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version36);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version37);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version38);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version39);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version40);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version41);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version42);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version43);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version44);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version45);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version46);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version47);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version48);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version49);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version50);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version51);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version52);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version53);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version54);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version55);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version56);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version57);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version58);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version59);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version60);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version61);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version62);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version63);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version64);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version65);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version66);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version67);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version68);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version69);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version70);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version71);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version72);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version73);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version74);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version75);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version76);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version77);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version78);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version79);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version80);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version81);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version82);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version83);
            HanXinSymbolSizes.Add(HanXinCodeSymbolVersion.Version84);


            HanXinErrorCorrectionLevel.Add(HanXinCodeErrorCorrectionLevel.L1);
            HanXinErrorCorrectionLevel.Add(HanXinCodeErrorCorrectionLevel.L2);
            HanXinErrorCorrectionLevel.Add(HanXinCodeErrorCorrectionLevel.L3);
            HanXinErrorCorrectionLevel.Add(HanXinCodeErrorCorrectionLevel.L4);
        }

        /// <summary>
        /// Initializes the QR symbol size and QR error correction level lists.
        /// </summary>
        private static void InitQRCodeLists()
        {
            QRCodeSymbolSizes.Add(QRSymbolVersion.Undefined);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version1);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version2);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version3);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version4);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version5);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version6);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version7);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version8);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version9);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version10);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version11);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version12);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version13);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version14);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version15);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version16);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version17);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version18);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version19);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version20);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version21);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version22);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version23);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version24);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version25);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version26);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version27);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version28);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version29);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version30);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version31);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version32);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version33);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version34);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version35);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version36);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version37);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version38);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version39);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Version40);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Model1Version1);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Model1Version2);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Model1Version3);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Model1Version4);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Model1Version5);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Model1Version6);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Model1Version7);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Model1Version8);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Model1Version9);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Model1Version10);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Model1Version11);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Model1Version12);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Model1Version13);
            QRCodeSymbolSizes.Add(QRSymbolVersion.Model1Version14);

            QRCodeErrorCorrectionLevel.Add(QRErrorCorrectionLevel.L);
            QRCodeErrorCorrectionLevel.Add(QRErrorCorrectionLevel.M);
            QRCodeErrorCorrectionLevel.Add(QRErrorCorrectionLevel.Q);
            QRCodeErrorCorrectionLevel.Add(QRErrorCorrectionLevel.H);
        }

        /// <summary>
        /// Initializes the MicroQR symbol size and MicroQR error correction level lists.
        /// </summary>
        private static void InitMicroQRLists()
        {
            MicroQRSymbolSizes.Add(QRSymbolVersion.Undefined);
            MicroQRSymbolSizes.Add(QRSymbolVersion.VersionM1);
            MicroQRSymbolSizes.Add(QRSymbolVersion.VersionM2);
            MicroQRSymbolSizes.Add(QRSymbolVersion.VersionM3);
            MicroQRSymbolSizes.Add(QRSymbolVersion.VersionM4);

            MicroQRErrorCorrectionLevel.Add(QRErrorCorrectionLevel.L);
            MicroQRErrorCorrectionLevel.Add(QRErrorCorrectionLevel.M);
            MicroQRErrorCorrectionLevel.Add(QRErrorCorrectionLevel.Q);
        }

        /// <summary>
        /// Initializes the PDF417 rows, columns and error correction level lists.
        /// </summary>
        private static void InitPDF417Lists()
        {
            PDF417Rows.Add(-1);
            for (int i = 1; i <= 90; i++)
            {
                PDF417Rows.Add(i);
            }

            PDF417Columns.Add(-1);
            for (int i = 1; i <= 30; i++)
            {
                PDF417Columns.Add(i);
            }

            PDF417ErrorCorrectionLevelList.Add(PDF417ErrorCorrectionLevel.Undefined);
            PDF417ErrorCorrectionLevelList.Add(PDF417ErrorCorrectionLevel.Level0);
            PDF417ErrorCorrectionLevelList.Add(PDF417ErrorCorrectionLevel.Level1);
            PDF417ErrorCorrectionLevelList.Add(PDF417ErrorCorrectionLevel.Level2);
            PDF417ErrorCorrectionLevelList.Add(PDF417ErrorCorrectionLevel.Level3);
            PDF417ErrorCorrectionLevelList.Add(PDF417ErrorCorrectionLevel.Level4);
            PDF417ErrorCorrectionLevelList.Add(PDF417ErrorCorrectionLevel.Level5);
            PDF417ErrorCorrectionLevelList.Add(PDF417ErrorCorrectionLevel.Level6);
            PDF417ErrorCorrectionLevelList.Add(PDF417ErrorCorrectionLevel.Level7);
            PDF417ErrorCorrectionLevelList.Add(PDF417ErrorCorrectionLevel.Level8);
        }

        /// <summary>
        /// Initializes a MicroPDF417 symbol size list.
        /// </summary>
        private static void InitMicroPDF417Lists()
        {
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Undefined);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col1Row11);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col1Row14);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col1Row17);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col1Row20);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col1Row24);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col1Row28);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col2Row8);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col2Row11);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col2Row14);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col2Row17);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col2Row20);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col2Row23);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col2Row26);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col3Row6);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col3Row8);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col3Row10);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col3Row12);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col3Row15);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col3Row20);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col3Row26);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col3Row32);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col3Row38);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col3Row44);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col4Row4);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col4Row6);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col4Row8);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col4Row10);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col4Row12);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col4Row15);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col4Row20);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col4Row26);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col4Row32);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col4Row38);
            MicroPDF417SimbolSizes.Add(MicroPDF417SymbolType.Col4Row44);
        }

        #endregion

        #endregion

        #endregion

    }
}