using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {
	/// <summary>
	/// 地图版本号？
	/// </summary>
	const int mapFileVersion = 5;
	/// <summary>
	/// 
	/// </summary>
	public int cellCountX = 20, cellCountZ = 15;
	public bool wrapping;
	/// <summary>
	/// 单元预设体
	/// </summary>
	public HexCell cellPrefab;
	/// <summary>
	/// 单元上的文本
	/// </summary>
	public Text cellLabelPrefab;
	/// <summary>
	/// 分块的网格
	/// </summary>
	public HexGridChunk chunkPrefab;
	/// <summary>
	/// 地图上的物件，这里直接放武将
	/// </summary>
	public HexUnit unitPrefab;
	/// <summary>
	/// 城市物件
	/// </summary>
	public HexUnit cityPrefab;
	/// <summary>
	/// 噪声生成的贴图
	/// </summary>
	public Texture2D noiseSource;
	/// <summary>
	/// 随机种子
	/// </summary>
	public int seed;

	public bool HasPath {
		get {
			return currentPathExists;
		}
	}

	Transform[] columns;
	HexGridChunk[] chunks;
	HexCell[] cells;

	int chunkCountX, chunkCountZ;

	HexCellPriorityQueue searchFrontier;

	int searchFrontierPhase;

	HexCell currentPathFrom, currentPathTo;
	bool currentPathExists;

	int currentCenterColumnIndex = -1;

	List<HexUnit> units = new List<HexUnit> ();

	HexCellShaderData cellShaderData;

	void Awake () {
		//InitNewMap ();
		//先加载老地图数据
		LoadOldMap ();
		// setCamera ();
	}

	private void Start () {
		StartCoroutine ("StartLoadMap");
	}

	public HexCell[] getCells () {
		return cells;
	}

	IEnumerator StartLoadMap () {
		//把老地图显示出来
		tryLoadMap ();
		yield return null;
	}

	/// <summary>
	/// 设置相机到指定位置
	/// </summary>
	void setCamera () {
		HexMapCamera.setCameraPosition (30f, 20f);
	}

	/// <summary>
	/// 创建新的地图
	/// </summary>
	void InitNewMap () {
		HexMetrics.noiseSource = noiseSource;
		HexMetrics.InitializeHashGrid (seed);
		HexUnit.unitPrefab = unitPrefab;
		HexUnit.unitCity = cityPrefab;
		cellShaderData = gameObject.AddComponent<HexCellShaderData> ();
		cellShaderData.Grid = this;
		CreateMap (cellCountX, cellCountZ, wrapping);
	}
	/// <summary>
	/// 加载事先保存的地图
	/// </summary>
	void LoadOldMap () {
		HexMetrics.noiseSource = noiseSource;
		HexMetrics.InitializeHashGrid (seed);
		HexUnit.unitPrefab = unitPrefab;
		HexUnit.unitCity = cityPrefab;
		cellShaderData = gameObject.AddComponent<HexCellShaderData> ();
		cellShaderData.Grid = this;
	}

	/// <summary>
	/// 加载已经存在的地图
	/// </summary>
	void tryLoadMap () {
		//拿到地图路径
		string path = GetSelectedPath ("test"); //C:/Users/Administrator/AppData/LocalLow/DefaultCompany/TestHex\\test.map
		if (path == null) {
			return;
		}
		//通过指定的路径，加载地图
		TryLoad (path);
	}

	string GetSelectedPath (string mapName) {
		if (mapName.Length == 0) {
			return null;
		}
		return Path.Combine (Application.persistentDataPath, mapName + ".map");
	}
	/// <summary>
	/// 尝试加载
	/// </summary>
	/// <param name="path"></param>
	void TryLoad (string path) {
		if (!File.Exists (path)) {
			//不存在
			Debug.LogError ("File does not exist " + path);
			return;
		}
		using (BinaryReader reader = new BinaryReader (File.OpenRead (path))) {
			int header = reader.ReadInt32 ();
			//判断地图版本号
			if (header <= mapFileVersion) {
				//导入地图
				Load (reader, header);
				setCamera ();
				//加载地图完成,发送事件
				CEventDispatcherObj.cEventDispatcher.dispatchEvent (new CEvent (CEventName.INIT_COMPLETE), this);
			} else {
				Debug.LogWarning ("Unknown map format " + header);
			}
		}
	}

	/// <summary>
	/// 放置指定武将到指定位置，以及设定旋转方向
	/// </summary>
	/// <param name="unit"></param>
	/// <param name="location"></param>
	/// <param name="orientation"></param>
	public void AddUnit (HexUnit unit, HexCell location, float orientation) {
		units.Add (unit);
		unit.Grid = this;
		unit.Location = location;
		unit.Orientation = orientation;

		// RobotManager.addRobot (unit);
		RobotManager.mInstance.addRobot (unit);
	}

	/// <summary>
	/// 移除武将
	/// </summary>
	/// <param name="unit"></param>
	public void RemoveUnit (HexUnit unit) {
		units.Remove (unit);
		unit.Die ();
	}

	public void MakeChildOfColumn (Transform child, int columnIndex) {
		child.SetParent (columns[columnIndex], false);
	}
	/// <summary>
	/// 创建地图
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
	/// <param name="wrapping"></param>
	/// <returns></returns>
	public bool CreateMap (int x, int z, bool wrapping) {
		if (
			x <= 0 || x % HexMetrics.chunkSizeX != 0 ||
			z <= 0 || z % HexMetrics.chunkSizeZ != 0
		) {
			//超出边界的情况
			Debug.LogError ("Unsupported map size.");
			return false;
		}
		//清理掉路径
		ClearPath ();
		//清理掉物品
		ClearUnits ();
		//销毁掉？？？
		if (columns != null) {
			for (int i = 0; i < columns.Length; i++) {
				Destroy (columns[i].gameObject);
			}
		}

		cellCountX = x;
		cellCountZ = z;
		this.wrapping = wrapping;
		currentCenterColumnIndex = -1;
		HexMetrics.wrapSize = wrapping ? cellCountX : 0;
		chunkCountX = cellCountX / HexMetrics.chunkSizeX;
		chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;
		// Debug.Log ("cellShaderData   " + (cellShaderData == null));
		//着色器初始化
		cellShaderData.Initialize (cellCountX, cellCountZ);
		//创建分块
		CreateChunks ();
		//创建单元
		CreateCells ();
		return true;
	}
	/// <summary>
	/// 创建块
	/// </summary>
	void CreateChunks () {
		columns = new Transform[chunkCountX];
		for (int x = 0; x < chunkCountX; x++) {
			columns[x] = new GameObject ("Column").transform;
			columns[x].SetParent (transform, false);
		}

		chunks = new HexGridChunk[chunkCountX * chunkCountZ];
		for (int z = 0, i = 0; z < chunkCountZ; z++) {
			for (int x = 0; x < chunkCountX; x++) {
				HexGridChunk chunk = chunks[i++] = Instantiate (chunkPrefab);
				chunk.transform.SetParent (columns[x], false);
			}
		}
	}
	/// <summary>
	/// 创建单元
	/// </summary>
	void CreateCells () {
		cells = new HexCell[cellCountZ * cellCountX];

		for (int z = 0, i = 0; z < cellCountZ; z++) {
			for (int x = 0; x < cellCountX; x++) {
				CreateCell (x, z, i++);
			}
		}
	}
	/// <summary>
	/// 清除所有武将
	/// </summary>
	void ClearUnits () {
		for (int i = 0; i < units.Count; i++) {
			units[i].Die ();
		}
		units.Clear ();
	}

	void OnEnable () {
		//如果此时噪声生成的贴图依然是空的，那么就再次生成
		if (!HexMetrics.noiseSource) {
			HexMetrics.noiseSource = noiseSource;
			HexMetrics.InitializeHashGrid (seed);
			HexUnit.unitPrefab = unitPrefab;
			HexUnit.unitCity = cityPrefab;
			HexMetrics.wrapSize = wrapping ? cellCountX : 0;
			ResetVisibility ();
		}
	}
	/// <summary>
	/// 选中某个单元，通过射线获取
	/// </summary>
	/// <param name="ray"></param>
	/// <returns></returns>
	public HexCell GetCell (Ray ray) {
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			return GetCell (hit.point);
		}
		return null;
	}
	/// <summary>
	/// 选中某个单元，通过坐标
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	public HexCell GetCell (Vector3 position) {
		position = transform.InverseTransformPoint (position);
		HexCoordinates coordinates = HexCoordinates.FromPosition (position);
		return GetCell (coordinates);
	}
	/// <summary>
	/// 通过数组坐标获取单元
	/// </summary>
	/// <param name="coordinates"></param>
	/// <returns></returns>
	public HexCell GetCell (HexCoordinates coordinates) {
		int z = coordinates.Z;
		if (z < 0 || z >= cellCountZ) {
			return null;
		}
		int x = coordinates.X + z / 2;
		if (x < 0 || x >= cellCountX) {
			return null;
		}
		return cells[x + z * cellCountX];
	}
	/// <summary>
	/// 通过指定值获取
	/// </summary>
	/// <param name="xOffset"></param>
	/// <param name="zOffset"></param>
	/// <returns></returns>
	public HexCell GetCell (int xOffset, int zOffset) {
		return cells[xOffset + zOffset * cellCountX];
	}
	/// <summary>
	/// 通过索引获取
	/// </summary>
	/// <param name="cellIndex"></param>
	/// <returns></returns>
	public HexCell GetCell (int cellIndex) {
		return cells[cellIndex];
	}
	/// <summary>
	/// 显示UI
	/// </summary>
	/// <param name="visible"></param>
	public void ShowUI (bool visible) {
		for (int i = 0; i < chunks.Length; i++) {
			chunks[i].ShowUI (visible);
		}
	}
	/// <summary>
	/// 创建单元
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
	/// <param name="i"></param>
	void CreateCell (int x, int z, int i) {
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * HexMetrics.innerDiameter;
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell> (cellPrefab);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates (x, z);
		cell.Index = i;
		cell.ColumnIndex = x / HexMetrics.chunkSizeX;
		cell.ShaderData = cellShaderData;

		if (wrapping) {
			cell.Explorable = z > 0 && z < cellCountZ - 1;
		} else {
			cell.Explorable =
				x > 0 && z > 0 && x < cellCountX - 1 && z < cellCountZ - 1;
		}

		if (x > 0) {
			cell.SetNeighbor (HexDirection.W, cells[i - 1]);
			if (wrapping && x == cellCountX - 1) {
				cell.SetNeighbor (HexDirection.E, cells[i - x]);
			}
		}
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor (HexDirection.SE, cells[i - cellCountX]);
				if (x > 0) {
					cell.SetNeighbor (HexDirection.SW, cells[i - cellCountX - 1]);
				} else if (wrapping) {
					cell.SetNeighbor (HexDirection.SW, cells[i - 1]);
				}
			} else {
				cell.SetNeighbor (HexDirection.SW, cells[i - cellCountX]);
				if (x < cellCountX - 1) {
					cell.SetNeighbor (HexDirection.SE, cells[i - cellCountX + 1]);
				} else if (wrapping) {
					cell.SetNeighbor (
						HexDirection.SE, cells[i - cellCountX * 2 + 1]
					);
				}
			}
		}

		Text label = Instantiate<Text> (cellLabelPrefab);
		label.rectTransform.anchoredPosition =
			new Vector2 (position.x, position.z);
		cell.uiRect = label.rectTransform;

		cell.Elevation = 0;

		AddCellToChunk (x, z, cell);
	}
	/// <summary>
	/// 把单元创建到分块中
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
	/// <param name="cell"></param>
	void AddCellToChunk (int x, int z, HexCell cell) {
		int chunkX = x / HexMetrics.chunkSizeX;
		int chunkZ = z / HexMetrics.chunkSizeZ;
		HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

		int localX = x - chunkX * HexMetrics.chunkSizeX;
		int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
		chunk.AddCell (localX + localZ * HexMetrics.chunkSizeX, cell);
	}
	/// <summary>
	/// 保存地图的所有信息
	/// </summary>
	/// <param name="writer"></param>
	public void Save (BinaryWriter writer) {
		//保存宽高
		writer.Write (cellCountX);
		writer.Write (cellCountZ);
		writer.Write (wrapping);
		//保存每个单元
		for (int i = 0; i < cells.Length; i++) {
			cells[i].Save (writer);
		}
		//保存每个物件
		writer.Write (units.Count);
		for (int i = 0; i < units.Count; i++) {
			units[i].Save (writer);
		}
	}
	/// <summary>
	/// 加载地图
	/// </summary>
	/// <param name="reader"></param>
	/// <param name="header"></param>
	public void Load (BinaryReader reader, int header) {
		//清理路径
		ClearPath ();
		//清理物件
		ClearUnits ();
		int x = 20, z = 15;
		if (header >= 1) {
			x = reader.ReadInt32 ();
			z = reader.ReadInt32 ();
		}
		bool wrapping = header >= 5 ? reader.ReadBoolean () : false;
		if (x != cellCountX || z != cellCountZ || this.wrapping != wrapping) {
			if (!CreateMap (x, z, wrapping)) {
				return;
			}
		}

		bool originalImmediateMode = cellShaderData.ImmediateMode;
		cellShaderData.ImmediateMode = true;
		//加载每一个单元格
		for (int i = 0; i < cells.Length; i++) {
			cells[i].Load (reader, header);
		}
		//刷新分块
		for (int i = 0; i < chunks.Length; i++) {
			chunks[i].Refresh ();
		}
		//加载物件
		if (header >= 2) {
			int unitCount = reader.ReadInt32 ();
			for (int i = 0; i < unitCount; i++) {
				HexUnit.Load (reader, this);
			}
		}

		cellShaderData.ImmediateMode = originalImmediateMode;
	}
	/// <summary>
	/// 获得一个路径
	/// </summary>
	/// <returns></returns>
	public List<HexCell> GetPath () {
		if (!currentPathExists) {
			return null;
		}
		List<HexCell> path = ListPool<HexCell>.Get ();
		for (HexCell c = currentPathTo; c != currentPathFrom; c = c.PathFrom) {
			path.Add (c);
		}
		path.Add (currentPathFrom);
		path.Reverse ();
		return path;
	}
	/// <summary>
	/// 清理掉路径
	/// </summary>
	public void ClearPath () {
		if (currentPathExists) {
			HexCell current = currentPathTo;
			while (current != currentPathFrom) {
				current.SetLabel (null);
				current.DisableHighlight ();
				current = current.PathFrom;
			}
			current.DisableHighlight ();
			currentPathExists = false;
		} else if (currentPathFrom) {
			currentPathFrom.DisableHighlight ();
			currentPathTo.DisableHighlight ();
		}
		currentPathFrom = currentPathTo = null;
	}
	/// <summary>
	/// 显示路径
	/// </summary>
	/// <param name="speed"></param>
	void ShowPath (int speed) {
		if (currentPathExists) {
			HexCell current = currentPathTo;
			while (current != currentPathFrom) {
				int turn = (current.Distance - 1) / speed;
				current.SetLabel (turn.ToString ());
				current.EnableHighlight (Color.white);
				current = current.PathFrom;
			}
		}
		currentPathFrom.EnableHighlight (Color.blue);
		currentPathTo.EnableHighlight (Color.red);
	}
	/// <summary>
	/// 查找路径，通过指定的信息
	/// </summary>
	/// <param name="fromCell"></param>
	/// <param name="toCell"></param>
	/// <param name="unit"></param>
	public void FindPath (HexCell fromCell, HexCell toCell, HexUnit unit) {
		ClearPath ();
		currentPathFrom = fromCell;
		currentPathTo = toCell;
		currentPathExists = Search (fromCell, toCell, unit);
		ShowPath (unit.Speed);
	}
	/// <summary>
	/// 判断是否可以找到路径
	/// </summary>
	/// <param name="fromCell"></param>
	/// <param name="toCell"></param>
	/// <param name="unit"></param>
	/// <returns></returns>
	bool Search (HexCell fromCell, HexCell toCell, HexUnit unit) {
		int speed = unit.Speed;
		searchFrontierPhase += 2;
		if (searchFrontier == null) {
			searchFrontier = new HexCellPriorityQueue ();
		} else {
			searchFrontier.Clear ();
		}

		fromCell.SearchPhase = searchFrontierPhase;
		fromCell.Distance = 0;
		searchFrontier.Enqueue (fromCell);
		while (searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue ();
			current.SearchPhase += 1;

			if (current == toCell) {
				return true;
			}

			int currentTurn = (current.Distance - 1) / speed;

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor (d);
				if (
					neighbor == null ||
					neighbor.SearchPhase > searchFrontierPhase
				) {
					continue;
				}
				if (!unit.IsValidDestination (neighbor)) {
					continue;
				}
				int moveCost = unit.GetMoveCost (current, neighbor, d);
				if (moveCost < 0) {
					continue;
				}

				int distance = current.Distance + moveCost;
				int turn = (distance - 1) / speed;
				if (turn > currentTurn) {
					distance = turn * speed + moveCost;
				}

				if (neighbor.SearchPhase < searchFrontierPhase) {
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = distance;
					//记录链接的点
					neighbor.PathFrom = current;
					neighbor.SearchHeuristic =
						neighbor.coordinates.DistanceTo (toCell.coordinates);
					searchFrontier.Enqueue (neighbor);
				} else if (distance < neighbor.Distance) {
					int oldPriority = neighbor.SearchPriority;
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					searchFrontier.Change (neighbor, oldPriority);
				}
			}
		}
		return false;
	}

	/// <summary>
	/// 找到有效路径，即使无法到达目的地，也要找到更接近目的地的位置
	/// </summary>
	public void FindEnablePath (HexCell fromCell, HexCell toCell, HexUnit unit) {
		ClearPath ();
		currentPathFrom = fromCell;
		SearchEnable (fromCell, toCell, unit);
		currentPathExists = true;
		// ShowPath (unit.Speed);
	}

	/// <summary>
	/// 找路径，当目的地无法到达时，找一个最接近目的地在位置
	/// </summary>
	/// <param name="fromCell"></param>
	/// <param name="toCell"></param>
	/// <param name="unit"></param>
	/// <returns></returns>
	bool SearchEnable (HexCell fromCell, HexCell toCell, HexUnit unit) {
		HexCell tempCell = null;
		int speed = unit.Speed;
		//查找优先级
		searchFrontierPhase += 2;
		//优先队列
		if (searchFrontier == null) {
			searchFrontier = new HexCellPriorityQueue ();
		} else {
			searchFrontier.Clear ();
		}
		fromCell.SearchPhase = searchFrontierPhase;
		fromCell.Distance = 0;
		searchFrontier.Enqueue (fromCell);
		while (searchFrontier.Count > 0) {
			//取出队列中的单元
			HexCell current = searchFrontier.Dequeue ();
			current.SearchPhase += 1;
			//找到目的地
			if (current == toCell) {
				currentPathTo = toCell;
				return true;
			}

			int currentTurn = (current.Distance - 1) / speed;
			//查找六个方向
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor (d);
				if (
					neighbor == null ||
					neighbor.SearchPhase > searchFrontierPhase
				) {
					//空的，或者已经查找过
					continue;
				}
				if (!unit.IsValidDestination (neighbor)) {
					//不是有效目的地
					continue;
				}
				//计算花费
				int moveCost = unit.GetMoveCost (current, neighbor, d);
				if (moveCost < 0) {
					//无法到达
					continue;
				}

				int distance = current.Distance + moveCost;
				int turn = (distance - 1) / speed;
				if (turn > currentTurn) {
					distance = turn * speed + moveCost;
				}

				if (neighbor.SearchPhase < searchFrontierPhase) {
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					neighbor.SearchHeuristic =
						neighbor.coordinates.DistanceTo (toCell.coordinates);
					searchFrontier.Enqueue (neighbor);
				} else if (distance < neighbor.Distance) {
					int oldPriority = neighbor.SearchPriority;
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					searchFrontier.Change (neighbor, oldPriority);
				}
				tempCell = neighbor;
			}
		}
		currentPathTo = tempCell;
		return false;
	}

	/// <summary>
	/// ????
	/// </summary>
	/// <param name="fromCell"></param>
	/// <param name="range"></param>
	public void IncreaseVisibility (HexCell fromCell, int range) {
		List<HexCell> cells = GetVisibleCells (fromCell, range);
		for (int i = 0; i < cells.Count; i++) {
			cells[i].IncreaseVisibility ();
		}
		ListPool<HexCell>.Add (cells);
	}
	/// <summary>
	/// ????
	/// </summary>
	/// <param name="fromCell"></param>
	/// <param name="range"></param>
	public void DecreaseVisibility (HexCell fromCell, int range) {
		List<HexCell> cells = GetVisibleCells (fromCell, range);
		for (int i = 0; i < cells.Count; i++) {
			cells[i].DecreaseVisibility ();
		}
		ListPool<HexCell>.Add (cells);
	}
	/// <summary>
	/// ???重置可见
	/// </summary>
	public void ResetVisibility () {
		for (int i = 0; i < cells.Length; i++) {
			cells[i].ResetVisibility ();
		}
		for (int i = 0; i < units.Count; i++) {
			HexUnit unit = units[i];
			IncreaseVisibility (unit.Location, unit.VisionRange);
		}
	}

	/// <summary>
	/// 获得可见的单元
	/// </summary>
	/// <param name="fromCell"></param>
	/// <param name="range"></param>
	/// <returns></returns>
	List<HexCell> GetVisibleCells (HexCell fromCell, int range) {
		List<HexCell> visibleCells = ListPool<HexCell>.Get ();

		searchFrontierPhase += 2;
		if (searchFrontier == null) {
			searchFrontier = new HexCellPriorityQueue ();
		} else {
			searchFrontier.Clear ();
		}

		range += fromCell.ViewElevation;
		fromCell.SearchPhase = searchFrontierPhase;
		fromCell.Distance = 0;
		searchFrontier.Enqueue (fromCell);
		HexCoordinates fromCoordinates = fromCell.coordinates;
		while (searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue ();
			current.SearchPhase += 1;
			visibleCells.Add (current);

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor (d);
				if (
					neighbor == null ||
					neighbor.SearchPhase > searchFrontierPhase ||
					!neighbor.Explorable
				) {
					continue;
				}

				int distance = current.Distance + 1;
				if (distance + neighbor.ViewElevation > range ||
					distance > fromCoordinates.DistanceTo (neighbor.coordinates)
				) {
					continue;
				}

				if (neighbor.SearchPhase < searchFrontierPhase) {
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = distance;
					neighbor.SearchHeuristic = 0;
					searchFrontier.Enqueue (neighbor);
				} else if (distance < neighbor.Distance) {
					int oldPriority = neighbor.SearchPriority;
					neighbor.Distance = distance;
					searchFrontier.Change (neighbor, oldPriority);
				}
			}
		}
		return visibleCells;
	}
	/// <summary>
	/// ？？？
	/// </summary>
	/// <param name="xPosition"></param>
	public void CenterMap (float xPosition) {
		int centerColumnIndex = (int)
			(xPosition / (HexMetrics.innerDiameter * HexMetrics.chunkSizeX));

		if (centerColumnIndex == currentCenterColumnIndex) {
			return;
		}
		currentCenterColumnIndex = centerColumnIndex;

		int minColumnIndex = centerColumnIndex - chunkCountX / 2;
		int maxColumnIndex = centerColumnIndex + chunkCountX / 2;

		Vector3 position;
		position.y = position.z = 0f;
		if (columns == null) return;
		for (int i = 0; i < columns.Length; i++) {
			if (i < minColumnIndex) {
				position.x = chunkCountX *
					(HexMetrics.innerDiameter * HexMetrics.chunkSizeX);
			} else if (i > maxColumnIndex) {
				position.x = chunkCountX *
					-(HexMetrics.innerDiameter * HexMetrics.chunkSizeX);
			} else {
				position.x = 0f;
			}
			columns[i].localPosition = position;
		}
	}
}