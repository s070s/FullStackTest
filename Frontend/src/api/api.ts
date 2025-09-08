
export type UserDto = {
  id: number;
  username: string;
  email: string;
  createdUtc: string; // ISO string from server
  isActive: boolean;
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



// Base URL of your API (adjust as needed)
const API_BASE_URL = "http://localhost:5203"; // change to your API http port


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

// Login a user
export async function loginUser(data: LoginUserDto): Promise<{ token: string }> {
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

