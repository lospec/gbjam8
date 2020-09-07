using UnityEngine;

namespace Utility
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public GameObject playerObject;

        [SerializeField] private HudManager hud;

        public CharacterController PlayerController { get; set; }

        private void Awake()
        {
            instance = this;
            PlayerController = playerObject.GetComponent<CharacterController>();
        }

        private void OnValidate()
        {
            EventValidator.ValidatePlayerHud(hud, playerObject);
        }
    }
}