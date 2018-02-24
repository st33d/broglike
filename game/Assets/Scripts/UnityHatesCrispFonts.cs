using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Whhhyyyyyyyy */
public class UnityHatesCrispFonts : MonoBehaviour {

	public static UnityHatesCrispFonts inst;

	public List<Font> targets;

	void Awake(){
		targets.ForEach(f => f.material.mainTexture.filterMode = FilterMode.Point);
	}

}
