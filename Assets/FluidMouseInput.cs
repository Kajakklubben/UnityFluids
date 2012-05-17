using UnityEngine;
using System.Collections;

public class FluidMouseInput : MonoBehaviour {
	
	public FluidDrawer fluidDrawer;
	public FluidSolver solver;
	
	Vector2 lastPos;
	bool applyForce;
	
	void Start () {
	
	}
	
	void Update () {
		
		//Is the mouse down ?
		if (!Input.GetMouseButton (0)){
			StopMove();			
			return;	
		}
		
		
		//Are we hitting an object ?
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);	
		
		RaycastHit hit; 
		
		if(!Physics.Raycast(ray, out hit)){
			StopMove();		
			return;
		}
			
		
		//Does the object have renderer, material and texture ?
  		Renderer renderer = hit.collider.renderer;
		
		MeshCollider meshCollider = hit.collider as MeshCollider;
		
   		if (renderer == null || renderer.sharedMaterial == null ||renderer.sharedMaterial.mainTexture == null || meshCollider == null){
			StopMove();
			return;
		}
				
		//Draw a pixel where we hit the object
		Texture2D tex = renderer.material.mainTexture as Texture2D;
		
		Vector2 uv = hit.textureCoord;    

	    //Draw color
		uv.x *= tex.width;
	    uv.y *= tex.height;	
		fluidDrawer.DrawPixel((int)uv.x, (int)uv.y);
		
		//Apply force
		if(applyForce){
			Vector2 force = uv-lastPos;					
			fluidDrawer.AddForce((int)uv.x, (int)uv.y, force);
		}
		
		lastPos = uv;
		applyForce = true;
	}
	
	void StopMove(){
		applyForce = false; 		
	}
}
