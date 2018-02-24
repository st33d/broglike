using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rog {
	/* Recyclable ParticleEmitters */
	public class FX : MonoBehaviour {

		public static Map map;
		public ParticleSystem particles;
		public SpriteRenderer spriteRenderer;
		public ID id;

		public enum ID {
			BANG, SHOT_HIT
		}

		/* Create an FX at a location,
		 * @sprite: Submit a sprite to create a dummy of the thing getting hit that disappears
		 */
		public void Spawn(ID id, Vector3 pos, Sprite sprite = null) {
			gameObject.SetActive(true);
			this.id = id;
			transform.localPosition = pos;
			particles.Play();
			spriteRenderer.sprite = sprite;
			StartCoroutine(Autodestroy());
		}

		IEnumerator Autodestroy() {
			if(spriteRenderer.sprite != null) {
				yield return new WaitForSeconds(particles.main.duration * 0.5f);
				spriteRenderer.sprite = null;
				yield return new WaitForSeconds(particles.main.duration * 0.5f);
			} else {
				yield return new WaitForSeconds(particles.main.duration);
			}
			Kill();
		}

		public void Kill() {
			if(!enabled) return;
			particles.Stop();
			if(!map.fxPool.ContainsKey(id)) {
				map.fxPool[id] = new Stack<FX>();
			}
			map.fxPool[id].Push(this);
			gameObject.SetActive(false);
		}

	}
}
