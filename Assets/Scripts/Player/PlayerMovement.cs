using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private OnScreenJoystick moveJoystick;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool fourDirectionalMove = false;

    private CharacterController controller; // yoksa Transform ile de hareket ettirilebilir
    private Vector3 currentMoveDir;

    [SerializeField] private Animator animator;
    private PlayerShooting playerShooting;

    private void Awake()
    {
        controller = GetComponent<CharacterController>(); // opsiyonel, null olabilir
        // animator = GetComponent<Animator>();
        playerShooting = GetComponent<PlayerShooting>();
    }

    private void Update()
    {
        Vector2 joystickInput = moveJoystick.Direction;
        Vector2 keyboardInput = moveAction.action.ReadValue<Vector2>();

        Vector2 input = joystickInput.sqrMagnitude > 0.0001f ? joystickInput : keyboardInput;


        if (fourDirectionalMove)
            input = SnapToFourDirections(input);

        currentMoveDir = new Vector3(input.x, 0f, input.y);

        if (currentMoveDir.sqrMagnitude > 0.0001f)
        {
            Vector3 motion = currentMoveDir * moveSpeed * Time.deltaTime;

            if (controller != null)
                controller.Move(motion);
            else
                transform.position += motion;

            Vector3 aimDir = playerShooting.AimDirection;

            if (aimDir.sqrMagnitude > 0.0001f)
            {
                float dot = Vector3.Dot(currentMoveDir.normalized, aimDir.normalized);
                animator.SetFloat("speedMultiplier", dot < 0f ? -1f : 1f);
            }
            else
            {
                animator.SetFloat("speedMultiplier", 1f);
            }

                animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    public bool IsMoving => currentMoveDir.sqrMagnitude > 0.0001f;
    public Vector3 MoveDirection => currentMoveDir;

    private Vector2 SnapToFourDirections(Vector2 dir)
    {
        if (dir.magnitude < 0.15f) return Vector2.zero;
        return Mathf.Abs(dir.x) > Mathf.Abs(dir.y)
            ? new Vector2(Mathf.Sign(dir.x), 0f)
            : new Vector2(0f, Mathf.Sign(dir.y));
    }
}