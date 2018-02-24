using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rog {
	/* Handles player input */
	public class Player : MonoBehaviour {

		public Thing thing;
		public Map map;

		private bool keyDown;

		void Update() {
			var v = new Vector3Int(
				Mathf.CeilToInt(Input.GetAxisRaw("Horizontal")),
				-Mathf.CeilToInt(Input.GetAxisRaw("Vertical"))
				, 0);
			if(!Dialog.open && Input.anyKey) {
				if(map.state == Map.State.WAIT) {
					if(v != Vector3Int.zero) {
						var next = thing.pos + v;
						thing.prevAct = thing.act;
						thing.act = thing.MoveTo(next);
						thing.FaceDir();
						// stop the player from auto-rutting the walls
						if(thing.HumpHump() && keyDown) {
							thing.travel = null;
							return;
						}
						map.DoTurn(Map.ID.GOOD);

					} else if(Input.GetKey(KeyCode.Space)) {
						// fire
						if(map.power.value > 0) {
							map.power.value--;
							thing.act = thing.Shoot(thing.dir);
							map.DoTurn(Map.ID.GOOD);
						}
					}
				}
				keyDown = true;
			} else {
				keyDown = false;
			}
		}
	}
}
