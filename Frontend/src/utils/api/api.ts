import type { UserDto, RegisterUserDto, LoginUserDto, CreateUserDto, UpdateUserDto, TokenPairDto } from "../data/userdtos";
import type { ClientUpdateDto } from "../data/clientdtos";
import type { TrainerUpdateDto } from "../data/trainerdtos";
// Base URL of your API - uses environment variable or falls back to production URL
export const API_BASE_URL = import.meta.env.VITE_API_DEV_URL || import.meta.env.VITE_API_URL;



//#region Authentication API Functions
// Register a new user
export async function registerUser(data: RegisterUserDto): Promise<UserDto> {
  const response = await fetch(`${API_BASE_URL}/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText);
  }
  return response.json();
}

// Authenticate a user (login and get JWT token)
export async function authenticateUser(data: LoginUserDto): Promise<TokenPairDto> {
  const response = await fetch(`${API_BASE_URL}/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText);
  }
  return response.json();
}

export async function refreshAuthToken(refreshToken: string): Promise<TokenPairDto> {
  const response = await fetch(`${API_BASE_URL}/refresh`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ refreshToken }),
  });
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Unable to refresh token");
  }
  return response.json();
}
// Used to fetch data from a protected api endpoint using JWT token after login
export async function fetchWithAuth(
  endpoint: string,
  token: string,
  options: RequestInit = {}
) {
  const isFormData = options.body instanceof FormData;

  // Convert headers to plain object if needed
  let baseHeaders: Record<string, string> = {};
  if (options.headers instanceof Headers) {
    options.headers.forEach((value, key) => {
      baseHeaders[key] = value;
    });
  } else if (typeof options.headers === "object" && options.headers !== null) {
    baseHeaders = { ...options.headers as Record<string, string> };
  }

  baseHeaders["Authorization"] = `Bearer ${token}`;
  if (!isFormData) {
    baseHeaders["Content-Type"] = "application/json";
  }

  const headers = new Headers(baseHeaders);
  return fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    headers,
  });
}
//#endregion



//#region Admin Only User Management API Functions
// Admin:Fetch all users
export async function adminFetchAllUsers(
  token: string,
  options?: {
    page?: number;
    pageSize?: number;
    sortBy?: string;
    sortOrder?: "asc" | "desc";
  }
): Promise<{ users: UserDto[]; total: number }> {
  const params = new URLSearchParams();
  if (options?.page) params.append("page", options.page.toString());
  if (options?.pageSize) params.append("pageSize", options.pageSize.toString());
  if (options?.sortBy) params.append("sortBy", options.sortBy);
  if (options?.sortOrder) params.append("sortOrder", options.sortOrder);

  const endpoint = `/users${params.toString() ? "?" + params.toString() : ""}`;
  const response = await fetchWithAuth(endpoint, token);
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText);
  }
  return response.json();
}

//Admin:Fetch all User Statistics
export async function adminFetchUserStatistics(token: string): Promise<any> {
  const response = await fetchWithAuth("/users/statistics", token);
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText);
  }
  return response.json();
}

//Admin:Create a new User
export async function adminCreateUser(token: string, data: CreateUserDto): Promise<UserDto> {
  const response = await fetchWithAuth("/users", token, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText);
  }
  return response.json();
}

//Admin:Delete a user by ID
export async function adminDeleteUser(token: string, userId: number): Promise<void> {
  const response = await fetchWithAuth(`/users/${userId}`, token, {
    method: "DELETE",
  });
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText);
  }
}


//Admin:Update a user by ID
export async function adminUpdateUser(
  token: string,
  userId: number,
  data: UpdateUserDto
): Promise<UserDto> {
  const response = await fetchWithAuth(`/users/${userId}`, token, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText);
  }
  return response.json();
}

//#endregion

//#region User Profile API Functions

export async function fetchCurrentUser(token: string): Promise<UserDto> {
  const response = await fetchWithAuth("/users/me", token);
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText);
  }
  return response.json();
}
// Upload User Profile Photo
export async function uploadProfilePhoto(
  token: string,
  userId: number,
  file: File
): Promise<void> {
  const formData = new FormData();
  formData.append("photo", file); // must match backend field name

  const response = await fetchWithAuth(`/users/${userId}/upload-photo`, token, {
    method: "POST",
    body: formData,
    headers: {}, // don't set Content-Type
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText);
  }
}

// Update User Profile
export async function updateUserProfile(
  token: string,
  userId: number,
  updateData: Partial<TrainerUpdateDto> | Partial<ClientUpdateDto>
): Promise<UserDto> {
  const response = await fetchWithAuth(`/users/${userId}/profile`, token, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(updateData),
  });
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText);
  }
  return response.json();
}
//#endregion




