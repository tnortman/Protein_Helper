using Android.App;
using Android.Widget;
using Android.OS;
using System;
using System.Threading.Tasks;

namespace ProteinHelper
{
	[Activity (Label = "Protein Tracker", Icon = "@drawable/ic_proteinLogo")]
	public class HomeActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			var queryBtn = FindViewById<ImageButton> (Resource.Id.searchButton);
			var myInvBtn = FindViewById<ImageButton> (Resource.Id.myInventoryButton);

			queryBtn.Click += delegate {
				StartActivity(typeof(QueryActivity));
			};

			myInvBtn.Click += delegate {
				StartActivity(typeof(InventoryActivity));
			};
				
		}
	}
}
