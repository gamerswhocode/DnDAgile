using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    public Attack longPunch = new Attack(new Vector3(1.8f, 0.1f, 0f), new Vector3(1f, 2f, 1f), new Vector3(0f, 0f, 90f), 0.6f);

    private float lastAttack;
    private Coroutine _attackCoroutine;

    public float attackInterval;


    // Update is called once per frame
    void Update()
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
