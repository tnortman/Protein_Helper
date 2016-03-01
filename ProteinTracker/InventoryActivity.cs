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
using Android;
using Nutritionix;
using Android.Text;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using Android.Graphics;

using Runnable = Java.Lang.Runnable;
using Android.Animation;
using Android.Views.Animations;

namespace ProteinHelper
{
	[Activity (Label = "My Inventory")]			
	public class InventoryActivity : Activity
	{
		ListView inventoryListView;
		InventoryAdapter inventoryAdapter;
		MyTouchListener swipeDetector;
		TextView emptyView;
		//ProgressDialog nutriProgDialog;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Inventory);

			//Refresh cache of foodItems before we layout with adapter
			Task refreshCache = Task.Run(()=> Inventory.InventoryService.RefreshCache());
			refreshCache.Wait ();

			swipeDetector = new MyTouchListener ();

			inventoryListView = FindViewById<ListView> (Resource.Id.inventoryListView);
			emptyView = FindViewById<TextView> (Resource.Id.empty);

			inventoryAdapter = new InventoryAdapter (this);
			inventoryListView.Adapter = inventoryAdapter;
			inventoryListView.EmptyView = emptyView;


			inventoryListView.SetOnTouchListener (swipeDetector);

			inventoryListView.ItemClick += FoodItemClicked;
			inventoryListView.ItemLongClick += FoodItemClicked;
			 
		}

		//Creates the menu options for the InventoryListView Activity
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.InventoryListViewMenu, menu);
			return base.OnCreateOptionsMenu (menu);
		}

		//Handles when the user clicks on a menu option for the InventoryListView Activity
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId) 
			{
				case Resource.Id.scan:
					ScanBarcode ();
					return true;
				case Resource.Id.addCustom:
					StartActivity (typeof(FoodItemDetail));
					return true;
				case Resource.Id.refresh:
					RefreshCache ();
					return true;
				default:
					return base.OnOptionsItemSelected (item);
			}
		}

		/**
		 * ScanBarcode - Scans a barcode and adds new item to inventory
		 * */
		protected async void ScanBarcode()
		{
			var scanner = new ZXing.Mobile.MobileBarcodeScanner();
			scanner.TopText = "Center barcode on scanning line";
			scanner.CancelButtonText = "Cancel";
			var options = new ZXing.Mobile.MobileBarcodeScanningOptions ();

			//We don't need to worry about qr codes or any other barcode formats besides UPC_A (and mayber UPC_E)
			options.PossibleFormats = new List<ZXing.BarcodeFormat> (){ 
				ZXing.BarcodeFormat.UPC_A,
				ZXing.BarcodeFormat.UPC_E
			};
			var result = await scanner.Scan (options);

			if (result != null) {

				Console.WriteLine ("Scanned Barcode: " + result.Text);
				NutritionixService nutService = new NutritionixService ();
				Item foodItem = nutService.SearchByUPC (result.Text);
				if (foodItem != null) {
					AddScaned (foodItem);
				}
			} else {
				Toast.MakeText (this, "No items found.", ToastLength.Short).Show ();
			}
			
		}

		protected void ConfirmedAddScaned(Item foodItem)
		{
			//Add new FoodItem to Inventory
			bool generateFakeUPC = false;
			long upc;
			//For cases where the item doesnt contain a UPC...it's happened
			if (foodItem.UPC == null) {
				upc = 0;
				generateFakeUPC = true;
			}
			else
				upc = Convert.ToInt64(foodItem.UPC);

			string brandName = foodItem.BrandName;
			string tempName = foodItem.Name;
			double tempProtein = (double)foodItem.NutritionFact_Protein;
			double tempCal = (double)foodItem.NutritionFact_Calories;

			FoodItem foodItemToAdd = new FoodItem(upc,brandName, tempName, tempProtein, tempCal);
			if (generateFakeUPC)
				foodItemToAdd.GenerateUPC ();
			Inventory.InventoryService.insertUpdateFoodItem(foodItemToAdd);

			RefreshCache ();

			Toast.MakeText (this, tempName + " added to inventory", ToastLength.Short).Show();
		}

		protected void AddScaned(Item foodItem)
		{
			AlertDialog.Builder alert = new AlertDialog.Builder (this);
			alert.SetCancelable (false);

			long scannedUPC = Convert.ToInt64 (foodItem.UPC);

			//If we find a UPC or product name that already exists in the inventory
			if (Inventory.InventoryService.matchingUPC (scannedUPC) ||
			   Inventory.InventoryService.matchingFoodItemByProductName (foodItem.Name)) 
			{
				alert.SetTitle ("Item found");
				alert.SetNeutralButton ("OK", delegate {});
				alert.SetMessage (String.Format("Your inventory already contains {0} item", foodItem.Name));
				alert.Show ();
			}

			//If we dont find any matches in the inventory, we can add it
			else
			{
				alert.SetTitle ("Item found");
				alert.SetPositiveButton ("Add", (senderAlert, args)=>{
					ConfirmedAddScaned(foodItem);
				});
				alert.SetNegativeButton ("Cancel", delegate {});
				alert.SetMessage (String.Format("Product: {0}\nBrand Name: {1}", foodItem.Name, foodItem.BrandName));
				alert.Show ();
			}
		}

		protected void FoodItemClicked(object sender, ListView.ItemClickEventArgs e)
		{
			//If we have a swipe of some sort
			if (swipeDetector.swipeDetected ())
			{
				//Logic for swiping left to right goes here
				if (swipeDetector.getAction () == MyTouchListener.Action.LR) {
					//Swipe left to right logic	
				}
				//Logic for swiping right to left goes here
				if (swipeDetector.getAction () == MyTouchListener.Action.RL) {
					int indexToBeDeleted = e.Position - inventoryListView.FirstVisiblePosition;

					//Toast.MakeText (this, "RL: " + e.Id.ToString (), ToastLength.Short).Show ();

					//Get screen size
					Display display = WindowManager.DefaultDisplay;
					Point point = new Point ();
					display.GetSize (point);

					inventoryListView.GetChildAt(indexToBeDeleted).ClearAnimation();
					TranslateAnimation translateAnim = new TranslateAnimation (0, -point.X, 0, 0);
					Animation fade = new AlphaAnimation (1.0f, 0.0f);
					AnimationSet deleteAnimation = new AnimationSet (false);
					deleteAnimation.AddAnimation (fade);
					deleteAnimation.AddAnimation (translateAnim);
					deleteAnimation.Duration = 250;
					deleteAnimation.SetAnimationListener(new MyAnimationListener(inventoryAdapter, e.Position));

					inventoryListView.GetChildAt (indexToBeDeleted).StartAnimation (deleteAnimation);

					//The actual delete happens in the animation listener, it waits for the animation to be done b4 deleting
					Toast.MakeText(this, Inventory.InventoryService.GetFoodItemByUPC(e.Id).productName + " Deleted" ,ToastLength.Short).Show();
				}

			} 
			//else we send user to the detail food item screen
			else {
				
				Intent foodItemDetailIntent = new Intent (this, typeof(FoodItemDetail));
				//long UPC = Inventory.InventoryService.GetFoodItemByPosition ((int)e.Id).UPC.Value;
				foodItemDetailIntent.PutExtra ("UPC", e.Id);
				StartActivity (foodItemDetailIntent);
			}
		}

		protected void FoodItemClicked(object sender, ListView.ItemLongClickEventArgs e)
		{
			//If we have a swipe of some sort
			if (swipeDetector.swipeDetected ())
			{
				//Logic for swiping left to right goes here
				if (swipeDetector.getAction () == MyTouchListener.Action.LR) {
					//Swipe left to right logic	
				}
				//Logic for swiping right to left goes here
				if (swipeDetector.getAction () == MyTouchListener.Action.RL) {
					int indexToBeDeleted = e.Position - inventoryListView.FirstVisiblePosition;

					//Toast.MakeText (this, "RL: " + e.Id.ToString (), ToastLength.Short).Show ();

					//Get screen size
					Display display = WindowManager.DefaultDisplay;
					Point point = new Point ();
					display.GetSize (point);

					inventoryListView.GetChildAt(indexToBeDeleted).ClearAnimation();
					TranslateAnimation translateAnim = new TranslateAnimation (0, -point.X, 0, 0);
					Animation fade = new AlphaAnimation (1.0f, 0.0f);
					AnimationSet deleteAnimation = new AnimationSet (false);
					deleteAnimation.AddAnimation (fade);
					deleteAnimation.AddAnimation (translateAnim);
					deleteAnimation.Duration = 250;
					deleteAnimation.SetAnimationListener(new MyAnimationListener(inventoryAdapter, e.Position));


					inventoryListView.GetChildAt (indexToBeDeleted).StartAnimation (deleteAnimation);


					//The actual delete happens in the animation listener, it waits for the animation to be done b4 deleting}
					Toast.MakeText(this, Inventory.InventoryService.GetFoodItemByUPC(e.Id).productName + " Deleted" ,ToastLength.Short).Show();
				} 
				//else we send user to the detail food item screen
				else{
					Intent foodItemDetailIntent = new Intent (this, typeof(FoodItemDetail));
					//long UPC = Inventory.InventoryService.GetFoodItemByPosition ((int)e.Id).UPC.Value;
					foodItemDetailIntent.PutExtra ("UPC", e.Id);
					StartActivity (foodItemDetailIntent);
				}
			}
		}


		protected override void OnResume()
		{
			base.OnResume ();
			RefreshCache ();
		}
			
		protected void RefreshCache()
		{
			//Makes sure to refresh cache and wait b4 updating, else we will get an empty cache or previous version
			Task refreshCache = Task.Run(()=>Inventory.InventoryService.RefreshCache ());
			refreshCache.Wait ();
			inventoryAdapter.NotifyDataSetChanged ();
		}
	}

	public class MyAnimationListener: Java.Lang.Object, Android.Views.Animations.Animation.IAnimationListener
	{
		InventoryAdapter inventoryAdapter;
		int position;
		public MyAnimationListener(InventoryAdapter inventoryAdapter, int position)
		{
			this.inventoryAdapter = inventoryAdapter;
			this.position = position;
		}

		public void OnAnimationEnd (Animation animation)
		{
			FoodItem toBeDeleted = Inventory.InventoryService.GetFoodItemByPosition (position);
			Inventory.InventoryService.DeleteFood (toBeDeleted);
			inventoryAdapter.deleteFoodItem (position);
			inventoryAdapter.NotifyDataSetChanged ();
		}

		public void OnAnimationRepeat (Animation animation)
		{
			
		}

		public void OnAnimationStart (Animation animation)
		{
			
		}
	}
}



