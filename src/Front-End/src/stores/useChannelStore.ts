import { create } from "zustand";
import { requestAPI } from "../utils/api";
import type { Category } from "./useCategoryStore";

export const AdFormats = { 1: "Post" };
export const PriceTypes = {
	1: "Per hour",
	2: "Per day",
	3: "Per 1000 views",
};

export type AdFormat = keyof typeof AdFormats;
export type PriceType = keyof typeof PriceTypes;

export type Channel = {
	id?: string;
	chat_id: number;
	owner_id?: string;
	category_id: string;
	category?: {
		id: string;
		name: string;
		description: string;
		icon: string;
		display_order: number;
	};
	title: string;
	username?: string;
	description?: string;
	subscriber_count: number;
	average_views: number;
	premium_count: number;
	language_distribution_json: [
		{
			language: string;
			percentage: number;
		},
	];
	status: number;
	pricings: {
		ad_format: number;
		price_type: number;
		price_ton: number;
	}[];
};

type ChannelState = {
	myChannels: Channel[];
	draftChannel: Partial<Channel>;
	unVerifiedChannels: Channel[];
	getMyChannels: () => Promise<void>;
	verifyChannel: () => Promise<void>;
	setUnVerifiedChannels: (channels: Channel[]) => void;
	setDraftChannel: (channel: Channel) => void;
	clearDraftChannel: () => void;
	addDraftChannelPrice: () => void;
	removeDraftChannelPrice: (index: number) => void;
	setDraftChannelCategory: (category: any) => void;
	setDraftChannelPriceType: (priceType: number, index: number) => void;
	setDraftChannelAdFormat: (adFormat: number, index: number) => void;
	setDraftChannelPrice: (priceTon: number, index: number) => void;

	influencers: {
		elements: {
			$type: string;
			icon: string;
			label: string;
			items: Channel[] | Category[];
		}[];
	};
	getInfluencers: () => Promise<void>;

	campaigns: {
		elements: {
			$type: string;
			icon: string;
			label: string;
			items: Channel[] | Category[];
		}[];
	};
	getCampaigns: () => Promise<void>;

	activeChannel?: Channel;
	setActiveChannel: (channel: Channel) => void;
};

const useChannelStore = create<ChannelState>((set, get) => ({
	myChannels: [],
	draftChannel: {
		pricings: [
			{
				ad_format: 1,
				price_type: 1,
				price_ton: 0,
			},
		],
	},
	unVerifiedChannels: [],
	influencers: {
		elements: [],
	},
	campaigns: {
		elements: [],
	},
	async getMyChannels() {
		const response = await requestAPI("/api/channels/my", {}, "GET");

		if (response.value) set({ myChannels: response.value });
	},
	async verifyChannel() {
		const response = await requestAPI("/api/channels/verify-add", {
			look_back_seconds: 900,
		});

		set(() => ({
			unVerifiedChannels: response.value,
		}));
	},
	setUnVerifiedChannels(channels: Channel[]) {
		set({ unVerifiedChannels: channels });
	},
	setDraftChannel(channel) {
		set((state) => ({
			draftChannel: {
				...state.draftChannel,
				// chat_id: channel.chat_id,
				// title: channel.title,
				...channel,
			},
		}));
	},
	clearDraftChannel() {
		set({
			draftChannel: {
				pricings: [
					{
						ad_format: 1,
						price_type: 1,
						price_ton: 0,
					},
				],
			},
		});
	},
	setDraftChannelCategory(category: any) {
		set((state) => ({
			draftChannel: {
				...state.draftChannel,
				category_id: category.id,
				category,
			},
		}));
	},
	addDraftChannelPrice() {
		const allPriceTypes = get().draftChannel.pricings?.map(
			(item) => item.price_type,
		);

		const availablePriceType = Object.keys(PriceTypes).filter(
			(item) => !allPriceTypes!.includes(Number(item)),
		);

		set((state) => ({
			draftChannel: {
				...state.draftChannel,
				pricings: [
					...(state.draftChannel.pricings ?? []),
					{
						ad_format: 1,
						price_type: Number(availablePriceType[0]),
						price_ton: 0,
					},
				],
			},
		}));
	},
	removeDraftChannelPrice(index: number) {
		set((state) => ({
			draftChannel: {
				...state.draftChannel,
				pricings: state.draftChannel.pricings?.filter((_, i) => i !== index),
			},
		}));
	},
	setDraftChannelPriceType(priceType: number, index: number) {
		set((state) => ({
			draftChannel: {
				...state.draftChannel,
				pricings: state.draftChannel.pricings?.map((item, i) =>
					i === index ? { ...item, price_type: priceType } : item,
				),
			},
		}));
	},
	setDraftChannelAdFormat(adFormat: number, index: number) {
		set((state) => ({
			draftChannel: {
				...state.draftChannel,
				pricings: state.draftChannel.pricings?.map((item, i) =>
					i === index ? { ...item, ad_format: adFormat } : item,
				),
			},
		}));
	},
	setDraftChannelPrice(priceTon: number, index: number) {
		priceTon = Math.max(0, Math.min(100000, priceTon));

		set((state) => ({
			draftChannel: {
				...state.draftChannel,
				pricings: state.draftChannel.pricings?.map((item, i) =>
					i === index ? { ...item, price_ton: priceTon } : item,
				),
			},
		}));
	},

	getInfluencers: async () => {
		const response = await requestAPI("/api/channels/home", {}, "GET");

		if (response.value) {
			set({
				influencers: response.value,
			});
		}
	},
	getCampaigns: async () => {
		const response = await requestAPI("/api/campaigns/home", {}, "GET");

		if (response.value) {
			set({
				campaigns: response.value,
			});
		}
	},

	setActiveChannel(channel) {
		set({ activeChannel: channel });
	},
}));

export default useChannelStore;
