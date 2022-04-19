using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

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
    private GameObject skeletonPrefab;
    [SerializeField]
    private GameObject orcPrefab;
    [SerializeField]
    private GameObject orcOrangePrefab;
    [SerializeField]
    private GameObject dragonPrefab;
    [SerializeField]
    private GameObject dragonGreenPrefab;
    private List<GameObject> stage2Monsters = new List<GameObject>();
    [SerializeField]
    private GameObject fireballPrefab;
    [SerializeField]
    private GameObject lightningPrefab;
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private GameObject dungeonPrefab;

    private GameObject emptyGameObject;
    private GameObject placedMonster;

    private string stage1Code;
    private string stage2Code;

    private ARRaycastManager raycastManager;
    private static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private ARSessionOrigin mSessionOrigin;

    private System.Random random = new System.Random();

    [SerializeField]
    private GameObject levelCanvas;
    [SerializeField]
    private GameObject codeCanvas;
    [SerializeField]
    private TMP_Text code;
    [SerializeField]
    GameObject menuHandlerObject;
    [SerializeField]
    private GameObject objectivePanel;
    [SerializeField]
    private TMP_Text objectiveText;
    [SerializeField]
    private GameObject prelevelCanvas;
    [SerializeField]
    private GameObject pauseMenu;

    private int health = 3;
    [SerializeField]
    private GameObject heart1;
    [SerializeField]
    private GameObject heart2;
    [SerializeField]
    private GameObject heart3;
    [SerializeField]
    private GameObject heart4;
    [SerializeField]
    private GameObject emptyHeart4;

    private bool isAttackingAllowed = true;
    
    private int stage = 1;

    private string correctAttack;

    private Camera firstPersonCamera;

    private MonsterInfo monsterInfo;

    private void Awake()
    {
        firstPersonCamera = GameObject.Find("AR Camera").GetComponent<Camera>();
        raycastManager = GetComponent<ARRaycastManager>();
        mSessionOrigin = GetComponent<ARSessionOrigin>();

        stage1Monsters.Add(slimePrefab);
        stage1Monsters.Add(batPrefab);
        stage1Monsters.Add(spiderPrefab);

        stage2Monsters.Add(orcPrefab);
        stage2Monsters.Add(orcOrangePrefab);
        stage2Monsters.Add(dragonPrefab);
        stage2Monsters.Add(dragonGreenPrefab);
        stage2Monsters.Add(skeletonPrefab);

        stage1Code = "if not(isFlying):\n  if name == 'Slime':\n    Use Fireball\n  else:\n    Use Fireball\nelse:\n  Use LightningBolt";
        stage2Code = "if colour == 'Red':\n  Use LightningBolt\nelif colour == 'Green':\n  if not(hasWeapon):\n    if isFlying:\n      Use explode\n    else:\n      Use LightningBolt\nelse:\n    Use FireBall\nelif name == 'Skeleton' or name == 'Dragon':\n  Use explode\nelse:\n  Use LightningBolt";
    }

    private void Update()
    {
        if (prelevelCanvas.activeSelf || pauseMenu.activeSelf)
        {
            return;
        }

        RaycastHit hit;
        Ray targetRay = new Ray(firstPersonCamera.transform.position, firstPersonCamera.transform.forward);

        if (Physics.Raycast(targetRay, out hit, 20f)) {
            if (hit.transform.name == placedMonster.transform.name)
            {
                monsterInfo.SelectTarget();
            } 
            else
            {
                monsterInfo.DeselectTarget();
            }
        }

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
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinBounds))
                {
                    CreateAnchor(hits[0]);
                    levelCanvas.SetActive(true);
                    codeCanvas.SetActive(true);
                    StartCoroutine(ObjectiveUpdate("Read the code and select the correct attack based on it!", 6));
                }
            }
        }
    }

    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        ARAnchor anchor;

        emptyGameObject = Instantiate(new GameObject("EmptyGameObject"));

        Instantiate(dungeonPrefab, emptyGameObject.transform.position, emptyGameObject.transform.rotation);

        mSessionOrigin.MakeContentAppearAt(emptyGameObject.transform, hit.pose.position, hit.pose.rotation);

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
            monsterInfo = placedMonster.GetComponent<MonsterInfo>();
            correctAttack = monsterInfo.correctAttack;
            code.text = stage1Code;
            stage1Monsters.RemoveAt(index);
            if (stage1Monsters.Count == 0)
            {
                stage++;
            }
        } 
        else if (stage == 2)
        {
            if (stage2Monsters.Count == 5)
            {
                emptyHeart4.SetActive(true);
                health = 4;
                UpdateHealth();
                StartCoroutine(ObjectiveUpdate("Well done! Your hp has been restored and you gained an extra heart!", 6));
            }
            if (stage2Monsters.Count == 2)
            {
                StartCoroutine(FinishLevel(false));
                return;
            }
            Destroy(placedMonster);
            int index = random.Next(stage2Monsters.Count);
            placedMonster = Instantiate(stage2Monsters[index]);
            placedMonster.transform.position = emptyGameObject.transform.position;
            monsterInfo = placedMonster.GetComponent<MonsterInfo>();
            correctAttack = monsterInfo.correctAttack;
            code.text = stage2Code;
            stage2Monsters.RemoveAt(index);
        }
    }

    public void Attack(string attack)
    {

        if (!isAttackingAllowed)
        {
            return;
        }
        isAttackingAllowed = false;

        StartCoroutine(AttackAnimation(attack));
        StartCoroutine(MonsterResponse(Equals(attack, correctAttack)));
    }

    private void UpdateHealth()
    {
        if (health == 4)
        {
            heart1.SetActive(true);
            heart2.SetActive(true);
            heart3.SetActive(true);
            heart4.SetActive(true);
        }
        else if (health == 3)
        {
            heart1.SetActive(true);
            heart2.SetActive(true);
            heart3.SetActive(true);
            heart4.SetActive(false);
        }
        else if (health == 2)
        {
            heart1.SetActive(true);
            heart2.SetActive(true);
            heart3.SetActive(false);
            heart4.SetActive(false);
        }
        else if (health == 1)
        {
            heart1.SetActive(true);
            heart2.SetActive(false);
            heart3.SetActive(false);
            heart4.SetActive(false);
        } else
        {
            heart1.SetActive(false);
            heart2.SetActive(false);
            heart3.SetActive(false);
            heart4.SetActive(false);
            StartCoroutine(FinishLevel(true));
        }
    }

    private IEnumerator MonsterResponse(bool correctAttack)
    {
        if (correctAttack)
        {
            if (monsterInfo.health == 1)
            {
                monsterInfo.health--;
            } else
            {
            monsterInfo.health -= 2;
            }
        } else
        {
            if (monsterInfo.name == "Skeleton")
            {
                monsterInfo.EnterBlockStance();
            } else
            {
                monsterInfo.health--;
            }
        }
        if (monsterInfo.health <= 0)
        {
            yield return new WaitForSeconds(4);
            SpawnMonster();
            yield return new WaitForSeconds(1);
            isAttackingAllowed = true;
        } else
        {
            yield return new WaitForSeconds(1);
            health--;
            UpdateHealth();
            yield return new WaitForSeconds(4);
            isAttackingAllowed = true;
        }
    }

    private IEnumerator AttackAnimation(string attack)
    {
        Camera firstPersonCamera = GameObject.Find("AR Camera").GetComponent<Camera>();
        Vector3 direction = placedMonster.transform.position - firstPersonCamera.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);

        switch (attack)
        {
            case "fireball":
                GameObject fireball = Instantiate(fireballPrefab, firstPersonCamera.transform.position, rotation);
                fireball.gameObject.name = "fireball";
                break;
            case "lightningbolt":
                GameObject lightningBolt = Instantiate(lightningPrefab, firstPersonCamera.transform.position, rotation);
                lightningBolt.gameObject.name = "lightningbolt";
                break;
            case "explode":
                GameObject explosion = Instantiate(explosionPrefab, new Vector3(placedMonster.transform.position.x, placedMonster.transform.position.y + 0.15f, placedMonster.transform.position.z), rotation);
                yield return new WaitForSeconds(1.75f);
                if (monsterInfo.health > 0)
                {
                    monsterInfo.Attack();
                } else
                {
                    monsterInfo.Die();
                }
                break;
        }
    }

    private IEnumerator FinishLevel(bool isGameOver)
    {
        yield return new WaitForSeconds(2);
        menuHandlerObject.GetComponent<IfStatementRPGMenuHandler>().FinishLevel(isGameOver);
    }

    private IEnumerator ObjectiveUpdate(string text, float waitLength)
    {
        objectivePanel.SetActive(true);
        objectiveText.text = text;

        yield return new WaitForSeconds(waitLength);

        objectivePanel.SetActive(false);
    }
}
