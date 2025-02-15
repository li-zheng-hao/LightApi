package main

import (
	"embed"
	"io/fs"
	"net/http"
	"os"

	_ "go-template/docs"
	"go-template/internal/core"
	"go-template/internal/middlewares"
	"go-template/internal/migrations"
	"go-template/internal/routes"

	"github.com/gin-gonic/gin"
	swaggerfiles "github.com/swaggo/files"
	ginSwagger "github.com/swaggo/gin-swagger"
)

var (
	//go:embed static/*
	StaticDir embed.FS
)

func main() {
	args := os.Args
	// 环境参数
	env := "Development"
	if len(args) > 1 {
		env = args[1]
	}
	core.InitApp(env)

	migrations.MigrateDatabase()

	r := gin.Default()
	// 使用异常中间件
	r.Use(middlewares.ExceptionMiddleware())
	// 使用认证中间件
	r.Use(middlewares.AuthMiddleware())
	// 使用日志中间件
	r.Use(middlewares.LogMiddleware())
	routes.RegisterRoutes(r)

	if core.App.Env != "Production" {
		r.GET("/swagger/*any", ginSwagger.WrapHandler(swaggerfiles.Handler))
	}

	// 嵌入的 embed 就是一个文件系统，获取 web 目录下的资源文件
	staticFp, _ := fs.Sub(StaticDir, "static")
	// 所有请求先匹配 api 路由，如果没匹配到就当作静态资源文件处理
	r.NoRoute(gin.WrapH(http.FileServer(http.FS(staticFp))))
	r.Run(":8080")
}
