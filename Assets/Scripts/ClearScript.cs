using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class ClearScript : MonoBehaviour
    {
        public static ClearScript instance;

        [Header("クリアUIで最初に選択されているボタン")]
        [SerializeField] GameObject _firstSelectButton = default;

        //EventSystem
        [SerializeField] GameObject _eventSystem = default;

        private EventSystem _system = default;

        private void Start()
        {
            _system = _eventSystem.GetComponent<EventSystem>();
            SetClearUI();
            print("UI");
        }

        public void SetClearUI()
        {
            _system.firstSelectedGameObject = _firstSelectButton;
            //print("fdajslk;");
        }
    }
}