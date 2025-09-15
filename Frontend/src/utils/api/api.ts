import type { UserDto, RegisterUserDto, LoginUserDto } from "../data/userdtos";

// Base URL of your API (adjust as needed)
const API_BASE_URL = "http://localhost:5203"; // change to your API http port



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
  const headers = {
    ...(options.headers || {}),
    "Authorization": `Bearer ${token}`,
    "Content-Type": "application/json",
  };

  return fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    headers,
  });
}

//#endregion


//#region Admin Only User Management API Functions
// Fetch all users
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
//#endregion






