import type { UserDto, RegisterUserDto, LoginUserDto, CreateUserDto, UpdateUserDto } from "../data/userdtos";
import type { ClientUpdateDto } from "../data/clientdtos";
import type { TrainerUpdateDto,TrainerDto } from "../data/trainerdtos";
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
export async function authenticateUser(data: LoginUserDto): Promise<{ accessToken: string; accessTokenExpiresUtc: string }> {
  const res = await fetch(`${API_BASE_URL}/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify(data),
  });
  if (!res.ok) {
    const txt = await res.text();
    throw new Error(txt || "Login failed");
  }
  return res.json();
}

export async function refreshAuthToken(): Promise<{ accessToken: string; accessTokenExpiresUtc: string }> {
  const res = await fetch(`${API_BASE_URL}/refresh`, {
    method: "POST",
    credentials: "include" // crucial: send HttpOnly refresh cookie
  });
  if (!res.ok) {
    const txt = await res.text();
    throw new Error(txt || "Unable to refresh token");
  }
  return res.json();
}
// Used to fetch data from a protected api endpoint using JWT token after login
export async function fetchWithAuth(
  endpoint: string,
  token: string,
  options: RequestInit = {}
) {
  // ensure 'credentials' has the correct RequestCredentials type
  const opts: RequestInit = { ...options, credentials: "include" as RequestCredentials };
  const isFormData = opts.body instanceof FormData;

  // Convert headers to plain object if needed
  let baseHeaders: Record<string, string> = {};

  if (opts.headers instanceof Headers) {
    opts.headers.forEach((value, key) => {
      baseHeaders[key] = value;
    });
  } else if (Array.isArray(opts.headers)) {
    for (const [key, value] of opts.headers) {
      baseHeaders[key] = value;
    }
  } else if (typeof opts.headers === "object" && opts.headers !== null) {
    baseHeaders = { ...(opts.headers as Record<string, string>) };
  }

  if (token) baseHeaders["Authorization"] = `Bearer ${token}`;

  const hasContentType = Object.keys(baseHeaders).some(k => k.toLowerCase() === "content-type");
  if (!isFormData && !hasContentType) {
    baseHeaders["Content-Type"] = "application/json";
  }

  const headers = new Headers(baseHeaders);
  return fetch(`${API_BASE_URL}${endpoint}`, {
    ...opts,
    headers,
  });
}
//#endregion




//#region Admin Only User Management API Functions
// Admin:Fetch all users with Pagination and Sorting
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



//#region Client Dashboard Specific API Functions
export async function readAllTrainersPaginated(token: string, page: number, pageSize: number, sortBy?: string, sortOrder?: "asc" | "desc"): Promise<{ trainers: TrainerDto[]; total: number }>
{
  const params = new URLSearchParams();
  if(page) params.append("page",page.toString());
  if(pageSize) params.append("pageSize",pageSize.toString());
  if(sortBy) params.append("sortBy",sortBy);
  if(sortOrder) params.append("sortOrder",sortOrder);
  
  const endpoint =`/trainers${params.toString() ? "?" + params.toString() : ""}`;
  const response = await fetchWithAuth(endpoint, token);
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText);
  }
  return response.json();
}
// Subscribe a client to a trainer
export async function subscribeToTrainer(token: string, userId: number, trainerId: number): Promise<void> {
  const response = await fetchWithAuth(`/clients/${userId}/subscribe/${trainerId}`, token, {
    method: "POST",
  });
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Failed to subscribe to trainer.");
  }
}
// Unsubscribe a client from a trainer
export async function unsubscribeFromTrainer(token: string, userId: number, trainerId: number): Promise<void> {
  const response = await fetchWithAuth(`/clients/${userId}/unsubscribe/${trainerId}`, token, {
    method: "POST",
  });
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Failed to unsubscribe from trainer.");
  }
}

// Get list of trainer IDs the client is subscribed to
export async function getSubscribedTrainerIds(token: string, userId: number): Promise<number[]> {
  const response = await fetchWithAuth(`/clients/${userId}/subscriptions`, token);
  if (!response.ok) throw new Error("Failed to fetch subscribed trainers.");
  return await response.json();
}
//#endregion

