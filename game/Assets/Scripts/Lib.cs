using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Prefab and resource lookup */
public class Lib : ScriptableObject {

	[Header("Things")]
	public GameObject player;
	public GameObject wall;
	public GameObject hardWall;
	public GameObject mon;
	public GameObject shot;
	public GameObject loot;

	[Header("FX")]
	public GameObject bang;
	public GameObject shotHit;

	[Header("Sprites")]
	public Sprite monSpawner;
	public Sprite monHunter;
	public Sprite monFlee;

}
