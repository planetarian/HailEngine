#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Hail.Core;
#endregion

namespace Hail
{
#if WINDOWS || LINUX || WINRT
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
#if WINRT
        static void Main()
        {
            var factory = new MonoGame.Framework.GameFrameworkViewSource<HailGame>();
            Windows.ApplicationModel.Core.CoreApplication.Run(factory);
        }
#else
        [STAThread]
        static void Main()
        {
            using (var game = new HailGame())
                game.Run();
        }
#endif

    }
#endif
}
