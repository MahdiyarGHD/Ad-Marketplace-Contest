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
	getCategory: (id: string) => Category | undefined;
	getCategoryByName: (name: string) => Category | undefined;
};

const useCategoryStore = create<CategoryState>((set, get) => ({
	categories: [],
	async getCategories() {
		const response = await requestAPI("/api/categories", {}, "GET");

		set({ categories: response.value });
	},
	getCategory(id: string) {
		const category = get().categories.find((cat) => cat.id === id);

		return category;
	},
	getCategoryByName(name: string) {
		const category = get().categories.find((cat) => cat.name === name);

		return category;
	},
}));

export default useCategoryStore;
