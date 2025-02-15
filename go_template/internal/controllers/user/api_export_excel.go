package user

import (
	"go-template/internal/utils"
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/xuri/excelize/v2"
)

// @Summary 导出excel
// @Description 导出excel
// @Accept json
// @Produce json
// @Success 200 {object} PartCost
// @Router /api/user/export_excel [get]
func ExportExcel(c *gin.Context) {
	// 创建一个新的 Excel 文件
	f := excelize.NewFile()
	defer f.Close() // 确保文件关闭

	// 创建一个工作表
	sheetName := "Sheet1"
	index, err := f.NewSheet(sheetName)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": "无法创建工作表"})
		return
	}

	// 设置表头
	headers := []string{"ID", "名称", "成本"}
	for col, header := range headers {
		cell, _ := excelize.CoordinatesToCellName(col+1, 1) // 第一行是表头
		f.SetCellValue(sheetName, cell, header)
	}

	// 填充数据
	data := []struct {
		ID   int
		Name string
		Cost float64
	}{
		{1, "零件A", 100.50},
		{2, "零件B", 200.75},
		{3, "零件C", 300.00},
	}
	for row, item := range data {
		cell, _ := excelize.CoordinatesToCellName(1, row+2) // 第1列
		f.SetCellValue(sheetName, cell, item.ID)
		cell, _ = excelize.CoordinatesToCellName(2, row+2) // 第2列
		f.SetCellValue(sheetName, cell, item.Name)
		cell, _ = excelize.CoordinatesToCellName(3, row+2) // 第3列
		f.SetCellValue(sheetName, cell, item.Cost)
	}

	// 设置活动工作表
	f.SetActiveSheet(index)

	// 将 Excel 文件写入 HTTP 响应
	c.Header("Content-Type", "application/octet-stream")
	c.Header("Content-Disposition", "attachment; filename=export.xlsx")
	c.Header("Content-Transfer-Encoding", "binary")

	// 将文件写入响应
	if err := f.Write(c.Writer); err != nil {
		utils.Error(c, http.StatusInternalServerError, "无法写入Excel文件")
		return
	}
}
