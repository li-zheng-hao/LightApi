package model

type SysUser struct {
	// 用户编号
	Id int `json:"id"`
	// 用户名
	Username string `json:"username"`
	Password string `json:"password"`
	Email    string `json:"email"`
	Phone    string `json:"phone"`
}
