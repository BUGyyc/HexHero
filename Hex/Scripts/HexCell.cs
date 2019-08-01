using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 地图的单元格
/// </summary>
public class HexCell : MonoBehaviour {

	public HexCoordinates coordinates;

	public RectTransform uiRect;

	public HexGridChunk chunk;

	public int Index { get; set; }

	public int ColumnIndex { get; set; }

	/// <summary>
	/// 海拔
	/// </summary>
	/// <value></value>
	public int Elevation {
		get {
			return elevation;
		}
		set {
			if (elevation == value) {
				return;
			}
			int originalViewElevation = ViewElevation;
			elevation = value;
			if (ViewElevation != originalViewElevation) {
				ShaderData.ViewElevationChanged ();
			}
			RefreshPosition ();
			ValidateRivers ();

			for (int i = 0; i < roads.Length; i++) {
				if (roads[i] && GetElevationDifference ((HexDirection) i) > 1) {
					SetRoad (i, false);
				}
			}

			Refresh ();
		}
	}
	/// <summary>
	/// 水的深度
	/// </summary>
	/// <value></value>
	public int WaterLevel {
		get {
			return waterLevel;
		}
		set {
			if (waterLevel == value) {
				return;
			}
			int originalViewElevation = ViewElevation;
			waterLevel = value;
			if (ViewElevation != originalViewElevation) {
				ShaderData.ViewElevationChanged ();
			}
			ValidateRivers ();
			Refresh ();
		}
	}

	/// <summary>
	/// 被水遮住，显示的深度
	/// </summary>
	/// <value></value>
	public int ViewElevation {
		get {
			return elevation >= waterLevel ? elevation : waterLevel;
		}
	}

	/// <summary>
	/// 是否在水下
	/// </summary>
	/// <value></value>
	public bool IsUnderwater {
		get {
			return waterLevel > elevation;
		}
	}

	/// <summary>
	/// 上游？
	/// </summary>
	/// <value></value>
	public bool HasIncomingRiver {
		get {
			return hasIncomingRiver;
		}
	}
	/// <summary>
	/// 下游
	/// </summary>
	/// <value></value>
	public bool HasOutgoingRiver {
		get {
			return hasOutgoingRiver;
		}
	}
	/// <summary>
	/// 是否有河流
	/// </summary>
	/// <value></value>
	public bool HasRiver {
		get {
			return hasIncomingRiver || hasOutgoingRiver;
		}
	}

	public bool HasRiverBeginOrEnd {
		get {
			return hasIncomingRiver != hasOutgoingRiver;
		}
	}

	public HexDirection RiverBeginOrEndDirection {
		get {
			return hasIncomingRiver ? incomingRiver : outgoingRiver;
		}
	}
	/// <summary>
	/// 是否有路
	/// </summary>
	/// <value></value>
	public bool HasRoads {
		get {
			for (int i = 0; i < roads.Length; i++) {
				if (roads[i]) {
					return true;
				}
			}
			return false;
		}
	}

	public HexDirection IncomingRiver {
		get {
			return incomingRiver;
		}
	}

	public HexDirection OutgoingRiver {
		get {
			return outgoingRiver;
		}
	}

	public Vector3 Position {
		get {
			return transform.localPosition;
		}
	}

	public float StreamBedY {
		get {
			return (elevation + HexMetrics.streamBedElevationOffset) *
				HexMetrics.elevationStep;
		}
	}

	public float RiverSurfaceY {
		get {
			return (elevation + HexMetrics.waterElevationOffset) *
				HexMetrics.elevationStep;
		}
	}

	public float WaterSurfaceY {
		get {
			return (waterLevel + HexMetrics.waterElevationOffset) *
				HexMetrics.elevationStep;
		}
	}
	/// <summary>
	/// 城镇等级
	/// </summary>
	/// <value></value>
	public int UrbanLevel {
		get {
			return urbanLevel;
		}
		set {
			if (urbanLevel != value) {
				urbanLevel = value;
				RefreshSelfOnly ();
			}
		}
	}

	/// <summary>
	/// 农场等级
	/// </summary>
	/// <value></value>
	public int FarmLevel {
		get {
			return farmLevel;
		}
		set {
			if (farmLevel != value) {
				farmLevel = value;
				RefreshSelfOnly ();
			}
		}
	}
	/// <summary>
	/// 植物等级
	/// </summary>
	/// <value></value>
	public int PlantLevel {
		get {
			return plantLevel;
		}
		set {
			if (plantLevel != value) {
				plantLevel = value;
				RefreshSelfOnly ();
			}
		}
	}
	/// <summary>
	/// 特别物品
	/// </summary>
	/// <value></value>
	public int SpecialIndex {
		get {
			return specialIndex;
		}
		set {
			if (specialIndex != value && !HasRiver) {
				specialIndex = value;
				RemoveRoads ();
				RefreshSelfOnly ();
			}
		}
	}
	/// <summary>
	/// 是否拥有特别物品
	/// </summary>
	/// <value></value>
	public bool IsSpecial {
		get {
			return specialIndex > 0;
		}
	}

	/// <summary>
	/// 是否被墙？
	/// </summary>
	/// <value></value>
	public bool Walled {
		get {
			return walled;
		}
		set {
			if (walled != value) {
				walled = value;
				Refresh ();
			}
		}
	}
	/// <summary>
	/// 土地类型
	/// </summary>
	/// <value></value>
	public int TerrainTypeIndex {
		get {
			return terrainTypeIndex;
		}
		set {
			if (terrainTypeIndex != value) {
				terrainTypeIndex = value;
				//暂时屏蔽
				// ShaderData.RefreshTerrain (this);
			}
		}
	}
	/// <summary>
	/// 是否可见
	/// </summary>
	/// <value></value>
	public bool IsVisible {
		get {
			return visibility > 0 && Explorable;
		}
	}
	/// <summary>
	/// 扩展？
	/// </summary>
	/// <value></value>
	public bool IsExplored {
		get {
			return explored && Explorable;
		}
		private set {
			explored = value;
		}
	}

	public bool Explorable { get; set; }
	/// <summary>
	/// 距离
	/// </summary>
	/// <value></value>
	public int Distance {
		get {
			return distance;
		}
		set {
			distance = value;
			// UpdateDistanceLabel ();
		}
	}
	/// <summary>
	/// 更新显示距离
	/// </summary>
	/// <param name="maxStep"></param>
	public void UpdateDistanceLabel (int maxStep) {
		UnityEngine.UI.Text label = uiRect.GetComponent<Text> ();
		label.text = (distance > maxStep) ? "" : distance.ToString ();
	}
	/// <summary>
	/// 隐藏显示距离
	/// </summary>
	public void HideLabel () {
		UnityEngine.UI.Text label = uiRect.GetComponent<Text> ();
		label.text = "";
	}
	/// <summary>
	/// 英雄
	/// </summary>
	/// <value></value>
	public HexUnit Unit { get; set; }

	public HexCell PathFrom { get; set; }

	public int SearchHeuristic { get; set; }

	public int SearchPriority {
		get {
			return distance + SearchHeuristic;
		}
	}

	public int SearchPhase { get; set; }

	public HexCell NextWithSamePriority { get; set; }

	public HexCellShaderData ShaderData { get; set; }

	int terrainTypeIndex;

	int elevation = int.MinValue;
	int waterLevel;

	int urbanLevel, farmLevel, plantLevel;

	int specialIndex;

	int distance;

	int visibility;

	bool explored;

	bool walled;

	bool hasIncomingRiver, hasOutgoingRiver;
	HexDirection incomingRiver, outgoingRiver;

	[SerializeField]
	HexCell[] neighbors;

	[SerializeField]
	bool[] roads;

	public void IncreaseVisibility () {
		visibility += 1;
		if (visibility == 1) {
			IsExplored = true;
			ShaderData.RefreshVisibility (this);
		}
	}

	public void DecreaseVisibility () {
		visibility -= 1;
		if (visibility == 0) {
			ShaderData.RefreshVisibility (this);
		}
	}

	public void ResetVisibility () {
		if (visibility > 0) {
			visibility = 0;
			ShaderData.RefreshVisibility (this);
		}
	}

	/// <summary>
	/// 获取相连的单元
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public HexCell GetNeighbor (HexDirection direction) {
		return neighbors[(int) direction];
	}
	/// <summary>
	/// 设置相邻的单元
	/// </summary>
	/// <param name="direction"></param>
	/// <param name="cell"></param>
	public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int) direction] = cell;
		cell.neighbors[(int) direction.Opposite ()] = this;
	}
	/// <summary>
	/// 边界类型
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public HexEdgeType GetEdgeType (HexDirection direction) {
		return HexMetrics.GetEdgeType (
			elevation, neighbors[(int) direction].elevation
		);
	}
	/// <summary>
	/// 边界类型
	/// </summary>
	/// <param name="otherCell"></param>
	/// <returns></returns>
	public HexEdgeType GetEdgeType (HexCell otherCell) {
		return HexMetrics.GetEdgeType (
			elevation, otherCell.elevation
		);
	}
	/// <summary>
	/// 是否有河流穿过边界
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public bool HasRiverThroughEdge (HexDirection direction) {
		return
		hasIncomingRiver && incomingRiver == direction ||
			hasOutgoingRiver && outgoingRiver == direction;
	}

	public void RemoveIncomingRiver () {
		if (!hasIncomingRiver) {
			return;
		}
		hasIncomingRiver = false;
		RefreshSelfOnly ();

		HexCell neighbor = GetNeighbor (incomingRiver);
		neighbor.hasOutgoingRiver = false;
		neighbor.RefreshSelfOnly ();
	}

	public void RemoveOutgoingRiver () {
		if (!hasOutgoingRiver) {
			return;
		}
		hasOutgoingRiver = false;
		RefreshSelfOnly ();

		HexCell neighbor = GetNeighbor (outgoingRiver);
		neighbor.hasIncomingRiver = false;
		neighbor.RefreshSelfOnly ();
	}

	public void RemoveRiver () {
		RemoveOutgoingRiver ();
		RemoveIncomingRiver ();
	}

	public void SetOutgoingRiver (HexDirection direction) {
		if (hasOutgoingRiver && outgoingRiver == direction) {
			return;
		}

		HexCell neighbor = GetNeighbor (direction);
		if (!IsValidRiverDestination (neighbor)) {
			return;
		}

		RemoveOutgoingRiver ();
		if (hasIncomingRiver && incomingRiver == direction) {
			RemoveIncomingRiver ();
		}
		hasOutgoingRiver = true;
		outgoingRiver = direction;
		specialIndex = 0;

		neighbor.RemoveIncomingRiver ();
		neighbor.hasIncomingRiver = true;
		neighbor.incomingRiver = direction.Opposite ();
		neighbor.specialIndex = 0;

		SetRoad ((int) direction, false);
	}
	/// <summary>
	/// 是否有路穿过边界
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public bool HasRoadThroughEdge (HexDirection direction) {
		return roads[(int) direction];
	}
	/// <summary>
	/// 添加路径
	/// </summary>
	/// <param name="direction"></param>
	public void AddRoad (HexDirection direction) {
		if (!roads[(int) direction] && !HasRiverThroughEdge (direction) &&
			!IsSpecial && !GetNeighbor (direction).IsSpecial &&
			GetElevationDifference (direction) <= 1
		) {
			SetRoad ((int) direction, true);
		}
	}
	/// <summary>
	/// 移除路径
	/// </summary>
	public void RemoveRoads () {
		for (int i = 0; i < neighbors.Length; i++) {
			if (roads[i]) {
				SetRoad (i, false);
			}
		}
	}
	/// <summary>
	/// 获得海拔相邻的差值
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public int GetElevationDifference (HexDirection direction) {
		int difference = elevation - GetNeighbor (direction).elevation;
		return difference >= 0 ? difference : -difference;
	}

	bool IsValidRiverDestination (HexCell neighbor) {
		return neighbor && (
			elevation >= neighbor.elevation || waterLevel == neighbor.elevation
		);
	}

	void ValidateRivers () {
		if (
			hasOutgoingRiver &&
			!IsValidRiverDestination (GetNeighbor (outgoingRiver))
		) {
			RemoveOutgoingRiver ();
		}
		if (
			hasIncomingRiver &&
			!GetNeighbor (incomingRiver).IsValidRiverDestination (this)
		) {
			RemoveIncomingRiver ();
		}
	}
	/// <summary>
	/// 设置路径
	/// </summary>
	/// <param name="index"></param>
	/// <param name="state"></param>
	void SetRoad (int index, bool state) {
		roads[index] = state;
		neighbors[index].roads[(int) ((HexDirection) index).Opposite ()] = state;
		neighbors[index].RefreshSelfOnly ();
		RefreshSelfOnly ();
	}
	/// <summary>
	/// 刷新坐标
	/// </summary>
	void RefreshPosition () {
		Vector3 position = transform.localPosition;
		position.y = elevation * HexMetrics.elevationStep;
		//噪声生成高度
		position.y +=
			(HexMetrics.SampleNoise (position).y * 2f - 1f) *
			HexMetrics.elevationPerturbStrength;
		//设定新的坐标
		transform.localPosition = position;

		Vector3 uiPosition = uiRect.localPosition;
		uiPosition.z = -position.y;
		uiRect.localPosition = uiPosition;
	}
	/// <summary>
	/// 刷新
	/// </summary>
	void Refresh () {
		if (chunk) {
			chunk.Refresh ();
			for (int i = 0; i < neighbors.Length; i++) {
				HexCell neighbor = neighbors[i];
				if (neighbor != null && neighbor.chunk != chunk) {
					neighbor.chunk.Refresh ();
				}
			}
			if (Unit) {
				Unit.ValidateLocation ();
			}
		}
	}

	void RefreshSelfOnly () {
		chunk.Refresh ();
		if (Unit) {
			Unit.ValidateLocation ();
		}
	}

	/// <summary>
	/// 保存
	/// </summary>
	/// <param name="writer"></param>
	public void Save (BinaryWriter writer) {
		writer.Write ((byte) terrainTypeIndex); //土地的类型
		writer.Write ((byte) (elevation + 127)); //海拔高度
		writer.Write ((byte) waterLevel); //水的深度
		writer.Write ((byte) urbanLevel); //城市等级
		writer.Write ((byte) farmLevel); //农场等级
		writer.Write ((byte) plantLevel); //植物等级
		writer.Write ((byte) specialIndex); //特别物品等级
		writer.Write (walled); //是否被城墙包裹

		if (hasIncomingRiver) {
			writer.Write ((byte) (incomingRiver + 128)); //上游？
		} else {
			writer.Write ((byte) 0);
		}

		if (hasOutgoingRiver) {
			writer.Write ((byte) (outgoingRiver + 128)); //下游？
		} else {
			writer.Write ((byte) 0);
		}

		int roadFlags = 0;
		for (int i = 0; i < roads.Length; i++) {
			if (roads[i]) {
				roadFlags |= 1 << i;
			}
		}
		writer.Write ((byte) roadFlags); //是否有路
		writer.Write (IsExplored); //是否是扩展土地
	}

	/// <summary>
	/// 加载指定单元的数据
	/// </summary>
	/// <param name="reader"></param>
	/// <param name="header"></param>
	public void Load (BinaryReader reader, int header) {
		terrainTypeIndex = reader.ReadByte ();
		ShaderData.RefreshTerrain (this);
		elevation = reader.ReadByte ();
		if (header >= 4) {
			elevation -= 127;
		}
		//刷新坐标
		RefreshPosition ();
		waterLevel = reader.ReadByte ();
		urbanLevel = reader.ReadByte ();
		farmLevel = reader.ReadByte ();
		plantLevel = reader.ReadByte ();
		specialIndex = reader.ReadByte ();
		walled = reader.ReadBoolean ();

		byte riverData = reader.ReadByte ();
		if (riverData >= 128) {
			hasIncomingRiver = true;
			incomingRiver = (HexDirection) (riverData - 128);
		} else {
			hasIncomingRiver = false;
		}

		riverData = reader.ReadByte ();
		if (riverData >= 128) {
			hasOutgoingRiver = true;
			outgoingRiver = (HexDirection) (riverData - 128);
		} else {
			hasOutgoingRiver = false;
		}

		int roadFlags = reader.ReadByte ();
		for (int i = 0; i < roads.Length; i++) {
			roads[i] = (roadFlags & (1 << i)) != 0;
		}

		IsExplored = header >= 3 ? reader.ReadBoolean () : false;
		ShaderData.RefreshVisibility (this);
	}
	/// <summary>
	/// 设置文字显示
	/// </summary>
	/// <param name="text"></param>
	public void SetLabel (string text) {
		UnityEngine.UI.Text label = uiRect.GetComponent<Text> ();
		label.text = text;
	}
	/// <summary>
	/// 隐藏光圈
	/// </summary>
	public void DisableHighlight () {
		Image highlight = uiRect.GetChild (0).GetComponent<Image> ();
		highlight.enabled = false;
	}
	/// <summary>
	/// 展示光圈，并设置特定颜色
	/// </summary>
	/// <param name="color"></param>
	public void EnableHighlight (Color color) {
		Image highlight = uiRect.GetChild (0).GetComponent<Image> ();
		highlight.color = color;
		highlight.enabled = true;
	}

	public void SetMapData (float data) {
		ShaderData.SetMapData (this, data);
	}

	/// <summary>
	/// 是否是相邻的单元
	/// </summary>
	/// <param name="other"></param>
	public bool IsNeighbor (HexCell other) {
		foreach (var item in neighbors) {
			if (item == other) return true;
		}
		return false;
	}

	/// <summary>
	/// 获取周围的英雄
	/// </summary>
	/// <returns></returns>
	private List<HexCell> getAllNeighborHexCell () {
		List<HexCell> list = new List<HexCell> ();
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			HexCell neighbor = GetNeighbor (d);
			if (neighbor.Unit) {
				list.Add (neighbor);
			}
		}
		return list;
	}

	/// <summary>
	/// 周围敌对势力英雄
	/// </summary>
	/// <param name="faction"></param>
	/// <returns></returns>
	public List<HexCell> getEnemyNeighborHexCell (Faction faction) {
		int factionID = FactionManager.getIdByFaction (faction);
		List<HexCell> list = getAllNeighborHexCell ();
		List<HexCell> enemyList = new List<HexCell> ();
		if (list == null) return null;
		foreach (var item in list) {
			HexUnit mHexUnit = item.Unit;
			if (mHexUnit == null) continue;
			HeroItem heroItem = mHexUnit.GetComponent<HeroUI> ().GetHeroItem ();
			if (heroItem != null && heroItem.gj != factionID) {
				enemyList.Add (item);
			}
		}
		return enemyList;
	}
}