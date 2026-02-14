import { backButton } from "@tma.js/sdk-react";
import { memo, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { invokeHapticFeedbackImpact } from "../utils/common";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import Transition from "../components/Transition";
import useApplicationStore, {
	type ApplicationType,
} from "../stores/useApplicationStore";
import { useShallow } from "zustand/shallow";
import RLottie from "../components/RLottie";
import { CheckIcon, EllipsisIcon, XIcon } from "lucide-react";
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

function Applications({ type }: { type: "campaign" | "channel" }) {
	const [application, setApplication] = useState<ApplicationType | null>(null);
	const [showApplication, setShowApplication] = useState<boolean>(false);

	const applications = useApplicationStore((state) => state.applications);

	const { getChannelApplications } = useApplicationStore(
		useShallow((state) => ({
			getChannelApplications: state.getChannelApplications,
		})),
	);

	const { id } = useParams();

	const navigate = useNavigate();

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

		if (type && id) {
			switch (type) {
				case "campaign":
					break;
				case "channel":
					getChannelApplications(id);
					break;
				default:
					break;
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
					<div className="Avatar peer-color-6">
						<XIcon />
					</div>
				);
			default:
				return null;
		}
	};

	return (
		<div className="Applications scrollable">
			<PageHeader>
				<PageHeaderTitle>Applications</PageHeaderTitle>
			</PageHeader>

			<Transition state eachElement eachElementDelay={20}>
				{applications?.map((application) => (
					<div className="Items" key={application.id}>
						<div className="ChatItem">
							{renderStatus(application.status ?? 0)}
							<div className="body">
								<div className="title">{application.advertiser_name}</div>
								<div className="subtitle">{application.message}</div>
							</div>
							<div className="meta">
								{ApplicationStatus[application.status!]}
							</div>
						</div>
						<div className="Item">
							<div className="body">
								<div className="title">Ad Format</div>
							</div>
							<div className="meta">
								{AdFormats[application.proposed_ad_format as AdFormat]}
							</div>
						</div>
						<div className="Item">
							<div className="body">
								<div className="title">Post Type</div>
							</div>
							<div className="meta">
								{PriceTypes[application.proposed_price_type as PriceType]}
							</div>
						</div>
						<div className="Item">
							<div className="body">
								<div className="title">Price</div>
							</div>
							<div className="meta">
								{application.proposed_price_ton
									? `${application.proposed_price_ton.toFixed(2)} TON`
									: "N/A"}
							</div>
						</div>
						<MainButton
							text="Check Application"
							onClick={() => {
								setApplication(application);
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
						onClose={() => setShowApplication(false)}
					/>
				)}
			</Modal>
			{applications && applications.length === 0 && (
				<div className="Placeholder">
					<div className="Emoji">
						<RLottie sticker="pepe" autoplay width={120} height={120} />
					</div>
					<h2 className="Title">No Applications</h2>
					<p className="Subtitle">
						There are no applications for this {type} yet.
					</p>
				</div>
			)}
		</div>
	);
}

export default memo(Applications);
