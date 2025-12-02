import { Roles } from './roles';

// Current User (profile)
export interface CurrentUser {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  isActive: boolean;
  roleId: Roles;
  imageUrl: string;
  createdOn: string;
}

// Update your profile (includes image upload, excludes role and active)
export interface UpdateProfileRequest {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
}

// Update your preferences
export interface UpdatePreferencesRequest {
  darkModeDefault: boolean;
  pageSizeDefault: number;
}

// Update your password
export interface ChangePasswordRequest {
  password: string;
  newPassword: string;
  confirmNewPassword: string;
}

// Change your profile image
export interface ChangeProfileImageRequest {
  DeleteCurrentImage?: boolean;
  ImageFile?: Blob;
}
