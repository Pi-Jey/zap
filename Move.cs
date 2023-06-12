using UnityEngine;

public class Move : MonoBehaviour
{

    public CharacterController2D controller;
    public GameObject AndroidController;
    public Joystick joystick;
    public Animator animator;

    public float speed = 40f;
    private float HorizontalMove = 0f;

    private bool jump = false;
    private bool dash = false;
    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidController.SetActive(true);
        }
    }
    void Update()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            HorizontalMove = Input.GetAxisRaw("Horizontal") * speed;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                jump = true;
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                dash = true;
            }
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            HorizontalMove = joystick.Horizontal * speed;
        }
        animator.SetFloat("Speed", Mathf.Abs(HorizontalMove));
    }
    public void OnJumpButtonDown()
    {
        jump = true;
    }
    public void OnDashButtonDown()
    {
        dash = true;
    }
    public void OnFallilng()
    {
        animator.SetBool("IsJumping", true);
    }
    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }
    void FixedUpdate()
    {
        controller.Move(HorizontalMove * Time.fixedDeltaTime, jump, dash);
        jump = false;
        dash = false;
    }

}
