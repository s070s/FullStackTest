export type UserDto = {
  id: number;
  username: string;
  email: string;
  createdUtc: string; // ISO string from server
  createdUtcFormatted?: string;
  isActive: boolean;
  role: string;
  profilePhotoUrl?: string;
  trainerProfile?: any;
  clientProfile?: any;
};

export type RegisterUserDto = {
  username: string;
  email: string;
  password: string;
  role?: string; // optional, defaults to "Client"
};

export type LoginUserDto = {
  username: string;
  password: string;
};