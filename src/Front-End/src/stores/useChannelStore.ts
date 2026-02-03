import { create } from "zustand";
import { requestAPI } from "../utils/api";

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
  setDraftChannel: (channel: Channel) => void;
  addDraftChannelPrice: () => void;
  setDraftChannelCategory: (category: any) => void;
  setDraftChannelPriceType: (priceType: number, index: number) => void;
  setDraftChannelAdFormat: (adFormat: number, index: number) => void;
};

const useChannelStore = create<ChannelState>((set) => ({
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
  addDraftChannelPrice() {
    set((state) => ({
      draftChannel: {
        ...state.draftChannel,
        pricing: [
          ...(state.draftChannel.pricing ?? []),
          { ad_format: 0, price_type: 0, price_ton: 0 },
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
        adFormat,
      },
    }));
  },
}));

export default useChannelStore;
