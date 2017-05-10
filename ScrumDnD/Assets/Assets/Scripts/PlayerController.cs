﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityStandardAssets.Utility; 
using System.IO;

public class PlayerController : MonoBehaviour {
    //TODO : polish physics and interactions
    //TODO : enemy AI
    //TODO : set initial stage
    //TODO : character 2
    //TODO : Select character
    //TODO : Fix camera with multiple players at once
    //TODO : universal controls (or at least work with generic work controller)


    public BuffManager _buffManager;

    private Coroutine _buffingCoroutine;
    private Coroutine _damageCoroutine;
    private Coroutine _invulnerableCoroutine;

    public bool _currentlyInvulnerable = false;
    public float _invulnerabilityFrames = 0.3f;



    public Helper.PlayerStatus _playerStatus;

    private GameObject _menu;

    public bool _disabledMovement = false;
    public bool _flippedCharacter = false;

    public float _abilityActivationRate = 1.35f;

    public float _jumpForce = 6f;
    

    public float speed = 1f;
    private float maxSpeedX = 5f;
    private float maxSpeedZ = 15f;

    
    public Vector3 _menuOffset = new Vector3(2f,3f,0f);

    private float distToGround;

    void Start () {
        _buffManager = gameObject.AddComponent<BuffManager>();
        

        distToGround = GetComponent<Collider>().bounds.extents.y;
        _playerStatus = Helper.PlayerStatus.Neutral;
    }


    private IEnumerator InvulnerabilityFrames()
    {
        while (true)
        {
            yield return new WaitForSeconds(_invulnerabilityFrames);
            _currentlyInvulnerable = false;
            StopCoroutine(_invulnerableCoroutine);
            _invulnerableCoroutine = null;
        }
    }

    private IEnumerator TakeDamage(float duration)
    {
        while (true)
        {
            yield return new WaitForSeconds(duration);
            _playerStatus = Helper.PlayerStatus.Neutral;
            StopCoroutine(_damageCoroutine);
            _damageCoroutine = null;
            _invulnerableCoroutine = StartCoroutine(InvulnerabilityFrames());
        }
    }

    void Update()
    {
        Debug.Log("Currently Invulnerable : "+_currentlyInvulnerable);

        if (_playerStatus != Helper.PlayerStatus.TakingDamage)
        {
            ValidateKeyInputs();
            HandlePlayerStats();
        }
        else
        {
            ValidateDamageInteraction();
        }

    }

    private void ValidateDamageInteraction()
    {
        if(_damageCoroutine == null && !_currentlyInvulnerable)
        { 
            _currentlyInvulnerable = true;
            _damageCoroutine = StartCoroutine(TakeDamage(0.7f));
        }
    }


    private void HandlePlayerStats()
    {
        if (_buffManager.ManagerSpeedActive())
        {
            speed = 3;
        }
        else
            speed = 1;
    }

    public bool isCharacterFlipped()
    {
        return _flippedCharacter;
    }

    private void DetermineFlipSprite(float x)
    {
        if (x < 0)
        { 
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
            _flippedCharacter = true;
        }
        else if (x > 0)
        { 
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
            _flippedCharacter = false;
        }
    }

    private bool PlayerMovementDisabled()
    {
        return _playerStatus == Helper.PlayerStatus.ActivatingBuff ||
            _playerStatus == Helper.PlayerStatus.Attacking ||
            _playerStatus == Helper.PlayerStatus.CastingSpell ||
            _playerStatus == Helper.PlayerStatus.TakingDamage;
    }

    private void Walk(float xMovement, float yMovement)
    {
        var x = xMovement * Time.deltaTime * (maxSpeedX * speed);
        var z = yMovement * Time.deltaTime * (maxSpeedZ * speed);
        DetermineFlipSprite(x);
        Vector3 movement = new Vector3(x, 0.0f, z);
        GetComponent<Rigidbody>().transform.Translate(movement);
        if (IsGrounded())
            _playerStatus = Helper.PlayerStatus.Moving;
    }

    private void Run(float xMovement, float yMovement)
    {
        var x = xMovement * Time.deltaTime * (maxSpeedX * speed * 2);
        var z = yMovement * Time.deltaTime * (maxSpeedZ * speed * 2);
        DetermineFlipSprite(x);
        Vector3 movement = new Vector3(x, 0.0f, z);
        GetComponent<Rigidbody>().transform.Translate(movement);
        if (IsGrounded())
            _playerStatus = Helper.PlayerStatus.Dashing;
    }

    private void DoCharacterMovement()
    {
        if (!PlayerMovementDisabled())
        {
            var xMovement = Input.GetAxis("Horizontal");
            var yMovement = Input.GetAxis("Vertical");

            if ((xMovement != 0 || yMovement != 0))
            {
                if (Input.GetButton("Fire3"))
                    Run(xMovement, yMovement);
                else
                    Walk(xMovement, yMovement);
            }
            else
            {
                if (IsGrounded())
                    _playerStatus = Helper.PlayerStatus.Neutral;
            }
        }
    }

    private void RemoveAbilityMenu()
    {
        Destroy(_menu, 0.05f);
    }

    private void ValidateKeyInputs()
    { 

        if (Input.GetAxis(("RightTrigger")) == 0)
        {
            Destroy(_menu, 0.05f);
        }

        if (!PlayerMovementDisabled())
        {

            ProcessAbilityInput();
            DoCharacterMovement();
            if ((Input.GetAxis("RightTrigger") != 0f && GameObject.FindGameObjectWithTag("Ability_Menu") == null))
            {
                AbilityMenu();
            }
            if (Input.GetButtonDown("Fire1"))
                Jump();
        }
    }

    public bool PlayerCanAttack()
    {

        return _playerStatus == Helper.PlayerStatus.Neutral ||
            _playerStatus == Helper.PlayerStatus.Moving ||
            _playerStatus == Helper.PlayerStatus.Dashing ||
            _playerStatus == Helper.PlayerStatus.Jumping;
    }

    private void Jump()
    {
        if(IsGrounded())
        {
            _playerStatus = Helper.PlayerStatus.Jumping;
            GetComponent<Rigidbody>().AddForce(new Vector3(0, _jumpForce, 0), ForceMode.Impulse);
        }
    }

    private void ProcessAbilityInput()
    {
        if (AbilityMenuIsActive() && _buffManager.ManagerCanBuffPlayer())
        {
            if (Input.GetButtonDown("Fire3"))
            {
                _playerStatus = Helper.PlayerStatus.ActivatingBuff;
                AbilityTwo();
            }

            if (Input.GetButtonDown("Fire2"))
            {
                _playerStatus = Helper.PlayerStatus.ActivatingBuff;
                AbilityTwo();
            }
        }
    }
    

    private void AbilityTwo()
    {
        _buffingCoroutine = StartCoroutine(ActivatingAbility("Speed"));
    }

    private void AbilityOne()
    {
        _buffingCoroutine = StartCoroutine(ActivatingAbility("Range"));
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.001f);
    }

    private IEnumerator ActivatingAbility(string ability)
    {
        while (true)
        {
            yield return new WaitForSeconds(_abilityActivationRate);
            _playerStatus = Helper.PlayerStatus.Neutral;
            StopCoroutine(_buffingCoroutine);
            _buffManager.ManagerBuffPlayer(ability);
        }
    }

    private bool AbilityMenuIsActive()
    {
        return GameObject.FindGameObjectWithTag("Ability_Menu") != null;
    }

    private void AbilityMenu()
    {
        _menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var targetedMenu = _menu.AddComponent<FollowTarget>();
        targetedMenu.target = this.gameObject.transform;
        targetedMenu.tag = "Ability_Menu";
        targetedMenu.menuOffset = _menuOffset;
        targetedMenu.isMenu = true;
        _menu.transform.localScale = new Vector3(3f, 2.5f, 0.1f);
        
    }
}