using System;
using UnityEngine;

// Token: 0x02000022 RID: 34
public class RPG_Camera : MonoBehaviour
{
    // Token: 0x06000178 RID: 376 RVA: 0x000103AC File Offset: 0x0000E5AC
    private void Start()
    {
        if (this.lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Token: 0x06000179 RID: 377 RVA: 0x000103C2 File Offset: 0x0000E5C2
    private void LateUpdate()
    {
        this.updateCamera();
    }

    // Token: 0x0600017A RID: 378 RVA: 0x000103CC File Offset: 0x0000E5CC
    private void updateCamera()
    {
        this.yaw += Input.GetAxis("Mouse X") * this.mouseSensitivity;
        this.pitch -= Input.GetAxis("Mouse Y") * this.mouseSensitivity;
        this.pitch = Mathf.Clamp(this.pitch, this.pitchMinMax.x, this.pitchMinMax.y);
        this.currentRotation = Vector3.SmoothDamp(this.currentRotation, new Vector3(this.pitch, this.yaw), ref this.rotationSmoothVelocity, this.rotationSmoothTime);
        base.transform.eulerAngles = this.currentRotation;
        base.transform.position = this.target.position - base.transform.forward * this.dstFromTarget;
        this.updateCollisions();
    }

    // Token: 0x0600017B RID: 379 RVA: 0x000104B4 File Offset: 0x0000E6B4
    private void updateCollisions()
    {
        Vector3 position = base.transform.position;
        Vector3 vector = base.transform.position - (this.target.position + this.currentRotation);
        float maxDistance = this.Distance(this.target.position + this.currentRotation, base.transform.position);
        if (Physics.Raycast(this.target.position + this.currentRotation, vector, out this.hit, maxDistance, this.collisionLayer))
        {
            position = this.hit.point + base.transform.forward / 1f;
            Debug.DrawRay(this.target.position + this.currentRotation, vector, Color.green);
        }
        base.transform.position = position;
    }

    // Token: 0x0600017C RID: 380 RVA: 0x000105A0 File Offset: 0x0000E7A0
    public float Distance(Vector3 a, Vector3 b)
    {
        Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
    }

    // Token: 0x0400021D RID: 541
    public bool lockCursor;

    // Token: 0x0400021E RID: 542
    public float mouseSensitivity = 10f;

    // Token: 0x0400021F RID: 543
    public Transform target;

    // Token: 0x04000220 RID: 544
    public float dstFromTarget = 2f;

    // Token: 0x04000221 RID: 545
    public Vector2 pitchMinMax = new Vector2(-40f, 85f);

    // Token: 0x04000222 RID: 546
    public float rotationSmoothTime = 0.12f;

    // Token: 0x04000223 RID: 547
    private Vector3 rotationSmoothVelocity;

    // Token: 0x04000224 RID: 548
    private Vector3 currentRotation;

    // Token: 0x04000225 RID: 549
    private float yaw;

    // Token: 0x04000226 RID: 550
    private float pitch;

    // Token: 0x04000227 RID: 551
    public LayerMask collisionLayer;

    // Token: 0x04000228 RID: 552
    private RaycastHit hit;

    // Token: 0x04000229 RID: 553
    public static RPG_Camera instance;

    // Token: 0x0400022A RID: 554
    public static Camera MainCamera;

    // Token: 0x0400022B RID: 555
    public Transform cameraPivot;

    // Token: 0x0400022C RID: 556
    public float distance = 5f;

    // Token: 0x0400022D RID: 557
    public float distanceMax = 30f;

    // Token: 0x0400022E RID: 558
    public float distanceMin = 2f;

    // Token: 0x0400022F RID: 559
    public float mouseSpeed = 8f;

    // Token: 0x04000230 RID: 560
    public float mouseScroll = 15f;

    // Token: 0x04000231 RID: 561
    public float mouseSmoothingFactor = 0.08f;

    // Token: 0x04000232 RID: 562
    public float camDistanceSpeed = 0.7f;

    // Token: 0x04000233 RID: 563
    public float camBottomDistance = 1f;

    // Token: 0x04000234 RID: 564
    public float firstPersonThreshold = 0.8f;

    // Token: 0x04000235 RID: 565
    public float characterFadeThreshold = 1.8f;

    // Token: 0x04000236 RID: 566
    public Vector3 desiredPosition;

    // Token: 0x04000237 RID: 567
    public float desiredDistance;

    // Token: 0x04000238 RID: 568
    private float lastDistance;

    // Token: 0x04000239 RID: 569
    public float mouseX;

    // Token: 0x0400023A RID: 570
    public float mouseXSmooth;

    // Token: 0x0400023B RID: 571
    private float mouseXVel;

    // Token: 0x0400023C RID: 572
    public float mouseY;

    // Token: 0x0400023D RID: 573
    public float mouseYSmooth;

    // Token: 0x0400023E RID: 574
    private float mouseYVel;

    // Token: 0x0400023F RID: 575
    private float mouseYMin = -89.5f;

    // Token: 0x04000240 RID: 576
    private float mouseYMax = 89.5f;

    // Token: 0x04000241 RID: 577
    private float distanceVel;

    // Token: 0x04000242 RID: 578
    private bool camBottom;

    // Token: 0x04000243 RID: 579
    private bool constraint;

    // Token: 0x04000244 RID: 580
    public bool invertAxisX;

    // Token: 0x04000245 RID: 581
    public bool invertAxisY;

    // Token: 0x04000246 RID: 582
    public float sensitivity;

    // Token: 0x04000247 RID: 583
    private static float halfFieldOfView;

    // Token: 0x04000248 RID: 584
    private static float planeAspect;

    // Token: 0x04000249 RID: 585
    private static float halfPlaneHeight;

    // Token: 0x0400024A RID: 586
    private static float halfPlaneWidth;

    // Token: 0x0200004B RID: 75
    public struct ClipPlaneVertexes
    {
        // Token: 0x04000323 RID: 803
        public Vector3 UpperLeft;

        // Token: 0x04000324 RID: 804
        public Vector3 UpperRight;

        // Token: 0x04000325 RID: 805
        public Vector3 LowerLeft;

        // Token: 0x04000326 RID: 806
        public Vector3 LowerRight;
    }
}
