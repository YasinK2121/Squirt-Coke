using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Animator anim;
    private Vector3 startPos;
    public Vector3[] positions = new Vector3[20];
    public Transform point1, point2, point3, particleSpawn;
    public GameObject particleFly;
    public GameObject part;
    public LineRenderer lineRenderer;
    public Rigidbody playerRig;

    public Image cursor;
    public Image indicator;

    static string[] ANIMNAME = new string[] { "Empty", "UpBottle", "DownBottle", "ShakeBottle" };
    public int numPoints, whichLinePos, timer;
    public float mouseSpeed, flySpeed;
    public bool readyFly, curveReady, startFly;

    void Start()
    {
        lineRenderer.positionCount = numPoints;
        lineRenderer.gameObject.SetActive(false);
        startPos = transform.position;
        playerRig.isKinematic = true;
    }

    void Update()
    {
        cursor.rectTransform.rotation = Quaternion.Slerp(cursor.rectTransform.rotation, Quaternion.Euler(0, 0, (indicator.fillAmount * 180) * -1), 150);
        DrawQuadraticCurve();
        if (!readyFly)
        {
            FirstGamePlay();
        }
        else
        {
            SetAnim("Empty");
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 180), Time.deltaTime * 5);
            timer++;
            if (timer == 10)
            {
                curveReady = true;
                lineRenderer.gameObject.SetActive(true);
            }
        }

        if (curveReady)
        {
            SecondGamePlay();
        }

        if (startPos.y > transform.position.y && indicator.fillAmount <= 0.1)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
            SetAnim("Empty");
            transform.position = new Vector3(transform.position.x, startPos.y, transform.position.z);
        }
        if (startFly)
        {
            Fly();
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
        point1.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1.1f);
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        if (Input.GetMouseButtonDown(0))
        {
            worldPosition2 = Camera.main.ScreenToWorldPoint(screenPosition);
        }

        if (Input.GetMouseButton(0))
        {
            move = worldPosition - worldPosition2;
        }

        if (Input.GetMouseButtonUp(0))
        {
            part = Instantiate(particleFly, particleSpawn.position, particleSpawn.rotation);
            part.transform.parent = particleSpawn;
            startFly = true;
            curveReady = false;
        }

        point2.transform.position = new Vector3(move.x * 25, move.y * 100, point2.transform.position.z);
        point3.transform.position = new Vector3(move.x * 50, move.y * 50, point3.transform.position.z);
    }

    void Fly()
    {
        //flySpeed -= Time.deltaTime
        lineRenderer.gameObject.SetActive(false);
        indicator.fillAmount -= Time.deltaTime / 2;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(positions[whichLinePos].x, positions[whichLinePos].y, transform.position.z), flySpeed);
        if (positions[whichLinePos].y == transform.position.y && positions[whichLinePos].x == transform.position.x)
        {
            whichLinePos++;
        }
        if (indicator.fillAmount == 0)
        {
            startFly = false;
            playerRig.isKinematic = false;
            Destroy(part);
        }
    }

    private void DrawQuadraticCurve()
    {
        for (int i = 1; i < numPoints + 1; i++)
        {
            float t = i / (float)numPoints;
            positions[i - 1] = CalculateQuadraticBezierPoint(t, point1.position, point2.position, point3.position);
        }
        lineRenderer.SetPositions(positions);
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
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
