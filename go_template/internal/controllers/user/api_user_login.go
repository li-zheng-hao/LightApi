package user

import (
	"encoding/json"
	"go-template/internal/config"
	"go-template/internal/utils"
	"net/http"
	"time"

	"github.com/gin-gonic/gin"
)

// LoginRequest 登录请求参数
// @Description 登录请求参数
type LoginRequest struct {
	// 用户名
	Username string `json:"username" binding:"required"`
	// 密码
	Password string `json:"password" binding:"required"`
}
type CookiePayload struct {
	UserId     int    `json:"userId"`
	UserName   string `json:"userName"`
	ExpireTime int64  `json:"expireTime"`
}

// @Summary 登录
// @Description 登录
// @Accept json
// @Produce json
// @Param loginRequest body LoginRequest true "登录请求"
// @Success 200 {string} string "登录成功"
// @Failure 400 {string} string "登录失败"
// @Router       /api/user/login [post]
func Login(g *gin.Context) {
	var request LoginRequest
	if err := g.ShouldBindJSON(&request); err != nil {
		g.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
	}
	if request.Username == "admin" && request.Password == "123456" {
		payload := CookiePayload{
			UserId:     1,
			UserName:   "admin",
			ExpireTime: time.Now().Unix() + int64(7*24*3600),
		}

		cookie, err := encryptCookie(payload)
		if err != nil {
			// 处理错误
			return
		}

		g.SetCookie("sys.auth",
			cookie,
			7*24*3600,
			"/",
			"",
			true,
			true,
		)
		g.JSON(http.StatusOK, gin.H{"message": "登录成功"})
	} else {
		g.JSON(http.StatusUnauthorized, gin.H{"error": "用户名或密码错误"})
	}
}

// 加密cookie数据
func encryptCookie(payload CookiePayload) (string, error) {
	data, err := json.Marshal(payload)
	if err != nil {
		return "", err
	}
	encrypted, err := utils.AesEncrypt(string(data), config.ConfigInstance.SecretKey)
	if err != nil {
		return "", err
	}
	return encrypted, nil
}
