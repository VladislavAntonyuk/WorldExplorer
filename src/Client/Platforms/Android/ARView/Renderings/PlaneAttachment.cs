namespace Client.Renderings;

using Google.AR.Core;

public class PlaneAttachment
{
	private readonly Anchor anchor;
	private readonly float[] mPoseRotation = new float[4];

	// Allocate temporary storage to avoid multiple allocations per frame.
	private readonly float[] mPoseTranslation = new float[3];
	private readonly Plane plane;

	public PlaneAttachment(Plane plane, Anchor anchor)
	{
		this.plane = plane;
		this.anchor = anchor;
	}

	public bool IsTracking => /*true if*/plane.TrackingState == TrackingState.Tracking &&
										 anchor.TrackingState == TrackingState.Tracking;

	public Pose GetPose()
	{
		var pose = anchor.Pose;
		pose.GetTranslation(mPoseTranslation, 0);
		pose.GetRotationQuaternion(mPoseRotation, 0);
		mPoseTranslation[1] = plane.CenterPose.Ty();
		return new Pose(mPoseTranslation, mPoseRotation);
	}

	public Anchor GetAnchor()
	{
		return anchor;
	}
}