import { create } from "zustand";
import { requestAPI } from "../utils/api";

type AppState = {
	token?: string;
	isAuth: boolean;
	startParamHandled: boolean;
	userId: string | undefined;
	authenticate: (initData: string | undefined) => Promise<void>;
	getMe: () => Promise<void>;
};

const useAppStore = create<AppState>((set) => ({
	isAuth: false,
	startParamHandled: false,
	userId: undefined,
	async authenticate(initData) {
		const response = await requestAPI("/api/authentication/authenticate", {
			init_data: initData,
		});

		set({ token: response.value.access_token, isAuth: true });

		console.log("token", response.value.access_token);
	},
	async getMe() {
		const response = await requestAPI("/api/user/me", {}, "GET");

		if (response.value) {
			set({ userId: response.value.id });
		}
	},
}));

export default useAppStore;
