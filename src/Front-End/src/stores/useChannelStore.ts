import { create } from "zustand";
import { requestAPI } from "../utils/api";

type ChannelState = {
  myChannels: any[];
  getMyChannels: () => Promise<void>;
  verifyChannel: () => Promise<void>;
};

const useChannelStore = create<ChannelState>((set) => ({
  myChannels: [],
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
}));

export default useChannelStore;
