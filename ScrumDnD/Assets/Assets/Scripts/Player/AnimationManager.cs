using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.Utility;

public class AnimationManager : MonoBehaviour {

    private bool _flippedCharacter = false;

    private  Vector3 _menuOffset;
    private GameObject _menu;

    private Sprite _shootSprite;
    private Sprite _neutralSprite;
    private Sprite _activeAbility;
    private Sprite _jumpingSprite;
    private Sprite _dashSrpite;
    private Sprite _walkingSprite;
    private Animator _walkingTest;

    public void SetAnimations(Sprite shoot, Sprite neutral, Sprite active, Sprite jump, Sprite dash, Sprite walking)
    {
        _shootSprite = shoot;
        _neutralSprite = neutral;
        _activeAbility = active;
        _jumpingSprite = jump;
        _dashSrpite = dash;
        _walkingSprite = walking;
    }
    //CastingSpell, ActivatingBuff, Neutral, Moving, Dashing, Attacking,
    //    Jumping, JumpAttacking, TakingDamage

    public void UpdateAnimations(Helper.PlayerStatus playerStatus)
    {
        //update based on playerStatus
        switch (playerStatus)
        {
            case Helper.PlayerStatus.Attacking:
                CharacterToPunchAnimation();
                break;
            case Helper.PlayerStatus.ActivatingBuff:
                CharacterToBuffAnimation();
                break;
            case Helper.PlayerStatus.Jumping:
                CharacterToJumpAnimation();
                break;
            case Helper.PlayerStatus.Moving:
                CharacterToMoveAnimation();
                break;
            case Helper.PlayerStatus.Dashing:
                CharacterToDashAnimation();
                break;
            default:
                CharacterToNeutralAnimation();
                break;
        }

    }

    private void CharacterToMoveAnimation()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _walkingSprite;
    }

    private void CharacterToDashAnimation()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _dashSrpite;
    }

    private void CharacterToPunchAnimation()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _shootSprite;
    }

    private void CharacterToBuffAnimation()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _activeAbility;
    }

    private void CharacterToJumpAnimation()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _jumpingSprite;
    }

    private void CharacterToNeutralAnimation()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _neutralSprite;
    }

    public void DetermineFlipSprite(float x)
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

    #region Menu


    private bool AbilityMenuIsActive()
    {
        return GameObject.FindGameObjectWithTag("Ability_Menu") != null;
    }

    private void AbilityMenu()
    {
        _menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _menu.transform.localScale = new Vector3(3f, 2.5f, 0.1f);
        var targetedMenu = _menu.AddComponent<FollowTarget>();
        targetedMenu.target = this.gameObject.transform;
        targetedMenu.tag = "Ability_Menu";
        targetedMenu.menuOffset = _menuOffset;
        targetedMenu.isMenu = true;
    }

    #endregion

    #region Attacks

    #endregion
}
