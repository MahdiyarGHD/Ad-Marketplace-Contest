import { create } from "zustand";
import { requestAPI } from "../utils/api";

type AppState = {
	token?: string;
	authenticate: (initData: string | undefined) => Promise<void>;
};

const useAppStore = create<AppState>((set) => ({
	async authenticate(initData) {
		const response = await requestAPI("/authenticate", { initData });

		set({ token: response.value.accessToken });
	},
}));

export default useAppStore;
