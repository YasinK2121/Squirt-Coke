using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlaySecond : MonoBehaviour
{
    public Animator anim;
    private Vector3 startPos;
    public Transform particleSpawn;
    public GameObject particleFly;
    public GameObject part;
    public Rigidbody playerRig;

    public Image cursor;
    public Image indicator;

    static string[] ANIMNAME = new string[] { "Empty", "UpBottle", "DownBottle", "ShakeBottle" };
    public int timer;
    public float mouseSpeed, flySpeed, lastPosZ;
    public bool readyFly, startFly;

    void Start()
    {
        startPos = transform.position;
        playerRig.isKinematic = true;
    }

    void Update()
    {
        cursor.rectTransform.rotation = Quaternion.Slerp(cursor.rectTransform.rotation, Quaternion.Euler(0, 0, (indicator.fillAmount * 180) * -1), 150);
        if (!readyFly)
        {
            FirstGamePlay();
        }
        else
        {
            timer++;
            if (timer <= 100)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 180), Time.deltaTime * 5);
                SetAnim("Empty");
                startFly = true;
            }
        }
        if (!startFly)
        {
            if (startPos.y > transform.position.y && indicator.fillAmount <= 0.1)
            {
                transform.rotation = new Quaternion(0, 0, 0, 0);
                SetAnim("Empty");
                transform.position = new Vector3(transform.position.x, startPos.y, transform.position.z);
            }
        }

        if (startFly)
        {
            SecondGamePlay();
        }
    }

    void FirstGamePlay()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetAnim("UpBottle");
        }

        if (Input.GetMouseButton(0))
        {
            mouseSpeed = Input.GetAxis("Mouse X") / Time.deltaTime;
            if (mouseSpeed <= 0)
            {
                mouseSpeed *= -1;
            }
            if (mouseSpeed > 8)
            {
                SetAnim("ShakeBottle");
                indicator.fillAmount += Time.deltaTime * (mouseSpeed / 500);
            }
            if (mouseSpeed == 0)
            {
                SetAnim("UpBottle");
                indicator.fillAmount -= Time.deltaTime / 10;
                transform.rotation = new Quaternion(0, 0, 0, 0);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            //mouseSpeed = 0;
            transform.rotation = new Quaternion(0, 0, 0, 0);
            if (indicator.fillAmount <= 0.1)
            {
                SetAnim("DownBottle");
                indicator.fillAmount -= Time.deltaTime / 10;
                if (startPos.y > transform.position.y)
                {
                    transform.position = new Vector3(transform.position.x, startPos.y, transform.position.z);
                }
            }
            else
            {
                transform.position = new Vector3(transform.position.x, startPos.y + 2.5f, transform.position.z);
                readyFly = true;
            }
        }
    }

    private Vector3 screenPosition;
    private Vector3 worldPosition;
    private Vector3 worldPosition2;
    public Vector3 move;
    void SecondGamePlay()
    {
        screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1.1f);
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        if (Input.GetMouseButtonDown(0))
        {
            lastPosZ = transform.eulerAngles.z;
            playerRig.isKinematic = true;
            worldPosition2 = Camera.main.ScreenToWorldPoint(screenPosition);
            part = Instantiate(particleFly, particleSpawn.position, particleSpawn.rotation);
            part.transform.parent = particleSpawn;
        }

        if (Input.GetMouseButton(0))
        {
            move = worldPosition - worldPosition2;
            transform.position += transform.up * Time.deltaTime * flySpeed * -1;
            indicator.fillAmount -= Time.deltaTime / 10;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, (move.y * 120) + lastPosZ), 150);
        }

        if (Input.GetMouseButtonUp(0))
        {
            startFly = false;
            playerRig.isKinematic = false;
            Destroy(part);
        }

        if (indicator.fillAmount == 0)
        {
            startFly = false;
            playerRig.isKinematic = false;
            Destroy(part);
        }
    }

    void SetAnim(string whichAnim)
    {
        for (int i = 0; i < ANIMNAME.Length; i++)
        {
            if (whichAnim == ANIMNAME[i])
            {
                anim.SetBool(ANIMNAME[i], true);
                if (ANIMNAME[i] == "ShakeBottle")
                {
                    anim.speed = 10;
                }
                else
                {
                    anim.speed = 3;
                }
            }
            else
            {
                anim.SetBool(ANIMNAME[i], false);
            }
        }
    }
}
