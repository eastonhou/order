using System;
using System.IO;

namespace Order.Utility
{
    class Environment
    {
        internal static readonly string ExecutionFolder = AppDomain.CurrentDomain.BaseDirectory;
        internal static readonly string DatabasePath = Path.Combine(ExecutionFolder, "data.db");
    }
}
