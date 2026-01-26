import useAppStore from "../stores/useAppStore";

type HTTPMethod = "GET" | "POST" | "PUT" | "PATCH" | "DELETE";

export async function requestAPI<TResponse = any>(
	path: string = "/",
	body: { [key: string]: string | Blob | undefined } = {},
	method: HTTPMethod = "POST",
): Promise<TResponse> {
	const { token } = useAppStore.getState();

	const headers: { [key: string]: string } = {};

	if (token) {
		headers.Authorization = `Bearer ${token}`;
	}

	const res = await fetch(import.meta.env.VITE_BACKEND_BASE_URL + path, {
		method,
		headers: {
			"Content-Type": "application/json",
			...headers,
		},
		body: body ? JSON.stringify(body) : undefined,
	});

	if (!res.ok) {
		const errorText = await res.text();
		throw new Error(`API Error ${res.status}: ${errorText || res.statusText}`);
	}

	return res.json() as Promise<TResponse>;
}
