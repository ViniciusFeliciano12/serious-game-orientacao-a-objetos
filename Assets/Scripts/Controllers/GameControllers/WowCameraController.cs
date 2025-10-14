using Assets.Scripts.Interaction.Interactives;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class WowCameraController : MonoBehaviour
{
    public static WowCameraController Instance { get; private set; }

    public Transform target;
    public Transform playerTarget;
    public Transform minotaurTarget;

    public float targetHeight = 1.7f;
    public float distance = 5.0f;
    public float offsetFromWall = 0.1f;

    public float maxDistance = 20;
    public float minDistance = .6f;
    public float speedDistance = 5;

    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;

    public int yMinLimit = -40;
    public int yMaxLimit = 80;

    public int zoomRate = 40;
    public float zoomDampening = 5.0f;

    public LayerMask collisionLayers = -1;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private float correctedDistance;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        xDeg = angles.y;
        yDeg = angles.x;

        currentDistance = distance;
        desiredDistance = distance;
        correctedDistance = distance;

        if (this.gameObject.GetComponent<Rigidbody>())
            this.gameObject.GetComponent<Rigidbody>().freezeRotation = true;
    }

    void LateUpdate()
    {
        Cursor.lockState = PauseController.Instance.TimeStopped || DialogueManagement.Instance.HasActiveDialogue() ? CursorLockMode.None : CursorLockMode.Locked;

        if (PauseController.Instance.TimeStopped || DialogueManagement.Instance.HasActiveDialogue() || !target)
            return;

        if (GUIUtility.hotControl == 0)
        {
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
        }

        yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance) * speedDistance;
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);

        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

        Quaternion rotation = Quaternion.Euler(yDeg, xDeg, 0);

        Vector3 targetPositionWithOffset = target.position + new Vector3(0, targetHeight, 0);
        Vector3 desiredCameraPos = targetPositionWithOffset - (rotation * Vector3.forward * currentDistance);

        RaycastHit collisionHit;

        // **AQUI ESTÁ A CORREÇÃO**
        // Declaramos a variável 'position' antes do 'if' para que ela exista no escopo correto.
        Vector3 position;

        if (Physics.Linecast(targetPositionWithOffset, desiredCameraPos, out collisionHit, collisionLayers.value))
        {
            correctedDistance = Vector3.Distance(targetPositionWithOffset, collisionHit.point) - offsetFromWall;
            position = targetPositionWithOffset - (rotation * Vector3.forward * correctedDistance);
        }
        else
        {
            position = desiredCameraPos;
        }

        transform.rotation = rotation;
        transform.position = position;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public async void FocusOnMinotaur()
    {
        target = minotaurTarget;
        await Task.Delay(2500);
        target = playerTarget;
    }
}