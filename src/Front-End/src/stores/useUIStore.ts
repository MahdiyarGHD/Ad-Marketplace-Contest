import type { ReactNode } from "react";
import { create } from "zustand";

type Toast = {
	id?: number;
	icon?: ReactNode;
	title: string;
};

type UIState = {
	topBarTitle?: string;
	topBarButtons?: ReactNode;
	mainButton?: { text: string; onClick?: () => void };
	toasts: Toast[];
	setTopBarTitle: (value: string) => void;
	setTopBarButtons: (value: ReactNode) => void;
	// setMainButton: (value: {text: string; onClick: () => void}) => void;
	showToast: (toast: Toast) => void;
	removeToast: (id: number) => void;
};

const useUIStore = create<UIState>((set) => ({
	toasts: [],
	setTopBarTitle(value) {
		set({ topBarTitle: value });
	},
	setTopBarButtons(value) {
		set({ topBarButtons: value });
	},
	showToast(toast) {
		toast.id = Date.now();

		set((state) => ({
			toasts: [...state.toasts, toast],
		}));
	},
	removeToast(id) {
		set((state) => ({
			toasts: state.toasts.filter((t) => t.id !== id),
		}));
	},
}));

export default useUIStore;
