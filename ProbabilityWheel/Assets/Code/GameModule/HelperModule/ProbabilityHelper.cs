using System.Collections.Generic;
using ProbabilityWheel.Imports.PickerWheel.Scripts;

namespace ProbabilityWheel.GameModule.HelperModule
{
    public class ProbabilityHelper
    {
        private readonly System.Random _random = new System.Random();
        private double _accumulatedWeight;
        
        public int GetRandomPieceIndex(WheelPiece[] __wheelPieces)
        {
            double r = _random.NextDouble() * _accumulatedWeight;

            for (int i = 0; i < __wheelPieces.Length; i++)
                if (__wheelPieces[i].Weight >= r)
                    return i;

            return 0;
        }

        public void CalculateWeightsAndIndices(WheelPiece[] __wheelPieces, ref List<int> __nonZeroChancesIndices)
        {
            for (int i = 0; i < __wheelPieces.Length; i++)
            {
                WheelPiece piece = __wheelPieces[i];
                
                _accumulatedWeight += piece.Chance;
                piece.Weight = _accumulatedWeight;

                piece.Index = i;
                
                if (piece.Chance > 0)
                    __nonZeroChancesIndices.Add(i);
            }
        }
    }
}