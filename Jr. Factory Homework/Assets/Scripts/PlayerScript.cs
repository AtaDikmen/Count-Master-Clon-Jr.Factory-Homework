using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Transform player;
    public int numberOfModels,numberOfRedModels;
    [SerializeField] public TextMeshPro CounterText;
    [SerializeField] private GameObject model;

    [Range(0f, 1f)] [SerializeField] private float distance, Radius;

    // Playe Move

    public bool moveByTouch, gameState;
    private Vector3 mouseStartPos, playerStartPos;
    public float playerSpeed,roadSpeed;
    private Camera cameraP;

    [SerializeField] private Transform road,cylinder;

    [SerializeField] private Transform redCircle;
    private bool attack;
    private bool finish = false;

    public static PlayerScript PlayerScInstance;
    public GameObject SecondCam;
    public bool cameraMove = false;

    void Start()
    {
        player = transform;

        numberOfModels = transform.childCount - 1;

        CounterText.text = numberOfModels.ToString();

        cameraP = Camera.main;

        PlayerScInstance = this;
    }

    void Update()
    {
        if (attack)
        {
            var circleDirection = new Vector3(redCircle.position.x,transform.position.y,redCircle.position.z) - transform.position;

            for (int i = 1; i < transform.childCount; i++)
            {
                transform.GetChild(i).rotation = Quaternion.Slerp(transform.GetChild(i).rotation, 
                    Quaternion.LookRotation(circleDirection, Vector3.up), Time.deltaTime * 3f);
            }

            if (redCircle.GetChild(1).childCount > 1)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    var distance = redCircle.GetChild(1).GetChild(0).position - transform.GetChild(i).position;

                    if (distance.magnitude < 10f)
                    {
                        transform.GetChild(i).position = Vector3.Lerp(transform.GetChild(i).position,
                            new Vector3(redCircle.GetChild(1).GetChild(0).position.x,transform.GetChild(i).position.y,
                            redCircle.GetChild(1).GetChild(0).position.z), Time.deltaTime * 1f);
                    }
                }
            }

            else
            {
                attack = false;
                roadSpeed = 8f;

                MakeCircle();

                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).rotation = Quaternion.LookRotation(transform.forward);
                }

                redCircle.gameObject.SetActive(false);

            }

            if (transform.childCount == 1)
            {
                redCircle.GetChild(1).GetComponent<RedModelScript>().StopAttack();
                gameObject.SetActive(false);
            }

        }
        else
        {
            MovePlayer();
        }

        if (gameState)
        {
            road.Translate(road.forward * roadSpeed * Time.deltaTime);
            cylinder.position += -transform.forward * roadSpeed * Time.deltaTime;
        }

        if (cameraMove && transform.childCount > 1)
        {
            var cinemachineTransposer = SecondCam.GetComponent<CinemachineVirtualCamera>()
              .GetCinemachineComponent<CinemachineTransposer>();

            var cinemachineComposer = SecondCam.GetComponent<CinemachineVirtualCamera>()
                .GetCinemachineComponent<CinemachineComposer>();

            cinemachineTransposer.m_FollowOffset = new Vector3(5f, Mathf.Lerp(cinemachineTransposer.m_FollowOffset.y,
                transform.GetChild(1).position.y + 5f, Time.deltaTime * 1f), 10f);

            cinemachineComposer.m_TrackedObjectOffset = new Vector3(5f, Mathf.Lerp(cinemachineComposer.m_TrackedObjectOffset.y,
                15f, Time.deltaTime * 1f), 10f);


        }
    }

    private void MovePlayer()
    {
        if (Input.GetMouseButtonDown(0) && gameState && !finish)
        {
            moveByTouch = true;

            var plane = new Plane(Vector3.up, 0f);
            var ray = cameraP.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out var distance))
            {
                mouseStartPos = ray.GetPoint(distance + 1f);
                playerStartPos = transform.position;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            moveByTouch = false;
        }

        if (moveByTouch && !finish)
        {
            var plane = new Plane(Vector3.up, 0f);
            var ray = cameraP.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray,out var distance))
            {
                var mousePos = ray.GetPoint(distance + 1f);
                var move = mousePos - mouseStartPos;
                var control = playerStartPos + move;

                if (numberOfModels > 30)
                {
                    control.x = Mathf.Clamp(control.x, -3f, 3f);
                }
                else
                {
                    control.x = Mathf.Clamp(control.x, -6f, 6f);
                }

                transform.position = new Vector3(Mathf.Lerp(transform.position.x, control.x, 
                    Time.deltaTime * playerSpeed), transform.position.y, transform.position.z);
            }
        }

        
    }


    private void CreateModel(int num)
    {
        for (int i = numberOfModels + 1; i < num; i++)
        {
            Instantiate(model, transform.position, Quaternion.LookRotation(transform.forward), transform);
        }

        numberOfModels = transform.childCount - 1;

        CounterText.text = numberOfModels.ToString();

        MakeCircle();
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Gate"))
        {
            c.transform.parent.GetChild(0).GetComponent<BoxCollider>().enabled = false;
            c.transform.parent.GetChild(1).GetComponent<BoxCollider>().enabled = false;

            var gateScript = c.GetComponent<GateScript>();

            if (gateScript.multiply)
            {
                CreateModel(numberOfModels * gateScript.randomNum);
            }
            else
            {
                CreateModel(numberOfModels + gateScript.randomNum);
            }
        }

        if (c.CompareTag("RedCircle"))
        {
            redCircle = c.transform;
            attack = true;

            roadSpeed = 1f;

            c.transform.GetChild(1).GetComponent<RedModelScript>().Attack(transform);

            StartCoroutine(UpdateModelNumbers());
        }

        if (c.CompareTag("Finish"))
        {
            SecondCam.SetActive(true);
            //FinishLine = true;
            TowerScript.TowerInstance.CreateTower(transform.childCount - 1);
            transform.GetChild(0).gameObject.SetActive(false);
            finish = true;
        }
    }

    public void MakeCircle()
    {
        for (int i = 1; i < player.childCount; i++)
        {
            var x = distance * Mathf.Sqrt(i) * Mathf.Cos(i * Radius);
            var z = distance * Mathf.Sqrt(i) * Mathf.Sin(i * Radius);

            var position = new Vector3(x, -0.656f, z);

            player.transform.GetChild(i).DOLocalMove(position, 0.5f).SetEase(Ease.OutBack);
        }
    }

    public IEnumerator UpdateModelNumbers()
    {
        numberOfRedModels = redCircle.transform.GetChild(1).childCount - 1;
        numberOfModels = transform.childCount - 1;

        while (numberOfRedModels > 0 && numberOfModels > 0)
        {
            numberOfRedModels--;
            numberOfModels--;

            redCircle.transform.GetChild(1).GetComponent<RedModelScript>().CounterText.text = numberOfRedModels.ToString();
            CounterText.text = numberOfModels.ToString();

            yield return null;
        }

        if (numberOfRedModels == 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).rotation = Quaternion.identity;
            }
        }
    }
}
