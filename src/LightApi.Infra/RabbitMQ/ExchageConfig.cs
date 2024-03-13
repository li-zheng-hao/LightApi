namespace Adnc.Infra.EventBus.RabbitMq
{
    public class ExchageConfig
    {
        public string Name { get; set; } = string.Empty;
        public ExchangeType Type { get; set; } = default!;
        public string DeadExchangeName { get; set; } = string.Empty;

        /// <summary>
        /// 是否自动创建
        /// </summary>
        public bool AutoCreate { get; set; } = false;
    }
}