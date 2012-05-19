using UnityEngine;
using System.Collections;

public class FluidDrawer : MonoBehaviour {

	public FluidSolver solver;
	
	public Texture2D tex;
	public Texture2D buffer;
	public RenderTexture renderTex;
	
	public bool debugForces = false;
	
	public float debugScale = 5.0f;
	public float movementThreshold = 0.0010f;
	public float FluidFadeOut;
	public float AddRate = 0.05f;
	
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
		RenderTexture.active = renderTex;
		tex.SetPixels(solver.dens);
		
		buffer.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
		
		float velX,velY;
		int i = 0;
		foreach(Color c in solver.dens)
		{
			int y = Mathf.RoundToInt(i/FluidSolver.width);
			int x = i % FluidSolver.width;
			
			
			solver.dens[i]*=1f-AddRate;
			solver.dens[i] += buffer.GetPixel(x,y)*AddRate;
			
			velX = Mathf.Abs(solver.u[i]);
			velY = Mathf.Abs(solver.v[i]);
			
			if(velX<movementThreshold && velY<movementThreshold)
				solver.dens[i].a = Mathf.Clamp(solver.dens[i].a-Time.deltaTime*FluidFadeOut,0f,1f);
			
			i++;
		}
		
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