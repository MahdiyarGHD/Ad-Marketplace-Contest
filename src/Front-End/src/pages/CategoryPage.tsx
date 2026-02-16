import { memo, useEffect, useState } from "react";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import { renderCampaign, renderChannel } from "./Home";
import Transition from "../components/Transition";
import type { Channel } from "../stores/useChannelStore";
import { requestAPI } from "../utils/api";
import { useNavigate, useParams, useSearchParams } from "react-router-dom";
import { backButton } from "@tma.js/sdk-react";
import useChannelStore from "../stores/useChannelStore";
import useCategoryStore from "../stores/useCategoryStore";
import type { Campaign } from "../stores/useCampaignStore";
import useCampaignStore from "../stores/useCampaignStore";
import { useShallow } from "zustand/shallow";
import { invokeHapticFeedbackImpact } from "../utils/common";

function CategoryPage() {
	const [list, setList] = useState<Channel[] | Campaign[]>([]);

	const setActiveChannel = useChannelStore(
		useShallow((state) => state.setActiveChannel),
	);
	const setActiveCampaign = useCampaignStore(
		useShallow((state) => state.setActiveCampaign),
	);

	const { getCategory } = useCategoryStore();

	const { categoryId } = useParams();
	const [searchParams] = useSearchParams();

	const type = searchParams.get("type") || "channel";

	const category = getCategory(categoryId ?? "");

	const navigate = useNavigate();

	const getChannels = async () => {
		const response = await requestAPI(
			`/api/channels/by-category/${categoryId}`,
			{},
			"GET",
		);

		if (response.value?.channels) {
			setList(response.value.channels);
		}
	};

	const getCampaigns = async () => {
		const response = await requestAPI(
			`/api/campaigns/search?CategoryId=${categoryId}&Skip=0&Take=32`,
			{},
			"GET",
		);

		if (response.value?.campaigns) {
			setList(response.value.campaigns);
		}
	};

	const showChannelProfile = (channel: Channel) => {
		setActiveChannel(channel);

		navigate(`/channel/${channel.id}`);
	};

	const showCampaignPage = (campaign: Campaign) => {
		setActiveCampaign(campaign);

		navigate(`/campaign/${campaign.id}`);
	};

	const onBackButton = () => {
		window.history.back();
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
		if (!categoryId) return onBackButton();

		if (type === "channel") getChannels();
		else if (type === "campaign") getCampaigns();
	}, [categoryId, type]);

	return (
		<div className="CategoryPage">
			<PageHeader>
				<PageHeaderTitle>{category?.name}</PageHeaderTitle>
			</PageHeader>

			{list.length !== 0 ? (
				<div className="ChatList">
					<Transition
						state
						key={list?.length}
						eachElement
						eachElementDelay={40}
					>
						{list.map((item) =>
							type === "channel"
								? renderChannel(item as Channel, showChannelProfile)
								: renderCampaign(item as Campaign, showCampaignPage),
						)}
					</Transition>
				</div>
			) : (
				<LoadingSkeleton />
			)}
		</div>
	);
}

const LoadingSkeleton = memo(() => (
	<div className="Skeleton">
		<div className="Items">
			{Array.from({ length: 1 }).map(() => (
				<div className="ChatItem" key={Math.random()}>
					<div className="Avatar" />
					<div className="body">
						<div className="title Shimmer" />
						<div className="subtitle Shimmer" />
					</div>
				</div>
			))}
		</div>
	</div>
));

export default memo(CategoryPage);
