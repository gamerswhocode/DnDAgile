using UnityEngine;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour {

    // Use this for initialization
    private Slider _playerHealth;
    public float _maxHealth;
    public float _currentHealth;

    void Start()
    {
        
        _playerHealth = GameObject.Find("PlayerHealth").GetComponent<Slider>();
        _maxHealth = 100f;
        _currentHealth = _maxHealth;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("EnemyAttack") && ! this.gameObject.GetComponent<PlayerController>()._currentlyInvulnerable)
        {
            _currentHealth -= 15;
            _playerHealth.maxValue = _maxHealth;
            _playerHealth.value = _currentHealth;
            this.gameObject.GetComponent<PlayerController>()._playerStatus = Helper.PlayerStatus.TakingDamage;
        }

        if (_currentHealth <= 0)
            Destroy(this.gameObject);
    }
}
