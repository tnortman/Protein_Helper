using System;
using Nutritionix;

namespace ProteinHelper
{
	public class NutritionixService
	{
		private const string appId = "47117521";
		private const string appKey = "dc35128245a717589201496e811c5a18";

		public NutritionixService ()
		{
		}

		public Item SearchByUPC(String upc)
		{
			var nutritionx = new NutritionixClient ();
			nutritionx.Initialize (appId, appKey);

			Item upcItem = nutritionx.RetrieveItemByUPC (upc);

			if (upcItem != null)
				Console.WriteLine ("Success, scanned in {0}", upcItem.Name);
			else
				Console.WriteLine ("Scan unsuccessful");
			return upcItem;
		}
	}
}

