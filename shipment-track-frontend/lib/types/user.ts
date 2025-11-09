export enum UserRole {
  Admin = 0,
  Staff = 1,
  Customer = 2,
}

export enum UserStatus {
  Active = 0,
  Inactive = 1,
  Suspended = 2,
}

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  status: UserStatus;
  emailVerified: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateUserPayload {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  role: UserRole;
}

export interface UpdateUserPayload {
  email?: string;
  firstName?: string;
  lastName?: string;
  role?: UserRole;
  status?: UserStatus;
}

export interface ListUsersParams {
  page?: number;
  pageSize?: number;
  search?: string;
  role?: UserRole;
  status?: UserStatus;
}

export function getUserRoleLabel(role: UserRole): string {
  switch (role) {
    case UserRole.Admin:
      return 'Admin';
    case UserRole.Staff:
      return 'Staff';
    case UserRole.Customer:
      return 'Customer';
    default:
      return 'Unknown';
  }
}

export function getUserStatusLabel(status: UserStatus): string {
  switch (status) {
    case UserStatus.Active:
      return 'Active';
    case UserStatus.Inactive:
      return 'Inactive';
    case UserStatus.Suspended:
      return 'Suspended';
    default:
      return 'Unknown';
  }
}

export function parseUserRole(roleString: string | undefined): UserRole {
  if (!roleString) return UserRole.Customer;
  switch (roleString) {
    case 'Admin':
      return UserRole.Admin;
    case 'Staff':
      return UserRole.Staff;
    case 'Customer':
      return UserRole.Customer;
    default:
      return UserRole.Customer;
  }
}

export function parseUserStatus(statusString: string | undefined): UserStatus {
  if (!statusString) return UserStatus.Active;
  switch (statusString) {
    case 'Active':
      return UserStatus.Active;
    case 'Inactive':
      return UserStatus.Inactive;
    case 'Suspended':
      return UserStatus.Suspended;
    default:
      return UserStatus.Active;
  }
}

export function userRoleToString(role: UserRole): string {
  return getUserRoleLabel(role);
}

export function userStatusToString(status: UserStatus): string {
  return getUserStatusLabel(status);
}
