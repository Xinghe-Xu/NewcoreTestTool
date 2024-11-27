namespace NewcoreTestTool
{
    internal class NewCoreToken
    {
        /// <summary>
        /// 鉴权字符串
        /// </summary>
        public string accessToken { get; set; }
        /// <summary>
        /// 鉴权字符串有效期，默认2小时
        /// </summary>
        public long accessTokenExpireIn { get; set; }
        /// <summary>
        /// 刷新鉴权字符串，鉴权字符串到期时用于获取新的鉴权
        /// </summary>
        public string refreshToken { get; set; }
        /// <summary>
        /// 刷新鉴权字符串有效期，默认1个月
        /// </summary>
        public long refreshTokenExpireIn { get; set; }
    }
}
