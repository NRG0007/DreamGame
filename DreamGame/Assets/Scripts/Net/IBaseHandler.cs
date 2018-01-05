using Net.Framework.Server;

namespace Net.Client
{
    /// <summary>
    /// 处理基接口
    /// </summary>
    public interface IBaseHandler 
    {
        /// <summary>
        /// 接收服务器消息
        /// </summary>
        /// <param name="message"></param>
        void MessageReceive(SocketModel message);
    }
}