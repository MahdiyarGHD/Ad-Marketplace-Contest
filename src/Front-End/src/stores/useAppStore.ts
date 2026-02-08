import { create } from "zustand";
import { requestAPI } from "../utils/api";

type AppState = {
	token?: string;
	isAuth: boolean;
	authenticate: (initData: string | undefined) => Promise<void>;
};

const useAppStore = create<AppState>((set) => ({
	isAuth: false,
	async authenticate(initData) {
		const response = await requestAPI("/api/authentication/authenticate", {
			init_data: initData,
		});

		set({ token: response.value.access_token, isAuth: true });

		console.log("token", response.value.access_token);
	},
}));

export default useAppStore;
