using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Services.Core
{
    public class CancellationAsyncService : IService
    {
        public CancellationToken Token { get; private set; }

        public void Initialize(MonoBehaviour monoBehaviour)
        {
            Token = monoBehaviour.GetCancellationTokenOnDestroy();
        }
    }
}