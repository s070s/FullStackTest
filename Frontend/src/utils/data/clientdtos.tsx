export type ClientDto = {
  id: number;
  userId: number;
  bio?: string;
  experienceLevel: string; // or enum type if defined
  preferredIntensityLevel: string; // or enum type if defined
  firstName?: string;
  lastName?: string;
  dateOfBirth?: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  weight?: number;
  height?: number;
  bmr?: number;
  bmi?: number;
  age?: number;
};



//Todo convert all record dtos on the backend to classes 
export type ClientUpdateDto = {
  firstName?: string;
  lastName?: string;
  bio?: string;
  dateOfBirth?: string;
  height?: number;
  weight?: number;
  phoneNumber?: string;
  country?: string;
  city?: string;
  address?: string;
  zipCode?: string;
  state?: string;
  experienceLevel: string;
};