using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FluidParticles : MonoBehaviour {
	
	public Transform ParticlePrefab;
	public List<Transform> Particles;
	public FluidSolver Solver;
	public float ParticleSpeed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetMouseButtonDown(1))
		{
			for(int i =0;i<50;i++)
			{
			Vector3 offset = new Vector3(collider.bounds.min.x+Random.Range(0f,1f)*collider.bounds.size.z,3,collider.bounds.min.z+Random.Range(0f,1f)*collider.bounds.size.z);
			Particles.Add( (Transform)Instantiate(ParticlePrefab,offset,Quaternion.identity));

			}
		}
		
		Vector3 vel = Vector3.zero;
		foreach(Transform p in Particles)
		{
			//get index in arrays
			int x = FluidSolver.width-Mathf.RoundToInt((p.position.x-collider.bounds.min.x)/collider.bounds.size.x*FluidSolver.width);
			int y = FluidSolver.height-Mathf.RoundToInt((p.position.z-collider.bounds.min.z)/collider.bounds.size.z*FluidSolver.height);
			x = Mathf.Clamp(x,1,FluidSolver.width-2);
			y = Mathf.Clamp(y,1,FluidSolver.height-2);
			int i = y * FluidSolver.height + (int)x;
			vel.x = -Solver.u[i];		
			vel.z = -Solver.v[i];
			p.position+=vel*ParticleSpeed*Time.deltaTime;
			p.LookAt(p.position+vel);
		}
		
	}
}
