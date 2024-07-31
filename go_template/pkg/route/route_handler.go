package route

import (
	"github.com/labstack/echo/v4"
	echoSwagger "github.com/swaggo/echo-swagger"
	"pkg/service"
)

func RouteHandlers(e *echo.Echo) {

	e.GET("/swagger/*", echoSwagger.WrapHandler)

	// 添加路由处理函数
	userGroup := e.Group("/api/user")
	userGroup.GET("/get", service.GetUser)
	userGroup.POST("/add", service.AddUser)
}
