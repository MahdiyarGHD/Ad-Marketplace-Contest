import { memo, useEffect } from "react";
import PageHeader, {
	PageHeaderButtons,
	PageHeaderTitle,
} from "../../components/PageHeader";
import Avatar from "../../components/Avatar";
import { useNavigate, useParams } from "react-router-dom";
import { requestAPI } from "../../utils/api";
import useChannelStore, {
	AdFormats,
	PriceTypes,
} from "../../stores/useChannelStore";
import { useShallow } from "zustand/shallow";
import { backButton } from "@tma.js/sdk-react";
import { buildClassName, invokeHapticFeedbackImpact } from "../../utils/common";
import "../Statistics.scss";
import { Shimmer } from "../../components/Shimmer";
import Transition from "../../components/Transition";

function ChannelProfile() {
	const { id } = useParams();

	const {
		chat_id,
		title,
		username,
		category_id,
		subscriber_count,
		average_views,
		pricings,
	} = useChannelStore(useShallow((state) => state.activeChannel)) || {};
	const { setActiveChannel } = useChannelStore();

	const navigate = useNavigate();

	const getChannelInfo = async () => {
		const response = await requestAPI(`/api/channels/${id}`, {}, "GET");

		setActiveChannel(response.value);
		console.log(response);
	};

	const onBackButton = () => {
		navigate("/");
	};

	useEffect(() => {
		backButton.show();

		backButton.onClick(onBackButton);

		invokeHapticFeedbackImpact("medium");

		return () => {
			backButton.hide();

			backButton.offClick(onBackButton);
		};
	}, []);

	useEffect(() => {
		if (!id) return;

		getChannelInfo();
	}, [id]);

	return (
		<div className="ChannelProfile Profile">
			<PageHeader>
				<PageHeaderTitle>Profile</PageHeaderTitle>
				<PageHeaderButtons />
			</PageHeader>

			<div className="Chat">
				<Avatar id={chat_id!} title={title ?? "C"} photo="" size={80} />
				<div className="info">
					<div className={buildClassName("title", !title && "Shimmer")}>
						{title}
					</div>
					<div className="subtitle">
						{username ? `@${username}` : "private channel"}
					</div>
				</div>
			</div>

			<div className="Statistics">
				<div className="Item">
					<Shimmer className="title">
						<span>{subscriber_count}</span>
					</Shimmer>
					<div className="subtitle">Subscribers</div>
				</div>
				<div className="Item">
					<Shimmer className="title" state={!!average_views}>
						<span>{average_views}</span>
					</Shimmer>
					<div className="subtitle">Average Views</div>
				</div>
			</div>
			<div className="Section">
				<div className="title">Pricing</div>
				<div
					className={buildClassName(
						"StatItems",
						!pricings?.length && "loading",
					)}
				>
					<Transition
						state
						key={pricings?.length}
						eachElement
						eachElementDelay={100}
					>
						{(pricings ?? Array.from({ length: 1 })).map((price, index) => (
							// biome-ignore lint/suspicious/noArrayIndexKey: <explanation>
							<div className="Item" key={index}>
								<div className="flex-1">
									{/* <DollarSign /> */}
									<Shimmer className="title" state={!!price?.price_ton}>
										{price?.price_ton} TON
									</Shimmer>
									<div className="subtitle">Price</div>
								</div>
								<div>
									<Shimmer className="title" state={!!price?.price_type}>
										{PriceTypes[price?.price_type - 1]}
									</Shimmer>
									<div className="subtitle">Price type</div>
								</div>
								<div>
									<Shimmer className="title" state={!!price?.ad_format}>
										{AdFormats[price?.ad_format - 1]}
									</Shimmer>
									<div className="subtitle">Ad format</div>
								</div>
							</div>
						))}
					</Transition>
				</div>
			</div>
		</div>
	);
}

export default memo(ChannelProfile);
