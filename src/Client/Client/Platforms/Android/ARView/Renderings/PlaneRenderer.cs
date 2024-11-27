namespace Client.Renderings;

using Android.Content;
using Android.Graphics;
using Android.Opengl;
using Google.AR.Core;
using Java.Lang;
using Java.Nio;
using Math = System.Math;
using Matrix = Android.Opengl.Matrix;
using String = System.String;

public class PlaneRenderer
{
	private const string Tag = "PLANERENDERER";

	private const int BytesPerFloat = Float.Size / 8;
	private const int BytesPerShort = Short.Size / 8;
	private const int CoordsPerVertex = 3; // x, z, alpha

	private const int VertsPerBoundaryVert = 2;
	private const int IndicesPerBoundaryVert = 3;
	private const int InitialBufferBoundaryVerts = 64;

	private const int InitialVertexBufferSizeBytes =
		BytesPerFloat * CoordsPerVertex * VertsPerBoundaryVert * InitialBufferBoundaryVerts;

	private const int InitialIndexBufferSizeBytes = BytesPerShort *
	                                                IndicesPerBoundaryVert *
	                                                IndicesPerBoundaryVert *
	                                                InitialBufferBoundaryVerts;

	private const float FadeRadiusM = 0.25f;
	private const float DotsPerMeter = 10.0f;
	private static readonly float EquilateralTriangleScale = (float)(1 / Math.Sqrt(3));

	// Using the "signed distance field" approach to render sharp lines and circles.
	// {dotThreshold, lineThreshold, lineFadeSpeed, occlusionScale}
	// dotThreshold/lineThreshold: red/green intensity above which dots/lines are present
	// lineFadeShrink:  lines will fade in between alpha = 1-(1/lineFadeShrink) and 1.0
	// occlusionShrink: occluded planes will fade out between alpha = 0 and 1/occlusionShrink
	private static readonly float[] GridControl = [0.2f, 0.4f, 2.0f, 1.5f];

	private static readonly int[] PlaneColorsRgba =
	[
		//0xFFFFFFFF,
		//0xF44336FF,
		//0xE91E63FF,
		//0x9C27B0FF,
		0x673AB7FF, 0x3F51B5FF, 0x2196F3FF, 0x03A9F4FF, 0x00BCD4FF, 0x009688FF, 0x4CAF50FF
		//0x8BC34AFF,
		//0xCDDC39FF,
		//0xFFEB3BFF,
		//0xFFC107FF,
		//0xFF9800FF,
	];

	private static readonly ByteOrder ByteOrder =
		ByteOrder.NativeOrder() ?? throw new Exception("ByteOrder NativeOrder is null");

	// Temporary lists/matrices allocated here to reduce number of allocations for each frame.
	private readonly float[] mModelMatrix = new float[16];
	private readonly float[] mModelViewMatrix = new float[16];
	private readonly float[] mModelViewProjectionMatrix = new float[16];
	private readonly float[] mPlaneAngleUvMatrix = new float[4]; // 2x2 rotation matrix applied to uv coords.
	private readonly float[] mPlaneColor = new float[4];

	private readonly Dictionary<Plane, int> mPlaneIndexMap = [];
	private readonly int[] mTextures = new int[1];

	private int mDotColorUniform;
	private int mGridControlUniform;

	private ShortBuffer mIndexBuffer = ByteBuffer.AllocateDirect(InitialIndexBufferSizeBytes)
	                                             .Order(ByteOrder)
	                                             .AsShortBuffer();

	private int mLineColorUniform;

	private int mPlaneModelUniform;
	private int mPlaneModelViewProjectionUniform;

	private int mPlaneProgram;
	private int mPlaneUvMatrixUniform;

	private int mPlaneXzPositionAlphaAttribute;
	private int mTextureUniform;

	private FloatBuffer mVertexBuffer = ByteBuffer.AllocateDirect(InitialVertexBufferSizeBytes)
	                                              .Order(ByteOrder)
	                                              .AsFloatBuffer();

	/**
	 * Allocates and initializes OpenGL resources needed by the plane renderer.  Must be
	 * called on the OpenGL thread, typically in
	 * {@link GLSurfaceView.Renderer#onSurfaceCreated(GL10, EGLConfig)}.
	 *
	 * @param context Needed to access shader source and texture PNG.
	 * @param gridDistanceTextureName  Name of the PNG file containing the grid texture.
	 */
	public void CreateOnGlThread(Context context, String gridDistanceTextureName)
	{
		var vertexShader = ShaderUtil.LoadGlShader(context, GLES20.GlVertexShader, Resource.Raw.plane_vertex);
		var passthroughShader = ShaderUtil.LoadGlShader(context, GLES20.GlFragmentShader, Resource.Raw.plane_fragment);

		mPlaneProgram = GLES20.GlCreateProgram();
		GLES20.GlAttachShader(mPlaneProgram, vertexShader);
		GLES20.GlAttachShader(mPlaneProgram, passthroughShader);
		GLES20.GlLinkProgram(mPlaneProgram);
		GLES20.GlUseProgram(mPlaneProgram);

		ShaderUtil.CheckGlError(Tag, "Program creation");

		// Read the texture.
		var textureBitmap = BitmapFactory.DecodeStream(context.Assets?.Open(gridDistanceTextureName));

		GLES20.GlActiveTexture(GLES20.GlTexture0);
		GLES20.GlGenTextures(mTextures.Length, mTextures, 0);
		GLES20.GlBindTexture(GLES20.GlTexture2d, mTextures[0]);

		GLES20.GlTexParameteri(GLES20.GlTexture2d, GLES20.GlTextureMinFilter, GLES20.GlLinearMipmapLinear);
		GLES20.GlTexParameteri(GLES20.GlTexture2d, GLES20.GlTextureMagFilter, GLES20.GlLinear);
		GLUtils.TexImage2D(GLES20.GlTexture2d, 0, textureBitmap, 0);
		GLES20.GlGenerateMipmap(GLES20.GlTexture2d);
		GLES20.GlBindTexture(GLES20.GlTexture2d, 0);

		ShaderUtil.CheckGlError(Tag, "Texture loading");

		mPlaneXzPositionAlphaAttribute = GLES20.GlGetAttribLocation(mPlaneProgram, "a_XZPositionAlpha");

		mPlaneModelUniform = GLES20.GlGetUniformLocation(mPlaneProgram, "u_Model");
		mPlaneModelViewProjectionUniform = GLES20.GlGetUniformLocation(mPlaneProgram, "u_ModelViewProjection");
		mTextureUniform = GLES20.GlGetUniformLocation(mPlaneProgram, "u_Texture");
		mLineColorUniform = GLES20.GlGetUniformLocation(mPlaneProgram, "u_lineColor");
		mDotColorUniform = GLES20.GlGetUniformLocation(mPlaneProgram, "u_dotColor");
		mGridControlUniform = GLES20.GlGetUniformLocation(mPlaneProgram, "u_gridControl");
		mPlaneUvMatrixUniform = GLES20.GlGetUniformLocation(mPlaneProgram, "u_PlaneUvMatrix");

		ShaderUtil.CheckGlError(Tag, "Program parameters");
	}

	/**
		 * Updates the plane model transform matrix and extents.
		 */
	private void UpdatePlaneParameters(float[] planeMatrix, float extentX, float extentZ, FloatBuffer? boundary)
	{
		Array.Copy(planeMatrix, 0, mModelMatrix, 0, 16);
		if (boundary == null)
		{
			mVertexBuffer.Limit(0);
			mIndexBuffer.Limit(0);
			return;
		}

		// Generate a new set of vertices and a corresponding triangle strip index set so that
		// the plane boundary polygon has a fading edge. This is done by making a copy of the
		// boundary polygon vertices and scaling it down around center to push it inwards. Then
		// the index buffer is setup accordingly.
		boundary.Rewind();
		var boundaryVertices = boundary.Limit() / 2;
		int numVertices;
		int numIndices;

		numVertices = boundaryVertices * VertsPerBoundaryVert;
		// drawn as GL_TRIANGLE_STRIP with 3n-2 triangles (n-2 for fill, 2n for perimeter).
		numIndices = boundaryVertices * IndicesPerBoundaryVert;

		if (mVertexBuffer.Capacity() < numVertices * CoordsPerVertex)
		{
			var size = mVertexBuffer.Capacity();
			while (size < numVertices * CoordsPerVertex)
			{
				size *= 2;
			}

			mVertexBuffer = ByteBuffer.AllocateDirect(BytesPerFloat * size).Order(ByteOrder).AsFloatBuffer();
		}

		mVertexBuffer.Rewind();
		mVertexBuffer.Limit(numVertices * CoordsPerVertex);


		if (mIndexBuffer.Capacity() < numIndices)
		{
			var size = mIndexBuffer.Capacity();
			while (size < numIndices)
			{
				size *= 2;
			}

			mIndexBuffer = ByteBuffer.AllocateDirect(BytesPerShort * size).Order(ByteOrder).AsShortBuffer();
		}

		mIndexBuffer.Rewind();
		mIndexBuffer.Limit(numIndices);

		// Note: when either dimension of the bounding box is smaller than 2*FADE_RADIUS_M we
		// generate a bunch of 0-area triangles.  These don't get rendered though so it works
		// out ok.
		var xScale = Math.Max((extentX - (2 * FadeRadiusM)) / extentX, 0.0f);
		var zScale = Math.Max((extentZ - (2 * FadeRadiusM)) / extentZ, 0.0f);

		while (boundary.HasRemaining)
		{
			var x = boundary.Get();
			var z = boundary.Get();
			mVertexBuffer.Put(x);
			mVertexBuffer.Put(z);
			mVertexBuffer.Put(0.0f);
			mVertexBuffer.Put(x * xScale);
			mVertexBuffer.Put(z * zScale);
			mVertexBuffer.Put(1.0f);
		}

		// step 1, perimeter
		mIndexBuffer.Put((short)((boundaryVertices - 1) * 2));
		for (var i = 0; i < boundaryVertices; ++i)
		{
			mIndexBuffer.Put((short)(i * 2));
			mIndexBuffer.Put((short)((i * 2) + 1));
		}

		mIndexBuffer.Put(1);
		// This leaves us on the interior edge of the perimeter between the inset vertices
		// for boundary verts n-1 and 0.

		// step 2, interior:
#pragma warning disable S2583
		for (var i = 1; i < boundaryVertices / 2; ++i)
#pragma warning restore S2583
		{
			mIndexBuffer.Put((short)(((boundaryVertices - 1 - i) * 2) + 1));
			mIndexBuffer.Put((short)((i * 2) + 1));
		}

		if (boundaryVertices % 2 != 0)
		{
			mIndexBuffer.Put((short)((boundaryVertices / 2 * 2) + 1));
		}
	}

	private void Draw(float[] cameraView, float[] cameraPerspective)
	{
		// Build the ModelView and ModelViewProjection matrices
		// for calculating cube position and light.
		Matrix.MultiplyMM(mModelViewMatrix, 0, cameraView, 0, mModelMatrix, 0);
		Matrix.MultiplyMM(mModelViewProjectionMatrix, 0, cameraPerspective, 0, mModelViewMatrix, 0);

		// Set the position of the plane
		mVertexBuffer.Rewind();
		GLES20.GlVertexAttribPointer(mPlaneXzPositionAlphaAttribute, CoordsPerVertex, GLES20.GlFloat, false,
		                             BytesPerFloat * CoordsPerVertex, mVertexBuffer);

		// Set the Model and ModelViewProjection matrices in the shader.
		GLES20.GlUniformMatrix4fv(mPlaneModelUniform, 1, false, mModelMatrix, 0);
		GLES20.GlUniformMatrix4fv(mPlaneModelViewProjectionUniform, 1, false, mModelViewProjectionMatrix, 0);

		mIndexBuffer.Rewind();
		GLES20.GlDrawElements(GLES20.GlTriangleStrip, mIndexBuffer.Limit(), GLES20.GlUnsignedShort, mIndexBuffer);
		ShaderUtil.CheckGlError(Tag, "Drawing plane");
	}

	/**
	 * Draws the collection of tracked planes, with closer planes hiding more distant ones.
	 *
	 * @param allPlanes The collection of planes to draw.
	 * @param cameraPose The pose of the camera, as returned by {@link Frame#getPose()}
	 * @param cameraPerspective The projection matrix, as returned by
	 * {@link Session#getProjectionMatrix(float[], int, float, float)}
	 */
	public void DrawPlanes(IEnumerable<Plane> allPlanes, Pose cameraPose, float[] cameraPerspective)
	{
		// Planes must be sorted by distance from camera so that we draw closer planes first, and
		// they occlude the farther planes.
		var sortedPlanes = new List<SortablePlane>();
		var normal = new float[3];
		var cameraX = cameraPose.Tx();
		var cameraY = cameraPose.Ty();
		var cameraZ = cameraPose.Tz();
		foreach (var plane in allPlanes)
		{
			if (plane.TrackingState != TrackingState.Tracking || plane.SubsumedBy != null)
			{
				continue;
			}

			var center = plane.CenterPose;
			// Get transformed Y axis of plane's coordinate system.
			center.GetTransformedAxis(1, 1.0f, normal, 0);
			// Compute dot product of plane's normal with vector from camera to plane center.
			var distance = ((cameraX - center.Tx()) * normal[0]) +
			               ((cameraY - center.Ty()) * normal[1]) +
			               ((cameraZ - center.Tz()) * normal[2]);
			if (distance < 0)
			{
				// Plane is back-facing.
				continue;
			}

			sortedPlanes.Add(new SortablePlane(distance, plane));
		}

		sortedPlanes.Sort((x, y) => x.Distance.CompareTo(y.Distance));


		var cameraView = new float[16];
		cameraPose.Inverse().ToMatrix(cameraView, 0);

		// Planes are drawn with additive blending, masked by the alpha channel for occlusion.

		// Start by clearing the alpha channel of the color buffer to 1.0.
		GLES20.GlClearColor(1, 1, 1, 1);
		GLES20.GlColorMask(false, false, false, true);
		GLES20.GlClear(GLES20.GlColorBufferBit);
		GLES20.GlColorMask(true, true, true, true);

		// Disable depth write.
		GLES20.GlDepthMask(false);

		// Additive blending, masked by alpha chanel, clearing alpha channel.
		GLES20.GlEnable(GLES20.GlBlend);
		GLES20.GlBlendFuncSeparate(GLES20.GlDstAlpha, GLES20.GlOne, // RGB (src, dest)
		                           GLES20.GlZero, GLES20.GlOneMinusSrcAlpha); // ALPHA (src, dest)

		// Set up the shader.
		GLES20.GlUseProgram(mPlaneProgram);

		// Attach the texture.
		GLES20.GlActiveTexture(GLES20.GlTexture0);
		GLES20.GlBindTexture(GLES20.GlTexture2d, mTextures[0]);
		GLES20.GlUniform1i(mTextureUniform, 0);

		// Shared fragment uniforms.
		GLES20.GlUniform4fv(mGridControlUniform, 1, GridControl, 0);

		// Enable vertex arrays
		GLES20.GlEnableVertexAttribArray(mPlaneXzPositionAlphaAttribute);

		ShaderUtil.CheckGlError(Tag, "Setting up to draw planes");

		foreach (var plane in sortedPlanes.Select(x => x.Plane))
		{
			var planeMatrix = new float[16];
			plane.CenterPose.ToMatrix(planeMatrix, 0);


			UpdatePlaneParameters(planeMatrix, plane.ExtentX, plane.ExtentZ, plane.Polygon);

			// Get plane index. Keep a map to assign same indices to same planes.

			if (!mPlaneIndexMap.TryGetValue(plane, out var planeIndex))
			{
				planeIndex = Integer.ValueOf(mPlaneIndexMap.Count).IntValue();
				mPlaneIndexMap.Add(plane, planeIndex);
			}

			// Set plane color. Computed deterministically from the Plane index.
			var colorIndex = planeIndex % PlaneColorsRgba.Length;

			ColorRgbaToFloat(mPlaneColor, PlaneColorsRgba[colorIndex]);
			GLES20.GlUniform4fv(mLineColorUniform, 1, mPlaneColor, 0);
			GLES20.GlUniform4fv(mDotColorUniform, 1, mPlaneColor, 0);

			// Each plane will have its own angle offset from others, to make them easier to
			// distinguish. Compute a 2x2 rotation matrix from the angle.
			var angleRadians = planeIndex * 0.144f;
			var uScale = DotsPerMeter;
			var vScale = DotsPerMeter * EquilateralTriangleScale;
			mPlaneAngleUvMatrix[0] = +(float)Math.Cos(angleRadians) * uScale;
			mPlaneAngleUvMatrix[1] = -(float)Math.Sin(angleRadians) * uScale;
			mPlaneAngleUvMatrix[2] = +(float)Math.Sin(angleRadians) * vScale;
			mPlaneAngleUvMatrix[3] = +(float)Math.Cos(angleRadians) * vScale;
			GLES20.GlUniformMatrix2fv(mPlaneUvMatrixUniform, 1, false, mPlaneAngleUvMatrix, 0);


			Draw(cameraView, cameraPerspective);
		}

		// Clean up the state we set
		GLES20.GlDisableVertexAttribArray(mPlaneXzPositionAlphaAttribute);
		GLES20.GlBindTexture(GLES20.GlTexture2d, 0);
		GLES20.GlDisable(GLES20.GlBlend);
		GLES20.GlDepthMask(true);

		ShaderUtil.CheckGlError(Tag, "Cleaning up after drawing planes");
	}

	private static void ColorRgbaToFloat(float[] planeColor, int colorRgba)
	{
		planeColor[0] = ((colorRgba >> 24) & 0xff) / 255.0f;
		planeColor[1] = ((colorRgba >> 16) & 0xff) / 255.0f;
		planeColor[2] = ((colorRgba >> 8) & 0xff) / 255.0f;
		planeColor[3] = ((colorRgba >> 0) & 0xff) / 255.0f;
	}

	public class SortablePlane(float distance, Plane plane)
	{
		public float Distance { get; } = distance;
		public Plane Plane { get; } = plane;
	}
}