package context

type UserContext struct {
	Id    int32
	Name  string
	Roles []string
}

func (receiver *UserContext) IsLogin() bool {
	if receiver.Id == 0 {
		return false
	}
	return true
}
