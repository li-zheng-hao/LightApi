package core

import (
	"go-template/internal/config"
	"io"
	"log/slog"
	"os"

	"github.com/gin-gonic/gin"
	"github.com/jmoiron/sqlx"
	_ "github.com/microsoft/go-mssqldb" // SQL Server 驱动
	"gopkg.in/natefinch/lumberjack.v2"
)

type GlobalApp struct {
	Config *config.Config
	Env    string
	Logger *slog.Logger
	DB     *sqlx.DB // 添加数据库连接对象
}

var App *GlobalApp

func InitApp(env string) *GlobalApp {
	defer func() {
		if err := recover(); err != nil {
			App.Logger.Error("程序启动失败", "error", err)
			os.Exit(1)
		}
	}()
	if env == "Production" {
		gin.SetMode(gin.ReleaseMode)
	}
	App = &GlobalApp{}
	initLogger(App)
	config, err := config.InitConfig(env)
	if err != nil {
		App.Logger.Error("配置初始化失败", "error", err)
		os.Exit(1)
	}
	App.Config = config
	App.Env = env
	initDatabase(App)
	return App
}

func initLogger(app *GlobalApp) {
	// 配置 lumberjack 日志轮转
	logRotator := &lumberjack.Logger{
		Filename: "logs/app.log", // 日志文件路径
		MaxSize:  500,            // 每个日志文件的最大大小（MB）
		MaxAge:   30,             // 保留旧日志文件的最大天数
		Compress: true,           // 是否压缩旧日志文件
	}

	// 创建多重输出 writer，同时写入文件和标准输出
	multiWriter := io.MultiWriter(os.Stdout, logRotator)

	// 创建 JSON 格式的日志处理器，使用多重输出
	logger := slog.New(slog.NewJSONHandler(multiWriter, &slog.HandlerOptions{
		Level: slog.LevelDebug,
	}))

	// 设置为默认的全局 logger
	slog.SetDefault(logger)
	app.Logger = logger
}

func initDatabase(app *GlobalApp) {
	app.Logger.Debug("初始化数据库...")
	var connStr string
	// 从配置中获取数据库连接字符串
	if app.Config.SqlServerUrlEnv != "" {
		// 从系统环境变量中获取数据库连接字符串
		connStr = os.Getenv(app.Config.SqlServerUrlEnv)
		app.Config.SqlServerUrl = connStr

	} else {

		connStr = app.Config.SqlServerUrl
	}
	if connStr == "" {
		app.Logger.Error("Database connection string is empty or not found in environment variables")
		os.Exit(1)
	}
	// 连接数据库
	db := sqlx.MustConnect("mssql", connStr)
	db.SetMaxOpenConns(200)
	db.SetMaxIdleConns(1)

	// 测试数据库连接
	err := db.Ping()
	if err != nil {
		app.Logger.Error("Failed to ping database", "error", err)
		os.Exit(1)
	}

	app.DB = db // 将数据库连接对象存储到 GlobalApp 中
	app.Logger.Debug("数据库初始化成功")
}
