using System;
using Android.Views;
using Android.App;


namespace ProteinHelper
{
	public class MyTouchListener : Java.Lang.Object, View.IOnTouchListener
	{
		public enum Action {
			LR, //Left to Right
			RL, //Right to Left
			TB, //Top to Bottom
			BT, //Bottom to Top
			None //When no action is detected
		}

		//private const String logTag = "SwipeDetector";

		//sets how far the user has to swipe to delete
		private const int HORIZONTAL_MIN_DISTANCE = 200;
		//private const int VERTICAL_MIN_DISTANCE = 80;
		private float downX, upX;
		//private float downY, upY;
		private Action mSwipeDetected = Action.None;

		public MyTouchListener ()
		{
			
		}

		public bool swipeDetected()
		{
			return mSwipeDetected != Action.None;
		}
		public Action getAction()
		{
			return mSwipeDetected;
		}

		#region IOnTouchListener implementation
		public bool OnTouch (View v, MotionEvent e)
		{
			switch (e.Action) {
			case MotionEventActions.Down:
				downX = e.GetX ();
				//downY = e.GetY ();
				mSwipeDetected = Action.None;
				return false; //allow other events like click to be processed
			case MotionEventActions.Move:
				upX = e.GetX ();
				//upY = e.GetY ();
				float deltaX = downX - upX;
				//float deltaY = downY - upY;

				//horizontal swipe detection
				if (Math.Abs (deltaX) > HORIZONTAL_MIN_DISTANCE) {
					//left or right?
					if (deltaX < 0) {
						//Console.WriteLine (logTag + "Swipe Left to Right");
						mSwipeDetected = Action.LR;
						return true;
					}
					if (deltaX > 0) {
						//Console.WriteLine (logTag + "Swipe Right to Left");
						mSwipeDetected = Action.RL;
						return true;
					}
				} 
				//vertical swipe detection
				else 
				{
					return false;
//					if (Math.Abs (deltaY) > VERTICAL_MIN_DISTANCE) {
//						//Up or down?
//						if (deltaY < 0) {
//							
//							Console.WriteLine (logTag + "Swipe Top to Bottom");
//							mSwipeDetected = Action.TB;
//							return true;
//						}
//						if (deltaY > 0) {
//							Console.WriteLine (logTag + "Swipe Bottom to Top");
//							mSwipeDetected = Action.BT;
//							return true;
//						}
//					}

				}
				return true;
			}
			return false;

		}
		#endregion
	}
}

