package global

import (
	"context"
	"fmt"
	"go.opentelemetry.io/otel"
	"go.opentelemetry.io/otel/trace"
	"go.uber.org/zap"
	"gorm.io/gorm"
	"pkg/config"
	"pkg/db"
	"pkg/opentelemetry"
)

type AppContext struct {
	Db           *gorm.DB
	Logger       *zap.Logger
	Config       *config.AppConfig
	Tracer       trace.Tracer
	otelShutdown func(context.Context) error
}

var (
	App *AppContext
)

// InitAppContext 初始化app context
func InitAppContext(env string, service string, port int) {
	App = &AppContext{}

	appConfig := config.LoadConfig(env, service, port)
	App.Config = appConfig

	logger, shutdown, err := opentelemetry.SetupOTelSDK(context.Background())
	if err != nil {
		panic(fmt.Errorf(" opentelemetry setup error: %v", err))
	}
	App.Logger = logger
	App.otelShutdown = shutdown
	dbCon, err := db.NewDbConnection(appConfig.DbConnectionString)
	if err != nil {
		panic(fmt.Errorf(" db connection error: %v", err))
	}
	App.Db = dbCon

	App.Tracer = otel.GetTracerProvider().Tracer("service_a")
}

// ShutdownAppContext 销毁app context
func ShutdownAppContext() error {
	if App.otelShutdown != nil {
		err := App.otelShutdown(context.Background())
		if err != nil {
			return err
		}
	}
	return nil
}
