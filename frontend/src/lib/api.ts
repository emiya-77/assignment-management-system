const API_URL =
  process.env.NEXT_PUBLIC_API_URL ??
  "https://localhost:5001/api";

interface ApiRequestOptions extends RequestInit {
  token?: string;
}

export async function api<T>(
  endpoint: string,
  options: ApiRequestOptions = {}
): Promise<T> {
  const { token, headers, ...rest } = options;

  const response = await fetch(
    `${API_URL}${endpoint}`,
    {
      ...rest,
      headers: {
        "Content-Type": "application/json",
        ...(token
          ? {
              Authorization: `Bearer ${token}`,
            }
          : {}),
        ...headers,
      },
    }
  );

  if (!response.ok) {
    const error = await response
      .json()
      .catch(() => null);

    throw new Error(
      error?.message ??
        "Something went wrong. Please try again."
    );
  }

  if (response.status === 204) {
    return null as T;
  }

  return response.json();
}