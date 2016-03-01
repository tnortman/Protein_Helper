using System;
using Nutritionix;
using Android.App;

namespace ProteinHelper
{
	public class NutritionixService
	{
		private const string appId = "e2da987b";
		private const string appKey = "4906ebfa7447b8b6a38949457522a98b";
		public NutritionixService ()
		{
			
		}

		public Item SearchByUPC(String upc)
		{
			var nutritionx = new NutritionixClient ();
			nutritionx.Initialize (appId, appKey);

			try{
				Item upcItem = nutritionx.RetrieveItemByUPC (upc);
				if (upcItem != null)
					Console.WriteLine ("Success, scanned in {0}", upcItem.Name);
				else
					Console.WriteLine ("Scan unsuccessful");
				return upcItem;
			}
			catch(NutritionixException e){
				Console.WriteLine (e.Message);
				return null;
			}
		}
	}
}

