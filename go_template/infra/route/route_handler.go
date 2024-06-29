package route

import (
	"go_template/infra/service"

	"github.com/labstack/echo/v4"
)

func RouteHandlers(e *echo.Echo) {
	// 添加路由处理函数
	userGroup:=e.Group("/api/user")
	userGroup.GET("/:id", service.GetUser)
}