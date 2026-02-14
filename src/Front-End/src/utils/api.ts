import useAppStore from "../stores/useAppStore";
import useUIStore from "../stores/useUIStore";

type HTTPMethod = "GET" | "POST" | "PUT" | "PATCH" | "DELETE";

export async function requestAPI<
	TResponse = any,
	TBody = { [key: string]: string | number | Blob | undefined },
>(
	path: string = "/",
	body: TBody = {} as TBody,
	method: HTTPMethod = "POST",
): Promise<TResponse | false> {
	const { token } = useAppStore.getState();
	const { showToast } = useUIStore.getState();

	const headers: { [key: string]: string } = {};
	const hasBody = body && Object.keys(body).length > 0;

	if (token) {
		headers.Authorization = `Bearer ${token}`;
	}

	if (method === "GET" && hasBody) {
		const queryParams = new URLSearchParams(
			body as Record<string, string>,
		).toString();
		path += `?${queryParams}`;
	}

	try {
		const res = await fetch(import.meta.env.VITE_BACKEND_BASE_URL + path, {
			method,
			headers: {
				"Content-Type": method !== "GET" && hasBody ? "application/json" : "",
				...headers,
			},
			body: method !== "GET" && hasBody ? JSON.stringify(body) : undefined,
		});

		if (!res.ok) {
			const error = await res.json();
			throw new Error(
				`API Error ${res.status}: ${error.first_error || error.message || res.statusText}`,
			);
		}

		return res.json() as Promise<TResponse>;
	} catch (error) {
		showToast({
			title: error instanceof Error ? error.message : String(error),
		});

		console.error("API request error:", error);

		return false;
	}
}
