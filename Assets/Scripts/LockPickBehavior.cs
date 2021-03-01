using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LockPickBehavior : MonoBehaviour
{
    public Camera camera;
    public Transform innerLock;
    public Transform follow;

    public float maxAngle = 90.0f; // how far you're able to turn your pick;
    public float lockSpeed = 10.0f; // how fast the lock can turn

    private float eulerAngle; // keep track of the current angle it locks at
    private float unlockingAngle; // what angle the lock unlocks at
    private Vector2 unlockingRange; // the lock range
    private float keyPressTime = 0.0f;
    private bool movePick = true; // if the user can move the pick

    [Min(1)] [Range(1, 25)] // difficulty of the lock;
    public float lockRange = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        LockPicking();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = follow.position; // makes the pick always in the right position

        if(movePick)
        {
            Vector3 direction = Input.mousePosition - camera.WorldToScreenPoint(transform.position); // creates direction from the mouse to the current position
            eulerAngle = Vector3.Angle(direction, Vector3.up); // the axis it's going to rotate around
            Vector3 cross = Vector3.Cross(Vector3.up, direction); // gives the cross product of the vector and direction

            if(cross.z < 0) // cross.z because using vector.up
            {
                eulerAngle = -eulerAngle; // can get both a negagive and positive range, starting at 0 as the up direction.
            }

            eulerAngle = Mathf.Clamp(eulerAngle, -maxAngle, maxAngle); // clamp so it doesn't get out of bounds
            Quaternion rotateTo = Quaternion.AngleAxis(eulerAngle, Vector3.forward); // rotate around the x axis
            transform.rotation = rotateTo;
        }

        if(Input.GetKeyDown(KeyCode.D)) // input to rotate the lock
        {
            movePick = false;
            keyPressTime = 1;
        }
        if(Input.GetKeyUp(KeyCode.D))
        {
            movePick = true;
            keyPressTime = 0;
        }

        keyPressTime = Mathf.Clamp(keyPressTime, 0, 1);

        float percentage = Mathf.Round(100 - Mathf.Abs(((eulerAngle - unlockingAngle) / 100) * 100));
        float lockRotation  = ((percentage / 100) * maxAngle) * keyPressTime;
        float maxRotation = (percentage / 100) * maxAngle;

        float lockLerp = Mathf.Lerp(innerLock.eulerAngles.z, lockRotation, Time.deltaTime * lockSpeed); // smooths out the turning of the lock
        innerLock.eulerAngles = new Vector3(0, 0, lockLerp);

        if(lockLerp >= maxRotation - 1) // small range, so unlocking doesn't have to be exact
        {
            if(eulerAngle < unlockingRange.y && eulerAngle > unlockingRange.x)
            {
                Debug.Log("This lock has been unlocked!!");
                LockPicking();
                movePick = true;
                keyPressTime = 0;
                SceneManager.LoadScene("Win");
            }
            else // the else is to have the pick to shake, indicating the lock is not open yet.
            {
                float randomRotation = Random.insideUnitCircle.x; // random value inside of a circle
                transform.eulerAngles += new Vector3(0, 0, Random.Range(-randomRotation, randomRotation));
            }
        }
    }

    void LockPicking()
    {
        unlockingAngle = Random.Range(-maxAngle + lockRange, maxAngle - lockRange); // the unlocking angle isn't larger than the unlock range
        unlockingRange = new Vector2(unlockingAngle - lockRange, unlockingAngle + lockRange); // gives a bit of space to move the lock around, the larger the unlockrange makes it easier
    }
}
