package config

import "errors"

type Config struct {
	ServerPort      string
	SqlServerUrl    string
	SqlServerUrlEnv string
	LogLevel        string
	SecretKey       string // 对称加密密钥

}

var ConfigInstance *Config

func InitConfig(env string) (*Config, error) {
	if env == "Development" {
		ConfigInstance = getDevelopmentConfig()
	} else if env == "Testing" {
		ConfigInstance = getTestingConfig()
	} else if env == "Production" {
		ConfigInstance = getProductionConfig()
	} else {
		return nil, errors.New("环境变量错误: " + env)
	}
	return ConfigInstance, nil
}

func getDevelopmentConfig() *Config {
	return &Config{
		ServerPort:      "8080",
		SqlServerUrlEnv: "APP_GOLANG_MSSQL_CONNECTION_STRING",
		LogLevel:        "debug",
		SecretKey:       "1234567891234567",
	}
}
func getTestingConfig() *Config {
	return &Config{
		ServerPort:   "8080",
		SqlServerUrl: "Server=localhost\\SQLEXPRESS;Database=Test;Trusted_Connection=True;",
		LogLevel:     "1234567891234567",
	}
}

func getProductionConfig() *Config {
	return &Config{
		ServerPort:   "8080",
		SqlServerUrl: "Server=localhost\\SQLEXPRESS;Database=Test;Trusted_Connection=True;",
		LogLevel:     "1234567891234567",
	}
}
