using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace LeastSquares.Spark
{
    public static class WebRequestExtensions
    {
        public static async Task<UnityWebRequest> SendWebRequestAsync(this UnityWebRequest request, IProgress<double> progress, CancellationToken token = default)
        {
            Debug.Log("SendingWebRequestAsync now");
            
            var tcs = new TaskCompletionSource<UnityWebRequest>();
            var webRequestAsyncOperation = request.SendWebRequest();
            if (Application.isEditor)
            {
                Debug.Log("Application.isEditor");
                var i = 0;
                while (!webRequestAsyncOperation.isDone)
                {
                    i = (i + 1) % 100;
                    progress?.Report(i / 100f);
                    //progress.Report(webRequestAsyncOperation.progress);
                    if (token.IsCancellationRequested)
                        webRequestAsyncOperation.webRequest.Abort();
                    await Task.Delay(10);
                }
            }

            Debug.Log("completed, set result");
            
            webRequestAsyncOperation.completed += _ =>
            {
                tcs.SetResult(request);
            };


            Debug.Log("Setted result");

            return await tcs.Task;
        }
    }
}