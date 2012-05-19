using UnityEngine;
using System.Collections;

public class FluidMouseInput : MonoBehaviour {
	
	public FluidDrawer fluidDrawer;
	public FluidSolver solver;
	public float mouseForce = -0.05f;
	public Color drawColor;
	
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
		
		solver.AddDensity(uv, drawColor);		
		
		//Apply force
		if(applyForce){
			Vector2 force = lastPos-uv;	
			
			solver.AddForce(uv, force*mouseForce);			
		}
		
		lastPos = uv;
		applyForce = true;
	}
	
	void StopMove(){
		applyForce = false; 		
	}
}
