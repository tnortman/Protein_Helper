using System;
using SQLite;

namespace ProteinHelper
{
	public class FoodItem
	{
		[PrimaryKey]
		public long? UPC{ get; set; }
		public String brandName{ get; set;}
		public String productName { get; set; }
		public double protein{ get; set; }
		public double caloriesPerServing{ get; set; }
		public String imageUrl { get; set;}


		public FoodItem ()
		{
			UPC = null;
			productName = "";
			protein = 0.0;
			caloriesPerServing = 0.0;
			imageUrl = "";
		}


		public FoodItem(long UPC, String brandName, String itemName, double protein, double calories)
		{
			this.brandName = brandName;
			this.UPC = UPC;
			this.productName = itemName;
			this.protein = protein;
			caloriesPerServing = calories;
			this.imageUrl = "";
		}

		public FoodItem(long UPC, String brandName, String itemName, double protein, double calories, String imageUrl)
		{
			this.brandName = brandName;
			this.UPC = UPC;
			this.productName = itemName;
			this.protein = protein;
			caloriesPerServing = calories;
			this.imageUrl = imageUrl;
		}

		//Generates unique UPC for a custom foodItem
		public void GenerateUPC() 
		{
			Random rand = new Random();
			long min = 000000000000000001;
			long max = 999999999999999999;
			byte[] buf = new byte[8];
			rand.NextBytes(buf);
			long longRand = BitConverter.ToInt64(buf, 0);
			UPC = Math.Abs(longRand % (max - min)) + min;
		}
			




	}
}

