import type { ClientDto } from './clientdtos';
export type TrainerDto = {
    id: number;
    userId: number;
    bio?: string;
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
    clients: ClientDto[]; // Replace 'any' with ClientDto[] if you have it
    specializations: any[]; // Replace 'any' with TrainerSpecializationDto[] if you have it
};