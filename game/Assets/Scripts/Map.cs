using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Rog {
	/* Game Engine, Prefab factory, AI, reference switch-board */
	public class Map : MonoBehaviour {

		public State state;
		public BoundsInt bounds;
		public float turnDelay = 0.25f;// how long in seconds does one team take to animate?
		public float turnElapsed = 0f;// how far into this turn we are: 0-1f
		public Player player;// listens for player input
		public CamCtrl camCtrl;// Camera stuff
		public Transform fxParent;// Just for the sake of tidyness
		public List<Thing> goodies;// player actors
		public List<Thing> baddies;// enemy actors
		public List<Thing> spawned;// new actors
		public List<Thing> graveyard;// dead actors (awaiting garbage collection / resurrection)
		public SpriteRenderer floor;// stretchy repeating floor texture
		public Dialog dialog;// popups
		public LogBox logBox;// "Welcome Rogue..."
		public ScoreText score;
		public StatPips health;
		public PowerBar power;
		public int roundCount;// number of rounds elapsed, a round is a complete goodie turn and baddie turn
		public float gameOverDelay = 1f;


		public Dictionary<Vector3Int, Thing> things;// map of entities
		public Dictionary<Vector3Int, ID> ids;// enums for creating initial map
		public Dictionary<Vector3Int, int> paths;// current pathfinding flood
		public Dictionary<ID, Dictionary<Vector3Int, int>> teamPaths;// floods for teams / ids
		public Dictionary<Vector3Int, List<Thing>> triggers;// non-solid traps, activated by Things
		public System.Random rng;// Avoiding UnityEngine.Random for fresher RNG

		public Lib lib;// prefabs, sprites, etc.

		// recycling
		public Dictionary<ID, Stack<Thing>> pool;
		public Dictionary<FX.ID, Stack<FX>> fxPool;

		public const int PATH_STEP = 10;// AI distance is scaled up to allow small adjustments
		public const int SHAKE_DIST_MAX = 15;// sources of shakes get weaker further away

		public enum State {
			WAIT, GOOD_TURN, BAD_TURN, END_ROUND, GAME_OVER
		}

		public static ID PLAYER_ID = ID.PLAYER | ID.GOOD;

		[Flags]
		public enum ID {
			// counting bitwise - extracted into countId
			// this also allows packing an arbitrary enum into the ID
			C1 = 1 << 0
			, C2 = 1 << 1
			, C3 = 1 << 2
			, C4 = 1 << 3
			, C5 = 1 << 4
			// prefab type
			, WALL = 1 << 5
			, HARD_WALL = 1 << 6// indestructible
			, PLAYER = 1 << 7
			, MON = 1 << 8
			, SHOT = 1 << 9
			, LOOT = 1 << 10
			// team
			, GOOD = 1 << 11
			, BAD = 1 << 12
			// variant super class?
			, BOSS = 1 << 14
			//,= 1 << 
		}

		public const ID PREFAB_MASK = ID.WALL | ID.HARD_WALL | ID.PLAYER | ID.MON | ID.SHOT | ID.LOOT;
		public const ID COUNT_MASK = ID.C1 | ID.C2 | ID.C3 | ID.C4 | ID.C5;
		public const ID TEAM_MASK = ID.GOOD | ID.BAD;
		public const ID TRIGGER_MASK = ID.SHOT | ID.LOOT;
		public const ID ACTOR_MASK = PREFAB_MASK & (~TRIGGER_MASK);

		public static Vector3Int playerStart = new Vector3Int(5, 5, 0);
		public static Vector3Int[] DIRS = new Vector3Int[] {
		Thing.UP, Thing.RIGHT, Thing.LEFT, Thing.DOWN
	};

		void Start() {
			Thing.map = this; // eh, cannae be fucked with maintaining multiple maps
			FX.map = this;
			CamCtrl.Init();
			dialog.Init();

			Dialog.Show("Instructions");

			// go go go
			GameInit();
		}

		public void GameInit() {
			// take no prisoners
			foreach(Transform childTransform in transform) {
				Destroy(childTransform.gameObject);
			}
			// obj pooling
			pool = new Dictionary<ID, Stack<Thing>>();
			fxPool = new Dictionary<FX.ID, Stack<FX>>();

			goodies.Clear();
			baddies.Clear();
			spawned.Clear();
			graveyard.Clear();
			logBox.Clear();
			rng = new System.Random();
			var size = 21;
			playerStart.x = size / 2;
			playerStart.y = size / 2;
			bounds.size = new Vector3Int(size, size, 1);

			var floorSize = new Vector2(bounds.size.x * Thing.SCALE.x, bounds.size.y * Thing.SCALE.y);
			floor.size = floorSize;
			floor.transform.localPosition = new Vector3(floorSize.x * 0.5f, -floorSize.y * 0.5f);
			ids = new Dictionary<Vector3Int, ID>();
			things = new Dictionary<Vector3Int, Thing>();
			teamPaths = new Dictionary<ID, Dictionary<Vector3Int, int>> {
			{ID.GOOD, new Dictionary<Vector3Int, int>() },
			{ID.BAD, new Dictionary<Vector3Int, int>() }
		};
			triggers = new Dictionary<Vector3Int, List<Thing>>();

			// generate level
			Fill(bounds, ID.LOOT | ID.GOOD, 0.05f);
			//Fill(bounds, ID.MON | ID.BAD, 0.1f);
			Fill(bounds, ID.WALL, 0.2f);
			Fill(bounds, ID.HARD_WALL, 0.1f);
			ids[playerStart] = PLAYER_ID;
			// it's a monster, it's a baddie, and it's the spawner variant
			ids[playerStart + new Vector3Int(5, 0, 0)] = ID.MON | ID.BAD | (ID)Thing.Mon.SPAWNER;

			// convert
			foreach(var pos in ids.Keys) {
				GetMapThing(pos);
			}
			health.total = player.thing.hpTotal;
			health.value = player.thing.hp;
			score.value = 0;
			power.Init(10, 5);
			camCtrl.SetTarget(playerStart);
			camCtrl.SkipPan();
			state = State.WAIT;

			logBox.Say("Welcome Rogue.");
		}

		void FixedUpdate() {

			// cancel any game logic till pop ups are closed
			if(Dialog.open) {
				return;
			}

			switch(state) {
				case State.WAIT:
					// DoTurn(ID.GOOD) called from Player
					break;
				// player and their allies animate
				case State.GOOD_TURN:
					turnElapsed += Time.fixedDeltaTime / turnDelay;
					if(turnElapsed >= 1f) turnElapsed = 1f;
					player.thing.UpdateAct();
					foreach(var t in goodies) {
						if(t.hp > 0) t.UpdateAct();
					}
					// end of animation
					if(turnElapsed == 1f) {
						turnElapsed = 0;
						if(player.thing.act == Thing.Act.NOPE) {
							state = State.WAIT;
							break;
						}
						EndTeam(ID.GOOD);
						if(player.thing.hp <= 0) {
							GameOver();
							goto case State.GAME_OVER;
						}
						DoTurn(ID.BAD);
					}
					break;
				// enemies animate
				case State.BAD_TURN:
					turnElapsed += Time.fixedDeltaTime / turnDelay;
					if(turnElapsed >= 1f) turnElapsed = 1f;
					foreach(var t in baddies) {
						if(t.hp > 0) t.UpdateAct();
					}
					// end of animation
					if(turnElapsed == 1f) {
						turnElapsed = 0;
						EndTeam(ID.BAD);
						goto case State.END_ROUND;
					}
					break;
				// clean up
				case State.END_ROUND:
					if(player.thing.hp <= 0) {
						GameOver();
						goto case State.GAME_OVER;
					}
					roundCount++;
					state = State.WAIT;
					break;
				case State.GAME_OVER:
					turnElapsed += Time.fixedDeltaTime / gameOverDelay;
					if(turnElapsed >= 1f) {
						Dialog.Show("GameOver", GameInit);
					}
					break;
			}

			if(player.thing.act != Thing.Act.NOPE) {
				camCtrl.SetTarget(player.gameObject, new Vector3());
			}
		}

		/* Call EndAct on our team and empty the graveyard (traps may have been triggered) */
		public void EndTeam(ID team) {
			switch(team) {
				case ID.GOOD:
					player.thing.EndAct();
					foreach(var t in goodies) {
						t.EndAct();
					}
					break;
				case ID.BAD:
					foreach(var t in baddies) {
						t.EndAct();
					}
					break;
			}
			EmptyGraveyard();
		}

		public void GameOver() {
			turnElapsed = 0f;
			state = State.GAME_OVER;

			// hacky game over text update
			var gameOverTextTransform = dialog.transform.Find("GameOver/Score");
			var text = gameOverTextTransform.GetComponent<UnityEngine.UI.Text>();
			text.text = score.value.ToString("D"+score.digits);
		}

		/* Run AI, negotiate for space, decide movements and animations
		 * Iterations run backwards so we can change teams */
		public void DoTurn(ID team) {
			switch(team) {
				case ID.GOOD:
					FloodPaths(baddies, teamPaths[team], ID.WALL | ID.HARD_WALL, ID.MON);
					// player has already performed StartAct
					if(player.thing.act != Thing.Act.NOPE) {
						for(int i = goodies.Count - 1; i > -1; i--) {
							var thing = goodies[i];
							if(thing.hp > 0) thing.StartAct();
						}
					}
					state = State.GOOD_TURN;
					break;
				case ID.BAD:
					var list = new List<Thing>(goodies);
					list.Add(player.thing);
					FloodPaths(list, teamPaths[team], ID.WALL | ID.HARD_WALL, ID.PLAYER);
					for(int i = baddies.Count - 1; i > -1; i--) {
						var thing = baddies[i];
						if(thing.hp > 0) thing.StartAct();
					}
					state = State.BAD_TURN;
					break;
			}
			foreach(var t in spawned) {
				if(t.team == ID.GOOD) {
					goodies.Add(t);
				} else if(t.team == ID.BAD) {
					baddies.Add(t);
				}
				if(t.team == team) {
					if(t.hp > 0) t.StartAct();
				}
			}
			spawned.Clear();
			EmptyGraveyard();
			turnElapsed = 0f;
		}

		/* Pool dead objects */
		public void EmptyGraveyard() {
			foreach(var t in graveyard) {
				t.Kill();
				if(t.team == ID.GOOD) {
					goodies.Remove(t);
				} else if(t.team == ID.BAD) {
					baddies.Remove(t);
				}
			}
			graveyard.Clear();
		}

		/* AI - Hill-climbing, declare team positions as 0 and flood distances outwards, stops at "steps" iterations
		 * 
		 * @targets: A list of potential Things to flood from
		 * @paths: Set the current path to fill (shows up in editor)
		 * @wallMask: A mask of IDs that cannot be flooded through
		 * @targetMask: A mask of IDs to filter out targets to flood from (eg: just the player, no projectiles)
		 * @steps: Max distance of flood walk, stops when bounds are exhausted.
		 * @trail: Encourages kiting AI (eg: player's last 8 steps)
		 */
		public void FloodPaths(List<Thing> targets, Dictionary<Vector3Int, int> paths, ID wallMask, ID targetMask, int steps = 8, List<Vector3Int> trail = null) {
			var q = new Queue<Vector3Int>();
			this.paths = paths;
			paths.Clear();
			foreach(var t in targets) {
				if((t.id & targetMask) != 0) {
					q.Enqueue(t.pos);
					paths[t.pos] = 0;
				}
			}
			int d = 0;
			while(q.Count > 0 && steps-- > 0) {
				int count = q.Count;
				while(count-- > 0) {
					Vector3Int n = q.Dequeue();
					for(int i = 0; i < DIRS.Length; i++) {
						Vector3Int p = n + DIRS[i];
						if(bounds.Contains(p)) {
							if(!things.ContainsKey(p) || (things[p].id & wallMask) == 0) {
								if(!paths.ContainsKey(p)) {
									paths[p] = d + PATH_STEP;
									q.Enqueue(p);
								}
							}
						}
					}
				}
				d += PATH_STEP;// increase by a big step and we can subtract from it to guide AI
			}
			// apply an optional modifier to the distance map based on the path the player has walked
			// this facilitates kiting for the player as opposed to trying to brute force the AI till it rotates to fit
			if(trail != null) {
				for(int i = trail.Count - 2; i > -1; i--) {// skips target current position
					Vector3Int p = trail[i];
					if(paths.ContainsKey(p)) {
						paths[p] -= i + 1;
					}
				}
			}
		}

		public void Fill(BoundsInt area, ID id, float density = 1) {
			Iterate(area, (Vector3Int pos) => {
				if(density == 1 || rng.NextDouble() <= density) ids[pos] = id;
			});
		}

		/* Create an entity from ids lookup table */
		public void GetMapThing(Vector3Int pos, bool spawn = false) {
			// fetch id
			var id = (ID)0;
			ids.TryGetValue(pos, out id);
			if(id != 0) {
				GetThing(id, pos, spawn);
			}
		}

		/* Create an entity */
		public Thing GetThing(ID id, Vector3Int pos, bool spawn = false) {
			var team = (id & TEAM_MASK);
			var fabId = (id & PREFAB_MASK);
			GameObject obj = null;
			Thing thing = null;
			if(pool.ContainsKey(fabId) && pool[fabId].Count > 0) {
				thing = pool[fabId].Pop();
				thing.gameObject.SetActive(true);
				switch(fabId) {
					case 0: break;
					case ID.PLAYER:
						player = obj.GetComponent<Player>();
						player.map = this;
						thing.hp = thing.hpTotal = 3;
						goto default;
					case ID.SHOT: goto default;
					case ID.LOOT: goto default;
					default:
						thing.InitPos(pos);
						thing.SetID(id);
						break;
				}
			} else {
				switch(fabId) {
					case 0:
						break;
					case ID.PLAYER:
						obj = Instantiate(lib.player);
						player = obj.GetComponent<Player>();
						player.map = this;
						thing = obj.GetComponent<Thing>();
						thing.SetID(id);
						thing.InitPos(pos);
						return thing;
					case ID.WALL:
						obj = Instantiate(lib.wall, transform); goto default;
					case ID.HARD_WALL:
						obj = Instantiate(lib.hardWall, transform); goto default;
					case ID.MON:
						obj = Instantiate(lib.mon, transform); goto default;
					case ID.SHOT:
						obj = Instantiate(lib.shot, transform); goto default;
					case ID.LOOT:
						obj = Instantiate(lib.loot, transform); goto default;
					default:
						thing = obj.GetComponent<Thing>();
						thing.SetID(id);
						thing.InitPos(pos);
						break;
				}
			}
			if(thing != null) {
				if(spawn) {
					spawned.Add(thing);
				} else {
					if(team != 0) {
						if(team == ID.GOOD) {
							goodies.Add(thing);
						} else if(team == ID.BAD) {
							baddies.Add(thing);
						}
					}
				}
			}
			//Debug.Log(thing.id+" "+pos);
			return thing;
		}

		/* Create a particle effect */
		public FX GetFX(FX.ID id, Vector3 pos, Sprite sprite = null) {
			FX fx = null;
			if(fxPool.ContainsKey(id) && fxPool[id].Count > 0) {
				fx = fxPool[id].Pop();
			} else {
				GameObject obj = null;
				switch(id) {
					case FX.ID.BANG: obj = Instantiate(lib.bang, fxParent); break;
					case FX.ID.SHOT_HIT: obj = Instantiate(lib.shotHit, fxParent); break;
				}
				fx = obj.GetComponent<FX>();
			}
			fx.Spawn(id, pos, sprite);
			return fx;
		}

		/* Return the closest target of id that is orthoganal to pos */
		public Vector3Int GetTarget(Vector3Int pos, ID id) {
			int best = int.MaxValue;
			Vector3Int bestPos = pos;
			for(int i = 0; i < DIRS.Length; i++) {
				var p = pos;
				var d = DIRS[i];
				int n = 0;
				while(bounds.Contains(p) && n < best) {
					p += d;
					n++;
					if(things.ContainsKey(p)) {
						if(things[p].id == id) {
							bestPos = p;
							best = n;
						}
						break;
					}
				}
			}
			return bestPos;
		}

		// a and b MUST be parallel
		public bool LOS(Vector3Int a, Vector3Int b, int maxDist) {
			int x = a.x == b.x ? 0 : a.x > b.x ? -1 : 1;
			int y = a.y == b.y ? 0 : a.y > b.y ? -1 : 1;
			do {
				a.x += x;
				a.y += y;
			} while(
				a != b &&
				bounds.Contains(a) &&
				!things.ContainsKey(a) &&
				--maxDist > 0
			);
			return a == b;
		}

		/* Pick a random empty direction - if there is one */
		public Vector3Int Wander(Vector3Int pos, bool reserve) {
			int len = DIRS.Length;
			int n = rng.Next(len);
			for(int i = 0; i < len; i++) {
				Vector3Int p = pos + DIRS[(i + n) % len]; // rotate to random
				if(bounds.Contains(p) && !things.ContainsKey(p)) {
					pos = p;
					break;
				}
			}
			if(reserve) paths.Remove(pos);
			return pos;
		}

		/* Hunt targets given by paths flood, or wait */
		public Vector3Int Chase(Thing thing, bool reserve) {
			int best = int.MaxValue;
			var pos = thing.pos;
			int len = DIRS.Length;
			for(int i = 0; i < len; i++) {
				int n = ((roundCount - thing.roundCreated) + i) % len;// rotate start of search each round
				var d = DIRS[n];
				var p = thing.pos + d;
				if(paths.ContainsKey(p)) {
					int value = paths[p];
					if(value < best || (value == best && d == thing.dir)) { // favour current direction, otherwise AI dithers
						if(things.ContainsKey(p)) {
							// avoid directions walking through blockers
							if((things[p].id & thing.blockMask) != 0) {
								continue;
							}
						}
						best = value;
						pos = p;
					}
				}
			}
			// we don't want other agents on this trail
			if(reserve) {
				// unless it's the target
				if(paths.ContainsKey(pos) && paths[pos] > 0) paths.Remove(pos);
			}
			return pos;
		}
		/* Run away from targets given by paths flood.
		 * Has issue with getting trapped, recommend calling Wander() in edge cases if you really need it */
		public Vector3Int Flee(Thing thing, bool reserve) {
			// will move towards edge of paths[] when not standing on paths[] 
			if(!paths.ContainsKey(thing.pos)) return thing.pos;
			var pos = thing.pos;
			int best = paths[pos];
			int len = DIRS.Length;
			for(int i = 0; i < DIRS.Length; i++) {
				int n = ((roundCount - thing.roundCreated) + i) % len;// rotate start of search each round
				var d = DIRS[n];
				var p = thing.pos + d;
				if(paths.ContainsKey(p)) {
					int value = paths[p];
					if(value > best || (value == best && d == thing.dir)) { // favour current direction, otherwise AI dithers
						if(things.ContainsKey(p)) {
							// avoid directions walking through blockers
							if((things[p].id & thing.blockMask) != 0) {
								continue;
							}
						}
						best = value;
						pos = p;
					}
				}
			}
			if(reserve) paths.Remove(pos);
			return pos;
		}

		/* Um, should turn this into an extension really */
		public static void Iterate(BoundsInt area, Action<Vector3Int> callback) {
			var zMin = area.min.z;
			var zMax = area.max.z;
			var yMin = area.min.y;
			var yMax = area.max.y;
			var xMin = area.min.x;
			var xMax = area.max.x;
			for(var z = zMin; z < zMax; z++) {
				for(var y = yMin; y < yMax; y++) {
					for(var x = xMin; x < xMax; x++) {
						callback(new Vector3Int(x, y, z));
					}
				}
			}
		}

		public void Shake(Vector3Int pos, int strength) {
			camCtrl.ShakeDist(player.thing.pos, pos, SHAKE_DIST_MAX, strength);
		}
#if UNITY_EDITOR
		void OnDrawGizmos() {
			Gizmos.color = new Color(1, 1, 0, 0.5f);
			var size = new Vector3(
				Thing.SCALE.x * bounds.size.x,
				Thing.SCALE.y * bounds.size.y,
				1
			);
			var offset = size * 0.5f;
			offset.y *= -1;
			Gizmos.DrawWireCube(transform.position + offset, size);
			Handles.color = Color.red;
			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.yellow;
			if(paths != null) {
				foreach(var pos in paths.Keys) {
					var d = paths[pos];
					Handles.Label(transform.position + Thing.GridPos(pos), ""+d, style);
				}
			}
		}
#endif
	}
}
