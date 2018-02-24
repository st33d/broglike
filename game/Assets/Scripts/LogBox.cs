using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/* Classic roguelike log, animated to make it easier to notice new entries */
public class LogBox : MonoBehaviour {

	public Text text;
	public List<string> log;
	public int lines = 3;
	public float lineHeight = 8f;
	public float scrollDelay = 0.25f;
	public Travel travel;

	private List<string> newLines;
	private Vector3 textStart;
	private float scrollElapsed;

	void Awake() {
		textStart = text.transform.localPosition;
		travel = new Travel(
			text.transform,
			text.transform.localPosition + new Vector3(0, lineHeight),
			text.transform.localPosition,
			Easing.Linear
		);
	}

	public void Clear() {
		text.text = "";
		text.transform.localPosition = textStart;
		log = new List<string>();
		newLines = new List<string>();
		scrollElapsed = 0;
	}

	public void Say(string s){
		if(s.IndexOf("\n") > -1) {
			newLines.AddRange(s.Split('\n'));
		} else {
			newLines.Add(s);
		}
	}

	void Update() {
		if(scrollElapsed < 1f) {
			scrollElapsed += Time.deltaTime / scrollDelay;
			if(scrollElapsed >= 1f) scrollElapsed = 1;
			travel.SetDelta(scrollElapsed);
		} else if(newLines.Count > 0) {
			var c = newLines.Count - 1;
			log.Add(newLines[c]);
			newLines.RemoveAt(c);
			var textLines = log.GetRange(
				Mathf.Max(log.Count - lines, 0),
				Mathf.Min(log.Count, lines)
			);
			textLines.Reverse();
			text.text = System.String.Join("\n", textLines.ToArray());
			scrollElapsed = 0;
			travel.SetDelta(scrollElapsed);
		}
	}

}
