using System;
using System.IO;

namespace ProteinHelper
{
	public class Inventory
	{
		private static string folder = Environment.GetFolderPath (Environment.SpecialFolder.Personal);

		public static readonly InventoryService InventoryService = new InventoryService(
			System.IO.Path.Combine(folder, "FoodItem.db"));



	}
}

