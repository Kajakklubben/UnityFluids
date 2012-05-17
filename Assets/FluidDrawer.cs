using UnityEngine;
using System.Collections;

public class FluidDrawer : MonoBehaviour {

	public FluidSolver solver;
	
	public Texture2D tex;
	
	int index;
	bool debugForces = true;
	
	void Start () {
	
	}
		
	void Update () {
		
		
		UpdateTexture();
		
		if(debugForces) DebugForces();
	}
	
	
	void UpdateTexture(){
	
		tex.SetPixels(solver.color);
		
		tex.Apply();
	}
	
	public void DrawPixel(int x, int y){
		index = y*solver.height + x;
		
		solver.colorOld[index] = new Color(100,0,0);		
	}
	
	public void AddForce(int x, int y, Vector2 f){
		index = y*solver.height + x;
		
		solver.uvOld[index] += f;		
	}
	
	public void DebugForces(){
		float w = renderer.bounds.extents.x;
		
		float h = renderer.bounds.extents.z;
		
		Vector2 pos = new Vector2(0, 0);
		
		Vector2 dir = new Vector2(0, 0);
		
		//for(int i=0; i<solver.  c in solver.color
			
		//Debug.DrawLine(pos, dir);
	}
}