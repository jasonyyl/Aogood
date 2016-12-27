
namespace Aogood.SHLib
{
    public class CSystemParameters
    {
        public const int NetMessageRange = 1000;
    }
    public enum EModuleType : byte
    {
        /// <summary>
        ///起始
        /// </summary>
        NONE = 0,
        /// <summary>
        /// 心跳包系统
        /// </summary>
        HEART_SYSTEM,
        /// <summary>
        /// 登录系统
        /// </summary>
        LOGIN_SYSTEM,
        /// <summary>
        /// 聊天系统
        /// </summary>
        CHAT_SYSTEM,
    }
}
