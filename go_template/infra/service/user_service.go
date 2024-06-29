package service

import (
	"go_template/infra/model"

	"github.com/labstack/echo/v4"
)

// ID       int    `json:"id"`
// 	Username string `json:"username"`
// 	Password string `json:"password"`
// 	Email    string `json:"email"`
// 	Phone    string `json:"phone"`
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