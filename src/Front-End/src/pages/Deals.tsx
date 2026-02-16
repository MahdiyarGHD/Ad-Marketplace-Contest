import { backButton } from "@tma.js/sdk-react";
import { memo, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { invokeHapticFeedbackImpact } from "../utils/common";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import Transition from "../components/Transition";
import { useShallow } from "zustand/shallow";
import RLottie from "../components/RLottie";
import {
	CheckIcon,
	Clock3Icon,
	EllipsisIcon,
	HandshakeIcon,
	XIcon,
} from "lucide-react";
import "./Applications.scss";
import MainButton from "../components/MainButton";
import Modal from "../components/Modal";
import useDealStore, { type DealType } from "../stores/useDealStore";
import DealReview, { DealStatusType } from "../components/DealReview";

export const DealStatus: { [key: number]: string } = {
	0: "Awaiting Payment",
	1: "Escrow Funded",
	2: "Draft Submitted",
	3: "Draft Rejected",
	4: "Draft Approved",
	5: "Scheduled",
	6: "Posted",
	7: "Verifying",
	8: "Completed",
	9: "Refunded",
	10: "Cancelled",
	11: "Disputed",
};

function Deals({ type = "my" }: { type?: "campaign" | "channel" | "my" }) {
	const [showDeal, setShowDeal] = useState<boolean>(false);

	const deals = useDealStore((state) => state.deals);
	const setDeal = useDealStore((state) => state.setDeal);
	const clearDeal = useDealStore((state) => state.clearDeal);

	const { getChannelDeals, getCampaignDeals, getMyDeals } = useDealStore(
		useShallow((state) => ({
			getChannelDeals: state.getChannelDeals,
			getCampaignDeals: state.getCampaignDeals,
			getMyDeals: state.getMyDeals,
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

	const handleDealById = async (id: string) => {
		if (!id) return;

		const deal = deals?.find((deal) => deal.id === id);

		if (!deal) return;

		clearDeal();
		setDeal(deal);
		setShowDeal(true);
	};

	useEffect(() => {
		backButton.show();

		backButton.onClick(onBackButton);

		invokeHapticFeedbackImpact("medium");

		if ((type && id) || type === "my") {
			switch (type) {
				case "campaign":
					getCampaignDeals(id!);
					break;
				case "channel":
					getChannelDeals(id!);
					break;
				case "my":
					getMyDeals();
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

	useEffect(() => {
		if (id && deals) {
			handleDealById(id);
		}
	}, [id, deals]);

	const renderStatus = (status: number) => {
		switch (status) {
			case DealStatusType.AwaitingPayment:
				return (
					<div className="Avatar peer-color-1">
						<EllipsisIcon />
					</div>
				);
			case DealStatusType.EscrowFunded:
				return (
					<div className="Avatar peer-color-2">
						<HandshakeIcon />
					</div>
				);
			case DealStatusType.DraftRejected:
				return (
					<div className="Avatar peer-color-0">
						<HandshakeIcon />
					</div>
				);
			case DealStatusType.DraftApproved | DealStatusType.Verifying:
				return (
					<div className="Avatar peer-color-3">
						<HandshakeIcon />
					</div>
				);
			case DealStatusType.Scheduled:
				return (
					<div className="Avatar peer-color-2">
						<Clock3Icon />
					</div>
				);
			case DealStatusType.Completed:
				return (
					<div className="Avatar peer-color-3">
						<CheckIcon />
					</div>
				);
			case DealStatusType.Cancelled:
				return (
					<div className="Avatar peer-color-0">
						<XIcon />
					</div>
				);
			default:
				return (
					<div className="Avatar peer-color-4">
						<HandshakeIcon />
					</div>
				);
		}
	};

	const renderActiveItem = (deal: DealType) => {
		switch (deal.status) {
			case DealStatusType.Scheduled:
				return (
					<div className="Item">
						<div className="body">
							<div className="title">Post Time</div>
						</div>
						<div className="meta">
							{new Date(deal.scheduled_post_time).toLocaleString("en-US", {
								month: "short",
								day: "numeric",
								hour: "numeric",
								minute: "numeric",
							})}
						</div>
					</div>
				);
			default:
				return (
					<div className="Item">
						<div className="body">
							<div className="title">Price</div>
						</div>
						<div className="meta">
							{deal.amount_ton ? `${deal.amount_ton.toFixed(2)} TON` : "N/A"}
						</div>
					</div>
				);
		}
	};

	return (
		<div className="Deals scrollable">
			<PageHeader>
				<PageHeaderTitle>Deals</PageHeaderTitle>
			</PageHeader>

			<Transition state eachElement eachElementDelay={20}>
				{deals?.map((deal) => (
					<div className="Items" key={deal.id}>
						<div className="ChatItem">
							{renderStatus(deal.status ?? 0)}
							<div className="body">
								<div className="title">
									{deal.channel_title ?? deal.campaign_title}
								</div>
								<div className="subtitle" dir="auto">
									{new Date(deal.created_at).toLocaleDateString("en-US", {
										month: "short",
										day: "numeric",
									})}
								</div>
							</div>
							<div className="meta">{DealStatus[deal.status ?? 0]}</div>
						</div>
						{renderActiveItem(deal)}
						<MainButton
							text="Check Deal"
							onClick={() => {
								clearDeal();
								setDeal(deal);
								setShowDeal(true);
							}}
						/>
					</div>
				))}
			</Transition>
			<Modal
				className="secondary-bg"
				fullscreen={true}
				open={showDeal}
				onClose={() => setShowDeal(false)}
			>
				<DealReview onClose={() => setShowDeal(false)} />
			</Modal>
			{deals && deals.length === 0 && (
				<div className="Placeholder">
					<div className="Emoji">
						<RLottie sticker="notfound" autoplay width={120} height={120} />
					</div>
					<h2 className="Title">No Deals</h2>
					<p className="Subtitle">There are no deals yet.</p>
				</div>
			)}
		</div>
	);
}

export default memo(Deals);
