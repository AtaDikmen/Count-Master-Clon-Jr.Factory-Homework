using TMPro;
using UnityEngine;

public class RedModelScript : MonoBehaviour
{
    public TextMeshPro CounterText;
    [SerializeField] private GameObject model;
    [Range(0f, 1f)] [SerializeField] private float distance, Radius;

    public Transform redCircle;
    public bool attack;

    private TryAgainScript tryAgainSc;

    void Start()
    {
        for (int i = 0; i < Random.Range(20,100); i++)
        {
            Instantiate(model, transform.position, Quaternion.identity, transform);
        }

        CounterText.text = (transform.childCount - 1).ToString();

        MakeCircle();

        tryAgainSc = GameObject.Find("_Scripts").GetComponent<TryAgainScript>();
    }

    private void MakeCircle()
    {
        Debug.Log("Fonskiyon Çalýþtý");

        for (int i = 0; i < transform.childCount; i++)
        {
            var x = distance * Mathf.Sqrt(i) * Mathf.Cos(i * Radius);
            var z = distance * Mathf.Sqrt(i) * Mathf.Sin(i * Radius);

            var position = new Vector3(x, -0.656f, z);

            transform.transform.GetChild(i).localPosition = position;
        }
    }


    void Update()
    {
        if (attack && transform.childCount > 1)
        {
            var circlePos = new Vector3(redCircle.position.x, transform.position.y, redCircle.position.z);
            var circleDirection = redCircle.position - transform.position;

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).rotation = Quaternion.Slerp(transform.GetChild(i).rotation,
                    Quaternion.LookRotation(circleDirection, Vector3.up), Time.deltaTime * 3f);

                if (redCircle.childCount > 1)
                {
                    var distance = redCircle.GetChild(i).position - transform.GetChild(i).position;

                    if (distance.magnitude < 7f)
                    {
                        transform.GetChild(i).position = Vector3.Lerp(transform.GetChild(i).position,
                            redCircle.GetChild(i).position, Time.deltaTime * 2f);
                    }
                }  
            }
        }
    }

    public void Attack(Transform circleForce)
    {
        redCircle = circleForce;
        attack = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Animator>().SetBool("Run", true);
        }
    }

    public void StopAttack()
    {
        PlayerScript.PlayerScInstance.gameState = attack = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Animator>().SetBool("Run", false);
        }

        MakeCircle();

        tryAgainSc.Canvas();
    }
}
