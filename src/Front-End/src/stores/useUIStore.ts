import type { ReactNode } from "react";
import { create } from "zustand";
import type { Channel } from "./useChannelStore";
import type { Campaign } from "./useCampaignStore";

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
	minKey: string;
	maxKey: string;
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

type SearchState<T> = {
	query: string;
	filters: FilterValues;
	results?: T[];
	setQuery: (value: string) => void;
	setFilter: (key: string, value: string | number | [number, number]) => void;
	setFilters: (filters: FilterValues) => void;
	setResults: (results?: T[]) => void;
};

const createSearchState =
	<T>() =>
	(set: any, key: "channels" | "campaigns"): SearchState<T> => ({
		query: "",
		filters: {},
		setQuery(value) {
			set((state: UIState) => ({
				search: {
					...state.search,
					[key]: {
						...state.search[key],
						query: value,
					},
				},
			}));
		},
		setFilters(filters) {
			set((state: UIState) => ({
				search: {
					...state.search,
					[key]: {
						...state.search[key],
						filters,
					},
				},
			}));
		},
		setFilter(keyName, value) {
			set((state: UIState) => ({
				search: {
					...state.search,
					[key]: {
						...state.search[key],
						filters: {
							...state.search[key].filters,
							[keyName]: value,
						},
					},
				},
			}));
		},
		setResults(results) {
			set((state: UIState) => ({
				search: {
					...state.search,
					[key]: {
						...state.search[key],
						results,
					},
				},
			}));
		},
	});

type UIState = {
	topBarTitle?: string;
	topBarButtons?: ReactNode;
	mainButton?: { text: string; onClick?: () => void };
	textButton?: { text: string; onClick?: () => void };
	toasts: Toast[];
	search: {
		channels: SearchState<Channel>;
		campaigns: SearchState<Campaign>;
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
		channels: createSearchState<Channel>()(set, "channels"),
		campaigns: createSearchState<Campaign>()(set, "campaigns"),
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
