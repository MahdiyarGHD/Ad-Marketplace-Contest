import { CircleAlertIcon } from "lucide-react";
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

	if (token) {
		headers.Authorization = `Bearer ${token}`;
	}

	try {
		const res = await fetch(import.meta.env.VITE_BACKEND_BASE_URL + path, {
			method,
			headers: {
				"Content-Type": "application/json",
				...headers,
			},
			body: method === "POST" && body ? JSON.stringify(body) : undefined,
		});

		if (!res.ok) {
			const error = await res.json();
			throw new Error(
				`API Error ${res.status}: ${error.message || res.statusText}`,
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
