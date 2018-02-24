using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/* Animated power bar */
public class PowerBar : MonoBehaviour {

	public Image bar;
	public Image back;
	public float animDelay = 0;
	public RectOffset padding;
	public bool onPixel;
	public bool barIsValue = true;

	private float delta;
	private float animValue;
	private float animElapsed = 1;
	
	[SerializeField] private float _total;
	public float total {
		get { return _total; }
		set {
			_total = value;
			if(_total == 0) _total = 1;// sanity check
			animValue = _value;
			delta = 0;
			UpdateBar();
		}
	}

	[SerializeField] private float _value;
	public float value {
		get { return _value; }
		set {
			_value = Mathf.Min(value, _total);
			delta = _value - animValue;
			if(animDelay > 0) {
				animElapsed = 0;
			} else {
				UpdateBar();
			}
		}
	}

	public void Init(float total, float value) {
		_total = total;
		_value = animValue = value;
		animElapsed = 1f;
		UpdateBar();
	}


	// Update is called once per frame
	void Update () {
		if(animDelay > 0) {
			if(animElapsed < 1f) {
				animElapsed += Time.deltaTime / animDelay;
				if(animElapsed >= 1f) {
					animElapsed = 1f;
					animValue = _value;
				} else {
					animValue = (_value - delta) + animElapsed * delta;
				}
				UpdateBar();
			}
		}
	}

	public void UpdateBar(){
		float w = back.rectTransform.sizeDelta.x;
		w -= padding.horizontal;
		if(onPixel) w = Mathf.Ceil(w);
		if(barIsValue){
			w *= (1f / _total) * animValue;
			bar.transform.localPosition = new Vector3(
				-back.rectTransform.sizeDelta.x * 0.5f + w * 0.5f + padding.left, 0
			);
		} else {
			w *= 1f - ((1f / _total) * animValue);
			bar.transform.localPosition = new Vector3(
				back.rectTransform.sizeDelta.x * 0.5f - (w * 0.5f + padding.left), 0
			);
		}
		bar.rectTransform.sizeDelta = new Vector2(w, back.rectTransform.sizeDelta.y - padding.vertical);
	}
}
