import { create } from "zustand";
import { requestAPI } from "../utils/api";

export type Category = {
	id: string;
	name: string;
	description?: string;
	icon: string;
	display_order?: number;
};

type CategoryState = {
	categories: Category[];
	getCategories: () => Promise<void>;
};

const useCategoryStore = create<CategoryState>((set) => ({
	categories: [],
	async getCategories() {
		const response = await requestAPI("/api/categories", {}, "GET");

		set({ categories: response.value });
	},
}));

export default useCategoryStore;
