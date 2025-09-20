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

export type CreateUserDto = {
  username: string;
  email: string;
  password: string;
  role?: string; // optional, defaults to "Client"
};

export type UpdateUserDto = {
  id: number;
  username?: string;
  email?: string;
  password?: string;
  isActive?: boolean;
  role?: string;
}

export type UserStatisticsDto = {
  totalUsers: number;
  activeUsers: number;
  inactiveUsers: number;
  admins: number;
  trainers: number;
  clients: number;
};