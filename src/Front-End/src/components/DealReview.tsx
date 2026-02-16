import { memo, useEffect, useState } from "react";
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
import { buildClassName, invokeHapticFeedbackImpact } from "../utils/common";
import { useShallow } from "zustand/shallow";
import type { DealType } from "../stores/useDealStore";
import useDealStore from "../stores/useDealStore";
import useAppStore from "../stores/useAppStore";
import { ChevronRight, MoreHorizontalIcon } from "lucide-react";
import "./Deals.scss";
import { DealStatus } from "../pages/Deals";
import Modal from "./Modal";
import Feedback from "./Feedback";
import { openTelegramLink } from "@tma.js/sdk-react";

export const DealStatusType = {
	AwaitingPayment: 0,
	EscrowFunded: 1,
	DraftSubmitted: 2,
	DraftRejected: 3,
	DraftApproved: 4,
	Scheduled: 5,
	Posted: 6,
	Verifying: 7,
	Completed: 8,
	Refunded: 9,
	Cancelled: 10,
	Disputed: 11,
};

function DealReview({ onClose }: { onClose: () => void }) {
	const [showFeedback, setShowFeedback] = useState(false);

	const deal = useDealStore(useShallow((state) => state.deal)) as DealType;
	const setDeal = useDealStore(useShallow((state) => state.setDeal));
	const updateDeal = useDealStore(useShallow((state) => state.updateDeal));

	const { showToast } = useUIStore.getState();

	const isAdvertiser = deal.advertiser_id === useAppStore.getState().userId;

	const endpoint = `/api/deals/${deal.id}`;

	const getDealInfo = async () => {
		const response = await requestAPI(`/api/deals/${deal.id}`, {}, "GET");

		if (!response.isError && response.value) {
			setDeal(response.value as DealType);

			invokeHapticFeedbackImpact("light");
		}
	};

	const getStatus = () => {
		if (deal.status === DealStatusType.AwaitingPayment)
			return isAdvertiser
				? "Awaiting Payment"
				: `Waiting for ${deal.campaign_title ?? deal.advertiser_first_name} to fund`;
		if (deal.status === DealStatusType.EscrowFunded)
			return isAdvertiser
				? `Waiting for ${deal.channel_title} to submit ad draft`
				: "Waiting for ad draft submission";
		if (deal.status === DealStatusType.DraftSubmitted)
			return isAdvertiser
				? `Ad draft submitted, waiting for your review`
				: `Waiting for ${deal.channel_title} to review ad draft`;
		if (deal.status === DealStatusType.DraftRejected)
			return isAdvertiser
				? "Ad draft rejected, waiting for review"
				: "Ad draft rejected, waiting for resubmission";
		if (deal.status === DealStatusType.Verifying) return "Ad is live";

		return DealStatus[deal.status];
	};

	const onPayment = async () => {
		const response = await requestAPI(`${endpoint}/fund`);

		if (!response.isError && response.value) {
			showToast({ title: "Deal funded" });
			updateDeal({ id: deal.id, status: DealStatusType.EscrowFunded });
			onClose();
		} else {
			showToast({ title: response.first_error?.description });
		}
	};

	const onApprove = async () => {
		const response = await requestAPI(`${endpoint}/approve`);

		if (!response.isError && response.value) {
			showToast({ title: "Deal approved" });
			updateDeal({ id: deal.id, status: DealStatusType.DraftApproved });
			onClose();
		} else {
			showToast({ title: response.first_error?.description });
		}
	};

	const onRejectDraft = async (feedback: string) => {
		const response = await requestAPI(`${endpoint}/reject`, {
			feedback,
		});

		if (!response.isError && response.value) {
			showToast({ title: "Your feedback has been submitted" });
			updateDeal({ id: deal.id, status: DealStatusType.DraftRejected });
			onClose();
		} else {
			showToast({ title: response.first_error?.description });
		}
	};

	const onReject = async () => {
		const response = await requestAPI(`${endpoint}/cancel`);

		if (!response.isError && response.value) {
			showToast({ title: "Deal rejected" });
			updateDeal({ id: deal.id, status: DealStatusType.Cancelled });
			onClose();
		} else {
			showToast({ title: response.first_error?.description });
		}
	};

	const showDraftMessage = () => {
		openTelegramLink(`https://t.me/${import.meta.env.VITE_BOT_USERNAME}`);
	};

	useEffect(() => {
		if (!deal.id) return;

		getDealInfo();
	}, [deal.id]);

	const renderActions = () => {
		if (deal.status === DealStatusType.AwaitingPayment) {
			if (isAdvertiser) {
				return (
					<div className="Actions">
						<div className="Button" onClick={onPayment}>
							Pay
						</div>
					</div>
				);
			}
		}

		if (
			deal.status === DealStatusType.EscrowFunded ||
			deal.status === DealStatusType.DraftRejected
		) {
			if (!isAdvertiser) {
				return (
					<div className="Actions">
						<div
							className="Button"
							onClick={() =>
								openTelegramLink(
									`https://t.me/${import.meta.env.VITE_BOT_USERNAME}`,
								)
							}
						>
							Open Bot
						</div>
					</div>
				);
			}
		}

		if (deal.status === DealStatusType.DraftSubmitted) {
			if (isAdvertiser) {
				return (
					<>
						<div
							className="TextButton destructive"
							onClick={() => setShowFeedback(true)}
						>
							Reject Draft
						</div>
						<div className="Actions">
							<div className="Button secondary" onClick={onReject}>
								Reject
							</div>
							<div className="Button" onClick={onApprove}>
								Approve
							</div>
						</div>
					</>
				);
			}
		}

		return (
			<div className="Actions">
				<div className="Button" onClick={onClose}>
					OK
				</div>
			</div>
		);
	};

	return (
		<div className="DealReview Profile">
			<div className="User">
				<div className="DealAvatars">
					<Avatar
						id={deal.channel_id ?? ""}
						title={deal.channel_title ?? "Channel"}
						size={64}
						isUuid
					/>
					<div className="icon">
						<MoreHorizontalIcon />
					</div>
					<Avatar
						id={deal.campaign_id ?? deal.advertiser_id}
						title={deal.advertiser_first_name}
						size={64}
						isCampaign={!!deal.campaign_id}
						isUuid={!deal.campaign_id}
					/>
				</div>
				<div className="info">
					<Shimmer className="title">
						{isAdvertiser
							? deal.channel_title
							: (deal.campaign_title ?? deal.advertiser_first_name)}
					</Shimmer>
					<div
						className={buildClassName(
							"subtitle",
							// deal.status === 1 && "success",
							// deal.status === 2 && "destructive",
						)}
					>
						{getStatus()}
					</div>
				</div>
			</div>

			<table className="Details">
				<tr>
					<td className="label">Channel</td>
					<td className="value">{deal.channel_title}</td>
				</tr>
				{deal.campaign_id && (
					<tr>
						<td className="label">Campaign</td>
						<td className="value">{deal.campaign_title}</td>
					</tr>
				)}
				{deal.advertiser_first_name && (
					<tr>
						<td className="label">Advertiser</td>
						<td className="value">
							{deal.advertiser_first_name} {deal.advertiser_last_name}
						</td>
					</tr>
				)}
				<tr>
					<td className="label">Ad Format</td>
					<td className="value">
						<Shimmer state={!!deal.ad_format}>
							{AdFormats[deal.ad_format as AdFormat]}
						</Shimmer>
					</td>
				</tr>
				<tr>
					<td className="label">Price Type</td>
					<td className="value">
						<Shimmer state={!!deal.price_type}>
							{PriceTypes[deal.price_type as PriceType]}
						</Shimmer>
					</td>
				</tr>
				<tr>
					<td className="label">Price</td>
					<td className="value">{deal.amount_ton} TON</td>
				</tr>
				<tr>
					<td className="label">Scheduled Post Time</td>
					<td className="value">
						{new Date(deal.scheduled_post_time).toLocaleString("en-US", {
							dateStyle: "medium",
							timeStyle: "short",
						})}
					</td>
				</tr>
				{(deal.draft_message_id || deal.posted_message_id) && (
					<tr>
						<td className="label">
							{deal.posted_message_id ? "Posted" : "Drafted"} Message
						</td>
						<td className="value primary pointer" onClick={showDraftMessage}>
							<div className="flex">
								<div className="title">
									View {deal.posted_message_id ? "Posted" : "Drafted"} Message
								</div>
								<div className="icon">
									<ChevronRight />
								</div>
							</div>
						</td>
					</tr>
				)}
			</table>
			{renderActions()}
			{deal.status === DealStatusType.DraftSubmitted && (
				<Modal
					title="Draft Rejection"
					open={showFeedback}
					onClose={() => setShowFeedback(false)}
				>
					<Feedback onSubmit={onRejectDraft} />
				</Modal>
			)}
		</div>
	);
}

export default memo(DealReview);
