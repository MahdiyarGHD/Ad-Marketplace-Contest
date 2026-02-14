import { create } from "zustand";
import type { Channel } from "./useChannelStore";
import type { Campaign } from "./useCampaignStore";

export type ApplicationType = {
	channel_id: string;
	channel?: Channel;
	campaign_id?: string;
	campaign?: Campaign;
	proposed_value?: number;
	proposed_ad_format: number;
	proposed_price_type: number;
	proposed_price_ton: number;
	proposed_posting_time: string;
	message: string;
};

type ApplicationState = {
	application?: Partial<ApplicationType>;
	setApplication: (application: Partial<ApplicationType>) => void;
	clearApplication: () => void;
};

const useApplicationStore = create<ApplicationState>((set) => ({
	setApplication(application: Partial<ApplicationType>) {
		set((state) => ({
			application: {
				...state.application,
				...application,
			},
		}));
	},
	clearApplication() {
		set({ application: undefined });
	},
}));

export default useApplicationStore;
