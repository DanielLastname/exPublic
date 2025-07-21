using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Transform camTran;
    public bool LockRotation = true;
    
    public float speed = 5f;
    public float jump = 2f;
    private float gravity = -9.8f;

    private CharacterController controller;
    private Animator[] animator;
    private Vector3 moveInput;
    private Vector3 velocity;

    private PlayerControls controls;

    public GameObject CurrentAttack;

    public Stats playerStats;
    public IInteractable CurrentInteractable;

    public bool InteruptPlayerController = false;

    private void OnEnable()
    {
        // Initialize the controls
        controls = new PlayerControls();

        // Subscribe to the Weapon action performed event
        controls.GamePlay.Weapon.performed += ctx => WeaponAction();
        controls.GamePlay.Interact.performed += ctx => InteractAction();

        // Enable the controls
        controls.Enable();
    }

    private void OnDisable()
    {
        // Disable the controls to avoid memory leaks
        controls.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentsInChildren<Animator>();
        var cam = FindFirstObjectByType<Camera>();
        camTran = cam.transform;
        playerStats = GetComponent<Stats>();

    }

    // Update is called once per frame
    void Update()
    {
        if (InteruptPlayerController == true) return;

        Vector3 forward = camTran.forward;
        Vector3 right = camTran.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
        controller.Move(moveDirection * speed * Time.deltaTime);

        if (LockRotation && moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion toRotaion = Quaternion.LookRotation( moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotaion, 10f * Time.deltaTime);
            for (int i = 0; i < animator.Length; i ++)
            {
                animator[i].SetBool("walk", true);
            }
            
        }
        else
        {
            for (int i = 0; i < animator.Length; i++)
            {
                animator[i].SetBool("walk", false);
                animator[i].SetFloat("speed", moveDirection.sqrMagnitude);
            }
               
        }
        /*
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(move * Time.deltaTime * speed);
        */
        velocity.y += Time.deltaTime * gravity;
        controller.Move(velocity * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        //Debug.Log("aghhhhh");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (controller.isGrounded && context.performed)
        {
            //Debug.Log("Das yumpin zee");
            velocity.y = Mathf.Sqrt(jump * -2f * gravity);
            for (int i = 0; i < animator.Length; i++)
            {
                animator[i].SetTrigger("jump");
            }
                
        }
        else 
        {
            //animator.SetBool("jump", false);
        }
    }
    /*
    public void OnWeapon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("attack");
            animator.SetTrigger("attack");
            
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("interact");
        if (context.performed)
        {
            Debug.Log("interacack");
            animator.SetTrigger("attack");

        }
    }
    */
    public void WeaponAction()
    {
        //Debug.Log("Weapon action triggered!");
        for (int i = 0; i < animator.Length; i++)
        {
            animator[i].SetTrigger("attack");
        }
            
        StartCoroutine(ActivateDeactivateAttack());
    }

    IEnumerator ActivateDeactivateAttack()
    {
        yield return new WaitForSeconds(0.3f);
        CurrentAttack.SetActive(true);
        DamageTrigger t = CurrentAttack.GetComponent<DamageTrigger>();
        t.playerStats = playerStats;
        if (playerStats == null) Debug.LogError("PLAYER STATS NOT FOUND");

        yield return new WaitForSeconds(0.3f);
        CurrentAttack.SetActive(false);
    }

    public void InteractAction ()
    {
        //Debug.Log("interact action triggered!");

        if (CurrentInteractable != null)
        {
            CurrentInteractable.Interact();
        }
        else
        {
            Debug.Log("Null interactable");
        }
    }

    public void RefreshAnimators()
    {
        animator = GetComponentsInChildren<Animator>();
    }

}
