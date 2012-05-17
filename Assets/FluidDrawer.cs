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
	
	
	void UpdateTexture()
	{
	
		tex.SetPixels(solver.color);
		
		tex.Apply();
	}
	
	public void DrawPixel(int x, int y)
	{
		index = y*solver.height + x;
		
		solver.colorOld[index] = new Color(100,0,0);		
	}
	
	public void AddForce(int x, int y, Vector2 f)
	{
		index = y*solver.height + x;
		
		solver.uvOld[index] += f;		
	}
	
	public void DebugForces()
	{		
		Vector3 b = renderer.bounds.extents;
		
		float wi = (b.x * 2) / solver.width;
		float hi = (b.z * 2) / solver.height;
		
		Vector3 start = transform.position + new Vector3(b.x, 0, b.z);			

		for(int i=0; i<solver.color.Length; i++)
		{
			int x = i % solver.width;
			int y = (int)Mathf.Floor(i / solver.height);
			
			Vector3 pos = start - new Vector3(x*wi, 0, y*hi);			
			Vector3 dir = pos + new Vector3(solver.uvOld[i].x/2, 0, solver.uvOld[i].y/2);			
			
			Debug.DrawLine(pos, dir, Color.red);	
		}	
		
	}
}