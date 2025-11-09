import { apiFetch } from '../api/http';
import type { PagedResult } from '../types/pagination';
import type { User, CreateUserPayload, UpdateUserPayload, ListUsersParams } from '../types/user';
import { parseUserRole, parseUserStatus, userRoleToString, userStatusToString } from '../types/user';
import { getValidAccessToken } from './auth.service';

// Backend API response types
interface ApiUser {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  status: string;
  emailConfirmed: boolean;
  createdAt: string;
  updatedAt: string;
}

/**
 * Transform API user to frontend user type
 */
function transformApiUser(apiUser: ApiUser): User {
  return {
    id: apiUser.id,
    email: apiUser.email,
    firstName: apiUser.firstName,
    lastName: apiUser.lastName,
    role: parseUserRole(apiUser.role),
    status: parseUserStatus(apiUser.status),
    emailVerified: apiUser.emailConfirmed,
    createdAt: apiUser.createdAt,
    updatedAt: apiUser.updatedAt,
  };
}

/**
 * Wrapper to add authorization header with token refresh
 */
async function withAuthorization<T>(
  apiCall: (token: string) => Promise<T>
): Promise<T> {
  const token = await getValidAccessToken();
  if (!token) {
    throw new Error('No valid access token available');
  }

  try {
    return await apiCall(token);
  } catch (error: any) {
    // If we get a 401, try refreshing the token once
    if (error.status === 401) {
      const newToken = await getValidAccessToken();
      if (!newToken) {
        throw new Error('Failed to refresh access token');
      }
      return await apiCall(newToken);
    }
    throw error;
  }
}

/**
 * List users with pagination and filtering
 */
export async function listUsers(
  params: ListUsersParams = {}
): Promise<PagedResult<User>> {
  return withAuthorization(async (token) => {
    const queryParams = new URLSearchParams();

    if (params.page !== undefined) {
      queryParams.append('pageNumber', params.page.toString());
    }
    if (params.pageSize !== undefined) {
      queryParams.append('pageSize', params.pageSize.toString());
    }
    if (params.search) {
      queryParams.append('searchTerm', params.search);
    }
    if (params.role !== undefined) {
      queryParams.append('role', params.role.toString());
    }
    if (params.status !== undefined) {
      queryParams.append('status', params.status.toString());
    }

    const queryString = queryParams.toString();
    const url = queryString ? `/users?${queryString}` : '/users';

    const response = await apiFetch<PagedResult<ApiUser>>(url, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    return {
      ...response,
      items: response.items.map(transformApiUser),
    };
  });
}

/**
 * Get a single user by ID
 */
export async function getUserById(userId: string): Promise<User> {
  return withAuthorization(async (token) => {
    return apiFetch<User>(`/users/${userId}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  });
}

/**
 * Create a new user
 */
export async function createUser(payload: CreateUserPayload): Promise<User> {
  return withAuthorization(async (token) => {
    const apiPayload = {
      email: payload.email,
      password: payload.password,
      firstName: payload.firstName,
      lastName: payload.lastName,
      role: userRoleToString(payload.role),
    };

    const response = await apiFetch<ApiUser>('/users', {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(apiPayload),
    });

    return transformApiUser(response);
  });
}

/**
 * Update an existing user
 */
export async function updateUser(
  userId: string,
  payload: UpdateUserPayload
): Promise<User> {
  return withAuthorization(async (token) => {
    const apiPayload: any = {};

    if (payload.email !== undefined) {
      apiPayload.email = payload.email;
    }
    if (payload.firstName !== undefined) {
      apiPayload.firstName = payload.firstName;
    }
    if (payload.lastName !== undefined) {
      apiPayload.lastName = payload.lastName;
    }
    if (payload.role !== undefined) {
      apiPayload.role = userRoleToString(payload.role);
    }
    if (payload.status !== undefined) {
      apiPayload.status = userStatusToString(payload.status);
    }

    const response = await apiFetch<ApiUser>(`/users/${userId}`, {
      method: 'PUT',
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(apiPayload),
    });

    return transformApiUser(response);
  });
}

/**
 * Delete a user
 */
export async function deleteUser(userId: string): Promise<void> {
  return withAuthorization(async (token) => {
    return apiFetch<void>(`/users/${userId}`, {
      method: 'DELETE',
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  });
}
