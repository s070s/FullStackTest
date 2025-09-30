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
export type TrainerUpdateDto = {
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
    specializations?: any[]; // Replace 'any' with your specialization type if available
};