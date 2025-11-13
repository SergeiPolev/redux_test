using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Extensions
{
    public static class TweenExtension
    {
        public static UniTask ToUniTask(this Tween t, CancellationToken token)
        {
            if (t != null)
            {
                return t.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(token);
            }

            return UniTask.NextFrame(token);
        }

        public static Tween KillTo0(this Tween t, bool andPlay = true)
        {
            if (t != null)
            {
                t.Goto(0, andPlay);
                t.Kill();
            }

            return null;
        }
    }
}