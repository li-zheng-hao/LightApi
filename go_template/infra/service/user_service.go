package service

import (
	"go_template/infra/model"

	"github.com/labstack/echo/v4"
)

// @Summary  获取用户
// @Description  获取用户信息
// @Tags  用户
// @Accept  json
// @Produce  json
// @Success  200  {object}  model.SysUser
// @Router  /api/user/get [get]
func GetUser(c echo.Context) error {
	// 调用数据库或其他数据源获取用户信息
    user:=&model.SysUser{
        Id:  1,
        Username: "admin",
        Password: "admin",
        Email:    "admin@example.com",
        Phone:    "1234567890",
    }
    return c.JSON(200, user)
}

// @Summary  添加用户
// @Description  添加用户信息
// @Tags  用户
// @Accept  json
// @Produce  json
// @Param   user  body  model.SysUser  true  "用户信息"
// @Success  200  {object}  model.SysUser
// @Router  /api/user/add [post]
func AddUser(c echo.Context) error {
    var user model.SysUser
    // 调用数据库或其他数据源添加用户信息
    c.Bind(&user)
    c.Logger().Info("AddUser", user)
    // 返回成功信息
    return c.JSON(200, user)
}

