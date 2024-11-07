import type { PageRequest } from '@/api/base/page';

export interface FetchUserRequest extends PageRequest {
  userName: string | null | undefined;
}

/**
 *
 */
export interface User {
  // 编号
  id: number;
  // 名称
  name: string;
  age: number;
  address: string;
  city: string;
  province: string;
  email: string;
  phone: string;
  regin: string;
  url: string;
  date: string;
}
