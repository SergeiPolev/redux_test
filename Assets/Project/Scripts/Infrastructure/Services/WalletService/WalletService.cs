using System.Collections.Generic;
using UnityEngine;
using System;
using Data;

namespace Services
{
    [Serializable]
    public class WalletData : SaveData
    {
        public List<CurrencyData> CurrencyData;
        public WalletData()
        {
            CurrencyData = new List<CurrencyData>
            {
                new(CurrencyId.Coins, 0f),
                new(CurrencyId.Crystal, 0f)
            };
        }
    }

    [Serializable]
    public class CurrencyData 
    {
        public CurrencyId CurrencyId;
        public float Value;

        public CurrencyData(CurrencyId currencyId, float value)
        {
            CurrencyId = currencyId;
            Value = value;
        }
    }

    public class WalletService : IService, ISaveProgress
    {
        public const string SAVE_ID = "Wallet";

        private WalletData _wallet;

        public event Action<CurrencyId> OnCurrencyValueChanged;
        
        public void Initialize()
        {

        }

        public void WriteProgress(SaveLoadService saveService)
        {
            saveService.SetData(SAVE_ID, _wallet);
        }

        public void LoadProgress(SaveLoadService saveService)
        {
            if (saveService.GetSaveData(SAVE_ID, out _wallet) == false)
            {
                _wallet = new WalletData();
                saveService.SaveProgress(this);
            }
        }

        public void AddValue(CurrencyId currencyId, float value)
        {
            if (value <= 0)
            {
                Debug.Log($"Incorrect add {currencyId} value {value}");
                return;
            }

            AddValue(ref GetCurrencyValue(currencyId), value);
            OnCurrencyValueChanged?.Invoke(currencyId);
        }

        public void SpendValue(CurrencyId currencyId, float value)
        {
            if (value <= 0)
            {
                Debug.Log($"Incorrect add {currencyId} value {value}");
                return;
            }

            
            SpendValue(ref GetCurrencyValue(currencyId), value);
            OnCurrencyValueChanged?.Invoke(currencyId);
        }

        public int GetValue(CurrencyId resourceType)
        {
            return (int)GetCurrencyValue(resourceType);

        }
        public bool IsEnough(CurrencyId resourceType, float value)
        {
            return value <= GetValue(resourceType);
        }

        private void AddValue(ref float resourceValue, float value)
        {
            resourceValue += value;
        }

        private void SpendValue(ref float resourceValue, float value)
        {
            if (resourceValue < value)
            {
                Debug.LogError($"Incorrect resource value {resourceValue}");
                return;
            }

            resourceValue -= value;
        }

        public void SetValue(CurrencyId currencyId, float value)
        {
            GetCurrencyValue(currencyId) = value;
            OnCurrencyValueChanged?.Invoke(currencyId);
        }

        private ref float GetCurrencyValue(CurrencyId currencyId)
        {
            CurrencyData currencyData = _wallet.CurrencyData.Find(x => x.CurrencyId == currencyId);

            if (currencyData != null)
            {
                return ref currencyData.Value;
            }

            throw new Exception($"Incorrect resource type {currencyId}");
        }
    }
}