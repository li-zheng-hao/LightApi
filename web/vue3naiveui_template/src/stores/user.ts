import { apiClient } from '@/api/client/apiClient'
import { clearAuthTokens, getAccessToken, setAuthTokens } from 'axios-jwt'
import { defineStore } from 'pinia'

export interface IUserState {
  username: string
  avatar: string
  permissions: string[]
}

export const useUserStore = defineStore({
  id: 'app-user',
  state: (): IUserState => ({
    username: '',
    avatar: '',
    permissions: [] as string[]
  }),
  getters: {
    async getToken(): Promise<string | undefined> {
      return await getAccessToken()
    },
    getAvatar(): string {
      return this.avatar
    },
    getNickname(): string {
      return this.username
    },
    getPermissions(): string[] {
      return this.permissions
    }
  },
  actions: {
    async login(username: string, password: string): Promise<void> {
      const userInfo = await apiClient.request<any>({
        url: '/user/login',
        method: 'post',
        data: {
          username,
          password
        }
      })
      // save tokens to storage
      setAuthTokens({
        accessToken: userInfo.accessToken,
        refreshToken: userInfo.refreshToken
      })
      this.username = userInfo.username
      this.avatar = userInfo.avator
      this.permissions = userInfo.permissions
    },
    /**
     * 刷新当前登录用户信息，一般用于登录后每次路由跳转都刷新权限，后端需要加缓存
     */
    async refreshUserInfo(): Promise<void> {
        const userInfo = await apiClient.request<any>({
            url: '/user/info',
            method: 'get'
        })
        this.setAvatar(userInfo.avator)
        this.setNickname(userInfo.username)
        this.setPermissions(userInfo.permissions)
    },
    setNickname(nickname: string): void {
        this.username = nickname
    },
    setAvatar(avatar: string): void {
        this.avatar = avatar
    },
    setPermissions(permissions: string[]): void {
        this.permissions = permissions
    },
    logOut(): void {
        window.localStorage.clear()
    }

  },
  persist:{}
})
