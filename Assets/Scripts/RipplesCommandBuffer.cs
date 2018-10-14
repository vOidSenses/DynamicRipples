using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class RipplesCommandBuffer : MonoBehaviour {

	public List<ParticleSystem> particles;
	private Dictionary<Camera, CommandBuffer> m_Cameras = new Dictionary<Camera, CommandBuffer>();

	public int texSize = 2048;
	public float rippleDist = 64.0f;
	public RenderTexture targetTex;

	void Start()
	{
		CreateTexture();
	}

	void CreateTexture()
	{
		targetTex = new RenderTexture( texSize, texSize, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear );
		targetTex.Create();
		Shader.SetGlobalTexture( "_DynamicRippleTexture", targetTex );
	}

	// Remove command buffers from all cameras we added into
	private void Cleanup()
	{
		foreach( var cam in m_Cameras )
		{
			if( cam.Key )
			{
				cam.Key.RemoveCommandBuffer( CameraEvent.AfterSkybox, cam.Value );
			}
		}
		m_Cameras.Clear();
	}

	public void OnEnable()
	{
		Shader.EnableKeyword( "DYNAMIC_RIPPLES_ON" );
		Cleanup();
	}

	public void OnDisable()
	{
		Shader.DisableKeyword( "DYNAMIC_RIPPLES_ON" );
		Cleanup();
	}

	public void OnWillRenderObject()
	{
		var act = gameObject.activeInHierarchy && enabled;
		if( !act )
		{
			Cleanup();
			return;
		}

		var cam = Camera.current;
		if( !cam )
			return;

		CommandBuffer buf = null;
		if( m_Cameras.ContainsKey( cam ) )
			return;

		buf = new CommandBuffer();
		buf.SetRenderTarget( targetTex );
		buf.name = "Projected Ripples";
		buf.ClearRenderTarget( true, true, new Color( 0.5f, 0.5f, 0.5f, 0.5f ) );
		m_Cameras[ cam ] = buf;
		Matrix4x4 V = Matrix4x4.LookAt( new Vector3( 0, 258f, 0 ), Vector3.zero, Vector3.forward ).inverse;
		Matrix4x4 P = Matrix4x4.Ortho( -32, 32, -32, 32, -1, -500 );
		buf.SetViewProjectionMatrices( V, P );

		for( int i = 0; i < particles.Count; i++ )
		{
			ParticleSystemRenderer pr = particles[ i ].GetComponent<ParticleSystemRenderer>();
			buf.DrawRenderer( pr, pr.sharedMaterial );
		}

		cam.AddCommandBuffer( CameraEvent.AfterSkybox, buf );

		Shader.SetGlobalMatrix( "_DynamicRippleMatrix", V );
		Shader.SetGlobalFloat( "_DynamicRippleSize", 32 );
	}
}
