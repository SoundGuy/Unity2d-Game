﻿using UnityEngine;
using System.Collections;

public class Canon : Liveable
{
		//canon types
		public enum CanonType{
			Regular,
			AutoAim
		}

		//public
		public CanonType type;
		[Range(0.001f, 0.1f)] 
		public float bulletSpeed = 0.001f;
		[Range(5, 0.1f)] 
		public float fireRate = 1f;		
		public GameObject ammoPrefab;				
		
		//the current target for the auto aim
		GameObject target;
		GameObject bulletSpawnerLayer;
		float nextShoot;
		
		void Start ()
		{
			nextShoot += fireRate;
			Game.Canons.Add(this);			
			bulletSpawnerLayer = GameObject.FindGameObjectWithTag("SPAWNS"); //find the layer on which we will be spawning our bullets
		}
	
		void OnDestroy ()
		{
			Game.Canons.Remove (this);
		}

		void Update ()
		{
			if(type == CanonType.Regular)
			{
				rotateToPosition(Input.mousePosition,this.transform.position);
				if (Input.GetMouseButtonDown(0)) Fire(Input.mousePosition); //TODO: change to touch
			} 
			else if(type == CanonType.AutoAim && target){
			rotateToPosition(Camera.main.WorldToScreenPoint(target.transform.position),this.transform.position);				
				if (nextShoot < Time.time){
				Fire(Camera.main.WorldToScreenPoint(target.transform.position));
					nextShoot = Time.time + fireRate;
				}
			}
		}

		//fire 
		private void Fire (Vector3 pos)
		{
			pos.z = transform.position.z - Camera.main.transform.position.z;
			pos = Camera.main.ScreenToWorldPoint (pos);
			Quaternion q = Quaternion.FromToRotation (Vector3.up, pos - transform.position);
			GameObject go = (GameObject)Instantiate (ammoPrefab, this.transform.position, q);
			go.transform.parent = bulletSpawnerLayer.transform;
			go.rigidbody2D.AddForce (go.transform.up * bulletSpeed);
		}

		private void rotateToPosition(Vector3 mousePos,Vector3 originPos)
		{
			Vector3 canonWorldPos = Camera.main.WorldToScreenPoint (originPos);
			mousePos.x = mousePos.x - canonWorldPos.x;
			mousePos.y = mousePos.y - canonWorldPos.y;
			float angle = Mathf.Atan2 (mousePos.y, mousePos.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, angle));
		}
	
		void OnTriggerEnter2D(Collider2D other) 
		{
			if(type == CanonType.AutoAim && other.tag == "ENEMY" && !target){
			target = other.gameObject;
			}
		}
}
