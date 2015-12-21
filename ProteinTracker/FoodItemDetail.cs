
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
using Android.Graphics;
using System.Net;

namespace ProteinHelper
{
	[Activity (Label = "Edit Food")]			
	public class FoodItemDetail : Activity
	{
		FoodItem currentFoodItem;
		EditText brndNmEdtTxt;
		EditText prdNmEdtTxt;
		EditText prtnEdtTxt;
		EditText calEdtTxt;



		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.FoodItemDetail);

			brndNmEdtTxt = FindViewById<EditText> (Resource.Id.brndNameEditTxt);
			prdNmEdtTxt = FindViewById<EditText> (Resource.Id.prdNameEditTxt);
			prtnEdtTxt = FindViewById<EditText> (Resource.Id.prtnEditTxt);
			calEdtTxt = FindViewById<EditText> (Resource.Id.calEditTxt);

			if (Intent.HasExtra ("UPC")) {
				long UPC = Intent.GetLongExtra ("UPC", -1);
				currentFoodItem = Inventory.InventoryService.GetFoodItemByUPC (UPC);

			} else {
				currentFoodItem = new FoodItem ();
			}

			UpdateUI ();

		}

		protected void UpdateUI()
		{
			brndNmEdtTxt.Text = currentFoodItem.brandName;
			prdNmEdtTxt.Text = currentFoodItem.productName;
			prtnEdtTxt.Text = currentFoodItem.protein.ToString();
			calEdtTxt.Text = currentFoodItem.caloriesPerServing.ToString();

		}

		public override bool OnPrepareOptionsMenu(IMenu menu)
		{
			base.OnPrepareOptionsMenu (menu);
			//Disabling delete for a new custom food item
			if (currentFoodItem.UPC == null) {
				IMenuItem delete = menu.FindItem (Resource.Id.delete);
				delete.SetEnabled (false);
			}
			return true;
		}

		//Creates the menu options for the InventoryListView Activity
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.FoodItemDetailMenu, menu);
			return base.OnCreateOptionsMenu (menu);
		}

		//Handles when the user clicks on a menu option for the InventoryListView Activity
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId) 
			{
			case Resource.Id.save:
				SaveFoodItem ();
				return true;
			case Resource.Id.delete:
				DeleteFoodItem ();
				return true;
			default:
				return base.OnOptionsItemSelected (item);
			}
		}

		protected void SaveFoodItem()
		{
			bool validInput = true;

			//Validate BrandName field
			if (String.IsNullOrEmpty (brndNmEdtTxt.Text)) {
				brndNmEdtTxt.Error = "Enter Brand Name";
				validInput = false;
			} else
				brndNmEdtTxt.Error = null;

			//Validate Product Name field
			if (String.IsNullOrEmpty (prdNmEdtTxt.Text)) {
				prdNmEdtTxt.Error = "Enter Product Name";
				validInput = false;
			} else
				prdNmEdtTxt.Error = null;

			double? tempProtein = null;
			//Validate Protein Field
			if (String.IsNullOrEmpty (prtnEdtTxt.Text)) {
				try{
					tempProtein = Double.Parse(prtnEdtTxt.Text);
					if(tempProtein<0)
					{
						prtnEdtTxt.Error = "Must be a positive number";
						validInput = false;
					}
					else
						prtnEdtTxt.Error = null;
				}
				catch{
					prtnEdtTxt.Error = "Please enter a valid number (can be decimal).";
					validInput = false;
				}
			}

			//Validate Caloires Field
			double? tempCalories = null;
			if (String.IsNullOrEmpty (calEdtTxt.Text)) {
				try{
					tempCalories = Double.Parse(calEdtTxt.Text);
					if(tempCalories<0)
					{
						calEdtTxt.Error = "Must be a positive number";
						validInput = false;
					}
					else
						calEdtTxt.Error = null;
				}
				catch{
					calEdtTxt.Error = "Please enter a valid number (can be decimal).";
					validInput = false;
				}
			}

			if (validInput) {
				currentFoodItem.brandName = brndNmEdtTxt.Text;
				currentFoodItem.productName = prdNmEdtTxt.Text;
				currentFoodItem.protein = Convert.ToDouble (prtnEdtTxt.Text);
				currentFoodItem.caloriesPerServing = Convert.ToDouble (calEdtTxt.Text);

				bool newFoodItem = false;

				if (currentFoodItem.UPC == null) {
					currentFoodItem.GenerateUPC ();
					newFoodItem = true;

				}

				 Inventory.InventoryService.insertUpdateFoodItem (currentFoodItem);

				if (newFoodItem)
					Toast.MakeText (this, String.Format ("{0} added to inventory.", currentFoodItem.productName), ToastLength.Short).Show ();
				else
					Toast.MakeText (this, String.Format ("{0} updated.", currentFoodItem.productName), ToastLength.Short).Show ();

				Finish ();
			}

		}

	

		protected void DeleteFoodItem()
		{
			AlertDialog.Builder deleteConfirm = new AlertDialog.Builder (this);
			deleteConfirm.SetCancelable (false);
			deleteConfirm.SetPositiveButton ("Delete", ConfirmedDelete);
			deleteConfirm.SetNegativeButton ("Cancel", delegate {});
			deleteConfirm.SetMessage (String.Format ("Delete {0} from inventory?", currentFoodItem.productName));
			deleteConfirm.Show ();
		}

		protected void ConfirmedDelete(object sender, EventArgs e)
		{
			Inventory.InventoryService.DeleteFood (currentFoodItem);
			Toast.MakeText (this, String.Format("{0} deleted from inventory", currentFoodItem.productName), ToastLength.Short).Show ();
			Finish ();
		}

		private Bitmap GetImageBitmapFromUrl(string url)
		{
			Bitmap imageBitmap = null;

			using (var webClient = new WebClient())
			{
				var imageBytes = webClient.DownloadData(url);
				if (imageBytes != null && imageBytes.Length > 0)
				{
					imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
				}
			}

			return imageBitmap;
		}


	}
}

