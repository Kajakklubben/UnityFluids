using UnityEngine;
using System.Collections;

public class FluidAdder : MonoBehaviour {
	
	public bool emitForces = true;	
	public Vector2 constantForceAdded;
	public float dynamicForceForwards = 0;
	public float dynamicForceBackwards = 0;
	public Transform follow;
		
	
	public bool emitDensity = true;
	public Color color;
	public bool emitLargeDensity = false;
	//public int densityArea;
	
	FluidSolver solver;
	Vector2 pos;	
	Transform t;
	Collider c;
	Vector2 lastPos;	

	void Start () 
	{
		solver = GameObject.Find("fluids").GetComponent("FluidSolver") as FluidSolver;	
		t = this.transform;
		c = solver.collider;
	}
	
	void Update () 
	{		
		if(follow)
		{
			t.position = new Vector3(follow.position.x, t.position.y, follow.position.z);
		}
		
		FindPosition();
				
		if(emitForces) EmitForces();	
				
		if(emitDensity) EmitDensity();
		
		lastPos = pos;
	}
			
	void EmitForces()
	{
			
		//apply constant force
		if(constantForceAdded!=Vector2.zero)
		{		
			solver.AddForce(pos, constantForceAdded); 
		}
		
		//apply force depending on movement
		if(lastPos!=Vector2.zero && lastPos!=pos)
		{
		
			Vector2 movement = lastPos-pos;
			
			Vector2 forwardsForce = movement * -1 * dynamicForceForwards;
			
			Vector2 backwardsForce = movement * dynamicForceBackwards;
			
			if(forwardsForce != Vector2.zero)
			{				
				solver.AddForce(pos, forwardsForce);
			}
			
			if(backwardsForce != Vector2.zero)
			{				
				solver.AddForce(pos, backwardsForce);
			}			
		}
	}
	
	void EmitDensity()
	{		
		solver.AddDensity(pos, color);
		solver.AddDensity(new Vector2(pos.x-1, pos.y), color);
		solver.AddDensity(new Vector2(pos.x+1, pos.y), color);
		solver.AddDensity(new Vector2(pos.x, pos.y+1), color);
		solver.AddDensity(new Vector2(pos.x, pos.y-1), color);
	}
	
	void FindPosition()
	{
		int x = FluidSolver.width-Mathf.RoundToInt((t.position.x-c.bounds.min.x)/c.bounds.size.x*FluidSolver.width);
		int y = FluidSolver.height-Mathf.RoundToInt((t.position.z-c.bounds.min.z)/c.bounds.size.z*FluidSolver.height);
		
		x = Mathf.Clamp(x,1,FluidSolver.width-2);
		y = Mathf.Clamp(y,1,FluidSolver.height-2);
		
		pos.x = x;
		pos.y = y;		
	}
}
