using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SharpPlayground
{
    public class Utils
    {
        public static void Async<T>(Func<T> operation, Action<T> marshalToUI)
        {
            var dispatcher = Application.Current.Dispatcher;

            Task.Factory.StartNew(operation)
                .ContinueWith(result => dispatcher.BeginInvoke(marshalToUI, result.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}
