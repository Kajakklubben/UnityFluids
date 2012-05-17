using UnityEngine;
using System.Collections;

public class FluidMouseInput : MonoBehaviour {
	
	public FluidDrawer fluidDrawer;
	
	void Start () {
	
	}
	
	void Update () {
		
		//Is the mouse down?
		if (!Input.GetMouseButton (0)) return;	
		
		
		//Are we hitting an object?
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);	
		
		RaycastHit hit; 
		
		if(!Physics.Raycast(ray, out hit)) return;
			
		
		//Does the object have renderer, material and texture?
  		Renderer renderer = hit.collider.renderer;
		
		MeshCollider meshCollider = hit.collider as MeshCollider;
		
   		if (renderer == null || renderer.sharedMaterial == null ||renderer.sharedMaterial.mainTexture == null || meshCollider == null) return;
		
		
		//Draw a pixel where we hit the object
		Texture2D tex = renderer.material.mainTexture as Texture2D;
		
		Vector2 pixelUV = hit.textureCoord;    

	    pixelUV.x *= tex.width;
		
	    pixelUV.y *= tex.height;	
		
		
		fluidDrawer.DrawPixel((int)pixelUV.x, (int)pixelUV.y);
	}
}
