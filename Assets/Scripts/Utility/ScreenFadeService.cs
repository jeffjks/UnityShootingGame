using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ScreenFadeService : MonoBehaviour
{
	public enum FadeState
	{
		FadeIn,
		FadeOut,
		FadingIn,
		FadingOut,
	}
	
    private static ScreenFadeService Instance { get; set; }
    
    public static Color Color
    {
        get => _color; 
        set { _color = new Color(value.r, value.g, value.b, _alpha); _material.SetColor(_materialColorID, _color); }
    }

    public static float Alpha
    {
        get => _alpha;
        set { if (!Mathf.Approximately(_alpha, value)) { _alpha = value; _color.a = value; _material.SetColor(_materialColorID, _color); } }
    }
    
    private FadeState _currentState = FadeState.FadeIn;

    private static Color _color = new Color(0f, 0f, 0f, 0f);
    private static float _alpha = 0;

    private static Material _material;
    private static int _materialColorID;
    
    void Awake()
    {
        if (!_material)
        {
            _materialColorID = Shader.PropertyToID("_Color");

            var shader = Shader.Find("Hidden/Internal-Colored");
            _material = new Material(shader);
            _material.hideFlags = HideFlags.HideAndDontSave;
            // Turn off backface culling, depth writes, depth test.
            _material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            _material.SetInt("_ZWrite", 0);
            _material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);

            Color = _color;
        }

        Instance = this;
        RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    }

    void OnDestroy()
    {
        Instance = null;
        RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
    }
		
    private static void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        OnPostRenderUpdate();
    }

    /// <summary>
    /// Add a quad in front of camera to fade with color.
    /// </summary>
    private static void OnPostRenderUpdate()
    {
        GL.PushMatrix();
        GL.LoadOrtho();

        // activate the first shader pass (in this case we know it is the only pass)
        _material.SetPass(0);
        // draw a quad over whole screen
        GL.Begin(GL.QUADS);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(1, 0, 0);
        GL.Vertex3(1, 1, 0);
        GL.Vertex3(0, 1, 0);
        GL.End();

        GL.PopMatrix();
    }

	public static void ScreenFadeIn(float duration = 1f)
	{
		Instance._currentState = FadeState.FadingIn;
		Instance.StopAllCoroutines();
		Instance.StartCoroutine(Instance.CameraFadeIn(duration));
	}

	public static void ScreenFadeOut(float duration = 1f)
	{
		Instance._currentState = FadeState.FadingOut;
		Instance.StopAllCoroutines();
		Instance.StartCoroutine(Instance.CameraFadeOut(duration));
	}

	private IEnumerator CameraFadeIn(float duration)
	{
		if (Mathf.Approximately(duration, 0))
		{
			Alpha = 0;
			yield return null;
			_currentState = FadeState.FadeIn;
			yield break;
		}

		for (float i = Alpha; (i - Time.deltaTime / duration) > 0; i -= Time.deltaTime / duration)
		{
			Alpha = i;
			yield return null;
		}

		Alpha = 0;
		yield return null;
		_currentState = FadeState.FadeIn;
	}

	private IEnumerator CameraFadeOut(float duration)
	{
		if (Mathf.Approximately(duration, 0))
		{
			Alpha = 1;
			yield return null;
			_currentState = FadeState.FadeOut;
			yield break;
		}
		
		for (float i = Alpha; (i + Time.deltaTime / duration) < 1; i += Time.deltaTime / duration)
		{
			Alpha = i;
			yield return null;
		}

		Alpha = 1;
		yield return null;
		_currentState = FadeState.FadeOut;
	}
}
