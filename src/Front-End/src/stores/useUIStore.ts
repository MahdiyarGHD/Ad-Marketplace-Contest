import type { ReactNode } from "react";
import { create } from "zustand";

type UIState = {
	topBarTitle?: string;
	topBarButtons?: ReactNode;
	setTopBarTitle: (value: string) => void;
	setTopBarButtons: (value: ReactNode) => void;
};

const useUIStore = create<UIState>((set) => ({
	setTopBarTitle(value) {
		set({ topBarTitle: value });
	},
	setTopBarButtons(value) {
		set({ topBarButtons: value });
	},
}));

export default useUIStore;
