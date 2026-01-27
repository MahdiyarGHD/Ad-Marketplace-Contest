import { create } from "zustand";

type UIState = {
	topBarTitle?: string;
	setTopBarTitle: (value: string) => void;
};

const useUIStore = create<UIState>((set) => ({
	setTopBarTitle(value) {
		set({ topBarTitle: value });
	},
}));

export default useUIStore;
