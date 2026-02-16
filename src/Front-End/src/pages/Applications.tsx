import { backButton } from "@tma.js/sdk-react";
import { memo, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { invokeHapticFeedbackImpact } from "../utils/common";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import Transition from "../components/Transition";
import useApplicationStore, {
	type ApplicationType,
} from "../stores/useApplicationStore";
import { useShallow } from "zustand/shallow";
import RLottie from "../components/RLottie";
import { CheckIcon, EllipsisIcon, TagsIcon, XIcon } from "lucide-react";
import "./Applications.scss";
import {
	AdFormats,
	PriceTypes,
	type AdFormat,
	type PriceType,
} from "../stores/useChannelStore";
import MainButton from "../components/MainButton";
import Modal from "../components/Modal";
import ApplicationReview from "../components/ApplicationReview";

export const ApplicationStatus: { [key: number]: string } = {
	0: "Pending",
	1: "Accepted",
	2: "Rejected",
	3: "Withdrawn",
	4: "Counter Offer",
};

function Applications({
	type,
	invite,
}: {
	type: "campaign" | "channel" | "advertiser";
	invite?: boolean;
}) {
	const [application, setApplication] = useState<ApplicationType | null>(null);
	const [showApplication, setShowApplication] = useState<boolean>(false);

	const applications = useApplicationStore((state) => state.applications);

	const {
		getChannelApplications,
		getCampaignApplications,
		getMyApplications,
		getChannelInvitations,
		getCampaignInvitations,
	} = useApplicationStore(
		useShallow((state) => ({
			getChannelApplications: state.getChannelApplications,
			getCampaignApplications: state.getCampaignApplications,
			getMyApplications: state.getMyApplications,
			getChannelInvitations: state.getChannelInvitations,
			getCampaignInvitations: state.getCampaignInvitations,
		})),
	);

	const { id } = useParams();

	const onBackButton = () => {
		window.history.back();
	};

	// const onSelectChannel = (channel: any) => {
	// 	setDraftChannel(channel);
	// 	navigate("/set-channel-data");
	// };

	useEffect(() => {
		backButton.show();

		backButton.onClick(onBackButton);

		invokeHapticFeedbackImpact("medium");

		if ((type && id) || type === "advertiser") {
			if (invite && id) {
				switch (type) {
					case "campaign":
						getCampaignInvitations(id!);
						break;
					case "channel":
						getChannelInvitations(id!);
						break;
					default:
						break;
				}
			} else {
				switch (type) {
					case "campaign":
						getCampaignApplications(id!);
						break;
					case "channel":
						getChannelApplications(id!);
						break;
					case "advertiser":
						getMyApplications();
						break;
					default:
						break;
				}
			}
		}

		return () => {
			backButton.hide();

			backButton.offClick(onBackButton);
		};
	}, []);

	const renderStatus = (status: number) => {
		switch (status) {
			case 0:
				return (
					<div className="Avatar peer-color-1">
						<EllipsisIcon />
					</div>
				);
			case 1:
				return (
					<div className="Avatar peer-color-3">
						<CheckIcon />
					</div>
				);
			case 2:
				return (
					<div className="Avatar peer-color-0">
						<XIcon />
					</div>
				);
			case 4:
				return (
					<div className="Avatar peer-color-5">
						<TagsIcon />
					</div>
				);
			default:
				return null;
		}
	};

	return (
		<div className="Applications scrollable">
			<PageHeader>
				<PageHeaderTitle>
					{invite ? "Invitations" : "Applications"}
				</PageHeaderTitle>
			</PageHeader>

			<Transition state eachElement eachElementDelay={20}>
				{applications?.map((application) => (
					<div className="Items" key={application.id}>
						<div className="ChatItem">
							{renderStatus(application.status ?? 0)}
							<div className="body">
								<div className="title">
									{application.advertiser_name ??
										application.channel?.title ??
										application.channel_title ??
										application.campaign_title}
								</div>
								<div className="subtitle" dir="auto">
									{application.message}
								</div>
							</div>
							<div className="meta">
								{ApplicationStatus[application.status ?? 0]}
							</div>
						</div>
						<div className="Item">
							<div className="body">
								<div className="title">Ad Format</div>
							</div>
							<div className="meta">
								{
									AdFormats[
										(application.counter_ad_format ??
											application.proposed_ad_format) as AdFormat
									]
								}
							</div>
						</div>
						<div className="Item">
							<div className="body">
								<div className="title">Post Type</div>
							</div>
							<div className="meta">
								{
									PriceTypes[
										(application.counter_price_type ??
											application.proposed_price_type) as PriceType
									]
								}
							</div>
						</div>
						<div className="Item">
							<div className="body">
								<div className="title">Price</div>
							</div>
							<div className="meta">
								{(application.counter_price_ton ??
								application.proposed_price_ton)
									? `${(application.counter_price_ton ?? application.proposed_price_ton)!.toFixed(2)} TON`
									: "N/A"}
							</div>
						</div>
						<MainButton
							text="Check Application"
							onClick={() => {
								setApplication({
									...application,
									channel_id: application.channel_id ?? id,
								});
								setShowApplication(true);
							}}
						/>
					</div>
				))}
			</Transition>
			<Modal
				className="secondary-bg"
				open={showApplication}
				onClose={() => setShowApplication(false)}
			>
				{application && (
					<ApplicationReview
						application={application}
						type={type}
						invite={invite}
						onClose={() => setShowApplication(false)}
					/>
				)}
			</Modal>
			{applications && applications.length === 0 && (
				<div className="Placeholder">
					<div className="Emoji">
						<RLottie sticker="notfound" autoplay width={120} height={120} />
					</div>
					<h2 className="Title">
						No {invite ? "Invitations" : "Applications"}
					</h2>
					<p className="Subtitle">
						There are no {invite ? "invitations" : "applications"} for this{" "}
						{type} yet.
					</p>
				</div>
			)}
		</div>
	);
}

export default memo(Applications);
