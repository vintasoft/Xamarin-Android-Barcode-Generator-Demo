using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Text;
using Android.Util;
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

        #region Fields

        /// <summary>
        /// The list of initial values.
        /// </summary>
        List<Utils.BarcodeInformation> _initialValues;

        /// <summary>
        /// The empty list message text view.
        /// </summary>
        TextView _emptyMessageTextView;

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
        /// Initializes a new instance of <see cref="BarcodeGeneratorFragment"/> class.
        /// </summary>
        public BarcodeGeneratorFragment()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="BarcodeGeneratorFragment"/> class.
        /// </summary>
        protected BarcodeGeneratorFragment(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer)
            : base(javaReference, transfer)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="BarcodeGeneratorFragment"/> class.
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
            View view = base.OnCreateView(inflater, container, savedInstanceState);

            _emptyMessageTextView = new TextView(Activity);
            _emptyMessageTextView.Text = Resources.GetString(Resource.String.empty_list_message);
            _emptyMessageTextView.SetTextSize(ComplexUnitType.Sp, 20);
            _emptyMessageTextView.Gravity = GravityFlags.Center;
            container.AddView(_emptyMessageTextView);

            return view;
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
            if (_initialValues == null)
                _initialValues = new List<Utils.BarcodeInformation>();
            ListAdapter = new BarcodeArrayAdapter(Activity, _initialValues);
        }

        /// <summary>
        /// Called when the Fragment is visible to the user and actively running.
        /// </summary>
        public override void OnResume()
        {
            base.OnResume();
            if (ListAdapter.Count == 0)
            {
                _emptyMessageTextView.Visibility = ViewStates.Visible;
            }
            else
            {
                _emptyMessageTextView.Visibility = ViewStates.Invisible;
            }
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

            _emptyMessageTextView.Visibility = ViewStates.Visible;
        }

        #endregion

        #endregion

    }
}