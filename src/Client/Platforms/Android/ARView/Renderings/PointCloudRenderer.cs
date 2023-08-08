namespace Client.Renderings;

using Android.Content;
using Android.Opengl;
using Google.AR.Core;
using Java.Lang;

public class PointCloudRenderer
{
	private const string Tag = "POINTCLOUDRENDERER";

	private const int BytesPerFloat = Float.Size / 8;
	private const int FloatsPerPoint = 4; // X,Y,Z,confidence.
	private const int BytesPerPoint = BytesPerFloat * FloatsPerPoint;
	private const int InitialBufferPoints = 1000;
	private int mColorUniform;

	// Keep track of the last point cloud rendered to avoid updating the VBO if point cloud
	// was not changed.
	private PointCloud? mLastPointCloud;
	private int mModelViewProjectionUniform;

	private int mNumPoints;
	private int mPointSizeUniform;
	private int mPositionAttribute;

	private int mProgramName;

	private int mVbo;
	private int mVboSize;

	/**
	 * Allocates and initializes OpenGL resources needed by the plane renderer.  Must be
	 * called on the OpenGL thread, typically in
	 * {@link GLSurfaceView.Renderer#onSurfaceCreated(GL10, EGLConfig)}.
	 *
	 * @param context Needed to access shader source.
	 */
	public void CreateOnGlThread(Context context)
	{
		ShaderUtil.CheckGlError(Tag, "before create");

		var buffers = new int[1];
		GLES20.GlGenBuffers(1, buffers, 0);
		mVbo = buffers[0];
		GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVbo);

		mVboSize = InitialBufferPoints * BytesPerPoint;
		GLES20.GlBufferData(GLES20.GlArrayBuffer, mVboSize, null, GLES20.GlDynamicDraw);
		GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);

		ShaderUtil.CheckGlError(Tag, "buffer alloc");

		var vertexShader = ShaderUtil.LoadGlShader(context, GLES20.GlVertexShader, Resource.Raw.point_cloud_vertex);
		var passthroughShader =
			ShaderUtil.LoadGlShader(context, GLES20.GlFragmentShader, Resource.Raw.passthrough_fragment);

		mProgramName = GLES20.GlCreateProgram();
		GLES20.GlAttachShader(mProgramName, vertexShader);
		GLES20.GlAttachShader(mProgramName, passthroughShader);
		GLES20.GlLinkProgram(mProgramName);
		GLES20.GlUseProgram(mProgramName);

		ShaderUtil.CheckGlError(Tag, "program");

		mPositionAttribute = GLES20.GlGetAttribLocation(mProgramName, "a_Position");
		mColorUniform = GLES20.GlGetUniformLocation(mProgramName, "u_Color");
		mModelViewProjectionUniform = GLES20.GlGetUniformLocation(mProgramName, "u_ModelViewProjection");
		mPointSizeUniform = GLES20.GlGetUniformLocation(mProgramName, "u_PointSize");

		ShaderUtil.CheckGlError(Tag, "program  params");
	}

	/**
	 * Updates the OpenGL buffer contents to the provided point.  Repeated calls with the same
	 * point cloud will be ignored.
	 */
	public void Update(PointCloud cloud)
	{
		if (mLastPointCloud == cloud)
		{
			// Redundant call.
			return;
		}

		ShaderUtil.CheckGlError(Tag, "before update");

		GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVbo);
		mLastPointCloud = cloud;

		// If the VBO is not large enough to fit the new point cloud, resize it.
		mNumPoints = mLastPointCloud.Points.Remaining() / FloatsPerPoint;
		if (mNumPoints * BytesPerPoint > mVboSize)
		{
			while (mNumPoints * BytesPerPoint > mVboSize)
			{
				mVboSize *= 2;
			}

			GLES20.GlBufferData(GLES20.GlArrayBuffer, mVboSize, null, GLES20.GlDynamicDraw);
		}

		GLES20.GlBufferSubData(GLES20.GlArrayBuffer, 0, mNumPoints * BytesPerPoint, mLastPointCloud.Points);
		GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);

		ShaderUtil.CheckGlError(Tag, "after update");
	}

	/**
	 * Renders the point cloud.
	 *
	 * @param pose the current point cloud pose, from {@link Frame#getPointCloudPose()}.
	 * @param cameraView the camera view matrix for this frame, typically from
	 * {@link Frame#getViewMatrix(float[], int)}.
	 * @param cameraPerspective the camera projection matrix for this frame, typically from
	 * {@link Session#getProjectionMatrix(float[], int, float, float)}.
	 */
	public void Draw(Pose pose, float[] cameraView, float[] cameraPerspective)
	{
		var modelMatrix = new float[16];
		pose.ToMatrix(modelMatrix, 0);

		var modelView = new float[16];
		var modelViewProjection = new float[16];
		Matrix.MultiplyMM(modelView, 0, cameraView, 0, modelMatrix, 0);
		Matrix.MultiplyMM(modelViewProjection, 0, cameraPerspective, 0, modelView, 0);

		ShaderUtil.CheckGlError(Tag, "Before draw");

		GLES20.GlUseProgram(mProgramName);
		GLES20.GlEnableVertexAttribArray(mPositionAttribute);
		GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVbo);
		GLES20.GlVertexAttribPointer(mPositionAttribute, 4, GLES20.GlFloat, false, BytesPerPoint, 0);
		GLES20.GlUniform4f(mColorUniform, 31.0f / 255.0f, 188.0f / 255.0f, 210.0f / 255.0f, 1.0f);
		GLES20.GlUniformMatrix4fv(mModelViewProjectionUniform, 1, false, modelViewProjection, 0);
		GLES20.GlUniform1f(mPointSizeUniform, 5.0f);

		GLES20.GlDrawArrays(GLES20.GlPoints, 0, mNumPoints);
		GLES20.GlDisableVertexAttribArray(mPositionAttribute);
		GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);

		ShaderUtil.CheckGlError(Tag, "Draw");
	}
}