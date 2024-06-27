using System;
using System.Collections.Generic;
using DG.Tweening;
using ProbabilityWheel.GameModule.HelperModule;
using ProbabilityWheel.Imports.PickerWheel.Scripts;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ProbabilityWheel.GameModule.UIModule
{
    public class Wheel : MonoBehaviour
    {
        [Header("View")] 
        [SerializeField] private GameObject _linePrefab;
        [SerializeField] private Transform _linesParent;
        [SerializeField] private Transform _pickerWheelTransform;
        [SerializeField] private Transform _wheelCircle;
        [SerializeField] private GameObject _wheelPiecePrefab;
        [SerializeField] private Transform _wheelPiecesParent;
        [SerializeField] private Vector2 _pieceSize;

        [Space(10)] [Header("Wheel Settings")] 
        [SerializeField] [Range(1, 20)] private int _spinDuration = 8;
        [SerializeField] [Range(.2f, 2f)] private float _wheelSize = 1f;

        [Space(10)] [Header("Wheel Pieces Settings")] [SerializeField]
        private WheelPiece[] _wheelPieces;
        private readonly int _piecesCount = 10;
        
        private ProbabilityHelper _probabilityHelper;
        public event Action onSpinStartEvent;
        public event Action onSpinEndEvent;
        [HideInInspector] public bool IsSpinning = false;
        private float _pieceAngle;
        private float _halfPieceAngle;
        private float _halfPieceAngleWithPaddings;
        private List<int> _nonZeroChancesIndices = new List<int>();
        private void Start()
        {
            _probabilityHelper = new ProbabilityHelper();
            
            _pieceAngle = 360f / _wheelPieces.Length;
            _halfPieceAngle = _pieceAngle / 2f;
            _halfPieceAngleWithPaddings = _halfPieceAngle - (_halfPieceAngle / 4f);

            GeneratePieces();

            _probabilityHelper.CalculateWeightsAndIndices(_wheelPieces, ref _nonZeroChancesIndices);
            if (_nonZeroChancesIndices.Count == 0)
                Debug.LogError("You can't set all pieces chance to zero");
        }

        private void GeneratePieces()
        {
            _wheelPiecePrefab = InstantiatePiece();

            RectTransform rt = _wheelPiecePrefab.transform.GetChild(0).GetComponent<RectTransform>();
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _pieceSize.x);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _pieceSize.y);

            for (int i = 0; i < _wheelPieces.Length; i++)
                DrawPiece(i);

            Destroy(_wheelPiecePrefab);
        }

        private void DrawPiece(int index)
        {
            WheelPiece piece = _wheelPieces[index];
            Transform pieceTrns = InstantiatePiece().transform.GetChild(0);

            pieceTrns.GetChild(0).GetComponent<Image>().sprite = piece.Icon;
            pieceTrns.GetChild(1).GetComponent<Text>().text = piece.Label;
            pieceTrns.GetChild(2).GetComponent<Text>().text = piece.Amount.ToString();
            
            Transform lineTrns = Instantiate(_linePrefab, _linesParent.position, Quaternion.identity, _linesParent)
                .transform;
            lineTrns.RotateAround(_wheelPiecesParent.position, Vector3.back, (_pieceAngle * index) + _halfPieceAngle);

            pieceTrns.RotateAround(_wheelPiecesParent.position, Vector3.back, _pieceAngle * index);
        }

        private GameObject InstantiatePiece()
        {
            return Instantiate(_wheelPiecePrefab, _wheelPiecesParent.position, Quaternion.identity, _wheelPiecesParent);
        }
        
        public void Spin()
        {
            if (!IsSpinning)
            {
                IsSpinning = true;
                if (onSpinStartEvent != null)
                    onSpinStartEvent.Invoke();

                int index = _probabilityHelper.GetRandomPieceIndex(_wheelPieces);
                WheelPiece piece = _wheelPieces[index];

                if (piece.Chance == 0 && _nonZeroChancesIndices.Count != 0)
                {
                    index = _nonZeroChancesIndices[Random.Range(0, _nonZeroChancesIndices.Count)];
                    piece = _wheelPieces[index];
                }

                float angle = -(_pieceAngle * index);

                float rightOffset = (angle - _halfPieceAngleWithPaddings) % 360;
                float leftOffset = (angle + _halfPieceAngleWithPaddings) % 360;

                float randomAngle = Random.Range(leftOffset, rightOffset);

                Vector3 targetRotation = Vector3.back * (randomAngle + 2 * 360 * _spinDuration);

                //float prevAngle = wheelCircle.eulerAngles.z + halfPieceAngle ;
                float prevAngle, currentAngle;
                prevAngle = currentAngle = _wheelCircle.eulerAngles.z;

                bool isIndicatorOnTheLine = false;

                _wheelCircle
                    .DORotate(targetRotation, _spinDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.InOutQuart)
                    .OnUpdate(() =>
                    {
                        float diff = Mathf.Abs(prevAngle - currentAngle);
                        if (diff >= _halfPieceAngle)
                        {
                            prevAngle = currentAngle;
                            isIndicatorOnTheLine = !isIndicatorOnTheLine;
                        }

                        currentAngle = _wheelCircle.eulerAngles.z;
                    })
                    .OnComplete(() =>
                    {
                        IsSpinning = false;
                        if (onSpinEndEvent != null)
                        {
                            onSpinEndEvent.Invoke();
                        }
                    });
            }
        }
        
        private void OnValidate()
        {
            if (_pickerWheelTransform != null)
                _pickerWheelTransform.localScale = new Vector3(_wheelSize, _wheelSize, 1f);

            if (_wheelPieces.Length > _piecesCount)
                Debug.LogError("[ PickerWheel ]  pieces count must be  " + _piecesCount);
        }
    }
}