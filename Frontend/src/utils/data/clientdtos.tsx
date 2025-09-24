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