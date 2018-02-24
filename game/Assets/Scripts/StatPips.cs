using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* A representation of property with tokens */
public class StatPips : MonoBehaviour {

	public GameObject pipFab;
	public Sprite empty;
	public Sprite full;
	public List<Image> pips;

	
	[SerializeField] private int _total;
	public int total {
		get { return _total; }
		set {
			// clear workshop
			foreach(Transform childTransform in transform) {
				Destroy(childTransform.gameObject);
			}
			pips.Clear();
			_total = Mathf.Max(value, 1);
			if(_value > _total) _value = _total;
			for(int i = 0; i < _total; i++) {
				var obj = Instantiate(pipFab, transform);
				var img = obj.GetComponent<Image>();
				img.sprite = i <= _value - 1 ? full : empty;
				pips.Add(img);
			}
		}
	}

	[SerializeField] private int _value;
	public int value {
		get { return _value; }
		set {
			_value = Mathf.Min(_total, value);
			for(int i = 0; i < pips.Count; i++) {
				pips[i].sprite = i <= _value - 1 ? full : empty;
			}
		}
	}

	public void SetPip(int index, Sprite sprite) {
		pips[index].sprite = sprite;
	}
}
