namespace Client.Renderings;

using Android.Content;
using Android.Opengl;
using Google.AR.Core;
using Java.Nio;

public class BackgroundRenderer
{
	private const string Tag = "BACKGROUNDRENDERER";

	private const int CoordsPerVertex = 3;
	private const int TexcoordsPerVertex = 2;
	private const int FloatSize = 4;

	private static readonly ByteOrder? ByteOrder = ByteOrder.NativeOrder();

	private static readonly float[] QuadCoords =
	[
		-1.0f,
		-1.0f,
		0.0f,
		-1.0f,
		+1.0f,
		0.0f,
		+1.0f,
		-1.0f,
		0.0f,
		+1.0f,
		+1.0f,
		0.0f
	];

	private static readonly float[] QuadTexcoords =
	[
		0.0f,
		1.0f,
		0.0f,
		0.0f,
		1.0f,
		1.0f,
		1.0f,
		0.0f
	];

	private readonly int mTextureTarget = GLES11Ext.GlTextureExternalOes;

	private int mQuadPositionParam;

	private int mQuadProgram;
	private FloatBuffer? mQuadTexCoord;
	private int mQuadTexCoordParam;
	private FloatBuffer? mQuadTexCoordTransformed;

	private FloatBuffer? mQuadVertices;

	public int TextureId
	{
		get;
		private set;
	} = -1;

	/**
	 * Allocates and initializes OpenGL resources needed by the background renderer.  Must be
	 * called on the OpenGL thread, typically in
	 * {@link GLSurfaceView.Renderer#onSurfaceCreated(GL10, EGLConfig)}.
	 * 
	 * @param context Needed to access shader source.
	 */
	public void CreateOnGlThread(Context context)
	{
		// Generate the background texture.
		var textures = new int[1];
		GLES20.GlGenTextures(1, textures, 0);
		TextureId = textures[0];
		GLES20.GlBindTexture(mTextureTarget, TextureId);
		GLES20.GlTexParameteri(mTextureTarget, GLES20.GlTextureWrapS, GLES20.GlClampToEdge);
		GLES20.GlTexParameteri(mTextureTarget, GLES20.GlTextureWrapT, GLES20.GlClampToEdge);
		GLES20.GlTexParameteri(mTextureTarget, GLES20.GlTextureMinFilter, GLES20.GlNearest);
		GLES20.GlTexParameteri(mTextureTarget, GLES20.GlTextureMagFilter, GLES20.GlNearest);

		var numVertices = 4;
		if (numVertices != QuadCoords.Length / CoordsPerVertex)
		{
			throw new InvalidDataException("Unexpected number of vertices in BackgroundRenderer.");
		}

		var bbVertices = ByteBuffer.AllocateDirect(QuadCoords.Length * FloatSize);
		ArgumentNullException.ThrowIfNull(ByteOrder);
		bbVertices.Order(ByteOrder);
		mQuadVertices = bbVertices.AsFloatBuffer();
		mQuadVertices.Put(QuadCoords);
		mQuadVertices.Position(0);

		var bbTexCoords = ByteBuffer.AllocateDirect(numVertices * TexcoordsPerVertex * FloatSize);
		bbTexCoords.Order(ByteOrder);
		mQuadTexCoord = bbTexCoords.AsFloatBuffer();
		mQuadTexCoord.Put(QuadTexcoords);
		mQuadTexCoord.Position(0);

		var bbTexCoordsTransformed = ByteBuffer.AllocateDirect(numVertices * TexcoordsPerVertex * FloatSize);
		bbTexCoordsTransformed.Order(ByteOrder);
		mQuadTexCoordTransformed = bbTexCoordsTransformed.AsFloatBuffer();

		var vertexShader = ShaderUtil.LoadGlShader(context, GLES20.GlVertexShader, Resource.Raw.screenquad_vertex);
		var fragmentShader =
			ShaderUtil.LoadGlShader(context, GLES20.GlFragmentShader, Resource.Raw.screenquad_fragment_oes);

		mQuadProgram = GLES20.GlCreateProgram();
		GLES20.GlAttachShader(mQuadProgram, vertexShader);
		GLES20.GlAttachShader(mQuadProgram, fragmentShader);
		GLES20.GlLinkProgram(mQuadProgram);
		GLES20.GlUseProgram(mQuadProgram);

		ShaderUtil.CheckGlError(Tag, "Program creation");

		mQuadPositionParam = GLES20.GlGetAttribLocation(mQuadProgram, "a_Position");
		mQuadTexCoordParam = GLES20.GlGetAttribLocation(mQuadProgram, "a_TexCoord");

		ShaderUtil.CheckGlError(Tag, "Program parameters");
	}

	/**
	 * Draws the AR background image.  The image will be drawn such that virtual content rendered
	 * with the matrices provided by {@link Frame#getViewMatrix(float[], int)} and
	 * {@link Session#getProjectionMatrix(float[], int, float, float)} will accurately follow
	 * static physical objects.  This must be called
	 * <b>before</b>
	 * drawing virtual content.
	 * 
	 * @param frame The last {@code Frame} returned by {@link Session#update()}.
	 */
	public void Draw(Frame frame)
	{
		// If display rotation changed (also includes view size change), we need to re-query the uv
		// coordinates for the screen rect, as they may have changed as well.
		if (frame.HasDisplayGeometryChanged)
		{
			frame.TransformCoordinates2d(Coordinates2d.ViewNormalized, mQuadTexCoord, Coordinates2d.TextureNormalized,
										 mQuadTexCoordTransformed);
		}

		// No need to test or write depth, the screen quad has arbitrary depth, and is expected
		// to be drawn first.
		GLES20.GlDisable(GLES20.GlDepthTest);
		GLES20.GlDepthMask(false);

		GLES20.GlBindTexture(GLES11Ext.GlTextureExternalOes, TextureId);

		GLES20.GlUseProgram(mQuadProgram);

		// Set the vertex positions.
		GLES20.GlVertexAttribPointer(mQuadPositionParam, CoordsPerVertex, GLES20.GlFloat, false, 0, mQuadVertices);

		// Set the texture coordinates.
		GLES20.GlVertexAttribPointer(mQuadTexCoordParam, TexcoordsPerVertex, GLES20.GlFloat, false, 0,
									 mQuadTexCoordTransformed);

		// Enable vertex arrays
		GLES20.GlEnableVertexAttribArray(mQuadPositionParam);
		GLES20.GlEnableVertexAttribArray(mQuadTexCoordParam);

		GLES20.GlDrawArrays(GLES20.GlTriangleStrip, 0, 4);

		// Disable vertex arrays
		GLES20.GlDisableVertexAttribArray(mQuadPositionParam);
		GLES20.GlDisableVertexAttribArray(mQuadTexCoordParam);

		// Restore the depth state for further drawing.
		GLES20.GlDepthMask(true);
		GLES20.GlEnable(GLES20.GlDepthTest);

		ShaderUtil.CheckGlError(Tag, "Draw");
	}
}