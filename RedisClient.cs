using StackExchange.Redis;
using System;
using System.Configuration;

namespace DataAccess {
    /// <summary>
    /// Redis 客户端
    /// </summary>
    public class RedisClient {

        /// <summary>
        /// lock
        /// </summary>
        private static readonly object lockObject = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        private RedisClient() { }

        /// <summary>
        /// Redis 连接配置
        /// </summary>
        private static Lazy<ConfigurationOptions> configOptions = new Lazy<ConfigurationOptions>(() => {
            var configOptions = new ConfigurationOptions();
            configOptions.EndPoints.Add(ConfigurationManager.ConnectionStrings[ApplicationKeys.DbRedisConnect].ToString());
            configOptions.ClientName = "KidoConnection";
            configOptions.ConnectTimeout = 3000; //5S
            configOptions.SyncTimeout = 6000;
            return configOptions;
        });

        private static IConnectionMultiplexer conn = null;
        /// <summary>
        /// 建立Redis连接
        /// </summary>
        public static IConnectionMultiplexer Conn {
            get {
                if (conn == null || !conn.IsConnected) {
                    lock (lockObject) {
                        if (conn == null) {
                            conn = ConnectionMultiplexer.Connect(configOptions.Value);
                        }
                    }
                }
                return conn;
            }
        }
    }
}
