using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    public Attack longPunch = new Attack(new Vector3(1.8f, 0.1f, 0f), new Vector3(1f, 2f, 1f), new Vector3(0f, 0f, 90f), 0.6f);

    private float lastAttack;
    private Coroutine _attackCoroutine;
    private GameObject _playerTarget;

    public float attackInterval;


    void Start()
    {
        _playerTarget = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

        if (inRange)
            ProcessAttack();
        else
            MoveToPlayer();


    }

    private void ProcessAttack()
    {
        if (Time.time > lastAttack + attackInterval)
        {
            //attack logic
            if (_attackCoroutine == null)
            {
                var punch = CreateHitObject(longPunch.attackSize, longPunch.attackRotation);
                punch.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                punch.transform.localPosition = gameObject.transform.localPosition +
                new Vector3(longPunch.attackOffest.x * -1f, 0f);
                _attackCoroutine = StartCoroutine(AttackRoutine(punch, longPunch.attackDuration));
            }
            lastAttack = Time.time;

        }
    }

    public float attackRange = 2.0f;
    public bool inRange = false;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Collider Enter");
        if (other.gameObject.tag == "Player")
        {
            inRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger Collider Exit");
        if (other.gameObject.tag == "Player")
        {
            inRange = false;
        }
    }

    private void MoveToPlayer()
    {
        transform.LookAt(_playerTarget.transform.position);
        transform.Rotate(new Vector3(0, -90f, 0), Space.Self);

        if (Vector3.Distance(transform.position, _playerTarget.transform.position) > attackRange)
        {
            transform.Translate(new Vector3(2f * Time.deltaTime, 0, 0));
        }
    }

    private IEnumerator AttackRoutine(GameObject attack, float timer)
    {
        while (true)
        {
            yield return new WaitForSeconds(timer);
            Destroy(attack);
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }
    }

    private GameObject CreateHitObject(Vector3 hitSize, Vector3 hitRotation)
    {
        GameObject Hit = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Hit.transform.Rotate(hitRotation);
        Hit.name = "EnemyAttack";
        Hit.transform.localScale = hitSize;
        //Hit.GetComponent<BoxCollider>().isTrigger = true;   
        var rb = Hit.AddComponent<Rigidbody>();
        Physics.IgnoreCollision(Hit.GetComponent<Collider>(), this.gameObject.GetComponent<Collider>());
        //rb.isKinematic = true;
        return Hit;
    }
}
