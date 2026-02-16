import { memo, useState } from "react";
import type { ApplicationType } from "../stores/useApplicationStore";
import Avatar from "./Avatar";
import { Shimmer } from "./Shimmer";
import {
	AdFormats,
	PriceTypes,
	type AdFormat,
	type PriceType,
} from "../stores/useChannelStore";
import { requestAPI } from "../utils/api";
import useUIStore from "../stores/useUIStore";
import { ApplicationStatus } from "../pages/Applications";
import { buildClassName } from "../utils/common";
import Modal from "./Modal";
import Application from "./Application";
import useApplicationStore from "../stores/useApplicationStore";
import { useShallow } from "zustand/shallow";

function ApplicationReview({
	application,
	type,
	invite = false,
	onClose,
}: {
	application: ApplicationType;
	type: "channel" | "campaign" | "advertiser";
	invite?: boolean;
	onClose: () => void;
}) {
	const [showCounterOffer, setShowCounterOffer] = useState<boolean>(false);

	const setApplication = useApplicationStore(
		useShallow((state) => state.setApplication),
	);

	const clearApplication = useApplicationStore(
		useShallow((state) => state.clearApplication),
	);

	const { showToast } = useUIStore.getState();

	const endpoint = `/api/${invite ? "invitations" : (type === "channel" || type === "advertiser") ? "channel-applications" : "applications"}/${application.id}`;

	const showActions = () => {
		if (type === "advertiser" && application.status !== 4) return false;

		if (type === "channel" && application.status === 4) return false;

		if (application.status === 1 || application.status === 2) return false;

		if (invite && type === "campaign") return false;

		return true;
	};

	const onAccept = async () => {
		const response = await requestAPI(`${endpoint}/accept`);

		if (!response.isError && response.value) {
			showToast({ title: "Application accepted" });
			setApplication({ status: 1 });
			onClose();
		} else {
			showToast({ title: response.first_error?.description });
		}
	};

	const onReject = async () => {
		const response = await requestAPI(`${endpoint}/reject`);

		if (!response.isError && response.value) {
			showToast({ title: "Application rejected" });
			setApplication({ status: 2 });
			onClose();
		} else {
			showToast({ title: response.first_error?.description });
		}
	};

	const onAcceptCounterOffer = async () => {
		const response = await requestAPI(`${endpoint}/accept-counter-offer`);

		if (!response.isError && response.value) {
			showToast({ title: "Counter offer accepted" });
			setApplication({ status: 1 });
			onClose();
		} else {
			showToast({ title: response.first_error?.description });
		}
	};

	const onCounterOffer = () => {
		if (!application.id) return;

		setApplication({
			id: application.id,
			channel_id: application.channel_id ?? application.channel?.id,
			channel: application.channel,
			campaign_id: application.campaign_id,
			proposed_price_ton: application.proposed_price_ton,
			proposed_ad_format: application.proposed_ad_format,
			proposed_price_type: application.proposed_price_type,
		});

		setShowCounterOffer(true);
	};

	const onCounterOfferClose = () => {
		setShowCounterOffer(false);

		clearApplication();
	};

	return (
		<div className="ApplicationReview Profile">
			<div className="User">
				<Avatar
					id={
						application?.advertiser_id ??
						application?.channel?.id ??
						application?.channel_id ??
						application.campaign_id ??
						""
					}
					size={80}
					title={
						application?.advertiser_name ??
						application?.channel?.title ??
						application?.channel_title ??
						application?.campaign_title ??
						""
					}
					isCampaign={!!application?.campaign_title}
					isUuid
				/>
				<div className="info">
					<Shimmer className="title">
						{application.advertiser_name ??
							application.channel?.title ??
							application.channel_title ??
							application.campaign_title}
					</Shimmer>
					<div
						className={buildClassName(
							"subtitle",
							application.status === 1 && "success",
							application.status === 2 && "destructive",
						)}
					>
						{ApplicationStatus[application.status ?? 0]}
					</div>
				</div>
			</div>

			<table
				className={buildClassName(
					"Details",
					application.counter_message && "counter-offer",
				)}
			>
				<tr>
					<td className="label">Ad Format</td>
					<td className="value">
						<span>{AdFormats[application.proposed_ad_format as AdFormat]}</span>{" "}
						{AdFormats[application.counter_ad_format as AdFormat]}
					</td>
				</tr>
				<tr>
					<td className="label">Price Type</td>
					<td className="value">
						<span>
							{PriceTypes[application.proposed_price_type as PriceType]}
						</span>{" "}
						{PriceTypes[application.counter_price_type as PriceType]}
					</td>
				</tr>
				<tr>
					<td className="label">Price</td>
					<td className="value">
						<span>{application.proposed_price_ton} TON</span>{" "}
						{application.counter_price_ton && (
							<span>{application.counter_price_ton} TON</span>
						)}
					</td>
				</tr>
				<tr>
					<td className="label">Posting Time</td>
					<td className="value">
						<span>
							{new Date(application.proposed_posting_time).toLocaleString(
								"en-US",
								{
									dateStyle: "long",
									timeStyle: "short",
								},
							)}
						</span>{" "}
						{application.counter_posting_time && (
							<span>
								{new Date(application.counter_posting_time).toLocaleString(
									"en-US",
									{
										dateStyle: "long",
										timeStyle: "short",
									},
								)}
							</span>
						)}
					</td>
				</tr>
				<tr>
					<td className="label">Message</td>
					<td className="value">
						<span>{application.message}</span> {application.counter_message}
					</td>
				</tr>
			</table>
			{showActions() && application.status !== 4 && !invite && (
				<div className="TextButton primary" onClick={onCounterOffer}>
					<span>Counter Offer</span>
				</div>
			)}
			<div className="Actions">
				{showActions() ? (
					<>
						<div className="Button secondary" onClick={onReject}>
							Reject
						</div>
						<div
							className="Button"
							onClick={
								application.status === 4 ? onAcceptCounterOffer : onAccept
							}
						>
							Accept
						</div>
					</>
				) : (
					<div className="Button" onClick={onClose}>
						OK
					</div>
				)}
			</div>
			<Modal
				open={showCounterOffer}
				onClose={onCounterOfferClose}
				title="Application"
			>
				{type !== "advertiser" && (
					<Application type={type} counterOffer onClose={onCounterOfferClose} />
				)}
			</Modal>
		</div>
	);
}

export default memo(ApplicationReview);
