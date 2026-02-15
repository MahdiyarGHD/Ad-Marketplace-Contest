import { create } from "zustand";
import { requestAPI } from "../utils/api";

export type DealType = {
	id: string;
	campaign_id: string;
	campaign_title: string;
	channel_id: string;
	channel_title: string;
	advertiser_id: string;
	amount_ton: number;
	ad_format: number;
	price_type: number;
	status: number;
	draft_status: number;
	advertiser_feedback: string;
	scheduled_post_time: string;
	actual_post_time: string;
	posted_message_id: number;
	draft_message_id: number;
	escrow_wallet_address: string;
	transaction_hash: string;
	auto_cancel_at: string;
	funds_released_at: string;
	created_at: string;
	updated_at: string;
};

type DealState = {
	deal?: Partial<DealType>;
	deals?: DealType[];
	setDeal: (deal: Partial<DealType>) => void;
	setDeals: (deals: DealType[]) => void;
	getChannelDeals: (channelId: string) => Promise<void>;
	getCampaignDeals: (campaignId: string) => Promise<void>;
	getMyDeals: () => Promise<void>;
	clearDeal: () => void;
};

const useDealStore = create<DealState>((set) => ({
	setDeal(deal: Partial<DealType>) {
		set((state) => ({
			deal: {
				...state.deal,
				...deal,
			},
		}));
	},
	clearDeal() {
		set({ deal: undefined });
	},
	setDeals(deals: DealType[]) {
		set({ deals });
	},
	async getChannelDeals(channelId: string) {
		set({ deals: undefined });

		const response = await requestAPI(
			`/api/deals/channel/${channelId}`,
			{},
			"GET",
		);
		if (!response.isError && response.value?.deals) {
			set({ deals: response.value.deals as DealType[] });
		}
	},
	async getCampaignDeals(campaignId: string) {
		set({ deals: undefined });

		const response = await requestAPI(
			`/api/deals/campaigns/${campaignId}`,
			{},
			"GET",
		);
		if (!response.isError && response.value?.deals) {
			set({ deals: response.value.deals as DealType[] });
		}
	},
	async getMyDeals() {
		set({ deals: undefined });

		const response = await requestAPI("/api/deals/my", {}, "GET");
		if (!response.isError && response.value?.deals) {
			set({ deals: response.value.deals as DealType[] });
		}
	},
}));

export default useDealStore;
