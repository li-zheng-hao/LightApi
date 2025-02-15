package hello

import (
	"net/http"

	"github.com/gin-gonic/gin"
)

// @Summary      Hello World
// @Tags         hello
// @Router       /api/hello/hello [get]
func HelloWorld(g *gin.Context) {
	// 实现获取用户的逻辑
	g.JSON(http.StatusOK, gin.H{
		"message": "Hello World",
	})
}
