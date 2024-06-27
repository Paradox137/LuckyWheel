using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ProbabilityWheel.GameModule.UIModule
{
    public class WheelMediator : MonoBehaviour
    {
        [SerializeField] private Button _spinButton;
        [SerializeField] private Mana _mana;
        [SerializeField] private Wheel _wheel;

        private bool _awaitEndOfSpin;

        private void OnEnable()
        {
            SetupEvents();
        }
        
        private void SetupEvents()
        {
            _spinButton.onClick.AddListener(_mana.Spend);
            _spinButton.onClick.AddListener(_wheel.Spin);
            _mana.onManaEmpty += BlockButton;
            _mana.onManaRestore += UnBlockButton;
            _wheel.onSpinStartEvent += BlockButton;
            _wheel.onSpinEndEvent += UnBlockButton;
        }
        
        private void UnBlockButton()
        {
            if (_wheel.IsSpinning && !_awaitEndOfSpin)
            {
                _awaitEndOfSpin = true;
                
                StartCoroutine(AwaitSpin());
                
                return;
            }
            
            if(!_wheel.IsSpinning && !_mana.IsEmpty)
                _spinButton.interactable = true;
        }

        private IEnumerator AwaitSpin()
        {
            yield return new WaitUntil(() => !_wheel.IsSpinning);
            
            _awaitEndOfSpin = false;
            
            _spinButton.interactable = true;
        }
        
        private void BlockButton() => _spinButton.interactable = false;

        private void OnDisable()
        {
            _spinButton.onClick.RemoveAllListeners();
            _mana.onManaEmpty -= BlockButton;
            _mana.onManaRestore -= UnBlockButton;
            _wheel.onSpinStartEvent -= BlockButton;
            _wheel.onSpinEndEvent -= UnBlockButton;
        }
    }
}