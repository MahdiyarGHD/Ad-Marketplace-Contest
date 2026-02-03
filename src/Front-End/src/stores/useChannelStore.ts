import { create } from "zustand";
import { requestAPI } from "../utils/api";

type ChannelState = {
	myChannels: any[];
	draftChannel: any;
	getMyChannels: () => Promise<void>;
	verifyChannel: () => Promise<void>;
	setDraftChannel: (channel: any) => void;
	setDraftChannelCategory: (category: any) => void;
	setDraftChannelPriceType: (priceType: number) => void;
	setDraftChannelAdFormat: (adFormat: number) => void;
};

const useChannelStore = create<ChannelState>((set) => ({
	myChannels: [],
	draftChannel: null,
	async getMyChannels() {
		const response = await requestAPI("/api/channels/my", {}, "GET");

		set({ myChannels: response.value });
	},
	async verifyChannel() {
		const response = await requestAPI("/api/channels/verify-add", {
			look_back_seconds: 900,
		});

		console.log(response);

		set((state) => ({
			myChannels: [...state.myChannels, ...response.value],
		}));
	},
	setDraftChannel(channel) {
		set({ draftChannel: channel });
	},
	setDraftChannelCategory(category: any) {
		set((state) => ({
			draftChannel: {
				...state.draftChannel,
				category,
			},
		}));
	},
	setDraftChannelPriceType(priceType: number) {
		set((state) => ({
			draftChannel: {
				...state.draftChannel,
				priceType,
			},
		}));
	},
	setDraftChannelAdFormat(adFormat: number) {
		set((state) => ({
			draftChannel: {
				...state.draftChannel,
				adFormat,
			},
		}));
	},
}));

export default useChannelStore;
