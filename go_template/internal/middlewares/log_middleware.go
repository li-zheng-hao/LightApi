package middlewares

import (
	"fmt"
	"go-template/internal/core"
	"time"

	"github.com/gin-gonic/gin"
	"github.com/google/uuid"
)

func LogMiddleware() gin.HandlerFunc {
	return func(c *gin.Context) {
		start := time.Now()
		// 生成一个guid
		guid := uuid.New().String()
		c.Set("request_id", guid)
		userId := c.GetInt("userId")
		userName := c.GetString("userName")
		c.Next()
		end := time.Now()
		latency := end.Sub(start)
		core.App.Logger.Info(fmt.Sprintf("请求: %s %s 耗时: %s 用户ID: %d 用户名: %s", c.Request.Method, c.Request.URL.Path, latency, userId, userName), "request_id", guid)
	}
}
