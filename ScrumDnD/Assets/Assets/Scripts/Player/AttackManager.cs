using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private PlayerController _playerController;

    public Attack punch1 = new Attack(new Vector3(1.6f, 0.1f, 0f), new Vector3(0.5f, 1f, 0.5f), new Vector3(0f, 0f, 0f), 0.2f);
    public Attack punch2 = new Attack(new Vector3(1.6f, 0.1f, 0f), new Vector3(0.5f, 1.5f, 0.5f), new Vector3(0f, 0f, 0f), 0.2f);
    public Attack punch3 = new Attack(new Vector3(1.6f, 0.1f, 0f), new Vector3(0.5f, 1f, 2.5f), new Vector3(0f, 0f, 0f), 0.3f);
    public Attack punch4 = new Attack(new Vector3(1.6f, 0.1f, 0f), new Vector3(1f, 1.5f, 1.5f), new Vector3(0f, 0f, 0f), 0.5f);

    public Attack longPunch = new Attack(new Vector3(1.8f, 0.1f, 0f), new Vector3(1f, 2f, 1f), new Vector3(0f, 0f, 90f), 0.6f);

    public Attack shoot1 = new Attack(new Vector3(1.6f, 0.1f, 0f), new Vector3(0.5f, 1f, 0.5f), new Vector3(0f, 0f, 90f), 0.6f);


    

    private KeyCombo HeavyAttack = new KeyCombo(new string[] { "right", "Fire2" });
    private KeyCombo HeavyAttack2 = new KeyCombo(new string[] { "left", "Fire2" });

    private KeyCombo LongRangeAttack = new KeyCombo(new string[] { "down", "right", "Fire2" });
    private KeyCombo LongRangeAttack2 = new KeyCombo(new string[] { "down", "left", "Fire2" });

    private KeyCombo DesperationAttack = new KeyCombo(new string[] { "left", "down", "right", "Fire2" });
    private KeyCombo DesperationAttack2 = new KeyCombo(new string[] { "right", "down", "left", "Fire2" });


    private Coroutine _attackCoroutine;
    private Coroutine _jumpAttackRoutine;
    private int regularCombo = 0;
    private float regularComboLastPress;
    public float regularComboTime = 0.6f;

    void Start()
    {
        _playerController = this.gameObject.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (_playerController.PlayerCanAttack())
        {
            if (_playerController._playerStatus == Helper.PlayerStatus.Jumping && Input.GetButtonDown("Fire2"))
            {
                DiveKick(punch1);
                //SecondaryJumpAttack(punch1);
            }

            else if (DesperationAttack.Check() || DesperationAttack2.Check())
            {
                // do the falcon punch
                Debug.Log("Defensive");
                Punch(longPunch);
                resetIndexAll();
            }

            else if (LongRangeAttack.Check() || LongRangeAttack2.Check())
            {
                Shoot(shoot1);
                resetIndexAll();
            }

            else if (HeavyAttack.Check() || HeavyAttack2.Check())
            {
                Punch(longPunch);
                resetIndexAll();
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                UpdateRegularComboIndex();
                switch (regularCombo)
                {
                    case 0:
                        Punch(punch1);
                        //1st attack
                        break;
                    case 1:
                        Punch(punch2);
                        //2nd attack
                        break;
                    case 2:
                        Punch(punch3);
                        //3d attack
                        break;
                    case 3:
                        Punch(punch4);
                        regularCombo = 0;
                        //4th attack
                        break;
                    default:
                        Punch(punch1);
                        break;
                }
                regularComboLastPress = Time.time;
            }

        }
    }

    private void UpdateRegularComboIndex()
    {
        if (Time.time > regularComboLastPress + regularComboTime)
            regularCombo = 0;
        else 
        { 
            regularCombo++;
            regularComboLastPress = Time.time;
        }
    }

    private void resetIndexAll()
    {
        HeavyAttack.resetIndex();
        DesperationAttack.resetIndex();
        LongRangeAttack.resetIndex();
        regularCombo = 0;
    }


    private GameObject CreateHitObject(Vector3 hitSize, Vector3 hitRotation)
    {
        GameObject Hit = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Hit.transform.Rotate(hitRotation);
        Hit.name = "PlayerAttack";
        Hit.transform.localScale = hitSize;
        //Hit.GetComponent<BoxCollider>().isTrigger = true;   
        var rb = Hit.AddComponent<Rigidbody>();
        //rb.isKinematic = true;
        Physics.IgnoreCollision(Hit.GetComponent<Collider>(), this.gameObject.GetComponent<Collider>());
        return Hit;
    }

    private IEnumerator JumpAttackRoutine(float timer)
    {
        while (true)
        {
            //if (_playerController.IsGrounded())
            {
                yield return new WaitForSeconds(timer);
                _playerController._playerStatus = Helper.PlayerStatus.Neutral;
                StopCoroutine(_jumpAttackRoutine);
                _jumpAttackRoutine = null;
                this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
        
    }

    private void SecondaryJumpAttack(Attack pAttack)
    {
        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        var diveKick = CreateHitObject(pAttack.attackSize, pAttack.attackRotation);
        //diveKick.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        diveKick.GetComponent<Rigidbody>().AddForce(_playerController.isCharacterFlipped() ? new Vector3(-30f, -30f, 0f) :
            new Vector3(30f, -30f, 0f),
            ForceMode.VelocityChange);
        diveKick.transform.localPosition = gameObject.transform.localPosition +
        (_playerController.isCharacterFlipped() ? new Vector3(pAttack.attackOffest.x * -1f, 0f) : pAttack.attackOffest);
        _jumpAttackRoutine = StartCoroutine(JumpAttackRoutine(pAttack.attackDuration));
        _playerController._playerStatus = Helper.PlayerStatus.JumpAttacking;
        Destroy(diveKick, 1f);
    }

    private void DiveKick(Attack pAttack)
    {
        if (_jumpAttackRoutine == null)
        {
            //var diveKick = CreateHitObject(pAttack.attackSize, pAttack.attackRotation);
            //diveKick.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            this.gameObject.GetComponent<Rigidbody>().AddForce(_playerController.isCharacterFlipped() ? new Vector3(-30f, -30f, 0f) :
                new Vector3(30f, -30f, 0f),
                ForceMode.VelocityChange);
            //diveKick.transform.localPosition = gameObject.transform.localPosition +
            //(_playerController.isCharacterFlipped() ? new Vector3(pAttack.attackOffest.x * -1f, 0f) : pAttack.attackOffest);
            _jumpAttackRoutine = StartCoroutine(JumpAttackRoutine(pAttack.attackDuration));
            _playerController._playerStatus = Helper.PlayerStatus.JumpAttacking;
        }
    }


    private IEnumerator AttackRoutine(GameObject attack, float timer)
    {
        while (true)
        {
            yield return new WaitForSeconds(timer);
            Destroy(attack);
            _playerController._playerStatus = Helper.PlayerStatus.Neutral;
            StopCoroutine(_attackCoroutine);
            Destroy(attack);
            _attackCoroutine = null;
        }
    }


    private void Shoot(Attack pAttack)
    {
        if (_attackCoroutine == null)
        {
            var shot = CreateHitObject(pAttack.attackSize, pAttack.attackRotation);
            shot.GetComponent<Rigidbody>().AddForce(new Vector3(_playerController.isCharacterFlipped() ? -3000f : 3000f, 0f, 0f));
            shot.transform.position = GameObject.FindGameObjectWithTag("Player").transform.position +
                (_playerController.isCharacterFlipped() ? new Vector3(pAttack.attackOffest.x * -1f, 0f) : pAttack.attackOffest);
            _attackCoroutine = StartCoroutine(AttackRoutine(shot, pAttack.attackDuration));
            _playerController._playerStatus = Helper.PlayerStatus.Attacking;
        }
    }

    private void Punch(Attack pAttack)
    {
        if (_attackCoroutine == null)
        {
            var punch = CreateHitObject(pAttack.attackSize, pAttack.attackRotation);
            punch.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            punch.transform.localPosition = gameObject.transform.localPosition +
            (_playerController.isCharacterFlipped() ? new Vector3(pAttack.attackOffest.x * -1f, 0f) : pAttack.attackOffest);
            _attackCoroutine = StartCoroutine(AttackRoutine(punch, pAttack.attackDuration));
            _playerController._playerStatus = Helper.PlayerStatus.Attacking;
        }
    }



}

public class Attack
{
    public Vector3 attackOffest;
    public Vector3 attackRotation;
    public Vector3 attackSize;
    public float attackDuration;

    public Attack(Vector3 offset, Vector3 size, Vector3 rotation, float duration)
    {
        attackOffest = offset;
        attackSize = size;
        attackRotation = rotation;
        attackDuration = duration;
    }
}

public class KeyCombo
{
    public string[] buttons;
    private int currentIndex = 0; //moves along the array as buttons are pressed

    public float allowedTimeBetweenButtons = 0.2f; //tweak as needed
    private float timeLastButtonPressed;

    public void resetIndex()
    {
        currentIndex = 0;
    }

    public KeyCombo(string[] b)
    {
        buttons = b;
    }

    //usage: call this once a frame. when the combo has been completed, it will return true
    public bool Check()
    {
        if (Time.time > timeLastButtonPressed + allowedTimeBetweenButtons)
            currentIndex = 0;
        {
            if (currentIndex < buttons.Length)
            {
                if ((buttons[currentIndex] == "down" && Input.GetAxisRaw("Vertical") == -1) ||
                (buttons[currentIndex] == "up" && Input.GetAxisRaw("Vertical") == 1) ||
                (buttons[currentIndex] == "left" && Input.GetAxisRaw("Horizontal") == -1) ||
                (buttons[currentIndex] == "right" && Input.GetAxisRaw("Horizontal") == 1) ||
                (buttons[currentIndex] != "down" && buttons[currentIndex] != "up" && buttons[currentIndex] != "left" && buttons[currentIndex] != "right" && Input.GetButtonDown(buttons[currentIndex])))
                {
                    timeLastButtonPressed = Time.time;
                    currentIndex++;
                }

                if (currentIndex >= buttons.Length)
                {
                    currentIndex = 0;
                    return true;
                }
                else return false;
            }
        }

        return false;
    }
}
