using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Some text with a number next to it */
public class ScoreText : MonoBehaviour {

	public Text text;
	public string prefix = "score:";
	public int digits = 5;

	[SerializeField] private int _value = 0;
	
	public int value {
		get { return _value; }
		set {
			_value = value;
			text.text = prefix + _value.ToString("D"+digits);
		}
	}

}
