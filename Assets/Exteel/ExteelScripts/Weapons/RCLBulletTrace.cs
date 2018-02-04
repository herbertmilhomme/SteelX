﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCLBulletTrace : MonoBehaviour {

	public GameObject bulletImpact;
	private Rigidbody rb;
	public HUD hud;
	public Camera cam;
	public GameObject Shooter;
	public string ShooterName;
	private int bulletdmg = 100;

	private ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[1];
	private Transform Target;
	private float bulletSpeed = 200;
	private bool isCollided = false;
	ParticleSystem ps ;


	void Start () {
		ps = GetComponent<ParticleSystem>();
		ps.Play();

		GetComponent<Rigidbody> ().velocity = transform.forward * bulletSpeed;
		Destroy(gameObject, 2f);
	}

	void OnParticleCollision(GameObject other){
		Vector3 collisionHitLoc = new Vector3(0,0,0);
		if (isCollided == true || other == Shooter)
			return;
		isCollided = true;
		ps.Stop ();
			
		int numCollisionEvents = GetComponent<ParticleSystem> ().GetCollisionEvents (other, collisionEvents);
		for(int i=0;i < numCollisionEvents;i++) {
			collisionHitLoc = collisionEvents [i].intersection;
			GameObject temp = Instantiate (bulletImpact, collisionHitLoc, Quaternion.identity);
			temp.GetComponent<ParticleSystem> ().Play ();
		}

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);
		for (int i=0;i < hitColliders.Length;i++)
		{
			if(hitColliders[i].gameObject.layer == 8  && hitColliders[i].gameObject.name!=ShooterName){
				hitColliders[i].GetComponent<Transform>().position += transform.forward*5f;
				if (PhotonNetwork.playerName == ShooterName) {
					if (hitColliders[i].gameObject.GetComponent<Combat>().CurrentHP() - bulletdmg<= 0) {
						hud.ShowText (cam, hitColliders [i].transform.position + new Vector3 (0, 5f, 0), "Kill");
					} else {
						hud.ShowText (cam, hitColliders [i].transform.position + new Vector3 (0, 5f, 0), "Hit");
					}
				}
				if(hitColliders[i].GetComponent<PhotonView>().isMine)	//avoid multi-calls
				{
					hitColliders[i].GetComponent<PhotonView>().RPC("OnHit", PhotonTargets.All, bulletdmg, ShooterName); 
					print ("call RCL ONhit with shooterName: " + ShooterName);
				}
			}
			print (hitColliders [i].gameObject.name);
		}
	}

}
