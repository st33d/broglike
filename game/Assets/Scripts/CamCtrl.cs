using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rog;

/* Basic scrolling, camera shake, screen measuring */
public class CamCtrl : MonoBehaviour {

	public Camera cam;
	public Map map;
	// offsets the camera and clamps at map limits (accomodates our clunky roguelike UI)
	public RectTransform view;
	public Vector3 real;
	public Vector3 current;
	public Vector3 target;
	public bool lockX;
	public bool lockY;
	
	private Vector3 shakeOffset;
	private int shakeDirX;
	private int shakeDirY;

	public static Dictionary<Corner, Vector2> corners;
	public static Vector2 SIZE;
	public static float WIDTH;
	public static float HEIGHT;
	public static float HALF_WIDTH;
	public static float HALF_HEIGHT;
	public const float INTERPOLATION = 0.4f;
	public enum Corner {
		TR,
		TL,
		BR,
		BL
	}

	public static void Init() {
		corners = new Dictionary<Corner, Vector2>{
			{Corner.TR, new Vector2(Camera.main.orthographicSize * Screen.width / Screen.height, Camera.main.orthographicSize)},
			{Corner.TL, new Vector2(-Camera.main.orthographicSize * Screen.width / Screen.height, Camera.main.orthographicSize)},
			{Corner.BR, new Vector2(Camera.main.orthographicSize * Screen.width / Screen.height, -Camera.main.orthographicSize)},
			{Corner.BL, new Vector2(-Camera.main.orthographicSize * Screen.width / Screen.height, -Camera.main.orthographicSize)}
		};
		WIDTH = (corners[Corner.TR] - corners[Corner.TL]).x;
		HEIGHT = (corners[Corner.TR] - corners[Corner.BR]).y;
		HALF_WIDTH = WIDTH * 0.5f;
		HALF_HEIGHT = HEIGHT * 0.5f;
		SIZE = new Vector2(WIDTH, HEIGHT);

		Debug.Log("cam init : "+WIDTH+" "+HEIGHT);
	}

	void Start () {
		cam.transparencySortMode = TransparencySortMode.Orthographic;// sort by z property
		Debug.Log("view init : "+view.rect+" "+view.anchoredPosition);
	}

	public void SetTarget(Vector3Int p) {
		target = new Vector3(
			(p.x + 0.5f) * Thing.SCALE.x, -(p.y + 0.5f) * Thing.SCALE.y
		) - (Vector3)view.anchoredPosition;
		ClampTarget();
	}

	public void SetTarget(GameObject obj, Vector3 offset) {
		target = (obj.transform.localPosition + offset) - (Vector3)view.anchoredPosition;
		ClampTarget();
	}

	private void ClampTarget() {
		var mapSize = new Vector2(map.bounds.size.x * Thing.SCALE.x, map.bounds.size.y * Thing.SCALE.y);
		if(mapSize.x >= view.rect.width && !lockX) {
			target.x = Mathf.Max(target.x, view.rect.width * 0.5f - view.anchoredPosition.x);
			target.x = Mathf.Min(target.x, mapSize.x - (view.rect.width * 0.5f + view.anchoredPosition.x));
		} else {
			target.x = mapSize.x * 0.5f;
		}
		if(mapSize.y >= view.rect.height && !lockY) {
			target.y = Mathf.Min(target.y, -view.rect.height * 0.5f - view.anchoredPosition.y);
			target.y = Mathf.Max(target.y, -mapSize.y + (view.rect.height * 0.5f - view.anchoredPosition.y));
		} else {
			target.y = -mapSize.y * 0.5f;
		}
	}

	public void SkipPan() {
		real = target;
		current = new Vector3(
			Mathf.Floor(real.x),
			Mathf.Floor(real.y),
			cam.transform.localPosition.z
		);
		cam.transform.localPosition = current;
	}

	// Update is called once per frame
	void Update () {
		Vector3 dist = (target - real) * INTERPOLATION;
		real += dist;
		Vector3 v = real + shakeOffset;
		current = new Vector3(
			Mathf.Floor(v.x),
			Mathf.Floor(v.y),
			cam.transform.localPosition.z
		);
		cam.transform.localPosition = current;
		UpdateShakes();
	}

	/* Shake the screen in any direction */
	public void Shake(Vector3Int v) {
		// ignore lesser shakes
		if(Mathf.Abs(v.x) > Mathf.Abs(shakeOffset.x)) {
			shakeOffset.x = v.x;
			shakeDirX = v.x > 0 ? 1 : -1;
		}
		if(Mathf.Abs(v.y) > Mathf.Abs(shakeOffset.y)) {
			shakeOffset.y = v.y;
			shakeDirY = v.y > 0 ? 1 : -1;
		}
	}

	public void ShakeDist(Vector3Int a, Vector3Int b, int distMax, int strength) {
		Shake(new Vector3Int(0, (int)(DistToModifier(a, b, distMax) * strength), 0));
	}

	/* Create a modifier based on the distance to a Pos */
	public float DistToModifier(Vector3Int a, Vector3Int b, int distMax) {
		float dist = (b - a).magnitude;
		if(dist < distMax) {
			return (distMax - dist) * (1.0f / distMax);
		}
		return 0;
	}

	/* resolve the shake */
	public void UpdateShakes() {
		if(shakeOffset.y != 0) {
			shakeOffset.y = -shakeOffset.y;
			if(shakeDirY == 1 && shakeOffset.y > 0) shakeOffset.y--;
			if(shakeDirY == -1 && shakeOffset.y < 0) shakeOffset.y++;
		}
		if(shakeOffset.x != 0) {
			shakeOffset.x = -shakeOffset.x;
			if(shakeDirX == 1 && shakeOffset.x > 0) shakeOffset.x--;
			if(shakeDirX == -1 && shakeOffset.x < 0) shakeOffset.x++;
		}
	}
}
