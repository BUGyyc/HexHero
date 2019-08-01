using UnityEngine;
/// <summary>
/// 地图场景的相机
/// </summary>
public class HexMapCamera : MonoBehaviour {
	/// <summary>
	/// 相关大小范围的设定
	/// </summary>
	public float stickMinZoom, stickMaxZoom;
	/// <summary>
	/// 相关大小范围的设定
	/// </summary>
	public float swivelMinZoom, swivelMaxZoom;

	public float moveSpeedMinZoom, moveSpeedMaxZoom;
	/// <summary>
	/// 旋转速度
	/// </summary>
	public float rotationSpeed;

	Transform swivel, stick;

	public HexGrid grid;

	float zoom = 1f;

	float rotationAngle;
	/// <summary>
	/// 取单例？？？感觉容易代码混乱
	/// </summary>
	static HexMapCamera instance;
	/// <summary>
	/// 加锁
	/// </summary>
	/// <value></value>
	public static bool Locked {
		set {
			instance.enabled = !value;
		}
	}

	private static Transform lookAtTransform = null;

	private static bool isAutoMove = false;

	private static float l_x = 0;
	private static float l_z = 0;

	public static void ValidatePosition () {
		instance.AdjustPosition (0f, 0f);
	}
	/// <summary>
	/// 设置相机坐标
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static void setCameraPosition (float x, float y) {
		instance.AdjustPosition (x, y);
	}

	void Awake () {
		swivel = transform.GetChild (0);
		stick = swivel.GetChild (0);
		CEventDispatcherObj.cEventDispatcher.addEventListener (CEventName.CAMERA_FOLLOW_TARGET, this.onSetLookAt);

		CEventDispatcherObj.cEventDispatcher.addEventListener (CEventName.CAMERA_FOLLOW_HERO, this.onFollowHero);
	}

	void OnEnable () {
		instance = this;
		ValidatePosition ();
	}

	private void OnDestroy () {
		CEventDispatcherObj.cEventDispatcher.removeEventListener (CEventName.CAMERA_FOLLOW_TARGET, this.onSetLookAt);
		CEventDispatcherObj.cEventDispatcher.removeEventListener (CEventName.CAMERA_FOLLOW_HERO, this.onFollowHero);
	}

	/// <summary>
	/// 设置镜头跟随英雄
	/// </summary>
	/// <param name="cvt"></param>
	private void onFollowHero (CEvent cvt) {
		HexUnit unit = cvt.eventParams as HexUnit;
		if (unit) SetLookAt (unit.transform);
	}

	void Update () {
		InputLogic ();

		if (isAutoMove) {
			onAutoCameraFollow ();
		}
	}

	void onAutoCameraFollow () {
		if (lookAtTransform == null) return;
		Vector3 targetPosition = lookAtTransform.localPosition;
		Vector3 temp = Vector3.Lerp (transform.localPosition, targetPosition, Time.deltaTime * 5);
		transform.localPosition = ClampPosition (temp);
	}

	private void onSetLookAt (CEvent evt) {
		HexCell cell = evt.eventParams as HexCell;
		if (cell) SetLookAt (cell.transform);
	}

	/// <summary>
	/// 设置跟随的对象
	/// </summary>
	/// <param name="gameObject"></param>
	public static void SetLookAt (Transform t) {
		lookAtTransform = t;
		isAutoMove = true;
	}

	public static void SetLookAt (float x, float z) {
		l_x = x;
		l_z = z;
		isAutoMove = true;
	}
	/// <summary>
	/// 清理掉被跟随的对象
	/// </summary>
	public static void CancelLookAt () {
		lookAtTransform = null;
		isAutoMove = false;
	}

	/// <summary>
	/// 输入控制
	/// </summary>
	void InputLogic () {
		float zoomDelta = Input.GetAxis ("Mouse ScrollWheel");
		if (zoomDelta != 0f) {
			AdjustZoom (zoomDelta);
		}

		float rotationDelta = Input.GetAxis ("Rotation");
		if (rotationDelta != 0f) {
			AdjustRotation (rotationDelta);
		}

		float xDelta = Input.GetAxis ("Horizontal");
		float zDelta = Input.GetAxis ("Vertical");
		if (xDelta != 0f || zDelta != 0f) {
			AdjustPosition (xDelta, zDelta);
		}
	}

	void AdjustZoom (float delta) {
		zoom = Mathf.Clamp01 (zoom + delta);

		float distance = Mathf.Lerp (stickMinZoom, stickMaxZoom, zoom);
		stick.localPosition = new Vector3 (0f, 0f, distance);

		float angle = Mathf.Lerp (swivelMinZoom, swivelMaxZoom, zoom);
		swivel.localRotation = Quaternion.Euler (angle, 0f, 0f);
	}

	void AdjustRotation (float delta) {
		rotationAngle += delta * rotationSpeed * Time.deltaTime;
		if (rotationAngle < 0f) {
			rotationAngle += 360f;
		} else if (rotationAngle >= 360f) {
			rotationAngle -= 360f;
		}
		transform.localRotation = Quaternion.Euler (0f, rotationAngle, 0f);
	}

	void AdjustPosition (float xDelta, float zDelta) {
		Vector3 direction =
			transform.localRotation *
			new Vector3 (xDelta, 0f, zDelta).normalized;
		float damping = Mathf.Max (Mathf.Abs (xDelta), Mathf.Abs (zDelta));
		float distance =
			Mathf.Lerp (moveSpeedMinZoom, moveSpeedMaxZoom, zoom) *
			damping * Time.deltaTime;

		Vector3 position = transform.localPosition;
		position += direction * distance;
		transform.localPosition =
			grid.wrapping ? WrapPosition (position) : ClampPosition (position);
	}

	Vector3 ClampPosition (Vector3 position) {
		float xMax = (grid.cellCountX - 0.5f) * HexMetrics.innerDiameter;
		position.x = Mathf.Clamp (position.x, 0f, xMax);

		float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
		position.z = Mathf.Clamp (position.z, 0f, zMax);

		return position;
	}

	Vector3 WrapPosition (Vector3 position) {
		float width = grid.cellCountX * HexMetrics.innerDiameter;
		while (position.x < 0f) {
			position.x += width;
		}
		while (position.x > width) {
			position.x -= width;
		}

		float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
		position.z = Mathf.Clamp (position.z, 0f, zMax);

		grid.CenterMap (position.x);
		return position;
	}
}