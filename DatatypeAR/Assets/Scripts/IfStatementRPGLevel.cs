using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class IfStatementRPGLevel : MonoBehaviour
{

    [SerializeField]
    private GameObject slimePrefab;
    [SerializeField]
    private GameObject batPrefab;
    [SerializeField]
    private GameObject spiderPrefab;
    private List<GameObject> stage1Monsters = new List<GameObject>();
    [SerializeField]
    private GameObject fireballPrefab;
    [SerializeField]
    private GameObject lightningPrefab;

    private GameObject emptyGameObject;
    private GameObject placedMonster;
    private GameObject placedLightning;

    private ARRaycastManager mRaycastManager;
    private Vector2 touchPosition;
    private RaycastHit hitObject;
    private static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private ARSessionOrigin mSessionOrigin;

    private System.Random random = new System.Random();

    [SerializeField]
    private GameObject levelCanvas;
    [SerializeField]
    private GameObject codeCanvas;
    [SerializeField]
    private TMP_Text code;

    private int health = 3;
    [SerializeField]
    private GameObject heart1;
    [SerializeField]
    private GameObject heart2;
    [SerializeField]
    private GameObject heart3;
    

    private int stage = 1;

    private string correctAttack;

    private void Awake()
    {
        mRaycastManager = GetComponent<ARRaycastManager>();
        mSessionOrigin = GetComponent<ARSessionOrigin>();

        stage1Monsters.Add(slimePrefab);
        stage1Monsters.Add(batPrefab);
        stage1Monsters.Add(spiderPrefab);
    }

    private void Update()
    {
        if (emptyGameObject == null)
        {
            Touch touch;
            if (Input.touchCount < 1)
            {
                return;
            }

            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (emptyGameObject == null)
                {
                    if (mRaycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds))
                    {
                        CreateAnchor(hits[0]);
                        levelCanvas.SetActive(true);
                        codeCanvas.SetActive(true);
                    }
                }
            }
        }
    }

    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        ARAnchor anchor;

        emptyGameObject = Instantiate(new GameObject("EmptyGameObject"));


        mSessionOrigin.MakeContentAppearAt(emptyGameObject.transform, hit.pose.position, hit.pose.rotation);

        placedLightning = Instantiate(lightningPrefab);
        placedLightning.SetActive(false);

        anchor = emptyGameObject.GetComponent<ARAnchor>();
        if (anchor == null)
        {
            anchor = emptyGameObject.AddComponent<ARAnchor>();
        }

        SpawnMonster();

        return anchor;
    }

    private void SpawnMonster()
    {
        if (stage == 1)
        {
            Destroy(placedMonster);
            int index = random.Next(stage1Monsters.Count);
            placedMonster = Instantiate(stage1Monsters[index]);
            placedMonster.transform.position = emptyGameObject.transform.position;
            correctAttack = placedMonster.GetComponent<MonsterInfo>().correctAttack;
            code.text = placedMonster.GetComponent<MonsterInfo>().ifStatementCode;
            stage1Monsters.RemoveAt(index);
            if (stage1Monsters.Count == 0)
            {
                stage++;
            }
        }
    }

    public void Attack(string attack)
    {
        StartCoroutine(AttackAnimation(attack));
        if (Equals(attack, correctAttack))
        {
            Debug.LogWarning("Correct attack");
            StartCoroutine(KillMonster());

        } else
        {
            Debug.LogWarning("Incorrect attack");
            placedMonster.GetComponent<MonsterInfo>().Attack();
            health--;
        }
    }

    private void UpdateHealth()
    {
        if (health == 3)
        {
            heart1.SetActive(true);
            heart2.SetActive(true);
            heart3.SetActive(true);
        }
        else if (health == 2)
        {
            heart1.SetActive(true);
            heart2.SetActive(true);
            heart3.SetActive(false);
        }
        else if (health == 1)
        {
            heart1.SetActive(true);
            heart2.SetActive(false);
            heart3.SetActive(false);
        }
    }

    private IEnumerator KillMonster()
    {
        placedMonster.GetComponent<MonsterInfo>().Die();
        yield return new WaitForSeconds(1);
        SpawnMonster();
    }

    private IEnumerator AttackAnimation(string attack)
    {
        Camera firstPersonCamera = GameObject.Find("AR Camera").GetComponent<Camera>();

        switch(attack)
        {
            case "fireball":
                GameObject fireball = Instantiate(fireballPrefab);
                fireball.transform.position = firstPersonCamera.transform.position;
                Vector3 direction = placedMonster.transform.position - firstPersonCamera.transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction);
                fireball.transform.rotation = rotation;
                yield return new WaitForSeconds(1);
                Destroy(fireball);
                UpdateHealth();
                break;
            case "lightningbolt":
                placedLightning.GetComponent<DigitalRuby.LightningBolt.LightningBoltScript>().StartPosition = firstPersonCamera.transform.position;
                placedLightning.GetComponent<DigitalRuby.LightningBolt.LightningBoltScript>().EndObject = placedMonster;
                placedLightning.SetActive(true);
                placedLightning.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                placedLightning.SetActive(false);
                UpdateHealth();
                break;
        }
    }

}
