import type { UserDto, RegisterUserDto, LoginUserDto,CreateUserDto } from "../data/userdtos";

// Base URL of your API (adjust as needed)
export const API_BASE_URL = "http://localhost:5203"; // change to your API http port



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
export async function authenticateUser(data: LoginUserDto): Promise<{ token: string }> {
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
//#endregion




