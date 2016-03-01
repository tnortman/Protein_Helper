using System;
using System.Collections.Generic;
using System.IO;
using SQLite;
using System.Threading.Tasks;
using System.Linq;

namespace ProteinHelper
{
	public class InventoryService
	{
		
		public List<FoodItem> inventory = new List<FoodItem> ();
		private string storagePath;

		public InventoryService (string storagePath)
		{
			this.storagePath = storagePath;

			//Initiate our DB
			Task initializeDB = Task.Run(()=> initConnectionToDB());
			initializeDB.Wait ();
		}

		protected void initConnectionToDB()
		{
			try{
				var conn = new SQLiteConnection (storagePath);
				conn.CreateTable<FoodItem>();
			}
			catch(SQLiteException ex) {
				Console.WriteLine(ex.Message);
			}
		}


		public InventoryService(List<FoodItem> inventory)
		{
			this.inventory = inventory;
		}

		public String DeleteFood(FoodItem foodItem)
		{
			try
			{
				var conn = new SQLiteConnection(storagePath);
				conn.Delete(foodItem);
				return String.Format("{0} deleted", foodItem.productName);
			}
			catch(SQLiteException ex) {
				return ex.Message;
			}
		}

		//Returns a FoodItem from the inventory that has a matching UPC
		public FoodItem GetFoodItemByUPC(long UPC)
		{
			foreach(FoodItem f in inventory)
			{
				if (f.UPC == UPC)
					return f;
			}

			return null;
		}

		//Returns a FoodItem at position passed in (from inventory arrayList)
		public FoodItem GetFoodItemByPosition(int id)
		{
			return inventory [id];
		}

//		public void AddFoodItem(FoodItem foodItem)
//		{
//			string name = foodItem.Name;
//			double protein = (double)foodItem.NutritionFact_Protein;
//			double calories = (double)foodItem.NutritionFact_Calories;
//			FoodItem foodToAdd = new FoodItem (name, protein, calories);
//			inventory.Add (foodToAdd);
//		}
//
//		public void UpdateFoodItem(FoodItem foodItem)
//		{
//			
//		}

		public String insertUpdateFoodItem(FoodItem foodItem)
		{
			try{
				var db = new SQLiteConnection(storagePath);
				db.InsertOrReplace(foodItem);
				return String.Format("{0} Added/Updated", foodItem.productName);
			}
			catch(SQLiteException ex) {
				return ex.Message;
			}
		}

		public int foodItemCount()
		{
			try{
				var db = new SQLiteConnection(storagePath);
				var count = db.ExecuteScalar<int>("SELECT Count(*) FROM FoodItem");
				return count;
			}

			catch(SQLiteException ex){
				Console.WriteLine ("No Data: " + ex.Message);
				return -1;
			}
		}

		/**
		 * @UPC - a long that holds the UPC
		 * 
		 * Returns true if their is a food item in the inventory with matching UPC, else false
		 * */
		public bool matchingUPC(long UPC)
		{
			foreach(FoodItem f in inventory)
			{
				if (f.UPC == UPC) {
					return true;
				}	
			}
			return false;
		}


		public void RefreshCache()
		{
			inventory.Clear ();

			var conn = new SQLiteConnection (storagePath);
			List<FoodItem> newCache = conn.Table<FoodItem> ().OrderBy(fi=>fi.protein).ToList<FoodItem> ();

			foreach (FoodItem foodItem in newCache) {
				inventory.Add (foodItem);
			}

//			var conn = new SQLiteAsyncConnection (storagePath);
//			conn.Table<FoodItem> ().ToListAsync ().ContinueWith ((t) => {
//				if(t.Result.Count != 0)
//				{
//					foreach (var foodItem in t.Result) {
//						inventory.Add (foodItem);
//					}
//				}
//			});
		}
	
		public int GetMaxProteinInInventory()
		{
			int maxPrtn = 0;

			foreach (FoodItem f in inventory) {
				if (f.protein > maxPrtn)
					maxPrtn =(int) f.protein;
			}
			return maxPrtn;
		}

		public bool matchingFoodItemByProductName(string productName)
		{
			foreach (FoodItem f in inventory) {
				if (f.productName.Equals (productName)) {
					return true;
				}
			}
			return false;
		}





	}
}

