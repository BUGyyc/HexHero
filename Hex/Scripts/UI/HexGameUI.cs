using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// 游戏输入操作
/// </summary>
public class HexGameUI : MonoBehaviour {

	public HexGrid grid;

	HexCell currentCell;

	HexCell lastCell;

	HexUnit selectedUnit;

	HexUnit nextUnit; //第二次选中的物件

	HexUnit lastUnit;

	// HexCell[] linghtCells; //高亮的单元
	List<HexCell> lightCellList;

	public HexCell[] cells;

	public GameObject hexGameCamera;

	/// <summary>
	/// 点击状态
	/// </summary>
	enum ClickState {
		Normal, //普通状态
		Select_Path, //选择行走状态
		Choose_City, //选中城池状态
	}

	ClickState myClickState;

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start () {
		cells = null;
		myClickState = ClickState.Normal;
		lightCellList = new List<HexCell> ();
	}

	/// <summary>
	/// 点击选中英雄单位或者城池
	/// </summary>
	private void onMouseClickHandler () {

		//创建英雄模式
		if (Global.isEditMode == true) {

			//左键选择单位
			if (Input.GetMouseButtonDown (0)) {
				//清理路径
				grid.ClearPath ();
				//更新选中的单元
				UpdateCurrentCell ();
				//选中位置不为空
				if (currentCell) {
					//在所选位置创建
					CreateUnit (currentCell);
				}
			}
		}

		//取到场景上所有的单元
		if (cells == null) {
			cells = grid.getCells ();
		}
		//左键选择单位
		if (Input.GetMouseButtonDown (0)) {
			selectUnit ();
		}

		//右键测试技能
		if (Input.GetMouseButtonDown (1)) {

		}
	}

	/// <summary>
	/// 选择单元
	/// </summary>
	private void selectUnit () {
		//清理路径
		grid.ClearPath ();
		//更新选中的单元
		UpdateCurrentCell ();

		if (currentCell) {
			// MyLog.Log ("HexCell 坐标  " + currentCell.coordinates + "     " + currentCell.Position + "   " + hexGameCamera.transform.localPosition);
			// hexGameCamera.transform.localPosition = currentCell.Position;
		}

		//没有选中任何东西
		if (myClickState == ClickState.Normal) {
			//选中单元不为空
			if (currentCell) {
				//选中单元上的英雄
				selectedUnit = currentCell.Unit;
				//保存这一次的选中
				lastCell = currentCell;
				//最大步子
				int maxStep = 10;
				//有选中英雄
				if (selectedUnit) {
					//如果选中了英雄，那么久展示行走区域
					StartCoroutine (drawHeroRoadPath (currentCell, maxStep));
					//画出可以被攻击的格子
					StartCoroutine (drawHeroAttackGroup (currentCell));
					//更新点击状态为选路径状态
					myClickState = ClickState.Select_Path;
				} else {
					//没有选中英雄
					//上次的单元不为空的话，就把上次单元的光圈隐藏
					if (lastCell) lastCell.DisableHighlight ();
					//如果选中城池
					if (currentCell.IsSpecial) {
						//更新状态为选中城池状态
						myClickState = ClickState.Choose_City;
						
						// onOpenMyCityScene ();


						// //派发事件，打开UI
						// CEventDispatcherObj.cEventDispatcher.dispatchEvent (new CEvent (CEventName.OPEN_TOWN_UI), this);
					}
				}
			}
			//已选中英雄，再点击就是执行行走
		} else if (myClickState == ClickState.Select_Path) {
			//二次选择这个区域，则默认为清空选项
			if (lastCell == currentCell) {
				resetSelect ();
				//两次选中不同区域
			} else {
				//当前选择单元不为空
				if (currentCell) {
					nextUnit = currentCell.Unit;
					//没有选中英雄，则执行行走
					if (nextUnit == null) {
						//查找路径
						if (currentCell && selectedUnit.IsValidDestination (currentCell)) {
							grid.FindPath (selectedUnit.Location, currentCell, selectedUnit);
						} else {
							grid.ClearPath ();
						}
						//有路，执行路径
						if (grid.HasPath) {
							selectedUnit.Travel (grid.GetPath ());
							grid.ClearPath ();
						}
						//最后重置
						resetSelect ();
					} else {
						//执行攻击对方
						if (nextUnit != selectedUnit) {
							// selectedUnit.us

							if (lastCell.IsNeighbor (currentCell) == true) {
								Common.getInstance ().showPopTip ("---攻击对方英雄---", 1);
								selectedUnit.Attack (currentCell);
								nextUnit.Damage ();
							}
						}
					}
				}
			}
			//已选中城池
		} else if (myClickState == ClickState.Choose_City) {
			currentCell.DisableHighlight ();
			lastCell = null;
			currentCell = null;
			this.onHideAllLabel ();
			myClickState = ClickState.Normal;
		}
	}

	/// <summary>
	/// 重置选择
	/// </summary>
	void resetSelect () {
		//隐藏所有光圈
		foreach (var hc in lightCellList) {
			hc.DisableHighlight ();
		}
		//清空list
		// lightCellList.RemoveAll ();
		lightCellList.Clear ();
		//上一个单元
		lastCell = null;
		//当前单元
		currentCell = null;
		//隐藏所有距离显示
		this.onHideAllLabel ();
		//设置当前选择状态为普通状态
		myClickState = ClickState.Normal;
	}

	/// <summary>
	/// 绘制英雄的可走路径
	/// </summary>
	/// <returns></returns>
	IEnumerator drawHeroRoadPath (HexCell cell, int maxStep) {
		WaitForSeconds delay = new WaitForSeconds (1 / 100f);
		Queue<HexCell> frontier = new Queue<HexCell> ();
		cell.Distance = 0;
		frontier.Enqueue (cell);
		int count = 0;
		//从起点出发，最多找50个
		while (frontier.Count > 0 && count < 50) {
			yield return delay; //delay;
			HexCell current = frontier.Dequeue ();
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor (d);
				if (neighbor == null) {
					continue;
				}
				//如果是水，则不能走
				if (neighbor.IsUnderwater) {
					continue;
				}
				//如果是悬崖，则不能走
				if (current.GetEdgeType (neighbor) == HexEdgeType.Cliff) {
					continue;
				}
				//根据海拔相差来计算消耗
				int disH = Mathf.Abs (current.Elevation - neighbor.Elevation);
				neighbor.Distance = current.Distance + disH + 1;
				if (cell == current) { //原点蓝圈
					current.EnableHighlight (Color.blue);
					lightCellList.Add (current);
				} else {
					// current.EnableHighlight (Color.green);
					lightCellList.Add (current);
				}
				//展示距离
				// neighbor.UpdateDistanceLabel (maxStep);
				frontier.Enqueue (neighbor);
				count++;
			}

		}
	}

	/// <summary>
	/// 绘制英雄的可走路径
	/// </summary>
	/// <param name="cell"></param>
	/// <param name="max"></param>
	/// <returns></returns>
	IEnumerator drawHeroMovePath (HexCell cell, int max) {
		int l_x = grid.cellCountX;
		int l_z = grid.cellCountZ;
		HexCoordinates temp = cell.coordinates;
		int startX = temp.X;
		int startZ = temp.Z;

		for (int x = startX - 4; x < startX + 4; x++) {
			for (int z = startZ - 4; z < startZ + 4; z++) {
				//越界
				if (x < 0 || x > l_x) continue;
				//越界
				if (z < 0 || z > l_z) continue;
				//起始点
				if (x == startX && z == startZ) continue;
				HexCell l_cell = grid.GetCell (new HexCoordinates (x, z));
				if (l_cell) {
					//画出能走的部分
					l_cell.EnableHighlight (Color.green);
				}
				yield return 0;
			}
		}
	}

	/// <summary>
	/// 把能被选中英雄攻击的英雄标记出来
	/// </summary>
	/// <param name="cell"></param>
	/// <returns></returns>
	IEnumerator drawHeroAttackGroup (HexCell cell) {
		// Debug.Log ("把能被选中英雄攻击的英雄标记出来");
		HexCell temp = null;
		HexUnit tempUnit = null;
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			temp = cell.GetNeighbor (d);
			if (temp) {
				tempUnit = temp.Unit;
				if (tempUnit) { //如果是英雄
					Debug.Log ("tempUnit ==============   ", tempUnit);
					temp.EnableHighlight (Color.red);
				}
			}
			yield return 0;
		}
	}

	/// <summary>
	/// 是否开启编辑模式
	/// </summary>
	/// <param name="toggle"></param>
	public void SetEditMode (bool toggle) {
		enabled = !toggle;
		grid.ShowUI (!toggle);
		grid.ClearPath ();
		if (toggle) {
			Shader.EnableKeyword ("HEX_MAP_EDIT_MODE");
		} else {
			Shader.DisableKeyword ("HEX_MAP_EDIT_MODE");
		}
	}

	void Update () {
		//是否穿透UI层，如果穿透就直接拦截
		if (!EventSystem.current.IsPointerOverGameObject ()) {
			//监听鼠标操作
			onMouseClickHandler ();
		}
	}
	/// <summary>
	/// 选中单元
	/// </summary>
	void DoSelection () {
		grid.ClearPath ();
		UpdateCurrentCell ();
		if (currentCell) { //有选中单位
			if (currentCell.Unit) selectedUnit = currentCell.Unit;
			if (lastCell == currentCell) { //二次选择一样，则是取消选择
				currentCell.DisableHighlight ();
				lastCell = null;
				currentCell = null;
				selectedUnit = null;
				this.onHideAllLabel ();
			} else { //两次选择不一样，那么可能是执行行走
				//执行行走，执行完取消上次选择

				//不是行走，取消上次选择

				//隐藏上次的单位、高亮选中单元
				if (lastCell) lastCell.DisableHighlight ();
				// currentCell.EnableHighlight (Color.blue);

				lastCell = currentCell;
				bool isHero = true;
				int maxStep = 10;
				if (selectedUnit && isHero) { //如果有选中单元，并且选中单元是英雄 ，那么久展示可走区域
					StartCoroutine (drawHeroRoadPath (currentCell, maxStep));
				}
			}
		}
	}

	/// <summary>
	/// 隐藏所有距离lab
	/// </summary>
	void onHideAllLabel () {
		for (int i = 0; i < cells.Length; i++) {
			cells[i].Distance = int.MaxValue;
			cells[i].HideLabel ();
		}
	}
	/// <summary>
	/// 查找目标值路径
	/// </summary>
	void DoPathfinding () {
		if (UpdateCurrentCell ()) {
			if (currentCell && selectedUnit.IsValidDestination (currentCell)) {
				grid.FindPath (selectedUnit.Location, currentCell, selectedUnit);
			} else {
				grid.ClearPath ();
			}
		}
	}

	/// <summary>
	/// 执行移动
	/// </summary>
	void DoMove () {
		if (grid.HasPath) {
			selectedUnit.Travel (grid.GetPath ());
			grid.ClearPath ();
		}
	}

	/// <summary>
	/// 选择的单元
	/// </summary>
	/// <returns></returns>
	bool UpdateCurrentCell () {
		HexCell cell =
			grid.GetCell (Camera.main.ScreenPointToRay (Input.mousePosition));
		if (cell != currentCell) {
			currentCell = cell;
			return true;
		}
		return false;
	}

	public void AutoCreateUnit (HexCell cell, HeroItem heroItem) {
		onCreateUnit (cell, heroItem);
	}

	private void onCreateUnit (HexCell cell, HeroItem heroItem) {
		HeroItem item = heroItem;
		HexUnit unit = HexUnit.unitPrefab;
		unit.IsHero = true; //是否是英雄，或者城池 
		if (cell && !cell.Unit) {
			grid.AddUnit (
				Instantiate (unit), cell, 0f //Random.Range (0f, 360f)
			);
			unit.GetComponent<HeroBehaviours> ().setHeroId (item.id);
			unit.GetComponent<HeroBehaviours> ().setHeroItem (item);
			unit.GetComponent<HeroUI> ().setHeroItem (item);
			unit.GetComponent<HeroUI> ().updateHeroImf ();
		}
	}

	/// <summary>
	/// 创建物件
	/// 是英雄
	/// </summary>
	void CreateUnit (HexCell cell) {
		if (Global.heroItem == null) return;
		HeroItem item = Global.heroItem;
		HexUnit unit = HexUnit.unitPrefab;
		unit.IsHero = true; //是否是英雄，或者城池 
		if (cell && !cell.Unit) {
			grid.AddUnit (
				Instantiate (unit), cell, 0f //Random.Range (0f, 360f)
			);
			unit.GetComponent<HeroBehaviours> ().setHeroId (item.id);
			unit.GetComponent<HeroBehaviours> ().setHeroItem (item);
		}
	}

	private AsyncOperation prog;
	//打开我的城池场景
	private void onOpenMyCityScene () {
		prog = SceneManager.LoadSceneAsync ("MyCity"); //异步加载场景
		StartCoroutine ("LoadingScene");
	}

	IEnumerator LoadingScene () {
		prog.allowSceneActivation = false;
		int toProgress = 0;
		int showProgress = 0;
		while (prog.progress < 0.9f) {
			toProgress = (int) (prog.progress * 100);
			while (showProgress < toProgress) {
				showProgress++;
			}
			yield return new WaitForEndOfFrame (); //等待一帧
		}
		//计算0.9---1   其实0.9就是加载好了，我估计真正进入到场景是1  
		toProgress = 100;
		while (showProgress < toProgress) {
			showProgress++;
			yield return new WaitForEndOfFrame (); //等待一帧
		}
		prog.allowSceneActivation = true; //如果加载完成，可以进入场景
	}
}
///
/// /////////////////////////////////////////////////////////////////////////////
// /// <summary>
// 	/// 选择单元
// 	/// </summary>
// 	private void selectUnit () {
// 		grid.ClearPath ();
// 		UpdateCurrentCell ();
// 		if (myClickState == ClickState.Normal) { //没有选中任何东西
// 			if (currentCell) {
// 				// currentCell.EnableHighlight (Color.blue);
// 				// lightCellList.Add (currentCell);
// 				// linghtCells
// 				selectedUnit = currentCell.Unit;
// 				lastCell = currentCell; //保存这一次的选中
// 				bool isHero = true;
// 				// myClickState = (isHero) ? ClickState.Select_Path : ClickState.Choose_City;
// 				int maxStep = 10;
// 				if (selectedUnit) {
// 					if (isHero) {
// 						//选中英雄
// 						// HexMapCamera.SetLookAt (currentCell.coordinates.X, currentCell.coordinates.Z); // (selectedUnit.gameObject);
// 						Common.getInstance ().showPopTip ("选中英雄", 1);
// 						StartCoroutine (drawHeroRoadPath (currentCell, maxStep)); //如果选中了英雄，那么久展示行走区域
// 						StartCoroutine (drawHeroAttackGroup (currentCell));
// 						myClickState = ClickState.Select_Path;
// 					} else {
// 						//选中城池
// 						myClickState = ClickState.Choose_City;
// 						CEventDispatcherObj.cEventDispatcher.dispatchEvent (new CEvent (CEventName.OPEN_TOWN_UI), this);
// 					}
// 				} else {
// 					if (lastCell) lastCell.DisableHighlight ();
// 				}
// 			}
// 		} else if (myClickState == ClickState.Select_Path) { //已选中英雄，再点击就是执行行走
// 			if (lastCell == currentCell) { //二次选择这个区域
// 				resetSelect ();
// 			} else { //两次选中不同区域
// 				if (currentCell) {
// 					nextUnit = currentCell.Unit;
// 					if (nextUnit == null) {
// 						if (currentCell && selectedUnit.IsValidDestination (currentCell)) {
// 							grid.FindPath (selectedUnit.Location, currentCell, selectedUnit);
// 						} else {
// 							grid.ClearPath ();
// 						}
// 						//有路，执行路径
// 						if (grid.HasPath) {
// 							selectedUnit.Travel (grid.GetPath ());
// 							grid.ClearPath ();
// 						}
// 						resetSelect (); //最后重置
// 					} else {
// 						if (nextUnit != selectedUnit) {
// 							if (lastCell.IsNeighbor (currentCell) == true) {
// 								Common.getInstance ().showPopTip ("---攻击对方英雄---", 1);
// 								selectedUnit.Attack (currentCell);
// 								nextUnit.Damage ();
// 							}
// 						}
// 					}
// 				}
// 			}
// 		} else if (myClickState == ClickState.Choose_City) { //已选中城池
// 			if (lastCell == currentCell) { //二次选择这个区域
// 				currentCell.DisableHighlight ();
// 				lastCell = null;
// 				currentCell = null;
// 				this.onHideAllLabel ();
// 				myClickState = ClickState.Normal;
// 			}
// 		}
// 	}

//////////////////////////////////////////////////////////////////////////////////

// IEnumerator drawHeroRoadPath (HexCell cell, int maxStep) {
// 		Debug.Log ("drawHeroRoadPath协程 ---- ");
// 		// WaitForSeconds delay = new WaitForSeconds (1 / 100f);
// 		Queue<HexCell> frontier = new Queue<HexCell> ();
// 		cell.Distance = 0;
// 		frontier.Enqueue (cell);
// 		int count = 0;
// 		//从起点出发，最多找50个
// 		while (frontier.Count > 0) { //count++;//&& count < 50
// 			yield return 0; //delay;
// 			HexCell current = frontier.Dequeue ();
// 			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
// 				HexCell neighbor = current.GetNeighbor (d);
// 				if (neighbor == null) {
// 					continue;
// 				}
// 				//如果是水，则不能走
// 				if (neighbor.IsUnderwater) {
// 					continue;
// 				}
// 				//如果是悬崖，则不能走
// 				if (current.GetEdgeType (neighbor) == HexEdgeType.Cliff) {
// 					continue;
// 				}
// 				//根据海拔相差来计算消耗
// 				int disH = Mathf.Abs (current.Elevation - neighbor.Elevation);
// 				neighbor.Distance = current.Distance + 1 + disH;
// 				if (cell == current) { //原点蓝圈
// 					current.EnableHighlight (Color.blue);
// 					lightCellList.Add (current);
// 				} else {
// 					if (neighbor.Distance < maxStep) { //可走路径为绿圈
// 						// current.EnableHighlight (Color.green);
// 						//保存到list中，方便控制所有的光圈
// 						lightCellList.Add (current);
// 					} else if (neighbor.Distance >= maxStep) {
// 						// current.DisableHighlight ();
// 						// lightCellList.Remove (current);
// 					}
// 				}
// 				//展示距离
// 				// neighbor.UpdateDistanceLabel (maxStep);
// 				frontier.Enqueue (neighbor);
// 				// count++;
// 			}

// 		}
// 	}