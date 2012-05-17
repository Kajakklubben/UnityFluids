using UnityEngine;
using System.Collections;

public class FluidDrawer : MonoBehaviour {

	public FluidSolver solver;
	
	public Texture2D tex;
	
	int index;
	public bool debugForces = true;
	
	Color[] colors;
	
	void Start () {
		colors = new Color[64*64];
	}
		
	void Update () {
		
		
		UpdateTexture();
		
		if(debugForces) DebugForces();
	}
	
	
	void UpdateTexture()
	{
		for(int i=0;i<solver.size;i++){
			colors[i].g = solver.dens[i]/255.0f;
		}
		
		tex.SetPixels(colors);
		
		tex.Apply();
	}
	
	public void DrawPixel(int x, int y)
	{
		index = (int)y*FluidSolver.height + x;
		
		solver.dens_prev[index] += 255;		
	}
	
	public void AddForce(int x, int y, Vector2 f)
	{
		index = y*FluidSolver.height + x;
		
		solver.u_prev[index] += f.x;		
		solver.v_prev[index] += f.y;		
	}
	
	public void DebugForces()
	{		
		Vector3 b = renderer.bounds.extents;
		
		float wi = (b.x * 2) / FluidSolver.width;
		float hi = (b.z * 2) / FluidSolver.height;
		
		Vector3 start = transform.position + new Vector3(b.x, 0, b.z);			

		for(int i=0; i<solver.size; i++)
		{
			int x = i % FluidSolver.width;
			int y = (int)Mathf.Floor(i / FluidSolver.height);
			
			Vector3 pos = start - new Vector3(x*wi, 0, y*hi);			
			Vector3 dir = pos + new Vector3(solver.u_prev[i]/2.0f, 0, solver.v_prev[i]/2.0f);			
			
			Debug.DrawLine(pos, dir, Color.red);	
		}	
		
	}
}