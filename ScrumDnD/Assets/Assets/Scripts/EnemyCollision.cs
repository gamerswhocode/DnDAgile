using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace UnityStandardAssets.Utility
{
    public class EnemyCollision : MonoBehaviour
    {
        public Slider _enemyHealthGUI;
        public Text _enemyTextGUI;
        public float _maxHealth;
        public float _currentHealth;

        void Start()
        {
            _maxHealth = Random.Range(100, 200);
            _currentHealth = _maxHealth;
            _enemyHealthGUI = GameObject.Find("EnemyHealth").GetComponent<Slider>();
            _enemyTextGUI = GameObject.Find("EnemyNameTxt").GetComponent<Text>();
            _enemyHealthGUI.minValue = 0;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name.Equals("PlayerAttack") || 
                //DIVE KICK TEMP IMPLEMENTATION
                collision.gameObject.tag.Equals("Player"))
            {
                _currentHealth -= 20;
                _enemyHealthGUI.maxValue = _maxHealth;
                _enemyHealthGUI.value = _currentHealth;
                _enemyTextGUI.text = this.gameObject.name;
            }

            if (_currentHealth <= 0)
                Destroy(this.gameObject);
        }
    }
}
