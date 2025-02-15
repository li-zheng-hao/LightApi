package routes

import (
	"go-template/internal/controllers/hello"
	"go-template/internal/controllers/user"

	"github.com/gin-gonic/gin"
)

func RegisterRoutes(r *gin.Engine) {
	r.POST("/api/user/login", user.Login)
	r.GET("/api/hello/hello", hello.HelloWorld)
	r.GET("/api/user/query", user.QueryUser)
	r.GET("/api/user/export_excel", user.ExportExcel)
}
