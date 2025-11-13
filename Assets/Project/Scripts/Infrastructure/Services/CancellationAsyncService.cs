using System.Threading;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace Infrastructure
{
    public class CancellationAsyncService : IService
    {
        private CancellationToken _token;
        
        public CancellationToken Token => _token;
        
        public void Initialize(MonoBehaviour monoBehaviour)
        {
            _token = monoBehaviour.GetCancellationTokenOnDestroy();
        }
    }
}