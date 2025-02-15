package middlewares

import (
	"encoding/json"
	"errors"
	"go-template/internal/config"
	"go-template/internal/utils"
	"net/http"
	"strings"
	"time"

	"github.com/gin-gonic/gin"
)

const (
	cookieName          = "sys.auth"
	cookieMaxAge        = 7 * 24 * 3600 // 7天
	cookieRefreshBefore = 3 * 24 * 3600 // 3天
)

type CookiePayload struct {
	UserId     int    `json:"userId"`
	UserName   string `json:"userName"`
	ExpireTime int64  `json:"expireTime"`
}

var ignorePath = []string{"/login", "/swagger/", "/api/hello"}

func AuthMiddleware() gin.HandlerFunc {
	return func(c *gin.Context) {
		var allowAnonymous bool = false
		for _, path := range ignorePath {
			if strings.HasPrefix(c.Request.URL.Path, path) {
				allowAnonymous = true
				break
			}
		}
		cookie, err := c.Cookie(cookieName)
		if err != nil && !allowAnonymous {
			c.AbortWithStatus(http.StatusUnauthorized)
			return
		}
		if err == nil {
			// 解密并验证cookie
			payload, err := decryptAndValidateCookie(cookie)
			if err != nil && !allowAnonymous {
				c.AbortWithStatus(http.StatusUnauthorized)
				return
			}
			if payload != nil {
				// 将用户信息存储到上下文中
				c.Set("userId", payload.UserId)
				c.Set("userName", payload.UserName)
				// 检查是否需要刷新cookie
				if time.Now().Unix() > payload.ExpireTime-int64(cookieRefreshBefore) {
					// 创建新的cookie
					newPayload := CookiePayload{
						UserId:     payload.UserId,
						ExpireTime: time.Now().Unix() + int64(cookieMaxAge),
					}

					// 加密cookie数据
					newCookie, err := encryptCookie(newPayload)
					if err == nil {
						c.SetCookie(cookieName,
							newCookie,
							cookieMaxAge,
							"/",
							"",
							true,
							true,
						)
					}
				}
			}
		}

		c.Next()
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

// 解密并验证cookie
func decryptAndValidateCookie(encryptedCookie string) (*CookiePayload, error) {
	decrypted, err := utils.AesDecrypt(encryptedCookie, config.ConfigInstance.SecretKey)
	if err != nil {
		return nil, err
	}

	// 解析JSON
	var payload CookiePayload
	if err := json.Unmarshal([]byte(decrypted), &payload); err != nil {
		return nil, err
	}

	// 验证是否过期
	if time.Now().Unix() > payload.ExpireTime {
		return nil, errors.New("登录已过期")
	}

	return &payload, nil
}
