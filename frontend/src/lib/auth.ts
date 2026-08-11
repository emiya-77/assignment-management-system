import { LoginResponse } from "@/types/auth";

const AUTH_KEY = "assignment-management-auth";

export function saveAuth(
  auth: LoginResponse
) {
  if (typeof window === "undefined") {
    return;
  }

  localStorage.setItem(
    AUTH_KEY,
    JSON.stringify(auth)
  );
}

export function getAuth(): LoginResponse | null {
  if (typeof window === "undefined") {
    return null;
  }

  const storedAuth =
    localStorage.getItem(AUTH_KEY);

  if (!storedAuth) {
    return null;
  }

  try {
    return JSON.parse(storedAuth);
  } catch {
    return null;
  }
}

export function getToken(): string | null {
  return getAuth()?.token ?? null;
}

export function logout() {
  if (typeof window === "undefined") {
    return;
  }

  localStorage.removeItem(AUTH_KEY);
}