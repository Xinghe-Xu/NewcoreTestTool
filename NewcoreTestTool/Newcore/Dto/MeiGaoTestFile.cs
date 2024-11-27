using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewcoreTestTool.Dto
{
    public class TestFileUtil
    {
        public static string GROUPNAME_T1 = @"T1";
        public static string GROUPNAME_AGING = @"老化";
        public static string GROUPNAME_T2 = @"T2";
        private static List<string> GROUPNAME = new List<string>{ GROUPNAME_T1, GROUPNAME_AGING, GROUPNAME_T2 };
        private static string[] EXECUTE = { @"未执行", @"执行成功", @"执行失败" };
        private static string[] UPLOAD = { @"未上传", @"上传MES成功", @"上传MES失败" };
        public static string FILENOTEXIST = @"文件不存在";
        public static string FILEEXIST = @"文件存在";

        public static string GetExecuteDesc(TestFileExt.Status stauts) => EXECUTE[(int)stauts];
        public static string GetUploadDesc(TestFileExt.Status stauts) => UPLOAD[(int)stauts];

        // 优先级排序，需求按照@"T1",@"老化", @"T2" ，排序，如果不在这之内，暂时按照3优先级
        public static int GetGroupIndex(string groupName)
        {
            int index = GROUPNAME.IndexOf(groupName);
            return index == -1 ? 3: index;
        }
    }

    public class MeiGaoTestFile
    {
        public string sn { get; set; }
        public object itemBasicDTO { get; set; }
        public TestFile[] codes { get; set; }
    }


    public class TestFile
    {
        public string name { get; set; }
        public string url { get; set; }
        public string group { get; set; }
        public string sn { get; set; }

        public bool Equals<T>(T other) where T : TestFile
        {
            return string.Equals(name, other.name) && string.Equals(url, other.url) && string.Equals(group, other.group) && string.Equals(sn, other.sn);
        }
    }

    public class TestFileExt : TestFile
    {
        public int index { get; set; }
        public enum Status { INIT = 0, SUCCESS = 1, FAIL = 2 }
        public Status status { get; set; }

        public Status upload { get; set; }

        public string filePath { get; set; }

        public static TestFileExt valueOf(TestFile tf, string filePath, int index)
        {
            return new TestFileExt
            {
                index = index,
                name = tf.name,
                url = tf.url,
                group = tf.group,
                sn = tf.sn,
                status = Status.INIT,
                upload = Status.INIT,
                filePath = filePath
            };
        }

        public bool TestPass() => status == Status.SUCCESS;
    }

    public class TestFileGroup
    {
        public string group { get; set; }

        public string id { get; set; } = string.Empty;
        public List<TestFileExt> files { get; set;}
    }

}
