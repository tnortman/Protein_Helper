using System;
using Android.Views;
using Android.Widget;
using Android.App;


namespace ProteinHelper
{
	public class MyGestureListener : GestureDetector.SimpleOnGestureListener
	{
		private static int SWIPE_MIN_DISTANCE = 150;
		private static int SWIPE_MAX_OFF_PATH = 100;
		private static int SWIPE_THRESHOLD_VELOCITY = 100;

		Activity context;

		private MotionEvent mLastOnDownEvent = null;



		public MyGestureListener (Activity context)
		{
			this.context = context;	
		}


		public override bool OnDown(MotionEvent e)
		{
			mLastOnDownEvent = e;
			return base.OnDown (e);
		}

		public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			if (e1 == null)
				e1 = mLastOnDownEvent;
			if (e1 == null || e2 == null)
				return false;
			float dX = e2.GetX () - e1.GetX ();
			float dY = e1.GetY () - e2.GetY ();
			if (Math.Abs (dY) < SWIPE_MAX_OFF_PATH && Math.Abs (velocityX) >= SWIPE_THRESHOLD_VELOCITY && Math.Abs (dX) >= SWIPE_MIN_DISTANCE) {
				if (dX > 0) {
					Toast.MakeText (context, "Right Swipe", ToastLength.Short).Show ();
				} else {
					Toast.MakeText (context, "Left Swipe", ToastLength.Short).Show ();
				}
				return true;
			} 
			else if (Math.Abs (dX) < SWIPE_MAX_OFF_PATH && Math.Abs (velocityY) >= SWIPE_THRESHOLD_VELOCITY && Math.Abs (dY) >= SWIPE_MIN_DISTANCE) {
				if (dY > 0) {
					Toast.MakeText (context, "Up Swipe", ToastLength.Short).Show ();
				} else {
					Toast.MakeText (context, "Down Swipe", ToastLength.Short).Show ();
				}
				return true;
			}
			return false;
		}
			 
	}
}

