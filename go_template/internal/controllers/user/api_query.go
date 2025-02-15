package user

import (
	"go-template/internal/core"
	"go-template/internal/utils"
	"net/http"
	"time"

	"github.com/gin-gonic/gin"
)

type PartCost struct {
	Id         int       `json:"id" db:"Id"`
	IsDeleted  bool      `json:"is_deleted" db:"IsDeleted"`
	CreateTime time.Time `json:"create_time" db:"CreateTime"`
	UpdateTime time.Time `json:"update_time" db:"UpdateTime"`
}

// @Summary 查询用户
// @Description 查询用户
// @Accept json
// @Produce json
// @Success 200 {object} PartCost
// @Router /api/user/query [get]
func QueryUser(c *gin.Context) {
	var partCost PartCost
	err := core.App.DB.Unsafe().Get(&partCost, "select top 1 * from PartCost")
	if err != nil {
		utils.Error(c, http.StatusInternalServerError, err.Error())
		return
	}

	utils.Success(c, partCost)
}
