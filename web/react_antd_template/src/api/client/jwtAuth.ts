// import axios, { AxiosError } from 'axios'
// import { applyAuthTokenInterceptor, type IAuthTokens, type TokenRefreshRequest } from 'axios-jwt'
// import { ApiClient, apiClient } from './apiClient'
// import router from '@/router'
//
// const requestRefresh: TokenRefreshRequest = async (
//   refreshToken: string
// ): Promise<IAuthTokens | string> => {
//   // Important! Do NOT use the axios instance that you supplied to applyAuthTokenInterceptor (in our case 'axiosInstance')
//   // because this will result in an infinite loop when trying to refresh the token.
//   // Use the global axios client or a different instance
//
//   const response = await axios.post(`${apiClient.baseConfig.baseURL}/user/refresh-token`, {
//     token: refreshToken
//   })
//   .catch((error) => {
//     return error.response
//   })
//
//   if(response.status !== 200) {
//     window.$message.error('登录已过期，请重新登录')
//     router.push('/login')
//   }
//
//   // If your backend supports rotating refresh tokens, you may also choose to return an object containing both tokens:
//   return {
//     accessToken: response.data.data.accessToken,
//     refreshToken: response.data.data.refreshToken
//   }
// }
//
// /**
//  * 使用jwt
//  * @param client
//  */
// export function useJwt(client: ApiClient)  {
//   // 3. Add interceptor to your axios instance
//   applyAuthTokenInterceptor(client.axiosInstance, { requestRefresh })
// }
