using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{
    public class GameLogic : MonoBehaviour
    {
        public System.Action OnCastleReached;
        public System.Action OnTowerCollected;

        private void OnEnable()
        {
            OnCastleReached += EndGame;
        }

        private void OnDisable()
        {
            OnCastleReached -= EndGame;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void EndGame()
        {

        }
    }
}
