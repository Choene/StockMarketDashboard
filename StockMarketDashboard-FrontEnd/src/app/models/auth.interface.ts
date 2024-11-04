export interface LoginRequest {
  username: string;
  password: string;
}

export interface UserDto {
  username: string;
  role: string;
  token: string;
}
