using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    float rotX;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //get input
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;

        float joystickX = Input.GetAxis("Joystick X") * sens * Time.deltaTime;
        float joystickY = Input.GetAxis("Joystick Y") * sens * Time.deltaTime;


        if (invertY)
        {
            if (mouseY != 0)
                rotX += mouseY;
            else
                rotX += joystickY;
        }
        else
        {
            if(mouseY  != 0)
                rotX -= mouseY;
            else
                rotX += joystickY;
        }

        //clamp the camera on the x-axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        //rotate camera on x-axis to look up and down
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        //rotate the player on the y-axis to look left and right
        if (mouseX != 0)
        {
            transform.parent.Rotate(Vector3.up * mouseX);
        }
        else
        {
            transform.parent.Rotate(Vector3.up * joystickX);
        }

    }
}
