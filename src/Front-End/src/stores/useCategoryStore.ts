import { create } from "zustand";
import { requestAPI } from "../utils/api";

type CategoryState = {
	categories: any[];
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
