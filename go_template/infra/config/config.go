package config

type AppConfig struct {
	Port               uint   // 端口
	ServiceName        string // 服务名称
	DbConnectionString string // 数据库连接字符串
}

const (
	Development = "Development"
	Testing     = "Testing"
	Preview     = "Preview"
	Production  = "Production"
)

func LoadConfig(env string) *AppConfig {
	switch env {
	case Development:
		return loadDevelopmentConfig()
	case Testing:
		return loadTestingConfig()
	case Preview:
		return loadPreviewConfig()
	case Production:
		return loadProductionConfig()
	default:
		return loadProductionConfig()
	}
}

func loadProductionConfig() *AppConfig {
	return &AppConfig{
		Port:               8888,
		ServiceName:        "go_template",
		DbConnectionString: "postgres://username:password@localhost:5432/database_name",
	}
}

func loadPreviewConfig() *AppConfig {
	return &AppConfig{
		Port:               8888,
		ServiceName:        "go_template",
		DbConnectionString: "postgres://username:password@localhost:5432/database_name",
	}
}

func loadTestingConfig() *AppConfig {
	return &AppConfig{
		ServiceName:        "go_template",
		Port:               8888,
		DbConnectionString: "postgres://username:password@localhost:5432/database_name",
	}
}

func loadDevelopmentConfig() *AppConfig {
	return &AppConfig{
		Port:               8888,
		DbConnectionString: "postgres://username:password@localhost:5432/database_name",
	}
}