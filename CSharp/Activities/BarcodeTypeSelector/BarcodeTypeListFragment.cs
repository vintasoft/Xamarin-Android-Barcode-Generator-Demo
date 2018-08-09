using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using System;
using System.Collections.Generic;

namespace BarcodeGeneratorDemo
{
    /// <summary>
    /// A list fragment that allows to choose barcode type.
    /// </summary>
    public class BarcodeTypeListFragment : ListFragment
    {

        #region Enum

        /// <summary>
        /// Specifies available barcode categories.
        /// </summary>
        private enum BarcodeCategories
        {
            Popular = 0,
            GS1Barcodes = 1,
            TwoDimential = 2,
            OneDimential = 3,
            OneDimentialPostal = 4
        }

        #endregion



        #region Fields

        /// <summary>
        /// A list values.
        /// </summary>
        IList<string> _values;

        /// <summary>
        /// Determines whether parent is fragment.
        /// </summary>
        bool _isParentFragment = false;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="BarcodeTypeListFragment"/> class.
        /// </summary>
        public BarcodeTypeListFragment()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BarcodeTypeListFragment"/> class.
        /// </summary>
        public BarcodeTypeListFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BarcodeTypeListFragment"/> class.
        /// </summary>
        /// <param name="values">A values list.</param>
        /// <param name="isParentFragment">A value that determines whether parent is fragment.</param>
        internal BarcodeTypeListFragment(IList<string> values, bool isParentFragment)
            : base()
        {
            _values = values;
            _isParentFragment = isParentFragment;
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
        }

        /// <summary>
        /// Called to have the fragment instantiate its user interface view.
        /// </summary>
        /// <param name="inflater">The <see cref="LayoutInflater"/> object that can be used to inflate any view in the fragment.</param>
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
        /// Called when the fragment's activity has been created and this fragment's view hierarchy instantiated.
        /// </summary>
        /// <param name="savedInstanceState">
        /// If the fragment is being re-created from a previous saved state, this is the state.
        /// </param>
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            // create adapter
            ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, _values);
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
            // if parent is not fragment
            if (!_isParentFragment)
            {
                // switch to the next list
                SwitchToNextList(position);
            }
            else
            {
                // switch to the barcode editor
                SwitchToBarcodeEditorFragment((string)ListAdapter.GetItem(position));
            }
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Switches to the barcode editor fragment.
        /// </summary>
        /// <param name="barcodeTypeString">A string representation of barcode type.</param>
        private void SwitchToBarcodeEditorFragment(string barcodeTypeString)
        {
            // create a new transaction
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            // add fragment to the container
            transaction.Replace(Resource.Id.mainContentFrame, new BarcodeEditorFragment(barcodeTypeString));
            transaction.AddToBackStack(null);
            // commit the transaction
            transaction.Commit();
        }

        /// <summary>
        /// Switches to the next list fragment.
        /// </summary>
        /// <param name="index">A list index.</param>
        private void SwitchToNextList(int index)
        {
            string[] barcodeTypeList = null;
            switch ((BarcodeCategories)index)
            {
                case BarcodeCategories.Popular:
                    barcodeTypeList = Utils.SortedPopularBarcodeTypeNames;
                    break;
                case BarcodeCategories.GS1Barcodes:
                    barcodeTypeList = Utils.SortedBarcodeGS1TypeNames;
                    break;
                case BarcodeCategories.TwoDimential:
                    barcodeTypeList = Utils.SortedBarcode2DTypeNames;
                    break;
                case BarcodeCategories.OneDimential:
                    barcodeTypeList = Utils.SortedBarcode1DTypeNames;
                    break;
                case BarcodeCategories.OneDimentialPostal:
                    barcodeTypeList = Utils.SortedBarcode1DPostalTypeNames;
                    break;
            }

            // create a new transaction
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            // add fragment to the container
            transaction.Replace(Resource.Id.mainContentFrame, new BarcodeTypeListFragment(barcodeTypeList, true));
            transaction.AddToBackStack(null);
            // commit the transaction
            transaction.Commit();
        }

        #endregion

        #endregion

    }
}