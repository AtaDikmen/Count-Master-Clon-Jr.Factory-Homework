using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ModelControlScript : MonoBehaviour
{
    [SerializeField] private ParticleSystem popping;
    private PlayerScript PlayerSc;
    private FinishScript FinishSc;
    private AudioSource AC;

    private void Start()
    {
        PlayerSc = GetComponentInParent<PlayerScript>();
        FinishSc = GameObject.Find("_Scripts").GetComponent<FinishScript>();
        AC = GetComponentInParent<AudioSource>();
    }
    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Red") && c.transform.parent.childCount > 0)
        {
            Destroy(c.gameObject);
            Destroy(gameObject);

            Instantiate(popping, transform.position, Quaternion.identity);
            AC.Play();
        }

        switch (c.tag)
        {
            case "Red":
                if (c.transform.parent.childCount > 0)
                {
                    Destroy(c.gameObject);
                    Destroy(gameObject);
                }
                break;
            case "Ramp":

                transform.DOJump(transform.position, 4f, 1, 2f).SetEase(Ease.Flash);
                StartCoroutine(ResetCircle());

                break;
        }

        if (c.CompareTag("Stair"))
        {
            transform.parent.parent = null;
            transform.parent = null;
            GetComponent<Rigidbody>().isKinematic = GetComponent<Collider>().isTrigger = false;

            if (!PlayerScript.PlayerScInstance.cameraMove)
            {
                PlayerScript.PlayerScInstance.cameraMove = true;
            }

            if (PlayerScript.PlayerScInstance.player.transform.childCount == 1)
            {
                c.GetComponent<Renderer>().material.DOColor(new Color(0.4f, 0.98f, 0.65f), 0.5f).SetLoops(1000, LoopType.Yoyo).
                    SetEase(Ease.Flash);

                StartCoroutine(CameraStop());
                StartCoroutine(FinishSc.EndGame());
            }
        }

        if (c.CompareTag("Cylinder"))
        {
            AC.Play();
            Destroy(gameObject);
            Instantiate(popping, transform.position, Quaternion.identity);
            PlayerSc.numberOfModels = PlayerSc.transform.childCount - 1;
            PlayerSc.CounterText.text = PlayerSc.numberOfModels.ToString();
        }
    }

    IEnumerator ResetCircle()
    {
        yield return new WaitForSecondsRealtime(2.5f);

        PlayerScript.PlayerScInstance.MakeCircle();
    }

    IEnumerator CameraStop()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        PlayerScript.PlayerScInstance.gameState = false;
    }
}
