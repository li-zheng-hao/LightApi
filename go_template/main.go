package main

import (
	"context"
	"flag"
	"go_template/infra/config"
	"go_template/infra/route"
	"net/http"
	"os"
	"os/signal"
	"time"

	"github.com/labstack/echo/v4"
	"github.com/labstack/gommon/log"
)

func main() {
	e := echo.New()

	e.Logger.SetLevel(log.DEBUG)

	env := flag.String("env", "Production", "系统环境")

	e.Logger.Info("启动系统环境:", *env)

	appConfig:=config.LoadConfig(*env)

	e.Logger.Info("服务启动:", appConfig.ServiceName)

	route.RouteHandlers(e)

	ctx, stop := signal.NotifyContext(context.Background(), os.Interrupt)
	defer stop()
	// Start server
	go func() {
		if err := e.Start(":8888"); err != nil && err != http.ErrServerClosed {
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
	
}