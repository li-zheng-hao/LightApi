package service_a

import (
	"context"
	"flag"
	"github.com/labstack/echo/v4"
	"github.com/labstack/echo/v4/middleware"
	"github.com/labstack/gommon/log"
	"io"
	"net/http"
	"os"
	"os/signal"
	"pkg/config"
	"pkg/route"
	"time"
)

var (
	env string
)

func init() {
	flag.StringVar(&env, "e", "Production", "系统环境")

}

// @title Swagger Example API
// @version 1.0
// @description This is a sample server Petstore server.
// @termsOfService http://swagger.io/terms/

// @contact.name API Support
// @contact.url http://www.swagger.io/support
// @contact.email support@swagger.io
// @schemes http https
// @license.name Apache 2.0
// @license.url http://www.apache.org/licenses/LICENSE-2.0.html

// @host localhost:8888
// @BasePath
func main() {
	e := echo.New()

	flag.Parse()

	e.Logger.Info("启动系统环境:", env)

	e.Logger.SetLevel(log.DEBUG)

	if env != "Development" {
		// 不需要控制台
		e.Logger.SetOutput(io.Discard)
	}

	appConfig := config.LoadConfig(env)

	e.Logger.Info("服务启动:", appConfig.ServiceName)

	e.Use(middleware.CORS())

	route.RouteHandlers(e)
	ctx, stop := signal.NotifyContext(context.Background(), os.Interrupt)
	defer stop()
	// Start server
	go func() {
		if err := e.Start(":8888"); err != nil && err != http.ErrServerClosed {
			e.Logger.Error("启动服务失败:", err)
			e.Logger.Fatal("停止服务")

		}
	}()

	// Wait for interrupt signal to gracefully shutdown the server with a timeout of 10 seconds.
	<-ctx.Done()
	ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	defer cancel()
	if err := e.Shutdown(ctx); err != nil {
		e.Logger.Fatal(err)
	}
	e.Logger.Info("停止服务")

}
