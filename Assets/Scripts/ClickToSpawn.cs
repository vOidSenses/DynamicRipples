using UnityEngine;
using System.Collections;

public class ClickToSpawn : MonoBehaviour
{

	public GameObject prefabToSpawn;
	private Camera thisCamera;

	// Use this for initialization
	void Start()
	{
		thisCamera = Camera.main;
	}

	// Update is called once per frame
	void Update()
	{
		if( Input.GetMouseButtonDown( 0 ) )
		{
			Ray ray = thisCamera.ScreenPointToRay( Input.mousePosition );
			RaycastHit hit;
			if( Physics.Raycast( ray, out hit, 10000.0f ) )
			{
				var go = GameObject.Instantiate( prefabToSpawn, hit.point, Quaternion.identity );
				if( RipplesCommandBuffer.instance != null && RipplesCommandBuffer.instance.bufferIsOn && RipplesCommandBuffer.instance.refreshBuffer )
				{
					RipplesCommandBuffer.instance.particles.Add( go.transform.Find( "SplashRipple" ).GetComponent<ParticleSystem>() );
					RipplesCommandBuffer.instance.particles.Add( go.transform.Find( "SplashRipple/SplashSubRipple" ).GetComponent<ParticleSystem>() );
				}
			}
		}
	}
}
