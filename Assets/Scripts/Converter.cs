using UnityEngine;

namespace Homework
{
    public sealed class Converter
    {
        private readonly ResourceZone _loadingZone;
        private readonly ResourceZone _unloadingZone;

        private readonly int _amountToTake;
        private readonly int _amountToDeliver;
        private readonly float _conversionTime;

        private bool _isOn;
        private bool _isConverting;

        private float _timer;

        private int _tempStored;

        public Converter(ResourceZone loadingZone, ResourceZone unloadingZone,
            int amountToTake, int amountToDeliver, float conversionTime)
        {
            _loadingZone = loadingZone;
            _unloadingZone = unloadingZone;
            _amountToTake = amountToTake;
            _amountToDeliver = amountToDeliver;
            _conversionTime = conversionTime;
        }

        public void ToggleConverter(bool turnOn)
        {
            _isOn = turnOn;
            if (!_isOn)
            {
                StopConversionAndReturnResources();
            }
        }

        public void Update(float deltaTime)
        {
            if (!_isOn) return;

            if (!_isConverting)
            {
                TryStartNewCycle();
            }

            if (!_isConverting) return;

            _timer += deltaTime;
            if (_timer >= _conversionTime)
            {
                FinishConversionCycle();
            }
        }

        private void TryStartNewCycle()
        {
            if (_loadingZone.CurrentAmount >= _amountToTake)
            {
                _loadingZone.RemoveResource(_amountToTake);
                _tempStored = _amountToTake;

                _isConverting = true;
                _timer = 0f;
            }
        }

        private void FinishConversionCycle()
        {
            _timer = 0f;
            _isConverting = false;

            int leftover = _unloadingZone.AddResource(_amountToDeliver);
            if (leftover > 0)
            {
                Debug.Log($"Burned {leftover} resources");
            }

            _tempStored = 0;
        }

        private void StopConversionAndReturnResources()
        {
            if (_isConverting && _tempStored > 0)
            {
                int leftover = _loadingZone.AddResource(_tempStored);
                if (leftover > 0)
                {
                    Debug.Log($"Burned {leftover} resources");
                }
            }

            _tempStored = 0;
            _isConverting = false;
            _timer = 0f;
        }
    }
}