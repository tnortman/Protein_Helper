﻿using System.Threading;

using Android.App;
using Android.OS;

namespace ProteinHelper
{
	

	[Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
	public class SplashActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			StartActivity(typeof(HomeActivity));
			this.Finish ();
		}
	}
}