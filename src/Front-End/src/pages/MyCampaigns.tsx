import { memo, useEffect } from "react";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import Avatar from "../components/Avatar";
import { PlusIcon } from "lucide-react";
import "./MyChannels.scss";
import { useNavigate } from "react-router-dom";
import Transition from "../components/Transition";
import { backButton } from "@tma.js/sdk-react";
import { invokeHapticFeedbackImpact } from "../utils/common";
import useCampaignStore, { type Campaign } from "../stores/useCampaignStore";

const CampaignStatus: { [key: number]: string } = {
	0: "Draft",
	1: "Active",
	2: "Paused",
	3: "Completed",
	4: "Canceled",
};

function MyCampaigns() {
	const { myCampaigns, getMyCampaigns, setActiveCampaign } = useCampaignStore();

	const navigate = useNavigate();

	const onBackButton = () => {
		navigate("/profile");
	};

	const onCampaignClick = (campaign: Campaign) => {
		setActiveCampaign(campaign);

		navigate(`/campaign/${campaign.id}`);
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
		<div className="MyCampaigns scrollable">
			<PageHeader>
				<PageHeaderTitle>My Campaigns</PageHeaderTitle>
			</PageHeader>

			<div className="ChatList">
				<Transition state eachElement eachElementDelay={20}>
					<div
						className="Item primary"
						onClick={() => navigate("/add-campaign")}
					>
						<div className="icon">
							<PlusIcon />
						</div>
						<div className="title">Add Your Campaign</div>
					</div>
					{myCampaigns.map((campaign) => (
						<div
							key={campaign.id}
							className="ChatItem"
							onClick={() => onCampaignClick(campaign)}
						>
							<Avatar id={campaign.id!} isCampaign />
							<div className="body">
								<div className="title" dir="auto">
									{campaign.title}
								</div>
								<div className="subtitle" dir="auto">
									{campaign.description}
								</div>
							</div>
							<div className="meta">{CampaignStatus[campaign.status]}</div>
						</div>
					))}
				</Transition>
			</div>

			<div className="NoCampaign"></div>
		</div>
	);
}

export default memo(MyCampaigns);
