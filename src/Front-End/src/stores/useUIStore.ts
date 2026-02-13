import type { ReactNode } from "react";
import { create } from "zustand";

type Toast = {
	id?: number;
	icon?: ReactNode;
	title: string;
};

type BaseFilter = {
	key: string;
	title: string;
	onClick?: () => void;
};

type NumberFilter = BaseFilter & {
	type: "number";
	unit?: string;
	range: [number, number];
};

type RangeFilter = BaseFilter & {
	type: "range";
	range: [number, number];
};

export type OptionsFilter = BaseFilter & {
	type: "options";
	options: Record<string | number, string>;
};

export type Filter =
	| (BaseFilter & {
			type: "text";
	  })
	| NumberFilter
	| RangeFilter
	| OptionsFilter;

export type FilterValues = {
	[key: string]: string | number | [number, number];
};

type UIState = {
	topBarTitle?: string;
	topBarButtons?: ReactNode;
	mainButton?: { text: string; onClick?: () => void };
	toasts: Toast[];
	search?: {
		query: string;
		filters: FilterValues;
		setQuery: (value: string) => void;
		setFilter: (key: string, value: string | number | [number, number]) => void;
	};
	setTopBarTitle: (value: string) => void;
	setTopBarButtons: (value: ReactNode) => void;
	// setMainButton: (value: {text: string; onClick: () => void}) => void;
	showToast: (toast: Toast) => void;
	removeToast: (id: number) => void;
};

const useUIStore = create<UIState>((set) => ({
	toasts: [],
	search: {
		query: "",
		filters: {},
		setQuery(value) {
			set((state) => ({
				search: {
					...state.search!,
					query: value,
					filters: state.search!.filters,
					setQuery: state.search!.setQuery,
					setFilter: state.search!.setFilter,
				},
			}));
		},
		setFilter(key, value) {
			set((state) => ({
				search: {
					...state.search!,
					filters: {
						...state.search!.filters,
						[key]: value,
					},
				},
			}));
		},
	},
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
