import { create } from "zustand";
import { requestAPI } from "../utils/api";
import type { Category } from "./useCategoryStore";

export type Campaign = {
	id?: string;
	category_id: number;
	category?: {
		id: string;
		name: string;
		description: string;
		icon: string;
		display_order: number;
	};
	title: string;
	description: string;
	budget_ton: number;
	brief: string;
	max_price_per_placement: number;
	status: number;
	targeting: {
		min_subscribers: number;
		max_subscribers: number;
		min_average_views: number;
		min_premium_count: number;
		preferred_category_ids: string[];
		preferred_languages: string[];
		preferred_ad_formats: number[];
		preferred_price_types: number[];
	};
	creative: {
		call_to_action_buttons: {
			text: string;
			url: string;
		}[];
		requires_approval: boolean;
	};
	starts_at: string;
	ends_at: string;
	application_deadline: string;
};

type CampaignState = {
	myCampaigns: Campaign[];
	draftCampaign: Campaign;
	getMyCampaigns: () => Promise<void>;
	setDraftCampaign: (campaign: Campaign) => void;
	clearDraftCampaign: () => void;
	setDraftCampaignCategory: (category: any) => void;
	setDraftCampaignPreferredCategory: (category: any) => void;
	setDraftCampaignAdFormat: (ad_format: number) => void;
	setDraftCampaignPriceType: (price_type: number) => void;

	campaigns: {
		elements: {
			$type: string;
			icon: string;
			label: string;
			items: Campaign[] | Category[];
		}[];
	};
	getCampaigns: () => Promise<void>;

	activeCampaign?: Campaign;
	setActiveCampaign: (campaign: Campaign) => void;
};

const useCampaignStore = create<CampaignState>((set) => ({
	myCampaigns: [],
	draftCampaign: {} as Campaign,
	campaigns: {
		elements: [],
	},
	async getMyCampaigns() {
		const response = await requestAPI("/api/campaigns/my", {}, "GET");

		if (response.value) set({ myCampaigns: response.value.campaigns });
	},
	setDraftCampaign(campaign) {
		set((state) => ({
			draftCampaign: {
				...state.draftCampaign,
				...campaign,
			},
		}));
	},
	clearDraftCampaign() {
		set({
			draftCampaign: {} as Campaign,
		});
	},
	setDraftCampaignCategory(category: any) {
		set((state) => ({
			draftCampaign: {
				...state.draftCampaign,
				category_id: category.id,
				category,
			},
		}));
	},
	setDraftCampaignPreferredCategory(category: any) {
		set((state) => ({
			draftCampaign: {
				...state.draftCampaign,
				targeting: {
					...state.draftCampaign.targeting,
					preferred_category_ids:
						state.draftCampaign.targeting?.preferred_category_ids?.includes(
							category.id,
						)
							? state.draftCampaign.targeting.preferred_category_ids.filter(
									(id) => id !== category.id,
								)
							: [
									...(state.draftCampaign.targeting?.preferred_category_ids ||
										[]),
									category.id,
								],
				},
			},
		}));
	},
	setDraftCampaignAdFormat(ad_format: number) {
		set((state) => ({
			draftCampaign: {
				...state.draftCampaign,
				targeting: {
					...state.draftCampaign.targeting,
					preferred_ad_formats:
						state.draftCampaign.targeting?.preferred_ad_formats?.includes(
							ad_format,
						)
							? state.draftCampaign.targeting.preferred_ad_formats.filter(
									(i) => i !== ad_format,
								)
							: [
									...(state.draftCampaign.targeting?.preferred_ad_formats ||
										[]),
									ad_format,
								],
				},
			},
		}));
	},
	setDraftCampaignPriceType(price_type: number) {
		set((state) => ({
			draftCampaign: {
				...state.draftCampaign,
				targeting: {
					...state.draftCampaign.targeting,
					preferred_price_types:
						state.draftCampaign.targeting?.preferred_price_types?.includes(
							price_type,
						)
							? state.draftCampaign.targeting.preferred_price_types.filter(
									(i) => i !== price_type,
								)
							: [
									...(state.draftCampaign.targeting?.preferred_price_types ||
										[]),
									price_type,
								],
				},
			},
		}));
	},

	getCampaigns: async () => {
		const response = await requestAPI("/api/campaigns/home", {}, "GET");

		console.log(response);

		if (response.value) {
			set({
				campaigns: response.value,
			});
		}
	},

	setActiveCampaign(campaign) {
		set({ activeCampaign: campaign });
	},
}));

export default useCampaignStore;
