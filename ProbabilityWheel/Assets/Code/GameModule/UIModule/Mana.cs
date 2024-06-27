using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace ProbabilityWheel.GameModule.UIModule
{
    public class Mana : MonoBehaviour
    {
        [Header("In Seconds")]
        [SerializeField] private float _manaRestoreTime;
        [SerializeField] private uint _startManaCount;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private TextMeshProUGUI _manaCountText;
        
        [HideInInspector] public bool IsEmpty;
        private uint _currentMana;
        public event Action onManaEmpty;
        public event Action onManaRestore;
        
        private void Start()
        {
            _currentMana = _startManaCount;
            _manaCountText.text = _currentMana.ToString();
            
            if (_currentMana == 0)
            {
                IsEmpty = true;
                onManaEmpty?.Invoke();
            }

            StartCoroutine(StartTimer());
        }

        private IEnumerator StartTimer()
        {
            float counter = _manaRestoreTime;
            
            while (counter >= 0)
            {
                yield return new WaitForSeconds(1);
                _timerText.text = counter + "s";
                counter--;
            }
            
            _currentMana++;
            _manaCountText.text = _currentMana.ToString();

            if (_currentMana == 1)
            {
                IsEmpty = false;
                
                onManaRestore?.Invoke();
            }

            StartCoroutine(StartTimer());
        }

        public void Spend()
        {
            if (!IsEmpty)
            {
                _currentMana--;
                
                _manaCountText.text = _currentMana.ToString();
                
                CheckForEmpty();
            }
        }

        private void CheckForEmpty()
        {
            if (_currentMana <= 0)
            {
                IsEmpty = true;
                
                onManaEmpty?.Invoke();
            }
        }
    }
}