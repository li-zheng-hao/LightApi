package middleware

import (
	"errors"

	"github.com/labstack/echo/v4"
)

// 自定义用户信息初始化中间件
func UserContextMiddleware(next echo.HandlerFunc) echo.HandlerFunc {
	return func(c echo.Context) error {
		// 从Cookie读取 auth_token
		authToken, err := c.Cookie("auth_token")

		if err != nil {
			return err
		}
		// 验证auth_token的有效性
		if len(authToken.Value) == 0 {
			return errors.New("auth_token is invalid")
		}

		// 设置用户信息到Context
		return next(c)
	}
}
