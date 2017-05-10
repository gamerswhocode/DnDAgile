using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class Player : MonoBehaviour{

    private int _HP;
    private int _MP;

    private float _speed;
    private float _jumpForce;

    private Vector3 _punchRange;
    private Vector3 _kickRange;
    private Vector3 _jumpAttackRange;
    
    private PlayerController _inputManager;
    private Animator _animatorManager;
    private AttackManager _attackManager;


    public void setPlayerStats(int hp, int mp)
    {
        _HP = hp;
        _MP = mp;
    }

    void Start()
    {
        _inputManager = gameObject.AddComponent<PlayerController>();
        _animatorManager = gameObject.GetComponent<Animator>();
        _attackManager = gameObject.AddComponent<AttackManager>();
    }

    void LateUpdate()
    {
        //CastingSpell = 0, ActivatingBuff, Neutral, Moving, Dashing, Attacking,
        //Jumping, JumpAttacking, TakingDamage
        _animatorManager.SetInteger("StateId", (int)_inputManager._playerStatus);

    }
	
}
