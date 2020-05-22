using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jetsun.AspNetCore.QuartzJobs
{
    #region 各版本类型定义
    public class TaskJobS1 : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath => BASE_PATH;
        public static string GetBatchFilePath() => Path.Combine(BASE_PATH, "BatchFiles");
        public static string GetJSSVCFilePath() => Path.Combine(BASE_PATH, "Lib");
        public override string BatchFilesPath => GetBatchFilePath();
        public override string JSSVCFilePath => GetJSSVCFilePath();
    }

    public class TaskJobSY : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath => BASE_PATH;
        public static string GetBatchFilePath() => Path.Combine(BASE_PATH, "BatchFiles");
        public static string GetJSSVCFilePath() => Path.Combine(BASE_PATH, "Lib");
        public override string BatchFilesPath => GetBatchFilePath();
        public override string JSSVCFilePath => GetJSSVCFilePath();
    }

    public class TaskJobBasicRC : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath => BASE_PATH;
        public static string GetBatchFilePath() => Path.Combine(BASE_PATH, "BatchFiles");
        public static string GetJSSVCFilePath() => Path.Combine(BASE_PATH, "Lib");
        public override string BatchFilesPath => GetBatchFilePath();
        public override string JSSVCFilePath => GetJSSVCFilePath();
    }

    public class TaskJobGXNL : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath => BASE_PATH;
        public static string GetBatchFilePath() => Path.Combine(BASE_PATH, "BatchFiles");
        public static string GetJSSVCFilePath() => Path.Combine(BASE_PATH, "Lib");
        public override string BatchFilesPath => GetBatchFilePath();
        public override string JSSVCFilePath => GetJSSVCFilePath();
    }

    public class TaskJobJM : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath => BASE_PATH;
        public static string GetBatchFilePath() => Path.Combine(BASE_PATH, "BatchFiles");
        public static string GetJSSVCFilePath() => Path.Combine(BASE_PATH, "Lib");
        public override string BatchFilesPath => GetBatchFilePath();
        public override string JSSVCFilePath => GetJSSVCFilePath();
    }

    public class TaskJobJMXH : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath => BASE_PATH;
        public static string GetBatchFilePath() => Path.Combine(BASE_PATH, "BatchFiles");
        public static string GetJSSVCFilePath() => Path.Combine(BASE_PATH, "Lib");
        public override string BatchFilesPath => GetBatchFilePath();
        public override string JSSVCFilePath => GetJSSVCFilePath();
    }

    public class TaskJobS12 : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath => BASE_PATH;
        public static string GetBatchFilePath() => Path.Combine(BASE_PATH, "BatchFiles");
        public static string GetJSSVCFilePath() => Path.Combine(BASE_PATH, "Lib");
        public override string BatchFilesPath => GetBatchFilePath();
        public override string JSSVCFilePath => GetJSSVCFilePath();
    }

    public class TaskJobYX : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath => BASE_PATH;
        public static string GetBatchFilePath() => Path.Combine(BASE_PATH, "BatchFiles");
        public static string GetJSSVCFilePath() => Path.Combine(BASE_PATH, "Lib");
        public override string BatchFilesPath => GetBatchFilePath();
        public override string JSSVCFilePath => GetJSSVCFilePath();
    }

    public class TaskJobYXZY : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath => BASE_PATH;
        public static string GetBatchFilePath() => Path.Combine(BASE_PATH, "BatchFiles");
        public static string GetJSSVCFilePath() => Path.Combine(BASE_PATH, "Lib");
        public override string BatchFilesPath => GetBatchFilePath();
        public override string JSSVCFilePath => GetJSSVCFilePath();
    }

    public class TaskJobZS2 : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath => BASE_PATH;
        public static string GetBatchFilePath() => Path.Combine(BASE_PATH, "BatchFiles");
        public static string GetJSSVCFilePath() => Path.Combine(BASE_PATH, "Lib");
        public override string BatchFilesPath => GetBatchFilePath();
        public override string JSSVCFilePath => GetJSSVCFilePath();
    }
    #endregion 各版本类型定义
}
