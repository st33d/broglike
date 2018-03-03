using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Rog {
	/* An entity in a roguelike */
	public class Thing : MonoBehaviour {

		public static Map map;
		public Map.ID id;
		public Map.ID fabId;
		public Map.ID team;
		public int countId;
		public Map.ID blockMask;
		public int roundCreated;
		public Vector3Int pos;
		public Vector3Int prevPos;
		public Vector3Int prevDir;
		public Vector3Int dir;
		public Vector3Int face;
		public int hp;// an thing without hp can no longer participate in turns
		public int hpTotal;
		public Act act;
		public Act prevAct;
		public SpriteRenderer spriteRenderer;
		public Travel travel;// from Easing.cs
		public static int thingCount = 0;// order of instantiation
		public static readonly Vector2 SCALE = new Vector2(16, 16);

		public enum Act {
			WAIT, MOVE, MELEE, NOPE, SHOOT
		}

		public enum Mon {
			SPAWNER, HUNTER, FLEE
		}

		public static readonly Vector3Int NONE = new Vector3Int(0, 0, 0);
		public static readonly Vector3Int UP = new Vector3Int(0, -1, 0);
		public static readonly Vector3Int RIGHT = new Vector3Int(1, 0, 0);
		public static readonly Vector3Int DOWN = new Vector3Int(0, 1, 0);
		public static readonly Vector3Int LEFT = new Vector3Int(-1, 0, 0);
		public static readonly Vector3Int FORWARD = new Vector3Int(0, 0, 1);
		public static readonly Vector3Int BACK = new Vector3Int(0, 0, -1);

		public static Dictionary<Vector3Int, Quaternion> DIR_ROT = new Dictionary<Vector3Int, Quaternion> {
		{NONE, Quaternion.identity },
		{UP, Quaternion.identity },
		{RIGHT, Quaternion.Euler(new Vector3(0, 0, -90)) },
		{DOWN, Quaternion.Euler(new Vector3(0, 0, 180)) },
		{LEFT, Quaternion.Euler(new Vector3(0, 0, 90)) }
	};
		public static Quaternion FLIP_VERT = Quaternion.Euler(new Vector3(-180, 0, 0));
		public static Quaternion FLIP_HORIZ = Quaternion.Euler(new Vector3(0, -180, 0));

		public void SetID(Map.ID id) {
			gameObject.SetActive(true);
			this.id = id;
			fabId = (id & Map.PREFAB_MASK);
			team = (id & Map.TEAM_MASK);
			countId = (int)(id & Map.COUNT_MASK);
			name = fabId.ToString() + " c"+countId+" "+thingCount++;
			dir = UP;
			face = UP;
			hp = hpTotal = 1;
			blockMask = Map.ID.HARD_WALL;
			roundCreated = map.roundCount;


			switch(fabId) {
				case Map.ID.PLAYER:
					hp = hpTotal = 3;
					break;
				case Map.ID.MON:
					switch((Mon)countId) {
						case Mon.SPAWNER:
							spriteRenderer.sprite = map.lib.monSpawner;
							break;
						case Mon.HUNTER:
							spriteRenderer.sprite = map.lib.monHunter;
							break;
						case Mon.FLEE:
							spriteRenderer.sprite = map.lib.monFlee;
							break;
					}
					blockMask |= team;
					break;
			}
		}

		/* Get an action from interacting with a location */
		public Act MoveTo(Vector3Int p) {
			travel = null;
			// sanity check
			if(pos == p) {
				return Act.WAIT;
			}
			switch(fabId) {
				case Map.ID.PLAYER: goto case Map.ID.MON;
				case Map.ID.MON:
					if(!map.bounds.Contains(p)) {
						travel = new Travel(transform, SpritePos(pos), SpritePos(p), Easing.Nope);
						return Act.NOPE;
					}
					if(map.things.ContainsKey(p)) {
						var thing = map.things[p];
						var posId = thing.id;
						// hit it?
						if((posId & blockMask) == 0) {
							travel = new Travel(transform, SpritePos(pos), SpritePos(p), Easing.Nope);
							prevDir = dir;
							dir = DirTo(p);
							thing.Hit(this);
							return Act.MELEE;
							// a wall?
						} else {
							travel = new Travel(transform, SpritePos(pos), SpritePos(p), Easing.Nope);
							return Act.NOPE;
						}
					} else {
						travel = new Travel(transform, SpritePos(pos), SpritePos(p), Easing.Linear);
						MovePos(p);
						return Act.MOVE;
					}
				case Map.ID.SHOT:
					if(!map.bounds.Contains(p)) {
						Death(null);
						return Act.WAIT;
					} else if(map.things.ContainsKey(p)) {
						var posId = map.things[p].id;
						// blocker?
						if((posId & blockMask) != 0) {
							Death(null);
							return Act.WAIT;
						}
					}
					travel = new Travel(transform, SpritePos(pos), SpritePos(p), Easing.Linear);
					MovePos(p);
					return Act.MOVE;
			}
			return Act.WAIT;
		}

		public Act Shoot(Vector3Int d, Map.ID block = 0) {
			var t = map.GetThing(Map.ID.SHOT | team, pos, true);
			t.dir = d;
			t.blockMask |= block;
			return Act.SHOOT;
		}

		public void FaceDir() {
			if(DIR_ROT.ContainsKey(dir)) {
				transform.localRotation = DIR_ROT[dir];
			}
		}

		/* Are we trying to rut the blockage? */
		public bool HumpHump() {
			return prevAct == act && act == Act.NOPE;
		}

		/* Perform an action based on id */
		public void StartAct() {
			prevAct = act;
			act = Act.WAIT;
			// find a pos to do something to
			switch(fabId) {
				case Map.ID.SHOT:
					if(map.things.ContainsKey(pos)) {
						var thing = map.things[pos];
						var posTeam = (thing.id & Map.TEAM_MASK);
						// no friendly fire?
						if(posTeam != team) {
							thing.Hit(this);
							Death(thing);
						}
					}
					act = MoveTo(pos + dir);
					break;
				case Map.ID.LOOT: break;
				case Map.ID.MON:
					// just spawned?
					if(roundCreated > 0 && roundCreated == map.roundCount) {
						if(prevPos != pos) {
							travel = new Travel(transform, SpritePos(prevPos), SpritePos(pos), Easing.Linear);
							travel.SetDelta(0);
							act = Act.MOVE;
							return;
						}
					}
					Vector3Int target = new Vector3Int();
					switch((Mon)countId) {
						case Mon.SPAWNER:
							if((map.roundCount - roundCreated) % 4 == 3) {
								var p = map.Wander(pos, true);
								if(!map.things.ContainsKey(p)) {
									var spawnId = Map.ID.MON | Map.ID.BAD;
									spawnId |= (map.rng.NextDouble() < 0.5 ? (Map.ID)Mon.HUNTER : (Map.ID)Mon.FLEE);
									Spawn(spawnId, p);
									act = Act.WAIT;
									return;
								}
							}
							// wander harmlessly
							target = map.Wander(pos, true);
							act = MoveTo(target);
							break;
						case Mon.HUNTER:
							// hunt player
							target = map.Chase(this, true);
							act = MoveTo(target);
							break;
						case Mon.FLEE:
							// flee player
							target = map.Flee(this, true);
							act = MoveTo(target);
							break;
					}
					FaceDir();
					break;
			}
		}

		/* Animate the aftermath of the action */
		public void UpdateAct() {
			switch(act) {
				default:
					if(travel != null) {
						travel.SetDelta(map.turnElapsed);
					}
					break;
			}
		}

		/* Perform moves to be called at end of phase */
		public void EndAct() {
			switch(fabId) {
				case Map.ID.SHOT:
					if(map.things.ContainsKey(pos)) {
						var thing = map.things[pos];
						var posTeam = (thing.id & Map.TEAM_MASK);
						// no friendly fire?
						if(posTeam != team) {
							thing.Hit(this);
							Death(thing);
						}
					}
					break;
				case Map.ID.LOOT:
					break;
				default:
					// fire all triggers we have stepped on
					if(map.triggers.ContainsKey(pos)) {
						var list = map.triggers[pos];
						// count backwards to allow triggers to die
						for(int i = list.Count - 1; i > -1; i--) {
							var thing = list[i];
							thing.Trigger(this);
						}
					}
					break;
			}
			travel = null;// clear this for future turns
		}

		/* Take damage */
		public void Hit(Thing source) {
			//Debug.Log(id+" hit by "+source.id);
			hp--;
			if(fabId == Map.ID.PLAYER) {
				map.health.value = hp;
			}
			// allow hits beyond death, but only one death event
			if(hp == 0) {
				Death(source);
				map.Shake(pos, 5);
			} else {
				map.Shake(pos, 3);
			}
		}

		/* Activate behaviour upon caller */
		public void Trigger(Thing caller) {
			switch(fabId) {
				case Map.ID.SHOT:
					if((caller.team & team) == 0) {
						caller.Hit(this);
						Death(caller);
					}
					break;
				case Map.ID.LOOT:
					if((caller.team & team) != 0) {
						PickUp(caller);
					}
					break;
			}
		}

		/* Get picked up by collector */
		public void PickUp(Thing collector) {
			switch(collector.fabId) {
				case Map.ID.PLAYER:
					map.logBox.Say("Got Loot!");
					map.score.value++;
					map.power.value++;
					break;
			}
			Death(collector);
		}

		public void Spawn(Map.ID id, Vector3Int p) {
			var t = map.GetThing(id, p, true);
			t.prevPos = pos;
			t.dir = DirTo(p);
			t.FaceDir();
		}

		/* Where the sprite should end up after scaling (y axis flipped, so the map reads like a text document)*/
		public static Vector3 SpritePos(Vector3Int p) {
			return new Vector3((p.x + 0.5f) * SCALE.x, -(p.y + 0.5f) * SCALE.x);
		}

		/* Top left corner of grid position */
		public static Vector3 GridPos(Vector3Int p) {
			return new Vector3((p.x) * SCALE.x, -(p.y) * SCALE.x);
		}

		/* Set gfx and map position, call once on init */
		public void InitPos(Vector3Int p) {
			pos = prevPos = p;
			transform.localPosition = SpritePos(p);
			if((id & Map.ACTOR_MASK) != 0) map.things[p] = this;
			if((id & Map.TRIGGER_MASK) != 0) {
				if(map.triggers.ContainsKey(p)) {
					map.triggers[p].Add(this);
				} else {
					map.triggers[p] = new List<Thing> { this };
				}
			}
		}

		/* Moves map position and updates dir */
		public void MovePos(Vector3Int p) {
			if((id & Map.ACTOR_MASK) != 0) map.things.Remove(pos);
			if((id & Map.TRIGGER_MASK) != 0) {
				var list = map.triggers[pos];
				list.Remove(this);
				if(list.Count <= 0) {
					map.triggers.Remove(pos);
				}
			}
			prevDir = dir;
			dir = DirTo(p);
			prevPos = pos;
			pos = p;
			if((id & Map.ACTOR_MASK) != 0) map.things[p] = this;
			if((id & Map.TRIGGER_MASK) != 0) {
				if(map.triggers.ContainsKey(p)) {
					map.triggers[p].Add(this);
				} else {
					map.triggers[p] = new List<Thing> { this };
				}
			}
		}

		public Vector3Int DirTo(Vector3Int p) {
			if(pos.x < p.x) return RIGHT;
			else if(pos.y < p.y) return DOWN;
			else if(pos.x > p.x) return LEFT;
			else if(pos.y > p.y) return UP;
			return NONE;
		}

		/* Hide and add to graveyard for clean up later */
		public void Death(Thing source) {
			//Debug.Log(id+" death by "+(source != null ? ""+source.id : ""));

			if(hp > 0) hp = 0; // avoid future visits, record the damage
			switch(fabId) {
				case Map.ID.MON:
					map.GetFX(FX.ID.BANG, transform.localPosition, spriteRenderer.sprite);
					break;
				case Map.ID.PLAYER:
					map.GetFX(FX.ID.BANG, transform.localPosition, spriteRenderer.sprite);
					break;
				case Map.ID.SHOT:
					map.GetFX(FX.ID.SHOT_HIT, transform.localPosition);
					break;
				case Map.ID.WALL:
					map.GetFX(FX.ID.BANG, transform.localPosition, spriteRenderer.sprite);
					break;
			}
			gameObject.SetActive(false);
			map.graveyard.Add(this);
		}

		/* Called from main loop to recycle and remove map presence */
		public void Kill(bool pool = true) {
			if((id & Map.ACTOR_MASK) != 0) map.things.Remove(pos);
			if((id & Map.TRIGGER_MASK) != 0) {
				var list = map.triggers[pos];
				list.Remove(this);
				if(list.Count <= 0) {
					map.triggers.Remove(pos);
				}
			}
			if(pool) {
				if(!map.pool.ContainsKey(fabId)) {
					map.pool[fabId] = new Stack<Thing>();
				}
				map.pool[fabId].Push(this);

				// deactivate
			} else {
				Destroy(gameObject);
			}
		}

	}
}
