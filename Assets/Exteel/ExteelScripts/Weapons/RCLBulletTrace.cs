﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCLBulletTrace : MonoBehaviour {

	public GameObject bulletImpact;
	private Rigidbody rb;
	public HUD hud;
	public Camera cam;
	public GameObject Shooter;

	private ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[1];
	private Transform Target;
	private float bulletSpeed = 200;
	private bool isCollided = false;

	void Start () {
		ParticleSystem ps = GetComponent<ParticleSystem>();
		ps.Play();

		GetComponent<Rigidbody> ().velocity = transform.forward * bulletSpeed;
		Destroy(gameObject, 2f);
	
	}

	void Update(){
		

	}

	void OnParticleCollision(GameObject other){
		Vector3 collisionHitLoc = new Vector3(0,0,0);
		if (isCollided == true || other == Shooter)
			return;
		isCollided = true;

		GetComponent<ParticleSystem> ().Stop ();
			
		int numCollisionEvents = GetComponent<ParticleSystem> ().GetCollisionEvents (other, collisionEvents);
		int i = 0;
		while (i < numCollisionEvents) {
			collisionHitLoc = collisionEvents [i].intersection;

				//play impact
			GameObject temp = Instantiate (bulletImpact, collisionHitLoc, Quaternion.identity);
			temp.GetComponent<ParticleSystem> ().Play ();
			i++;
		}
		if (other.layer == 8) { // collides player
			other.GetComponent<Transform>().position += transform.forward*5f;
			print ("Forced move.");
			hud.ShowText (cam, collisionHitLoc, "Hit");
		}else{
			//collides environment
			//do nothing currently
		}

			Destroy (gameObject, 0.5f);

	}
}