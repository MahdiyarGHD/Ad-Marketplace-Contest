import { memo, useEffect } from "react";
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
import { MoreHorizontalIcon } from "lucide-react";
import "./Deals.scss";
import { DealStatus } from "../pages/Deals";

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
	const deal = useDealStore(useShallow((state) => state.deal)) as DealType;
	const setDeal = useDealStore(useShallow((state) => state.setDeal));

	const { showToast } = useUIStore.getState();

	const isAdvertiser = deal.advertiser_id === useAppStore.getState().userId;

	const endpoint = `/api/deals/${deal.id}`;

	const getDealInfo = async () => {
		const response = await requestAPI(`/api/deals/${deal.id}`, {}, "GET");

		if (!response.isError && response.value) {
			console.log("va");
			setDeal(response.value as DealType);

			invokeHapticFeedbackImpact("light");
		}
	};

	const getStatus = () => {
		if (deal.status === 0)
			return isAdvertiser
				? "Awaiting Payment"
				: `Waiting for ${deal.campaign_title} to pay`;

		return DealStatus[deal.status];
	};

	// const onAccept = async () => {
	// 	const response = await requestAPI(`${endpoint}/accept`);

	// 	if (!response.isError && response.value) {
	// 		showToast({ title: "Deal accepted" });
	// 		setDeal({ status: 1 });
	// 		onClose();
	// 	} else {
	// 		showToast({ title: response.first_error?.description });
	// 	}
	// };

	// const onReject = async () => {
	// 	const response = await requestAPI(`${endpoint}/reject`);

	// 	if (!response.isError && response.value) {
	// 		showToast({ title: "Deal rejected" });
	// 		setDeal({ status: 2 });
	// 		onClose();
	// 	} else {
	// 		showToast({ title: response.first_error?.description });
	// 	}
	// };

	useEffect(() => {
		if (!deal.id) return;

		getDealInfo();
	}, [deal.id]);

	const renderActions = () => {
		if (deal.status === DealStatusType.AwaitingPayment) {
			if (isAdvertiser) {
				return (
					<div className="Button" onClick={onClose}>
						Payment
					</div>
				);
			}
		}

		return (
			<div className="Button" onClick={onClose}>
				OK
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
					<Avatar id={deal.campaign_id ?? ""} size={64} isCampaign />
				</div>
				<div className="info">
					<Shimmer className="title">
						{isAdvertiser ? deal.campaign_title : deal.channel_title}
					</Shimmer>
					<div
						className={buildClassName(
							"subtitle",
							deal.status === 1 && "success",
							deal.status === 2 && "destructive",
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
				<tr>
					<td className="label">Campaign</td>
					<td className="value">{deal.campaign_title}</td>
				</tr>
				<tr>
					<td className="label">Ad Format</td>
					<td className="value">{AdFormats[deal.ad_format as AdFormat]}</td>
				</tr>
				<tr>
					<td className="label">Price Type</td>
					<td className="value">{PriceTypes[deal.price_type as PriceType]}</td>
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
			</table>
			<div className="Actions">{renderActions()}</div>
		</div>
	);
}

export default memo(DealReview);
