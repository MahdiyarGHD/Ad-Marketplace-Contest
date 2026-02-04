import { create } from "zustand";
import { requestAPI } from "../utils/api";

export const PriceTypes = ["Per hour", "Per day", "Per 1000 views"];

export type Channel = {
	chat_id: number;
	category_id: number;
	category?: {
		id: string;
		name: string;
		description: string;
		icon: string;
		display_order: number;
	};
	title: string;
	status: number;
	pricing: {
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
	addDraftChannelPrice: () => void;
	setDraftChannelCategory: (category: any) => void;
	setDraftChannelPriceType: (priceType: number, index: number) => void;
	setDraftChannelAdFormat: (adFormat: number, index: number) => void;
	setDraftChannelPrice: (priceTon: number, index: number) => void;
};

const useChannelStore = create<ChannelState>((set, get) => ({
	myChannels: [],
	draftChannel: {
		pricing: [
			{
				ad_format: 0,
				price_type: 0,
				price_ton: 0,
			},
		],
	},
	unVerifiedChannels: [],
	async getMyChannels() {
		const response = await requestAPI("/api/channels/my", {}, "GET");

		if (response.value) set({ myChannels: response.value });
	},
	async verifyChannel() {
		const response = await requestAPI("/api/channels/verify-add", {
			look_back_seconds: 900,
		});

		console.log(response);

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
				chat_id: channel.chat_id,
				title: channel.title,
			},
		}));
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
		const allPriceTypes = get().draftChannel.pricing?.map(
			(item) => item.price_type,
		);

		const availablePriceType = PriceTypes.filter(
			(_, i) => !allPriceTypes!.includes(i),
		);

		set((state) => ({
			draftChannel: {
				...state.draftChannel,
				pricing: [
					...(state.draftChannel.pricing ?? []),
					{
						ad_format: 0,
						price_type: PriceTypes.indexOf(availablePriceType[0]),
						price_ton: 0,
					},
				],
			},
		}));
	},
	setDraftChannelPriceType(priceType: number, index: number) {
		set((state) => ({
			draftChannel: {
				...state.draftChannel,
				pricing: state.draftChannel.pricing?.map((item, i) =>
					i === index ? { ...item, price_type: priceType } : item,
				),
			},
		}));
	},
	setDraftChannelAdFormat(adFormat: number, index: number) {
		set((state) => ({
			draftChannel: {
				...state.draftChannel,
				pricing: state.draftChannel.pricing?.map((item, i) =>
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
				pricing: state.draftChannel.pricing?.map((item, i) =>
					i === index ? { ...item, price_ton: priceTon } : item,
				),
			},
		}));
	},
}));

export default useChannelStore;
