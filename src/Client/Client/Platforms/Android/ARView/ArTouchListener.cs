namespace Client;

using System.Collections.Concurrent;
using Android.Content;
using Android.Views;
using Java.Lang;

public class ArTouchListener : Object, View.IOnTouchListener
{
	public static readonly ConcurrentQueue<MotionEvent> MQueuedSingleTaps = new();
	private readonly GestureDetector mGestureDetector;

	public ArTouchListener(Context context)
	{
		mGestureDetector = new GestureDetector(context, new SimpleTapGestureDetector
		{
			SingleTapUpHandler = arg =>
			{
				OnSingleTap(arg);
				return true;
			},
			DownHandler = _ => true
		});
	}

	public bool OnTouch(View? v, MotionEvent? e)
	{
		if (e is null)
		{
			return false;
		}

		return mGestureDetector.OnTouchEvent(e);
	}

	private void OnSingleTap(MotionEvent e)
	{
		// Queue tap if there is space. Tap is lost if queue is full.
		if (MQueuedSingleTaps.Count < 16)
		{
			MQueuedSingleTaps.Enqueue(e);
		}
	}
}