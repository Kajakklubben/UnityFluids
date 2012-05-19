using UnityEngine;
using System.Collections;

public class FluidDrawer : MonoBehaviour {

	public FluidSolver solver;
	
	public Texture2D tex;
	
	public bool debugForces = false;
	
	public float debugScale = 5.0f;
	
	int index;
	
//	Color[] colors;
	
	void Start () {
//		colors = new Color[solver.GetSize()];	
		solver.SetSize(tex.width ,tex.height);
	}
		
	void Update () {
		
		
		UpdateTexture();
		
		if(debugForces) DebugForces();
	}
	
	
	void UpdateTexture()
	{
		//for(int i=0;i<solver.GetSize();i++){
			/*float y = Mathf.Round(i/FluidSolver.width);
			y /= FluidSolver.height;
			
			float x = i%FluidSolver.width;
			x /= FluidSolver.width;
			
			colors[i].b = solver.dens[i]/255.0f*y;
			colors[i].r = solver.dens[i]/255.0f*x;			
			colors[i].a = solver.dens[i]/255.0f;*/
		//	colors[i] = solver.dens[i];
		//}
		
		tex.SetPixels(solver.dens);
		
		tex.Apply();
	}
	
	public void DebugForces()
	{		
		Vector3 b = renderer.bounds.extents;
		
		float wi = (b.x * 2) / FluidSolver.width;
		float hi = (b.z * 2) / FluidSolver.height;
		
		Vector3 start = transform.position + new Vector3(b.x, 0, b.z);			

		for(int i=0; i<solver.GetSize(); i++)
		{
			int x = i % FluidSolver.width;
			int y = (int)Mathf.Floor(i / FluidSolver.height);
			
			Vector3 pos = start - new Vector3(x*wi, 0, y*hi);			
			Vector3 dir = pos + new Vector3(solver.u[i]*debugScale, 0, solver.v[i]*debugScale);							
			
			Debug.DrawLine(pos, dir, Color.red);	
		}	
		
	}
}