using UnityEngine;
using System.Collections;

public class FluidDrawer : MonoBehaviour {

	public FluidSolver solver;
	
	public Texture2D tex;
	
	int index;
		
	void Start () {
	
	}
		
	void Update () {
		
		
		UpdateTexture();
	}
	
	
	void UpdateTexture(){
		
		tex.SetPixels(solver.color);
		
		tex.Apply();
	}
	
	public void DrawPixel(int x, int y){
		index = y*solver.height + x;
		
		solver.color[index] = new Color(100,0,0);
		
	}
}