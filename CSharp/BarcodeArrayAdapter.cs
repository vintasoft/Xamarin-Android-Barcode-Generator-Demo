using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace BarcodeGeneratorDemo
{
    /// <summary>
    /// The array adapter to show barcode info in double string list view.
    /// </summary>
    internal class BarcodeArrayAdapter : ArrayAdapter<Utils.BarcodeInformation>
    {

        #region Fields

        /// <summary>
        /// The resource layout id.
        /// </summary>
        int _resource;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="BarcodeArrayAdapter"/> class.
        /// </summary>
        /// <param name="context">A constext.</param>
        /// <param name="values">A values list.</param>
        internal BarcodeArrayAdapter(Context context, IList<Utils.BarcodeInformation> values)
            : base(context, Android.Resource.Layout.SimpleListItem2, values)
        {
            _resource = Android.Resource.Layout.SimpleListItem2;
        }

        #endregion



        #region Methods

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
                view = LayoutInflater.From(Context).Inflate(_resource, parent, false);

            // get the first text view
            TextView textView1 = view.FindViewById<TextView>(Android.Resource.Id.Text1);
            // set single line property
            textView1.SetSingleLine(true);
            // set max length of line
            textView1.SetMaxEms(20);
            // set ending of the line
            textView1.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
            // get the second text view
            TextView textView2 = view.FindViewById<TextView>(Android.Resource.Id.Text2);

            // get item by position
            Utils.BarcodeInformation item = GetItem(position);

            // format string
            string format;
            if (item.BarcodeDescription == null || item.BarcodeDescription == "")
                format = "{0}";
            else
                format = "{0} ({1})";

            // barcode type string
            string barcodeType;
            if (item.BarcodeSubsetName == null || item.BarcodeSubsetName == "")
                barcodeType = Utils.BarcodeTypeToString(item.BarcodeWriterSetting.Barcode);
            else
                barcodeType = item.BarcodeSubsetName;

            textView1.Text = string.Format(format, barcodeType, item.BarcodeDescription);
            textView2.Text = item.BarcodeValue.ToString();

            return view;
        }

        #endregion

    }
}