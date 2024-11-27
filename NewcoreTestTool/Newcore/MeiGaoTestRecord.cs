using NewcoreTestTool.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewcoreTestTool
{
    public class MeiGaoTestRecord
    {
        public Dictionary<string, TestFileGroup> TestFileResultDict { get; set; } = new Dictionary<string, TestFileGroup>();
        public void Clear()
        {
            TestFileResultDict.Clear();
        }

        public bool Add(TestFile file, string filePath, int index)
        {
            if (file == null || string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            if (!TestFileResultDict.ContainsKey(file.group))
            {
                TestFileResultDict.Add(file.group, new TestFileGroup { group = file.group, files = new List<TestFileExt>()});
            }

            TestFileResultDict[file.group].files.Add(TestFileExt.valueOf(file, filePath, index));
            return true;
        }

        public TestFileExt findTestFileExt(TestFile file)
        {
            if (file == null)
            {
                return null;
            }

            if (TestFileResultDict.ContainsKey(file.group))
            {
                foreach (var f in TestFileResultDict[file.group].files)
                {
                    if (file.Equals(f))
                    {
                        return f;
                    }
                }
            }
            return null;
        }

        public bool Remove(TestFile file)
        {
            var testFileExt = findTestFileExt(file);
            if (testFileExt != null)
            {
                TestFileResultDict[testFileExt.group].files.Remove(testFileExt);
                return true;
            }

            return false;
        }

        public bool SetExecuteStatus(TestFile file, TestFileExt.Status status)
        {
            var testFileExt = findTestFileExt(file);
            if (testFileExt != null)
            {
                testFileExt.status = status;
                return true;
            }

            return false;
        }

        public bool SetExecuteSuccess(TestFile file) => SetExecuteStatus(file, TestFileExt.Status.SUCCESS);
        public bool SetExecuteFailed(TestFile file) => SetExecuteStatus(file, TestFileExt.Status.FAIL);

        public bool IsFileExeSuccess(TestFile file)
        {
            var testFileExt = findTestFileExt(file);
            if (testFileExt != null)
            {
                return testFileExt.status == TestFileExt.Status.SUCCESS;
            }

            return false;
        }

        public bool IsGroupFileExeSuccess(string group)
        {
            if (!TestFileResultDict.ContainsKey(group))
            {
                return false;
            }

            foreach(var item in TestFileResultDict[group].files)
            {
                if(item.status != TestFileExt.Status.SUCCESS)
                {
                    return false;
                }
            }
            return true;

        }

        public List<TestFileExt> GetAll()
        {
            List<TestFileExt> result = new List<TestFileExt>();
            foreach(var item in TestFileResultDict)
            {
                result.AddRange(item.Value.files);
            }

            return result;
        }

        public TestFileGroup GetGroup(string group)
        {
            if (TestFileResultDict.ContainsKey(group))
            {
                return TestFileResultDict[group];
            }
            return null;
        }
    }
}
