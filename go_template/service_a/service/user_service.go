package service

import (
	"fmt"
	"github.com/labstack/echo/v4"
	"pkg/context"
	"pkg/model"
)

// GetUser
// @Summary  获取用户
// @Description  获取用户信息
// @Tags  用户
// @Accept  json
// @Produce  json
// @Success  200  {object}  model.SysUser
// @Router  /api/user/get [get]
func GetUser(c echo.Context) error {
	reqContext := c.(*context.RequestContext)
	span := reqContext.StartWithNewTracer(fmt.Sprintf("[%v]%v", c.Request().Method, c.Request().RequestURI))
	defer (*span).End()
	// 调用数据库或其他数据源获取用户信息
	user := &model.SysUser{
		Id:       1,
		Username: "admin",
		Password: "admin",
		Email:    "admin@example.com",
		Phone:    "1234567890",
	}
	reqContext.ContextLogger.Info(fmt.Sprintf("查询用户结果完成"))

	return c.JSON(200, user)
}
