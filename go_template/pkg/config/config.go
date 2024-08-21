package config

type AppConfig struct {
	Port               int    // 端口
	ServiceName        string // 服务名称
	DbConnectionString string // 数据库连接字符串
	EnvironmentName    string // 环境名称
}

const (
	Development = "Development"
	Testing     = "Testing"
	Preview     = "Preview"
	Production  = "Production"
)

func LoadConfig(environmentName string, serviceName string, port int) *AppConfig {
	var appConfig = &AppConfig{}
	switch environmentName {
	case Development:
		appConfig = loadDevelopmentConfig()
	case Testing:
		appConfig = loadTestingConfig()
	case Preview:
		appConfig = loadPreviewConfig()
	case Production:
		appConfig = loadProductionConfig()
	default:
		appConfig = loadProductionConfig()
	}
	appConfig.EnvironmentName = environmentName
	appConfig.ServiceName = serviceName
	appConfig.Port = port
	return appConfig
}

func loadProductionConfig() *AppConfig {
	return &AppConfig{
		Port:               8888,
		DbConnectionString: "postgres://username:password@localhost:5432/database_name",
	}
}

func loadPreviewConfig() *AppConfig {
	return &AppConfig{
		Port:               8888,
		DbConnectionString: "postgres://username:password@localhost:5432/database_name",
	}
}

func loadTestingConfig() *AppConfig {
	return &AppConfig{
		Port:               8888,
		DbConnectionString: "postgres://username:password@localhost:5432/database_name",
	}
}

func loadDevelopmentConfig() *AppConfig {
	return &AppConfig{
		Port:               8888,
		DbConnectionString: "sqlserver://sa:BJMY%21QAZ2wsx%23EDC@172.10.4.138?database=ent_dev&TrustServerCertificate=true",
	}
}
