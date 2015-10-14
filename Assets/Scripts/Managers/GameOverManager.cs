using UnityEngine;

namespace CompleteProject
{
    public class GameOverManager : MonoBehaviour
    {
        public PlayerHealth playerHealth;       // Reference to the player's health.
        public GameObject restartButton;
        public GameObject quitButton;

        Animator anim;                          // Reference to the animator component.

        void Awake ()
        {
            // Set up the reference.
            anim = GetComponent <Animator> ();
            restartButton.SetActive(false);
            quitButton.SetActive(false);
        }


        void Update ()
        {
            // If the player has run out of health...
            if(playerHealth.currentHealth <= 0)
            {
                // ... tell the animator the game is over.
                anim.SetTrigger ("GameOver");
                restartButton.SetActive(true);
                quitButton.SetActive(true);
            }
        }
    }
}