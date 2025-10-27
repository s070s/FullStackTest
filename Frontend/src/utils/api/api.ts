import type { UserDto, RegisterUserDto, LoginUserDto, CreateUserDto, UpdateUserDto } from "../data/userdtos";
import type { ClientUpdateDto } from "../data/clientdtos";
import type { TrainerUpdateDto,TrainerDto } from "../data/trainerdtos";

// API base URL from env; defaults to production URL
export const API_BASE_URL = import.meta.env.VITE_API_DEV_URL || import.meta.env.VITE_API_URL;

//#region Auth
// Register user
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

// Login; returns access token
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

// Refresh access token using refresh cookie
export async function refreshAuthToken(): Promise<{ accessToken: string; accessTokenExpiresUtc: string } | null> {
  const res = await fetch(`${API_BASE_URL}/refresh`, {
    method: "POST",
    credentials: "include"
  });
  if (res.status === 401) return null;
  if (!res.ok) {
    const msg = await res.text().catch(() => "");
    throw new Error(msg || `Refresh failed (${res.status}).`);
  }
  return res.json();
}

// Fetch helper with Bearer token
export async function fetchWithAuth(
  endpoint: string,
  token: string,
  options: RequestInit = {}
) {
  // Ensure credentials type
  const opts: RequestInit = { ...options, credentials: "include" as RequestCredentials };
  const isFormData = opts.body instanceof FormData;

  // Normalize headers
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

  // Attach Authorization header if token
  if (token) baseHeaders["Authorization"] = `Bearer ${token}`;

  // Set JSON Content-Type when not sending FormData
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

//#region Admin users
// List users (paged)
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

// Get user stats
export async function adminFetchUserStatistics(token: string): Promise<any> {
  const response = await fetchWithAuth("/users/statistics", token);
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText);
  }
  return response.json();
}

// Create user
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

// Delete user
export async function adminDeleteUser(token: string, userId: number): Promise<void> {
  const response = await fetchWithAuth(`/users/${userId}`, token, {
    method: "DELETE",
  });
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText);
  }
}

// Update user
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

//#region Profile
export async function fetchCurrentUser(token: string): Promise<UserDto> {
  const response = await fetchWithAuth("/users/me", token);
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText);
  }
  return response.json();
}

// Upload profile photo
export async function uploadProfilePhoto(
  token: string,
  userId: number,
  file: File
): Promise<void> {
  const formData = new FormData();
  formData.append("photo", file); // field name must match backend

  const response = await fetchWithAuth(`/users/${userId}/upload-photo`, token, {
    method: "POST",
    body: formData,
    headers: {}, // let browser set Content-Type
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText);
  }
}

// Update profile
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

//#region Client dashboard
// List trainers (paged)
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

// Subscribe client to trainer
export async function subscribeToTrainer(token: string, userId: number, trainerId: number): Promise<void> {
  const response = await fetchWithAuth(`/clients/${userId}/subscribe/${trainerId}`, token, {
    method: "POST",
  });
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Failed to subscribe to trainer.");
  }
}

// Unsubscribe client from trainer
export async function unsubscribeFromTrainer(token: string, userId: number, trainerId: number): Promise<void> {
  const response = await fetchWithAuth(`/clients/${userId}/unsubscribe/${trainerId}`, token, {
    method: "POST",
  });
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Failed to unsubscribe from trainer.");
  }
}

// Get subscribed trainer IDs
export async function getSubscribedTrainerIds(token: string, userId: number): Promise<number[]> {
  const response = await fetchWithAuth(`/clients/${userId}/subscriptions`, token);
  if (!response.ok) throw new Error("Failed to fetch subscribed trainers.");
  return await response.json();
}
//#endregion

