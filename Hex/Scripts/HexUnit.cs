using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
/// <summary>
/// 地图的物件类
/// </summary>
public class HexUnit : MonoBehaviour {
	/// <summary>
	/// 旋转速度
	/// </summary>
	const float rotationSpeed = 180f;
	const float travelSpeed = 4f;

	/// <summary>
	/// 英雄
	/// </summary>
	public static HexUnit unitPrefab;
	/// <summary>
	/// 城池
	/// </summary>
	public static HexUnit unitCity;

	public HexGrid Grid { get; set; }

	/// <summary>
	/// 是否是英雄，或者是城池
	/// </summary>
	private static bool isHero;

	/// <summary>
	/// 伤害显示值
	/// </summary>
	public GameObject PopupDamage;

	public HexCell Location {
		get {
			return location;
		}
		set {
			if (location) {
				Grid.DecreaseVisibility (location, VisionRange);
				location.Unit = null;
			}
			location = value;
			value.Unit = this;
			Grid.IncreaseVisibility (value, VisionRange);
			transform.localPosition = value.Position;
			Grid.MakeChildOfColumn (transform, value.ColumnIndex);
		}
	}

	HexCell location, currentTravelLocation;

	private Faction mFaction;

	/// <summary>
	/// 设置所属方
	/// </summary>
	/// <param name="f"></param>
	public void setFaction (Faction f) {
		mFaction = f;
	}

	/// <summary>
	/// 方向
	/// </summary>
	/// <value></value>
	public float Orientation {
		get {
			return orientation;
		}
		set {
			orientation = value;
			transform.localRotation = Quaternion.Euler (0f, value, 0f);
		}
	}

	public bool IsHero {
		get {
			return isHero;
		}
		set {
			isHero = value;
		}
	}

	public int Speed {
		get {
			return 24;
		}
	}

	public int VisionRange {
		get {
			return 3;
		}
	}

	float orientation;

	List<HexCell> pathToTravel;

	public void ValidateLocation () {
		transform.localPosition = location.Position;
	}

	/// <summary>
	/// 是否是有效的目的地
	/// </summary>
	/// <param name="cell"></param>
	/// <returns></returns>
	public bool IsValidDestination (HexCell cell) {
		return cell.IsExplored && !cell.IsUnderwater && !cell.Unit;
	}

	/// <summary>
	/// 攻击对方
	/// </summary>
	public void Attack (HexCell cell) {
		StopAllCoroutines ();
		StartCoroutine (PlayAttack (cell));
	}

	/// <summary>
	/// //攻击某个单元
	/// </summary>
	/// <param name="cell"></param>
	/// <returns></returns>
	IEnumerator PlayAttack (HexCell cell) {
		//朝向攻击目标
		yield return LookAt (cell.Position);
		WaitForSeconds delay = new WaitForSeconds (1f);
		yield return delay;
		this.GetComponent<HeroBehaviours> ().SetAnimatorState ("isAttack");
		this.useSkill (cell);
		yield return delay;
		this.GetComponent<HeroBehaviours> ().SetAnimatorState ("isIdle");
	}

	/// <summary>
	/// 被攻击
	/// </summary>
	public void Damage () {
		StopAllCoroutines ();
		StartCoroutine (PlayBeDamage ());
		HeroUI heroUI = this.GetComponent<HeroUI> ();
		heroUI.PopDamageText (100);
	}

	/// <summary>
	/// 被攻击
	/// </summary>
	/// <returns></returns>
	IEnumerator PlayBeDamage () {
		WaitForSeconds delay = new WaitForSeconds (1f);
		yield return delay;
		this.GetComponent<HeroBehaviours> ().SetAnimatorState ("isDamage");
		yield return delay;
		this.GetComponent<HeroBehaviours> ().SetAnimatorState ("isIdle");
	}

	/// <summary>
	/// 英雄使用技能
	/// </summary>
	private void useSkill (HexCell cell) {
		int heroId = this.GetComponent<HeroBehaviours> ().HeroId;
		int skillId = HeroManager.getSkill_0_byHeroId (heroId, true, 1); //获取英雄被动技能
		if (skillId == -1) return;
		//广播使用的技能，展示UI
		// CEventDispatcherObj.cEventDispatcher.dispatchEvent (new CEvent (CEventName.UPDATE_SKILL_BROADCAST_PANEL), this);
		Grid.GetComponent<SkillManager> ().onPlayEffectById (skillId, cell.gameObject.transform.position, 3f);
	}

	/// <summary>
	/// 按照路径移动 
	/// </summary>
	/// <param name="path"></param>
	public void Travel (List<HexCell> path) {
		location.Unit = null;
		location = path[path.Count - 1];
		location.Unit = this;
		pathToTravel = path;
		StopAllCoroutines ();
		StartCoroutine (TravelPath ());
		this.GetComponent<HeroBehaviours> ().SetAnimatorState ("isRun");
	}

	public void TravelOfAI (List<HexCell> path) {
		location.Unit = null;
		location = path[path.Count - 1];
		location.Unit = this;
		pathToTravel = path;
		StopAllCoroutines ();
		StartCoroutine (TravelPath ());
		this.GetComponent<HeroBehaviours> ().SetAnimatorState ("isRun");
	}
	/// <summary>
	/// 按路径移动过去
	/// </summary>
	/// <returns></returns>
	IEnumerator TravelPath () {
		Vector3 a, b, c = pathToTravel[0].Position;
		//朝向
		yield return LookAt (pathToTravel[1].Position);

		if (!currentTravelLocation) {
			currentTravelLocation = pathToTravel[0];
		}
		Grid.DecreaseVisibility (currentTravelLocation, VisionRange);
		int currentColumn = currentTravelLocation.ColumnIndex;

		float t = Time.deltaTime * travelSpeed;
		for (int i = 1; i < pathToTravel.Count; i++) {
			currentTravelLocation = pathToTravel[i];
			a = c;
			b = pathToTravel[i - 1].Position;

			int nextColumn = currentTravelLocation.ColumnIndex;
			if (currentColumn != nextColumn) {
				if (nextColumn < currentColumn - 1) {
					a.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
					b.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
				} else if (nextColumn > currentColumn + 1) {
					a.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
					b.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
				}
				Grid.MakeChildOfColumn (transform, nextColumn);
				currentColumn = nextColumn;
			}

			c = (b + currentTravelLocation.Position) * 0.5f;
			Grid.IncreaseVisibility (pathToTravel[i], VisionRange);

			for (; t < 1f; t += Time.deltaTime * travelSpeed) {
				transform.localPosition = Bezier.GetPoint (a, b, c, t);
				Vector3 d = Bezier.GetDerivative (a, b, c, t);
				d.y = 0f;
				transform.localRotation = Quaternion.LookRotation (d);
				yield return null;
			}
			Grid.DecreaseVisibility (pathToTravel[i], VisionRange);
			t -= 1f;
		}
		currentTravelLocation = null;

		a = c;
		b = location.Position;
		c = b;
		Grid.IncreaseVisibility (location, VisionRange);
		for (; t < 1f; t += Time.deltaTime * travelSpeed) {
			transform.localPosition = Bezier.GetPoint (a, b, c, t);
			Vector3 d = Bezier.GetDerivative (a, b, c, t);
			d.y = 0f;
			transform.localRotation = Quaternion.LookRotation (d);
			yield return null;
		}

		transform.localPosition = location.Position;
		orientation = transform.localRotation.eulerAngles.y;
		ListPool<HexCell>.Add (pathToTravel);
		pathToTravel = null;
		//移动完成，返回默认状态动画
		this.GetComponent<HeroBehaviours> ().SetAnimatorState ("isIdle");
	}

	/// <summary>
	/// 朝向对方
	/// </summary>
	/// <param name="point"></param>
	/// <returns></returns>
	IEnumerator LookAt (Vector3 point) {
		if (HexMetrics.Wrapping) {
			float xDistance = point.x - transform.localPosition.x;
			if (xDistance < -HexMetrics.innerRadius * HexMetrics.wrapSize) {
				point.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
			} else if (xDistance > HexMetrics.innerRadius * HexMetrics.wrapSize) {
				point.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
			}
		}

		point.y = transform.localPosition.y;
		Quaternion fromRotation = transform.localRotation;
		Quaternion toRotation =
			Quaternion.LookRotation (point - transform.localPosition);
		float angle = Quaternion.Angle (fromRotation, toRotation);

		if (angle > 0f) {
			float speed = rotationSpeed / angle;
			for (
				float t = Time.deltaTime * speed; t < 1f; t += Time.deltaTime * speed
			) {
				transform.localRotation =
					Quaternion.Slerp (fromRotation, toRotation, t);
				yield return null;
			}
		}

		transform.LookAt (point);
		orientation = transform.localRotation.eulerAngles.y;
	}
	/// <summary>
	/// 获得移动成本
	/// </summary>
	/// <param name="fromCell"></param>
	/// <param name="toCell"></param>
	/// <param name="direction"></param>
	/// <returns></returns>
	public int GetMoveCost (
		HexCell fromCell, HexCell toCell, HexDirection direction) {
		//是否可以达到
		if (!IsValidDestination (toCell)) {
			return -1;
		}
		//边界的类型
		HexEdgeType edgeType = fromCell.GetEdgeType (toCell);
		if (edgeType == HexEdgeType.Cliff) {
			//悬崖式的是不能行走的
			return -1;
		}
		int moveCost;
		if (fromCell.HasRoadThroughEdge (direction)) {
			moveCost = 1;
		} else if (fromCell.Walled != toCell.Walled) {
			//被城墙中断了？
			return -1;
		} else {
			//其他状况，累计到成本里面
			moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
			moveCost +=
				toCell.UrbanLevel + toCell.FarmLevel + toCell.PlantLevel;
		}
		return moveCost;
	}
	/// <summary>
	/// 物件死亡逻辑
	/// </summary>
	public void Die () {
		if (location) {
			//关闭路径显示
			Grid.DecreaseVisibility (location, VisionRange);
		}
		location.Unit = null;
		Destroy (gameObject);
	}
	/// <summary>
	/// 保存
	/// </summary>
	/// <param name="writer"></param>
	public void Save (BinaryWriter writer) {
		location.coordinates.Save (writer);
		writer.Write (orientation);
		writer.Write (isHero);
	}
	/// <summary>
	/// 加载物件
	/// </summary>
	/// <param name="reader"></param>
	/// <param name="grid"></param>
	public static void Load (BinaryReader reader, HexGrid grid) {
		HexCoordinates coordinates = HexCoordinates.Load (reader);
		//取方向
		float orientation = reader.ReadSingle ();
		//是否是英雄，还是城市
		isHero = reader.ReadBoolean ();
		HexUnit hu = (isHero) ? unitPrefab : unitCity;
		//把物件添加到地图上
		grid.AddUnit (
			Instantiate (hu), grid.GetCell (coordinates), orientation
		);

	}

	void OnEnable () {
		if (location) {
			transform.localPosition = location.Position;
			if (currentTravelLocation) {
				Grid.IncreaseVisibility (location, VisionRange);
				Grid.DecreaseVisibility (currentTravelLocation, VisionRange);
				currentTravelLocation = null;
			}
		}
	}

	//	void OnDrawGizmos () {
	//		if (pathToTravel == null || pathToTravel.Count == 0) {
	//			return;
	//		}
	//
	//		Vector3 a, b, c = pathToTravel[0].Position;
	//
	//		for (int i = 1; i < pathToTravel.Count; i++) {
	//			a = c;
	//			b = pathToTravel[i - 1].Position;
	//			c = (b + pathToTravel[i].Position) * 0.5f;
	//			for (float t = 0f; t < 1f; t += 0.1f) {
	//				Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
	//			}
	//		}
	//
	//		a = c;
	//		b = pathToTravel[pathToTravel.Count - 1].Position;
	//		c = b;
	//		for (float t = 0f; t < 1f; t += 0.1f) {
	//			Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
	//		}
	//	}
}