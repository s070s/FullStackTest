export type CreateUserDto = {
  username: string;
  email: string;
};

export type UpdateUserDto = {
  email?: string | null;
  isActive?: boolean | null;
};


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



// Base URL of your API (adjust as needed)
const API_BASE_URL = "http://localhost:5203"; // change to your API http port

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


// Create a new user
export async function createUser(data: CreateUserDto): Promise<UserDto> {
  const response = await fetch(`${API_BASE_URL}/users`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data), // send the user data
  });

  if (!response.ok) {
    // If backend returns 409 conflict, throw error
    const errorText = await response.text();
    throw new Error(errorText);
  }

  return response.json(); // return created user data
}

// Get all users
export async function getAllUsers():Promise<UserDto[]>{
    const response = await fetch(`${API_BASE_URL}/users`);
    if(!response.ok)
    {
        throw new Error("Failed to Fetch Users");
    }
    return response.json();
}

