﻿namespace Client.Renderings;

using Android.Content;
using Android.Opengl;
using Android.Util;

public static class ShaderUtil
{
	public static int LoadGlShader(Context context, int type, int resId)
	{
		var code = ReadRawTextFile(context, resId);
		var shader = GLES20.GlCreateShader(type);

		GLES20.GlShaderSource(shader, code);
		GLES20.GlCompileShader(shader);

		var compileStatus = new int[1];
		GLES20.GlGetShaderiv(shader, GLES20.GlCompileStatus, compileStatus, 0);

		if (compileStatus[0] == 0)
		{
			GLES20.GlDeleteShader(shader);
			shader = 0;
		}

		if (shader == 0)
		{
			throw new AndroidException("Error creating shader");
		}

		return shader;
	}

	public static void CheckGlError(string tag, string label)
	{
		int error;
		while ((error = GLES20.GlGetError()) != GLES20.GlNoError)
		{
			Log.Error(tag, label + ": glError " + error);
		}
	}


	private static string? ReadRawTextFile(Context context, int resId)
	{
		string? result = null;

		using var rs = context.Resources?.OpenRawResource(resId);
		if (rs is null)
		{
			return result;
		}

		using var sr = new StreamReader(rs);
		result = sr.ReadToEnd();

		return result;
	}
}