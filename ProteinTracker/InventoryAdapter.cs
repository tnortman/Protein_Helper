using System;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;

using System.Collections.Generic;
using Android.Widget;
using Android.App;
using Android.Views;
using System.Collections;
using Android.Graphics;
using System.IO;
using Android.Graphics.Drawables;

namespace ProteinHelper
{
	public class InventoryAdapter : BaseAdapter<FoodItem>
	{
		
		private List<FoodItem> orig_inventory;
		private List<FoodItem> inventory;
		Activity context;

		 
		public InventoryAdapter (Activity context)
		{
			this.context = context;

			orig_inventory = Inventory.InventoryService.inventory;
			inventory = Inventory.InventoryService.inventory;
		}
			
		#region implemented abstract members of BaseAdapter
		public override long GetItemId (int position)
		{
			return inventory[position].UPC.Value;
		}

		public override View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			View view = convertView; //re-use an existing view, if one is available
			if(view == null) //otherwise create a new one
				view = context.LayoutInflater.Inflate(Resource.Layout.InventoryItem, null);

			FoodItem foodItem = inventory [position];

			view.FindViewById<TextView> (Resource.Id.proteinTxt).Text = foodItem.protein.ToString ()+"g";
			view.FindViewById<TextView> (Resource.Id.nameTxt).Text = foodItem.productName;
			view.FindViewById<TextView> (Resource.Id.calTxt).Text = foodItem.caloriesPerServing.ToString () + " calories per serving";
			int maxPrtn = Inventory.InventoryService.GetMaxProteinInInventory();
			view.FindViewById<ImageView> (Resource.Id.prtnIndicator).SetImageBitmap(GetProteinIndicatorImg ((int)foodItem.protein, 0, maxPrtn, 0, 5));

			return view;
		}

		public override int Count {
			get {
				return inventory.Count;
			}
		}
		#endregion

		#region implemented abstract members of BaseAdapter
		public override FoodItem this [int index] {
			get {
				return inventory[index];
			}
		}
		#endregion

		public void deleteFoodItem(int position)
		{
			inventory.RemoveAt (position);
		}

		public void filterOnProtein(double minPrtnSrchAmnt, double maxPrtnSrchAmnt)
		{
			//Creates new empty list that will hold our filterd items
			List<FoodItem> filtered = new List<FoodItem> ();

			foreach (FoodItem f in this.orig_inventory) {
				double protein = f.protein;
				if ((minPrtnSrchAmnt<= protein) && (protein <=maxPrtnSrchAmnt)) {
					filtered.Add (f);
				}
			}

			//set new (filtered) current list of fooditems
			inventory = filtered;

			//Notify ListView to Rebuild
			NotifyDataSetChanged();
		}

		public void filterOnCalories(double minCalSrchAmnt, double maxCalSrchAmnt)
		{
			List<FoodItem> filterd = new List<FoodItem> ();
			foreach (FoodItem f in orig_inventory) {
				double calories = f.caloriesPerServing;
				if ((minCalSrchAmnt<= calories)&&(calories <= maxCalSrchAmnt))
					filterd.Add (f);
			}
			inventory = filterd;
			NotifyDataSetChanged ();
		}

		public void filterOnCalAndProtein(double minPrtnSrchAmnt, double maxPrtnSrchAmnt,
											double minCalSrchAmnt, double maxCalSrchAmnt)
		{
			List<FoodItem> filterd = new List<FoodItem> ();
			foreach (FoodItem f in orig_inventory) {
				double calories = f.caloriesPerServing;
				double protein = f.protein;
				if ((minCalSrchAmnt<=calories) && (calories <= maxCalSrchAmnt)
					&&(minPrtnSrchAmnt <= protein) && (protein<=maxPrtnSrchAmnt))
					filterd.Add (f);
			}
			inventory = filterd;
			NotifyDataSetChanged ();
		}

		public void resetInventory()
		{
			orig_inventory = Inventory.InventoryService.inventory;
			inventory = Inventory.InventoryService.inventory;
			NotifyDataSetChanged ();
		}

		/**
		 * Takes in five parameters. Maps a protein value to the number of images availabel
		 * returns an image in matching range
		 * */
		public Bitmap GetProteinIndicatorImg(int currentFoodItemPrtn, int minPrtn, int maxPrtn, int minIndicators, int maxIndicators)
		{
			int prtnRange = maxPrtn - minPrtn;
			int imageRange = maxIndicators - minIndicators;

			float prtnScaled = ((float)currentFoodItemPrtn - minPrtn) / (float)prtnRange;

			float imgValue = minIndicators + (prtnScaled * imageRange);

			string imgName = "ic_";

			switch ((int)imgValue) {
			case 0:
				imgName += "lowest";
				break;
			case 1:
				imgName += "low";
				break;
			case 2:
				imgName += "medlow";
				break;
			case 3:
				imgName += "medhigh";
				break;
			case 4:
				imgName += "high";
				break;
			case 5:
				imgName += "highest";
				break;
			default:
				imgName += "lowest";
				break;	
			}

			var drawableImg = context.Resources.GetDrawable (context.Resources.GetIdentifier(imgName, "drawable", context.PackageName));
			return (drawableImg as BitmapDrawable).Bitmap;
		}


	}
}

