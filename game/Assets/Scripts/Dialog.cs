using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Dialog : MonoBehaviour {
	
	public RectTransform view;// we tween in relative to what the camera is looking at
	public RectTransform rectTransform;
	public State state;
	public System.Action confirmCallback;
	public System.Action closeCallback;
	public float animDelay = 0.5f;

	private Color backColor;
	private Travel travelIn;
	private Travel travelOut;
	private float animElapsed;

	public static Dialog inst; // singleton per scene, call Init() from your favourite Start()
	public static bool open;

	public static readonly Color CLEAR;

	public class DialogRequest{
		public string name;
		public System.Action confirmCallback;
		public System.Action closeCallback;
		public DialogRequest(string name, System.Action confirmCallback = null, System.Action closeCallback = null) {
			this.name = name;
			this.confirmCallback = confirmCallback;
			this.closeCallback = closeCallback;
		}
	}
	private Queue<DialogRequest> requestQueue;
	public enum State{
		IDLE,
		OPENING,
		OPEN,
		CLOSING
	}
	

	public void Init () {
		inst = this;
		var start = transform.localPosition;
		var end = start + new Vector3(0, -CamCtrl.HEIGHT);
		travelIn = new Travel(transform, end, start, Easing.QuadEaseIn); 
		travelOut = new Travel(transform, start, end, Easing.QuadEaseOut);
		travelIn.SetDelta(0);
		requestQueue = new Queue<DialogRequest>();
		open = false;
		animElapsed = 0;
	}

	// shorthand for Dialog.dialog.Open
	// submit queue as false to turn off popup-chaining
	public static void Show(string str, System.Action confirmCallback = null, System.Action closeCallback = null, bool queue = false) {
		inst.Open(str, confirmCallback, closeCallback, queue);
	}

	public void Open(string name, System.Action confirmCallback = null, System.Action closeCallback = null, bool queue = false){

		if(open){
			if(queue){
				var request = new DialogRequest(name, confirmCallback, closeCallback);
				requestQueue.Enqueue(request);
			}
			return;
		}
		
		open = true;
		this.confirmCallback = confirmCallback;
		this.closeCallback = closeCallback;
		animElapsed = 0;
		travelIn.SetDelta(0);
		// show relevant dialog
		foreach(Transform item in transform) {
			item.gameObject.SetActive(item.name == name);
		}
		state = State.OPENING;
	}

	void FixedUpdate() {
		switch(state) {
			case State.OPENING:
				animElapsed += Time.deltaTime / animDelay;
				if(animElapsed > 1) animElapsed = 1;
				travelIn.SetDelta(animElapsed);
				if(animElapsed == 1) {
					state = State.OPEN;
				}
				break;
			case State.OPEN:
				if(Input.GetKeyDown(KeyCode.Space)) {
					ConfirmClick();
				}
				if(Input.GetKeyDown(KeyCode.Escape)) {
					CloseClick();
				}
				break;
			case State.CLOSING:
				animElapsed += Time.deltaTime / animDelay;
				if(animElapsed > 1) animElapsed = 1;
				travelOut.SetDelta(animElapsed);
				if(animElapsed == 1) {
					open = false;
					if(requestQueue.Count > 0) {
						var request = requestQueue.Dequeue();
						Open(request.name, request.confirmCallback, request.closeCallback);
					} else {
						state = State.IDLE;
					}
				}
				break;
		}
	}
	
	public void CloseClick(){
		if(state != State.OPEN) return;// sanity check
		if(closeCallback != null){
			closeCallback();
		}
		animElapsed = 0;
		state = State.CLOSING;
	}
	
	public void ConfirmClick(){
		if(state != State.OPEN) return;// sanity check
		if(confirmCallback != null) {
			confirmCallback();
		}
		animElapsed = 0;
		state = State.CLOSING;
	}


}
