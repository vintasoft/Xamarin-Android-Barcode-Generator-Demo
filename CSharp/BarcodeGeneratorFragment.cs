using Android.App;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;

using System;
using System.Collections.Generic;

namespace BarcodeGeneratorDemo
{
    /// <summary>
    /// The barcode generator fragment.
    /// </summary>
    public class BarcodeGeneratorFragment : ListFragment
    {

        #region Nested classes

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
                textView1.Ellipsize = TextUtils.TruncateAt.End;
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

        #endregion



        #region Fields

        /// <summary>
        /// The list of initial values.
        /// </summary>
        List<Utils.BarcodeInformation> _initialValues;

        #endregion



        #region Properties

        int _clickedItemIndex = -1;
        /// <summary>
        /// Last clicked item index.
        /// </summary>
        internal int ClickedItemIndex
        {
            get
            {
                return _clickedItemIndex;
            }
        }

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="values"/> class.
        /// </summary>
        /// <param name="values">A list of initial values.</param>
        internal BarcodeGeneratorFragment(List<Utils.BarcodeInformation> values)
            : base()
        {
            _initialValues = values;
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
            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        /// <summary>
        /// Creates menu.
        /// </summary>
        /// <param name="menu">A menu.</param>
        /// <param name="inflater">An inflater.</param>
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            // create options menu
            inflater.Inflate(Resource.Menu.main_action_bar, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        /// <summary>
        /// Called when the fragment's activity has been created and this fragment's view hierarchy instantiated.
        /// </summary>
        /// <param name="savedInstanceState">The saved instance state if the fragment is being re-created from a previous saved state.</param>
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            // create adapter
            ListAdapter = new BarcodeArrayAdapter(Activity, _initialValues);
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
            switch (item.ItemId)
            {
                // if "About" button is pressed
                case Resource.Id.action_about:
                    ((MainActivity)Activity).ShowInfoDialog(
                        Resources.GetString(Resource.String.app_name),
                        string.Format(Resources.GetString(Resource.String.about_message), System.Reflection.Assembly.GetExecutingAssembly().GetName().Version));
                    return true;

                // if "Add" button is pressed
                case Resource.Id.action_add:
                    _clickedItemIndex = -1;
                    Intent addIntent = new Intent(Activity, typeof(BarcodeTypeSelectorActivity));
                    StartActivity(addIntent);
                    return true;

                // if "Scan" button is pressed
                case Resource.Id.action_scan:
                    _clickedItemIndex = -1;
                    ((MainActivity)Activity).ScanBarcode();
                    return true;

                // if "Delete all" button is pressed
                case Resource.Id.action_delete_all:
                    using (AlertDialog.Builder builder = new AlertDialog.Builder(Activity))
                    {
                        // if barcode list is empty
                        if (ListAdapter.Count == 0)
                        {
                            builder.SetPositiveButton(Resource.String.ok_button, (EventHandler<DialogClickEventArgs>)null);
                            builder.SetTitle(Resource.String.app_name);
                            builder.SetMessage(Resource.String.delete_all_empty_message);
                        }
                        else
                        {
                            builder.SetPositiveButton(Resource.String.delete_button, DeleteAllDialogPositiveButton_Clicked);
                            builder.SetNegativeButton(Resource.String.cancel_button, (EventHandler<DialogClickEventArgs>)null);
                            builder.SetTitle(Resource.String.app_name);
                            builder.SetMessage(Resource.String.delete_all_message);
                        }
                        // create dialog
                        AlertDialog dialog = builder.Create();
                        // show on screen
                        dialog.Show();

                    }
                    
                    return true;

                // if "Settings" button is pressed
                case Resource.Id.action_settings:
                    Intent settingsIntent = new Intent(Activity, typeof(SettingsActivity));
                    StartActivity(settingsIntent);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
        /// This method will be called when an item in the list is selected.
        /// </summary>
        /// <param name="l">The ListView where the click happened.</param>
        /// <param name="v">The view that was clicked within the ListView.</param>
        /// <param name="position">The position of the view in the list.</param>
        /// <param name="id">The row id of the item that was clicked.</param>
        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            base.OnListItemClick(l, v, position, id);

            // save clicked position
            _clickedItemIndex = position;
            // get clicked item
            Utils.BarcodeInformation item = ((BarcodeArrayAdapter)ListAdapter).GetItem(position);

            ((MainActivity)Activity).SwitchToBarcodeViewer(Activity, item);
        }

        #endregion


        #region INTERNAL

        /// <summary>
        /// Sets the last item index of barcode list to the clicked item index.
        /// </summary>
        internal void SetLastItemIndexToClickedItemIndex()
        {
            _clickedItemIndex = ListAdapter.Count - 1;
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// "Delete all" button is clicked.
        /// </summary>
        private void DeleteAllDialogPositiveButton_Clicked(object sender, DialogClickEventArgs args)
        {
            // get adapter
            ArrayAdapter<Utils.BarcodeInformation> adapter = ListAdapter as ArrayAdapter<Utils.BarcodeInformation>;
            // clear all data
            adapter.Clear();
            adapter.NotifyDataSetChanged();
            // delete saved history
            ((MainActivity)Activity).DeleteSavedHistory();
        }

        #endregion

        #endregion

    }
}