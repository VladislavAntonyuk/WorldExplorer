namespace Client;

using Android.Views;

internal class SimpleTapGestureDetector : GestureDetector.SimpleOnGestureListener
{
	public Func<MotionEvent, bool>? SingleTapUpHandler { get; set; }

	public Func<MotionEvent, bool>? DownHandler { get; set; }

	public override bool OnSingleTapUp(MotionEvent e)
	{
		return SingleTapUpHandler?.Invoke(e) ?? false;
	}

	public override bool OnDown(MotionEvent e)
	{
		return DownHandler?.Invoke(e) ?? false;
	}
}