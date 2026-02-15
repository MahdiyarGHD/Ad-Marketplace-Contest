import { backButton } from "@tma.js/sdk-react";
import { memo, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { invokeHapticFeedbackImpact } from "../utils/common";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import Transition from "../components/Transition";
import { useShallow } from "zustand/shallow";
import RLottie from "../components/RLottie";
import { CheckIcon, EllipsisIcon, XIcon } from "lucide-react";
import "./Applications.scss";
import MainButton from "../components/MainButton";
import Modal from "../components/Modal";
import type { DealType } from "../stores/useDealStore";
import useDealStore from "../stores/useDealStore";
import DealReview from "../components/DealReview";

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
			default:
				return null;
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
									{new Date(deal.created_at).toLocaleDateString()}
								</div>
							</div>
							<div className="meta">{DealStatus[deal.status ?? 0]}</div>
						</div>
						<div className="Item">
							<div className="body">
								<div className="title">Price</div>
							</div>
							<div className="meta">
								{deal.amount_ton ? `${deal.amount_ton.toFixed(2)} TON` : "N/A"}
							</div>
						</div>
						<MainButton
							text="Check Deal"
							onClick={() => {
								setDeal(deal);
								setShowDeal(true);
							}}
						/>
					</div>
				))}
			</Transition>
			<Modal
				className="secondary-bg"
				open={showDeal}
				onClose={() => setShowDeal(false)}
			>
				<DealReview onClose={() => setShowDeal(false)} />
			</Modal>
			{deals && deals.length === 0 && (
				<div className="Placeholder">
					<div className="Emoji">
						<RLottie sticker="pepe" autoplay width={120} height={120} />
					</div>
					<h2 className="Title">No Deals</h2>
					<p className="Subtitle">There are no deals yet.</p>
				</div>
			)}
		</div>
	);
}

export default memo(Deals);
