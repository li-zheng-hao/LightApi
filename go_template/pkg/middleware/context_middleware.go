package middleware

import (
	"github.com/labstack/echo/v4"
	"pkg/context"
	"pkg/global"
)

// AppContextMiddleware 自定义上下文中间件
func AppContextMiddleware(next echo.HandlerFunc) echo.HandlerFunc {
	return func(c echo.Context) error {
		cc := context.NewContext(c, global.App.Logger, &context.UserContext{})

		return next(cc)
	}
}
