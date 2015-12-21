
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ProteinHelper
{
	[Activity (Label = "Query Inventory")]			
	public class QueryActivity : Activity
	{
		ListView queryListView;
		InventoryAdapter queryAdapter;
		ToggleButton prtnTggleBtn;
		ToggleButton calTggleBtn;
		EditText minPrtnQueryTxt;
		EditText maxPrtnQueryTxt;
		EditText minCalQueryTxt;
		EditText maxCalQueryTxt;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Query);

			//Refresh our cache before populating listview
			Inventory.InventoryService.RefreshCache ();

			//Set up our listview for fooditems
			queryListView = FindViewById<ListView> (Resource.Id.queryListView);
			queryAdapter = new InventoryAdapter (this);
			queryListView.Adapter = queryAdapter;

			//Set up our buttons and input fields
			prtnTggleBtn = FindViewById<ToggleButton>(Resource.Id.prtnToggleBtn);
			calTggleBtn = FindViewById<ToggleButton> (Resource.Id.calToggleBtn);

			minPrtnQueryTxt = FindViewById<EditText> (Resource.Id.minPrtnQueryTxt);
			maxPrtnQueryTxt = FindViewById<EditText> (Resource.Id.maxPrtnQueryTxt);
			minCalQueryTxt = FindViewById<EditText> (Resource.Id.minCalQueryTxt);
			maxCalQueryTxt = FindViewById<EditText> (Resource.Id.maxCalQueryTxt);

			//Start with our toggle buttons disabled

			//Lets filter if the user clicks a toggle button or changes text
			prtnTggleBtn.Click += (object sender, EventArgs e) => {
				Filter();	
			};
			calTggleBtn.Click += (object sender, EventArgs e) => {
				Filter();
			};

			//Anytime the text changes, lets update our UI
			minPrtnQueryTxt.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => {
				setToggleButtons();
				Filter();
			}; 
			maxPrtnQueryTxt.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => {
				setToggleButtons();
				Filter();
			}; 
			minCalQueryTxt.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => {
				setToggleButtons();
				Filter();
			}; 
			maxCalQueryTxt.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => {
				setToggleButtons();
				Filter();
			}; 

		}

		protected override void OnResume()
		{
			base.OnResume ();

			Inventory.InventoryService.RefreshCache ();

			queryAdapter.NotifyDataSetChanged ();
		}
		protected void Filter()
		{
			bool minPrtnValid = !String.IsNullOrEmpty(minPrtnQueryTxt.Text);
			bool maxPrtnValid = !String.IsNullOrEmpty(maxPrtnQueryTxt.Text);
			bool minCalValid = !String.IsNullOrEmpty (minCalQueryTxt.Text);
			bool maxCalValid = !String.IsNullOrEmpty (maxCalQueryTxt.Text);
			bool allValid = minPrtnValid && maxPrtnValid && minCalValid && maxCalValid;

			//Only the protein filter is turned on
			if (prtnTggleBtn.Checked && !calTggleBtn.Checked) 
			{
				if (minPrtnValid&&maxPrtnValid) 
					queryAdapter.filterOnProtein (Convert.ToDouble (minPrtnQueryTxt.Text), Convert.ToDouble(maxPrtnQueryTxt.Text));
			} 
			//Only the calories filter is turned on
			else if (!prtnTggleBtn.Checked && calTggleBtn.Checked) 
			{
				if(minCalValid&&maxCalValid)
					queryAdapter.filterOnCalories (Convert.ToDouble (minCalQueryTxt.Text), Convert.ToDouble(maxCalQueryTxt.Text));
			}
			//Both filters are turned on
			else if (prtnTggleBtn.Checked && calTggleBtn.Checked) 
			{
				if (allValid)
					queryAdapter.filterOnCalAndProtein (Convert.ToDouble (minPrtnQueryTxt.Text), Convert.ToDouble (maxPrtnQueryTxt.Text),
						Convert.ToDouble(minCalQueryTxt.Text), Convert.ToDouble(maxCalQueryTxt.Text));
			}

			//Both filters are turned off
			else 
			{
				RefreshInventory ();
			}
		}

		//Refreshes the inventory in the InventoryAdapter
		//Basically clears any query 
		protected void RefreshInventory()
		{
			queryAdapter.resetInventory ();
		}

		protected void setToggleButtons()
		{
			bool prtnNotNull = !String.IsNullOrEmpty (minPrtnQueryTxt.Text) && !String.IsNullOrEmpty (maxPrtnQueryTxt.Text);
			bool calNotNull = !String.IsNullOrEmpty (minCalQueryTxt.Text) && !String.IsNullOrEmpty (maxCalQueryTxt.Text);



			if (prtnNotNull) {
				if (Convert.ToInt16 (minPrtnQueryTxt.Text) <= Convert.ToInt16 (maxPrtnQueryTxt.Text)) {
					minPrtnQueryTxt.Error = null;
					prtnTggleBtn.Enabled = true;
				} else {
					minPrtnQueryTxt.Error = "Must be less than max";
					prtnTggleBtn.Enabled = false;
				}
			}
			else {
				prtnTggleBtn.Enabled = false;
				RefreshInventory ();
			}
			if (calNotNull) {
				if (Convert.ToInt16 (minCalQueryTxt.Text) <= Convert.ToInt16 (maxCalQueryTxt.Text)) {
					minCalQueryTxt.Error = null;
					calTggleBtn.Enabled = true;
				} else {
					minCalQueryTxt.Error = "Must be less than max";
					calTggleBtn.Enabled = false;
				}
			}
			else {
				calTggleBtn.Enabled = false;
				RefreshInventory ();
			}
		}
	}
}

