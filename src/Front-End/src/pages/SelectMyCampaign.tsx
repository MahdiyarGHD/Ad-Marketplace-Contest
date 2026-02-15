import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import Transition from "../components/Transition";
import { memo, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { invokeHapticFeedbackImpact } from "../utils/common";
import { backButton } from "@tma.js/sdk-react";
import { renderCampaign } from "./Home";
import { useShallow } from "zustand/shallow";
import useApplicationStore from "../stores/useApplicationStore";
import useCampaignStore, { type Campaign } from "../stores/useCampaignStore";

function SelectChannel() {
	const myCampaigns = useCampaignStore(
		useShallow((state) => state.myCampaigns),
	);
	const getMyCampaigns = useCampaignStore(
		useShallow((state) => state.getMyCampaigns),
	);

	const applicationCampaign = useApplicationStore(
		useShallow((state) => state.application?.channel_id),
	);

	const setApplication = useApplicationStore(
		useShallow((state) => state.setApplication),
	);

	const { set } = useParams();

	const navigate = useNavigate();

	const onBackButton = () => {
		window.history.back();
	};

	const onSelectCampaign = (campaign: Campaign) => {
		switch (set) {
			case "application":
				setApplication({
					campaign_id: campaign.id,
					campaign,
				});
				navigate(`/channel/${applicationCampaign}/invite`, {
					replace: true,
				});
				break;
			default:
				navigate("/my-campaigns");
				break;
		}
	};

	useEffect(() => {
		backButton.show();

		backButton.onClick(onBackButton);

		invokeHapticFeedbackImpact("medium");

		getMyCampaigns();

		return () => {
			backButton.hide();

			backButton.offClick(onBackButton);
		};
	}, []);

	return (
		<div className="SelectChannel scrollable">
			<PageHeader>
				<PageHeaderTitle>Select Campaign</PageHeaderTitle>
			</PageHeader>

			<div className="ChannelList Items">
				<Transition state eachElement eachElementDelay={20}>
					{myCampaigns.map((campaign) =>
						renderCampaign(campaign, onSelectCampaign),
					)}
				</Transition>
			</div>
			{myCampaigns.length === 0 && (
				<div className="NothingPlaceholder">
					No campaigns available.
					<br /> Please add a campaign first.
				</div>
			)}
		</div>
	);
}

export default memo(SelectChannel);
