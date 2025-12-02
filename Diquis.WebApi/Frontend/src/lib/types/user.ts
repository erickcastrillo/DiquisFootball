// User
export interface User {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    isActive: boolean;
    roleId: string;
    imageUrl: string;
    createdOn: string;
}

// Register new user
export interface RegisterUserRequest {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    phoneNumber: string;
    roleId: string;
}

