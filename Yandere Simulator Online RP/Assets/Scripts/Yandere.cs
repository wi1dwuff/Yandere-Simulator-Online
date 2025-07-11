using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000011 RID: 17
public class Yandere : MonoBehaviour
{
    // Token: 0x06000046 RID: 70 RVA: 0x00004F45 File Offset: 0x00003145
    private void Start()
    {
        this.controller = base.GetComponent<CharacterController>();
        this.animations = base.GetComponent<Animation>();
    }

    // Token: 0x06000047 RID: 71 RVA: 0x00004F60 File Offset: 0x00003160
    public void Update()
    {
        if (Input.GetKeyDown("=") && Time.timeScale < 9f)
        {
            Time.timeScale += 1f;
        }
        if (Input.GetKeyDown("-") && Time.timeScale > 1f)
        {
            Time.timeScale -= 1f;
        }
        if (Input.GetKeyDown("`"))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }
        if (base.transform.position.y < -5f)
        {
            base.transform.position = Vector3.zero;
        }
        this.Running = (this.CanMove && Input.GetKey(KeyCode.LeftShift));
        if (this.CanMove)
        {
            this.controller.Move(Physics.gravity * 0.1f);
            float axisRaw = Input.GetAxisRaw("Vertical");
            float axisRaw2 = Input.GetAxisRaw("Horizontal");
            if (this.mainCamera.orthographic)
            {
                this.targetDirection = new Vector3(axisRaw2, 0f, axisRaw);
            }
            else
            {
                Vector3 a = this.mainCamera.transform.TransformDirection(Vector3.forward);
                a.y = 0f;
                a = a.normalized;
                Vector3 a2 = new Vector3(a.z, 0f, -a.x);
                this.targetDirection = axisRaw2 * a2 + axisRaw * a;
            }
            if (this.targetDirection != Vector3.zero)
            {
                this.targetRotation = Quaternion.LookRotation(this.targetDirection);
                base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.targetRotation, Time.deltaTime * 10f);
            }
            else
            {
                this.targetRotation = new Quaternion(0f, 0f, 0f, 0f);
            }
            if (axisRaw != 0f || axisRaw2 != 0f)
            {
                this.animations.CrossFade((!this.Running) ? this.WalkAnimation : this.SprintAnimation);
                this.controller.Move(base.transform.forward * Time.deltaTime * ((!this.Running) ? this.WalkSpeed : this.RunSpeed));
            }
            else
            {
                this.animations.CrossFade(this.IdleAnimation);
            }
        }
        else
        {
            this.animations.CrossFade(this.IdleAnimation);
        }
    }

    // Token: 0x04000078 RID: 120
    public bool CanMove = true;

    // Token: 0x04000079 RID: 121
    public bool Running;

    // Token: 0x0400007A RID: 122
    [Space]
    public float WalkSpeed = 1f;

    // Token: 0x0400007B RID: 123
    public float RunSpeed = 5f;

    // Token: 0x0400007C RID: 124
    [Space]
    public string IdleAnimation = "f02_idleShort_00";

    // Token: 0x0400007D RID: 125
    public string WalkAnimation = "f02_newWalk_00";

    // Token: 0x0400007E RID: 126
    public string SprintAnimation = "f02_newSprint_00";

    // Token: 0x0400007F RID: 127
    [Space]
    public Camera mainCamera;

    // Token: 0x04000080 RID: 128
    private CharacterController controller;

    // Token: 0x04000081 RID: 129
    private Animation animations;

    // Token: 0x04000082 RID: 130
    private Vector3 targetDirection;

    // Token: 0x04000083 RID: 131
    private Quaternion targetRotation;
}

