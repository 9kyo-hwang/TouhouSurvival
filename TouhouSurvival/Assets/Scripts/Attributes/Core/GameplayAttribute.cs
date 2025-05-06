using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class GameplayAttribute
    {
        public float BaseValue => _baseValue;

        public float CurrentValue
        {
            get
            {
                if (_shouldUpdate)
                {
                    _currentValue = CalculateFinalValue();
                    _shouldUpdate = false;
                }

                return _currentValue;
            }
        }

        public float MinValue => _minValue;
        public float MaxValue => _maxValue;

        public event EventHandler<AttributeChangedEventArgs> OnAttributeChanged;

        private float _baseValue;
        private float _currentValue;
        private float _minValue;
        private float _maxValue;
        
        private bool _shouldUpdate;
        private List<GameplayAttributeModifier> _modifiers;

        public GameplayAttribute(float baseValue, float minValue = float.MinValue, float maxValue = float.MaxValue, string description = "")
        {
            _baseValue = baseValue;
            _currentValue = baseValue;
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public void AddModifier(GameplayAttributeModifier modifier)
        {
            _modifiers.Add(modifier);
            _modifiers.Sort();

            _shouldUpdate = true;
        }

        private float CalculateFinalValue()
        {
            float finalValue = 0.0f;

            float flatSum = 0.0f;
            float percAdd = 1.0f;
            float percMul = 1.0f;

            for (int i = 0; i < _modifiers.Count; ++i)
            {
                switch(_modifiers[i].opcode)
                {
                    case GameplayAttributeOperator.Flat:
                        flatSum += _modifiers[i].value;
                        break;
                    case GameplayAttributeOperator.PercentAdd:
                        percAdd += _modifiers[i].value;
                        break;
                    case GameplayAttributeOperator.PercentMul:
                        percMul *= (1.0f + _modifiers[i].value);
                        break;
                }
            }

            finalValue = flatSum * percAdd * percMul;

            if (_minValue != float.MinValue && finalValue < _minValue)
                return _minValue;
            if (_maxValue != float.MaxValue && finalValue > _maxValue)
                return _maxValue;

            return finalValue;
        }
    }
}