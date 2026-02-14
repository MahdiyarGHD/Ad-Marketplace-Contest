import { create } from "zustand";
import type { Channel } from "./useChannelStore";
import type { Campaign } from "./useCampaignStore";
import { requestAPI } from "../utils/api";

export type ApplicationType = {
	id?: string;
	advertiser_id?: string;
	advertiser_name?: string;
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
	status?: number;
};

type ApplicationState = {
	application?: Partial<ApplicationType>;
	applications?: ApplicationType[];
	setApplication: (application: Partial<ApplicationType>) => void;
	setApplications: (applications: ApplicationType[]) => void;
	getChannelApplications: (channelId: string) => Promise<void>;
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
	setApplications(applications: ApplicationType[]) {
		set({ applications });
	},
	async getChannelApplications(channelId: string) {
		set({ applications: undefined });

		const response = await requestAPI(
			`/api/channels/${channelId}/applications`,
			{},
			"GET",
		);
		if (!response.isError && response.value?.applications) {
			set({ applications: response.value.applications as ApplicationType[] });
		}
	},
}));

export default useApplicationStore;
