namespace NewcoreTestTool
{
    internal class NewCoreKey
    {
        public string appKey { get; set; }

        public string appSecret { get; set; }

        public static bool Vaild(NewCoreKey key)
        {
            if (key == null || string.IsNullOrEmpty(key.appKey) || string.IsNullOrEmpty(key.appSecret))
            {
                return false;
            }
            return true;
        }
    }
}
