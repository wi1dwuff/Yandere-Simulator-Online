using System;
using UnityEngine;

public class RPG_Camera : MonoBehaviour
{
	private void Awake()
	{
		RPG_Camera.instance = this;
	}

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		RPG_Camera.MainCamera = base.GetComponent<Camera>();
		distance = Mathf.Clamp(distance, 0.05f, distanceMax);
		desiredDistance = distance;
		RPG_Camera.halfFieldOfView = RPG_Camera.MainCamera.fieldOfView / 2f * 0.017453292f;
		RPG_Camera.planeAspect = RPG_Camera.MainCamera.aspect;
		RPG_Camera.halfPlaneHeight = RPG_Camera.MainCamera.nearClipPlane * Mathf.Tan(RPG_Camera.halfFieldOfView);
		RPG_Camera.halfPlaneWidth = RPG_Camera.halfPlaneHeight * RPG_Camera.planeAspect;
		UpdateRotation();
	}

	public void UpdateRotation()
	{
		mouseX = cameraPivot.transform.eulerAngles.y;
		mouseY = 15f;
	}

	public static void CameraSetup()
	{
		GameObject gameObject;
		if (RPG_Camera.MainCamera != null)
		{
			gameObject = RPG_Camera.MainCamera.gameObject;
		}
		else
		{
			gameObject = new GameObject("Main Camera");
			gameObject.AddComponent<Camera>();
			gameObject.tag = "MainCamera";
		}
		if (!gameObject.GetComponent("RPG_Camera"))
		{
			gameObject.AddComponent<RPG_Camera>();
		}
		RPG_Camera rpg_Camera = gameObject.GetComponent("RPG_Camera") as RPG_Camera;
		GameObject gameObject2 = GameObject.Find("cameraPivot");
		rpg_Camera.cameraPivot = gameObject2.transform;
	}

	private void LateUpdate()
	{
		if (Time.deltaTime > 0f && Cursor.lockState == CursorLockMode.Locked)
		{
			GetInput();
			GetDesiredPosition();
			PositionUpdate();
		}
	}

	public void GetInput()
	{
		if ((double)distance > 0.1)
		{
			Debug.DrawLine(base.transform.position, base.transform.position - Vector3.up * camBottomDistance, Color.green);
			camBottom = Physics.Linecast(base.transform.position, base.transform.position - Vector3.up * camBottomDistance);
		}
		bool flag = camBottom && base.transform.position.y - cameraPivot.transform.position.y <= 0f;
		mouseX += Input.GetAxis("Mouse X") * mouseSpeed;
		if (flag)
		{
			if (Input.GetAxis("Mouse Y") < 0f)
			{
				mouseY -= Input.GetAxis("Mouse Y") * mouseSpeed;
			}
		}
		else
		{
			mouseY -= Input.GetAxis("Mouse Y") * mouseSpeed;
		}
		mouseY = ClampAngle(mouseY, -89.5f, 89.5f);
		mouseXSmooth = Mathf.SmoothDamp(mouseXSmooth, mouseX, ref mouseXVel, mouseSmoothingFactor);
		mouseYSmooth = Mathf.SmoothDamp(mouseYSmooth, mouseY, ref mouseYVel, mouseSmoothingFactor);
		if (flag)
		{
			mouseYMin = mouseY;
		}
		else
		{
			mouseYMin = -89.5f;
		}
		mouseYSmooth = ClampAngle(mouseYSmooth, mouseYMin, mouseYMax);
		desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * mouseScroll;
		if (desiredDistance > distanceMax)
		{
			desiredDistance = distanceMax;
		}
		if (desiredDistance < distanceMin)
		{
			desiredDistance = distanceMin;
		}
	}

	public void GetDesiredPosition()
	{
		distance = desiredDistance;
		desiredPosition = GetCameraPosition(mouseYSmooth, mouseXSmooth, distance);
		constraint = false;
		float num = CheckCameraClipPlane(cameraPivot.position, desiredPosition);
		if (num != -1f)
		{
			distance = num;
			desiredPosition = GetCameraPosition(mouseYSmooth, mouseXSmooth, distance);
			constraint = true;
		}
		if (RPG_Camera.MainCamera == null)
		{
			RPG_Camera.MainCamera = base.GetComponent<Camera>();
		}
		distance -= RPG_Camera.MainCamera.nearClipPlane;
		if (lastDistance < distance || !constraint)
		{
			distance = Mathf.SmoothDamp(lastDistance, distance, ref distanceVel, camDistanceSpeed);
		}
		if ((double)distance < 0.05)
		{
			distance = 0.05f;
		}
		lastDistance = distance;
		desiredPosition = GetCameraPosition(mouseYSmooth, mouseXSmooth, distance);
	}

	public void PositionUpdate()
	{
		base.transform.position = desiredPosition;
		if ((double)distance > 0.05)
		{
			base.transform.LookAt(cameraPivot);
		}
	}

	private Vector3 GetCameraPosition(float xAxis, float yAxis, float distance)
	{
		Vector3 point = new Vector3(0f, 0f, -distance);
		Quaternion rotation = Quaternion.Euler(xAxis, yAxis, 0f);
		return cameraPivot.position + rotation * point;
	}

	private float CheckCameraClipPlane(Vector3 from, Vector3 to)
	{
		float num = -1f;
		RPG_Camera.ClipPlaneVertexes clipPlaneAt = RPG_Camera.GetClipPlaneAt(to);
		Debug.DrawLine(clipPlaneAt.UpperLeft, clipPlaneAt.UpperRight);
		Debug.DrawLine(clipPlaneAt.UpperRight, clipPlaneAt.LowerRight);
		Debug.DrawLine(clipPlaneAt.LowerRight, clipPlaneAt.LowerLeft);
		Debug.DrawLine(clipPlaneAt.LowerLeft, clipPlaneAt.UpperLeft);
		Debug.DrawLine(from, to, Color.red);
		Debug.DrawLine(from - base.transform.right * RPG_Camera.halfPlaneWidth + base.transform.up * RPG_Camera.halfPlaneHeight, clipPlaneAt.UpperLeft, Color.cyan);
		Debug.DrawLine(from + base.transform.right * RPG_Camera.halfPlaneWidth + base.transform.up * RPG_Camera.halfPlaneHeight, clipPlaneAt.UpperRight, Color.cyan);
		Debug.DrawLine(from - base.transform.right * RPG_Camera.halfPlaneWidth - base.transform.up * RPG_Camera.halfPlaneHeight, clipPlaneAt.LowerLeft, Color.cyan);
		Debug.DrawLine(from + base.transform.right * RPG_Camera.halfPlaneWidth - base.transform.up * RPG_Camera.halfPlaneHeight, clipPlaneAt.LowerRight, Color.cyan);
		RaycastHit raycastHit;
		if (Physics.Linecast(from, to, out raycastHit) && raycastHit.collider.gameObject.layer == 0)
		{
			num = raycastHit.distance - RPG_Camera.MainCamera.nearClipPlane;
		}
		if (Physics.Linecast(from - base.transform.right * RPG_Camera.halfPlaneWidth + base.transform.up * RPG_Camera.halfPlaneHeight, clipPlaneAt.UpperLeft, out raycastHit) && raycastHit.collider.gameObject.layer == 0 && (raycastHit.distance < num || num == -1f))
		{
			num = Vector3.Distance(raycastHit.point + base.transform.right * RPG_Camera.halfPlaneWidth - base.transform.up * RPG_Camera.halfPlaneHeight, from);
		}
		if (Physics.Linecast(from + base.transform.right * RPG_Camera.halfPlaneWidth + base.transform.up * RPG_Camera.halfPlaneHeight, clipPlaneAt.UpperRight, out raycastHit) && raycastHit.collider.gameObject.layer == 0 && (raycastHit.distance < num || num == -1f))
		{
			num = Vector3.Distance(raycastHit.point - base.transform.right * RPG_Camera.halfPlaneWidth - base.transform.up * RPG_Camera.halfPlaneHeight, from);
		}
		if (Physics.Linecast(from - base.transform.right * RPG_Camera.halfPlaneWidth - base.transform.up * RPG_Camera.halfPlaneHeight, clipPlaneAt.LowerLeft, out raycastHit) && raycastHit.collider.gameObject.layer == 0 && (raycastHit.distance < num || num == -1f))
		{
			num = Vector3.Distance(raycastHit.point + base.transform.right * RPG_Camera.halfPlaneWidth + base.transform.up * RPG_Camera.halfPlaneHeight, from);
		}
		if (Physics.Linecast(from + base.transform.right * RPG_Camera.halfPlaneWidth - base.transform.up * RPG_Camera.halfPlaneHeight, clipPlaneAt.LowerRight, out raycastHit) && raycastHit.collider.gameObject.layer == 0 && (raycastHit.distance < num || num == -1f))
		{
			num = Vector3.Distance(raycastHit.point - base.transform.right * RPG_Camera.halfPlaneWidth + base.transform.up * RPG_Camera.halfPlaneHeight, from);
		}
		return num;
	}

	private float ClampAngle(float angle, float min, float max)
	{
		while (angle < -360f || angle > 360f)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
		}
		return Mathf.Clamp(angle, min, max);
	}

	public static RPG_Camera.ClipPlaneVertexes GetClipPlaneAt(Vector3 pos)
	{
		RPG_Camera.ClipPlaneVertexes result = default(RPG_Camera.ClipPlaneVertexes);
		if (RPG_Camera.MainCamera == null)
		{
			return result;
		}
		Transform transform = RPG_Camera.MainCamera.transform;
		float nearClipPlane = RPG_Camera.MainCamera.nearClipPlane;
		result.UpperLeft = pos - transform.right * RPG_Camera.halfPlaneWidth;
		result.UpperLeft += transform.up * RPG_Camera.halfPlaneHeight;
		result.UpperLeft += transform.forward * nearClipPlane;
		result.UpperRight = pos + transform.right * RPG_Camera.halfPlaneWidth;
		result.UpperRight += transform.up * RPG_Camera.halfPlaneHeight;
		result.UpperRight += transform.forward * nearClipPlane;
		result.LowerLeft = pos - transform.right * RPG_Camera.halfPlaneWidth;
		result.LowerLeft -= transform.up * RPG_Camera.halfPlaneHeight;
		result.LowerLeft += transform.forward * nearClipPlane;
		result.LowerRight = pos + transform.right * RPG_Camera.halfPlaneWidth;
		result.LowerRight -= transform.up * RPG_Camera.halfPlaneHeight;
		result.LowerRight += transform.forward * nearClipPlane;
		return result;
	}

	// Token: 0x04000016 RID: 22
	public static RPG_Camera instance;

	// Token: 0x04000017 RID: 23
	public static Camera MainCamera;

	// Token: 0x04000018 RID: 24
	public Transform cameraPivot;

	// Token: 0x04000019 RID: 25
	public float distance = 5f;

	// Token: 0x0400001A RID: 26
	public float distanceMax = 30f;

	// Token: 0x0400001B RID: 27
	public float distanceMin = 2f;

	// Token: 0x0400001C RID: 28
	public float mouseSpeed = 8f;

	// Token: 0x0400001D RID: 29
	public float mouseScroll = 15f;

	// Token: 0x0400001E RID: 30
	public float mouseSmoothingFactor = 0.08f;

	// Token: 0x0400001F RID: 31
	public float camDistanceSpeed = 0.7f;

	// Token: 0x04000020 RID: 32
	public float camBottomDistance = 1f;

	// Token: 0x04000021 RID: 33
	public float firstPersonThreshold = 0.8f;

	// Token: 0x04000022 RID: 34
	public float characterFadeThreshold = 1.8f;

	// Token: 0x04000023 RID: 35
	public Vector3 desiredPosition;

	// Token: 0x04000024 RID: 36
	public float desiredDistance;

	// Token: 0x04000025 RID: 37
	private float lastDistance;

	// Token: 0x04000026 RID: 38
	public float mouseX;

	// Token: 0x04000027 RID: 39
	public float mouseXSmooth;

	// Token: 0x04000028 RID: 40
	private float mouseXVel;

	// Token: 0x04000029 RID: 41
	public float mouseY;

	// Token: 0x0400002A RID: 42
	public float mouseYSmooth;

	// Token: 0x0400002B RID: 43
	private float mouseYVel;

	// Token: 0x0400002C RID: 44
	private float mouseYMin = -89.5f;

	// Token: 0x0400002D RID: 45
	private float mouseYMax = 89.5f;

	// Token: 0x0400002E RID: 46
	private float distanceVel;

	// Token: 0x0400002F RID: 47
	private bool camBottom;

	// Token: 0x04000030 RID: 48
	private bool constraint;

	// Token: 0x04000031 RID: 49
	private static float halfFieldOfView;

	// Token: 0x04000032 RID: 50
	private static float planeAspect;

	// Token: 0x04000033 RID: 51
	private static float halfPlaneHeight;

	// Token: 0x04000034 RID: 52
	private static float halfPlaneWidth;

	// Token: 0x02000007 RID: 7
	public struct ClipPlaneVertexes
	{
		// Token: 0x04000035 RID: 53
		public Vector3 UpperLeft;

		// Token: 0x04000036 RID: 54
		public Vector3 UpperRight;

		// Token: 0x04000037 RID: 55
		public Vector3 LowerLeft;

		// Token: 0x04000038 RID: 56
		public Vector3 LowerRight;
	}
}